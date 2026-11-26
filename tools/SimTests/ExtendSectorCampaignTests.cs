using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q150: pay to extend an active product-studio sector campaign.</summary>
public class ExtendSectorCampaignTests
{
    static Game At(int year = 2024) => ExpansionTestSupport.SoloAtYear(year, 2_000_000).WithStarterLab(gpus: 16);

    static void Studio(Game g)
    {
        ExpansionTestSupport.BuildExpansionRoom(g, "product-studio");
        g.Hire("product-lead", 1);
    }

    [Fact]
    public void Q150_main_extend_adds_weeks_to_timer()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        int before = g.Me.SectorCampaignUntilTick;
        g.Do(Command.ExtendSectorCampaign(g.Id));
        Assert.Equal(before + g.C.RoomPrograms.ProductStudio.ExtendCampaignWeeks, g.Me.SectorCampaignUntilTick);
    }

    [Fact]
    public void Q150_main_extend_keeps_appeal_active()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        Assert.True(ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion") > 0);
    }

    [Fact]
    public void Q150_zero_refused_without_campaign()
    {
        Game g = At();
        Studio(g);
        g.Refused(Command.ExtendSectorCampaign(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q150_bankrupt_refused()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Me.Bankrupt = true;
        g.Refused(Command.ExtendSectorCampaign(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q150_zero_weeks_extension_disabled()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024, tweak: c => c.RoomPrograms.ProductStudio.ExtendCampaignWeeks = 0).WithStarterLab();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Refused(Command.ExtendSectorCampaign(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q150_save_load_extended_timer()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        int until = g.Me.SectorCampaignUntilTick;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(until, loaded.Me.SectorCampaignUntilTick);
    }

    [Fact]
    public void Q150_cross_extend_then_end_campaign()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        g.Do(Command.EndSectorCampaign(g.Id));
        Assert.Equal(-1, g.Me.SectorCampaignUntilTick);
    }

    [Fact]
    public void Q150_main_extend_spends_cash()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        long cash = g.Me.Cash;
        g.Do(Command.ExtendSectorCampaign(g.Id));
        Assert.Equal(cash - g.C.RoomPrograms.ProductStudio.ExtendCampaignCashCost, g.Me.Cash);
    }

    [Fact]
    public void Q150_max_double_extend_stacks_weeks()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        int baseUntil = g.Me.SectorCampaignUntilTick;
        g.Do(Command.ExtendSectorCampaign(g.Id));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        int weeks = g.C.RoomPrograms.ProductStudio.ExtendCampaignWeeks;
        Assert.Equal(baseUntil + weeks * 2, g.Me.SectorCampaignUntilTick);
    }

    [Fact]
    public void Q150_cross_campaign_and_device_workshop_devices_sector()
    {
        Game g = At(2028);
        Studio(g);
        ExpansionTestSupport.BuildExpansionRoom(g, "device-workshop", 4, 4);
        ExpansionTestSupport.AddReleasedModel(g, 120_000, "devices");
        g.Do(Command.LaunchSectorCampaign(g.Id, "devices"));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        int workshop = DeviceWorkshopSystem.AppealBonusPermille(g.C, g.State, g.Me, "devices");
        int campaign = ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "devices");
        Assert.True(workshop + campaign > 0);
    }
}