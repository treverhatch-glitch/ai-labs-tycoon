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

/// <summary>Staff equity: a slice of the founder's stake buys lower salaries, steadier morale and a harder team to poach, and comes out of the sale price.</summary>
public class StaffEquityTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.RoomPerks.Clear();
            c.RolePerks.Clear();
            c.Funding.Round("angel").OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 2, gpus: 10);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) { p.TraitId = "steady"; p.Morale = 500; }
        return g;
    }

    static List<GameEvent> Grants(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.EquityGranted).ToList(); }

    // ---- nothing happens by itself

    [Fact]
    public void Nobody_has_equity_until_the_player_grants_it()
    {
        Game g = Lab();
        long salaries = LabQueries.WeeklySalaries(g.C, g.Me);
        int resting = Workplace.RestingMorale(g.C, g.Me);
        g.Weeks(30);
        Assert.Equal(0, g.Me.EquityLevel);
        Assert.Equal(0, g.Me.EquityPermille);
        Assert.Equal(0, StaffEquity.SalaryCutPermille(g.C, g.Me));
        Assert.Equal(0, StaffEquity.MoraleOffset(g.C, g.Me));
        Assert.Equal(0, StaffEquity.PoachDefencePermille(g.C, g.Me));
        Assert.Equal(salaries, LabQueries.WeeklySalaries(g.C, g.Me));
        Assert.Equal(resting, Workplace.RestingMorale(g.C, g.Me));
        Assert.Equal(0, DisplayQueries.ExitPriceParts(g.C, g.State, g.Me).StaffShare);
        Assert.Empty(Grants(g));
    }

    // ---- the command

    [Fact]
    public void A_grant_moves_a_slice_of_the_founder_s_stake_to_the_people_and_logs_it()
    {
        Game g = Lab();
        Assert.Equal(1000, g.Me.FounderSharePermille);
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(1, g.Me.EquityLevel);
        Assert.Equal(960, g.Me.FounderSharePermille);
        Assert.Equal(40, g.Me.EquityPermille);
        GameEvent e = Grants(g).Single();
        Assert.Equal(1, e.SubjectId);
        Assert.Equal(40, e.Amount);
        Assert.Contains("4.0%", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Each_level_takes_the_data_s_share_of_what_the_founder_has_left_and_the_stakes_always_add_up()
    {
        Game g = Lab();
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(40 + 38 + 36, g.Me.EquityPermille);
        Assert.Equal(1000 - 114, g.Me.FounderSharePermille);
        Assert.Equal(1000, g.Me.FounderSharePermille + g.Me.EquityPermille);
        Assert.Equal(3, g.Me.EquityLevel);
    }

    [Fact]
    public void The_most_the_data_allows_a_small_team_and_a_lost_board_are_refused_and_change_nothing()
    {
        Game g = Lab(c => c.Equity.MaxLevel = 1);
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Refused(Command.GrantStaffEquity(g.Id), CommandError.InvalidArgument);
        Assert.Equal(1, g.Me.EquityLevel);

        Game small = Lab();
        small.Me.Staff.RemoveRange(2, small.Me.Staff.Count - 2);
        small.Refused(Command.GrantStaffEquity(small.Id), CommandError.NotEnoughStaff);
        Assert.Equal(0, small.Me.EquityLevel);

        Game board = Lab();
        board.Me.FounderSharePermille = 460;
        board.Refused(Command.GrantStaffEquity(board.Id), CommandError.WouldLoseControl);
        Assert.Equal(460, board.Me.FounderSharePermille);
        Assert.Equal(0, board.Me.EquityPermille);
        board.Me.FounderSharePermille = 480;
        board.Do(Command.GrantStaffEquity(board.Id));
        Assert.Equal(461, board.Me.FounderSharePermille);
    }

    [Fact]
    public void A_rival_a_bankrupt_lab_and_a_game_without_equity_refuse_it()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.GrantStaffEquity(rival.Id), CommandError.InvalidArgument);
        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.GrantStaffEquity(broke.Id), CommandError.LabBankrupt);
        Game off = Lab(c => c.Equity.MaxLevel = 0);
        off.Refused(Command.GrantStaffEquity(off.Id), CommandError.InvalidArgument);
        Assert.False(StaffEquity.Enabled(off.C));
    }

    [Fact]
    public void A_grant_never_moves_less_than_one_permille()
    {
        Game g = Lab();
        g.Me.FounderSharePermille = 500;
        g.C.Equity.DilutionPermille = 1;
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(1, g.Me.EquityPermille);
        Assert.Equal(499, g.Me.FounderSharePermille);
    }

    // ---- what the team gets

    [Fact]
    public void Each_level_takes_a_share_off_every_salary_on_top_of_the_pay_policy()
    {
        Game g = Lab();
        long plain = LabQueries.WeeklySalaries(g.C, g.Me);
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(MathI.MulPermille(plain, 940), LabQueries.WeeklySalaries(g.C, g.Me));
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(MathI.MulPermille(plain, 820), LabQueries.WeeklySalaries(g.C, g.Me));
        g.Me.PayLevel = 2;
        long paid = LabQueries.WeeklySalaries(g.C, g.Me);
        int step = g.C.StaffPeople.PaySalaryStepPermille;
        Assert.Equal(MathI.MulPermille(plain, 1000 + 2 * step - 180), paid);
    }

    [Fact]
    public void The_weekly_close_pays_the_lower_salary_and_the_books_balance()
    {
        Game g = Lab();
        g.Do(Command.GrantStaffEquity(g.Id));
        long bill = LabQueries.WeeklySalaries(g.C, g.Me);
        long before = g.Me.Ledger.Lifetime.Salaries;
        g.Weeks(1);
        Assert.Equal(before - bill, g.Me.Ledger.Lifetime.Salaries);
    }

    [Fact]
    public void Morale_settles_higher_by_the_data_s_step_for_each_level()
    {
        Game g = Lab();
        int resting = Workplace.RestingMorale(g.C, g.Me);
        StaffMember p = g.Me.Staff[0];
        int personal = Traits.RestingMorale(g.C, g.Me, p);
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(personal + 20, Traits.RestingMorale(g.C, g.Me, p));
        Assert.Equal(resting + 20, Workplace.RestingMorale(g.C, g.Me));
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(resting + 60, Workplace.RestingMorale(g.C, g.Me));
    }

    [Fact]
    public void Over_months_the_team_ends_up_happier_than_the_same_team_without_equity()
    {
        Game plain = Lab();
        Game equity = Lab();
        equity.Do(Command.GrantStaffEquity(equity.Id));
        equity.Do(Command.GrantStaffEquity(equity.Id));
        plain.Weeks(30);
        equity.Weeks(30);
        Assert.True(Crunch.AverageMorale(equity.Me) > Crunch.AverageMorale(plain.Me));
    }

    [Fact]
    public void An_offsite_can_lift_morale_higher_when_the_team_owns_a_part_of_the_lab()
    {
        Game g = Lab();
        StaffMember p = g.Me.Staff[0];
        int before = Offsite.Ceiling(g.C, g.Me, p);
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(before + 20, Offsite.Ceiling(g.C, g.Me, p));
    }

    [Fact]
    public void Rivals_find_the_team_harder_to_poach()
    {
        Game g = Lab();
        int basePoach = Workplace.PoachChancePermille(g.C, g.Me);
        int defence = Workplace.PoachDefencePermille(g.C, g.Me);
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.Equal(defence + 100, Workplace.PoachDefencePermille(g.C, g.Me));
        Assert.True(Workplace.PoachChancePermille(g.C, g.Me) < basePoach);
    }

    // ---- what it costs

    [Fact]
    public void A_funding_round_dilutes_the_people_s_stake_with_the_founder_s()
    {
        Game g = Lab();
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        int stake = g.Me.EquityPermille;
        int founder = g.Me.FounderSharePermille;
        int dilution = g.C.Funding.Round("angel").DilutionPermille;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(MathI.MulPermille(stake, 1000 - dilution), g.Me.EquityPermille);
        Assert.Equal(MathI.MulPermille(founder, 1000 - dilution), g.Me.FounderSharePermille);
        Assert.True(g.Me.EquityPermille + g.Me.FounderSharePermille <= 1000);
        Assert.Equal(2, g.Me.EquityLevel);
    }

    [Fact]
    public void A_tiny_stake_still_counts_as_a_stake_after_a_round()
    {
        Game g = Lab();
        g.Me.EquityLevel = 1;
        g.Me.EquityPermille = 1;
        g.Me.FounderSharePermille = 999;
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Equal(1, g.Me.EquityPermille);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Giving_equity_can_make_the_next_round_take_the_board_s_control()
    {
        Game g = Lab();
        g.Me.FounderSharePermille = 480;
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.OustedId);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.OustedId);
    }

    [Fact]
    public void The_sale_price_has_the_people_s_stake_taken_out_of_it()
    {
        Game g = Lab(c => c.Endings.ExitRevenueYears = 2);
        g.Me.Ledger.ThisMonth.Revenue = 1_000_000;
        g.Me.Cash = 500_000;
        DisplayQueries.ExitPriceBreakdown none = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.Equal(0, none.StaffShare);
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        DisplayQueries.ExitPriceBreakdown b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        long gross = b.RevenueYears + b.FleetResale + b.CashOnHand;
        Assert.Equal(gross, none.Total);
        Assert.Equal(MathI.MulPermille(gross, 78), b.StaffShare);
        Assert.Equal(gross - b.StaffShare, b.Total);
        Assert.Equal(b.Total, EndingSystem.SellPrice(g.C, g.State, g.Me));
    }

    [Fact]
    public void Selling_pays_the_new_lab_the_net_price_and_the_old_lab_s_equity_is_cleared()
    {
        Game g = Lab(c => c.Endings.ExitRevenueYears = 2);
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        g.Me.Cash = 100_000;
        g.Do(Command.GrantStaffEquity(g.Id));
        long price = EndingSystem.SellPrice(g.C, g.State, g.Me);
        LabState old = g.Me;
        g.Do(Command.SellLab(g.Id, "found-again"));
        Assert.Equal(0, old.EquityLevel);
        Assert.Equal(0, old.EquityPermille);
        Assert.True(g.State.Labs[0].Cash >= price - 1);
        Assert.Equal(0, g.State.Labs[0].EquityLevel);
    }

    // ---- saving, loading, migrating

    [Fact]
    public void Equity_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.Player.EquityLevel);
        Assert.Equal(g.Me.EquityPermille, back.Player.EquityPermille);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2);
        g.Autopilot(20);
        Assert.True(g.Try(Command.GrantStaffEquity(g.Id)).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal(1, again.Player.EquityLevel);
    }

    [Fact]
    public void An_older_save_loads_with_no_equity()
    {
        Game g = Lab();
        g.Do(Command.GrantStaffEquity(g.Id));
        string old = SaveSchemaPretend.At(g.C, 52, g.Sim.Save());
        Assert.DoesNotContain("equityLevel", old);
        Assert.DoesNotContain("equityPermille", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(0, lab.EquityLevel); Assert.Equal(0, lab.EquityPermille); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_where_the_stakes_do_not_fit_is_reported()
    {
        Game g = Lab();
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("equity level"));
        g.Me.EquityLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("equity level"));
        g.Me.EquityLevel = 1;
        g.Me.EquityPermille = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("equity level"));
        g.Me.EquityPermille = 100;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("equity level"));
        g.Me.EquityPermille = 40;
        g.Me.EquityLevel = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("equity level"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Equity.MaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Equity.MaxLevel = 11;
        c.Equity.DilutionPermille = 0;
        c.Equity.SalaryCutPermille = 100;
        c.Equity.MoraleStep = 100;
        c.Equity.PoachStepPermille = 100;
        c.Equity.MinStaff = -1;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "maxLevel", "dilutionPermille", "salaryCutPermille", "moraleStep", "poachStepPermille", "minStaff" })
            Assert.Contains(problems, p => p.Contains("equity." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_grant_it_and_rival_salaries_are_untouched()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => { Assert.Equal(0, lab.EquityLevel); Assert.Equal(0, lab.EquityPermille); });
        Assert.Empty(Grants(g));
    }

    [Fact]
    public void An_autopilot_game_with_grants_and_rounds_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2, c =>
        {
            c.Funding.Round("angel").OpensYear = 1990;
            c.Funding.Round("angel").MinReputation = 0;
        });
        g.Autopilot(30);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int granted = 0;
        for (int i = 0; i < 200; i++)
        {
            g.Autopilot(1);
            if (i == 20 || i == 80) { if (g.Try(Command.GrantStaffEquity(g.Id)).Ok) granted++; }
            if (i == 50) g.Try(Command.RaiseFunding(g.Id, "angel"));
        }
        Assert.True(granted >= 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.True(g.Me.EquityPermille + g.Me.FounderSharePermille <= 1000);
        Assert.Equal(granted, Grants(g).Count);
    }
}
