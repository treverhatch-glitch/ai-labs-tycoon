using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BodiesUpwardExpansionTests
{
    [Fact]
    public void Robotics_arena_boosts_bodies_sectors_only_after_the_era_begins()
    {
        var g = Game.Solo();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2029);
        var model = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = "Robo",
            Status = ModelStatus.Trained, Capability = 200_000, CapBase = 200_000, Alignment = 600, DisplayedAlignment = 600,
        };
        g.Me.Models.Add(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "robotics-industry"));
        SectorConfig sector = g.C.Sector("robotics-industry");
        SectorState sectorState = g.State.Sector(sector.Id);
        long before = DisplayQueries.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.Equal(0, DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).ExpansionBoost);

        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 6, 6, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "robotics-arena", x, y, 6, 6));
        long after = DisplayQueries.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.True(after > before);
        DisplayQueries.AppealBreakdown parts = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.True(parts.ExpansionBoost > 0);
        Assert.Equal(parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.ChampionBoost + parts.ExpansionBoost, after);
    }

    [Fact]
    public void Orbital_uplink_boosts_upward_sectors_from_2033()
    {
        var g = Game.Solo();
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 2_000_000_000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2033);
        var model = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = "Space",
            Status = ModelStatus.Trained, Capability = 250_000, CapBase = 250_000, Alignment = 700, DisplayedAlignment = 700,
        };
        g.Me.Models.Add(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "machine-science"));
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 6, 6, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "orbital-uplink", x, y, 6, 6));
        SectorConfig sector = g.C.Sector("machine-science");
        SectorState sectorState = g.State.Sector(sector.Id);
        Assert.True(DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).ExpansionBoost > 0);
    }

    [Fact]
    public void Expansion_config_validates_era_room_and_sectors()
    {
        Assert.Empty(ConfigValidator.Problems(TestData.Config()));
    }
}