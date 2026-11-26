using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q154: BoostChipYield command.</summary>
public class BoostChipYieldCommandTests
{
    static (Game g, ChipDesign d) Ready(int yield = 400)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 4, StartEraId = "punchline", RivalIds = new System.Collections.Generic.List<string>() }));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 10_000_000_000);
        var d = new ChipDesign
        {
            Id = g.State.TakeId(), Status = ChipDesignStatus.Ready, YieldPermille = yield, Target = "inference",
            NodeId = "n28", DieId = "small", FormatId = "fp16", MemoryId = "ddr", CoolingId = "air",
            PointsRequired = 100, PointsDone = 100, ReadyTick = g.Tick, Name = "Test", Version = 1,
        };
        g.Me.ChipDesigns.Add(d);
        return (g, d);
    }

    [Fact]
    public void Q154_main_raises_yield_below_cap()
    {
        var (g, d) = Ready(400);
        int before = d.YieldPermille;
        g.Do(Command.BoostChipYield(g.Id, d.Id));
        Assert.True(d.YieldPermille > before);
    }

    [Fact]
    public void Q154_zero_boost_disabled()
    {
        var (g, d) = Ready(100);
        g.C.Chips.BoostYieldPermille = 0;
        g.Refused(Command.BoostChipYield(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q154_max_clamps_at_spec_max_yield()
    {
        var (g, d) = Ready();
        int max = ChipQueries.Spec(g.C, d).MaxYieldPermille;
        d.YieldPermille = max - 10;
        g.Do(Command.BoostChipYield(g.Id, d.Id));
        Assert.Equal(max, d.YieldPermille);
    }

    [Fact]
    public void Q154_bankrupt_refused()
    {
        var (g, d) = Ready();
        g.Me.Bankrupt = true;
        g.Refused(Command.BoostChipYield(g.Id, d.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q154_save_load_yield()
    {
        var (g, d) = Ready();
        g.Do(Command.BoostChipYield(g.Id, d.Id));
        int y = d.YieldPermille;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(y, loaded.Me.ChipDesigns[0].YieldPermille);
    }

    [Fact]
    public void Q154_cross_listing_price_uses_higher_yield()
    {
        var (g, d) = Ready(600);
        g.Do(Command.BoostChipYield(g.Id, d.Id));
        var r = g.Try(Command.SetChipListing(g.Id, d.Id, 5000, 8));
        Assert.True(r.Ok);
        Assert.Equal(5000, g.Me.ChipListings[0].PricePerChip);
    }

    [Fact]
    public void Q154_refused_at_max_yield()
    {
        var (g, d) = Ready();
        d.YieldPermille = ChipQueries.Spec(g.C, d).MaxYieldPermille;
        g.Refused(Command.BoostChipYield(g.Id, d.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q154_needs_cash()
    {
        var (g, d) = Ready();
        g.Me.Cash = g.C.Chips.BoostYieldCashCost - 1;
        g.Refused(Command.BoostChipYield(g.Id, d.Id), CommandError.NotEnoughCash);
    }
}