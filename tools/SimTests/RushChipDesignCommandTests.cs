using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q151: RushChipDesign command.</summary>
public class RushChipDesignCommandTests
{
    internal static Game ChipLab(long cash = 50_000_000_000L, System.Action<GameConfig> tweak = null)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(tweak), new NewGameOptions { Seed = 1, StartEraId = "punchline", RivalIds = new System.Collections.Generic.List<string>() }));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, cash);
        foreach (string t in new[] { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling", "chip-codesign" })
            if (!g.Me.Techs.Contains(t)) g.Me.Techs.Add(t);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        g.Do(Command.Hire(g.Id, "chip-architect", 2));
        return g;
    }

    static ChipDesign Start(Game g)
    {
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        return ChipQueries.DesignInProgress(g.Me);
    }

    [Fact]
    public void Q151_main_adds_configured_design_points()
    {
        Game g = ChipLab();
        ChipDesign d = Start(g);
        long before = d.PointsDone;
        g.Do(Command.RushChipDesign(g.Id, d.Id));
        Assert.Equal(before + g.C.Chips.RushDesignPoints, d.PointsDone);
    }

    [Fact]
    public void Q151_zero_rush_points_still_ok_when_cost_zero()
    {
        Game g = ChipLab(tweak: c => { c.Chips.RushDesignPoints = 0; c.Chips.RushDesignCashCost = 0; });
        ChipDesign d = Start(g);
        long before = d.PointsDone;
        g.Do(Command.RushChipDesign(g.Id, d.Id));
        Assert.Equal(before, d.PointsDone);
    }

    [Fact]
    public void Q151_max_clamps_at_required_points()
    {
        Game g = ChipLab();
        ChipDesign d = Start(g);
        d.PointsDone = d.PointsRequired - 5;
        g.Do(Command.RushChipDesign(g.Id, d.Id));
        Assert.Equal(d.PointsRequired, d.PointsDone);
    }

    [Fact]
    public void Q151_bankrupt_refused()
    {
        Game g = ChipLab();
        ChipDesign d = Start(g);
        g.Me.Bankrupt = true;
        g.Refused(Command.RushChipDesign(g.Id, d.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q151_save_load_points_persist()
    {
        Game g = ChipLab();
        ChipDesign d = Start(g);
        g.Do(Command.RushChipDesign(g.Id, d.Id));
        long pts = d.PointsDone;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(pts, loaded.Me.ChipDesigns.Find(x => x.Id == d.Id).PointsDone);
    }

    [Fact]
    public void Q151_cross_rush_reaches_tapeout_gate()
    {
        Game g = ChipLab();
        ChipDesign d = Start(g);
        while (d.PointsDone < d.PointsRequired)
            g.Do(Command.RushChipDesign(g.Id, d.Id));
        g.Do(Command.TapeOut(g.Id, d.Id));
        Assert.Equal(ChipDesignStatus.TapingOut, d.Status);
    }

    [Fact]
    public void Q151_refused_without_architect()
    {
        Game g = ChipLab();
        ChipDesign d = Start(g);
        g.Do(Command.Fire(g.Id, "chip-architect", 2));
        g.Refused(Command.RushChipDesign(g.Id, d.Id), CommandError.NotEnoughStaff);
    }

    [Fact]
    public void Q151_needs_cash_when_configured()
    {
        Game g = ChipLab(cash: 100);
        ChipDesign d = Start(g);
        g.Me.Cash = g.C.Chips.RushDesignCashCost - 1;
        g.Refused(Command.RushChipDesign(g.Id, d.Id), CommandError.NotEnoughCash);
    }
}