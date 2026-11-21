using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ChipAmbushStartDesignTests
{
    const string Inference = "inference";

    static readonly string[] ChipTechs =
    {
        "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling",
    };

    static Command Design(int labId) => Command.StartChipDesign(labId, Inference, "n28", "small", "fp16", "ddr", "air");

    static Game Lab(int year = 2013)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.World.Attention.DominanceFromPpm = MathI.Ppm),
            new NewGameOptions { Seed = 1, StartEraId = "punchline", RivalIds = new List<string>() }));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        foreach (string tech in ChipTechs) if (!g.Me.Techs.Contains(tech)) g.Me.Techs.Add(tech);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 20_000_000_000L);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 4, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 4));
        g.Do(Command.Hire(g.Id, "chip-architect", 4));
        return g;
    }

    [Fact]
    public void New_design_refused_during_ambush()
    {
        Game g = Lab();
        g.Me.ChipAmbushUntilTick = g.State.Tick + g.C.Vendor.AmbushWeeks;
        g.Refused(Design(g.Id), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Design_starts_after_ambush_window()
    {
        Game g = Lab();
        g.Me.ChipAmbushUntilTick = g.State.Tick;
        g.Do(Design(g.Id));
        Assert.Single(g.Me.ChipDesigns);
    }

    [Fact]
    public void In_progress_design_can_still_tape_out_during_ambush()
    {
        Game g = Lab();
        g.Do(Design(g.Id));
        ChipDesign d = g.Me.ChipDesigns[0];
        d.PointsDone = d.PointsRequired;
        g.Me.ChipAmbushUntilTick = g.State.Tick + 6;
        g.Do(Command.TapeOut(g.Id, d.Id));
        Assert.Equal(ChipDesignStatus.TapingOut, d.Status);
    }

    [Fact]
    public void Cancel_design_allowed_during_ambush()
    {
        Game g = Lab();
        g.Do(Design(g.Id));
        ChipDesign d = g.Me.ChipDesigns[0];
        g.Me.ChipAmbushUntilTick = g.State.Tick + 4;
        g.Do(Command.CancelChipDesign(g.Id, d.Id));
        Assert.Equal(ChipDesignStatus.Cancelled, d.Status);
    }

    [Fact]
    public void Keynote_blocks_fresh_design_start()
    {
        Game g = Lab();
        int t = ChipHardeningSupport.KeynoteTick(g.C, 2017);
        g.State.Tick = t;
        ChipHardeningSupport.InjectReady(g, g.Me, readyTick: t);
        ChipSystem.OnKeynote(g.C, g.State);
        g.Refused(Design(g.Id), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Bankrupt_lab_refused_before_ambush_gate()
    {
        Game g = Lab();
        g.Me.Bankrupt = true;
        g.Me.ChipAmbushUntilTick = g.State.Tick + 8;
        g.Refused(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Second_design_still_blocked_while_first_finishes()
    {
        Game g = Lab();
        g.Do(Design(g.Id));
        g.Me.ChipAmbushUntilTick = g.State.Tick + 8;
        g.Refused(Command.StartChipDesign(g.Id, "training", "n5", "medium", "fp8", "hbm", "air"), CommandError.KeynoteAmbush);
    }

    [Fact]
    public void Ambush_tick_boundary_allows_start()
    {
        Game g = Lab();
        g.Me.ChipAmbushUntilTick = g.State.Tick + 1;
        g.State.Tick = g.Me.ChipAmbushUntilTick;
        g.Do(Design(g.Id));
        Assert.Single(g.Me.ChipDesigns);
    }

    [Fact]
    public void Start_design_gate_survives_save_load()
    {
        Game g = Lab();
        g.Me.ChipAmbushUntilTick = g.State.Tick + 12;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Refused(Command.StartChipDesign(loaded.Id, "inference", "n28", "small", "fp16", "ddr", "air"), CommandError.KeynoteAmbush);
    }
}