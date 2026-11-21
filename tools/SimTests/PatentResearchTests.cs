using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q92: active patents grant weekly research points.</summary>
public class PatentResearchTests
{
    static Game WithPatent(string tech = "minimax")
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 2_000_000;
        g.GrantResearch(5_000);
        g.Research("state-machines");
        if (tech != "state-machines") g.Research(tech);
        g.Do(Command.FilePatent(g.Id, tech));
        return g;
    }

    [Fact]
    public void Shipped_patent_research_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(8, c.Research.Patents.ResearchPerActivePatent);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_patents_no_research_gain()
    {
        Game g = Game.Solo().WithStarterLab();
        long before = g.Me.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void One_active_patent_grants_research()
    {
        Game g = WithPatent();
        long before = g.Me.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Research.Patents.ResearchPerActivePatent, g.Me.ResearchPoints);
    }

    [Fact]
    public void Two_patents_double_weekly_research()
    {
        Game g = WithPatent("state-machines");
        g.Research("minimax");
        g.Do(Command.FilePatent(g.Id, "minimax"));
        long before = g.Me.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 2 * g.C.Research.Patents.ResearchPerActivePatent, g.Me.ResearchPoints);
    }

    [Fact]
    public void Zero_research_config_disables_hook()
    {
        Game g = WithPatent();
        g.C.Research.Patents.ResearchPerActivePatent = 0;
        long before = g.Me.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
    }

    [Fact]
    public void Research_stacks_with_reputation_grant()
    {
        Game g = WithPatent();
        long rp = g.Me.ResearchPoints;
        int rep = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.ResearchPoints > rp);
        Assert.True(g.Me.Reputation > rep);
    }

    [Fact]
    public void Expired_patent_stops_research_stipend()
    {
        Game g = WithPatent();
        g.Me.Patents[0].ExpiresTick = g.Tick;
        long before = g.Me.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.ResearchPoints);
        Assert.Empty(g.Me.Patents);
    }

    [Fact]
    public void Sim_week_runs_patent_research()
    {
        Game g = WithPatent();
        g.GrantResearch(0);
        foreach (StaffMember p in g.Me.Staff) p.RoleId = "sales";
        long before = g.Me.ResearchPoints;
        g.Weeks(1);
        Assert.True(g.Me.ResearchPoints >= before + g.C.Research.Patents.ResearchPerActivePatent);
    }

    [Fact]
    public void Save_load_preserves_patent_research()
    {
        Game g = WithPatent();
        g.Weeks(1);
        long points = g.Me.ResearchPoints;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Weeks(1);
        Assert.True(loaded.Me.ResearchPoints > points);
    }

    [Fact]
    public void Bankrupt_lab_still_accrues_patent_research()
    {
        Game g = WithPatent();
        g.Me.Bankrupt = true;
        long before = g.Me.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Research.Patents.ResearchPerActivePatent, g.Me.ResearchPoints);
    }

    [Fact]
    public void Active_patent_count_matches_research_multiply()
    {
        Game g = WithPatent();
        Assert.Equal(1, PatentSystem.ActivePatentCount(g.Me, g.Tick));
        long before = g.Me.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Research.Patents.ResearchPerActivePatent, g.Me.ResearchPoints);
    }

    [Fact]
    public void Plan_lab_patents_unaffected_when_player_hooks_run()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        long before = plan.ResearchPoints;
        PatentSystem.RunWeek(g.C, g.State, plan);
        Assert.Equal(before, plan.ResearchPoints);
    }

    [Fact]
    public void Negative_research_config_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Research.Patents.ResearchPerActivePatent = -1));
    }
}