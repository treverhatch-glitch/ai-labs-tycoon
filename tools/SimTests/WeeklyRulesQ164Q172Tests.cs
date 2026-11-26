using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q164–Q172 weekly rule systems (room programs, champions, listings, vendor, heatwave, peer debt, stash).</summary>
public class WeeklyRulesQ164Q172Tests
{
    static Game At(int year, long cash = 5_000_000, System.Action<GameConfig> tweak = null) =>
        ExpansionTestSupport.SoloAtYear(year, cash, tweak).WithStarterLab(gpus: 16);

    static void AddReadyListing(LabState lab, int designId, int listedSinceTick, bool spotlight = false)
    {
        lab.ChipDesigns.Add(new ChipDesign { Id = designId, Status = ChipDesignStatus.Ready, Name = "chip-" + designId });
        lab.ChipListings.Add(new ChipListing
        {
            DesignId = designId,
            PricePerChip = 100,
            MaxPerMonth = 10,
            ListedSinceTick = listedSinceTick,
            Spotlight = spotlight
        });
    }

    static Holding HeatwaveHall(Game g, int hallKw)
    {
        string siteId = g.C.World.Sites[0].Id;
        int kw = hallKw < 50 ? 50 : hallKw;
        Holding hall = WorldTests.Hall(g, siteId, kw);
        g.State.Site(siteId).HeatwaveUntilTick = g.Tick + 4;
        return hall;
    }

    static void StaffProductStudio(Game g)
    {
        ExpansionTestSupport.BuildExpansionRoom(g, "product-studio");
        g.Hire("product-lead", 1);
    }

