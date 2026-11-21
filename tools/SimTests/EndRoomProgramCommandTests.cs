using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q146–Q148: player commands to end active room programs early.</summary>
public class EndRoomProgramCommandTests
{
    static Game At(int year = 2024) => ExpansionTestSupport.SoloAtYear(year, 2_000_000).WithStarterLab(gpus: 16);

    static void StaffProductStudio(Game g)
    {
        ExpansionTestSupport.BuildExpansionRoom(g, "product-studio");
        g.Hire("product-lead", 1);
    }

    // —— Q146 EndSectorCampaign ——

    [Fact]
    public void Q146_main_end_clears_sector_campaign_and_appeal()
    {
        Game g = At();
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.EndSectorCampaign(g.Id));
        Assert.Equal(-1, g.Me.SectorCampaignUntilTick);
        Assert.Equal("", g.Me.SectorCampaignSectorId);
        Assert.Equal(0, ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion"));
    }

    [Fact]
    public void Q146_zero_refused_when_no_campaign_running()
    {
        Game g = At();
        StaffProductStudio(g);
        g.Refused(Command.EndSectorCampaign(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q146_max_double_end_second_refused()
    {
        Game g = At();
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.EndSectorCampaign(g.Id));
        g.Refused(Command.EndSectorCampaign(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q146_bankrupt_refused_even_if_timer_active()
    {
        Game g = At();
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Me.Bankrupt = true;
        g.Refused(Command.EndSectorCampaign(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q146_save_load_after_end()
    {
        Game g = At();
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.EndSectorCampaign(g.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(-1, loaded.Me.SectorCampaignUntilTick);
    }

    [Fact]
    public void Q146_cross_extend_then_end_stops_appeal()
    {
        Game g = At();
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        g.Do(Command.EndSectorCampaign(g.Id));
        Assert.Equal(0, ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion"));
    }

    // —— Q147 EndFactoryLine ——

    [Fact]
    public void Q147_main_end_restores_concurrent_run_cap()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall", 6, 6);
        int baseRuns = LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me);
        g.Do(Command.EngageFactoryLine(g.Id));
        g.Do(Command.EndFactoryLine(g.Id));
        Assert.Equal(baseRuns, LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me));
    }

    [Fact]
    public void Q147_zero_refused_without_factory_timer()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall");
        g.Refused(Command.EndFactoryLine(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q147_max_config_extra_runs_drops_after_end()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028, tweak: c => c.RoomPrograms.AiFactoryHall.ExtraConcurrentRuns = 3).WithStarterLab();
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall", 6, 6);
        int baseRuns = LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me);
        g.Do(Command.EngageFactoryLine(g.Id));
        Assert.Equal(baseRuns + 3, LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me));
        g.Do(Command.EndFactoryLine(g.Id));
        Assert.Equal(baseRuns, LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me));
    }

    [Fact]
    public void Q147_bankrupt_refused_while_engaged()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall");
        g.Do(Command.EngageFactoryLine(g.Id));
        g.Me.Bankrupt = true;
        g.Refused(Command.EndFactoryLine(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q147_save_load_factory_timer_cleared()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall");
        g.Do(Command.EngageFactoryLine(g.Id));
        g.Do(Command.EndFactoryLine(g.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(-1, loaded.Me.FactoryLineUntilTick);
    }

    [Fact]
    public void Q147_cross_factory_end_then_training_recipe_cap()
    {
        Game g = At(2028);
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall");
        g.Do(Command.EngageFactoryLine(g.Id));
        g.Do(Command.EndFactoryLine(g.Id));
        Assert.Equal(g.C.Training.MaxConcurrentRuns, LabQueries.MaxConcurrentRuns(g.C, g.State, g.Me));
    }

    // —— Q148 EndCoolantFlush ——

    [Fact]
    public void Q148_main_end_clears_cooling_cut()
    {
        Game g = At();
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        g.Do(Command.EndCoolantFlush(g.Id));
        Assert.Equal(0, LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Q148_zero_refused_without_flush()
    {
        Game g = At();
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Refused(Command.EndCoolantFlush(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q148_max_cut_config_cleared_on_end()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024, tweak: c => c.RoomPrograms.LiquidCoolingPod.CoolingCostCutPermille = 500)
            .WithStarterLab(gpus: 8);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        Assert.Equal(500, LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me));
        g.Do(Command.EndCoolantFlush(g.Id));
        Assert.Equal(0, LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Q148_bankrupt_refused_during_flush()
    {
        Game g = At();
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        g.Me.Bankrupt = true;
        g.Refused(Command.EndCoolantFlush(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q148_save_load_flush_cleared()
    {
        Game g = At();
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        g.Do(Command.EndCoolantFlush(g.Id));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(-1, loaded.Me.CoolantBoostUntilTick);
    }

    [Fact]
    public void Q148_cross_end_then_power_bill_unaffected_by_cut()
    {
        Game g = At();
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod", 4, 4);
        g.Do(Command.BuyLand(g.Id, "us-oregon"));
        g.Do(Command.FlushCoolant(g.Id));
        g.Do(Command.EndCoolantFlush(g.Id));
        Assert.Equal(0, LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me));
        Assert.True(WorldQueries.WeeklyPowerBillTotals(g.C, g.State, g.Me).Cooling >= 0);
    }
}