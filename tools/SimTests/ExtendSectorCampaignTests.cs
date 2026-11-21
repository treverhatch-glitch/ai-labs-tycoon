using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q150: pay to extend an active product-studio sector campaign.</summary>
public class ExtendSectorCampaignTests
{
    static Game At() => ExpansionTestSupport.SoloAtYear(2024, 2_000_000).WithStarterLab(gpus: 16);

    static void Studio(Game g)
    {
        ExpansionTestSupport.BuildExpansionRoom(g, "product-studio");
        g.Hire("product-lead", 1);
    }

    [Fact]
    public void Extend_adds_weeks_to_timer()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        int before = g.Me.SectorCampaignUntilTick;
        g.Do(Command.ExtendSectorCampaign(g.Id));
        Assert.Equal(before + g.C.RoomPrograms.ProductStudio.ExtendCampaignWeeks, g.Me.SectorCampaignUntilTick);
    }

    [Fact]
    public void Extend_keeps_appeal_active()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        Assert.True(ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion") > 0);
    }

    [Fact]
    public void Extend_refused_without_campaign()
    {
        Game g = At();
        Studio(g);
        g.Refused(Command.ExtendSectorCampaign(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Extend_bankrupt_inert()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Me.Bankrupt = true;
        g.Refused(Command.ExtendSectorCampaign(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Extend_zero_weeks_disabled()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024, tweak: c => c.RoomPrograms.ProductStudio.ExtendCampaignWeeks = 0).WithStarterLab();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Refused(Command.ExtendSectorCampaign(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Extend_save_load()
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
    public void Cross_extend_then_end_campaign()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Do(Command.ExtendSectorCampaign(g.Id));
        g.Do(Command.EndSectorCampaign(g.Id));
        Assert.Equal(-1, g.Me.SectorCampaignUntilTick);
    }

    [Fact]
    public void Extend_spends_cash()
    {
        Game g = At();
        Studio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        long cash = g.Me.Cash;
        g.Do(Command.ExtendSectorCampaign(g.Id));
        Assert.Equal(cash - g.C.RoomPrograms.ProductStudio.ExtendCampaignCashCost, g.Me.Cash);
    }
}