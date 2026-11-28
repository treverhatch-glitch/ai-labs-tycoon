using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q526 combination wiring for batch twenty-nine systems.</summary>
public class BatchTwentyNineTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(133, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.OpenWeights.MinEra = 0;
            c.EvalHarness.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 500;
        return g;
    }

    [Fact]
    public void Hearing_gets_era_grace()
    {
        Game g = Lab(c =>
        {
            c.OfficeHearingEraGrace.Enabled = 1;
            c.OfficeHearingEraGrace.MinEra = 0;
            c.OfficeHearingEraGrace.GraceWeeks = 4;
        });
        OfficeEvents.OnHearing(g.C, g.State, g.Me, g.Me.HomeBlocId);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OfficeHearingEraGraced);
        Assert.Equal(g.Tick + g.C.Office.ExpireWeeks + 4, g.Me.OfficeEventExpires);
    }

    [Fact]
    public void Open_weights_copy_era_relief()
    {
        Game g = Lab(c =>
        {
            c.OpenWeights.Enabled = 1;
            c.OpenWeights.CopyChancePermille = 1000;
            c.OpenWeights.CopyTrustLoss = 100;
            c.OpenWeightsCopyEraRelief.Enabled = 1;
            c.OpenWeightsCopyEraRelief.MinEra = 0;
            c.OpenWeightsCopyEraRelief.ReliefPermille = 200;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetWeightsPolicy(g.Id, m.Id, OpenWeights.Open));
        OpenWeights.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OpenWeightsCopyEraRelieved);
    }

    [Fact]
    public void Eval_fail_era_relief()
    {
        Game g = Lab(c =>
        {
            c.EvalHarness.Enabled = 1;
            c.EvalHarness.BarNarrow = 900;
            c.EvalHarness.BarBroad = 900;
            c.EvalHarness.TrustLoss = -40;
            c.EvalHarness.Cadence = 1;
            c.EvalFailEraRelief.Enabled = 1;
            c.EvalFailEraRelief.MinEra = 0;
            c.EvalFailEraRelief.ReliefPermille = 250;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 50;
        m.Alignment = 50;
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Weeks(1);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EvalFailEraRelieved);
    }
}