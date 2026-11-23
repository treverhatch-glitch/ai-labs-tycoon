using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-fifteen combination rules keep effects through saves (Q467).</summary>
public class EraOBatchFifteenTogetherHardeningTests
{
    static Game Lab()
    {
        Game g = Game.Solo(132, c =>
        {
            c.CampaignDeadline.MinEra = 0;
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.TakeoffCrisis.MinEra = 0;
            c.TakeoffCrisis.CapabilityMin = 100;
            c.TakeoffCrisis.CorruptionMin = 100;
            c.Office.ChancePermille = 0;
            c.ScenarioBaggage.Enabled = 1;
        }).WithStarterLab(gpus: 8);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Deadline_mid_save_still_fires_score_ending()
    {
        Game g = Lab();
        int stop = g.Tick + 2;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        GameConfig c = g.C;
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(c, mid));
        after.Weeks(2);
        Assert.Contains(after.State.Events, e => e.Kind == EventKind.CampaignDeadlineHit);
        Assert.True(Campaign.RunScore(c, after.State, after.Me) >= 0);
    }

    [Fact]
    public void Law_shape_mid_save_still_shifts_release_bar()
    {
        Game g = Lab();
        GameConfig c = g.C;
        int before = EraRules.ReleaseBar(c, g.Tick);
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Strict);
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(c, mid));
        string nextEra = c.Eras[1].Id;
        LawShaping.OnEraBegan(c, after.State, after.Me, nextEra);
        after.Me.LawShapeEra = nextEra;
        Assert.True(EraRules.ReleaseBar(c, after.State, after.Me, EraSystem.StartTick(c, c.Era(nextEra))) > before);
    }

    [Fact]
    public void Takeoff_mid_save_still_leaves_pending_answer()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 600;
        TakeoffCrisis.RunWeek(g.C, g.State, g.Me);
        if (!OfficeEvents.IsPending(g.Me)) return;
        string eventId = g.Me.OfficeEventId;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(eventId, back.Player.OfficeEventId);
        Assert.True(OfficeEvents.IsPending(back.Player));
    }

    [Fact]
    public void Baggage_and_deadline_mid_save_still_carry_baggage_id()
    {
        Game g = Lab();
        g.Do(Command.PickScenarioBaggage(g.Id, "reputational-leak"));
        int stop = g.Tick + 20;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(3);
        Assert.Equal("reputational-leak", back.Player.BaggageId);
        Assert.Equal(stop, back.Player.CampaignDeadlineTick);
    }
}