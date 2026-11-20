using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests.Support;

static class ExpansionTestSupport
{
    public static Game SoloAtYear(int year, long cash = 2_000_000_000, Action<GameConfig> tweak = null)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(tweak), Game.Solo(1).State.Seed));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, cash);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        return g;
    }

    public static ModelState AddReleasedModel(Game g, int capability, string sectorId, string name = "Test")
    {
        var model = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = name,
            Status = ModelStatus.Trained, Capability = capability, CapBase = capability,
            Alignment = 600, DisplayedAlignment = 600,
        };
        g.Me.Models.Add(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, sectorId));
        return model;
    }

    public static int BuildExpansionRoom(Game g, string roomTypeId, int w = 6, int h = 6)
    {
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, w, h, out int x, out int y);
        return g.Do(Command.BuildRoom(g.Id, roomTypeId, x, y, w, h)).CreatedId;
    }

    public static long ExpansionBoost(Game g, ModelState model, string sectorId)
    {
        SectorConfig sector = g.C.Sector(sectorId);
        SectorState sectorState = g.State.Sector(sectorId);
        return DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).ExpansionBoost;
    }

    public static long SectorRevenueAfterWeek(Game g, string sectorId)
    {
        g.Sim.Step();
        SectorState sector = g.State.Sector(sectorId);
        long total = 0;
        for (int i = 0; i < sector.Entries.Count; i++)
            if (sector.Entries[i].LabId == g.Id) total += sector.Entries[i].LastRevenue;
        return total;
    }
}