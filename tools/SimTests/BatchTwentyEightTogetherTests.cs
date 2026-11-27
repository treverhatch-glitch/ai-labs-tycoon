using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q522 combination wiring for batch twenty-eight systems.</summary>
public class BatchTwentyEightTogetherTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(132, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.RungSixCrisis.MinEra = 0;
            c.EvalHarness.MinEra = 0;
            c.PublicMetrics.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 500;
        return g;
    }

    [Fact]
    public void Rung_six_gets_era_grace()
    {
        Game g = Lab(c =>
        {
            c.RungSixEraGrace.Enabled = 1;
            c.RungSixEraGrace.MinEra = 0;
            c.RungSixEraGrace.GraceWeeks = 5;
            c.RungSixCrisis.Enabled = 1;
            c.RungSixCrisis.MinEra = 0;
            c.RungSixCrisis.CorruptionMin = 100;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Corruption = 800;
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        RungSixCrisis.RunWeek(g.C, g.State, g.Me);
        Assert.True(OfficeEvents.IsPending(g.Me));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RungSixEraGraced);
        Assert.Equal(g.Tick + g.C.Office.ExpireWeeks + 5, g.Me.OfficeEventExpires);
    }

    [Fact]
    public void Eval_register_era_relief()
    {
        Game g = Lab(c =>
        {
            c.EvalHarness.Enabled = 1;
            c.EvalHarness.Cost = 1000;
            c.EvalHarnessEraRelief.Enabled = 1;
            c.EvalHarnessEraRelief.MinEra = 0;
            c.EvalHarnessEraRelief.ReliefPermille = 200;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        long cash = g.Me.Cash;
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        Assert.Equal(cash - 800, g.Me.Cash);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EvalHarnessEraRelieved);
    }

    [Fact]
    public void Public_metrics_publish_discount()
    {
        Game g = Lab(c =>
        {
            c.PublicMetrics.Enabled = 1;
            c.PublicMetrics.PublishCost = 500;
            c.PublicMetricsEraDiscount.Enabled = 1;
            c.PublicMetricsEraDiscount.MinEra = 0;
            c.PublicMetricsEraDiscount.DiscountPermille = 250;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        long cash = g.Me.Cash;
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        Assert.Equal(cash - 375, g.Me.Cash);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PublicMetricsEraDiscounted);
    }
}