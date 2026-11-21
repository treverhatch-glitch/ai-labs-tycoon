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

/// <summary>Named starts: an archetype in a specific year, ready to pick.</summary>
public class ScenarioTests
{
    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    [Fact]
    public void The_shipped_scenarios_validate_and_cover_several_eras_and_founders()
    {
        GameConfig c = TestData.Config();
        List<ScenarioConfig> list = Scenarios.List(c);
        Assert.True(list.Count >= 6);
        Assert.True(list.Select(s => s.EraId).Distinct().Count() >= 5, "a spread of years");
        Assert.True(list.Select(s => s.FounderId).Distinct().Count() >= 4, "every founder gets one");
        Assert.All(list, s => Assert.False(string.IsNullOrWhiteSpace(s.Blurb) || string.IsNullOrWhiteSpace(s.LabName)));
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_validator_catches_bad_scenarios()
    {
        AssertProblem(Problems(c => c.Scenarios.Scenarios[0].EraId = "nope"), "unknown era");
        AssertProblem(Problems(c => c.Scenarios.Scenarios[0].FounderId = "nope"), "unknown founder");
        AssertProblem(Problems(c => c.Scenarios.Scenarios[0].StyleId = "nope"), "unknown style");
        AssertProblem(Problems(c => c.Scenarios.Scenarios[0].HomeBlocId = "nope"), "unknown home bloc");
        AssertProblem(Problems(c => c.Scenarios.Scenarios[0].RivalIds.Add("nope")), "unknown rival");
        AssertProblem(Problems(c => c.Scenarios.Scenarios[0].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Scenarios.Scenarios[0].Blurb = ""), "has no blurb");
        AssertProblem(Problems(c => c.Scenarios.Scenarios[1].IdValue = c.Scenarios.Scenarios[0].IdValue), "scenarios");
        AssertProblem(Problems(c =>
        {
            EraConfig era = c.Era(c.Scenarios.Scenarios[0].EraId);
            era.Startable = false;
        }), "not a starting era");
    }

    [Fact]
    public void Every_scenario_starts_a_game_as_described()
    {
        GameConfig c = TestData.Config();
        foreach (ScenarioConfig s in Scenarios.List(c))
        {
            Simulation sim = Scenarios.Start(c, s.IdValue, 7);
            LabState me = sim.Player;
            Assert.Equal(s.LabName, me.Name);
            Assert.Equal(s.HomeBlocId, me.HomeBlocId);
            Assert.Equal(s.StyleId, me.StyleId);
            Assert.Equal(s.EraId, sim.Era.Id);
            Assert.Equal(c.Era(s.EraId).StartYear, sim.Year);
            Assert.Empty(SaveGame.Problems(c, sim.State));
            if (s.RivalIds.Count > 0)
                Assert.Equal(s.RivalIds.Count, sim.State.Labs.Count(l => l.Controller != LabController.Player) + sim.State.PendingRivals.Count(r => s.RivalIds.Contains(r)));
        }
    }

    [Fact]
    public void The_options_carry_every_choice_and_the_seed()
    {
        GameConfig c = TestData.Config();
        ScenarioConfig s = c.Scenarios.Scenario("defectors-lawsuit");
        NewGameOptions o = Scenarios.Options(c, s.IdValue, 99);
        Assert.Equal(99, o.Seed);
        Assert.Equal(s.EraId, o.StartEraId);
        Assert.Equal(s.FounderId, o.FounderId);
        Assert.Equal(s.StyleId, o.StyleId);
        Assert.Equal(s.HomeBlocId, o.HomeBlocId);
        Assert.Equal(s.LabName, o.LabName);
        Assert.Equal(s.Leader, o.Leader);
        Assert.Equal(s.FamilyName, o.FamilyName);
        Assert.Null(o.RivalIds);
        Assert.Equal(new List<string> { "openaye", "deepsneak" }, Scenarios.Options(c, "two-horse-race", 1).RivalIds);
    }

    [Fact]
    public void An_unknown_scenario_is_the_callers_mistake()
    {
        Assert.Throws<ArgumentException>(() => Scenarios.Options(TestData.Config(), "nope", 1));
        Assert.Throws<ArgumentException>(() => Scenarios.Start(TestData.Config(), "nope", 1));
    }

    [Fact]
    public void The_same_scenario_and_seed_is_the_same_game_and_a_new_seed_a_different_one()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(Scenarios.Start(c, "heirs-gambit", 3).Digest(), Scenarios.Start(c, "heirs-gambit", 3).Digest());
        Assert.NotEqual(Scenarios.Start(c, "heirs-gambit", 3).Digest(), Scenarios.Start(c, "heirs-gambit", 4).Digest());
    }

    [Fact]
    public void The_founder_and_the_era_do_what_they_always_did()
    {
        GameConfig c = TestData.Config();
        Simulation sim = Scenarios.Start(c, "heirs-gambit", 1);
        Simulation plain = Simulation.NewGame(c, new NewGameOptions { Seed = 1, StartEraId = "chat-boom", FounderId = "heir", StyleId = "shotgun", HomeBlocId = "gulf", LabName = "Old Money Labs", Leader = "Alexandra", FamilyName = "Money" });
        Assert.Equal(plain.Digest(), sim.Digest());
        Assert.True(sim.Player.Cash > Simulation.NewGame(c, new NewGameOptions { Seed = 1, StartEraId = "chat-boom" }).Player.Cash, "the heir has the money");
    }

    [Fact]
    public void A_scenario_game_runs_for_a_year()
    {
        GameConfig c = TestData.Config();
        foreach (string id in new[] { "dial-up-prologue", "professors-grant", "nordic-quiet" })
        {
            var g = new Game(Scenarios.Start(c, id, 5));
            g.Weeks(52);
            Assert.Empty(SaveGame.Problems(c, g.State));
        }
    }
}
