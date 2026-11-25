using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class FoundationTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Cash = 1_000_000_000;
        return g;
    }

    static Game Funded(int focus, int payout = 2, System.Action<GameConfig> more = null)
    {
        Game g = Lab(more);
        g.Do(Command.EndowFoundation(g.Id, 1000, focus));
        g.Do(Command.SetFoundationPayout(g.Id, payout));
        return g;
    }

    [Fact]
    public void The_first_gift_sets_the_cause_and_takes_cash_through_the_capital_line()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.EndowFoundation(g.Id, 500, Foundation.Safety));
        Assert.Equal(500_000, g.Me.FoundFund);
        Assert.Equal(Foundation.Safety, g.Me.FoundFocus);
        Assert.Equal(cash - 500_000, g.Me.Cash);
        Assert.Equal(0, g.Me.FoundPayout);
        Assert.Equal(-500_000, g.Me.Ledger.Lifetime.CapitalIn);
        Assert.Equal(Foundation.Safety, g.State.Events.Last(e => e.Kind == EventKind.FoundationEndowed).SubjectId);
        g.Do(Command.EndowFoundation(g.Id, 1, -1));
        Assert.Equal(501_000, g.Me.FoundFund);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_gift_needs_a_cause_enough_money_and_a_game_with_foundations()
    {
        Game g = Lab();
        g.Refused(Command.EndowFoundation(g.Id, 1000, 0), CommandError.InvalidArgument);
        g.Refused(Command.EndowFoundation(g.Id, 1000, 4), CommandError.InvalidArgument);
        g.Refused(Command.EndowFoundation(g.Id, 99, 1), CommandError.InvalidArgument);
        g.Refused(Command.EndowFoundation(g.Id, 0, 1), CommandError.InvalidArgument);
        g.Me.Cash = 50_000;
        Assert.False(g.Try(Command.EndowFoundation(g.Id, 100, 1)).Ok);
        g.Refused(Command.SetFoundationPayout(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.SetFoundationFocus(g.Id, 2), CommandError.InvalidArgument);
        g.Refused(Command.WindUpFoundation(g.Id), CommandError.InvalidArgument);
        Game off = Lab(c => c.Foundation.Enabled = 0);
        off.Refused(Command.EndowFoundation(off.Id, 1000, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void The_fund_earns_gives_by_its_level_and_builds_goodwill_that_fades()
    {
        Game g = Funded(Foundation.Safety, 2);
        long fund = g.Me.FoundFund;
        Foundation.RunWeek(g.C, g.State, g.Me);
        long grown = fund + MathI.MulPermille(fund, g.C.Foundation.ReturnPermille);
        long gave = MathI.MulPermille(grown, g.C.Foundation.PayoutB);
        Assert.Equal(grown - gave, g.Me.FoundFund);
        Assert.Equal(gave, g.Me.FoundGiven);
        Assert.Equal((int)(gave / g.C.Foundation.GoodwillUnit), g.Me.FoundGoodwill);
        for (int i = 0; i < 300; i++) Foundation.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.FoundGoodwill <= g.C.Foundation.MaxGoodwill);
        g.Do(Command.SetFoundationPayout(g.Id, 0));
        int held = g.Me.FoundGoodwill;
        Foundation.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.FoundGoodwill < held);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_safety_foundation_damps_scandals_and_the_others_do_not()
    {
        Game g = Funded(Foundation.Safety, 3);
        Game bare = Lab();
        for (int i = 0; i < 100; i++) Foundation.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.FoundGoodwill > 100);
        Assert.Equal((int)MathI.Min(g.C.Foundation.MaxDamp, g.Me.FoundGoodwill * g.C.Foundation.SafetyDamp / 1000), Foundation.Damping(g.C, g.Me));
        Assert.True(PressSystem.Damp(g.C, g.Me, 100) < PressSystem.Damp(bare.C, bare.Me, 100));
        Game science = Funded(Foundation.OpenScience, 3);
        for (int i = 0; i < 100; i++) Foundation.RunWeek(science.C, science.State, science.Me);
        Assert.Equal(0, Foundation.Damping(science.C, science.Me));
    }

    [Fact]
    public void An_open_science_foundation_weakens_the_claims_filed_against_the_lab()
    {
        Game g = Funded(Foundation.OpenScience, 3, c => c.Litigation.Enabled = 1);
        Game bare = Lab(c => c.Litigation.Enabled = 1);
        for (int i = 0; i < 100; i++) Foundation.RunWeek(g.C, g.State, g.Me);
        var model = new ModelState { Name = "m", Incidents = 0 };
        Litigation.OnIncident(g.C, g.State, g.Me, model, 1_000_000);
        Litigation.OnIncident(bare.C, bare.State, bare.Me, model, 1_000_000);
        Assert.Equal(Foundation.ClaimStrength(g.C, g.Me), bare.Me.Cases.Last().Strength - g.Me.Cases.Last().Strength);
        Assert.True(Foundation.ClaimStrength(g.C, g.Me) > 0);
        Assert.Equal(0, Foundation.ClaimStrength(bare.C, bare.Me));
    }

    [Fact]
    public void An_education_foundation_slowly_lifts_reputation_once_it_has_the_goodwill()
    {
        Game g = Funded(Foundation.Education, 3);
        g.Me.Reputation = 300;
        for (int i = 0; i < 120; i++) Foundation.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.FoundGoodwill >= g.C.Foundation.EducationAt);
        Assert.True(g.Me.Reputation > 300);
        Game safe = Funded(Foundation.Safety, 3);
        safe.Me.Reputation = 300;
        for (int i = 0; i < 120; i++) Foundation.RunWeek(safe.C, safe.State, safe.Me);
        Assert.Equal(300, safe.Me.Reputation);
    }

    [Fact]
    public void Changing_the_cause_costs_goodwill_and_locks_the_choice()
    {
        Game g = Funded(Foundation.Safety, 3);
        g.Me.FoundGoodwill = 60;
        g.Refused(Command.SetFoundationFocus(g.Id, Foundation.Education), CommandError.InvalidArgument);
        g.Sim.StepMany(g.C.Foundation.LockWeeks);
        g.Refused(Command.SetFoundationFocus(g.Id, Foundation.Safety), CommandError.InvalidArgument);
        int goodwill = g.Me.FoundGoodwill;
        g.Do(Command.SetFoundationFocus(g.Id, Foundation.Education));
        Assert.Equal(Foundation.Education, g.Me.FoundFocus);
        Assert.Equal((int)MathI.MulPermille(goodwill, 1000 - g.C.Foundation.SwitchLoss), g.Me.FoundGoodwill);
        g.Refused(Command.SetFoundationFocus(g.Id, Foundation.OpenScience), CommandError.InvalidArgument);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.FoundationRefocused));
    }

    [Fact]
    public void Winding_up_returns_most_of_the_fund_and_loses_the_goodwill_and_some_reputation()
    {
        Game g = Funded(Foundation.Safety, 1);
        for (int i = 0; i < 30; i++) Foundation.RunWeek(g.C, g.State, g.Me);
        g.Me.Reputation = 400;
        long cash = g.Me.Cash;
        long back = Foundation.WindUpValue(g.C, g.Me);
        g.Do(Command.WindUpFoundation(g.Id));
        Assert.Equal(cash + back, g.Me.Cash);
        Assert.Equal(400 - g.C.Foundation.WindUpReputation, g.Me.Reputation);
        Assert.False(Foundation.Exists(g.Me));
        Assert.Equal(0, g.Me.FoundGoodwill);
        Assert.Equal(0, g.Me.FoundFocus);
        Assert.Equal(back, g.State.Events.Last(e => e.Kind == EventKind.FoundationWoundUp).Amount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Do(Command.EndowFoundation(g.Id, 200, Foundation.OpenScience));
    }

    [Fact]
    public void A_fund_that_gives_faster_than_it_earns_runs_dry_and_the_foundation_ends()
    {
        Game g = Funded(Foundation.Safety, 3, c => c.Foundation.ReturnPermille = 0);
        int weeks = 0;
        while (Foundation.Exists(g.Me) && weeks < 20000)
        {
            Foundation.RunWeek(g.C, g.State, g.Me);
            weeks++;
        }
        Assert.False(Foundation.Exists(g.Me));
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.FoundationRanDry));
        Assert.Equal(0, g.Me.FoundGiven);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_live_foundation_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Funded(Foundation.Education, 2);
        g.Weeks(60);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.FoundationReported);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 91, g.Sim.Save());
        Assert.DoesNotContain("foundFund", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).FoundFund);
    }

    [Fact]
    public void A_rival_foundation_and_bad_data_are_caught_and_the_data_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.FoundFund = 5;
        rival.FoundFocus = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("foundation"));
        rival.FoundFund = 0;
        rival.FoundFocus = 0;
        g.Me.FoundGoodwill = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("foundation"));
        g.Me.FoundGoodwill = 0;
        g.Me.FoundFund = 10;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("foundation"));
        g.Me.FoundFund = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Foundation.Enabled = 2;
        c.Foundation.MinGift = 0;
        c.Foundation.PayoutB = 0;
        c.Foundation.PayoutA = 5;
        c.Foundation.GoodwillUnit = 0;
        c.Foundation.MaxDamp = 900;
        c.Foundation.ReportWeeks = 0;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minGift", "payoutC", "goodwillUnit", "safetyDamp", "switchLoss" })
            Assert.Contains(problems, p => p.Contains("foundation." + key));
    }
}
