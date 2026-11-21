using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q83: device workshop boosts devices-sector revenue when gated.</summary>
public class DeviceWorkshopRevenueTests
{
    const string Workshop = "device-workshop";
    const string Devices = "devices";

    static Game HarnessLab(int year = 2028) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

    [Fact]
    public void Shipped_workshop_revenue_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(80, c.Market.DeviceWorkshop.RevenueBonusPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_revenue_bonus_without_room_or_release()
    {
        Game g = HarnessLab();
        Assert.Equal(0, DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Workshop_plus_release_enables_revenue_bonus()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(g.C.Market.DeviceWorkshop.RevenueBonusPermille,
            DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Revenue_bonus_only_on_devices_sector()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(0, DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, "game-ai"));
    }

    [Fact]
    public void Weekly_revenue_higher_with_workshop_gate()
    {
        Game bare = HarnessLab();
        Game wired = HarnessLab();
        ModelState mBare = ExpansionTestSupport.AddReleasedModel(bare, 120_000, Devices);
        ExpansionTestSupport.BuildExpansionRoom(wired, Workshop);
        ModelState mWired = ExpansionTestSupport.AddReleasedModel(wired, 120_000, Devices);
        bare.Weeks(4);
        wired.Weeks(4);
        long revBare = bare.State.Sector(Devices).Entry(bare.Id).LastRevenue;
        long revWired = wired.State.Sector(Devices).Entry(wired.Id).LastRevenue;
        Assert.True(revWired > revBare);
    }

    [Fact]
    public void Zero_revenue_config_disables_hook()
    {
        Game g = HarnessLab();
        g.C.Market.DeviceWorkshop.RevenueBonusPermille = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(0, DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Bankrupt_lab_no_workshop_revenue_bonus()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        g.Me.Bankrupt = true;
        Assert.Equal(0, DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Before_harnesses_era_no_bonus()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2025).WithStarterLab(gpus: 8);
        g.Me.Rooms.Add(new RoomState { TypeId = Workshop, X = 0, Y = 0, Width = 4, Height = 4, Id = g.State.TakeId() });
        Assert.Equal(0, DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Save_load_preserves_revenue_bonus_query()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        int before = DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(before, DeviceWorkshopSystem.RevenueBonusPermille(loaded.C, loaded.State, loaded.Me, Devices));
    }

    [Fact]
    public void Appeal_bonus_independent_of_revenue_bonus()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.NotEqual(DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices),
            DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Lab_last_week_revenue_reflects_workshop_on_devices()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        g.Weeks(3);
        Assert.True(g.Me.LastWeek.Revenue > 0);
        Assert.Equal(g.Me.LastWeek.Revenue, g.State.Sector(Devices).Entry(g.Id).LastRevenue);
    }

    [Fact]
    public void Released_other_sector_not_enough_for_devices_revenue_gate()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, "code-completion");
        Assert.Equal(0, DeviceWorkshopSystem.RevenueBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Market_week_applies_bonus_in_entry_last_revenue()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        g.Weeks(1);
        Game control = HarnessLab();
        ExpansionTestSupport.AddReleasedModel(control, 120_000, Devices);
        control.Weeks(1);
        Assert.True(g.Me.LastWeek.Revenue >= control.Me.LastWeek.Revenue);
    }
}