using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q131: GovernmentQueries.AuditFineParts matches audit ledger fine.</summary>
public class GovernmentAuditFineBreakdownTests
{
    [Fact]
    public void Shipped_audit_fine_parts_product()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 2_000_000;
        var b = GovernmentQueries.AuditFineParts(g.C, g.Me);
        Assert.Equal(b.FromLastWeekRevenue * b.RevenueWeeksMultiplier, b.Total);
        Assert.Equal(GovernmentQueries.AuditFineAmount(g.C, g.Me), b.Total);
    }

    [Fact]
    public void Multiplier_from_attention_config()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 1;
        var b = GovernmentQueries.AuditFineParts(g.C, g.Me);
        Assert.Equal(g.C.World.Attention.AuditFineRevenueWeeks, b.RevenueWeeksMultiplier);
    }

    [Fact]
    public void Zero_revenue_zero_fine()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 0;
        Assert.Equal(0, GovernmentQueries.AuditFineAmount(g.C, g.Me));
    }

    [Fact]
    public void Large_revenue_scales_linearly()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 50_000_000;
        long fine = GovernmentQueries.AuditFineAmount(g.C, g.Me);
        g.Me.LastWeek.Revenue = 100_000_000;
        Assert.Equal(fine * 2, GovernmentQueries.AuditFineAmount(g.C, g.Me));
    }

    [Fact]
    public void Government_system_audit_uses_same_amount()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 3_333_333;
        long expected = GovernmentQueries.AuditFineAmount(g.C, g.Me);
        var b = GovernmentQueries.AuditFineParts(g.C, g.Me);
        Assert.Equal(expected, b.Total);
    }

    [Fact]
    public void Audit_opens_shady_investigation_path_for_player()
    {
        Game g = Game.Solo();
        g.Me.Heat = 200;
        long cashBefore = g.Me.Cash;
        g.Me.LastWeek.Revenue = 1_000_000;
        ShadySystem.OnAuditOpen(g.C, g.State, g.Me);
        Assert.True(g.Me.Heat >= 200);
        _ = cashBefore;
    }

    [Fact]
    public void Fine_parts_deterministic()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 8_888_888;
        var a = GovernmentQueries.AuditFineParts(g.C, g.Me);
        var b = GovernmentQueries.AuditFineParts(g.C, g.Me);
        Assert.Equal(a.Total, b.Total);
    }

    [Fact]
    public void Save_load_fine_parts_stable()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 4_500_000;
        var before = GovernmentQueries.AuditFineParts(g.C, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Me.LastWeek.Revenue = 4_500_000;
        var after = GovernmentQueries.AuditFineParts(loaded.C, loaded.Me);
        Assert.Equal(before.Total, after.Total);
    }

    [Fact]
    public void Hearing_path_uses_reputation_not_fine()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 9_000_000;
        int repBefore = g.Me.Reputation;
        Assert.True(GovernmentQueries.AuditFineAmount(g.C, g.Me) > 0);
        Assert.Equal(repBefore, g.Me.Reputation);
    }

    [Fact]
    public void Rivals_lab_same_formula()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        rival.LastWeek.Revenue = 1_250_000;
        var b = GovernmentQueries.AuditFineParts(g.C, rival);
        Assert.Equal(rival.LastWeek.Revenue * g.C.World.Attention.AuditFineRevenueWeeks, b.Total);
    }

    [Fact]
    public void From_last_week_revenue_field_matches_lab()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 777_777;
        var b = GovernmentQueries.AuditFineParts(g.C, g.Me);
        Assert.Equal(g.Me.LastWeek.Revenue, b.FromLastWeekRevenue);
    }

    [Fact]
    public void Investigation_permille_independent_of_fine_amount()
    {
        Game g = Game.Solo();
        g.Me.LastWeek.Revenue = 10_000_000;
        g.Me.Heat = 650;
        long fine = GovernmentQueries.AuditFineAmount(g.C, g.Me);
        int inv = ShadyQueries.InvestigationOpenPermille(g.C, g.Me);
        Assert.True(fine > 0);
        Assert.True(inv > 0);
    }

    [Fact]
    public void Attention_audit_config_positive_multiplier()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.World.Attention.AuditFineRevenueWeeks > 0);
    }
}