    // —— Q164 Product studio campaign weekly ——
    [Fact]
    public void Q164_main_marketing_minus_upkeep_while_active()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        long before = g.Me.Cash;
        long net = g.C.RoomPrograms.ProductStudio.CampaignWeeklyMarketingCash
            - g.C.RoomPrograms.ProductStudio.CampaignWeeklyUpkeepCash;
        ProductStudioSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + net, g.Me.Cash);
    }

    [Fact]
    public void Q164_zero_weekly_amounts_no_cash_change()
    {
        Game g = At(2024, tweak: c =>
        {
            c.RoomPrograms.ProductStudio.CampaignWeeklyMarketingCash = 0;
            c.RoomPrograms.ProductStudio.CampaignWeeklyUpkeepCash = 0;
        });
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        long before = g.Me.Cash;
        ProductStudioSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q164_max_campaign_end_emits_event()
    {
        Game g = At(2024, tweak: c => c.RoomPrograms.ProductStudio.CampaignWeeks = 1);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Weeks(1);
        ProductStudioSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal("", g.Me.SectorCampaignSectorId);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SectorCampaignEnded && e.Ref == "code-completion");
    }

    [Fact]
    public void Q164_bankrupt_skips_without_throw()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Me.Bankrupt = true;
        ProductStudioSystem.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Q164_save_load_campaign_weekly()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        ProductStudioSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        long before = loaded.Me.Cash;
        ProductStudioSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.True(loaded.Me.Cash != before);
    }

    [Fact]
    public void Q164_cross_appeal_still_applies_during_weekly()
    {
        Game g = At(2024);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        Assert.True(ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion") > 0);
        ProductStudioSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(ProductStudioSystem.CampaignAppealPermille(g.C, g.State, g.Me, "code-completion") > 0);
    }

    [Fact]
    public void Q164_max_long_campaign_weeks_still_expires()
    {
        Game g = At(2024, tweak: c => c.RoomPrograms.ProductStudio.CampaignWeeks = 48);
        StaffProductStudio(g);
        g.Do(Command.LaunchSectorCampaign(g.Id, "code-completion"));
        g.Weeks(49);
        Assert.Equal("", g.Me.SectorCampaignSectorId);
    }

    // —— Q165 AI factory hall weekly ——
    static void StaffFactory(Game g)
    {
        ExpansionTestSupport.BuildExpansionRoom(g, "ai-factory-hall");
    }

    [Fact]
    public void Q165_main_factory_upkeep_and_research()
    {
        Game g = At(2026);
        StaffFactory(g);
        g.Do(Command.EngageFactoryLine(g.Id));
        long cash = g.Me.Cash;
        long rp = g.Me.ResearchPoints;
        AiFactoryHallSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash - g.C.RoomPrograms.AiFactoryHall.FactoryWeeklyUpkeepCash, g.Me.Cash);
        Assert.Equal(rp + g.C.RoomPrograms.AiFactoryHall.FactoryWeeklyResearchPoints, g.Me.ResearchPoints);
    }

    [Fact]
    public void Q165_zero_when_line_inactive()
    {
        Game g = At(2026);
        StaffFactory(g);
        long cash = g.Me.Cash;
        AiFactoryHallSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact]
    public void Q165_bankrupt_skips()
    {
        Game g = At(2026);
        StaffFactory(g);
        g.Do(Command.EngageFactoryLine(g.Id));
        g.Me.Bankrupt = true;
        AiFactoryHallSystem.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Q165_save_load_factory_weekly()
    {
        Game g = At(2026);
        StaffFactory(g);
        g.Do(Command.EngageFactoryLine(g.Id));
        AiFactoryHallSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        long rp = loaded.Me.ResearchPoints;
        AiFactoryHallSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(rp + loaded.C.RoomPrograms.AiFactoryHall.FactoryWeeklyResearchPoints, loaded.Me.ResearchPoints);
    }

    [Fact]
    public void Q165_cross_extra_concurrent_slot_while_active()
    {
        Game g = At(2026);
        StaffFactory(g);
        g.Do(Command.EngageFactoryLine(g.Id));
        Assert.True(AiFactoryHallSystem.ExtraConcurrentRuns(g.C, g.State, g.Me) > 0);
    }

    [Fact]
    public void Q165_max_factory_rp_at_config_saturation()
    {
        Game g = At(2026, tweak: c => c.RoomPrograms.AiFactoryHall.FactoryWeeklyResearchPoints = 99);
        StaffFactory(g);
        g.Do(Command.EngageFactoryLine(g.Id));
        long rp = g.Me.ResearchPoints;
        AiFactoryHallSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(rp + 99, g.Me.ResearchPoints);
    }

    // —— Q166 Liquid cooling fouling + flush rebate ——
    [Fact]
    public void Q166_main_flush_rebate_while_active()
    {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        long before = g.Me.Cash;
        long expected = LiquidCoolingPodSystem.WeeklyFlushRebate(g.C, g.State, g.Me);
        LiquidCoolingPodSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + expected, g.Me.Cash);
    }

    [Fact]
    public void Q166_zero_rebate_disabled()
    {
        Game g = At(2024, tweak: c => c.RoomPrograms.LiquidCoolingPod.FlushRebatePerRentedGpu = 0);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        long before = g.Me.Cash;
        LiquidCoolingPodSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q166_bankrupt_skips()
    {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        g.Me.Bankrupt = true;
        LiquidCoolingPodSystem.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Q166_cross_cooling_cut_still_applies()
    {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Do(Command.FlushCoolant(g.Id));
        Assert.True(LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me) > 0);
    }

    [Fact]
    public void Q166_max_fouling_caps_cooling_bonus()
    {
        Game g = At(2024, tweak: c =>
        {
            c.RoomPrograms.LiquidCoolingPod.MaxFoulingPermille = 200;
            c.RoomPrograms.LiquidCoolingPod.FoulingGainPermillePerWeek = 100;
        });
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Me.CoolantFoulingPermille = 200;
        int cut = LiquidCoolingPodSystem.CoolingCostCutPermille(g.C, g.State, g.Me);
        Assert.True(cut < g.C.RoomPrograms.LiquidCoolingPod.CoolingCostCutPermille);
    }

    [Fact]
    public void Q166_save_load_fouling_persists()
    {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        LiquidCoolingPodSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(g.Me.CoolantFoulingPermille, loaded.Me.CoolantFoulingPermille);
    }

  [Fact]
  public void Q166_fouling_resets_on_flush()
  {
        Game g = At(2024);
        ExpansionTestSupport.BuildExpansionRoom(g, "liquid-cooling-pod");
        g.Me.CoolantFoulingPermille = 120;
        g.Do(Command.FlushCoolant(g.Id));
        Assert.Equal(0, g.Me.CoolantFoulingPermille);
  }

    // —— Q167 Sector champion stipend ——
    [Fact]
    public void Q167_main_champion_cash_per_sector()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Sector("game-ai").ChampionLabId = g.Id;
        long before = g.Me.Cash;
        SectorChampionSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Market.SectorChampionCashPerWeek, g.Me.Cash);
    }

    [Fact]
    public void Q167_zero_no_champion_no_pay()
    {
        Game g = Game.Solo().WithStarterLab();
        long before = g.Me.Cash;
        SectorChampionSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q167_max_two_champion_sectors()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Sector("game-ai").ChampionLabId = g.Id;
        g.State.Sector("code-completion").ChampionLabId = g.Id;
        Assert.Equal(2, SectorChampionSystem.ChampionSectorCount(g.State, g.Id));
        long pay = SectorChampionSystem.WeeklyStipend(g.C, g.State, g.Me);
        SectorChampionSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Market.SectorChampionCashPerWeek * 2, pay);
    }

    [Fact]
    public void Q167_bankrupt_skips()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Sector("game-ai").ChampionLabId = g.Id;
        g.Me.Bankrupt = true;
        SectorChampionSystem.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Q167_save_load_champion_stipend()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Sector("game-ai").ChampionLabId = g.Id;
        SectorChampionSystem.RunWeek(g.C, g.State, g.Me);
        long after = g.Me.Cash;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        SectorChampionSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(after + loaded.C.Market.SectorChampionCashPerWeek, loaded.Me.Cash);
    }

    // —— Q168 Chip listing market rules ——
    [Fact]
    public void Q168_main_listing_upkeep_per_listing()
    {
        Game g = Game.Solo().WithStarterLab();
        AddReadyListing(g.Me, 1, 0);
        long before = g.Me.Cash;
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - g.C.Chips.ListingWeeklyUpkeep, g.Me.Cash);
    }

    [Fact]
    public void Q168_zero_upkeep_disabled()
    {
        Game g = Game.Solo(tweak: c => c.Chips.ListingWeeklyUpkeep = 0).WithStarterLab();
        AddReadyListing(g.Me, 1, 0);
        long before = g.Me.Cash;
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q168_max_spotlight_multiplies_upkeep()
    {
        Game g = Game.Solo().WithStarterLab();
        AddReadyListing(g.Me, 1, 0, spotlight: true);
        long fee = AiLabs.Rules.Core.MathI.MulPermille(g.C.Chips.ListingWeeklyUpkeep, g.C.Chips.ListingSpotlightUpkeepPermille);
        long before = g.Me.Cash;
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - fee, g.Me.Cash);
    }

    [Fact]
    public void Q168_bankrupt_still_skips_spend_when_broke()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Cash = 0;
        AddReadyListing(g.Me, 1, 0);
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.Cash);
    }

    [Fact]
    public void Q168_save_load_spotlight_command()
    {
        Game g = ChipHardeningSupport.Lab(2024);
        ChipDesign design = ChipHardeningSupport.InjectReady(g, g.Me);
        g.Do(Command.SetChipListing(g.Id, design.Id, 100, 10));
        g.Do(Command.SpotlightChipListing(g.Id, design.Id, true));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.True(loaded.Me.Listing(design.Id).Spotlight);
    }

    [Fact]
    public void Q168_cross_over_cap_drops_oldest_listing()
    {
        Game g = Game.Solo(tweak: c => c.Chips.ListingMaxActive = 1).WithStarterLab();
        AddReadyListing(g.Me, 1, 1);
        AddReadyListing(g.Me, 2, 5);
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Single(g.Me.ChipListings);
        Assert.Equal(2, g.Me.ChipListings[0].DesignId);
    }

    // —— Q169 Vendor standing weekly grant ——
    [Fact]
    public void Q169_main_high_standing_grant()
    {
        Game g = Game.Solo().WithStarterLab();
        VendorStanding s = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        s.Standing = 500;
        long before = g.Me.Cash;
        VendorSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Vendor.WeeklyGrantPerStanding * 500, g.Me.Cash);
    }

    [Fact]
    public void Q169_zero_below_threshold()
    {
        Game g = Game.Solo().WithStarterLab();
        VendorStanding s = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        s.Standing = g.C.Vendor.WeeklyGrantStandingAbove;
        long before = g.Me.Cash;
        VendorSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q169_bankrupt_skips()
    {
        Game g = Game.Solo().WithStarterLab();
        VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing = 600;
        g.Me.Bankrupt = true;
        VendorSystem.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Q169_save_load_vendor_grant()
    {
        Game g = Game.Solo().WithStarterLab();
        VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing = 500;
        VendorSystem.RunWeek(g.C, g.State, g.Me);
        long after = g.Me.Cash;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        VendorSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(after + loaded.C.Vendor.WeeklyGrantPerStanding * 500, loaded.Me.Cash);
    }

    [Fact]
    public void Q169_cross_patent_license_same_week()
    {
        Game g = Game.New().WithStarterLab();
        VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing = 500;
        g.Sim.Step();
        Assert.True(g.Me.Ledger.LastWeek.Grants > 0);
    }

    [Fact]
    public void Q169_zero_rival_pre_plan_start()
    {
        Game g = Game.New(seed: 1);
        LabState rival = g.Rival("xay");
        VendorSystem.EnsureStanding(g.C, g.State, rival.Id).Standing = 600;
        Assert.True(rival.PlanStartTick > g.Tick);
        long cash = rival.Cash;
        VendorSystem.RunWeek(g.C, g.State, rival);
        Assert.Equal(cash, rival.Cash);
    }

    [Fact]
    public void Q169_max_standing_at_permille_cap()
    {
        Game g = Game.Solo().WithStarterLab();
        VendorSystem.EnsureStanding(g.C, g.State, g.Id).Standing = 1000;
        long before = g.Me.Cash;
        VendorSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before + g.C.Vendor.WeeklyGrantPerStanding * 1000, g.Me.Cash);
    }

    // —— Q170 Heatwave emergency power ——
    [Fact]
    public void Q170_main_surcharge_during_heatwave()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = HeatwaveHall(g, 0);
        hall.PrivateKw = 100;
        long before = g.Me.Cash;
        long pay = HeatwavePowerSystem.WeeklySurcharge(g.C, g.State, g.Me);
        HeatwavePowerSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - pay, g.Me.Cash);
    }

    [Fact]
    public void Q170_zero_no_heatwave_no_fee()
    {
        Game g = Game.Solo().WithStarterLab();
        long before = g.Me.Cash;
        HeatwavePowerSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, g.Me.Cash);
    }

    [Fact]
    public void Q170_max_large_hall_kw_surcharge()
    {
        Game g = WorldTests.Rich("punchline", tweak: c => c.World.Heatwave.WeeklySurchargeCashPerKw = 20);
        HeatwaveHall(g, 500);
        Assert.Equal(10_000, HeatwavePowerSystem.WeeklySurcharge(g.C, g.State, g.Me));
    }

    [Fact]
    public void Q170_bankrupt_skips()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Site(g.C.World.Sites[0].Id).HeatwaveUntilTick = g.Tick + 2;
        g.Me.Bankrupt = true;
        HeatwavePowerSystem.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Q170_save_load_heatwave_fee()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = HeatwaveHall(g, 0);
        hall.PrivateKw = 50;
        HeatwavePowerSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        long pay = HeatwavePowerSystem.WeeklySurcharge(loaded.C, loaded.State, loaded.Me);
        long before = loaded.Me.Cash;
        HeatwavePowerSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(before - pay, loaded.Me.Cash);
    }

    [Fact]
    public void Q170_cross_matches_power_ledger()
    {
        Game g = WorldTests.Rich("punchline");
        Holding hall = HeatwaveHall(g, 0);
        hall.PrivateKw = 10;
        Finance.Spend(g.Me, LedgerLine.Power, HeatwavePowerSystem.WeeklySurcharge(g.C, g.State, g.Me));
        Assert.True(g.Me.Ledger.ThisWeek.Power < 0);
    }

    // —— Q171 Peer lease debt enforcement ——
    [Fact]
    public void Q171_main_trims_gpu_while_in_debt()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 2, GpuCount = 4 });
        g.Me.WeeksInDebt = 2;
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(3, g.Me.PeerComputeLeases[0].GpuCount);
    }

    [Fact]
    public void Q171_zero_when_solvent()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 2, GpuCount = 4 });
        g.Me.WeeksInDebt = 0;
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(4, g.Me.PeerComputeLeases[0].GpuCount);
    }

    [Fact]
    public void Q171_max_trims_until_lease_gone()
    {
        Game g = Game.Solo(tweak: c => c.Compute.PeerLeaseDebtGpuTrimPerWeek = 2).WithStarterLab();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 2, GpuCount = 2 });
        g.Me.WeeksInDebt = 1;
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.PeerComputeLeases);
    }

    [Fact]
    public void Q171_bankrupt_skips()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 2, GpuCount = 2 });
        g.Me.WeeksInDebt = 3;
        g.Me.Bankrupt = true;
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(2, g.Me.PeerComputeLeases[0].GpuCount);
    }

    [Fact]
    public void Q171_save_load_debt_trim()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 2, GpuCount = 3 });
        g.Me.WeeksInDebt = 1;
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Me.WeeksInDebt = 1;
        PeerLeaseEnforcementSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.Equal(1, loaded.Me.PeerComputeLeases[0].GpuCount);
    }

    [Fact]
    public void Q171_cross_emits_trim_event()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 2, GpuCount = 2 });
        g.Me.WeeksInDebt = 1;
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.PeerLeaseTrimmed);
    }

    // —— Q172 Stash storage fee ——
    [Fact]
    public void Q172_main_stash_storage_fee()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Stash = 100_000;
        long before = g.Me.Stash;
        long fee = StashStorageSystem.WeeklyFee(g.C, g.Me);
        StashStorageSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - fee, g.Me.Stash);
    }

    [Fact]
    public void Q172_zero_empty_stash()
    {
        Game g = Game.Solo().WithStarterLab();
        StashStorageSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.Stash);
    }

    [Fact]
    public void Q172_max_large_stash_fee()
    {
        Game g = Game.Solo(tweak: c => c.Shady.StashStoragePermillePerWeek = 100).WithStarterLab();
        g.Me.Stash = 1_000_000;
        Assert.Equal(100_000, StashStorageSystem.WeeklyFee(g.C, g.Me));
    }

    [Fact]
    public void Q172_bankrupt_skips()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Stash = 50_000;
        g.Me.Bankrupt = true;
        StashStorageSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(50_000, g.Me.Stash);
    }

    [Fact]
    public void Q172_save_load_stash_fee()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Stash = 80_000;
        StashStorageSystem.RunWeek(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        long before = loaded.Me.Stash;
        StashStorageSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.True(loaded.Me.Stash < before);
    }

    [Fact]
    public void Q172_cross_launder_avoids_cash_hit_not_stash_fee()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Me.Stash = 20_000;
        g.Me.Heat = 0;
        g.Do(Command.Launder(g.Id, 5_000));
        Assert.True(g.Me.Stash < 20_000);
    }
}