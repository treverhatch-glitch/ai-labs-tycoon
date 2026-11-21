using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests.Support;

static class ChipHardeningSupport
{
    internal const string Training = "training";
    internal const string Inference = "inference";

    internal static readonly string[] ChipTechs =
    {
        "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "chiplet-packaging", "low-precision", "liquid-cooling",
    };

    internal static Game Lab(int year, int seed = 3, bool rivals = true, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "punchline" };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            c.World.Attention.DominanceFromPpm = MathI.Ppm;
            c.Chips.YieldCrashPermille = 0;
            foreach (ChipNodeConfig n in c.Chips.Nodes) n.RespinPermille = 0;
            c.Vendor.DriftPerMonth = 0;
            tweak?.Invoke(c);
        }), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            foreach (string tech in ChipTechs) if (!lab.Techs.Contains(tech)) lab.Techs.Add(tech);
            Finance.Earn(lab, LedgerLine.CapitalIn, 20_000_000_000L);
            if (i != 0) continue;
            LotGrid.FindFreeRect(g.C.Lot(lab.LotId), lab.Rooms, 4, 3, out int x, out int y);
            g.Try(Command.BuildRoom(lab.Id, "chip-design-lab", x, y, 4, 3));
            g.Try(Command.Hire(lab.Id, "chip-architect", 4));
        }
        return g;
    }

    internal static int Hall(Game g, LabState lab, int kw = 1_000_000)
    {
        int id = g.Do(Command.BuyLand(lab.Id, "us-oregon")).CreatedId;
        lab.Holding(id).HallKw = kw;
        return id;
    }

    internal static ChipDesign InjectReady(Game g, LabState lab, string target = Inference, string node = "n28",
        string die = "small", int yield = 800, int readyTick = -1)
    {
        var d = new ChipDesign();
        ChipSpec spec = ChipQueries.Derive(g.C, target, node, "small", "fp16", "ddr", "air");
        d.Id = g.State.TakeId();
        d.Version = ChipQueries.NextVersion(lab);
        d.Name = lab.FamilyName + " Silicon " + d.Version;
        d.Target = target;
        d.NodeId = node;
        d.DieId = die;
        d.FormatId = "fp16";
        d.MemoryId = "ddr";
        d.CoolingId = "air";
        d.PointsRequired = spec.Points;
        d.PointsDone = spec.Points;
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = yield;
        d.ReadyTick = readyTick >= 0 ? readyTick : g.State.Tick;
        lab.ChipDesigns.Add(d);
        return d;
    }

    internal static int KeynoteTick(GameConfig c, int year) =>
        GameDate.TickOfYear(c.Calendar, year) + (c.Vendor.KeynoteMonth - 1) * c.Calendar.WeeksPerMonth;

    internal static void HireEngineers(Game g, int count)
    {
        for (int bays = (count + 3) / 4; bays > 0; bays--)
        {
            LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 4, out int x, out int y);
            g.Try(Command.BuildRoom(g.Id, "engineering-bay", x, y, 4, 4));
        }
        g.Try(Command.Hire(g.Id, "engineer", count));
    }

    internal static ChipDesign InjectTapingOut(Game g, LabState lab, string target = Training, string node = "n5")
    {
        ChipSpec spec = ChipQueries.Derive(g.C, target, node, "medium", target == Training ? "fp8" : "fp16",
            target == Training ? "hbm" : "ddr", "air");
        var d = new ChipDesign();
        d.Id = g.State.TakeId();
        d.Version = ChipQueries.NextVersion(lab);
        d.Name = lab.FamilyName + " Silicon " + d.Version;
        d.Target = target;
        d.NodeId = node;
        d.DieId = "medium";
        d.FormatId = target == Training ? "fp8" : "fp16";
        d.MemoryId = target == Training ? "hbm" : "ddr";
        d.CoolingId = "air";
        d.PointsRequired = spec.Points;
        d.PointsDone = spec.Points;
        d.Status = ChipDesignStatus.TapingOut;
        d.StageEndsTick = g.State.Tick;
        lab.ChipDesigns.Add(d);
        return d;
    }
}