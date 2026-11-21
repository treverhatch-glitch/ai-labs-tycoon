using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class CloudDealTests
{
    const string Training = "training";

    static Game SellerBuyerPair(long cash = 50_000_000)
    {
        var options = new NewGameOptions { Seed = 11, StartEraId = "punchline" };
        options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.World.Attention.DominanceFromPpm = MathI.Ppm), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2024);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, cash);
        foreach (string tech in new[] { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling" })
            if (!g.Me.Techs.Contains(tech)) g.Me.Techs.Add(tech);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        return g;
    }

    static ChipDesign ReadyDesign(Game g, bool trains = true)
    {
        string target = trains ? Training : "inference";
        ChipSpec spec = ChipQueries.Derive(g.C, target, "n28", "medium", "fp16", "ddr", "air");
        var d = new ChipDesign();
        d.Id = g.State.TakeId();
        d.Version = ChipQueries.NextVersion(g.Me);
        d.Name = g.Me.FamilyName + " Silicon " + d.Version;
        d.Target = target;
        d.NodeId = "n28";
        d.DieId = "medium";
        d.FormatId = "fp16";
        d.MemoryId = "ddr";
        d.CoolingId = "air";
        d.PointsRequired = spec.Points;
        d.PointsDone = spec.Points;
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = 900;
        d.StageEndsTick = -1;
        d.ReadyTick = g.Tick;
        g.Me.ChipDesigns.Add(d);
        if (g.Me.SoftwarePermille == 0) g.Me.SoftwarePermille = g.C.Chips.SoftwareStart;
        return d;
    }

    static LabState RentOnlyBuyer(Game g)
    {
        LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("openaye"), EraSystem.AtTick(g.C, g.Tick), true);
        buyer.PlanStartTick = 0;
        Finance.Earn(buyer, LedgerLine.CapitalIn, 5_000_000_000);
        Assert.True(g.C.Rival("openaye").Plan.UsesCloudDeals);
        Assert.Empty(buyer.Holdings);
        return buyer;
    }

    [Fact]
    public void A_rent_only_lab_can_buy_listed_chips_into_the_cloud()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 10_000, 100));
        LabState buyer = RentOnlyBuyer(g);
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 12));
        Assert.Single(buyer.ChipOrders);
        Assert.Equal(HardwareBatch.Cloud, buyer.ChipOrders[0].HoldingId);
        Assert.Equal(0, buyer.ChipOrders[0].ReservedKw);
    }

    [Fact]
    public void Cloud_delivery_is_refused_when_the_buyer_owns_a_data_centre()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 50));
        LabState buyer = RentOnlyBuyer(g);
        int land = g.Do(Command.BuyLand(buyer.Id, g.C.World.Sites[0].Id)).CreatedId;
        buyer.Holding(land).HallKw = 500_000;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void Delivered_cloud_chips_feed_general_compute_without_a_site()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 8000, 50));
        LabState buyer = RentOnlyBuyer(g);
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 20));
        int lead = g.C.Chips.Node(d.NodeId).LeadWeeks;
        g.Weeks(lead + 4);
        HardwareBatch batch = buyer.Fleet.Single(b => b.ChipDesignId == d.Id);
        Assert.Equal(HardwareBatch.Cloud, batch.HoldingId);
        Assert.True(LabQueries.Split(g.C, g.State, buyer, false).General > 0);
    }

    [Fact]
    public void Cloud_chips_bill_weekly_rent_on_the_gpu_rent_line()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        LabState renter = RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, renter, HardwareBatch.Cloud, g.Id, d.Id, 30);
        long before = renter.Ledger.Lifetime.GpuRent;
        g.Weeks(1);
        Assert.True(renter.Ledger.Lifetime.GpuRent < before);
        Assert.True(ChipQueries.WeeklyCloudChipRent(g.C, g.State, renter) > 0);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_start_a_cloud_chip_purchase()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        LabState buyer = RentOnlyBuyer(g);
        buyer.Bankrupt = true;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 1), CommandError.LabBankrupt);
    }

    [Fact]
    public void The_openaye_planner_buys_chips_into_cloud_when_listings_are_cheap()
    {
        var g = Game.New(seed: 2025, tweak: c => c.Rival("openaye").Plan.ReserveWeeks = 3);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2025);
        foreach (string tech in new[] { "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling" })
            if (!g.Me.Techs.Contains(tech)) g.Me.Techs.Add(tech);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000_000);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        ChipDesign d = ReadyDesign(g);
        g.Me.SoftwarePermille = 1000;
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        long vendor = VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen);
        ChipSpec spec = ChipQueries.Spec(g.C, d);
        long effective = MathI.MulPermille(spec.Compute, ChipQueries.EffectiveSoftware(g.C, g.Me));
        long price = vendor * effective / gen.ComputePerGpuWeek * 65 / 100;
        g.Do(Command.SetChipListing(g.Id, d.Id, (int)price, 500));
        LabState openaye = g.Rival("openaye");
        openaye.PlanStartTick = 0;
        openaye.RentedGpus = 2;
        Finance.Earn(openaye, LedgerLine.CapitalIn, 30_000_000_000);
        Assert.Empty(openaye.Holdings);
        bool bought = false;
        for (int w = 0; w < 96 && !bought; w++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick))
                LabPlanner.Run(g.C, g.State, openaye, g.C.Rival("openaye").Plan, cmd => g.Try(cmd));
            g.Sim.Step();
            bought = openaye.Log.Any(e => e.Action == DecisionAction.BoughtChips);
        }
        Assert.True(bought);
        Assert.True(openaye.ChipOrders.Any(o => o.HoldingId == HardwareBatch.Cloud) || openaye.Fleet.Any(b => b.HoldingId == HardwareBatch.Cloud));
    }

    [Fact]
    public void A_cloud_chip_trade_moves_feelings_when_the_seller_is_the_player()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 9000, 80));
        LabState buyer = RentOnlyBuyer(g);
        int before = buyer.FeelingToPlayer;
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 5));
        Assert.Equal(before + g.C.Rivalry.Feelings.ChipOrderPermille, buyer.FeelingToPlayer);
    }

    [Fact]
    public void Cloud_orders_and_fleet_survive_a_save_round_trip()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        LabState buyer = RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, buyer, HardwareBatch.Cloud, g.Id, d.Id, 15);
        buyer.ChipOrders.Add(new ChipOrder
        {
            Id = g.State.TakeId(),
            DesignId = d.Id,
            DesignLabId = g.Id,
            BuyerLabId = buyer.Id,
            HoldingId = HardwareBatch.Cloud,
            Count = 8,
            DeliveryTick = g.Tick + 10,
            ReservedKw = 0,
            FoundryMonth = 1,
            NodeId = d.NodeId
        });
        string json = g.Sim.Save();
        Simulation again = Simulation.Load(g.C, json);
        LabState loaded = again.State.Lab(buyer.Id);
        Assert.Contains(loaded.Fleet, b => b.HoldingId == HardwareBatch.Cloud && b.Count == 15);
        Assert.Single(loaded.ChipOrders, o => o.HoldingId == HardwareBatch.Cloud);
        Assert.Empty(SaveGame.Problems(g.C, again.State));
    }

    [Fact]
    public void Buying_zero_cloud_chips_is_refused()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 1000, 50));
        LabState buyer = RentOnlyBuyer(g);
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_freeze_in_the_home_bloc_blocks_cloud_chip_buys()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 6000, 40));
        LabState buyer = RentOnlyBuyer(g);
        BlocStanding home = buyer.Bloc(buyer.HomeBlocId);
        home.Level = BlocStanding.Freeze;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, d.Id, HardwareBatch.Cloud, 3), CommandError.PermitFrozen);
    }

    [Fact]
    public void Bankruptcy_clears_cloud_chip_batches_with_the_rest_of_the_fleet()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        LabState buyer = RentOnlyBuyer(g);
        ChipSystem.AddChips(g.State, buyer, HardwareBatch.Cloud, g.Id, d.Id, 25);
        Finance.GoBankrupt(g.C, g.State, buyer);
        Assert.DoesNotContain(buyer.Fleet, b => b.HoldingId == HardwareBatch.Cloud);
    }

    [Fact]
    public void Rival_plans_without_cloud_deals_still_need_a_hall_to_buy_listings()
    {
        Game g = SellerBuyerPair();
        ChipDesign d = ReadyDesign(g);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 100));
        PlanConfig plan = g.C.Rival("deepsneak").Plan;
        Assert.False(plan.UsesCloudDeals);
        LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("deepsneak"), EraSystem.AtTick(g.C, g.Tick), true);
        buyer.PlanStartTick = 0;
        Finance.Earn(buyer, LedgerLine.CapitalIn, 10_000_000_000);
        Assert.Empty(buyer.Holdings);
        int before = buyer.Log.Count;
        LabPlanner.Run(g.C, g.State, buyer, plan, cmd => g.Try(cmd));
        Assert.Equal(before, buyer.Log.Count(l => l.Action == DecisionAction.BoughtChips));
    }
}