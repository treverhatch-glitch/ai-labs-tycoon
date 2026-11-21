using System;
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

/// <summary>Global chip shortage: world news starts a crunch on GPU and foundry prices.</summary>
public class ChipShortageTests
{
    static int TickOfEvent(GameConfig c, string id) => EraSystem.EventTick(c, c.WorldEvent(id));

    static void FireCardShortage(Game g)
    {
        g.State.Tick = TickOfEvent(g.C, "card-shortage") - 1;
        g.Weeks(2);
    }

    static Game SoloAtShortageWeek(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: tweak);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000_000L);
        return g;
    }

    [Fact]
    public void Card_shortage_news_starts_global_crunch()
    {
        Game g = SoloAtShortageWeek();
        FireCardShortage(g);
        Assert.True(ChipShortageSystem.Active(g.State));
        Assert.Equal(1200, g.State.ChipShortageStrengthPermille);
    }

    [Fact]
    public void Began_and_world_news_events_fire_together()
    {
        Game g = SoloAtShortageWeek();
        FireCardShortage(g);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.WorldNews && e.Ref == "card-shortage");
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ChipShortageBegan && e.Amount == 1200);
    }

    [Fact]
    public void Gpu_buy_price_scales_with_strength()
    {
        Game g = SoloAtShortageWeek();
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.State.Tick);
        long normal = VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen);
        ChipShortageSystem.Start(g.C, g.State, 1200);
        long during = VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen);
        Assert.Equal(MathI.MulPermille(normal, 1200), during);
    }

    [Fact]
    public void Weekly_gpu_rent_rises_during_shortage()
    {
        Game g = SoloAtShortageWeek();
        g.Do(Command.SetRentedGpus(g.Id, 10));
        long before = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        ChipShortageSystem.Start(g.C, g.State, 1300);
        long during = LabQueries.WeeklyGpuRent(g.C, g.State, g.Me);
        Assert.True(during > before);
        Assert.Equal(MathI.MulPermille(before, 1300), during);
    }

    [Fact]
    public void Reserved_deposit_scales_with_shortage()
    {
        Game g = SoloAtShortageWeek();
        int count = 6;
        long deposit = ComputeContractSystem.Deposit(g.C, g.State, g.Me, count);
        ChipShortageSystem.Start(g.C, g.State, 1250);
        long during = ComputeContractSystem.Deposit(g.C, g.State, g.Me, count);
        long expected = MathI.MulPermille(deposit, 1250);
        Assert.InRange(during, expected - 3, expected + 3);
    }

    [Fact]
    public void Foundry_room_shrinks_when_prices_rise()
    {
        Game g = SoloAtShortageWeek();
        ChipNodeConfig node = g.C.Chips.Node("n28");
        long baseRoom = g.C.Chips.FoundryRoom(node);
        ChipShortageSystem.Start(g.C, g.State, 1200);
        Assert.Equal(MathI.MulPermille(baseRoom, 800), ChipShortageSystem.EffectiveFoundryRoom(g.C, g.State, node));
    }

    [Fact]
    public void Huge_order_fits_before_shortage_but_not_during()
    {
        Game g = SoloAtShortageWeek();
        ChipNodeConfig node = g.C.Chips.Node("n28");
        int huge = (int)g.C.Chips.FoundryRoom(node);
        Assert.True(ChipQueries.FirstFreeMonth(g.C, g.State, node, huge) >= 0);
        ChipShortageSystem.Start(g.C, g.State, 1400);
        Assert.Equal(-1, ChipQueries.FirstFreeMonth(g.C, g.State, node, huge));
    }

    [Fact]
    public void Fab_order_cost_helper_scales_with_shortage()
    {
        Game g = SoloAtShortageWeek();
        var spec = new ChipSpec { WaferCost = 90_000 };
        long normal = ChipQueries.OrderCost(spec, 800, 50);
        ChipShortageSystem.Start(g.C, g.State, 1200);
        long during = ChipShortageSystem.ScalePrice(g.State, normal);
        Assert.Equal(MathI.MulPermille(normal, 1200), during);
    }

    [Fact]
    public void Shortage_ends_after_configured_weeks()
    {
        Game g = SoloAtShortageWeek(c => c.Chips.Shortage.DurationWeeks = 4);
        ChipShortageSystem.Start(g.C, g.State, 1100);
        int until = g.State.ChipShortageUntilTick;
        g.State.Tick = until - 1;
        Assert.True(ChipShortageSystem.Active(g.State));
        g.State.Tick = until;
        g.Weeks(1);
        Assert.False(ChipShortageSystem.Active(g.State));
        Assert.Equal(-1, g.State.ChipShortageUntilTick);
    }

    [Fact]
    public void Ended_event_fires_when_crunch_lifts()
    {
        Game g = SoloAtShortageWeek(c => c.Chips.Shortage.DurationWeeks = 2);
        ChipShortageSystem.Start(g.C, g.State, 1150);
        g.State.Tick = g.State.ChipShortageUntilTick;
        int before = g.State.Events.Count;
        g.Weeks(1);
        Assert.Contains(g.State.Events.Skip(before), e => e.Kind == EventKind.ChipShortageEnded && e.Amount == 1150);
    }

    [Fact]
    public void Inactive_before_dated_news()
    {
        Game g = SoloAtShortageWeek();
        g.State.Tick = TickOfEvent(g.C, "card-shortage") - 2;
        Assert.False(ChipShortageSystem.Active(g.State));
    }

    [Fact]
    public void Price_multiplier_is_identity_when_inactive()
    {
        Game g = SoloAtShortageWeek();
        Assert.Equal(MathI.Permille, ChipShortageSystem.PriceMulPermille(g.State));
        Assert.Equal(100_000L, ChipShortageSystem.ScalePrice(g.State, 100_000L));
    }

    [Fact]
    public void Stronger_world_event_refreshes_duration_and_strength()
    {
        Game g = SoloAtShortageWeek(c => c.Chips.Shortage.DurationWeeks = 10);
        ChipShortageSystem.Start(g.C, g.State, 1100);
        int until1 = g.State.ChipShortageUntilTick;
        g.State.Tick += 5;
        WorldEventConfig strong = g.C.WorldEvent("card-shortage");
        strong.ChipShortageStrengthPermille = 1300;
        ChipShortageSystem.OnWorldEvent(g.C, g.State, strong);
        Assert.Equal(1300, g.State.ChipShortageStrengthPermille);
        Assert.True(g.State.ChipShortageUntilTick >= until1);
    }

    [Fact]
    public void Config_clamps_event_strength()
    {
        Game g = SoloAtShortageWeek(c =>
        {
            c.Chips.Shortage.MinStrengthPermille = 1100;
            c.Chips.Shortage.MaxStrengthPermille = 1250;
        });
        ChipShortageSystem.Start(g.C, g.State, 9999);
        Assert.Equal(1250, g.State.ChipShortageStrengthPermille);
    }

    [Fact]
    public void Save_and_load_keeps_shortage_clock()
    {
        Game g = SoloAtShortageWeek();
        ChipShortageSystem.Start(g.C, g.State, 1180);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(g.State.ChipShortageUntilTick, loaded.State.ChipShortageUntilTick);
        Assert.Equal(1180, loaded.State.ChipShortageStrengthPermille);
        Assert.True(ChipShortageSystem.Active(loaded.State));
    }

    [Fact]
    public void Schema_16_on_new_game()
    {
        Game g = Game.Solo();
        Assert.Equal(21, g.State.Schema);
        Assert.Equal(-1, g.State.ChipShortageUntilTick);
        Assert.Equal(0, g.State.ChipShortageStrengthPermille);
    }

    [Fact]
    public void Migration_15_to_16_adds_shortage_fields()
    {
        Game g = Game.Solo();
        string json = g.Sim.Save().Replace("\"schema\":21", "\"schema\":15");
        var loaded = Simulation.Load(g.C, json);
        Assert.Equal(21, loaded.State.Schema);
        Assert.Equal(-1, loaded.State.ChipShortageUntilTick);
    }

    [Fact]
    public void Validator_rejects_out_of_range_shortage_on_world_event()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.WorldEvent("card-shortage").ChipShortageStrengthPermille = 2500;
        List<string> problems = ConfigValidator.Problems(c);
        Assert.True(problems.Count > 0);
        Assert.Contains(problems, p => p.Contains("card-shortage") || p.Contains("chipShortage"));
    }

    [Fact]
    public void Flavor_news_without_shortage_flag_leaves_prices()
    {
        Game g = SoloAtShortageWeek();
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.State.Tick);
        long before = VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen);
        g.State.Tick = TickOfEvent(g.C, "first-ipo-spree") - 1;
        g.Weeks(1);
        Assert.False(ChipShortageSystem.Active(g.State));
        Assert.Equal(before, VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen));
    }

    [Fact]
    public void Finance_weekly_bill_reflects_higher_rent()
    {
        Game g = SoloAtShortageWeek();
        g.Do(Command.SetRentedGpus(g.Id, 8));
        Finance.RunWeek(g.C, g.State, g.Me);
        long rentNormal = g.Me.Ledger.ThisWeek.GpuRent;
        g = SoloAtShortageWeek();
        g.Do(Command.SetRentedGpus(g.Id, 8));
        ChipShortageSystem.Start(g.C, g.State, 1200);
        Finance.RunWeek(g.C, g.State, g.Me);
        long rentCrunch = g.Me.Ledger.ThisWeek.GpuRent;
        Assert.Equal(MathI.MulPermille(rentNormal, 1200), rentCrunch);
    }

    [Fact]
    public void Foundry_mul_clamps_at_floor()
    {
        Game g = SoloAtShortageWeek();
        ChipShortageSystem.Start(g.C, g.State, 1600);
        Assert.Equal(500, ChipShortageSystem.FoundryRoomMulPermille(g.State));
    }

    [Fact]
    public void After_shortage_ends_gpu_prices_normalize()
    {
        Game g = SoloAtShortageWeek(c => c.Chips.Shortage.DurationWeeks = 1);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.State.Tick);
        long basePrice = VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen);
        ChipShortageSystem.Start(g.C, g.State, 1200);
        g.Weeks(2);
        Assert.False(ChipShortageSystem.Active(g.State));
        Assert.Equal(basePrice, VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen));
    }

    [Fact]
    public void Realistic_card_shortage_week_via_sim_step()
    {
        Game g = SoloAtShortageWeek();
        FireCardShortage(g);
        Assert.True(ChipShortageSystem.Active(g.State));
        Assert.Equal(1200, g.State.ChipShortageStrengthPermille);
    }

    [Fact]
    public void Duplicate_start_while_active_keeps_higher_strength()
    {
        Game g = SoloAtShortageWeek();
        ChipShortageSystem.Start(g.C, g.State, 1100);
        ChipShortageSystem.Start(g.C, g.State, 1400);
        Assert.Equal(1400, g.State.ChipShortageStrengthPermille);
        int events = 0;
        for (int i = 0; i < g.State.Events.Count; i++)
            if (g.State.Events[i].Kind == EventKind.ChipShortageBegan) events++;
        Assert.Equal(1, events);
    }

    [Fact]
    public void Listed_chip_purchase_price_scales()
    {
        Game g = SoloAtShortageWeek();
        long listPrice = 50_000;
        long count = 20;
        long normal = listPrice * count;
        ChipShortageSystem.Start(g.C, g.State, 1150);
        Assert.Equal(MathI.MulPermille(normal, 1150), ChipShortageSystem.ScalePrice(g.State, normal));
    }
}