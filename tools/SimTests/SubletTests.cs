using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Subletting compute: part of the general compute is let to outsiders for part of the rent value, and leaves training and serving.</summary>
public class SubletTests
{
    static Game Lab(Action<GameConfig> tweak = null, int rented = 10)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 2, gpus: rented);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static long PerGpu(Game g) { return LabQueries.CurrentGeneration(g.C, g.Tick).ComputePerGpuWeek; }

    static long Rent(Game g) { return LabQueries.CurrentGeneration(g.C, g.Tick).RentPerGpuWeek; }

    static List<GameEvent> Changed(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.SubletChanged).ToList(); }

    // ---- nothing happens by itself

    [Fact]
    public void Nothing_is_let_and_nothing_is_earned_until_the_player_sets_a_share()
    {
        Game g = Lab();
        g.Weeks(20);
        Assert.Equal(0, g.Me.SubletPermille);
        Assert.Equal(0, Sublet.Compute(g.C, g.State, g.Me));
        Assert.Equal(0, Sublet.WeeklyRevenue(g.C, g.State, g.Me));
        Assert.Equal(0, g.Me.Ledger.Lifetime.Sublet);
        Assert.Equal(0, LabQueries.Split(g.C, g.State, g.Me, false).Sublet);
        Assert.Empty(Changed(g));
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.SubletPermille));
    }

    // ---- the command

    [Fact]
    public void Setting_a_share_logs_it_and_locks_changes_for_the_notice_weeks()
    {
        Game g = Lab();
        g.Do(Command.SetSublet(g.Id, 300));
        Assert.Equal(300, g.Me.SubletPermille);
        Assert.Equal(g.Tick + 4, g.Me.SubletLockedUntil);
        GameEvent e = Changed(g).Single();
        Assert.Equal(300, e.SubjectId);
        Assert.Equal(Sublet.WeeklyRevenue(g.C, g.State, g.Me), e.Amount);
        Assert.Contains("30% of its compute", EventText.Describe(g.C, g.State, e));
        g.Refused(Command.SetSublet(g.Id, 100), CommandError.SubletLocked);
        g.Refused(Command.SetSublet(g.Id, 0), CommandError.SubletLocked);
        g.Weeks(4);
        g.Do(Command.SetSublet(g.Id, 100));
        Assert.Equal(100, g.Me.SubletPermille);
    }

    [Fact]
    public void A_share_over_the_most_or_below_zero_or_the_same_share_is_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetSublet(g.Id, 501), CommandError.InvalidArgument);
        g.Refused(Command.SetSublet(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetSublet(g.Id, 0), CommandError.InvalidArgument);
        Assert.Equal(0, g.Me.SubletLockedUntil);
        g.Do(Command.SetSublet(g.Id, 500));
        Assert.Equal(500, g.Me.SubletPermille);
        g.Weeks(4);
        g.Refused(Command.SetSublet(g.Id, 500), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_rival_a_bankrupt_lab_and_a_game_without_subletting_refuse_it()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.SetSublet(rival.Id, 100), CommandError.InvalidArgument);
        Assert.Equal(0, rival.SubletPermille);
        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.SetSublet(broke.Id, 100), CommandError.LabBankrupt);
        Game off = Lab(c => c.Sublet.MaxPermille = 0);
        off.Refused(Command.SetSublet(off.Id, 100), CommandError.InvalidArgument);
        Assert.False(Sublet.Enabled(off.C));
    }

    [Fact]
    public void With_no_notice_weeks_the_share_can_be_changed_at_once()
    {
        Game g = Lab(c => c.Sublet.NoticeWeeks = 0);
        g.Do(Command.SetSublet(g.Id, 100));
        g.Do(Command.SetSublet(g.Id, 200));
        Assert.Equal(200, g.Me.SubletPermille);
    }

    // ---- the compute

    [Fact]
    public void The_share_comes_out_of_the_general_compute_and_the_inference_compute_is_untouched()
    {
        Game g = Lab();
        ComputeSplit before = LabQueries.Split(g.C, g.State, g.Me, false);
        g.Do(Command.SetSublet(g.Id, 300));
        ComputeSplit after = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal(MathI.MulPermille(before.General, 700), after.General);
        Assert.Equal(before.General - after.General, after.Sublet);
        Assert.Equal(before.Inference, after.Inference);
        Assert.Equal(after.General + after.Inference, after.Total);
        Assert.Equal(10 * PerGpu(g) * 3 / 10, after.Sublet);
    }

    [Fact]
    public void A_research_share_and_a_sublet_share_take_their_cuts_one_after_the_other()
    {
        Game g = Lab();
        long all = LabQueries.Split(g.C, g.State, g.Me, false).General;
        g.Do(Command.SetResearchShare(g.Id, 200));
        g.Do(Command.SetSublet(g.Id, 500));
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        long afterResearch = MathI.MulPermille(all, 800);
        Assert.Equal(MathI.MulPermille(afterResearch, 500), split.General);
        Assert.Equal(afterResearch - split.General, split.Sublet);
    }

    [Fact]
    public void A_lab_with_no_compute_lets_nothing_and_earns_nothing()
    {
        Game g = Lab(rented: 0);
        g.Do(Command.SetSublet(g.Id, 500));
        g.Weeks(5);
        Assert.Equal(0, g.Me.Ledger.Lifetime.Sublet);
        Assert.Equal(0, Sublet.Compute(g.C, g.State, g.Me));
    }

    // ---- the money

    [Fact]
    public void The_sublet_earns_the_data_s_share_of_the_rent_of_the_same_gpus_on_its_own_ledger_line()
    {
        Game control = Lab();
        Game g = Lab();
        g.Do(Command.SetSublet(g.Id, 500));
        long expected = 5 * Rent(g) * 500 / 1000;
        Assert.Equal(expected, Sublet.WeeklyRevenue(g.C, g.State, g.Me));
        control.Weeks(10);
        g.Weeks(10);
        Assert.Equal(10 * expected, g.Me.Ledger.Lifetime.Sublet);
        Assert.Equal(expected, g.Me.Ledger.LastWeek.Sublet);
        Assert.Equal(control.Me.Cash + 10 * expected, g.Me.Cash);
        Assert.Equal(0, control.Me.Ledger.Lifetime.Sublet);
    }

    [Fact]
    public void Letting_out_rented_gpus_never_earns_back_their_rent()
    {
        Game g = Lab();
        g.Do(Command.SetSublet(g.Id, 500));
        Assert.True(Sublet.WeeklyRevenue(g.C, g.State, g.Me) < 5 * Rent(g));
        Game full = Lab(c => c.Sublet.MaxPermille = 900);
        full.Do(Command.SetSublet(full.Id, 900));
        Assert.True(Sublet.WeeklyRevenue(full.C, full.State, full.Me) < 9 * Rent(full));
    }

    [Fact]
    public void Owned_gpus_let_out_pay_for_themselves_a_little()
    {
        Game g = Lab(rented: 0);
        g.Build("server-closet", 2, 2);
        g.Do(Command.BuyGpus(g.Id, 4));
        g.Do(Command.SetSublet(g.Id, 500));
        Assert.Equal(2 * Rent(g) * 500 / 1000, Sublet.WeeklyRevenue(g.C, g.State, g.Me));
        long before = g.Me.Ledger.Lifetime.Sublet;
        g.Weeks(3);
        Assert.Equal(before + 3 * (2 * Rent(g) / 2), g.Me.Ledger.Lifetime.Sublet);
    }

    [Fact]
    public void A_lab_in_debt_still_earns_the_sublet()
    {
        Game g = Lab();
        g.Do(Command.SetSublet(g.Id, 500));
        g.Me.Cash = -1000;
        g.Weeks(1);
        Assert.True(g.Me.Ledger.LastWeek.Sublet > 0);
    }

    // ---- what it costs

    [Fact]
    public void Training_gets_less_compute_and_a_run_takes_longer()
    {
        Game g = Lab();
        ModelLineConfig line = g.C.Training.Lines.First();
        int plain = TrainingSystem.EstimateWeeks(g.C, g.State, g.Me, line, 0);
        g.Do(Command.SetSublet(g.Id, 500));
        int slower = TrainingSystem.EstimateWeeks(g.C, g.State, g.Me, line, 0);
        Assert.True(plain > 0);
        Assert.True(slower > plain);
    }

    [Fact]
    public void Customers_get_less_serving_compute_while_a_share_is_let()
    {
        Game control = Lab();
        Game g = Lab();
        g.Do(Command.SetSublet(g.Id, 500));
        control.Weeks(1);
        g.Weeks(1);
        Assert.Equal(control.Me.LastWeek.ServingCompute / 2, g.Me.LastWeek.ServingCompute);
        Assert.Equal(control.Me.LastWeek.Compute / 2, g.Me.LastWeek.Compute);
    }

    [Fact]
    public void Taking_the_share_back_gives_the_compute_back()
    {
        Game g = Lab();
        long all = LabQueries.Split(g.C, g.State, g.Me, false).General;
        g.Do(Command.SetSublet(g.Id, 400));
        g.Weeks(4);
        g.Do(Command.SetSublet(g.Id, 0));
        Assert.Equal(all, LabQueries.Split(g.C, g.State, g.Me, false).General);
        long earned = g.Me.Ledger.Lifetime.Sublet;
        g.Weeks(6);
        Assert.Equal(earned, g.Me.Ledger.Lifetime.Sublet);
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_lab_that_sublets_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetSublet(g.Id, 250));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(250, back.Player.SubletPermille);
        Assert.Equal(g.Me.Ledger.Lifetime.Sublet, back.Player.Ledger.Lifetime.Sublet);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2);
        g.Autopilot(60);
        Assert.True(g.Try(Command.SetSublet(g.Id, 200)).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_sublet_and_an_empty_ledger_line()
    {
        Game g = Lab();
        g.Do(Command.SetSublet(g.Id, 300));
        g.Weeks(3);
        string old = SaveSchemaPretend.At(g.C, 49, g.Sim.Save());
        Assert.DoesNotContain("subletPermille", old);
        Assert.DoesNotContain("\"sublet\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.SubletPermille);
        Assert.Equal(0, sim.Player.SubletLockedUntil);
        Assert.Equal(0, sim.Player.Ledger.Lifetime.Sublet);
        Assert.Equal(0, sim.Player.Ledger.ThisWeek.Sublet);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_share_the_data_does_not_allow_is_reported()
    {
        Game g = Lab();
        g.Me.SubletPermille = 501;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("outsiders"));
        g.Me.SubletPermille = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("outsiders"));
        g.Me.SubletPermille = 0;
        g.Me.SubletLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("outsiders"));
        g.Me.SubletLockedUntil = 0;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("outsiders"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(500, c.Sublet.MaxPermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Sublet.MaxPermille = 901;
        c.Sublet.RevenuePermille = 1001;
        c.Sublet.NoticeWeeks = 521;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxPermille", "revenuePermille", "noticeWeeks" })
            Assert.Contains(problems, p => p.Contains("sublet." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_sublet()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.SubletPermille));
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.Ledger.Lifetime.Sublet));
        Assert.Empty(Changed(g));
    }

    [Fact]
    public void An_autopilot_game_that_sublets_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        g.Autopilot(48);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.SetSublet(g.Id, 200));
        for (int i = 0; i < 150; i++) g.Autopilot(1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.True(g.Me.Ledger.Lifetime.Sublet > 0);
        Assert.Equal(200, g.Me.SubletPermille);
    }
}
