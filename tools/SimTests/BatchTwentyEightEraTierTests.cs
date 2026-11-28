using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Schema 183–185 edge paths (Q519–Q521).</summary>
public class BatchTwentyEightEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(142, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PublicMetrics.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Save_load_preserves_schema_185_round_trip()
    {
        Game g = Lab(c =>
        {
            c.PublicMetricsEraDiscount.Enabled = 1;
            c.PublicMetricsEraDiscount.MinEra = 0;
            c.PublicMetrics.PublishCost = 100;
            c.PublicMetrics.Enabled = 1;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        string save = g.Sim.Save();
        Assert.Equal(188, g.State.Schema);
        Simulation loaded = Simulation.Load(g.C, save);
        Assert.Equal(188, loaded.State.Schema);
        Assert.Contains(loaded.State.Events, e => e.Kind == EventKind.PublicMetricsEraDiscounted);
    }

    [Fact]
    public void Zero_enabled_leaves_paths_unchanged()
    {
        Game g = Lab(c =>
        {
            c.RungSixEraGrace.Enabled = 0;
            c.EvalHarnessEraRelief.Enabled = 0;
            c.PublicMetricsEraDiscount.Enabled = 0;
        });
        Assert.Equal(g.C.Office.ExpireWeeks, RungSixEraGrace.AnswerWeeks(g.C, g.State));
        Assert.Equal(1000L, EvalHarnessEraRelief.RegisterCost(g.C, g.State, 1000));
        Assert.Equal(1000L, PublicMetricsEraDiscount.PublishCost(g.C, g.State, 1000));
    }
}