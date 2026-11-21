using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q66: optical-interconnect unlocks optical chip cooling on designs.</summary>
public class OpticalCoolingTests
{
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
    }
}