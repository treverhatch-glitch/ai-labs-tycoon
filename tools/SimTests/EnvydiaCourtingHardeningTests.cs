using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EnvydiaCourtingHardeningTests
{
    const string Training = "training";

    static readonly string[] ChipTechs =
    {
        "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling",
    };

    static Game ChipLab(int year = 2017, bool rivals = true, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 2, StartEraId = "punchline" };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            c.World.Attention.DominanceFromPpm = MathI.Ppm;
            c.Chips.YieldCrashPermille = 0;
            foreach (ChipNodeConfig n in c.Chips.Nodes) n.RespinPermille = 0;
            c.Vendor.DriftPerMonth = 0;
            c.Vendor.GpusPerStanding = 1_000_000;
            c.Vendor.CourtingSharePermille = 300;
            c.Vendor.CourtingStandingBoost = 20;
            tweak?.Invoke(c);
        }), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        foreach (LabState lab in g.State.Labs)
        {
            foreach (string tech in ChipTechs) if (!lab.Techs.Contains(tech)) lab.Techs.Add(tech);
            Finance.Earn(lab, LedgerLine.CapitalIn, 20_000_000_000L);
            LotGrid.FindFreeRect(g.C.Lot(lab.LotId), lab.Rooms, 4, 3, out int x, out int y);
            g.Try(Command.BuildRoom(lab.Id, "chip-design-lab", x, y, 4, 3));
            g.Try(Command.Hire(lab.Id, "chip-architect", 4));
        }
        return g;
    }

    static int Hall(Game g, LabState lab)
    {
        int id = g.Do(Command.BuyLand(lab.Id, "us-oregon")).CreatedId;
        lab.Holding(id).HallKw = 1_000_000;
        return id;
    }

    static ChipDesign ReadyTrainingChip(Game g, LabState lab, int yield = 900)
    {
        var d = new ChipDesign();
        ChipSpec spec = ChipQueries.Derive(g.C, Training, "n5", "medium", "fp8", "hbm", "air");
        d.Id = g.State.TakeId();
        d.Version = ChipQueries.NextVersion(lab);
        d.Name = lab.FamilyName + " Silicon " + d.Version;
        d.Target = Training;
        d.NodeId = "n5";
        d.DieId = "medium";
        d.FormatId = "fp8";
        d.MemoryId = "hbm";
        d.CoolingId = "air";
        d.PointsRequired = spec.Points;
        d.PointsDone = spec.Points;
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = yield;
        d.ReadyTick = g.State.Tick;
        lab.ChipDesigns.Add(d);
        return d;
    }

    static void LoadOwnTrainingChips(Game g, LabState lab, int count = 800)
    {
        int hall = Hall(g, lab);
        ChipDesign design = ReadyTrainingChip(g, lab);
        lab.SoftwarePermille = 1000;
        ChipSystem.AddChips(g.State, lab, hall, lab.Id, design.Id, count);
    }

    [Fact]
    public void Below_courting_share_does_not_boost_rivals()
    {
        Game g = ChipLab(tweak: c => c.Vendor.CourtingSharePermille = 800);
        g.Rent(40);
        LoadOwnTrainingChips(g, g.Me, 20);
        Assert.True(ChipQueries.OwnChipsShare(g.C, g.State, g.Me) < g.C.Vendor.CourtingSharePermille);
        LabState rival = g.State.Labs[1];
        int before = g.State.Vendor.Standing(rival.Id).Standing;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(before, g.State.Vendor.Standing(rival.Id).Standing);
        Assert.DoesNotContain(g.State.Vendor.Log, e => e.Action == DecisionAction.Courted);
    }

    [Fact]
    public void At_courting_share_every_other_lab_gains_standing()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        var before = g.State.Labs.Select(l => g.State.Vendor.Standing(l.Id).Standing).ToArray();
        ChipSystem.CourtRivals(g.C, g.State);
        for (int i = 0; i < g.State.Labs.Count; i++)
        {
            LabState lab = g.State.Labs[i];
            if (lab.Id == g.Id || lab.Bankrupt) continue;
            Assert.Equal(before[i] + g.C.Vendor.CourtingStandingBoost, g.State.Vendor.Standing(lab.Id).Standing);
        }
    }

    [Fact]
    public void The_chip_maker_is_not_boosted_by_its_own_courting()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        int before = g.State.Vendor.Standing(g.Id).Standing;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(before, g.State.Vendor.Standing(g.Id).Standing);
    }

    [Fact]
    public void Vendor_log_records_courting_once_per_quarter()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(1, g.State.Vendor.Log.Count(e => e.Action == DecisionAction.Courted && e.Ref == g.Id.ToString()));
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(1, g.State.Vendor.Log.Count(e => e.Action == DecisionAction.Courted && e.Ref == g.Id.ToString()));
        g.Weeks(g.C.Calendar.WeeksPerMonth * 3);
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(2, g.State.Vendor.Log.Count(e => e.Action == DecisionAction.Courted && e.Ref == g.Id.ToString()));
    }

    [Fact]
    public void Bankrupt_labs_skip_courting_as_targets()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        LabState broke = g.State.Labs[2];
        broke.Bankrupt = true;
        int brokeBefore = g.State.Vendor.Standing(broke.Id).Standing;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(brokeBefore, g.State.Vendor.Standing(broke.Id).Standing);
    }

    [Fact]
    public void Two_labs_above_share_each_log_their_own_courting()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        LoadOwnTrainingChips(g, g.State.Labs[1], 1200);
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Contains(g.State.Vendor.Log, e => e.Action == DecisionAction.Courted && e.Ref == g.Id.ToString());
        Assert.Contains(g.State.Vendor.Log, e => e.Action == DecisionAction.Courted && e.Ref == "1");
    }

    [Fact]
    public void Courting_runs_from_vendor_month_without_needing_keynote()
    {
        Game g = ChipLab(year: 2015);
        LoadOwnTrainingChips(g, g.Me, 1000);
        LabState rival = g.State.Labs[1];
        int rivalStanding = g.State.Vendor.Standing(rival.Id).Standing;
        VendorSystem.RunMonth(g.C, g.State);
        Assert.True(g.State.Vendor.Standing(rival.Id).Standing >= rivalStanding + g.C.Vendor.CourtingStandingBoost);
    }

    [Fact]
    public void Zero_general_compute_never_triggers_courting()
    {
        Game g = ChipLab(rivals: false);
        g.Me.Fleet.Clear();
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.DoesNotContain(g.State.Vendor.Log, e => e.Action == DecisionAction.Courted);
        Assert.Equal(0, ChipQueries.OwnChipsShare(g.C, g.State, g.Me));
    }

    [Fact]
    public void Courting_standing_boost_survives_save_load()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        ChipSystem.CourtRivals(g.C, g.State);
        LabState rival = g.State.Labs[1];
        int rivalStanding = g.State.Vendor.Standing(rival.Id).Standing;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(rivalStanding, loaded.State.Vendor.Standing(rival.Id).Standing);
        Assert.Contains(loaded.State.Vendor.Log, e => e.Action == DecisionAction.Courted);
    }
}