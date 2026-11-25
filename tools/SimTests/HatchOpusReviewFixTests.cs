using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Regression tests for Opus review must-fix bugs — month-end order, acquisitions, absorption.</summary>
public class HatchOpusReviewFixTests
{
    static void StepOneMonthEnd(Game g)
    {
        int wpm = g.C.Calendar.WeeksPerMonth;
        int stepsBeforeClose = wpm - 1 - (g.Tick % wpm);
        for (int i = 0; i < stepsBeforeClose; i++) g.Sim.Step();
        g.Sim.Step();
    }

    [Fact]
    public void Month_end_step_counts_market_revenue_before_bubble_cools()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.HotPermille = 300;
            c.Bubble.ReturnCoolingPermille = 30;
            c.Bubble.PostCrashResidualPermille = 50;
            c.Bubble.PopAtPermille = 1000;
            c.Bubble.WeakCashHaircutPermille = 0;
            c.Bubble.ReturnsNeededPermille = 500;
            c.Bubble.DecayPermillePerMonth = 0;
        });
        g.State.BubblePermille = 400;
        g.State.BubbleGraceMonths = 2;
        g.State.BubbleCapitalRaised = 100_000;
        Finance.Earn(g.Me, LedgerLine.Revenue, 50_000);
        StepOneMonthEnd(g);
        Assert.Equal(0, g.State.BubbleGraceMonths);
        Assert.Equal(370, g.State.BubblePermille);
    }

    [Fact]
    public void Solo_profitable_player_survives_bubble_pop_through_month_end_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Bubble.SurvivorRevenuePpm = 500_000;
            c.Bubble.PopAtPermille = 400;
            c.Bubble.WeakCashHaircutPermille = 250;
            c.Bubble.ThePopEndingForPlayer = true;
        });
        g.State.BubblePermille = 500;
        Finance.Earn(g.Me, LedgerLine.Revenue, 20_000_000);
        StepOneMonthEnd(g);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.ThePopId);
    }

    [Fact]
    public void Crash_acquire_then_gpu_sale_does_not_print_money()
    {
        var g = Game.New(seed: 77, tweak: c =>
        {
            c.Acquisitions.CrashDistressPricePermille = 450;
            c.Acquisitions.IntegrationCostPermille = 120;
            c.Reliability.Enabled = 0;
        });
        LabState rival = g.Rival("openaye");
        rival.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 700,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "DistressBot"
        });
        rival.Cash = -100_000;
        string genId = LabQueries.CurrentGeneration(g.C, g.State.Tick).Id;
        rival.Fleet.Add(new HardwareBatch { Id = g.State.TakeId(), Count = 80, GpuGenId = genId, ChipDesignId = -1, HoldingId = HardwareBatch.Hq });
        g.State.BubbleCrashMonthsRemaining = 12;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        long before = g.Me.Cash;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int owned = LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq);
        Assert.True(owned > 0);
        g.Refused(Command.SellGpus(g.Id, owned), CommandError.TargetNotEligible);
        g.Weeks(g.C.Acquisitions.FleetResaleLockWeeks);
        g.Do(Command.SellGpus(g.Id, owned));
        long delta = g.Me.Cash - before;
        Assert.True(delta <= 0, "acquire then sell netted " + delta);
    }

    [Fact]
    public void Target_cash_transfers_and_absorbed_lab_stays_idle()
    {
        var g = Game.New(seed: 78);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 500,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "CashCo"
        });
        rival.Cash = 500_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        long buyerBefore = g.Me.Cash;
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(0, rival.Cash);
        Assert.True(g.Me.Cash >= buyerBefore - q.Total);
        for (int i = 0; i < 8; i++) g.Sim.Step();
        Assert.Equal(0, rival.Cash);
        Assert.Empty(rival.Fleet);
        Assert.Empty(rival.Models);
    }

    [Fact]
    public void Buyer_keeps_stronger_sector_model_after_acquiring_weaker_rival()
    {
        var g = Game.New(seed: 79);
        ModelState mine = new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 900,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "BigBot"
        };
        g.Me.Models.Add(mine);
        g.Do(Command.ReleaseModel(g.Id, mine.Id, "game-ai"));
        LabState rival = g.Rival("openaye");
        var weak = new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 100,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "TinyBot"
        };
        rival.Models.Add(weak);
        g.Do(Command.ReleaseModel(rival.Id, weak.Id, "game-ai"));
        rival.Cash = -50_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        int modelIdBefore = g.State.Sector("game-ai").Entry(g.Id).ModelId;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        SectorEntry entry = g.State.Sector("game-ai").Entry(g.Id);
        Assert.Equal(modelIdBefore, entry.ModelId);
        Assert.Equal(900, g.Me.Model(entry.ModelId).Capability);
    }

    [Fact]
    public void Acquired_models_keep_ids_on_the_buyer()
    {
        var g = Game.New(seed: 81);
        LabState rival = g.Rival("openaye");
        int modelId = g.State.TakeId();
        var m = new ModelState
        {
            Id = modelId,
            LineId = "mini",
            Capability = 650,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "IdKeeper"
        };
        rival.Models.Add(m);
        rival.Cash = -30_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Contains(g.Me.Models, x => x.Id == modelId && x.Name == "IdKeeper");
    }

    [Fact]
    public void Site_gpus_and_holdings_transfer_on_acquire()
    {
        var g = Game.New(seed: 82, tweak: c => c.Rival("openaye").StartCash = 50000000);
        g.State.Vendor.SupplyThisMonth = 500;
        VendorSystem.RunMonth(g.C, g.State);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 600,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "SiteCo"
        });
        Finance.Earn(rival, LedgerLine.CapitalIn, 5_000_000);
        int land = g.Do(Command.BuyLand(rival.Id, "us-oregon")).CreatedId;
        g.Do(Command.BuildHall(rival.Id, land, 100));
        g.Until(() => rival.Holding(land).HallKw >= 100, 200, "rival hall");
        g.Do(Command.BuyGpus(rival.Id, 10, land));
        rival.Cash = -40_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        int siteUnits = 0;
        int siteHolding = -1;
        for (int i = 0; i < g.Me.Holdings.Count; i++)
        {
            if (g.Me.Holdings[i].SiteId == "us-oregon")
            {
                siteHolding = g.Me.Holdings[i].Id;
                siteUnits = LabQueries.OwnedGpuUnits(g.Me, siteHolding);
                break;
            }
        }
        Assert.True(siteHolding > 0);
        Assert.Equal(10, siteUnits);
    }

    [Fact]
    public void Absorbed_rival_skips_finance_vendor_and_reserved_compute_after_steps()
    {
        var g = Game.New(seed: 83, tweak: c => c.Compute.ReservedContractWeeks = 12);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 550,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "IdleCo"
        });
        rival.Cash = -25_000;
        Finance.Earn(rival, LedgerLine.CapitalIn, 20_000_000);
        g.Do(Command.SignReservedCompute(rival.Id, 6));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(0, rival.Cash);
        Assert.Empty(rival.ReservedContracts);
        int wpm = g.C.Calendar.WeeksPerMonth;
        for (int i = 0; i < wpm + 2; i++) g.Sim.Step();
        Assert.Equal(0, rival.Cash);
        Assert.Empty(rival.ReservedContracts);
        Assert.Equal(0, g.State.Vendor.Standing(rival.Id).AllocationThisMonth);
    }
}