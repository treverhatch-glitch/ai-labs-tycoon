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

/// <summary>Incident insurance for the player's lab: a weekly premium for a share of each fine, with exclusions, loading and a drop-out.</summary>
public class InsuranceTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(gpus: 8);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Released(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Release(m, "game-ai", "game-bots");
        return m;
    }

    /// <summary>One incident for sure, with the weekly roll forced to hit.</summary>
    static GameEvent Incident(Game g, ModelState m)
    {
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 1000;
        int before = g.State.Events.Count;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        return g.State.Events.Skip(before).First(e => e.Kind == EventKind.Incident);
    }

    // ---- the command

    [Fact]
    public void Nobody_is_insured_until_the_player_buys()
    {
        Game g = Lab();
        Assert.Equal(0, g.Me.InsuranceLevel);
        Assert.Equal(0, Insurance.CoverPermille(g.C, g.Me));
        Assert.Equal(0, Insurance.Premium(g.C, g.Me));
        g.Weeks(10);
        Assert.Equal(0, g.Me.Ledger.Lifetime.Insurance);
    }

    [Fact]
    public void Buying_a_level_sets_it_and_locks_changes_for_the_data_s_weeks()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 2));
        Assert.Equal(2, g.Me.InsuranceLevel);
        Assert.Equal(500, Insurance.CoverPermille(g.C, g.Me));
        Assert.Equal(g.Tick + 13, g.Me.InsuranceLockedUntil);
        g.Refused(Command.SetInsurance(g.Id, 3), CommandError.InsuranceLocked);
        g.Refused(Command.SetInsurance(g.Id, 0), CommandError.InsuranceLocked);
        Assert.Equal(2, g.Me.InsuranceLevel);
        g.Weeks(13);
        g.Do(Command.SetInsurance(g.Id, 3));
        Assert.Equal(3, g.Me.InsuranceLevel);
        Assert.Equal(750, Insurance.CoverPermille(g.C, g.Me));
    }

    [Fact]
    public void A_level_outside_the_range_or_the_same_level_is_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetInsurance(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.SetInsurance(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetInsurance(g.Id, 0), CommandError.InvalidArgument);
        Assert.Equal(0, g.Me.InsuranceLockedUntil);
    }

    [Fact]
    public void A_game_with_no_levels_in_the_data_refuses_every_policy()
    {
        Game g = Lab(c => c.Finance.InsuranceMaxLevel = 0);
        g.Refused(Command.SetInsurance(g.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Only_the_players_lab_can_buy()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents" };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        Assert.False(g.Try(Command.SetInsurance(rival.Id, 1)).Ok);
        Assert.Equal(0, rival.InsuranceLevel);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_buy_and_pays_no_premium()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 1));
        g.Me.Bankrupt = true;
        g.Refused(Command.SetInsurance(g.Id, 0), CommandError.LabBankrupt);
        long paid = g.Me.Ledger.Lifetime.Insurance;
        g.Weeks(5);
        Assert.Equal(paid, g.Me.Ledger.Lifetime.Insurance);
    }

    [Fact]
    public void A_lab_with_too_many_recent_claims_cannot_buy_more_but_can_cancel_or_keep_what_it_has()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 1));
        g.Weeks(13);
        g.Me.InsuranceClaims = 3;
        g.Refused(Command.SetInsurance(g.Id, 2), CommandError.Uninsurable);
        g.Do(Command.SetInsurance(g.Id, 0));
        Assert.Equal(0, g.Me.InsuranceLevel);
        g.Weeks(13);
        g.Refused(Command.SetInsurance(g.Id, 1), CommandError.Uninsurable);
    }

    // ---- the premium

    [Fact]
    public void With_no_revenue_the_premium_is_the_minimum_for_each_level()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 2));
        g.Me.Ledger.LastWeek.Revenue = 0;
        Assert.Equal(4000, Insurance.Premium(g.C, g.Me));
    }

    [Fact]
    public void With_revenue_the_premium_is_a_share_of_it_for_each_level()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 3));
        g.Me.Ledger.LastWeek.Revenue = 10_000_000;
        Assert.Equal(3 * 120_000, Insurance.Premium(g.C, g.Me));
    }

    [Fact]
    public void Each_remembered_claim_loads_the_premium()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 1));
        g.Me.Ledger.LastWeek.Revenue = 0;
        long plain = Insurance.Premium(g.C, g.Me);
        g.Me.InsuranceClaims = 2;
        Assert.Equal(MathI.MulPermille(plain, 1600), Insurance.Premium(g.C, g.Me));
    }

    [Fact]
    public void The_premium_comes_out_of_cash_and_into_its_own_ledger_line_every_week()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 1));
        g.Me.Ledger.LastWeek.Revenue = 0;
        long expected = Insurance.Premium(g.C, g.Me);
        long cash = g.Me.Cash;
        long lifetime = g.Me.Ledger.Lifetime.Insurance;
        g.Weeks(1);
        Assert.Equal(-expected, g.Me.Ledger.LastWeek.Insurance);
        Assert.Equal(lifetime - expected, g.Me.Ledger.Lifetime.Insurance);
        Assert.True(g.Me.Cash < cash);
    }

    [Fact]
    public void Cash_is_still_explained_by_the_ledger_with_a_policy_running()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 2));
        long before = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Weeks(30);
        Assert.Equal(before, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.True(g.Me.Ledger.Lifetime.Insurance < 0);
    }

    // ---- what it pays

    [Fact]
    public void An_insured_incident_costs_the_lab_less_by_exactly_what_the_insurer_paid()
    {
        Game plain = Lab();
        GameEvent a = Incident(plain, Released(plain));
        Game insured = Lab();
        ModelState m = Released(insured);
        insured.Do(Command.SetInsurance(insured.Id, 2));
        GameEvent b = Incident(insured, m);
        GameEvent claim = insured.State.Events.Last(e => e.Kind == EventKind.InsuranceClaimPaid);
        Assert.Equal(MathI.MulPermille(a.Amount, 500), claim.Amount);
        Assert.Equal(a.Amount - claim.Amount, b.Amount);
        Assert.Equal(1, insured.Me.InsuranceClaims);
        Assert.Contains("insurer paid", EventText.Describe(insured.C, insured.State, claim));
    }

    [Fact]
    public void The_top_level_never_covers_more_than_the_fine()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Finance.InsuranceMaxLevel * c.Finance.InsuranceCoverPermille <= 1000);
        c.Finance.InsuranceCoverPermille = 400;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("would cover more than the fine"));
    }

    [Fact]
    public void An_incident_at_an_excluded_rung_is_not_covered_and_is_not_a_claim()
    {
        Game g = Lab(c => c.Finance.InsuranceExcludedRung = 1);
        ModelState m = Released(g);
        g.Do(Command.SetInsurance(g.Id, 3));
        Incident(g, m);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.InsuranceClaimPaid);
        Assert.Equal(0, g.Me.InsuranceClaims);
    }

    [Fact]
    public void The_third_claim_ends_the_policy_but_still_pays_it()
    {
        Game g = Lab();
        ModelState m = Released(g);
        g.Do(Command.SetInsurance(g.Id, 1));
        g.Me.InsuranceClaims = 2;
        Incident(g, m);
        Assert.Equal(0, g.Me.InsuranceLevel);
        Assert.Equal(3, g.Me.InsuranceClaims);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.InsuranceClaimPaid);
        GameEvent dropped = g.State.Events.Last(e => e.Kind == EventKind.InsuranceDropped);
        Assert.Equal(3, dropped.Amount);
        Assert.Contains("ended", EventText.Describe(g.C, g.State, dropped));
        g.Weeks(1);
        Assert.Equal(0, Insurance.Premium(g.C, g.Me));
    }

    [Fact]
    public void An_uninsured_lab_makes_no_claims()
    {
        Game g = Lab();
        ModelState m = Released(g);
        Incident(g, m);
        Assert.Equal(0, g.Me.InsuranceClaims);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.InsuranceClaimPaid);
    }

    [Fact]
    public void A_claim_is_forgotten_every_forget_period()
    {
        Game g = Lab();
        g.Me.InsuranceClaims = 2;
        g.Weeks(52);
        Assert.Equal(1, g.Me.InsuranceClaims);
        g.Weeks(52);
        Assert.Equal(0, g.Me.InsuranceClaims);
        g.Weeks(52);
        Assert.Equal(0, g.Me.InsuranceClaims);
    }

    // ---- save, load, replay

    [Fact]
    public void A_policy_with_claims_saves_and_loads_back_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetInsurance(g.Id, 2));
        g.Me.InsuranceClaims = 1;
        g.Weeks(20);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(2, back.Player.InsuranceLevel);
        Assert.Equal(g.Me.Ledger.Lifetime.Insurance, back.Player.Ledger.Lifetime.Insurance);
    }

    [Fact]
    public void The_journal_replays_to_the_same_game_with_a_policy()
    {
        Game g = Lab(rich: false);
        g.Do(Command.SetInsurance(g.Id, 1));
        g.Weeks(30);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_schema_44_save_loads_with_no_policy_and_a_zero_premium_line()
    {
        Game g = Lab();
        g.Weeks(6);
        string old = SaveSchemaPretend.At(g.C, 43, g.Sim.Save());
        Assert.DoesNotContain("insuranceLevel", old);
        Assert.DoesNotContain("\"insurance\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.InsuranceLevel);
        Assert.Equal(0, sim.Player.Ledger.Lifetime.Insurance);
        Assert.Equal(0, sim.Player.Ledger.ThisWeek.Insurance);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_an_impossible_policy_is_reported()
    {
        Game g = Lab();
        g.Me.InsuranceLevel = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("incident insurance"));
        g.Me.InsuranceLevel = 0;
        g.Me.InsuranceClaims = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("incident insurance"));
        g.Me.InsuranceClaims = 0;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("incident insurance"));
    }

    [Fact]
    public void The_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Finance.InsuranceMaxLevel);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Finance.InsuranceMaxLevel = 11;
        c.Finance.InsuranceMinPremium = -1;
        c.Finance.InsuranceForgetWeeks = 0;
        c.Finance.InsuranceDropClaims = 0;
        c.Finance.InsuranceExcludedRung = 0;
        c.Finance.InsuranceLockWeeks = 521;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "insuranceMaxLevel", "insuranceMinPremium", "insuranceForgetWeeks", "insuranceDropClaims", "insuranceExcludedRung", "insuranceLockWeeks" })
            Assert.Contains(problems, p => p.Contains(key));
    }

    [Fact]
    public void The_describe_query_gives_the_screen_what_it_needs()
    {
        Game g = Lab();
        Insurance.Quote none = Insurance.Describe(g.C, g.Me);
        Assert.Equal(0, none.Level);
        Assert.True(none.CanBuyMore);
        g.Do(Command.SetInsurance(g.Id, 3));
        g.Me.Ledger.LastWeek.Revenue = 0;
        Insurance.Quote top = Insurance.Describe(g.C, g.Me);
        Assert.Equal(750, top.CoverPermille);
        Assert.Equal(6000, top.WeeklyPremium);
        Assert.False(top.CanBuyMore);
        Assert.Equal(g.Me.InsuranceLockedUntil, top.LockedUntilTick);
    }
}
