using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TakeoffCrisisEraGraceTests
{
    static Game Lab()
    {
        Game g = Game.Solo(97, c =>
        {
            c.TakeoffCrisis.MinEra = 0;
            c.TakeoffCrisis.CapabilityMin = 100;
            c.TakeoffCrisis.CorruptionMin = 100;
            c.Office.ChancePermille = 0;
            c.TakeoffCrisisEraGrace.Enabled = 1;
            c.TakeoffCrisisEraGrace.MinEra = 0;
            c.TakeoffCrisisEraGrace.GraceWeeks = 3;
        }).WithStarterLab(gpus: 4);
        return g;
    }

    [Fact]
    public void Takeoff_pose_gets_extra_expire_weeks()
    {
        Game g = Lab();
        int baseExpire = g.Tick + g.C.Office.ExpireWeeks;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 500;
        OfficeEvents.OnTakeoff(g.C, g.State, g.Me);
        Assert.True(g.Me.OfficeEventExpires > baseExpire);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.TakeoffCrisisEraGraced);
    }
}