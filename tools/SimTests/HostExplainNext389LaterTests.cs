using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Rival log save bound + NEXT item 3 explain-every-number (batch 393–394).</summary>
public class HostExplainNext389LaterTests
{
    [Fact]
    public void Rival_log_save_respects_trimmed_max()
    {
        Game g = Game.New(389, c => c.Rivalry.LogMax = 7);
        LabState rival = g.Rival("openaye");
        for (int i = 0; i < 12; i++)
            DecisionLog.Add(g.C, g.State, rival, i, DecisionAction.Paused, i.ToString(), 0);
        string json = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(7, loaded.Rival("openaye").Log.Count);
    }

    [Fact]
    public void Appeal_parts_sum_to_market_appeal()
    {
        var g = Game.Solo().WithStarterLab(sales: 2);
        var model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(2);
        var sector = g.C.Sector("game-ai");
        var sectorState = g.State.Sector("game-ai");
        DisplayQueries.AppealBreakdown b = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model);
        long appeal = MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.Equal(appeal, b.Total);
        Assert.Equal(b.Core + b.ReputationBoost + b.SalesBoost + b.TrustBoost + b.ChampionBoost + b.NamingConfusion, appeal);
    }

    [Fact]
    public void Incident_parts_sum_to_safety_permille()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 500, Corruption = 100, Status = ModelStatus.Released };
        DisplayQueries.IncidentBreakdown b = DisplayQueries.IncidentParts(g.C, g.Tick, model);
        Assert.Equal(SafetySystem.IncidentChancePermille(g.C, g.Tick, model), b.Total);
        Assert.Equal(b.FromConfig + b.FromAlignment + b.FromCorruption, b.Total);
    }

    [Fact]
    public void Hardware_fleet_parts_align_with_weekly_rent()
    {
        var g = Game.Solo().WithStarterLab(gpus: 4);
        g.Rent(2);
        DisplayQueries.HardwareFleetBreakdown fleet = DisplayQueries.HardwareFleetParts(g.C, g.State, g.Me);
        DisplayQueries.GpuRentBreakdown rent = DisplayQueries.GpuRentParts(g.C, g.State, g.Me);
        Assert.Equal(fleet.RentedGpus + fleet.OwnedHqGpus + fleet.OwnedOffsiteGpus + fleet.ReservedGpus, fleet.TotalUnits);
        Assert.Equal(LabQueries.WeeklyGpuRent(g.C, g.State, g.Me), rent.Total);
    }
}