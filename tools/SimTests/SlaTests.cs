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

/// <summary>The service-level pledge: a premium on revenue while the promise holds, a refund, reputation loss and a strike in a week it breaks.</summary>
public class SlaTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Safety.IncidentChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Reputation = 600;
        return g;
    }

    static void Week(Game g, long revenue, int servedPermille)
    {
        g.Me.LastWeek.Revenue = revenue;
        g.Me.LastWeek.ServingDemand = 100;
        g.Me.LastWeek.ServedPermille = servedPermille;
        Sla.RunWeek(g.C, g.State, g.Me);
    }

    static List<GameEvent> Of(Game g, EventKind k) { return g.State.Events.Where(e => e.Kind == k).ToList(); }

    // ---- the data

    [Fact]
    public void The_pledge_ships_and_the_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Sla.MaxLevel);
        Assert.Equal(40, c.Sla.PremiumPermillePerLevel);
        Assert.Equal(120, c.Sla.RefundPermillePerLevel);
        Assert.Equal(3, c.Sla.StrikeWeeks);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Sla.PremiumPermillePerLevel = 200;
        c.Sla.RefundPermillePerLevel = 100;
        c.Sla.BreachBelowPermille = 0;
        c.Sla.BreachReputationPerLevel = 100;
        c.Sla.MinReputation = 1001;
        c.Sla.LockWeeks = -1;
        c.Sla.StrikeWeeks = 53;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "premiumPermillePerLevel", "refundPermillePerLevel", "breachBelowPermille", "breachReputationPerLevel", "minReputation", "lockWeeks", "strikeWeeks" })
            Assert.Contains(problems, p => p.Contains("sla." + key));
    }

    [Fact]
    public void A_refund_that_is_smaller_than_the_premium_is_caught()
    {
        GameConfig c = TestData.Config();
        c.Sla.RefundPermillePerLevel = c.Sla.PremiumPermillePerLevel - 1;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("sla.refundPermillePerLevel"));
    }

    [Fact]
    public void Nothing_changes_until_the_player_pledges()
    {
        Game g = Lab();
        Assert.Equal(0, Sla.Level(g.C, g.Me));
        long cash = g.Me.Cash;
        Week(g, 1_000_000, 100);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(600, g.Me.Reputation);
        Assert.Empty(Of(g, EventKind.SlaBroken));
        Assert.Equal(0, Sla.PremiumPermille(g.C, g.Me));
    }

    // ---- the command

    [Fact]
    public void Pledging_sets_the_level_locks_it_and_logs_it()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 2));
        Assert.Equal(2, g.Me.SlaLevel);
        Assert.Equal(g.State.Tick + 8, g.Me.SlaLockedUntil);
        Assert.Equal(80, Sla.PremiumPermille(g.C, g.Me));
        Assert.Equal(240, Sla.RefundPermille(g.C, g.Me));
        Assert.Equal(16, Sla.BreachReputation(g.C, g.Me));
        GameEvent e = Assert.Single(Of(g, EventKind.SlaPledged));
        Assert.Equal(1, e.Pick);
        Assert.Equal(2, e.SubjectId);
        Assert.Contains("level 2", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_change_is_refused_while_locked_and_allowed_when_the_lock_ends()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 1));
        string before = g.Sim.Digest();
        g.Refused(Command.SetSla(g.Id, 2), CommandError.SlaLocked);
        g.Refused(Command.SetSla(g.Id, 0), CommandError.SlaLocked);
        Assert.Equal(before, g.Sim.Digest());
        g.Weeks(8);
        g.Do(Command.SetSla(g.Id, 0));
        Assert.Equal(0, g.Me.SlaLevel);
        Assert.Equal(0, Of(g, EventKind.SlaPledged).Last().Pick);
    }

    [Fact]
    public void Out_of_range_and_unchanged_levels_are_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetSla(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetSla(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetSla(g.Id, 0), CommandError.InvalidArgument);
        Assert.Empty(Of(g, EventKind.SlaPledged));
    }

    [Fact]
    public void A_pledge_needs_the_reputation()
    {
        Game g = Lab();
        g.Me.Reputation = 299;
        string before = g.Sim.Digest();
        g.Refused(Command.SetSla(g.Id, 1), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        g.Me.Reputation = 300;
        g.Do(Command.SetSla(g.Id, 1));
    }

    [Fact]
    public void Dropping_the_pledge_needs_no_reputation()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 2));
        g.Weeks(8);
        g.Me.Reputation = 0;
        g.Do(Command.SetSla(g.Id, 1));
        Assert.Equal(1, g.Me.SlaLevel);
    }

    [Fact]
    public void A_rival_lab_and_a_game_with_no_levels_cannot_pledge()
    {
        Game g = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        Assert.False(g.Sim.Apply(Command.SetSla(rival.Id, 1)).Ok);
        Game off = Lab(c => c.Sla.MaxLevel = 0);
        off.Refused(Command.SetSla(off.Id, 1), CommandError.InvalidArgument);
    }

    // ---- the weeks

    [Fact]
    public void A_week_that_keeps_the_promise_earns_the_premium_on_the_books()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 2));
        long cash = g.Me.Cash;
        long revLine = g.Me.Ledger.Lifetime.Revenue;
        Week(g, 1_000_000, 1000);
        Assert.Equal(cash + 80_000, g.Me.Cash);
        Assert.Equal(revLine + 80_000, g.Me.Ledger.Lifetime.Revenue);
        Assert.Equal(1_080_000, g.Me.LastWeek.Revenue);
        Assert.Empty(Of(g, EventKind.SlaBroken));
    }

    [Fact]
    public void Serving_exactly_the_bar_still_keeps_the_promise()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 1));
        Week(g, 1_000_000, 900);
        Assert.Empty(Of(g, EventKind.SlaBroken));
        Week(g, 1_000_000, 899);
        Assert.Single(Of(g, EventKind.SlaBroken));
    }

    [Fact]
    public void A_week_short_on_serving_compute_refunds_costs_reputation_and_earns_no_premium()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 2));
        long cash = g.Me.Cash;
        Week(g, 1_000_000, 500);
        Assert.Equal(cash - 240_000, g.Me.Cash);
        Assert.Equal(600 - 16, g.Me.Reputation);
        Assert.Equal(1, g.Me.SlaStrikes);
        GameEvent e = Assert.Single(Of(g, EventKind.SlaBroken));
        Assert.Equal(0, e.Pick);
        Assert.Equal(240_000, e.Amount);
        Assert.Contains("refunded", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void An_incident_in_the_week_breaks_the_promise_even_when_serving_is_fine()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        g.Do(Command.SetSla(g.Id, 1));
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        g.C.Safety.IncidentChancePermille = 0;
        long cash = g.Me.Cash;
        int rep = g.Me.Reputation;
        Week(g, 1_000_000, 1000);
        Assert.Equal(cash - 120_000, g.Me.Cash);
        Assert.Equal(rep - 8, g.Me.Reputation);
        Assert.Equal(1, Assert.Single(Of(g, EventKind.SlaBroken)).Pick);
    }

    [Fact]
    public void A_week_with_no_revenue_has_nothing_to_pledge_about()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 3));
        long cash = g.Me.Cash;
        Week(g, 0, 0);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(600, g.Me.Reputation);
        Assert.Equal(0, g.Me.SlaStrikes);
    }

    [Fact]
    public void A_good_week_clears_the_strikes()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 1));
        Week(g, 1000, 0);
        Week(g, 1000, 0);
        Assert.Equal(2, g.Me.SlaStrikes);
        Week(g, 1000, 1000);
        Assert.Equal(0, g.Me.SlaStrikes);
    }

    [Fact]
    public void A_run_of_broken_weeks_drops_the_pledge_a_level_and_locks_it()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 3));
        Week(g, 1000, 0);
        Week(g, 1000, 0);
        Assert.Equal(3, g.Me.SlaLevel);
        Week(g, 1000, 0);
        Assert.Equal(2, g.Me.SlaLevel);
        Assert.Equal(0, g.Me.SlaStrikes);
        Assert.Equal(g.State.Tick + 8, g.Me.SlaLockedUntil);
        GameEvent e = Of(g, EventKind.SlaPledged).Last();
        Assert.Equal(0, e.Pick);
        Assert.Equal(2, e.SubjectId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void With_no_strike_limit_the_pledge_is_never_dropped()
    {
        Game g = Lab(c => c.Sla.StrikeWeeks = 0);
        g.Do(Command.SetSla(g.Id, 3));
        for (int i = 0; i < 20; i++) Week(g, 1000, 0);
        Assert.Equal(3, g.Me.SlaLevel);
        Assert.Equal(20, g.Me.SlaStrikes);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Reputation_never_goes_below_zero_and_a_pledge_that_always_breaks_never_pays()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 3));
        g.Me.Reputation = 5;
        long cash = g.Me.Cash;
        Week(g, 1_000_000, 0);
        Assert.Equal(0, g.Me.Reputation);
        Assert.True(g.Me.Cash < cash);
        Assert.True(Sla.RefundPermille(g.C, g.Me) > Sla.PremiumPermille(g.C, g.Me));
    }

    [Fact]
    public void In_a_real_game_a_pledge_earns_more_than_none_when_the_promise_holds()
    {
        Game plain = Lab();
        ModelState a = plain.Train("mini", 0);
        plain.UntilTrained(a);
        plain.Do(Command.ReleaseModel(plain.Id, a.Id, 0, "game-ai", "game-bots"));
        Game pledged = Lab();
        ModelState b = pledged.Train("mini", 0);
        pledged.UntilTrained(b);
        pledged.Do(Command.ReleaseModel(pledged.Id, b.Id, 0, "game-ai", "game-bots"));
        pledged.Do(Command.SetSla(pledged.Id, 1));
        plain.Weeks(12);
        pledged.Weeks(12);
        Assert.Empty(SaveGame.Problems(pledged.C, pledged.State));
        if (Of(pledged, EventKind.SlaBroken).Count == 0) Assert.True(pledged.Me.Ledger.Lifetime.Revenue > plain.Me.Ledger.Lifetime.Revenue);
        else Assert.True(pledged.Me.Reputation <= plain.Me.Reputation);
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_pledge_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 2));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.Player.SlaLevel);
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.Solo(2, c => { c.Safety.IncidentChancePermille = 100; c.Sla.MinReputation = 0; });
        g.Do(Command.SetSla(g.Id, 2));
        g.Weeks(30);
        g.Do(Command.SetSla(g.Id, 1));
        g.Weeks(20);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_pledge()
    {
        Game g = Lab();
        g.Do(Command.SetSla(g.Id, 1));
        string old = SaveSchemaPretend.At(g.C, 58, g.Sim.Save());
        Assert.DoesNotContain("slaLevel", old);
        Assert.DoesNotContain("slaStrikes", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.SlaLevel); Assert.Equal(0, lab.SlaLockedUntil); Assert.Equal(0, lab.SlaStrikes); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_pledge_that_does_not_fit_the_data_is_reported()
    {
        Game g = Lab();
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("service-level pledge"));
        g.Me.SlaLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("service-level pledge"));
        g.Me.SlaLevel = 1;
        g.Me.SlaStrikes = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("service-level pledge"));
        g.Me.SlaStrikes = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("service-level pledge"));
        g.Me.SlaStrikes = 0;
        g.Me.SlaLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("service-level pledge"));
    }

    // ---- a long game

    [Fact]
    public void A_long_game_with_a_pledge_changed_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Game.Solo(11, c => { c.Safety.IncidentChancePermille = 300; }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        int changes = 0;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            if (i % 30 == 3 && g.Try(Command.SetSla(g.Id, (i / 30) % 4)).Ok) changes++;
        }
        Assert.True(changes > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
