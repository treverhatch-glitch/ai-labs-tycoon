using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Shady path depth: government card, grey compute, audit investigations, corruption trust, freeze extras.</summary>
public class ShadyPathLaterTests
{
    [Fact]
    public void Hack_government_success_drops_bloc_attention()
    {
        var g = Game.Solo(seed: 4, tweak: c =>
        {
            ShadyCardConfig card = c.Shady.Card("hack-government");
            card.Weeks = 1;
            card.SuccessPermille = 1000;
            card.TracePermille = 0;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2006);
        g.Me.Bloc("eu").Attention = 700;
        int before = g.Me.Bloc("eu").Attention;
        var model = new ModelState { Id = g.State.TakeId(), Capability = 900, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        g.Do(Command.StartShadyOp(g.Id, "hack-government", model.Id, -1, -1, "eu"));
        g.Until(() => g.Me.ShadyOps.Count == 0, 12, "hack");
        Assert.Equal(before - 300, g.Me.Bloc("eu").Attention);
    }

    [Fact]
    public void Hack_government_failure_raises_bloc_attention()
    {
        var g = Game.Solo(seed: 5, tweak: c =>
        {
            ShadyCardConfig card = c.Shady.Card("hack-government");
            card.Weeks = 1;
            card.SuccessPermille = 0;
            card.TracePermille = 0;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2006);
        g.Me.Bloc("eu").Attention = 500;
        int before = g.Me.Bloc("eu").Attention;
        var model = new ModelState { Id = g.State.TakeId(), Capability = 900, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        g.Do(Command.StartShadyOp(g.Id, "hack-government", model.Id, -1, -1, "eu"));
        g.Until(() => g.Me.ShadyOps.Count == 0, 12, "hack");
        Assert.Equal(before + 400, g.Me.Bloc("eu").Attention);
    }

    [Fact]
    public void Grey_compute_pays_cash_from_leased_kilowatts()
    {
        var g = Game.Solo(seed: 2, tweak: c =>
        {
            ShadyCardConfig card = c.Shady.Card("grey-compute");
            card.Weeks = 1;
            card.SuccessPermille = 1000;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2016);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 5_000_000);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 4000;
        g.Do(Command.BuyGpus(g.Id, 80, hall));
        var model = new ModelState { Id = g.State.TakeId(), Capability = 100, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        int halfKw = WorldQueries.InstalledKw(g.C, g.State, g.Me, hall) / 2;
        long greyBefore = g.Me.Ledger.Lifetime.GreyRevenue;
        g.Do(Command.StartShadyOp(g.Id, "grey-compute", model.Id, -1, hall, ""));
        g.Until(() => g.Me.ShadyOps.Count == 0, 14, "grey");
        long payout = g.Me.Ledger.Lifetime.GreyRevenue - greyBefore;
        halfKw = WorldQueries.InstalledKw(g.C, g.State, g.Me, hall) / 2;
        Assert.Equal(g.C.Shady.Card("grey-compute").BasePayout * halfKw, payout);
    }

    [Fact]
    public void Audit_level_calls_the_extra_investigation_roll()
    {
        Game g = null;
        for (long seed = 1; seed < 300; seed++)
        {
            g = Game.Solo(seed: seed);
            g.Me.Heat = 900;
            g.State.Rng = RngSet.FromSeed(seed);
            ShadySystem.OnAuditOpen(g.C, g.State, g.Me);
            if (g.Me.CaseEndsTick >= 0) break;
            g = null;
        }
        Assert.NotNull(g);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ShadyInvestigation && e.Ref == "audit");
    }

    [Fact]
    public void Corruption_cuts_effective_alignment_for_trust()
    {
        var g = Game.Solo();
        var m = new ModelState { Alignment = 800, Corruption = 400, Status = ModelStatus.Released };
        int trust = SafetySystem.EffectiveAlignment(g.C, m);
        Assert.Equal(800 - (int)MathI.MulPermille(400, g.C.Shady.CorruptionTrustWeight), trust);
    }

    [Fact]
    public void A_open_case_blocks_new_ops_and_laundering()
    {
        var g = Game.Solo();
        g.Me.Stash = 10_000;
        g.Me.CaseEndsTick = g.Tick + 8;
        var model = new ModelState { Id = g.State.TakeId(), Capability = 200, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        g.Refused(Command.StartShadyOp(g.Id, "liberate-funds", model.Id, -1, -1, ""), CommandError.CaseOpen);
        g.Refused(Command.Launder(g.Id, 1000), CommandError.CaseOpen);
    }

    [Fact]
    public void Acquittal_drops_heat_when_the_case_closes_clean()
    {
        bool acquitted = false;
        for (long seed = 1; seed < 400; seed++)
        {
            var g = Game.Solo(seed: seed);
            g.Me.Heat = 450;
            g.Me.CaseEndsTick = g.Tick + g.C.Shady.CaseWeeks;
            g.State.Tick = g.Me.CaseEndsTick;
            g.State.Rng = RngSet.FromSeed(seed);
            int heatBefore = g.Me.Heat;
            ShadySystem.RunMonth(g.C, g.State, g.Me);
            if (g.State.Endings.Any(e => e.EndingId == "handcuffs")) continue;
            if (g.Me.CaseEndsTick < 0 && g.Me.Heat == heatBefore - g.C.Shady.AcquittalHeatDrop)
            {
                acquitted = true;
                break;
            }
        }
        Assert.True(acquitted);
    }

    [Fact]
    public void Low_runway_posts_a_temptation_event()
    {
        var g = Game.Solo(seed: 33, tweak: c => c.Shady.TemptationRunwayWeeks = 20);
        g.Me.RentedGpus = 64;
        g.Me.Cash = 1000;
        ShadySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ShadyTemptation);
    }

    [Fact]
    public void Crypto_heist_adds_attention_in_every_bloc()
    {
        var g = Game.Solo(seed: 4);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2012);
        int[] before = g.Me.Blocs.Select(b => b.Attention).ToArray();
        var model = new ModelState { Id = g.State.TakeId(), Capability = 600, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        foreach (ShadyCardConfig card in g.C.Shady.Cards) if (card.Id == "crypto-heist") { card.SuccessPermille = 1000; card.TracePermille = 0; }
        g.Do(Command.StartShadyOp(g.Id, "crypto-heist", model.Id, -1, -1, ""));
        g.Until(() => g.Me.ShadyOps.Count == 0, 10, "heist");
        for (int i = 0; i < g.Me.Blocs.Count; i++)
            Assert.Equal(before[i] + 80, g.Me.Blocs[i].Attention);
    }

    [Fact]
    public void Intrusion_for_hire_scales_stash_by_capability()
    {
        var g = Game.Solo(seed: 5);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2000);
        g.Me.Stash = 0;
        var model = new ModelState { Id = g.State.TakeId(), Capability = 300, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        foreach (ShadyCardConfig card in g.C.Shady.Cards) if (card.Id == "intrusion-for-hire") { card.SuccessPermille = 1000; card.TracePermille = 0; }
        g.Do(Command.StartShadyOp(g.Id, "intrusion-for-hire", model.Id, -1, -1, ""));
        g.Until(() => g.Me.ShadyOps.Count == 0, 8, "intrusion");
        long expect = MathI.MulPermille(g.C.Shady.Card("intrusion-for-hire").BasePayout, 300 * MathI.Permille / 100);
        Assert.Equal(expect, g.Me.Stash);
    }

    [Fact]
    public void A_freeze_in_the_home_bloc_blocks_reserved_compute_signup()
    {
        var g = Game.Solo();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Me.Cash = 10_000_000;
        g.Refused(Command.SignReservedCompute(g.Id, 10), CommandError.PermitFrozen);
    }

    [Fact]
    public void Smuggle_chips_into_china_ignores_export_rules()
    {
        var g = Game.Solo(seed: 19);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2024);
        ChipDesign d = new ChipDesign { Id = 1, Target = "training", NodeId = "n5", Status = ChipDesignStatus.Ready };
        g.Me.ChipDesigns.Add(d);
        int us = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(us).HallKw = 5000;
        ChipSystem.AddChips(g.State, g.Me, us, g.Id, d.Id, 5);
        HardwareBatch batch = g.Me.Fleet.Single(b => b.ChipDesignId >= 0);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int china = g.Do(Command.BuyLand(g.Id, chinaSite)).CreatedId;
        var model = new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        foreach (ShadyCardConfig card in g.C.Shady.Cards) if (card.Id == "smuggle-chips") { card.SuccessPermille = 1000; card.TracePermille = 0; }
        g.Do(Command.StartShadyOp(g.Id, "smuggle-chips", model.Id, -1, batch.Id, china.ToString()));
        g.Until(() => g.Me.ShadyOps.Count == 0, 8, "smuggle");
        Assert.Equal(china, batch.HoldingId);
    }
}