using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ReliabilityTests
{
    /// <summary>A lab of 8 GPUs where exactly one fails a week (8 units at 125,000 millionths), unless told otherwise.</summary>
    static Game Lab(int basePpm = 125_000, System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Reliability.BaseFailPpm = basePpm;
            c.Reliability.AgePpm = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, 8);
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static int Units(Game g) { return HardwareReliability.Units(g.C, g.Me); }

    [Fact]
    public void Without_a_warranty_or_spares_a_failed_unit_is_lost()
    {
        Game g = Lab();
        Assert.Equal(8, Units(g));
        g.Weeks(1);
        Assert.Equal(7, Units(g));
        Assert.Equal(1, g.Me.GpuFailures);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.GpusFailed);
        Assert.Equal(0, e.Pick);
        Assert.Equal(1, e.Amount);
        Assert.Equal(0, g.Me.RmaUnits);
    }

    [Fact]
    public void The_chance_is_carried_whole_so_nobody_rolls_dice()
    {
        Game g = Lab(100_000);
        g.Weeks(1);
        Assert.Equal(8, Units(g));
        Assert.Equal(800_000, g.Me.FailCarry);
        g.Weeks(1);
        Assert.Equal(7, Units(g));
        Assert.Equal(600_000, g.Me.FailCarry);
        Assert.Equal(1, g.Me.GpuFailures);
    }

    [Fact]
    public void An_older_generation_fails_more_and_a_heatwave_at_the_site_makes_it_worse()
    {
        Game g = Lab(1000, c => c.Reliability.AgePpm = 5000);
        HardwareBatch batch = g.Me.Fleet.First(b => b.ChipDesignId < 0);
        long now = HardwareReliability.RatePpm(g.C, g.State, g.Me, batch);
        GpuGenerationConfig gen = g.C.Compute.Generation(batch.GpuGenId);
        long years = GameDate.Year(g.C.Calendar, g.State.Tick) - gen.OpensYear;
        Assert.Equal(1000 + 5000 * System.Math.Max(0, years), now);
        gen.OpensYear -= 3;
        Assert.Equal(now + 3 * 5000, HardwareReliability.RatePpm(g.C, g.State, g.Me, batch));

        int hall = ChipTests.Hall(g, g.Me);
        Fleet.AddGpus(g.State, g.Me, hall, gen.IdValue, 4);
        HardwareBatch away = g.Me.Fleet.First(b => b.HoldingId == hall);
        long calm = HardwareReliability.RatePpm(g.C, g.State, g.Me, away);
        g.State.Site(g.Me.Holding(hall).SiteId).HeatwaveUntilTick = g.Tick + 3;
        Assert.Equal(MathI.MulPermille(calm, g.C.Reliability.HeatwavePermille), HardwareReliability.RatePpm(g.C, g.State, g.Me, away));
        Assert.Equal(now + 3 * 5000, HardwareReliability.RatePpm(g.C, g.State, g.Me, batch));
    }

    [Fact]
    public void A_spare_takes_the_place_of_a_failed_unit_at_once_with_the_current_generation()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        long price = LabQueries.CurrentGeneration(g.C, g.State.Tick).BuyPrice;
        g.Do(Command.StockSpares(g.Id, 2));
        Assert.Equal(2, g.Me.Spares);
        Assert.True(g.Me.Cash <= cash - 2 * price);
        Assert.Equal(1, Count(g, EventKind.SparesStocked));
        g.Weeks(1);
        Assert.Equal(8, Units(g));
        Assert.Equal(1, g.Me.Spares);
        Assert.Equal(1, g.Me.GpuFailures);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.GpusFailed).Pick);
        string now = LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue;
        Assert.Contains(g.Me.Fleet, b => b.GpuGenId == now && b.ChipDesignId < 0);
    }

    [Fact]
    public void A_warranty_sends_a_failed_unit_back_and_it_returns_after_the_weeks_of_its_tier()
    {
        Game g = Lab(125_000, c => { c.Reliability.RmaWeeksTier1 = 5; c.Reliability.RmaWeeksTier2 = 2; });
        g.Do(Command.SetWarranty(g.Id, 1));
        int start = g.Tick;
        g.Weeks(1);
        Assert.Equal(1, g.Me.RmaUnits);
        Assert.Equal(7, Units(g));
        Assert.Equal(2, g.State.Events.Last(e => e.Kind == EventKind.GpusFailed).Pick);
        Assert.Equal(start + 5, g.Me.RmaDue);
        g.Weeks(3);
        int waiting = g.Me.RmaUnits;
        Assert.True(waiting >= 3);
        Assert.Equal(g.Me.GpuFailures, waiting);
        Assert.Equal(0, Count(g, EventKind.ReplacementsArrived));
        g.Weeks(1);
        Assert.Equal(1, Count(g, EventKind.ReplacementsArrived));
        Assert.Equal(waiting, g.State.Events.Last(e => e.Kind == EventKind.ReplacementsArrived).Amount);
        Assert.Equal(8 - g.Me.GpuFailures + waiting, Units(g));

        Game h = Lab(125_000, c => { c.Reliability.RmaWeeksTier1 = 5; c.Reliability.RmaWeeksTier2 = 2; });
        h.Do(Command.SetWarranty(h.Id, 2));
        int begin = h.Tick;
        h.Weeks(1);
        Assert.Equal(begin + 2, h.Me.RmaDue);
    }

    [Fact]
    public void The_warranty_costs_a_fee_for_each_unit_every_week_and_raising_it_costs_a_switch_fee()
    {
        Game g = Lab(0);
        long before = g.Me.Ledger.Lifetime.Insurance;
        g.Do(Command.SetWarranty(g.Id, 1));
        Assert.Equal(before - g.C.Reliability.SwitchFee, g.Me.Ledger.Lifetime.Insurance);
        long mid = g.Me.Ledger.Lifetime.Insurance;
        g.Weeks(1);
        long taken = mid - g.Me.Ledger.Lifetime.Insurance;
        Assert.True(taken >= 8 * g.C.Reliability.FeeTier1);
        long switched = g.Me.Ledger.Lifetime.Insurance;
        g.Do(Command.SetWarranty(g.Id, 0));
        Assert.Equal(switched, g.Me.Ledger.Lifetime.Insurance);
        Assert.Equal(0, g.Me.WarrantyTier);
        Assert.Equal(2, Count(g, EventKind.WarrantyChanged));
    }

    [Fact]
    public void Spares_cost_upkeep_and_a_waiting_replacement_can_be_rushed_for_a_price()
    {
        Game g = Lab();
        g.Do(Command.SetWarranty(g.Id, 1));
        g.Weeks(1);
        Assert.Equal(1, g.Me.RmaUnits);
        long price = HardwareReliability.ExpeditePrice(g.C, g.State, g.Me);
        Assert.Equal(MathI.MulPermille(LabQueries.CurrentGeneration(g.C, g.State.Tick).BuyPrice, g.C.Reliability.ExpeditePermille), price);
        long cash = g.Me.Cash;
        int units = Units(g);
        g.Do(Command.ExpediteReplacements(g.Id));
        Assert.Equal(0, g.Me.RmaUnits);
        Assert.Equal(units + 1, Units(g));
        Assert.True(g.Me.Cash <= cash - price);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.ReplacementsArrived).Pick);
        g.Refused(Command.ExpediteReplacements(g.Id), CommandError.InvalidArgument);

        Game h = Lab(0);
        h.Do(Command.StockSpares(h.Id, 4));
        long upkeep = h.Me.Ledger.Lifetime.Insurance;
        h.Weeks(1);
        Assert.Equal(upkeep - 4 * h.C.Reliability.SpareUpkeep, h.Me.Ledger.Lifetime.Insurance);
    }

    [Fact]
    public void Bad_orders_are_refused()
    {
        Game g = Lab(0);
        g.Refused(Command.SetWarranty(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetWarranty(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetWarranty(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.StockSpares(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.StockSpares(g.Id, g.C.Reliability.MaxSpares + 1), CommandError.InvalidArgument);
        g.Me.Cash = 100;
        Assert.False(g.Try(Command.SetWarranty(g.Id, 1)).Ok);
        Assert.False(g.Try(Command.StockSpares(g.Id, 1)).Ok);
        Assert.Equal(0, g.Me.WarrantyTier);
        Assert.Equal(0, g.Me.Spares);
    }

    [Fact]
    public void Nothing_happens_when_it_is_switched_off()
    {
        Game g = Lab(125_000, c => c.Reliability.Enabled = 0);
        g.Weeks(10);
        Assert.Equal(8, Units(g));
        Assert.Equal(0, g.Me.GpuFailures);
        g.Refused(Command.SetWarranty(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.StockSpares(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.ExpediteReplacements(g.Id), CommandError.InvalidArgument);
        Assert.Equal(0, Count(g, EventKind.GpusFailed));
    }

    [Fact]
    public void Failures_save_load_and_play_on_the_same_and_an_older_save_has_none()
    {
        Game g = Lab(90_000);
        g.Do(Command.SetWarranty(g.Id, 1));
        g.Do(Command.StockSpares(g.Id, 1));
        g.Weeks(14);
        Assert.True(g.Me.GpuFailures > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(40);
        g.Weeks(40);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 87, g.Sim.Save());
        Assert.DoesNotContain("warrantyTier", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.WarrantyTier);
        Assert.Equal(0, lab.GpuFailures);
    }

    [Fact]
    public void A_rival_with_a_warranty_is_not_sound_and_bad_data_is_caught()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.WarrantyTier = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("warranty or spares"));
        rival.WarrantyTier = 0;
        g.Me.RmaUnits = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("warranty or spares"));
        g.Me.RmaDue = 5;
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Me.RmaUnits = 0;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("warranty or spares"));
        g.Me.RmaDue = 0;
        g.Me.FailCarry = 1_000_000;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("warranty or spares"));
        g.Me.FailCarry = 0;
        g.Me.WarrantyTier = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("warranty or spares"));
        g.Me.WarrantyTier = 0;

        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Reliability.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Reliability.Enabled = 2;
        c.Reliability.BaseFailPpm = -1;
        c.Reliability.HeatwavePermille = 999;
        c.Reliability.FeeTier2 = c.Reliability.FeeTier1 - 1;
        c.Reliability.RmaWeeksTier2 = c.Reliability.RmaWeeksTier1 + 1;
        c.Reliability.SwitchFee = -1;
        c.Reliability.MaxSpares = -1;
        c.Reliability.ExpeditePermille = 1001;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "baseFailPpm", "heatwavePermille", "feeTier2", "rmaWeeksTier2", "switchFee", "maxSpares", "expeditePermille" })
            Assert.Contains(problems, p => p.Contains("reliability." + key));
    }
}
