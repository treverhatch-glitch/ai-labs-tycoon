using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q81: active patents grant weekly reputation.</summary>
public class PatentReputationTests
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
    public void Shipped_patent_reputation_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Research.Patents.ReputationPerActivePatent);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_patents_no_reputation_gain()
    {
        Game g = Game.Solo().WithStarterLab();
        int before = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void One_active_patent_grants_reputation()
    {
        Game g = WithPatent();
        int before = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Research.Patents.ReputationPerActivePatent, g.Me.Reputation);
    }

    [Fact]
    public void Active_patent_count_query()
    {
        Game g = WithPatent();
        Assert.Equal(1, PatentSystem.ActivePatentCount(g.Me, g.Tick));
        g.Me.Patents[0].ExpiresTick = g.Tick;
        Assert.Equal(0, PatentSystem.ActivePatentCount(g.Me, g.Tick));
    }

    [Fact]
    public void Two_patents_double_weekly_gain()
    {
        Game g = WithPatent("state-machines");
        g.Research("minimax");
        g.Do(Command.FilePatent(g.Id, "minimax"));
        int before = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + 2 * g.C.Research.Patents.ReputationPerActivePatent, g.Me.Reputation);
    }

    [Fact]
    public void Zero_config_skips_reputation()
    {
        Game g = WithPatent();
        g.C.Research.Patents.ReputationPerActivePatent = 0;
        int before = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Reputation);
    }

    [Fact]
    public void Expired_patent_no_reputation_that_week()
    {
        Game g = WithPatent();
        g.Me.Patents[0].ExpiresTick = g.Tick;
        int before = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Reputation);
        Assert.Empty(g.Me.Patents);
    }

    [Fact]
    public void Reputation_clamped_at_permille()
    {
        Game g = WithPatent();
        g.Me.Reputation = 998;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1000, g.Me.Reputation);
    }

    [Fact]
    public void Sim_week_runs_patent_reputation()
    {
        Game g = WithPatent();
        int before = g.Me.Reputation;
        g.Weeks(1);
        Assert.Equal(before + g.C.Research.Patents.ReputationPerActivePatent, g.Me.Reputation);
    }

    [Fact]
    public void Save_load_preserves_patent_reputation_path()
    {
        Game g = WithPatent();
        int before = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(1, PatentSystem.ActivePatentCount(loaded.Me, loaded.State.Tick));
        PatentSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(before + 2 * g.C.Research.Patents.ReputationPerActivePatent, loaded.Me.Reputation);
    }

    [Fact]
    public void Bankrupt_lab_still_runs_patent_week_in_isolation()
    {
        Game g = WithPatent();
        g.Me.Bankrupt = true;
        int before = g.Me.Reputation;
        PatentSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Reputation >= before);
    }

    [Fact]
    public void Appeal_rises_after_patent_reputation_weeks()
    {
        Game g = Game.Solo().WithStarterLab(sales: 1);
        g.Me.Cash = 2_000_000;
        g.GrantResearch(5_000);
        g.Research("state-machines");
        g.Research("minimax");
        var model = g.Train("mini", 400);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Do(Command.FilePatent(g.Id, "minimax"));
        var sector = g.C.Sector("game-ai");
        var sectorState = g.State.Sector("game-ai");
        long appealBefore = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).ReputationBoost;
        g.Weeks(4);
        long appealAfter = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).ReputationBoost;
        Assert.True(appealAfter > appealBefore);
    }

    [Fact]
    public void Patent_count_matches_non_expired_rows()
    {
        Game g = WithPatent();
        g.Me.Patents.Add(new PatentState { TechId = "dead", FiledTick = 0, ExpiresTick = g.Tick - 1 });
        Assert.Equal(1, PatentSystem.ActivePatentCount(g.Me, g.Tick));
    }
}