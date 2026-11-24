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

/// <summary>The ethics board: outsiders who refuse rushed launches, soften scandals and ease governments, and walk out loudly if the lab drops them.</summary>
public class EthicsBoardTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true, int safety = 2)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.World.Attention.PreparedPolicyStaff = 100;
            if (tweak != null) tweak(c);
        }).WithStarterLab(safety: safety, gpus: 8);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Trained(Game g, int alignment = -1)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        if (alignment >= 0) { m.Alignment = alignment; m.DisplayedAlignment = alignment; }
        return m;
    }

    static List<GameEvent> Changes(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.EthicsBoardChanged).ToList(); }

    static void Incident(Game g, ModelState m)
    {
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
    }

    // ---- nothing happens by itself

    [Fact]
    public void No_board_sits_until_the_player_seats_one()
    {
        Game g = Lab();
        ModelState m = Trained(g, 100);
        string bloc = g.Me.HomeBlocId;
        g.Me.IncidentsThisMonth = 6;
        long saving = WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0).ComplianceSaving;
        g.Weeks(30);
        Assert.Equal(0, g.Me.EthicsLevel);
        Assert.Equal(0, EthicsBoard.ScandalCutPermille(g.C, g.Me));
        Assert.Equal(0, EthicsBoard.AttentionReliefPermille(g.C, g.Me));
        Assert.Equal(0, EthicsBoard.HearingPoints(g.C, g.Me));
        Assert.Equal(0, EthicsBoard.WeeklyCost(g.C, g.Me));
        Assert.False(EthicsBoard.BlocksRush(g.C, g.Me));
        Assert.False(EthicsBoard.Vetoes(g.C, g.Me, m));
        Assert.Equal(saving, WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0).ComplianceSaving);
        Assert.Empty(Changes(g));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
    }

    // ---- the command

    [Fact]
    public void Seating_a_level_costs_the_setup_fee_sets_the_level_and_logs_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        Assert.Equal(1, g.Me.EthicsLevel);
        Assert.Equal(cash - 40000, g.Me.Cash);
        GameEvent e = Changes(g).Single();
        Assert.Equal(1, e.SubjectId);
        Assert.Equal(40000, e.Amount);
        Assert.Equal(1, e.Pick);
        Assert.Contains("seated an ethics board (level 1) for $40000", EventText.Describe(g.C, g.State, e));
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        Assert.Equal(cash - 80000, g.Me.Cash);
        Assert.Equal(2, EthicsBoard.Level(g.C, g.Me));
    }

    [Fact]
    public void Jumping_two_levels_pays_for_both_and_each_level_needs_its_safety_people()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        Assert.Equal(cash - 80000, g.Me.Cash);

        Game thin = Lab(safety: 1);
        thin.Do(Command.SetEthicsBoard(thin.Id, 1));
        thin.Refused(Command.SetEthicsBoard(thin.Id, 2), CommandError.NotEnoughStaff);
        Assert.Equal(1, thin.Me.EthicsLevel);

        Game none = Lab(safety: 0);
        none.Refused(Command.SetEthicsBoard(none.Id, 1), CommandError.NotEnoughStaff);
        Assert.Equal(0, none.Me.EthicsLevel);
    }

    [Fact]
    public void Bad_levels_the_same_level_a_poor_lab_a_rival_and_a_game_without_a_board_are_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetEthicsBoard(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetEthicsBoard(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetEthicsBoard(g.Id, 0), CommandError.InvalidArgument);

        Game poor = Lab(rich: false);
        poor.Me.Cash = 39_999;
        poor.Refused(Command.SetEthicsBoard(poor.Id, 1), CommandError.NotEnoughCash);
        Assert.Equal(0, poor.Me.EthicsLevel);

        Game game = Game.New(1);
        LabState rival = game.State.Labs.First(l => l.Id != game.State.PlayerLabId);
        game.Refused(Command.SetEthicsBoard(rival.Id, 1), CommandError.InvalidArgument);

        Game off = Lab(c => { c.Board.MaxLevel = 0; c.Board.VetoLevel = 0; });
        off.Refused(Command.SetEthicsBoard(off.Id, 1), CommandError.InvalidArgument);
        Assert.False(EthicsBoard.Enabled(off.C));

        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.SetEthicsBoard(broke.Id, 1), CommandError.LabBankrupt);
    }

    // ---- what a board does

    [Fact]
    public void Any_board_refuses_a_rushed_launch_and_changes_nothing()
    {
        Game g = Lab();
        ModelState m = Trained(g, 800);
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        long cash = g.Me.Cash;
        g.Refused(Command.RushRelease(g.Id, m.Id, 2, "game-ai", "game-bots"), CommandError.BoardVeto);
        Assert.Equal(ModelStatus.Trained, m.Status);
        Assert.Equal(0, m.RushWeeks);
        Assert.Equal(cash, g.Me.Cash);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        Assert.Equal(ModelStatus.Released, m.Status);
    }

    [Fact]
    public void Without_a_board_a_rushed_launch_still_works()
    {
        Game g = Lab();
        ModelState m = Trained(g, 800);
        g.Do(Command.RushRelease(g.Id, m.Id, 2, "game-ai", "game-bots"));
        Assert.Equal(2, m.RushWeeks);
    }

    [Fact]
    public void The_top_level_vetoes_a_first_release_whose_true_alignment_is_under_the_bar()
    {
        Game g = Lab();
        ModelState m = Trained(g, 300);
        m.DisplayedAlignment = 900;
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        Assert.True(EthicsBoard.Vetoes(g.C, g.Me, m));
        g.Refused(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"), CommandError.BoardVeto);
        Assert.Equal(ModelStatus.Trained, m.Status);
        m.Alignment = 400;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        Assert.Equal(ModelStatus.Released, m.Status);
    }

    [Fact]
    public void Level_one_does_not_veto_and_a_model_already_on_sale_can_be_sold_into_more_sectors()
    {
        Game g = Lab();
        ModelState m = Trained(g, 800);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Alignment = 100;
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        Assert.Contains("game-bots", m.Sectors);

        Game one = Lab();
        ModelState weak = Trained(one, 100);
        one.Do(Command.SetEthicsBoard(one.Id, 1));
        Assert.False(EthicsBoard.Vetoes(one.C, one.Me, weak));
        one.Do(Command.ReleaseModel(one.Id, weak.Id, 0, "game-ai", "game-bots"));
    }

    [Fact]
    public void A_board_with_no_veto_in_the_data_never_vetoes()
    {
        Game g = Lab(c => c.Board.VetoLevel = 0);
        ModelState m = Trained(g, 0);
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        Assert.False(EthicsBoard.Vetoes(g.C, g.Me, m));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
    }

    [Fact]
    public void An_incident_costs_less_reputation_with_a_board_and_less_again_with_a_bigger_one()
    {
        int Rep(int level)
        {
            Game g = Lab();
            ModelState m = Trained(g, 800);
            g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
            if (level > 0) g.Do(Command.SetEthicsBoard(g.Id, level));
            g.Me.Reputation = 500;
            Incident(g, m);
            Assert.Contains(g.State.Events, e => e.Kind == EventKind.Incident);
            return g.Me.Reputation;
        }
        int none = Rep(0);
        int one = Rep(1);
        int two = Rep(2);
        Assert.True(one > none);
        Assert.True(two > one);
    }

    [Fact]
    public void The_board_takes_its_share_off_regulator_attention_beside_the_compliance_office()
    {
        Game g = Lab();
        ModelState m = Trained(g, 800);
        g.Me.IncidentsThisMonth = 6;
        string bloc = g.Me.HomeBlocId;
        AttentionGain none = WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0);
        long afterPolicy = none.FromFootprint + none.FromIncidents + none.FromDominance + none.PolicySaving;
        Assert.True(afterPolicy > 0);
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        Assert.Equal(MathI.MulPermille(afterPolicy, 900) - afterPolicy, WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0).ComplianceSaving);
        g.Me.ComplianceLevel = 1;
        Assert.Equal(MathI.MulPermille(afterPolicy, 820) - afterPolicy, WorldQueries.GainFor(g.C, g.State, g.Me, bloc, 0).ComplianceSaving);
        Assert.NotNull(m);
    }

    [Fact]
    public void A_hearing_goes_better_with_a_board()
    {
        int Rep(int level)
        {
            Game g = Lab();
            if (level > 0) g.Do(Command.SetEthicsBoard(g.Id, level));
            g.Me.Reputation = 500;
            g.Me.Bloc("us").Level = BlocStanding.Letter;
            g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
            g.Me.Ledger.ThisMonth.Revenue = 0;
            GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
            Assert.Equal(BlocStanding.Hearing, g.Me.Bloc("us").Level);
            return g.Me.Reputation;
        }
        int plain = Rep(0);
        Assert.Equal(plain + 3, Rep(1));
        Assert.Equal(plain + 6, Rep(2));
    }

    [Fact]
    public void The_board_is_paid_every_week_by_level()
    {
        Game g = Lab();
        Game h = Lab();
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        g.Weeks(1);
        h.Weeks(1);
        Assert.Equal(5000, EthicsBoard.WeeklyCost(g.C, g.Me));
        Assert.Equal(5000, h.Me.Ledger.Lifetime.Salaries - g.Me.Ledger.Lifetime.Salaries);
    }

    // ---- stepping down and walking out

    [Fact]
    public void Stepping_down_costs_reputation_logs_it_and_locks_the_board_out_for_a_year()
    {
        Game g = Lab();
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        g.Me.Reputation = 500;
        long cash = g.Me.Cash;
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        Assert.Equal(480, g.Me.Reputation);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(g.Tick + 52, g.Me.EthicsLockedUntil);
        GameEvent e = Changes(g).Last();
        Assert.Equal(0, e.Pick);
        Assert.Contains("stepped its ethics board down to level 1", EventText.Describe(g.C, g.State, e));
        g.Refused(Command.SetEthicsBoard(g.Id, 2), CommandError.EthicsLocked);
        Assert.Equal(1, g.Me.EthicsLevel);
    }

    [Fact]
    public void The_lock_ends_after_the_data_s_weeks_and_a_lower_level_is_never_locked()
    {
        Game g = Lab(c => c.Board.LockWeeks = 4);
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        g.Do(Command.SetEthicsBoard(g.Id, 0));
        g.Refused(Command.SetEthicsBoard(g.Id, 1), CommandError.EthicsLocked);
        g.Weeks(4);
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        g.Do(Command.SetEthicsBoard(g.Id, 0));
        Assert.Equal(0, g.Me.EthicsLevel);
    }

    [Fact]
    public void A_board_whose_safety_people_have_gone_walks_out_with_the_same_price()
    {
        Game g = Lab();
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        g.Me.Reputation = 500;
        StaffMember gone = g.Me.Staff.First(p => p.RoleId == "safety");
        g.Me.Staff.Remove(gone);
        EthicsBoard.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.EthicsLevel);
        Assert.Equal(480, g.Me.Reputation);
        Assert.Equal(g.Tick + 52, g.Me.EthicsLockedUntil);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.EthicsBoardResigned);
        Assert.Equal(1, e.SubjectId);
        Assert.Equal(2, e.Amount);
        Assert.Contains("resigned from level 2 to 1", EventText.Describe(g.C, g.State, e));
        EthicsBoard.RunWeek(g.C, g.State, g.Me);
        Assert.Single(g.State.Events, x => x.Kind == EventKind.EthicsBoardResigned);
    }

    [Fact]
    public void Losing_every_safety_person_empties_the_board_and_reputation_never_goes_below_zero()
    {
        Game g = Lab();
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        g.Me.Reputation = 5;
        g.Me.Staff.RemoveAll(p => p.RoleId == "safety");
        g.Weeks(1);
        Assert.Equal(0, g.Me.EthicsLevel);
        Assert.True(g.Me.Reputation >= 0);
        Assert.Equal(0, EthicsBoard.WeeklyCost(g.C, g.Me));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void The_board_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetEthicsBoard(g.Id, 2));
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(1, back.Player.EthicsLevel);
        Assert.Equal(g.Me.EthicsLockedUntil, back.Player.EthicsLockedUntil);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => { c.Board.SetupCostPerLevel = 100; c.Board.SafetyStaffPerLevel = 0; });
        g.Autopilot(20);
        Assert.True(g.Try(Command.SetEthicsBoard(g.Id, 1)).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal(g.Me.EthicsLevel, again.Player.EthicsLevel);
    }

    [Fact]
    public void An_older_save_loads_with_no_board()
    {
        Game g = Lab();
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        string old = SaveSchemaPretend.At(g.C, 53, g.Sim.Save());
        Assert.DoesNotContain("ethicsLevel", old);
        Assert.DoesNotContain("ethicsLockedUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.EthicsLevel); Assert.Equal(0, lab.EthicsLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_board_the_data_does_not_allow_is_reported()
    {
        Game g = Lab();
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("ethics board"));
        g.Me.EthicsLevel = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("ethics board"));
        g.Me.EthicsLevel = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("ethics board"));
        g.Me.EthicsLevel = 1;
        g.Me.EthicsLockedUntil = -4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("ethics board"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(2, c.Board.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Board.MaxLevel = 6;
        c.Board.SetupCostPerLevel = -1;
        c.Board.WeeklyCostPerLevel = -1;
        c.Board.SafetyStaffPerLevel = -1;
        c.Board.ScandalCutPermillePerLevel = 1000;
        c.Board.HearingPointsPerLevel = 101;
        c.Board.AttentionPermillePerLevel = 1000;
        c.Board.VetoLevel = 7;
        c.Board.VetoAlignment = 1001;
        c.Board.ResignationReputation = 101;
        c.Board.LockWeeks = 521;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "setupCostPerLevel", "weeklyCostPerLevel", "safetyStaffPerLevel", "scandalCutPermillePerLevel", "hearingPointsPerLevel", "attentionPermillePerLevel", "vetoLevel", "vetoAlignment", "resignationReputation", "lockWeeks" })
            Assert.Contains(problems, p => p.Contains("ethicsBoard." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_seat_a_board()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.EthicsLevel));
        Assert.Empty(Changes(g));
    }

    [Fact]
    public void A_game_with_a_board_that_comes_and_goes_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2, c => { c.Board.SafetyStaffPerLevel = 0; });
        g.Autopilot(30);
        g.Me.Cash += 5_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int seated = 0;
        for (int i = 0; i < 220; i++)
        {
            g.Autopilot(1);
            if (i % 60 == 5 && g.Try(Command.SetEthicsBoard(g.Id, 2)).Ok) seated++;
            if (i % 60 == 35 && g.Me.EthicsLevel > 0) g.Try(Command.SetEthicsBoard(g.Id, 0));
        }
        Assert.True(seated >= 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
