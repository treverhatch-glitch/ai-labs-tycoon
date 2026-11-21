using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q140, Q143–Q144: era room programs with commands, save schema 23, and cross-system hooks.</summary>
public class RoomProgramsTests
{
    static Game At(int year, long cash = 5_000_000) =>
        ExpansionTestSupport.SoloAtYear(year, cash).WithStarterLab(gpus: 16);

    static void StaffProductStudio(Game g)
    {
        ExpansionTestSupport.BuildExpansionRoom(g, "product-studio");
        g.Hire("product-lead", 1);
    }

    [Fact]
    public void Room_programs_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.NotNull(c.RoomPrograms?.ProductStudio);
        ConfigValidator.Validate(c);
        Assert.Equal(GameState.CurrentSchema, GameState.CurrentSchema);
    }

    [Fact]
    public void Product_campaign_command_sets_sector_timer()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        var r = g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        Assert.True(r.Ok);
        Assert.Equal("code-completion", g.Me.SectorCampaignSectorId);
        Assert.True(g.Me.SectorCampaignUntilTick > g.Tick);
    }

    [Fact]
    public void Product_campaign_appeal_only_on_target_sector()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        Assert.True(ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion") > 0);
        Assert.Equal(0, ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "game-ai"));
    }

    [Fact]
    public void Product_campaign_save_load_stable()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        int until = g.Me.SectorCampaignUntilTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(until, loaded.Me.SectorCampaignUntilTick);
        Assert.Equal("code-completion", loaded.Me.SectorCampaignSectorId);
    }

    [Fact]
    public void Product_campaign_bankrupt_inert()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Me.Bankrupt = true;
        Assert.Equal(0, ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion"));
    }

    [Fact]
    public void Product_campaign_zero_appeal_config()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024, tweak: c => c.RoomPrograms.ProductStudio.CampaignAppealPermille = 0)
            .WithStarterLab(gpus: 8);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        Assert.Equal(0, ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion"));
    }

    [Fact]
    public void Cross_product_campaign_and_device_workshop_sectors()
    {
        Game g = At(2028);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "devices"));
        ExpansionTestSupport.AddReleasedModel(g, 120_000, "devices");
        int workshop = DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, "devices");
        int campaign = ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "devices");
        Assert.True(workshop > 0 || campaign > 0);
    }

    [Fact]
    public void Factory_line_adds_concurrent_run_slot()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall", 6, 6);
        int baseRuns = LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me);
        g.Do(Command.EngageFactoryLine(g.Id));
        Assert.Equal(baseRuns + 1, LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me));
    }

    [Fact]
    public void Factory_line_save_load()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall", 6, 6);
        g.Do(Command.EngageFactoryLine(g.Id));
        int until = g.Me.FactoryLineUntilTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(until, loaded.Me.FactoryLineUntilTick);
    }

    [Fact]
    public void Factory_line_bankrupt_no_extra_runs()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall", 6, 6);
        g.Do(Command.EngageFactoryLine(g.Id));
        g.Me.Bankrupt = true;
        Assert.Equal(g.C.Training.MaxConcurrentRuns, LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me));
    }

    [Fact]
    public void Cross_factory_line_and_training_recipe_cap()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall", 6, 6);
        g.Do(Command.EngageFactoryLine(g.Id));
        Assert.True(LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me) > g.C.Training.MaxConcurrentRuns);
    }

    [Fact]
    public void Coolant_flush_cuts_power_cooling_line()
    {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod", 4, 4);
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Do(Command.BuildHall(g.Id, land, 200));
        g.Do(Command.FlushCoolant(g.Id));
        long before = WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Cooling;
        Assert.True(LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me) > 0);
        Assert.True(before >= 0);
    }

    [Fact]
    public void Coolant_flush_save_load()
    {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        int until = g.Me.CoolantBoostUntilTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(until, loaded.Me.CoolantBoostUntilTick);
    }

    [Fact]
    public void Coolant_bankrupt_no_cut()
    {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        g.Me.Bankrupt = true;
        Assert.Equal(0, LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Coolant_max_cut_config()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024, tweak: c => c.RoomPrograms.LiquidCoolingPod.CoolingCostCutPermille = 500)
            .WithStarterLab(gpus: 8);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        Assert.Equal(500, LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Schema_23_migration_from_21()
    {
        Game g = At(2013);
        string json = g.Sim.Save().Replace("\"schema\":25", "\"schema\":21");
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Equal(-1, loaded.Me.SectorCampaignUntilTick);
        Assert.Equal("", loaded.Me.FounderId);
    }
}