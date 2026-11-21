using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q124–Q125: patent filing cost parts and fork sibling preview.</summary>
public class ResearchForkPatentBreakdownTests
{
    [Fact]
    public void Patent_cost_parts_match_filing_cost()
    {
        GameConfig c = TestData.Config();
        TechConfig tech = c.Tech("bayes-nets");
        var b = ResearchQueries.PatentFilingCostParts(c, tech);
        Assert.Equal(tech.Cost, b.TechCost);
        Assert.Equal(c.Research.Patents.FilingCostPermille, b.FilingPermille);
        Assert.Equal(ResearchQueries.PatentFilingCost(c, tech), b.Total);
    }

    [Fact]
    public void Patent_parts_mul_permille_math()
    {
        GameConfig c = TestData.Config();
        TechConfig tech = c.Tech("minimax");
        var b = ResearchQueries.PatentFilingCostParts(c, tech);
        Assert.Equal(b.TechCost * b.FilingPermille / 1000, b.Total);
    }

    [Fact]
    public void Null_tech_yields_zero_patent_parts()
    {
        var b = ResearchQueries.PatentFilingCostParts(TestData.Config(), null);
        Assert.Equal(0, b.Total);
    }

    [Fact]
    public void Fork_siblings_lists_other_path_before_pick()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(20_000);
        g.Research("rules-engines");
        g.Research("decision-trees");
        var list = ResearchQueries.ForkSiblingsToBlock(g.C, g.Me, "bayes-nets");
        Assert.Contains("svm", list);
        Assert.DoesNotContain("bayes-nets", list);
    }

    [Fact]
    public void Fork_apply_blocks_sibling_from_preview()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(20_000);
        g.Research("rules-engines");
        g.Research("decision-trees");
        var before = ResearchQueries.ForkSiblingsToBlock(g.C, g.Me, "bayes-nets");
        Assert.Contains("svm", before);
        g.Research("bayes-nets");
        Assert.Empty(ResearchQueries.ForkSiblingsToBlock(g.C, g.Me, "bayes-nets"));
        Assert.Contains("svm", g.Me.BlockedTechs);
    }

    [Fact]
    public void Research_command_refuses_blocked_fork_path()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(20_000);
        g.Research("rules-engines");
        g.Research("decision-trees");
        g.Research("bayes-nets");
        g.Refused(Command.ResearchTech(g.Id, "svm"), CommandError.TechForkBlocked);
    }

    [Fact]
    public void Already_owned_sibling_not_in_preview()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(20_000);
        g.Research("rules-engines");
        g.Research("decision-trees");
        g.Research("bayes-nets");
        Assert.Empty(ResearchQueries.ForkSiblingsToBlock(g.C, g.Me, "svm"));
        Assert.Contains("svm", g.Me.BlockedTechs);
    }

    [Fact]
    public void Non_fork_tech_empty_siblings()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Empty(ResearchQueries.ForkSiblingsToBlock(g.C, g.Me, "minimax"));
    }

    [Fact]
    public void Save_load_fork_blocks_persist()
    {
        Game g = Game.Solo().WithStarterLab();
        g.GrantResearch(20_000);
        g.Research("rules-engines");
        g.Research("decision-trees");
        g.Research("bayes-nets");
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.True(ResearchQueries.IsBlocked(loaded.Me, "svm"));
    }

    [Fact]
    public void File_patent_uses_cost_parts_total()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 5_000_000;
        g.GrantResearch(5000);
        g.Research("state-machines");
        g.Research("minimax");
        TechConfig tech = g.C.Tech("minimax");
        long cost = ResearchQueries.PatentFilingCostParts(g.C, tech).Total;
        long before = g.Me.Cash;
        g.Do(Command.FilePatent(g.Id, "minimax"));
        Assert.Equal(before - cost, g.Me.Cash);
    }

    [Fact]
    public void Classical_ml_fork_has_two_branches()
    {
        GameConfig c = TestData.Config();
        ResearchForkConfig fork = ResearchQueries.ForkFor(c, "bayes-nets");
        Assert.NotNull(fork);
        Assert.Equal(2, fork.Techs.Count);
    }

    [Fact]
    public void Fork_siblings_respect_prior_blocks()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.BlockedTechs.Add("svm");
        var list = ResearchQueries.ForkSiblingsToBlock(g.C, g.Me, "bayes-nets");
        Assert.Empty(list);
    }

    [Fact]
    public void Patent_permille_change_updates_parts()
    {
        Game g = Game.Solo();
        TechConfig tech = g.C.Tech("minimax");
        g.C.Research.Patents.FilingCostPermille = 500;
        var b = ResearchQueries.PatentFilingCostParts(g.C, tech);
        Assert.Equal(500, b.FilingPermille);
        Assert.Equal(ResearchQueries.PatentFilingCost(g.C, tech), b.Total);
    }
}