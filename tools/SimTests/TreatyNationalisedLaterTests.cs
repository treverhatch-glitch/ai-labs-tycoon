using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TreatyNationalisedLaterTests
{
    static void StepMonthEnd(Game g)
    {
        int wpm = g.C.Calendar.WeeksPerMonth;
        for (int i = g.Tick % wpm; i < wpm - 1; i++) g.Sim.Step();
        g.Sim.Step();
    }

    static Game SoloTreaty(int foreignCount = 2, int foreignLevel = BlocStanding.Audit)
    {
        return Game.Solo(tweak: c =>
        {
            c.Endings.TreatyForeignLevelMin = foreignLevel;
            c.Endings.TreatyForeignBlocCount = foreignCount;
            c.World.Attention.DecayPermillePerMonth = 0;
        });
    }

    static void SetBlocLevel(LabState lab, string blocId, int level)
    {
        BlocStanding s = lab.Bloc(blocId);
        s.Level = level;
        s.Attention = MathI.Permille;
    }

    [Fact]
    public void Home_freeze_records_nationalised_on_month_end_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Endings.NationalisedHomeLevelMin = BlocStanding.Freeze;
            c.World.Attention.DecayPermillePerMonth = 0;
        });
        SetBlocLevel(g.Me, g.Me.HomeBlocId, BlocStanding.Freeze);
        StepMonthEnd(g);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.NationalisedId);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EndingReached && e.Ref == EndingSystem.NationalisedId);
    }

    [Fact]
    public void Two_foreign_audits_record_treaty_on_month_end_step()
    {
        var g = SoloTreaty();
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        SetBlocLevel(g.Me, "china", BlocStanding.Audit);
        StepMonthEnd(g);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.TreatyId);
    }

    [Fact]
    public void One_foreign_audit_does_not_record_treaty()
    {
        var g = SoloTreaty();
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        StepMonthEnd(g);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.TreatyId);
    }

    [Fact]
    public void Home_hearing_does_not_record_nationalised_before_freeze()
    {
        var g = Game.Solo(tweak: c => c.Endings.NationalisedHomeLevelMin = BlocStanding.Freeze);
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        home.Level = BlocStanding.Hearing;
        home.Attention = 700;
        StepMonthEnd(g);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.NationalisedId);
    }

    [Fact]
    public void Foreign_hearing_does_not_count_toward_treaty()
    {
        var g = SoloTreaty();
        g.Me.Bloc("eu").Level = BlocStanding.Hearing;
        g.Me.Bloc("china").Level = BlocStanding.Hearing;
        StepMonthEnd(g);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.TreatyId);
    }

    [Fact]
    public void Bankrupt_player_skips_nationalised_and_treaty_on_step()
    {
        var g = SoloTreaty();
        g.Me.Bankrupt = true;
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        SetBlocLevel(g.Me, "china", BlocStanding.Audit);
        StepMonthEnd(g);
        Assert.DoesNotContain(g.State.Endings, e => e.EndingId == EndingSystem.NationalisedId || e.EndingId == EndingSystem.TreatyId);
    }

    [Fact]
    public void Zero_cash_player_still_records_treaty_when_foreign_audits_fire()
    {
        var g = SoloTreaty();
        g.Me.Cash = 0;
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        SetBlocLevel(g.Me, "china", BlocStanding.Audit);
        StepMonthEnd(g);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.TreatyId);
    }

    [Fact]
    public void Max_attention_on_home_and_foreign_blocs_triggers_both_endings_same_month()
    {
        var g = SoloTreaty();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Me.Bloc(g.Me.HomeBlocId).Attention = MathI.Permille;
        g.Me.Bloc("eu").Level = BlocStanding.Audit;
        g.Me.Bloc("eu").Attention = MathI.Permille;
        g.Me.Bloc("china").Level = BlocStanding.Freeze;
        g.Me.Bloc("china").Attention = MathI.Permille;
        StepMonthEnd(g);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.TreatyId);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.NationalisedId);
    }

    [Fact]
    public void Treaty_records_once_then_not_again_on_a_second_month_end_step()
    {
        var g = SoloTreaty();
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        SetBlocLevel(g.Me, "china", BlocStanding.Audit);
        StepMonthEnd(g);
        StepMonthEnd(g);
        Assert.Equal(1, g.State.Endings.Count(e => e.EndingId == EndingSystem.TreatyId));
    }

    [Fact]
    public void Nationalised_records_once_then_not_again_on_a_second_month_end_step()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Endings.NationalisedHomeLevelMin = BlocStanding.Freeze;
            c.World.Attention.DecayPermillePerMonth = 0;
        });
        SetBlocLevel(g.Me, g.Me.HomeBlocId, BlocStanding.Freeze);
        StepMonthEnd(g);
        StepMonthEnd(g);
        Assert.Equal(1, g.State.Endings.Count(e => e.EndingId == EndingSystem.NationalisedId));
    }

    [Fact]
    public void Save_load_round_trip_keeps_open_treaty_ending()
    {
        var g = SoloTreaty();
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        SetBlocLevel(g.Me, "china", BlocStanding.Audit);
        StepMonthEnd(g);
        string json = g.Sim.Save();
        Simulation loaded = Simulation.Load(g.C, json);
        Assert.Contains(loaded.State.Endings, e => e.EndingId == EndingSystem.TreatyId && e.Choice == "");
        SaveSchemaPretend.AssertRoundTrip(loaded);
    }

    [Fact]
    public void Migration_20_to_24_preserves_nationalised_ending_record()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Endings.NationalisedHomeLevelMin = BlocStanding.Freeze;
            c.World.Attention.DecayPermillePerMonth = 0;
        });
        SetBlocLevel(g.Me, g.Me.HomeBlocId, BlocStanding.Freeze);
        StepMonthEnd(g);
        string old = SaveSchemaPretend.At(g.C, 20, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Contains(sim.State.Endings, e => e.EndingId == EndingSystem.NationalisedId);
        SaveSchemaPretend.AssertRoundTrip(sim);
    }

    [Fact]
    public void Bubble_crash_month_and_treaty_both_run_on_month_end_step()
    {
        var g = SoloTreaty();
        g.State.BubbleCrashMonthsRemaining = 6;
        g.State.BubblePermille = g.C.Bubble.PostCrashResidualPermille;
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        SetBlocLevel(g.Me, "china", BlocStanding.Audit);
        int crashBefore = g.State.BubbleCrashMonthsRemaining;
        StepMonthEnd(g);
        Assert.True(g.State.BubbleCrashMonthsRemaining < crashBefore);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.TreatyId);
    }

    [Fact]
    public void Player_can_still_acquire_after_nationalised_ending()
    {
        var g = Game.New(seed: 201, tweak: c =>
        {
            c.Endings.NationalisedHomeLevelMin = BlocStanding.Freeze;
            c.Acquisitions.RegulatorDealRevenue = 1_000_000_000;
            c.World.Attention.DecayPermillePerMonth = 0;
        });
        SetBlocLevel(g.Me, g.Me.HomeBlocId, BlocStanding.Freeze);
        StepMonthEnd(g);
        LabState rival = g.Rival("openaye");
        rival.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 500,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "Distressed"
        });
        rival.Cash = -50_000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        int attBefore = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(g.Id, rival.AbsorbedByLabId);
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention >= attBefore);
    }

    [Fact]
    public void Rival_rollup_during_crash_still_runs_after_treaty_is_recorded()
    {
        var g = Game.New(seed: 202, tweak: c =>
        {
            c.Endings.TreatyForeignLevelMin = BlocStanding.Audit;
            c.Endings.TreatyForeignBlocCount = 2;
            c.Acquisitions.CrashDistressPricePermille = 500;
            c.World.Attention.DecayPermillePerMonth = 0;
        });
        SetBlocLevel(g.Me, "eu", BlocStanding.Audit);
        SetBlocLevel(g.Me, "china", BlocStanding.Audit);
        g.State.BubbleCrashMonthsRemaining = 12;
        StepMonthEnd(g);
        Assert.Contains(g.State.Endings, e => e.EndingId == EndingSystem.TreatyId);
        LabState buyer = g.Rival("openaye");
        LabState target = g.Rival("anthoptics");
        target.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 600,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "RollupTarget"
        });
        target.Cash = -100_000;
        Finance.Earn(buyer, LedgerLine.CapitalIn, 200_000_000);
        Assert.True(AcquisitionSystem.CanAcquire(g.C, g.State, buyer, target, out _));
        AcquisitionSystem.Apply(g.C, g.State, buyer, target);
        Assert.Equal(buyer.Id, target.AbsorbedByLabId);
    }
}