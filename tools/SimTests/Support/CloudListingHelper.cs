using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.SimTests.Support;

static class CloudListingHelper
{
    public static Game SellerBuyerPair(long cash = 50_000_000, System.Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 11, StartEraId = "punchline" };
        options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(tweak), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2024);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, cash);
        foreach (string tech in new[] { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling" })
            if (!g.Me.Techs.Contains(tech)) g.Me.Techs.Add(tech);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        return g;
    }

    public static ChipDesign ReadyDesign(Game g, bool trains = true) => ReadyDesignOn(g.Me, g, trains);

    public static ChipDesign ReadyDesignOn(LabState lab, Game g, bool trains = true)
    {
        string target = trains ? "training" : "inference";
        ChipSpec spec = ChipQueries.Derive(g.C, target, "n28", "medium", "fp16", "ddr", "air");
        int ver = ChipQueries.NextVersion(lab);
        var d = new ChipDesign
        {
            Id = g.State.TakeId(),
            Version = ver,
            Name = lab.FamilyName + " Silicon " + ver,
            Target = target,
            NodeId = "n28",
            DieId = "medium",
            FormatId = "fp16",
            MemoryId = "ddr",
            CoolingId = "air",
            PointsRequired = spec.Points,
            PointsDone = spec.Points,
            Status = ChipDesignStatus.Ready,
            YieldPermille = 900,
            StageEndsTick = -1,
            ReadyTick = g.Tick,
        };
        lab.ChipDesigns.Add(d);
        if (lab.SoftwarePermille == 0) lab.SoftwarePermille = g.C.Chips.SoftwareStart;
        return d;
    }

    public static LabState RentOnlyBuyer(Game g) => RentOnlyBuyerOn(g);

    public static LabState RentOnlyBuyerOn(Game g)
    {
        LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("openaye"), EraSystem.AtTick(g.C, g.Tick), true);
        buyer.PlanStartTick = 0;
        Finance.Earn(buyer, LedgerLine.CapitalIn, 5_000_000_000);
        return buyer;
    }
}