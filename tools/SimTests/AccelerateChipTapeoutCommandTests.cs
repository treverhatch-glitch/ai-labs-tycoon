using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q153: AccelerateChipTapeout command.</summary>
public class AccelerateChipTapeoutCommandTests
{
    static (Game g, ChipDesign d) TapingOut()
    {
        var g = new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 3, StartEraId = "punchline", RivalIds = new System.Collections.Generic.List<string>() }));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 10_000_000_000);
        string[] techs = { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling", "chip-codesign" };
        foreach (string t in techs) if (!g.Me.Techs.Contains(t)) g.Me.Techs.Add(t);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        g.Do(Command.Hire(g.Id, "chip-architect", 1));
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        ChipDesign d = ChipQueries.DesignInProgress(g.Me);
        d.PointsDone = d.PointsRequired;
        g.Do(Command.TapeOut(g.Id, d.Id));
        return (g, d);
    }

    [Fact]
    public void Q153_main_shortens_stage_ends_tick()
    {
        var (g, d) = TapingOut();
        int before = d.StageEndsTick;
        g.Do(Command.AccelerateChipTapeout(g.Id, d.Id));
        Assert.True(d.StageEndsTick < before);
    }

    [Fact]
    public void Q153_zero_weeks_disabled()
    {
        var (g, d) = TapingOut();
        g.C.Chips.AccelerateTapeoutWeeks = 0;
        g.Refused(Command.AccelerateChipTapeout(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q153_max_clamps_stage_to_next_week()
    {
        var (g, d) = TapingOut();
        d.StageEndsTick = g.Tick + 2;
        g.C.Chips.AccelerateTapeoutWeeks = 99;
        g.Do(Command.AccelerateChipTapeout(g.Id, d.Id));
        Assert.Equal(g.Tick + 1, d.StageEndsTick);
    }

    [Fact]
    public void Q153_bankrupt_refused()
    {
        var (g, d) = TapingOut();
        g.Me.Bankrupt = true;
        g.Refused(Command.AccelerateChipTapeout(g.Id, d.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q153_save_load_stage_tick()
    {
        var (g, d) = TapingOut();
        g.Do(Command.AccelerateChipTapeout(g.Id, d.Id));
        int end = d.StageEndsTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(end, loaded.Me.ChipDesigns.Find(x => x.Id == d.Id).StageEndsTick);
    }

    [Fact]
    public void Q153_cross_tune_software_then_accelerate()
    {
        var (g, d) = TapingOut();
        g.Do(Command.TuneChipSoftware(g.Id));
        g.Do(Command.AccelerateChipTapeout(g.Id, d.Id));
        Assert.True(d.StageEndsTick > g.Tick);
    }

    [Fact]
    public void Q153_refused_when_not_taping_out()
    {
        var (g, d) = TapingOut();
        d.Status = ChipDesignStatus.Designing;
        g.Refused(Command.AccelerateChipTapeout(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q153_needs_cash()
    {
        var (g, d) = TapingOut();
        g.Me.Cash = g.C.Chips.AccelerateTapeoutCashCost - 1;
        g.Refused(Command.AccelerateChipTapeout(g.Id, d.Id), CommandError.NotEnoughCash);
    }
}