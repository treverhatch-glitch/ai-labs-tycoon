using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q179–Q182 programs with weekly consequences.</summary>
public class PlayerProgramsQ179Q182Tests
{
    static Game Solo(System.Action<GameConfig> tweak = null) => Game.Solo(tweak: tweak).WithStarterLab(gpus: 16);
    static Game Play(System.Action<GameConfig> tweak = null) => Game.New(tweak: tweak).WithStarterLab(gpus: 16);

    static ModelState Released(Game g)
    {
        ModelState m = g.Train("mini", 400);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        return m;
    }

    // Q179
    [Fact] public void Q179_main_plan_upkeep_and_incident_cut() {
        Game g = Solo(c => c.Safety.IncidentPlanWeeklyUpkeep = 500);
        Released(g);
        g.Do(Command.AdoptIncidentPlan(g.Id));
        Assert.True(IncidentPlanSystem.Active(g.Me, g.Tick));
        long cash = g.Me.Cash;
        IncidentPlanSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash - 500, g.Me.Cash);
        Assert.Equal(250, IncidentPlanSystem.IncidentCutPermille(g.C, g.Me, g.Tick));
    }
    [Fact] public void Q179_zero_upkeep_disabled() {
        Game g = Solo(c => c.Safety.IncidentPlanWeeklyUpkeep = 0);
        g.Do(Command.AdoptIncidentPlan(g.Id));
        IncidentPlanSystem.RunWeek(g.C, g.State, g.Me);
    }
    [Fact] public void Q179_max_long_plan_weeks() {
        Game g = Solo(c => c.Safety.IncidentPlanWeeks = 48);
        g.Do(Command.AdoptIncidentPlan(g.Id));
        Assert.True(g.Me.IncidentPlanUntilTick >= g.Tick + 48);
    }
    [Fact] public void Q179_bankrupt_skips_upkeep() {
        Game g = Solo();
        g.Do(Command.AdoptIncidentPlan(g.Id));
        g.Me.Bankrupt = true;
        IncidentPlanSystem.RunWeek(g.C, g.State, g.Me);
    }
    [Fact] public void Q179_save_load_plan_tick() {
        Game g = Solo();
        g.Do(Command.AdoptIncidentPlan(g.Id));
        int until = g.Me.IncidentPlanUntilTick;
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(until, loaded.Player.IncidentPlanUntilTick);
    }
    [Fact] public void Q179_cross_plan_cuts_safety_incident_roll() {
        Game g = Solo(c => c.Safety.IncidentChancePermille = 800);
        ModelState m = g.Train("mini", 80);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        int bare = SafetySystem.IncidentChancePermille(g.C, m, g.Me);
        Assert.True(bare > 0);
        g.Do(Command.AdoptIncidentPlan(g.Id));
        int cut = IncidentPlanSystem.IncidentCutPermille(g.C, g.Me, g.State.Tick);
        Assert.True(cut > 0);
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        int afterWeek = (int)MathI.MulPermille(bare, MathI.Permille - cut);
        Assert.True(afterWeek < bare);
        Assert.True(IncidentPlanSystem.Active(g.Me, g.State.Tick));
    }

    // Q180
    [Fact] public void Q180_main_weekly_loan_payment() {
        Game g = Solo();
        g.Do(Command.TakeResearchLoan(g.Id));
        Assert.Equal(g.C.Research.LoanPrincipal, g.Me.ResearchLoanBalance);
        long cash = g.Me.Cash;
        ResearchLoanSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.ResearchLoanBalance < g.C.Research.LoanPrincipal);
        Assert.True(g.Me.Cash < cash);
    }
    [Fact] public void Q180_zero_payment_disabled() {
        Game g = Solo(c => c.Research.LoanWeeklyPayment = 0);
        g.Do(Command.TakeResearchLoan(g.Id));
        ResearchLoanSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Research.LoanPrincipal, g.Me.ResearchLoanBalance);
    }
    [Fact] public void Q180_max_large_loan_balance() {
        Game g = Solo(c => c.Research.LoanPrincipal = 1_000_000);
        g.Do(Command.TakeResearchLoan(g.Id));
        Assert.Equal(1_000_000, g.Me.ResearchLoanBalance);
    }
    [Fact] public void Q180_bankrupt_skips_payment() {
        Game g = Solo();
        g.Do(Command.TakeResearchLoan(g.Id));
        g.Me.Bankrupt = true;
        ResearchLoanSystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.ResearchLoanBalance > 0);
    }
    [Fact] public void Q180_save_load_loan_balance() {
        Game g = Solo();
        g.Do(Command.TakeResearchLoan(g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.ResearchLoanBalance, loaded.Player.ResearchLoanBalance);
    }
    [Fact] public void Q180_cross_loan_repayment_while_q171_trims_peer_gpus() {
        Game g = Play(c => c.Research.LoanWeeklyPayment = 500);
        g.Do(Command.TakeResearchLoan(g.Id));
        PeerDebt(g, out _);
        long loanBefore = g.Me.ResearchLoanBalance;
        int gpus = PeerComputeSystem.LeasedInGpus(g.Me);
        long cashBefore = g.Me.Cash;
        ResearchLoanSystem.RunWeek(g.C, g.State, g.Me);
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Finance.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.ResearchLoanBalance < loanBefore);
        Assert.True(PeerComputeSystem.LeasedInGpus(g.Me) < gpus);
        Assert.True(g.Me.Cash < cashBefore);
        g.Me.Cash = -1;
        g.Me.WeeksInDebt = 0;
        Finance.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.WeeksInDebt);
    }

    // Q181
    static void PeerDebt(Game g, out LabState landlord)
    {
        landlord = g.Rival("openaye");
        landlord.Fleet.Add(new HardwareBatch { Id = g.State.NextId++, HoldingId = HardwareBatch.Hq, GpuGenId = LabQueries.CurrentGeneration(g.C, g.Tick).Id, Count = 8 });
        g.Do(Command.RentPeerCompute(g.Id, landlord.Id, 4));
        g.Me.WeeksInDebt = 1;
    }

    [Fact] public void Q181_main_forbearance_blocks_trim() {
        Game g = Play();
        PeerDebt(g, out LabState landlord);
        g.Do(Command.GrantPeerForbearance(landlord.Id, g.Id));
        int gpus = PeerComputeSystem.LeasedInGpus(g.Me);
        PeerLeaseEnforcementSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(gpus, PeerComputeSystem.LeasedInGpus(g.Me));
    }
    [Fact] public void Q181_zero_subsidy_still_shields() {
        Game g = Play(c => c.Compute.PeerForbearanceSubsidyPerGpuWeek = 0);
        PeerDebt(g, out LabState landlord);
        g.Do(Command.GrantPeerForbearance(landlord.Id, g.Id));
        PeerForbearanceSystem.RunWeek(g.C, g.State);
    }
    [Fact] public void Q181_max_long_forbearance_weeks() {
        Game g = Play(c => c.Compute.PeerForbearanceWeeks = 40);
        PeerDebt(g, out LabState landlord);
        g.Do(Command.GrantPeerForbearance(landlord.Id, g.Id));
        Assert.True(landlord.PeerForbearances[0].UntilTick >= g.Tick + 40);
    }
    [Fact] public void Q181_bankrupt_landlord_skips_subsidy() {
        Game g = Play();
        PeerDebt(g, out LabState landlord);
        g.Do(Command.GrantPeerForbearance(landlord.Id, g.Id));
        landlord.Bankrupt = true;
        PeerForbearanceSystem.RunWeek(g.C, g.State);
    }
    [Fact] public void Q181_save_load_forbearance_list() {
        Game g = Play();
        LabState landlord = g.Rival("openaye");
        g.Me.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = landlord.Id, GpuCount = 2 });
        g.Do(Command.GrantPeerForbearance(landlord.Id, g.Id));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Single(loaded.State.Lab(landlord.Id).PeerForbearances);
    }
    [Fact] public void Q181_cross_subsidy_moves_cash() {
        Game g = Play(c => c.Compute.PeerForbearanceSubsidyPerGpuWeek = 500);
        PeerDebt(g, out LabState landlord);
        long lc = landlord.Cash;
        long rc = g.Me.Cash;
        g.Do(Command.GrantPeerForbearance(landlord.Id, g.Id));
        PeerForbearanceSystem.RunWeek(g.C, g.State);
        Assert.True(landlord.Cash < lc);
        Assert.True(g.Me.Cash > rc);
    }

    // Q182
    [Fact] public void Q182_main_pledge_upkeep_and_locks_reprice() {
        Game g = Solo(c => c.Market.SectorPricePledgeWeeklyUpkeep = 400);
        Released(g);
        g.Do(Command.PledgeSectorPrice(g.Id, "game-ai", 700));
        long cash = g.Me.Cash;
        SectorPricePledgeSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(cash - 400, g.Me.Cash);
        g.Refused(Command.SetSectorPrice(g.Id, "game-ai", 800), CommandError.InvalidArgument);
    }
    [Fact] public void Q182_zero_upkeep_still_pledges() {
        Game g = Solo(c => c.Market.SectorPricePledgeWeeklyUpkeep = 0);
        Released(g);
        g.Do(Command.PledgeSectorPrice(g.Id, "game-ai", 700));
    }
    [Fact] public void Q182_max_high_price_pledge() {
        Game g = Solo();
        Released(g);
        g.Do(Command.PledgeSectorPrice(g.Id, "game-ai", g.C.Market.SectorPriceMaxPermille));
    }
    [Fact] public void Q182_bankrupt_skips_upkeep() {
        Game g = Solo();
        Released(g);
        g.Do(Command.PledgeSectorPrice(g.Id, "game-ai", 700));
        g.Me.Bankrupt = true;
        SectorPricePledgeSystem.RunWeek(g.C, g.State, g.Me);
    }
    [Fact] public void Q182_save_load_pledge_fields() {
        Game g = Solo();
        Released(g);
        g.Do(Command.PledgeSectorPrice(g.Id, "game-ai", 700));
        var entry = g.State.Sector("game-ai").Entry(g.Id);
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        var e2 = loaded.State.Sector("game-ai").Entry(g.Id);
        Assert.Equal(entry.PricePledgeUntilTick, e2.PricePledgeUntilTick);
    }
    [Fact] public void Q182_cross_pledge_price_shapes_market_revenue() {
        Game g = Play();
        ModelState mine = Released(g);
        LabState rival = g.Rival("anthoptics");
        ModelState theirs = rival.Model(g.Try(Command.StartTraining(rival.Id, "mini", 800)).CreatedId);
        g.UntilTrained(theirs);
        g.Do(Command.ReleaseModel(rival.Id, theirs.Id, "game-ai"));
        g.Do(Command.PledgeSectorPrice(g.Id, "game-ai", g.C.Market.SectorPriceMinPermille));
        var entryPlayer = g.State.Sector("game-ai").Entry(g.Id);
        var entryRival = g.State.Sector("game-ai").Entry(rival.Id);
        entryPlayer.SharePpm = 500_000;
        entryRival.SharePpm = 500_000;
        entryRival.PricePermille = MathI.Permille;
        MarketSystem.RunWeek(g.C, g.State);
        Assert.Equal(g.C.Market.SectorPriceMinPermille, entryPlayer.PricePermille);
        Assert.True(entryRival.LastRevenue > entryPlayer.LastRevenue);
    }
}