using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ChipSoftwareBreakdownHardeningTests
{
    static Game ChipLab(int year = 2013, long cash = 20_000_000_000L)
    {
        var options = new NewGameOptions { Seed = 1, RivalIds = new List<string>(), StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, cash);
        g.Me.Techs.Add("chip-team");
        g.Me.Techs.Add("training-silicon");
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        g.Hire("chip-architect", 2);
        return g;
    }

    [Fact]
    public void Zero_lab_software_yields_empty_breakdown()
    {
        var g = Game.Solo();
        var parts = ChipQueries.SoftwareParts(g.C, g.Me);
        Assert.Equal(0, parts.Total);
        Assert.Equal(0, ChipQueries.EffectiveSoftware(g.C, g.Me));
    }

    [Fact]
    public void First_chip_design_sets_start_permille_and_parts_sum()
    {
        Game g = ChipLab();
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        var parts = ChipQueries.SoftwareParts(g.C, g.Me);
        Assert.Equal(g.C.Chips.SoftwareStart, parts.FromLab);
        Assert.Equal(parts.Total, ChipQueries.EffectiveSoftware(g.C, g.Me));
        Assert.Equal(parts.FromLab + parts.FromTech + parts.CapClip, parts.Total);
    }

    [Fact]
    public void Compiler_tech_adds_to_from_tech_part()
    {
        Game g = ChipLab(2020);
        g.Me.Techs.Add("compiler-stack-1");
        g.Me.SoftwarePermille = g.C.Chips.SoftwareStart;
        var withTech = ChipQueries.SoftwareParts(g.C, g.Me);
        int effWithTech = ChipQueries.EffectiveSoftware(g.C, g.Me);
        g.Me.Techs.Remove("compiler-stack-1");
        var plain = ChipQueries.SoftwareParts(g.C, g.Me);
        Assert.True(withTech.FromTech > plain.FromTech);
        Assert.Equal(withTech.Total, effWithTech);
    }

    [Fact]
    public void Software_above_cap_shows_negative_cap_clip()
    {
        Game g = ChipLab();
        g.Me.SoftwarePermille = g.C.Chips.SoftwareCap + 200;
        g.Me.Techs.Add("compiler-stack-1");
        g.Me.Techs.Add("compiler-stack-2");
        g.Me.Techs.Add("compiler-stack-3");
        var parts = ChipQueries.SoftwareParts(g.C, g.Me);
        Assert.Equal(g.C.Chips.SoftwareCap, parts.Total);
        Assert.True(parts.CapClip < 0);
        Assert.Equal(parts.FromLab + parts.FromTech + parts.CapClip, parts.Total);
    }

    [Fact]
    public void Batch_compute_scales_with_effective_software_parts()
    {
        Game g = ChipLab();
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        ChipDesign d = g.Me.ChipDesigns[0];
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = 800;
        g.Me.Fleet.Add(new HardwareBatch
        {
            Id = g.State.TakeId(),
            HoldingId = HardwareBatch.Hq,
            ChipDesignId = d.Id,
            DesignLabId = g.Id,
            Count = 20,
            ArrivedTick = g.Tick,
        });
        HardwareBatch batch = g.Me.Fleet[0];
        int eff = ChipQueries.EffectiveSoftware(g.C, g.Me);
        long withSoftware = ChipQueries.BatchCompute(g.C, g.State, batch);
        g.Me.SoftwarePermille = eff / 2;
        long reduced = ChipQueries.BatchCompute(g.C, g.State, batch);
        Assert.True(withSoftware > reduced);
        Assert.Equal(ChipQueries.SoftwareParts(g.C, g.Me).Total, ChipQueries.EffectiveSoftware(g.C, g.Me));
    }

    [Fact]
    public void Architects_raise_lab_software_over_weeks_while_designing()
    {
        Game g = ChipLab();
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        int start = ChipQueries.EffectiveSoftware(g.C, g.Me);
        for (int w = 0; w < 20; w++) g.Weeks(1);
        int grown = ChipQueries.EffectiveSoftware(g.C, g.Me);
        Assert.True(grown > start);
        Assert.Equal(grown, ChipQueries.SoftwareParts(g.C, g.Me).Total);
    }

    [Fact]
    public void Bankrupt_lab_still_reports_software_breakdown()
    {
        Game g = ChipLab();
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        g.Me.Bankrupt = true;
        var parts = ChipQueries.SoftwareParts(g.C, g.Me);
        Assert.True(parts.Total > 0);
        g.Refused(Command.StartChipDesign(g.Id, "training", "n28", "medium", "fp8", "ddr", "air"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Software_breakdown_survives_save_load()
    {
        Game g = ChipLab();
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        var before = ChipQueries.SoftwareParts(g.C, g.Me);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        var after = ChipQueries.SoftwareParts(loaded.C, loaded.Me);
        Assert.Equal(before.Total, after.Total);
        Assert.Equal(before.FromLab, after.FromLab);
    }
}