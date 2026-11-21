using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Research forks, dead-end tech, and patents (Q51).</summary>
public class ResearchForkPatentTests
{
    [Fact]
    public void Shipped_config_lists_forks_patents_and_dead_end()
    {
        GameConfig c = TestData.Config();
        Assert.Single(c.Research.Forks);
        Assert.Equal("classical-ml", c.Research.Forks[0].Id);
        Assert.Equal(52, c.Research.Patents.DurationWeeks);
        TechConfig dead = c.Tech("go-fish-heuristic");
        Assert.NotNull(dead);
        Assert.True(dead.IsDeadEnd);
        Assert.False(dead.Patentable);
    }

    [Fact]
    public void Classical_ml_fork_blocks_sibling()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(20_000);
        g.Research("rules-engines");
        g.Research("decision-trees");
        g.Research("bayes-nets");
        Assert.Contains("svm", g.Me.BlockedTechs);
        g.Refused(Command.ResearchTech(g.Id, "svm"), CommandError.TechForkBlocked);
    }

    [Fact]
    public void Dead_end_go_fish_is_researchable()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(2000);
        g.Research("minimax");
        g.Research("go-fish-heuristic");
        Assert.Contains("go-fish-heuristic", g.Me.Techs);
    }

    [Fact]
    public void File_patent_after_research()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 1_000_000;
        g.GrantResearch(500);
        g.Research("state-machines");
        long cost = ResearchQueries.PatentFilingCost(g.C, g.C.Tech("state-machines"));
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        Assert.Single(g.Me.Patents);
        Assert.Equal("state-machines", g.Me.Patents[0].TechId);
        Assert.Equal(g.Tick + g.C.Research.Patents.DurationWeeks, g.Me.Patents[0].ExpiresTick);
        Assert.Equal(1_000_000 - cost, g.Me.Cash);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PatentFiled && e.Ref == "state-machines");
    }

    [Fact]
    public void Patent_refusals()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 1_000_000;
        g.GrantResearch(2000);
        g.Refused(Command.FilePatent(g.Id, "minimax"), CommandError.MissingPrerequisite);
        g.Research("minimax");
        g.Research("go-fish-heuristic");
        g.Refused(Command.FilePatent(g.Id, "go-fish-heuristic"), CommandError.NotPatentable);
        g.Do(Command.FilePatent(g.Id, "minimax"));
        g.Refused(Command.FilePatent(g.Id, "minimax"), CommandError.PatentAlreadyFiled);
    }

    [Fact]
    public void Active_patent_blocks_rival_research()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        g.Me.Cash = 2_000_000;
        rival.Cash = 2_000_000;
        g.GrantResearch(500);
        rival.ResearchPoints = 500;
        g.Research("state-machines");
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        g.Refused(Command.ResearchTech(rival.Id, "state-machines"), CommandError.PatentBlocked);
    }

    [Fact]
    public void Patent_expires_after_duration()
    {
        Game g = Game.Solo(seed: 2, tweak: c => c.Research.Patents.DurationWeeks = 3);
        g.WithStarterLab();
        g.Me.Cash = 500_000;
        g.GrantResearch(500);
        g.Research("state-machines");
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        int filedTick = g.Tick;
        g.Weeks(4);
        Assert.Empty(g.Me.Patents);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PatentExpired && e.Ref == "state-machines" && e.Tick > filedTick);
        Assert.Contains("state-machines", g.Me.Techs);
    }

    [Fact]
    public void Plan_lab_cannot_file_patent()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        rival.Techs.Add("state-machines");
        g.Refused(Command.FilePatent(rival.Id, "state-machines"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Save_round_trip_keeps_fork_and_patent_fields()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 500_000;
        g.GrantResearch(50_000);
        g.Research("rules-engines");
        g.Research("decision-trees");
        g.Research("bayes-nets");
        g.Research("state-machines");
        g.Do(Command.FilePatent(g.Id, "state-machines"));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Contains("svm", loaded.Me.BlockedTechs);
        Assert.Single(loaded.Me.Patents);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
    }

    [Fact]
    public void Migrate_18_to_19_adds_fork_and_patent_arrays()
    {
        Game g = Game.Solo();
        string json = g.Sim.Save().Replace("\"schema\":25", "\"schema\":18");
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(25, loaded.State.Schema);
        Assert.NotNull(loaded.Me.BlockedTechs);
        Assert.NotNull(loaded.Me.Patents);
    }

    [Fact]
    public void Schema_22_on_new_game()
    {
        Assert.Equal(25, Game.Solo().State.Schema);
    }

    [Fact]
    public void Config_validator_rejects_broken_fork()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Research.Forks.Add(new ResearchForkConfig { IdValue = "dup", Techs = new System.Collections.Generic.List<string> { "state-machines" } });
        var problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, s => s.Contains("needs at least two techs"));
    }

    [Fact]
    public void Patent_filing_cost_uses_permille()
    {
        GameConfig c = TestData.Config(cfg => cfg.Research.Patents.FilingCostPermille = 1000);
        TechConfig tech = c.Tech("state-machines");
        Assert.Equal(tech.Cost, ResearchQueries.PatentFilingCost(c, tech));
    }

    [Fact]
    public void Research_queries_detect_holder()
    {
        Game g = Game.Solo();
        g.Me.Patents.Add(new PatentState { TechId = "state-machines", FiledTick = 0, ExpiresTick = 100 });
        Assert.True(ResearchQueries.PatentedElsewhere(g.C, g.State, g.Id + 99, "state-machines", out LabState holder));
        Assert.Equal(g.Me, holder);
        Assert.NotNull(ResearchQueries.ActivePatent(g.Me, "state-machines", 50));
        Assert.Null(ResearchQueries.ActivePatent(g.Me, "state-machines", 100));
    }
}