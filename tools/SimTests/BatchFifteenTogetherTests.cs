using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q467 combination wiring for batch fifteen systems.</summary>
public class BatchFifteenTogetherTests
{
    static Game Lab()
    {
        Game g = Game.Solo(46, c =>
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
    public void Deadline_with_era_score_ending()
    {
        Game g = Lab();
        int stop = g.Tick + 2;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        g.Weeks(2);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.CampaignDeadlineHit);
        Assert.True(Campaign.RunScore(g.C, g.State, g.Me) >= 0);
    }

    [Fact]
    public void Law_shape_with_alignment_bar()
    {
        Game g = Lab();
        int before = EraRules.ReleaseBar(g.C, g.Tick);
        g.Do(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict));
        string nextEra = g.C.Eras[1].Id;
        LawShaping.OnEraBegan(g.C, g.State, g.Me, nextEra);
        g.Me.LawShapeEra = nextEra;
        Assert.True(EraRules.ReleaseBar(g.C, g.State, g.Me, EraSystem.StartTick(g.C, g.C.Era(nextEra))) > before);
    }

    [Fact]
    public void Exit_with_loan_deduction()
    {
        Game g = Lab();
        g.Build("server-closet", 4, 4);
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Me.Ledger.ThisMonth.Revenue = 500_000;
        g.Do(Command.TakeLoan(g.Id, 500));
        var b = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.True(b.Loans > 0);
        Assert.True(b.Total < b.RevenueYears + b.CashOnHand);
    }

    [Fact]
    public void Takeoff_with_containment_room()
    {
        Game g = Lab();
        RoomTypeConfig room = g.C.RoomType("containment-wing");
        if (room != null && EraSystem.RoomOpen(g.C, room, g.Tick)) g.Build("containment-wing", 4, 4);
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Capability = 5000;
        m.Corruption = 600;
        TakeoffCrisis.RunWeek(g.C, g.State, g.Me);
        Assert.True(OfficeEvents.IsPending(g.Me));
    }

    [Fact]
    public void Baggage_with_trust()
    {
        var options = new NewGameOptions { Seed = 46, BaggageId = "reputational-leak", RivalIds = new List<string>() };
        Simulation sim = Simulation.NewGame(TestData.Config(), options);
        Assert.Equal("reputational-leak", sim.Player.BaggageId);
        Assert.True(sim.Player.Reputation < TestData.Config().PlayerStart.Reputation);
    }
}