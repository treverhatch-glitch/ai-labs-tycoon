using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q173–Q178 player-choice systems (arrears, hedge, reserved break, disclose, pause listing, vendor advocate).</summary>
public class PlayerChoicesQ173Q178Tests
{
    static Game Play(System.Action<GameConfig> tweak = null) =>
        Game.New(tweak: tweak).WithStarterLab(gpus: 16);

    static Game Solo(System.Action<GameConfig> tweak = null) =>
        Game.Solo(tweak: tweak).WithStarterLab(gpus: 16);

    static void PeerLeaseInDebt(Game g, int gpus = 4)
    {
        LabState landlord = g.Rival("openaye");
        landlord.Fleet.Add(new HardwareBatch
        {
            Id = g.State.NextId++,
            HoldingId = HardwareBatch.Hq,
            GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id,
            Count = gpus + 4,
        });
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, gpus));
        g.Me.WeeksInDebt = 1;
    }

    static void HeatwaveSite(Game g)
    {
        string siteId = g.C.World.Sites[0].Id;
        g.Me.Holdings.Add(new Holding { Id = g.State.NextId++, SiteId = siteId, HallKw = 200 });
        g.State.Site(siteId).HeatwaveUntilTick = g.Tick + 8;
    }

    static void ReadyListing(Game g, int designId)
    {
        var d = new ChipDesign
        {
            Id = designId,
            Status = ChipDesignStatus.Ready,
            Name = "c" + designId,
            Version = 1,
            YieldPermille = 800,
            Target = "inference",
            NodeId = "n28",
            DieId = "small",
            FormatId = "fp16",
            MemoryId = "ddr",
            CoolingId = "air",
            PointsRequired = 100,
            PointsDone = 100,
            ReadyTick = g.Tick,
        };
        g.Me.ChipDesigns.Add(d);
        g.Me.ChipListings.Add(new ChipListing { DesignId = designId, PricePerChip = 100, MaxPerMonth = 10, ListedSinceTick = g.Tick });
    }

    // Q173 PayPeerLeaseArrears
    [Fact] public void Q173_main_arrears_payment_sets_shield_and_blocks_trim()
    {
        Game g = Play();
        PeerLeaseInDebt(g);
        int before = PeerComputeSystem.LeasedInGpus(g.Me);
        g.Do(Command.PayPeerLeaseArrears(g.Id));
        Assert.True(g.Me.PeerLeaseShieldUntilTick > g.Tick);
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before, PeerComputeSystem.LeasedInGpus(g.Me));
    }

    [Fact] public void Q173_zero_arrears_cost_disabled_refuses()
    {
        Game g = Play(c => c.Compute.PeerLeaseArrearsCashPerGpu = 0);
        PeerLeaseInDebt(g);
        g.Refused(Command.PayPeerLeaseArrears(g.Id), CommandError.InvalidArgument);
    }

    [Fact] public void Q173_max_long_shield_weeks()
    {
        Game g = Play(c => c.Compute.PeerLeaseShieldWeeks = 52);
        PeerLeaseInDebt(g);
        g.Do(Command.PayPeerLeaseArrears(g.Id));
        Assert.True(g.Me.PeerLeaseShieldUntilTick >= g.Tick + 52);
    }

    [Fact] public void Q173_bankrupt_refuses()
    {
        Game g = Solo();
        g.Me.Bankrupt = true;
        g.Refused(Command.PayPeerLeaseArrears(g.Id), CommandError.LabBankrupt);
    }

    [Fact] public void Q173_save_load_shield_persists()
    {
        Game g = Solo();
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = 2, GpuCount = 4 });
        g.Me.WeeksInDebt = 1;
        g.Do(Command.PayPeerLeaseArrears(g.Id));
        int shield = g.Me.PeerLeaseShieldUntilTick;
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(shield, loaded.Player.PeerLeaseShieldUntilTick);
    }

    [Fact] public void Q173_cross_trim_without_shield()
    {
        Game g = Play();
        PeerLeaseInDebt(g);
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(PeerComputeSystem.LeasedInGpus(g.Me) < 4);
    }

    // Q174 BuyPowerHedge
    [Fact] public void Q174_main_hedge_cuts_heatwave_surcharge()
    {
        Game g = Play();
        HeatwaveSite(g);
        g.Do(Command.BuyPowerHedge(g.Id));
        long raw = HeatwavePowerSystem.WeeklySurcharge(g.C, g.State, g.Me);
        long cash0 = g.Me.Cash;
        HeatwavePowerSystem.RunWeek(g.C, g.State, g.Me);
        long paid = cash0 - g.Me.Cash;
        Assert.True(raw > 0);
        Assert.True(paid < raw);
    }

    [Fact] public void Q174_zero_hedge_cost_disabled()
    {
        Game g = Solo(c => c.World.Heatwave.HedgeCashCost = 0);
        g.Refused(Command.BuyPowerHedge(g.Id), CommandError.InvalidArgument);
    }

    [Fact] public void Q174_max_full_waiver_permille()
    {
        Game g = Play(c => c.World.Heatwave.HedgeWaiverPermille = 1000);
        HeatwaveSite(g);
        g.Do(Command.BuyPowerHedge(g.Id));
        long cash0 = g.Me.Cash;
        HeatwavePowerSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash0, g.Me.Cash);
    }

    [Fact] public void Q174_bankrupt_refuses() { Game g = Solo(); g.Me.Bankrupt = true; g.Refused(Command.BuyPowerHedge(g.Id), CommandError.LabBankrupt); }

    [Fact] public void Q174_save_load_hedge_tick()
    {
        Game g = Solo();
        g.Do(Command.BuyPowerHedge(g.Id));
        int until = g.Me.PowerHedgeUntilTick;
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(until, loaded.Player.PowerHedgeUntilTick);
    }

    [Fact] public void Q174_cross_hedge_waives_heatwave_but_finance_still_bills_power()
    {
        Game g = Play();
        HeatwaveSite(g);
        g.Do(Command.BuyPowerHedge(g.Id));
        long cash0 = g.Me.Cash;
        HeatwavePowerSystem.RunWeek(g.C, g.State, g.Me);
        long afterHeat = g.Me.Cash;
        Finance.RunWeek(g.C, g.State, g.Me);
        long powerBill = -g.Me.Ledger.ThisWeek.Power;
        Assert.True(afterHeat > cash0 - HeatwavePowerSystem.WeeklySurcharge(g.C, g.State, g.Me));
        Assert.True(powerBill > 0);
        Assert.True(g.Me.Cash < afterHeat);
    }

    // Q175 BreakReservedContract
    [Fact] public void Q175_main_break_contract_removes_slot()
    {
        Game g = Play(c => c.Compute.ReservedDepositWeeks = 0);
        g.Do(Command.SignReservedCompute(g.Id, 4));
        Assert.Single(g.Me.ReservedContracts);
        g.Do(Command.BreakReservedContract(g.Id, 0));
        Assert.Empty(g.Me.ReservedContracts);
    }

    [Fact] public void Q175_zero_penalty_when_disabled()
    {
        Game g = Play(c => { c.Compute.ReservedBreakPenaltyPermille = 0; c.Compute.ReservedDepositWeeks = 0; });
        g.Do(Command.SignReservedCompute(g.Id, 2));
        long cash = g.Me.Cash;
        g.Do(Command.BreakReservedContract(g.Id, 0));
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact] public void Q175_max_penalty_scales_with_weeks_left()
    {
        Game g = Play(c => { c.Compute.ReservedBreakPenaltyPermille = 1000; c.Compute.ReservedDepositWeeks = 0; });
        g.Me.Cash = 5_000_000;
        g.Do(Command.SignReservedCompute(g.Id, 8));
        long cash = g.Me.Cash;
        g.Do(Command.BreakReservedContract(g.Id, 0));
        Assert.True(g.Me.Cash < cash);
    }

    [Fact] public void Q175_bankrupt_refuses() { Game g = Solo(); g.Me.Bankrupt = true; g.Refused(Command.BreakReservedContract(g.Id, 0), CommandError.LabBankrupt); }

    [Fact] public void Q175_save_load_contracts_after_break()
    {
        Game g = Play(c => c.Compute.ReservedDepositWeeks = 0);
        g.Do(Command.SignReservedCompute(g.Id, 2));
        g.Do(Command.BreakReservedContract(g.Id, 0));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Empty(loaded.Player.ReservedContracts);
    }

    [Fact] public void Q175_cross_break_frees_reserved_from_compute_rent()
    {
        Game g = Play(c => c.Compute.ReservedDepositWeeks = 0);
        g.Do(Command.SignReservedCompute(g.Id, 6));
        Assert.Equal(6, ComputeContractSystem.ReservedGpus(g.Me));
        long reservedRent = ComputeContractSystem.WeeklyReservedRent(g.C, g.State, g.Me);
        g.Do(Command.BreakReservedContract(g.Id, 0));
        Assert.Equal(0, ComputeContractSystem.ReservedGpus(g.Me));
        Assert.True(reservedRent > 0);
        Assert.Equal(0, ComputeContractSystem.WeeklyReservedRent(g.C, g.State, g.Me));
    }

    // Q176 DiscloseModelAlignment
    [Fact] public void Q176_main_disclose_closes_gap_and_reputation()
    {
        Game g = Solo();
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 950, "game-ai"));
        int rep0 = g.Me.Reputation;
        g.Do(Command.DiscloseModelAlignment(g.Id, m.Id));
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, m));
        Assert.True(g.Me.Reputation > rep0);
    }

    [Fact] public void Q176_zero_reputation_gain_still_closes_gap()
    {
        Game g = Solo(c => c.Safety.DiscloseReputationGain = 0);
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 950, "game-ai"));
        int rep = g.Me.Reputation;
        g.Do(Command.DiscloseModelAlignment(g.Id, m.Id));
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, m));
        Assert.Equal(rep, g.Me.Reputation);
    }

    [Fact] public void Q176_max_reputation_capped_at_permille()
    {
        Game g = Solo(c => c.Safety.DiscloseReputationGain = 5000);
        g.Me.Reputation = 990;
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 950, "game-ai"));
        g.Do(Command.DiscloseModelAlignment(g.Id, m.Id));
        Assert.Equal(MathI.Permille, g.Me.Reputation);
    }

    [Fact] public void Q176_bankrupt_refuses() { Game g = Solo(); g.Me.Bankrupt = true; g.Refused(Command.DiscloseModelAlignment(g.Id, 1), CommandError.LabBankrupt); }

    [Fact] public void Q176_save_load_displayed_alignment()
    {
        Game g = Solo();
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 950, "game-ai"));
        g.Do(Command.DiscloseModelAlignment(g.Id, m.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(loaded.Player.Model(m.Id).Alignment, loaded.Player.Model(m.Id).DisplayedAlignment);
    }

    [Fact] public void Q176_cross_disclose_changes_safety_incident_rung()
    {
        Game g = Solo();
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 950, "game-ai"));
        Assert.Equal(3, SafetySystem.PickIncidentRung(g.C, m));
        g.Do(Command.DiscloseModelAlignment(g.Id, m.Id));
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, m));
        Assert.NotEqual(3, SafetySystem.PickIncidentRung(g.C, m));
    }

    // Q177 PauseChipListing
    [Fact] public void Q177_main_pause_skips_upkeep()
    {
        Game g = Solo(c => c.Chips.ListingWeeklyUpkeep = 500);
        ReadyListing(g, 1);
        g.Do(Command.PauseChipListing(g.Id, 1, true));
        long cash = g.Me.Cash;
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact] public void Q177_zero_upkeep_pause_still_blocks_buy()
    {
        Game g = Play(c => c.Chips.ListingWeeklyUpkeep = 0);
        ReadyListing(g, 1);
        g.Me.ChipListings[0].Paused = true;
        LabState buyer = g.Rival("anthoptics");
        buyer.Cash = 1_000_000;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, 1, HardwareBatch.Hq, 1), CommandError.InvalidArgument);
    }

    [Fact] public void Q177_max_many_listings_only_paused_spared()
    {
        Game g = Solo(c => c.Chips.ListingWeeklyUpkeep = 300);
        ReadyListing(g, 1);
        ReadyListing(g, 2);
        g.Do(Command.PauseChipListing(g.Id, 1, true));
        long cash = g.Me.Cash;
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash - 300, g.Me.Cash);
    }

    [Fact] public void Q177_bankrupt_still_pauses_listing()
    {
        Game g = Solo();
        ReadyListing(g, 1);
        g.Do(Command.PauseChipListing(g.Id, 1, true));
        g.Me.Bankrupt = true;
        Assert.True(g.Me.ChipListings[0].Paused);
    }

    [Fact] public void Q177_save_load_pause_flag()
    {
        Game g = Solo();
        ReadyListing(g, 1);
        g.Do(Command.PauseChipListing(g.Id, 1, true));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.True(loaded.Player.Listing(1).Paused);
    }

    [Fact] public void Q177_cross_pause_blocks_rival_chip_purchase()
    {
        Game g = Play(c => c.Chips.ListingWeeklyUpkeep = 400);
        ReadyListing(g, 1);
        g.Do(Command.PauseChipListing(g.Id, 1, true));
        LabState buyer = g.Rival("anthoptics");
        buyer.Cash = 2_000_000;
        g.Refused(Command.BuyListedChips(buyer.Id, g.Id, 1, HardwareBatch.Hq, 1), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        ChipListingSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash, g.Me.Cash);
    }

    // Q178 AdvocateVendor
    [Fact] public void Q178_main_advocate_raises_standing()
    {
        Game g = Solo();
        int before = VendorSystem.StandingOf(g.State, g.Me);
        g.Do(Command.AdvocateVendor(g.Id));
        Assert.True(VendorSystem.StandingOf(g.State, g.Me) > before);
    }

    [Fact] public void Q178_zero_advocate_disabled()
    {
        Game g = Solo(c => c.Vendor.AdvocateCashCost = 0);
        g.Refused(Command.AdvocateVendor(g.Id), CommandError.InvalidArgument);
    }

    [Fact] public void Q178_max_standing_hits_cap()
    {
        Game g = Play();
        VendorStanding s = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        s.Standing = g.C.Vendor.StandingMax - 5;
        g.Do(Command.AdvocateVendor(g.Id));
        Assert.Equal(g.C.Vendor.StandingMax, s.Standing);
    }

    [Fact] public void Q178_bankrupt_refuses() { Game g = Solo(); g.Me.Bankrupt = true; g.Refused(Command.AdvocateVendor(g.Id), CommandError.LabBankrupt); }

    [Fact] public void Q178_save_load_cooldown()
    {
        Game g = Solo();
        g.Do(Command.AdvocateVendor(g.Id));
        int cd = g.Me.VendorAdvocateCooldownUntilTick;
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(cd, loaded.Player.VendorAdvocateCooldownUntilTick);
    }

    [Fact] public void Q178_cross_advocate_improves_vendor_gpu_quote()
    {
        Game g = Solo(c =>
        {
            c.Vendor.HighStandingAbove = 400;
            c.Vendor.HighStandingDiscountPermille = 150;
        });
        VendorStanding s = VendorSystem.EnsureStanding(g.C, g.State, g.Id);
        s.Standing = 380;
        int before = VendorSystem.PricePermille(g.C, g.State, g.Me);
        g.Do(Command.AdvocateVendor(g.Id));
        int after = VendorSystem.PricePermille(g.C, g.State, g.Me);
        Assert.True(after < before);
    }
}