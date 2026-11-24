using System.Linq;
using AiLabs.Rules.Core;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class FreezeAuditLaterTests
{
    [Fact]
    public void QueueGpuPurchase_is_refused_when_the_home_bloc_is_frozen()
    {
        var g = Game.Solo();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.QueueGpuPurchase(g.Id, 2, HardwareBatch.Hq), CommandError.PermitFrozen);
    }

    [Fact]
    public void RaiseFunding_is_refused_when_the_home_bloc_is_frozen()
    {
        var g = Game.Solo(tweak: c =>
        {
            foreach (var r in c.Funding.Rounds) r.OpensYear = 1995;
            c.Funding.Round("angel").MinReputation = 0;
        });
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.RaiseFunding(g.Id, "angel"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Audit_level_fines_revenue_and_can_open_a_shady_investigation()
    {
        var g = Game.Solo(seed: 4, tweak: c => c.Shady.AuditInvestigationPermille = 1000);
        g.Me.Heat = 400;
        g.Me.LastWeek.Revenue = 50_000;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Hearing;
        g.Me.Bloc(g.Me.HomeBlocId).Attention = g.C.World.Attention.AuditAt + 100;
        long cashBefore = g.Me.Cash;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        Assert.Equal(BlocStanding.Audit, g.Me.Bloc(g.Me.HomeBlocId).Level);
        Assert.True(g.Me.Cash < cashBefore);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ShadyInvestigation && e.Ref == "audit");
        Assert.True(g.Me.CaseEndsTick > g.Tick);
    }

    [Fact]
    public void OnAuditOpen_respects_the_audit_floor_even_with_no_heat()
    {
        var g = Game.Solo(seed: 8, tweak: c => c.Shady.AuditInvestigationPermille = 1000);
        g.Me.Heat = 0;
        g.State.Rng = RngSet.FromSeed(8);
        ShadySystem.OnAuditOpen(g.C, g.State, g.Me);
        Assert.True(g.Me.CaseEndsTick > g.Tick);
    }

    [Fact]
    public void SignReservedCompute_still_respects_home_freeze()
    {
        var g = Game.Solo();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Me.Cash = 10_000_000;
        g.Refused(Command.SignReservedCompute(g.Id, 4), CommandError.PermitFrozen);
    }

    [Fact]
    public void BuyGpus_at_HQ_is_refused_when_home_is_frozen()
    {
        var g = Game.Solo().WithStarterLab();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Me.Cash = 10_000_000;
        g.Refused(Command.BuyGpus(g.Id, 1, HardwareBatch.Hq), CommandError.PermitFrozen);
    }

    [Fact]
    public void Leasing_more_colo_in_a_frozen_bloc_is_refused()
    {
        var g = Game.Solo();
        SiteConfig site = g.C.World.Site("eu-ireland");
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, site.OpensYear);
        g.Me.Bloc("eu").Level = BlocStanding.Freeze;
        g.Refused(Command.LeaseColo(g.Id, site.Id, 100), CommandError.PermitFrozen);
    }

    [Fact]
    public void Building_a_hall_in_a_frozen_bloc_is_refused()
    {
        var g = Game.Solo();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2005);
        string site = g.C.World.Sites.First(s => s.BlocId == "eu").Id;
        int land = g.Do(Command.BuyLand(g.Id, site)).CreatedId;
        g.Me.Bloc("eu").Level = BlocStanding.Freeze;
        g.Refused(Command.BuildHall(g.Id, land, 5000), CommandError.PermitFrozen);
    }

    [Fact]
    public void Hearing_hits_reputation_when_attention_climbs()
    {
        var g = Game.Solo();
        int rep = g.Me.Reputation;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        g.Me.Ledger.LastWeek.Revenue = 0;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        Assert.Equal(BlocStanding.Hearing, g.Me.Bloc("us").Level);
        Assert.True(g.Me.Reputation < rep);
    }

    [Fact]
    public void Freeze_lifts_when_attention_falls_below_the_exit_line()
    {
        var g = Game.Solo();
        g.Me.Bloc("us").Level = BlocStanding.Freeze;
        g.Me.Bloc("us").Attention = g.C.World.Attention.FreezeExitBelow - 10;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        Assert.True(g.Me.Bloc("us").Level < BlocStanding.Freeze);
    }

    [Fact]
    public void Schema_12_migration_adds_bubble_permille()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.State.BubblePermille);
    }

    [Fact]
    public void Buy_land_in_a_frozen_bloc_is_still_refused()
    {
        var g = Game.Solo();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2005);
        string site = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        g.Me.Bloc("china").Level = BlocStanding.Freeze;
        g.Refused(Command.BuyLand(g.Id, site), CommandError.PermitFrozen);
    }
}