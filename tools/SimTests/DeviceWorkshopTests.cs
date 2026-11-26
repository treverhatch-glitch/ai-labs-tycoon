using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q54: device workshop room boosts devices-sector appeal when the lab has a consumer release.</summary>
public class DeviceWorkshopTests
{
    const string Workshop = "device-workshop";
    const string Devices = "devices";

    static Game HarnessLab(int year = 2028)
    {
        Game g = ExpansionTestSupport.SoloAtYear(year);
        g.WithStarterLab(gpus: 24);
        return g;
    }

    [Fact]
    public void Shipped_device_workshop_config_and_room()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(Workshop, c.Market.DeviceWorkshop.RoomTypeId);
        Assert.NotNull(c.RoomType(Workshop));
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_bonus_before_harnesses_era()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2025);
        g.WithStarterLab(gpus: 8);
        g.Me.Rooms.Add(new RoomState { TypeId = Workshop, X = 0, Y = 0, Width = 4, Height = 4, Id = g.State.TakeId() });
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Room_alone_is_not_enough_without_released_devices_model()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Released_devices_without_room_gives_no_bonus()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Workshop_and_devices_release_gives_configured_appeal_permille()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(g.C.Market.DeviceWorkshop.AppealBonusPermille,
            DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices));
        Assert.True(DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model).WorkshopBoost > 0);
    }

    [Fact]
    public void Bonus_only_on_devices_sector_not_neighbors()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, "game-ai"));
        long devicesAppeal = DisplayQueries.Appeal(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model);
        long withoutWorkshop = devicesAppeal - DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model).WorkshopBoost;
        Assert.True(devicesAppeal > withoutWorkshop);
    }

    [Fact]
    public void Retired_devices_model_does_not_count()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        model.Status = ModelStatus.Retired;
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Higher_appeal_raises_devices_revenue_after_week()
    {
        Game g = HarnessLab();
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        long without = ExpansionTestSupport.SectorRevenueAfterWeek(g, Devices);
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        long with = ExpansionTestSupport.SectorRevenueAfterWeek(g, Devices);
        Assert.True(with >= without);
    }

    [Fact]
    public void Gauntlet_appeal_includes_workshop_when_configured()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        long appeal = DisplayQueries.Appeal(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model);
        long scaled = ExpansionSystem.ScaleAppeal(appeal, 0);
        Assert.True(appeal > scaled || g.C.Market.DeviceWorkshop.AppealBonusPermille > 0);
    }

    [Fact]
    public void Validator_rejects_unknown_workshop_room()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Market.DeviceWorkshop.RoomTypeId = "void-room";
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("deviceWorkshop") && p.Contains("room"));
    }

    [Fact]
    public void Validator_rejects_unknown_workshop_sector()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Market.DeviceWorkshop.SectorId = "void-sector";
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("deviceWorkshop") && p.Contains("sector"));
    }

    [Fact]
    public void Config_zero_bonus_still_validates()
    {
        GameConfig c = TestData.Config(cfg => cfg.Market.DeviceWorkshop.AppealBonusPermille = 0);
        ConfigValidator.Validate(c);
        Game g = ExpansionTestSupport.SoloAtYear(2028, tweak: cfg => cfg.Market.DeviceWorkshop.AppealBonusPermille = 0);
        g.WithStarterLab(gpus: 24);
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Max_appeal_permille_at_cap_scales_devices_appeal()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028, tweak: c => c.Market.DeviceWorkshop.AppealBonusPermille = 1000);
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        long appeal = DisplayQueries.Appeal(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model);
        long core = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model).Core
            + DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model).ReputationBoost
            + DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model).SalesBoost;
        Assert.True(appeal > ExpansionSystem.ScaleAppeal(core, 0));
    }

    [Fact]
    public void Bankrupt_lab_gets_no_workshop_bonus()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Finance.GoBankrupt(g.C, g.State, g.Me);
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices));
    }

    [Fact]
    public void Save_load_round_trip_preserves_workshop_appeal_boost()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        long boost = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(Devices), g.State.Sector(Devices), g.Me, model).WorkshopBoost;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState m2 = loaded.Me.Models.Single(x => x.Id == model.Id);
        long boost2 = DisplayQueries.AppealParts(loaded.C, loaded.State, loaded.C.Sector(Devices), loaded.State.Sector(Devices), loaded.Me, m2).WorkshopBoost;
        Assert.Equal(boost, boost2);
        Assert.True(boost2 > 0);
    }

    [Fact]
    public void Market_system_share_drifts_up_with_workshop_appeal()
    {
        Game g = HarnessLab();
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        g.Weeks(2);
        SectorEntry entry = g.State.Sector(Devices).Entry(g.Id);
        int shareBefore = entry.SharePpm;
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        g.Weeks(4);
        Assert.True(g.State.Sector(Devices).Entry(g.Id).SharePpm >= shareBefore);
    }

    [Fact]
    public void Workshop_bonus_stacks_with_expansion_on_other_sectors_only()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ExpansionTestSupport.BuildExpansionRoom(g, Workshop);
        ExpansionTestSupport.BuildExpansionRoom(g, "robotics-arena");
        ExpansionTestSupport.AddReleasedModel(g, 120_000, Devices);
        Assert.True(DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, Devices) > 0);
        Assert.Equal(0, DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, "robotics-industry"));
        Assert.True(ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, "robotics-industry") > 0);
    }
}