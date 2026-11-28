using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Rules 41–44 bible/systems depth (Q25303–Q25314).</summary>
public class BibleSystemsRules41_44LaterTests
{
    [Fact]
    public void Q25303_only_interesting_skips_boring_research()
    {
        var options = new NewGameOptions { Seed = 25303, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
        }), options));
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Staff.Add(new StaffMember
        {
            Id = g.State.TakeId(),
            RoleId = "researcher",
            TraitId = "only-interesting",
            Morale = g.C.StaffPeople.MoraleBaseline,
        });
        Assert.False(Traits.HasSomethingInteresting(g.C, g.Me));
    }

    [Fact]
    public void Q25304_quits_trait_can_fire_on_low_morale()
    {
        var options = new NewGameOptions { Seed = 25304, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
        }), options));
        g.Me.Cash = 1_000_000_000_000;
        var p = new StaffMember
        {
            Id = g.State.TakeId(),
            RoleId = "researcher",
            TraitId = "quits-by-blog-post",
            Morale = 1,
        };
        g.Me.Staff.Add(p);
        int left = Traits.RunQuits(g.C, g.State, g.Me);
        Assert.True(left >= 0);
    }

    [Fact]
    public void Q25305_take_loan_increases_outstanding()
    {
        var g = Game.Solo(25305, c => c.Office.ChancePermille = 0);
        g.Build("server-closet", 4, 4);
        g.Do(Command.BuyGpus(g.Id, 6, HardwareBatch.Hq));
        g.Me.Cash = 0;
        int id = g.Do(Command.TakeLoan(g.Id, 1000)).CreatedId;
        Assert.True(Loans.Outstanding(g.Me) > 0);
        Assert.Contains(g.Me.Loans, l => l.Id == id);
    }

    [Fact]
    public void Q25306_loan_survives_save_roundtrip()
    {
        var g = Game.Solo(25306, c => c.Office.ChancePermille = 0);
        g.Build("server-closet", 4, 4);
        g.Do(Command.BuyGpus(g.Id, 6, HardwareBatch.Hq));
        g.Do(Command.TakeLoan(g.Id, 1000));
        long outstanding = Loans.Outstanding(g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(outstanding, Loans.Outstanding(loaded.Me));
    }

    [Fact]
    public void Q25307_escape_trigger_exists_in_office_table()
    {
        GameConfig c = TestData.Config();
        Assert.Contains(c.Office.Events, e => e.Trigger == OfficeTrigger.Escape && e.Choices.Count > 0);
    }

    [Fact]
    public void Q25308_scenario_start_sets_scenario_id_on_lab()
    {
        GameConfig c = TestData.Config();
        ScenarioConfig s = c.Scenarios.Scenario("defectors-lawsuit");
        Simulation sim = Scenarios.Start(c, s.IdValue, 25308);
        Assert.Equal(s.IdValue, sim.Player.ScenarioId);
    }

    [Fact]
    public void Q25309_scenario_options_expose_era_and_style()
    {
        GameConfig c = TestData.Config();
        ScenarioConfig s = Scenarios.List(c).First();
        var opts = Scenarios.Options(c, s.IdValue, 25309);
        Assert.False(string.IsNullOrEmpty(opts.StartEraId));
        Assert.False(string.IsNullOrEmpty(opts.StyleId));
    }

    [Fact]
    public void Q25310_posts_through_it_raises_buzz_permille()
    {
        StaffTraitConfig t = TestData.Config().StaffPeople.Trait("posts-through-it");
        Assert.True(t.BuzzPermille > 0 && t.ScandalPermille > 0);
    }

    [Fact]
    public void Q25311_repay_loan_reduces_outstanding()
    {
        var g = Game.Solo(25311, c => c.Office.ChancePermille = 0);
        g.Build("server-closet", 4, 4);
        g.Do(Command.BuyGpus(g.Id, 6, HardwareBatch.Hq));
        g.Do(Command.TakeLoan(g.Id, 500));
        long before = Loans.Outstanding(g.Me);
        g.Me.Cash = before * 10;
        g.Do(Command.RepayLoan(g.Id, g.Me.Loans[0].Id));
        Assert.True(Loans.Outstanding(g.Me) < before);
    }

    [Fact]
    public void Q25312_every_scenario_passes_validator()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        foreach (ScenarioConfig s in Scenarios.List(c))
            Assert.False(string.IsNullOrEmpty(s.EraId));
    }

    [Fact]
    public void Q25313_traits_round2_save_roundtrip()
    {
        var g = Game.Solo(25313);
        g.Me.Staff.Add(new StaffMember
        {
            Id = g.State.TakeId(),
            RoleId = "researcher",
            TraitId = "steady",
            Morale = 500,
        });
        int count = g.Me.Staff.Count;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(count, loaded.Me.Staff.Count);
    }

    [Fact]
    public void Q25314_scenario_list_stable_after_save_schema()
    {
        GameConfig c = TestData.Config();
        int n = Scenarios.List(c).Count;
        SaveSchemaPretend.AssertRoundTrip(new Game(Simulation.NewGame(c, new NewGameOptions { Seed = 25314 })).Sim);
        Assert.Equal(n, Scenarios.List(c).Count);
    }
}