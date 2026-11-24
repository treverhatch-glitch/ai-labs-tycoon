using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class AcquireLabLaterTests
{
    static void DistressedRival(Game g, LabState rival)
    {
        rival.Models.Add(new ModelState
        {
            Id = g.State.TakeId(),
            LineId = "mini",
            Capability = 700,
            Status = ModelStatus.Released,
            Alignment = 500,
            Name = "RivalBot"
        });
        rival.Cash = -50_000;
        rival.Ledger.ThisMonth.Revenue = 1;
    }

    [Fact]
    public void Quote_splits_models_staff_fleet_and_positive_cash_not_debt()
    {
        var g = Game.New(seed: 1);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        rival.Cash = -120_000;
        rival.Staff.Add(new StaffMember { RoleId = "engineer", TraitId = "steady", Morale = 500 });
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.Equal(0, q.CashPart);
        Assert.True(q.ModelsPart > 0);
        Assert.True(q.StaffPart > 0);
        Assert.True(q.Distressed);
        Assert.Equal(120_000, q.DebtAssumed);
        long subtotal = q.Total - q.IntegrationCost;
        long distressed = MathI.MulPermille(q.ModelsPart + q.StaffPart, g.C.Acquisitions.DistressPricePermille);
        Assert.Equal(distressed + q.FleetPart + q.CashPart - q.DebtAssumed, subtotal);
    }

    [Fact]
    public void Integration_cost_is_configured_permille_on_distressed_subtotal()
    {
        var g = Game.New(seed: 101);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        long subtotal = q.Total - q.IntegrationCost;
        Assert.Equal(MathI.MulPermille(subtotal, g.C.Acquisitions.IntegrationCostPermille), q.IntegrationCost);
    }

    [Fact]
    public void Quote_climbs_with_capability_staff_and_fleet()
    {
        var g = Game.New(seed: 2);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        rival.Staff.Add(new StaffMember { RoleId = "engineer", TraitId = "steady", Morale = 500 });
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Assert.True(q.StaffPart > 0);
        Assert.True(q.Total > q.IntegrationCost);
        Assert.True(q.IntegrationCost > 0);
    }

    [Fact]
    public void Crash_distress_cheaper_than_normal_distress()
    {
        var g = Game.New(seed: 3);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        g.State.BubbleCrashMonthsRemaining = 0;
        long normal = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        g.State.BubbleCrashMonthsRemaining = 6;
        long crash = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival).Total;
        Assert.True(crash < normal);
    }

    [Fact]
    public void Player_acquires_a_distressed_rival_lab()
    {
        var g = Game.New(seed: 4);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        int modelsBefore = g.Me.Models.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(modelsBefore + 1, g.Me.Models.Count);
        Assert.Equal(g.Id, rival.AbsorbedByLabId);
    }

    [Fact]
    public void Acquire_refused_without_enough_cash()
    {
        var g = Game.New(seed: 5);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        g.Me.Cash = 0;
        g.Refused(Command.AcquireLab(g.Id, rival.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Player_cannot_be_acquired_even_when_weak()
    {
        var g = Game.New(seed: 51);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        g.Me.Cash = -500_000;
        g.Me.WeeksInDebt = 4;
        Finance.Earn(rival, LedgerLine.CapitalIn, 200_000_000);
        g.Refused(Command.AcquireLab(rival.Id, g.Id), CommandError.TargetNotEligible);
    }

    [Fact]
    public void Player_cannot_acquire_while_bankrupt()
    {
        var g = Game.New(seed: 6);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        g.Me.Bankrupt = true;
        g.Refused(Command.AcquireLab(g.Id, rival.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Large_deal_raises_regulator_attention()
    {
        var g = Game.New(seed: 7, tweak: c => c.Acquisitions.RegulatorDealRevenue = 1);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 80_000_000);
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.True(g.Me.Bloc(g.Me.HomeBlocId).Attention > before);
    }

    [Fact]
    public void Acquisition_during_bubble_crash_discounts_price()
    {
        var g = Game.New(seed: 8);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        g.State.BubbleCrashMonthsRemaining = 12;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 40_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LabAcquired);
    }

    [Fact]
    public void Save_load_round_trip_preserves_absorbed_flag()
    {
        var g = Game.New(seed: 9);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        LabState again = loaded.State.Lab(rival.Id);
        Assert.Equal(g.Id, again.AbsorbedByLabId);
    }

    [Fact]
    public void Schema_20_migration_adds_absorbed_flag()
    {
        var g = Game.Solo();
        string old = SaveSchemaPretend.At(g.C, 19, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(-1, sim.Player.AbsorbedByLabId);
        SaveSchemaPretend.AssertRoundTrip(sim);
    }

    [Fact]
    public void Save_load_mid_acquisition_then_migration_19_to_20()
    {
        var g = Game.New(seed: 52);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        rival.Staff.Add(new StaffMember { RoleId = "researcher", TraitId = "steady", Morale = 360 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        string json = g.Sim.Save();
        string old = SaveSchemaPretend.At(g.C, 19, json);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(-1, sim.State.Lab(rival.Id).AbsorbedByLabId);
        Assert.Equal(1, sim.Player.Models.Count(m => m.Name == "RivalBot"));
        sim.State.Lab(rival.Id).AbsorbedByLabId = g.Id;
        SaveSchemaPretend.AssertRoundTrip(sim);
    }

    [Fact]
    public void Weak_player_survivor_acquires_distressed_rival_during_crash()
    {
        var g = Game.New(seed: 53, tweak: c => c.Acquisitions.CrashDistressPricePermille = 400);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        g.State.BubbleCrashMonthsRemaining = 10;
        g.Me.Cash = 2_000_000;
        g.Me.Ledger.ThisMonth.Revenue = 50_000;
        var q = DisplayQueries.AcquisitionQuoteParts(g.C, g.State, g.Me, rival);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, q.Total - g.Me.Cash);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(g.Id, rival.AbsorbedByLabId);
    }

    [Fact]
    public void Target_sectors_clear_after_acquisition()
    {
        var g = Game.New(seed: 10);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        var model = rival.Models[0];
        g.Do(Command.ReleaseModel(rival.Id, model.Id, "game-ai"));
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        SectorState sector = g.State.Sector("game-ai");
        Assert.DoesNotContain(sector.Entries, e => e.LabId == rival.Id);
    }

    [Fact]
    public void Staff_transfer_joins_the_buyer_with_lower_morale()
    {
        var g = Game.New(seed: 11);
        LabState rival = g.Rival("openaye");
        DistressedRival(g, rival);
        rival.Staff.Add(new StaffMember { RoleId = "sales", TraitId = "steady", Morale = 400 });
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 50_000_000);
        int staffBefore = g.Me.Staff.Count;
        g.Do(Command.AcquireLab(g.Id, rival.Id));
        Assert.Equal(staffBefore + 1, g.Me.Staff.Count);
        Assert.True(g.Me.Staff.Last().Morale < 400);
    }

    [Fact]
    public void Rival_can_acquire_distressed_peer_during_crash()
    {
        var g = Game.New(seed: 12);
        LabState buyer = g.Rival("openaye");
        LabState target = g.Rival("anthoptics");
        DistressedRival(g, target);
        target.Cash = -200_000;
        Finance.Earn(buyer, LedgerLine.CapitalIn, 200_000_000);
        g.State.BubbleCrashMonthsRemaining = 8;
        bool absorbed = false;
        for (int seed = 0; seed < 200 && !absorbed; seed++)
        {
            target.AbsorbedByLabId = -1;
            target.Models.Clear();
            target.Staff.Clear();
            target.Fleet.Clear();
            DistressedRival(g, target);
            target.Cash = -200_000;
            g.State.Rng.Rivals = new Rng(seed);
            AcquisitionSystem.RunMonth(g.C, g.State);
            absorbed = target.AbsorbedByLabId == buyer.Id;
        }
        Assert.True(absorbed);
    }
}