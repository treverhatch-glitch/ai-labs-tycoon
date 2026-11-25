using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SecurityTests
{
    static Game Lab(bool released = true)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        if (released)
        {
            ModelState m = g.Train("mini", 300);
            g.UntilTrained(m);
            g.Release(m, "game-ai");
        }
        g.Me.Exposure = 0;
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void Breach(Game g)
    {
        g.Me.Exposure = g.C.Security.Threshold - 1;
        g.C.Security.ReliefPerTier = 0;
        g.Weeks(1);
        Assert.True(Security.Open(g.Me));
    }

    [Fact]
    public void Each_model_on_sale_adds_exposure_and_each_tier_takes_some_off_but_never_below_nothing()
    {
        Game quiet = Lab(released: false);
        Assert.Equal(0, Security.WeeklyExposure(quiet.C, quiet.Me));
        quiet.Weeks(10);
        Assert.Equal(0, quiet.Me.Exposure);
        Game g = Lab();
        Assert.Equal(g.C.Security.ExposurePerModel, Security.WeeklyExposure(g.C, g.Me));
        g.Me.Exposure = 0;
        g.Weeks(4);
        Assert.Equal(4 * g.C.Security.ExposurePerModel, g.Me.Exposure);
        g.Me.SecurityTier = 1;
        Assert.Equal(System.Math.Max(0, g.C.Security.ExposurePerModel - g.C.Security.ReliefPerTier), Security.WeeklyExposure(g.C, g.Me));
        g.Me.SecurityTier = g.C.Security.MaxTier;
        Assert.Equal(0, Security.WeeklyExposure(g.C, g.Me));
    }

    [Fact]
    public void A_tier_is_paid_now_built_over_weeks_and_costs_upkeep_once_it_stands()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.BuildSecurity(g.Id));
        Assert.Equal(1, g.Me.SecurityBuilding);
        Assert.Equal(cash - Security.TierPrice(g.C, 1), g.Me.Cash);
        g.Refused(Command.BuildSecurity(g.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.Security.BuildWeeks);
        Assert.Equal(1, g.Me.SecurityTier);
        Assert.Equal(0, g.Me.SecurityBuilding);
        Assert.Equal(1, Count(g, EventKind.SecurityBuilt));
        long before = g.Me.Ledger.Lifetime.Insurance;
        g.Weeks(1);
        Assert.Equal(before - g.C.Security.UpkeepPerTier, g.Me.Ledger.Lifetime.Insurance);
    }

    [Fact]
    public void Tiers_stop_at_the_top_and_a_tier_costs_its_number_times_the_base()
    {
        Game g = Lab();
        Assert.Equal(g.C.Security.TierCost * 3, Security.TierPrice(g.C, 3));
        g.Me.SecurityTier = g.C.Security.MaxTier;
        g.Refused(Command.BuildSecurity(g.Id), CommandError.InvalidArgument);
        g.Me.SecurityTier = 0;
        g.Me.Cash = 10;
        Assert.Equal(CommandError.NotEnoughCash, g.Try(Command.BuildSecurity(g.Id)).Error);
    }

    [Fact]
    public void A_breach_opens_when_the_exposure_is_full_and_leaks_if_nobody_answers()
    {
        Game g = Lab();
        Breach(g);
        Assert.Equal(1, Count(g, EventKind.BreachOpened));
        int rep = g.Me.Reputation;
        g.Weeks(g.C.Security.AnswerWeeks);
        Assert.False(Security.Open(g.Me));
        Assert.Equal(0, g.Me.Exposure);
        Assert.Equal(1, Count(g, EventKind.BreachLeaked));
        Assert.True(g.Me.Reputation < rep);
        LawCase claim = g.Me.Cases.Single();
        Assert.Equal(Litigation.Privacy, claim.Kind);
    }

    [Fact]
    public void Paying_the_ransom_clears_it_quietly_but_the_next_one_costs_more_and_a_disclosure_resets_the_price()
    {
        Game g = Lab();
        Breach(g);
        long first = Security.RansomCost(g.C, g.Me);
        long fines = g.Me.Ledger.Lifetime.Fines;
        int rep = g.Me.Reputation;
        g.Do(Command.AnswerBreach(g.Id, Security.Ransom));
        Assert.Equal(fines - first, g.Me.Ledger.Lifetime.Fines);
        Assert.Equal(1, g.Me.Ransoms);
        Assert.True(Security.RansomCost(g.C, g.Me) > first);
        Assert.Empty(g.Me.Cases);
        Assert.Equal(rep, g.Me.Reputation);
        Assert.False(Security.Open(g.Me));
        Breach(g);
        g.Do(Command.AnswerBreach(g.Id, Security.Disclose));
        Assert.Equal(0, g.Me.Ransoms);
        Assert.Equal(first, Security.RansomCost(g.C, g.Me));
        Assert.True(g.Me.Reputation < rep);
        Assert.Single(g.Me.Cases);
    }

    [Fact]
    public void A_disclosed_breach_draws_a_weaker_claim_than_a_leaked_one()
    {
        Game told = Lab();
        Breach(told);
        told.Do(Command.AnswerBreach(told.Id, Security.Disclose));
        Game leaked = Lab();
        Breach(leaked);
        leaked.Weeks(leaked.C.Security.AnswerWeeks);
        Assert.True(told.Me.Cases.Single().Strength < leaked.Me.Cases.Single().Strength);
    }

    [Fact]
    public void A_lockdown_buys_the_next_tier_at_once_at_double_the_price_and_is_refused_at_the_top_or_mid_build()
    {
        Game g = Lab();
        Breach(g);
        long price = Security.LockdownCost(g.C, g.Me);
        Assert.Equal(Security.TierPrice(g.C, 1) * 2, price);
        long insurance = g.Me.Ledger.Lifetime.Insurance;
        g.Do(Command.AnswerBreach(g.Id, Security.Lockdown));
        Assert.Equal(1, g.Me.SecurityTier);
        Assert.Equal(insurance - price, g.Me.Ledger.Lifetime.Insurance);
        Assert.Empty(g.Me.Cases);
        Assert.Equal(0, g.Me.Exposure);
        Breach(g);
        g.Me.SecurityBuilding = 2;
        g.Me.SecurityDue = g.State.Tick + 5;
        g.Refused(Command.AnswerBreach(g.Id, Security.Lockdown), CommandError.InvalidArgument);
        g.Me.SecurityBuilding = 0;
        g.Me.SecurityDue = 0;
        g.Me.SecurityTier = g.C.Security.MaxTier;
        g.Refused(Command.AnswerBreach(g.Id, Security.Lockdown), CommandError.InvalidArgument);
        g.Refused(Command.AnswerBreach(g.Id, 3), CommandError.InvalidArgument);
    }

    [Fact]
    public void Enough_tiers_keep_the_exposure_from_ever_filling()
    {
        Game g = Lab();
        g.Me.SecurityTier = 1;
        g.C.Security.ReliefPerTier = g.C.Security.ExposurePerModel;
        g.Me.Exposure = 0;
        g.Weeks(g.C.Security.Threshold);
        Assert.Equal(0, Count(g, EventKind.BreachOpened));
        Assert.Equal(0, g.Me.Exposure);
    }

    [Fact]
    public void Nothing_happens_when_it_is_switched_off_or_asked_of_the_wrong_lab_and_a_breach_in_flight_saves_and_loads()
    {
        Game g = Lab();
        Breach(g);
        g.Me.Ransoms = 2;
        g.Me.SecurityTier = 1;
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 81, g.Sim.Save());
        Assert.DoesNotContain("securityTier", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.SecurityTier);
        Assert.Equal(0, lab.BreachDue);
        Game off = Lab();
        off.C.Security.Enabled = 0;
        off.Weeks(40);
        Assert.Equal(0, off.Me.Exposure);
        off.Refused(Command.BuildSecurity(off.Id), CommandError.InvalidArgument);
        off.Refused(Command.AnswerBreach(off.Id, 0), CommandError.InvalidArgument);
    }
}
