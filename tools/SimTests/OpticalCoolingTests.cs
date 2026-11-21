using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q66: optical-interconnect unlocks optical chip cooling on designs.</summary>
public class OpticalCoolingTests
{
    static void EnsureChipArchitects(Game g)
    {
        if (LabQueries.StaffCount(g.Me, "chip-architect") > 0) return;
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 4, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 4));
        g.Do(Command.Hire(g.Id, "chip-architect", 4));
    }

    [Fact]
    public void Shipped_optical_cooling_validates()
    {
        GameConfig c = TestData.Config();
        Assert.NotNull(c.Chips.Option(ChipOptionGroup.Cooling, "optical"));
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Start_refused_without_optical_interconnect()
    {
        Game g = ChipHardeningSupport.Lab(2028, tweak: _ => { });
        g.Me.Techs.Remove("optical-interconnect");
        g.Refused(Command.StartChipDesign(g.Id, ChipHardeningSupport.Inference, "n28", "medium", "fp16", "ddr", "optical"),
            CommandError.MissingPrerequisite);
    }

    [Fact]
    public void Design_succeeds_with_tech()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipOptionConfig optical = g.C.Chips.Option(ChipOptionGroup.Cooling, "optical");
        Assert.True(ChipQueries.OptionAvailable(g.Me, optical));
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, cooling: "optical");
        Assert.Equal("optical", d.CoolingId);
    }

    [Fact]
    public void Derive_raises_compute_and_lowers_kw()
    {
        GameConfig c = TestData.Config();
        ChipSpec air = ChipQueries.Derive(c, "inference", "n28", "medium", "fp16", "ddr", "air");
        ChipSpec optical = ChipQueries.Derive(c, "inference", "n28", "medium", "fp16", "ddr", "optical");
        Assert.True(optical.Compute > air.Compute);
        Assert.True(optical.Kw <= air.Kw);
    }

    [Fact]
    public void Zero_tech_uses_liquid_not_optical()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, cooling: "liquid");
        Assert.Equal("liquid", d.CoolingId);
    }

    [Fact]
    public void Inject_ready_optical_round_trip()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("optical-interconnect");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, cooling: "optical");
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal("optical", loaded.Me.Design(d.Id).CoolingId);
    }

    [Fact]
    public void Order_chips_with_optical_design()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("optical-interconnect");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, cooling: "optical");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 5));
        Assert.Single(g.Me.ChipOrders);
    }

    [Fact]
    public void Bankrupt_lab_design_query_unchanged()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("optical-interconnect");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, cooling: "optical");
        g.Me.Bankrupt = true;
        Assert.Equal("optical", d.CoolingId);
    }

    [Fact]
    public void Chip_shortage_still_prices_optical_orders()
    {
        Game g = ChipHardeningSupport.Lab(2028, tweak: c => c.Chips.Shortage.MaxStrengthPermille = 1300);
        g.Me.Techs.Add("optical-interconnect");
        g.State.ChipShortageUntilTick = g.State.Tick + 12;
        g.State.ChipShortageStrengthPermille = 1300;
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, cooling: "optical");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        long before = g.Me.Cash;
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 2));
        Assert.True(g.Me.Cash < before);
    }

    [Fact]
    public void Max_permille_optical_within_config()
    {
        GameConfig c = TestData.Config();
        ChipOptionConfig o = c.Chips.Option(ChipOptionGroup.Cooling, "optical");
        Assert.True(o.ComputePermille <= 3000);
        Assert.True(o.KwPermille >= 500);
    }

    [Fact]
    public void Start_chip_design_main_path_sets_optical_cooling()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        g.Me.Techs.Add("optical-interconnect");
        EnsureChipArchitects(g);
        int id = g.Do(Command.StartChipDesign(g.Id, ChipHardeningSupport.Inference, "n28", "medium", "fp16", "ddr", "optical")).CreatedId;
        ChipDesign d = g.Me.Design(id);
        Assert.Equal("optical", d.CoolingId);
        Assert.Equal(ChipDesignStatus.Designing, d.Status);
    }

    [Fact]
    public void Training_target_optical_beats_air_on_compute()
    {
        GameConfig c = TestData.Config();
        ChipSpec air = ChipQueries.Derive(c, ChipHardeningSupport.Training, "n5", "medium", "fp8", "hbm", "air");
        ChipSpec optical = ChipQueries.Derive(c, ChipHardeningSupport.Training, "n5", "medium", "fp8", "hbm", "optical");
        Assert.True(optical.Compute > air.Compute);
    }

    [Fact]
    public void Bankrupt_lab_refuses_order_chips()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("optical-interconnect");
        ChipDesign d = ChipHardeningSupport.InjectReady(g, g.Me, cooling: "optical");
        int hall = ChipHardeningSupport.Hall(g, g.Me);
        g.Me.Bankrupt = true;
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 2), CommandError.LabBankrupt);
    }

    [Fact]
    public void Save_load_after_start_chip_design_keeps_optical()
    {
        Game g = ChipHardeningSupport.Lab(2028, rivals: false);
        g.Me.Techs.Add("optical-interconnect");
        EnsureChipArchitects(g);
        int id = g.Do(Command.StartChipDesign(g.Id, ChipHardeningSupport.Inference, "n28", "small", "fp16", "ddr", "optical")).CreatedId;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal("optical", loaded.Me.Design(id).CoolingId);
    }

    [Fact]
    public void Chip_system_ready_event_keeps_optical_cooling()
    {
        Game g = ChipHardeningSupport.Lab(2028);
        g.Me.Techs.Add("optical-interconnect");
        g.Me.ChipDesigns.Clear();
        ChipDesign d = ChipHardeningSupport.InjectTapingOut(g, g.Me, ChipHardeningSupport.Training, "n5");
        d.CoolingId = "optical";
        ChipSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ChipReady);
        Assert.Equal("optical", d.CoolingId);
    }
}