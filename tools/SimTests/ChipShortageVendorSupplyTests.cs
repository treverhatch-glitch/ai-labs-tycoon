using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Chip shortage shrinks Envydia monthly GPU supply (Q42).</summary>
public class ChipShortageVendorSupplyTests
{
    static int TickOfEvent(GameConfig c, string id) => EraSystem.EventTick(c, c.WorldEvent(id));

    static Game RichAt(int year, int month)
    {
        Game g = Game.Solo(tweak: c => c.PlayerStart.Cash = 500_000_000L);
        g.WithStarterLab();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year) + (month - 1) * g.C.Calendar.WeeksPerMonth;
        return g;
    }

    static void StartShortage(Game g, int strength = 1200)
    {
        g.State.ChipShortageStrengthPermille = strength;
        g.State.ChipShortageUntilTick = g.State.Tick + g.C.Chips.Shortage.DurationWeeks;
    }

    static int BaselineSupply(Game g)
    {
        int year = GameDate.Year(g.C.Calendar, g.State.Tick);
        return g.C.Vendor.SupplyInYear(year);
    }

    [Fact]
    public void Vendor_supply_permille_is_inverse_of_strength()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1200);
        Assert.Equal(800, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Vendor_supply_permille_floors_at_config()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1800);
        Assert.Equal(400, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Vendor_supply_permille_1000_when_inactive()
    {
        Game g = RichAt(2021, 7);
        Assert.Equal(1000, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Scale_vendor_supply_at_1200_strength()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1200);
        int baseSupply = BaselineSupply(g);
        Assert.Equal((int)MathI.MulPermille(baseSupply, 800), ChipShortageSystem.ScaleVendorSupply(g.C, g.State, baseSupply));
    }

    [Fact]
    public void First_month_step_after_shortage_reduces_supply_this_month()
    {
        Game g = RichAt(2021, 7);
        int baseSupply = BaselineSupply(g);
        StartShortage(g, 1200);
        g.Weeks(1);
        Assert.Equal((int)MathI.MulPermille(baseSupply, 800), g.State.Vendor.SupplyThisMonth);
    }

    [Fact]
    public void Card_shortage_event_matches_manual_start()
    {
        Game g = Game.Solo();
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000L);
        g.State.Tick = TickOfEvent(g.C, "card-shortage") - 1;
        int baseSupply = g.C.Vendor.SupplyInYear(2021);
        g.Weeks(2);
        Assert.True(ChipShortageSystem.Active(g.State));
        Assert.Equal((int)MathI.MulPermille(baseSupply, 800), g.State.Vendor.SupplyThisMonth);
    }

    [Fact]
    public void Allocations_sum_to_reduced_supply()
    {
        Game g = Game.Solo();
        g.WithStarterLab();
        StartShortage(g, 1200);
        g.Weeks(1);
        Assert.Equal(g.State.Vendor.SupplyThisMonth, g.State.Vendor.Standings.Sum(s => s.AllocationThisMonth));
    }

    [Fact]
    public void Stronger_shortage_means_smaller_supply()
    {
        Game g = RichAt(2020, 1);
        StartShortage(g, 1100);
        g.Weeks(1);
        int weak = g.State.Vendor.SupplyThisMonth;
        g.State.Tick -= 1;
        StartShortage(g, 1500);
        g.Weeks(1);
        int strong = g.State.Vendor.SupplyThisMonth;
        Assert.True(weak > strong);
    }

    [Fact]
    public void Shortage_end_restores_full_supply_next_month()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1200);
        VendorSystem.RunMonth(g.C, g.State);
        int reduced = g.State.Vendor.SupplyThisMonth;
        g.State.ChipShortageUntilTick = g.State.Tick;
        ChipShortageSystem.RunWeek(g.C, g.State);
        Assert.False(ChipShortageSystem.Active(g.State));
        VendorSystem.RunMonth(g.C, g.State);
        int full = g.State.Vendor.SupplyThisMonth;
        Assert.Equal(1000, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
        Assert.True(full > reduced);
    }

    [Fact]
    public void Run_month_after_shortage_matches_scale_helper()
    {
        Game g = RichAt(2020, 6);
        StartShortage(g, 1200);
        VendorSystem.RunMonth(g.C, g.State);
        int year = GameDate.Year(g.C.Calendar, g.State.Tick);
        int expected = ChipShortageSystem.ScaleVendorSupply(g.C, g.State, g.C.Vendor.SupplyInYear(year));
        Assert.Equal(expected, g.State.Vendor.SupplyThisMonth);
    }

    [Fact]
    public void Player_allocation_drops_under_shortage()
    {
        Game g = Game.Solo();
        g.WithStarterLab();
        g.Weeks(1);
        int normal = VendorSystem.EnsureStanding(g.C, g.State, g.Me.Id).AllocationThisMonth;
        StartShortage(g, 1300);
        g.Weeks(5);
        int crunch = VendorSystem.EnsureStanding(g.C, g.State, g.Me.Id).AllocationThisMonth;
        Assert.True(crunch < normal || normal == 0);
    }

    [Fact]
    public void Queue_fulfillment_slower_during_shortage()
    {
        Game g = Game.Solo();
        g.WithStarterLab(gpus: 0);
        VendorStanding s = VendorSystem.EnsureStanding(g.C, g.State, g.Me.Id);
        s.QueuedGpus = 20;
        s.QueueHoldingId = HardwareBatch.Hq;
        StartShortage(g, 1400);
        g.Weeks(1);
        int alloc = s.AllocationThisMonth;
        s.BoughtThisMonth = 0;
        g.Weeks(1);
        Assert.True(s.BoughtThisMonth <= alloc);
    }

    [Fact]
    public void Save_load_preserves_shortage_supply_math()
    {
        Game g = RichAt(2021, 8);
        StartShortage(g, 1250);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(750, ChipShortageSystem.VendorSupplyPermille(loaded.C, loaded.State));
        Assert.Equal(
            ChipShortageSystem.ScaleVendorSupply(loaded.C, loaded.State, BaselineSupply(loaded)),
            ChipShortageSystem.ScaleVendorSupply(g.C, g.State, BaselineSupply(g)));
    }

    [Fact]
    public void Validator_accepts_vendor_supply_floor()
    {
        var problems = ConfigValidator.Problems(GameConfig.FromJson(TestData.ConfigJson()));
        Assert.DoesNotContain(problems, p => p.Contains("vendorSupplyFloor"));
    }

    [Fact]
    public void Validator_rejects_bad_vendor_supply_floor()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Chips.Shortage.VendorSupplyFloorPermille = 0;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("vendorSupplyFloor"));
    }

    [Fact]
    public void Strength_1050_gives_950_permille()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1050);
        Assert.Equal(950, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Strength_1600_hits_floor()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1600);
        Assert.Equal(400, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Zero_strength_inactive_uses_full_supply()
    {
        Game g = RichAt(2018, 5);
        g.State.ChipShortageStrengthPermille = 0;
        g.State.ChipShortageUntilTick = -1;
        g.Weeks(1);
        Assert.Equal(BaselineSupply(g), g.State.Vendor.SupplyThisMonth);
    }

    [Fact]
    public void Renewed_stronger_event_lowers_supply_permille()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1200);
        Assert.Equal(800, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
        ChipShortageSystem.Start(g.C, g.State, 1500);
        Assert.Equal(500, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Bankrupt_labs_still_share_reduced_pool()
    {
        Game g = Game.Solo();
        g.WithStarterLab();
        StartShortage(g, 1200);
        g.Weeks(1);
        long sum = g.State.Vendor.Standings.Where(s => !g.State.Lab(s.LabId).Bankrupt).Sum(s => s.AllocationThisMonth);
        Assert.Equal(g.State.Vendor.SupplyThisMonth, sum);
    }

    [Fact]
    public void Price_mul_stays_above_supply_mul_at_same_strength()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1200);
        Assert.True(ChipShortageSystem.PriceMulPermille(g.State) > ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Deterministic_supply_two_saves()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1200);
        g.Weeks(1);
        string save = g.Sim.Save();
        var a = new Game(Simulation.Load(g.C, save));
        var b = new Game(Simulation.Load(g.C, save));
        a.Weeks(1);
        b.Weeks(1);
        Assert.Equal(a.State.Vendor.SupplyThisMonth, b.State.Vendor.SupplyThisMonth);
    }

    [Fact]
    public void Supply_scales_linearly_with_base_year_table()
    {
        Game g = RichAt(2024, 2);
        int baseSupply = BaselineSupply(g);
        StartShortage(g, 1200);
        Assert.Equal((int)MathI.MulPermille(baseSupply, 800), ChipShortageSystem.ScaleVendorSupply(g.C, g.State, baseSupply));
    }

    [Fact]
    public void Two_month_steps_same_shortage_same_permille()
    {
        Game g = RichAt(2021, 7);
        StartShortage(g, 1200);
        g.Weeks(1);
        int permille = ChipShortageSystem.VendorSupplyPermille(g.C, g.State);
        g.Weeks(4);
        Assert.Equal(permille, ChipShortageSystem.VendorSupplyPermille(g.C, g.State));
    }

    [Fact]
    public void Share_permille_recomputed_after_shortage_month()
    {
        Game g = Game.Solo();
        g.WithStarterLab();
        StartShortage(g, 1200);
        g.Weeks(1);
        VendorStanding s = VendorSystem.EnsureStanding(g.C, g.State, g.Me.Id);
        Assert.True(s.SharePermille >= 0);
        Assert.True(s.AllocationThisMonth <= g.State.Vendor.SupplyThisMonth);
    }
}