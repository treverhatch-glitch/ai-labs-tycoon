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

public class CourtingMakerAttentionTests
{
    const string Training = "training";

    static readonly string[] ChipTechs =
    {
        "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling",
    };

    static Game ChipLab(Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 5, StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            c.World.Attention.DominanceFromPpm = MathI.Ppm;
            c.Chips.YieldCrashPermille = 0;
            foreach (ChipNodeConfig n in c.Chips.Nodes) n.RespinPermille = 0;
            c.Vendor.DriftPerMonth = 0;
            c.Vendor.GpusPerStanding = 1_000_000;
            c.Vendor.CourtingSharePermille = 300;
            tweak?.Invoke(c);
        }), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2017);
        foreach (LabState lab in g.State.Labs)
        {
            foreach (string tech in ChipTechs) if (!lab.Techs.Contains(tech)) lab.Techs.Add(tech);
            Finance.Earn(lab, LedgerLine.CapitalIn, 20_000_000_000L);
            LotGrid.FindFreeRect(g.C.Lot(lab.LotId), lab.Rooms, 4, 3, out int x, out int y);
            g.Try(Command.BuildRoom(lab.Id, "chip-design-lab", x, y, 4, 3));
        }
        return g;
    }

    static void LoadOwnTrainingChips(Game g, LabState lab, int count)
    {
        int hall = g.Do(Command.BuyLand(lab.Id, "us-oregon")).CreatedId;
        lab.Holding(hall).HallKw = 1_000_000;
        ChipSpec spec = ChipQueries.Derive(g.C, Training, "n5", "medium", "fp8", "hbm", "air");
        var d = new ChipDesign();
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
        d.YieldPermille = 900;
        d.ReadyTick = g.State.Tick;
        lab.ChipDesigns.Add(d);
        lab.SoftwarePermille = 1000;
        ChipSystem.AddChips(g.State, lab, hall, lab.Id, d.Id, count);
    }

    [Fact]
    public void Courting_raises_home_bloc_attention_on_the_chip_maker()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        int before = home.Attention;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(before + g.C.Vendor.CourtingMakerAttentionPermille, home.Attention);
    }

    [Fact]
    public void Below_share_leaves_home_attention_unchanged()
    {
        Game g = ChipLab(c => c.Vendor.CourtingSharePermille = 900);
        g.Rent(40);
        LoadOwnTrainingChips(g, g.Me, 20);
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(before, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Attention_capped_at_permille()
    {
        Game g = ChipLab(c => c.Vendor.CourtingMakerAttentionPermille = 500);
        LoadOwnTrainingChips(g, g.Me, 1200);
        g.Me.Bloc(g.Me.HomeBlocId).Attention = MathI.Permille - 100;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(MathI.Permille, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Zero_config_skips_attention_even_when_courting()
    {
        Game g = ChipLab(c => c.Vendor.CourtingMakerAttentionPermille = 0);
        LoadOwnTrainingChips(g, g.Me, 1200);
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(before, g.Me.Bloc(g.Me.HomeBlocId).Attention);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.AttentionRose && e.LabId == g.Id);
    }

    [Fact]
    public void Attention_rise_logs_an_event()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AttentionRose && e.Ref == g.Me.HomeBlocId && e.LabId == g.Id);
    }

    [Fact]
    public void Each_courting_lab_adds_attention_on_its_own_home_bloc()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        LoadOwnTrainingChips(g, g.State.Labs[1], 1200);
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention >= g.C.Vendor.CourtingMakerAttentionPermille);
        Assert.True(g.State.Labs[1].Bloc(g.State.Labs[1].HomeBlocId).Attention >= g.C.Vendor.CourtingMakerAttentionPermille);
    }

    [Fact]
    public void Courting_attention_survives_save_load()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        ChipSystem.CourtRivals(g.C, g.State);
        int attention = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(attention, loaded.Me.Bloc(loaded.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Government_month_sees_higher_attention_after_courting()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        ChipSystem.CourtRivals(g.C, g.State);
        int afterCourting = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Assert.True(afterCourting > 0);
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention > 0);
    }

    [Fact]
    public void Vendor_standing_boost_and_attention_both_apply_same_quarter()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        LabState rival = g.State.Labs[1];
        int standingBefore = g.State.Vendor.Standing(rival.Id).Standing;
        int attentionBefore = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(standingBefore + g.C.Vendor.CourtingStandingBoost, g.State.Vendor.Standing(rival.Id).Standing);
        Assert.Equal(attentionBefore + g.C.Vendor.CourtingMakerAttentionPermille, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Attention_rises_once_per_quarter_not_every_court_call()
    {
        Game g = ChipLab();
        LoadOwnTrainingChips(g, g.Me, 1200);
        int start = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ChipSystem.CourtRivals(g.C, g.State);
        int afterFirst = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        Assert.Equal(g.C.Vendor.CourtingMakerAttentionPermille, afterFirst - start);
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(afterFirst, g.Me.Bloc(g.Me.HomeBlocId).Attention);
        int quarter = DecisionLog.QuarterOf(g.C, g.State.Tick);
        while (DecisionLog.QuarterOf(g.C, g.State.Tick) == quarter) g.Weeks(1);
        int beforeNext = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ChipSystem.CourtRivals(g.C, g.State);
        Assert.Equal(g.C.Vendor.CourtingMakerAttentionPermille, g.Me.Bloc(g.Me.HomeBlocId).Attention - beforeNext);
    }
}