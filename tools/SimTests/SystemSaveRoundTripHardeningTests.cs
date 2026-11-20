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

/// <summary>One mid-game save/load round-trip per major rules system.</summary>
public class SystemSaveRoundTripHardeningTests
{
    static readonly string[] ChipTechs =
    {
        "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling",
    };

    [Fact]
    public void Rivals_mid_state_round_trips()
    {
        Game g = Game.New(88_101);
        g.WithStarterLab(researchers: 3, engineers: 2, sales: 1, gpus: 10);
        g.Autopilot(120);
        Assert.True(g.State.Labs.Count(l => l.Controller == LabController.Plan) >= 3);
        Assert.True(g.Rival("openaye").Log.Count > 0);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 24);
    }

    [Fact]
    public void Chips_mid_state_round_trips()
    {
        var options = new NewGameOptions { Seed = 77, StartEraId = "punchline", RivalIds = new List<string>() };
        Game g = new Game(Simulation.NewGame(TestData.Config(c => c.World.Attention.DominanceFromPpm = MathI.Ppm), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2022);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 5_000_000_000);
        foreach (string tech in ChipTechs)
            if (!g.Me.Techs.Contains(tech)) g.Me.Techs.Add(tech);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        g.Hire("chip-architect", 2);
        g.Do(Command.StartChipDesign(g.Id, "inference", "n28", "small", "fp16", "ddr", "air"));
        g.Weeks(8);
        Assert.Contains(g.Me.ChipDesigns, d => d.Status == ChipDesignStatus.Designing || d.Status == ChipDesignStatus.Ready);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 12);
    }

    [Fact]
    public void World_map_and_sites_mid_state_round_trips()
    {
        Game g = Game.New(55_202, c =>
        {
            c.PlayerStart.Cash = 800_000_000;
            c.World.Attention.DominanceFromPpm = MathI.Ppm;
            c.World.Halls.TownVoteAttentionDivisor = 1_000_000;
            c.World.Site("us-oregon").GridKw = 20_000;
            foreach (SiteConfig site in c.World.Sites) site.TownVotePermille = 0;
        });
        int holdingId = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Do(Command.BuildHall(g.Id, holdingId, 500));
        g.Until(() => g.Me.Holding(holdingId).HallKw >= 500, 120, "hall build");
        Assert.Equal(g.C.World.Sites.Count, g.State.Sites.Count);
        Assert.True(g.Me.Holdings.Count > 0);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 16);
    }

    [Fact]
    public void Hardware_fleet_and_vendor_mid_state_round_trips()
    {
        Game g = Game.Solo(seed: 60_404);
        g.Build("server-closet", 3, 3);
        g.Build("server-closet", 3, 3);
        g.Me.Cash += 20_000_000;
        g.Do(Command.SignReservedCompute(g.Id, 4));
        VendorStanding standing = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        standing.BoughtThisMonth = standing.AllocationThisMonth;
        g.Do(Command.QueueGpuPurchase(g.Id, 2, HardwareBatch.Hq));
        Assert.True(ComputeContractSystem.ReservedGpus(g.Me) > 0);
        Assert.True(standing.QueuedGpus > 0 || LabQueries.OwnedUnits(g.Me) > 0);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 8);
    }

    [Fact]
    public void Alignment_and_incidents_mid_state_round_trips()
    {
        Game g = Game.Solo(seed: 71_505, tweak: c => c.Safety.IncidentChancePermille = 800);
        g.WithStarterLab(gpus: 12, safety: 2);
        ModelState model = g.Train("mini", 20);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Until(() => model.Incidents > 0 || g.Me.Ledger.Lifetime.Fines > 0, 80, "incident or fine");
        Assert.True(model.Alignment < 1000 || model.Incidents > 0);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 12);
    }

    [Fact]
    public void Staff_people_mid_state_round_trips()
    {
        Game g = Game.Solo(seed: 82_606);
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Hire("researcher", 3);
        g.Hire("engineer", 2);
        Assert.True(g.Me.Staff.Count >= 5);
        Assert.All(g.Me.Staff, p => Assert.False(string.IsNullOrEmpty(p.TraitId)));
        g.Weeks(8);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 12);
    }

    [Fact]
    public void Reserved_compute_and_queue_mid_state_round_trips()
    {
        Game g = Game.Solo(seed: 93_707);
        g.Build("server-closet", 3, 3);
        g.Do(Command.SignReservedCompute(g.Id, 6));
        g.Do(Command.SignReservedCompute(g.Id, 2));
        g.Me.Cash += 5_000_000;
        g.Do(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq));
        Assert.Equal(2, g.Me.ReservedContracts.Count);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 8);
    }

    [Fact]
    public void Funding_and_grants_mid_state_round_trips()
    {
        Game g = Game.New(44_808);
        g.WithStarterLab(gpus: 8);
        Assert.True(g.Me.GrantPerWeek > 0);
        int grantEnd = g.Me.GrantEndsTick;
        g.Autopilot(48);
        Assert.True(g.Me.Ledger.Lifetime.Grants > 0 || grantEnd > 0);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 16);
    }

    [Fact]
    public void Cloud_deals_mid_state_round_trips()
    {
        var options = new NewGameOptions { Seed = 11, StartEraId = "punchline", RivalIds = new List<string>() };
        Game g = new Game(Simulation.NewGame(TestData.Config(c => c.World.Attention.DominanceFromPpm = MathI.Ppm), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2024);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 200_000_000);
        foreach (string tech in ChipTechs)
            if (!g.Me.Techs.Contains(tech)) g.Me.Techs.Add(tech);
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 3, out int x, out int y);
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 3));
        var design = new ChipDesign
        {
            Id = g.State.TakeId(),
            Version = 1,
            Name = "Soak",
            Target = "training",
            NodeId = "n28",
            DieId = "medium",
            FormatId = "fp16",
            MemoryId = "ddr",
            CoolingId = "air",
            PointsRequired = 100,
            PointsDone = 100,
            Status = ChipDesignStatus.Ready,
            YieldPermille = 900,
            ReadyTick = g.Tick,
            StageEndsTick = -1,
        };
        g.Me.ChipDesigns.Add(design);
        g.Do(Command.SetChipListing(g.Id, design.Id, 12_000, 50));
        LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("openaye"), EraSystem.AtTick(g.C, g.Tick), true);
        buyer.PlanStartTick = 0;
        Finance.Earn(buyer, LedgerLine.CapitalIn, 2_000_000_000);
        g.Do(Command.BuyListedChips(buyer.Id, g.Id, design.Id, HardwareBatch.Cloud, 5));
        Assert.NotEmpty(buyer.ChipOrders);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 8);
    }

    [Fact]
    public void Endings_and_spectate_mid_state_round_trips()
    {
        Game g = Game.New(66_909);
        g.Me.Ledger.ThisMonth.Revenue = 5_000_000;
        g.Me.Cash = 80_000;
        g.Do(Command.SellLab(g.Id, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(8);
        SimHardeningSupport.AssertSaveLoadContinueIdenticallySpectating(g, 12);
    }

    [Fact]
    public void Shady_and_legal_mid_state_round_trips()
    {
        Game g = Game.Solo(seed: 17_010);
        var model = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 450, Status = ModelStatus.Trained, Alignment = 500 };
        g.Me.Models.Add(model);
        g.Me.Heat = 320;
        g.Me.Stash = 80_000;
        g.Do(Command.StartShadyOp(g.Id, "liberate-funds", model.Id, -1, -1, ""));
        g.Me.CaseEndsTick = g.Tick + g.C.Calendar.WeeksPerMonth * 2;
        Assert.Single(g.Me.ShadyOps);
        Assert.True(g.Me.CaseEndsTick > g.Tick);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 12);
    }
}