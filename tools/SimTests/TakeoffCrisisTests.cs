using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TakeoffCrisisTests
{
    static Game Lab()
    {
        Game g = Game.Solo(44, c =>
        {
            c.TakeoffCrisis.MinEra = 0;
            c.TakeoffCrisis.CapabilityMin = 100;
            c.TakeoffCrisis.CorruptionMin = 100;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 8);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Bar_crossed_poses_takeoff_event()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 500;
        g.Me.OfficeEventId = "";
        OfficeEvents.OnTakeoff(g.C, g.State, g.Me);
        Assert.True(OfficeEvents.IsPending(g.Me));
        Assert.Equal(OfficeTrigger.Takeoff, g.C.Office.Event(g.Me.OfficeEventId).Trigger);
    }

    [Fact]
    public void Respond_takeoff_clears_event()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 500;
        g.Me.OfficeEventId = "";
        OfficeEvents.OnTakeoff(g.C, g.State, g.Me);
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        g.Do(Command.RespondTakeoff(g.Id, e.Choices[0].IdValue));
        Assert.False(OfficeEvents.IsPending(g.Me));
    }
}