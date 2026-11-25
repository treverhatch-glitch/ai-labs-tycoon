using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DataSourcesTests
{
    static Game Lab()
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.DataSources.MinEra = 0;
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Acquire_and_weekly_run_tick_sources()
    {
        Game g = Lab();
        g.Do(Command.AcquireDataSource(g.Id, "licensed-corpus"));
        Assert.True(g.Me.SrcWeeks0 > 0);
        int before = g.Me.SrcWeeks0;
        g.Weeks(1);
        Assert.Equal(before - 1, g.Me.SrcWeeks0);
    }

    [Fact]
    public void Older_save_has_no_source_weeks()
    {
        Game g = Lab();
        g.Do(Command.AcquireDataSource(g.Id, "licensed-corpus"));
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 134, g.Sim.Save()));
        Assert.Equal(0, older.Player.SrcWeeks0 + older.Player.SrcWeeks1);
    }
}