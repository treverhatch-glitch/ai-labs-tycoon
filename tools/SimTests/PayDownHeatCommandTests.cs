using AiLabs.Rules.Sim;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q159: PayDownHeat command.</summary>
public class PayDownHeatCommandTests
{
    [Fact]
    public void Q159_main_spends_cash_and_lowers_heat()
    {
        var g = Game.Solo();
        g.Me.Heat = 250;
        long cash = g.Me.Cash;
        g.Do(Command.PayDownHeat(g.Id));
        Assert.Equal(cash - g.C.Shady.HeatBuydownCashCost, g.Me.Cash);
        Assert.Equal(150, g.Me.Heat);
    }

    [Fact]
    public void Q159_zero_refused_at_zero_heat()
    {
        var g = Game.Solo();
        g.Me.Heat = 0;
        g.Refused(Command.PayDownHeat(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q159_max_clamps_heat_at_zero()
    {
        var g = Game.Solo();
        g.Me.Heat = 30;
        g.Do(Command.PayDownHeat(g.Id));
        Assert.Equal(0, g.Me.Heat);
    }

    [Fact]
    public void Q159_zero_amount_disabled()
    {
        var g = Game.Solo();
        g.Me.Heat = 100;
        g.C.Shady.HeatBuydownAmount = 0;
        g.Refused(Command.PayDownHeat(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Q159_bankrupt_refused()
    {
        var g = Game.Solo();
        g.Me.Heat = 100;
        g.Me.Bankrupt = true;
        g.Refused(Command.PayDownHeat(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Q159_save_load_heat()
    {
        var g = Game.Solo();
        g.Me.Heat = 200;
        g.Do(Command.PayDownHeat(g.Id));
        int heat = g.Me.Heat;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(heat, loaded.Me.Heat);
    }

    [Fact]
    public void Q159_cross_launder_still_works()
    {
        var g = Game.Solo();
        g.Me.Heat = 300;
        g.Me.Stash = 50_000;
        g.Do(Command.PayDownHeat(g.Id));
        g.Do(Command.Launder(g.Id, 10_000));
        Assert.True(g.Me.Heat > 0);
    }

    [Fact]
    public void Q159_needs_cash()
    {
        var g = Game.Solo();
        g.Me.Heat = 100;
        g.Me.Cash = g.C.Shady.HeatBuydownCashCost - 1;
        g.Refused(Command.PayDownHeat(g.Id), CommandError.NotEnoughCash);
    }
}