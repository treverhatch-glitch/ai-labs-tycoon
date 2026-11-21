using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q103: cashPerFullLine via Grants ledger in MuseumSystem.RunWeek.</summary>
public class MuseumFullLineCashTests
{
    static void RetireMany(Game g, string line, int count, int capability = 80)
    {
        for (int i = 0; i < count; i++)
        {
            g.Me.Models.Add(new ModelState
            {
                Id = g.State.TakeId(),
                LineId = line,
                Name = line + "-v" + i,
                Status = ModelStatus.Retired,
                Capability = capability + i,
                TrainedTick = g.Tick - i,
                ReleasedTick = g.Tick - i
            });
        }
    }

    [Fact]
    public void Shipped_museum_cash_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(5000, c.Endings.Museum.CashPerFullLine);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Partial_line_grants_no_cash()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine - 1);
        long cashBefore = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cashBefore, g.Me.Cash);
        Assert.Equal(0, g.Me.Ledger.ThisWeek.Grants);
    }

    [Fact]
    public void Full_line_earns_cash_via_grants()
    {
        Game g = Game.Solo();
        int cap = g.C.Endings.Museum.PerLine;
        RetireMany(g, "mini", cap);
        long cashBefore = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cashBefore + g.C.Endings.Museum.CashPerFullLine, g.Me.Cash);
        Assert.Equal(g.C.Endings.Museum.CashPerFullLine, g.Me.Ledger.ThisWeek.Grants);
    }

    [Fact]
    public void Two_full_lines_double_cash_grant()
    {
        Game g = Game.Solo();
        int cap = g.C.Endings.Museum.PerLine;
        RetireMany(g, "mini", cap);
        RetireMany(g, "max", cap);
        long cashBefore = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long grant = 2 * g.C.Endings.Museum.CashPerFullLine;
        Assert.Equal(cashBefore + grant, g.Me.Cash);
        Assert.Equal(grant, g.Me.Ledger.ThisWeek.Grants);
    }

    [Fact]
    public void Zero_cash_config_disables_hook()
    {
        Game g = Game.Solo();
        g.C.Endings.Museum.CashPerFullLine = 0;
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        long cashBefore = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cashBefore, g.Me.Cash);
    }

    [Fact]
    public void Cash_grant_stacks_with_reputation_and_research()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        int repBefore = g.Me.Reputation;
        long rpBefore = g.Me.ResearchPoints;
        long cashBefore = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.Reputation > repBefore);
        Assert.True(g.Me.ResearchPoints > rpBefore);
        Assert.True(g.Me.Cash > cashBefore);
    }

    [Fact]
    public void Sim_week_runs_museum_cash_grant()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        long cashBefore = g.Me.Cash;
        g.Weeks(1);
        Assert.True(g.Me.Cash >= cashBefore + g.C.Endings.Museum.CashPerFullLine);
    }

    [Fact]
    public void Save_load_preserves_cash_grant_behavior()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        long cash = g.Me.Cash;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        RetireMany(loaded, "mini", loaded.C.Endings.Museum.PerLine);
        long before = loaded.Me.Cash;
        MuseumSystem.RunWeek(loaded.C, loaded.State, loaded.Me);
        Assert.True(loaded.Me.Cash > before);
        Assert.True(loaded.Me.Cash > cash);
    }

    [Fact]
    public void Bankrupt_lab_still_gets_museum_cash()
    {
        Game g = Game.Solo();
        g.Me.Bankrupt = true;
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        long cashBefore = g.Me.Cash;
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cashBefore + g.C.Endings.Museum.CashPerFullLine, g.Me.Cash);
    }

    [Fact]
    public void RetiredOnLine_query_matches_cash_gate()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        Assert.Equal(g.C.Endings.Museum.PerLine, MuseumSystem.RetiredOnLine(g.Me, "mini"));
    }

    [Fact]
    public void Extra_retired_beyond_cap_still_pays_full_line()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine + 2);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Endings.Museum.CashPerFullLine, g.Me.Ledger.ThisWeek.Grants);
    }

    [Fact]
    public void Negative_cash_config_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Endings.Museum.CashPerFullLine = -1));
    }

    [Fact]
    public void Grants_ledger_line_used_not_revenue()
    {
        Game g = Game.Solo();
        RetireMany(g, "mini", g.C.Endings.Museum.PerLine);
        MuseumSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Endings.Museum.CashPerFullLine, g.Me.Ledger.ThisWeek.Grants);
        Assert.Equal(0, g.Me.Ledger.ThisWeek.Revenue);
    }
}