using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class CharterTests
{
    static Game Lab(int form = Charter.ForProfit)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Security.Enabled = 0;
            c.Support.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Charter = form;
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void A_change_is_paid_for_now_reviewed_for_weeks_and_then_made()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.ConvertCharter(g.Id, Charter.Capped));
        Assert.Equal(cash - g.C.Charter.ConvertFee, g.Me.Cash);
        Assert.Equal(Charter.Capped + 1, g.Me.CharterTo);
        g.Weeks(g.C.Charter.ReviewWeeks - 1);
        Assert.Equal(Charter.ForProfit, g.Me.Charter);
        g.Weeks(1);
        Assert.Equal(Charter.Capped, g.Me.Charter);
        Assert.False(Charter.UnderReview(g.Me));
        GameEvent changed = g.State.Events.Last(e => e.Kind == EventKind.CharterChanged);
        Assert.Equal(Charter.Capped, changed.Pick);
        Assert.Equal(Charter.ForProfit, changed.Amount);
        Assert.Equal(0, g.Me.CapPaid);
    }

    [Fact]
    public void A_change_is_refused_for_the_same_form_a_bad_form_a_second_review_or_no_cash_and_can_be_dropped_without_a_refund()
    {
        Game g = Lab();
        g.Refused(Command.ConvertCharter(g.Id, Charter.ForProfit), CommandError.InvalidArgument);
        g.Refused(Command.ConvertCharter(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.CancelCharterChange(g.Id), CommandError.InvalidArgument);
        g.Do(Command.ConvertCharter(g.Id, Charter.Nonprofit));
        g.Refused(Command.ConvertCharter(g.Id, Charter.Capped), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        g.Do(Command.CancelCharterChange(g.Id));
        Assert.Equal(cash, g.Me.Cash);
        Assert.False(Charter.UnderReview(g.Me));
        Assert.Equal(Charter.Nonprofit, g.State.Events.Last(e => e.Kind == EventKind.CharterReviewDropped).Pick);
        g.Weeks(g.C.Charter.ReviewWeeks);
        Assert.Equal(Charter.ForProfit, g.Me.Charter);
        g.Me.Cash = 10;
        Assert.Equal(CommandError.NotEnoughCash, g.Try(Command.ConvertCharter(g.Id, Charter.Capped)).Error);
    }

    [Fact]
    public void A_nonprofit_cannot_raise_funding_and_a_capped_lab_raises_a_share()
    {
        FundingRoundConfig round = Lab().C.Funding.Rounds[0];
        Game forProfit = Lab();
        Game capped = Lab(Charter.Capped);
        Game nonprofit = Lab(Charter.Nonprofit);
        long a = FundingSystem.EffectiveCash(forProfit.C, forProfit.State, round);
        FundingSystem.ApplyRaise(forProfit.C, forProfit.State, forProfit.Me, round);
        FundingSystem.ApplyRaise(capped.C, capped.State, capped.Me, round);
        Assert.Equal(a, forProfit.State.Events.Last(e => e.Kind == EventKind.FundingClosed).Amount);
        Assert.Equal(MathI.MulPermille(a, capped.C.Charter.CappedFundingPermille), capped.State.Events.Last(e => e.Kind == EventKind.FundingClosed).Amount);
        nonprofit.Refused(Command.RaiseFunding(nonprofit.Id, round.Id), CommandError.InvalidArgument);
        Assert.Empty(nonprofit.Me.FundingRounds);
    }

    [Fact]
    public void Research_grants_are_scaled_by_the_charter_in_the_award_and_the_clawback()
    {
        Game fp = Lab();
        Game cp = Lab(Charter.Capped);
        Game np = Lab(Charter.Nonprofit);
        long baseAward = ResearchGrants.Award(fp.C, 2);
        Assert.Equal(MathI.MulPermille(baseAward, fp.C.Charter.ForProfitGrantPermille), Charter.GrantAward(fp.C, fp.Me, baseAward));
        Assert.True(Charter.GrantAward(cp.C, cp.Me, baseAward) > Charter.GrantAward(fp.C, fp.Me, baseAward));
        Assert.True(Charter.GrantAward(np.C, np.Me, baseAward) > Charter.GrantAward(cp.C, cp.Me, baseAward));
        fp.Me.ResearchGrantTier = 2;
        np.Me.ResearchGrantTier = 2;
        Assert.True(ResearchGrants.Clawback(np.C, np.Me) > ResearchGrants.Clawback(fp.C, fp.Me));
    }

    [Fact]
    public void A_nonprofit_gets_donations_from_its_prestige_every_week_and_nobody_else_does()
    {
        Game fp = Lab();
        Game np = Lab(Charter.Nonprofit);
        fp.Me.Prestige = 100;
        np.Me.Prestige = 100;
        Assert.Equal(100L * np.C.Charter.DonationPerPrestige, Charter.Donations(np.C, np.Me));
        Assert.Equal(0, Charter.Donations(fp.C, fp.Me));
        fp.Weeks(5);
        np.Weeks(5);
        Assert.True(np.Me.Ledger.Lifetime.Grants - fp.Me.Ledger.Lifetime.Grants >= 4 * Charter.Donations(np.C, np.Me));
    }

    [Fact]
    public void A_capped_lab_pays_investors_a_share_of_a_good_week_until_the_cap_and_then_stops()
    {
        Game g = Lab(Charter.Capped);
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        long net = g.Me.Ledger.LastWeek.Net;
        Assert.Equal(MathI.MulPermille(net, g.C.Charter.DistributionPermille), Charter.Distribution(g.C, g.Me));
        g.Me.CapPaid = g.C.Charter.CapAmount - 100;
        Assert.Equal(100, Charter.Distribution(g.C, g.Me));
        g.Me.CapPaid = g.C.Charter.CapAmount;
        Assert.Equal(0, Charter.Distribution(g.C, g.Me));
        g.Me.CapPaid = 0;
        g.Me.Ledger.LastWeek.Revenue = -100_000;
        Assert.Equal(0, Charter.Distribution(g.C, g.Me));
        Game fp = Lab();
        fp.Me.Ledger.LastWeek.Revenue = 100_000;
        Assert.Equal(0, Charter.Distribution(fp.C, fp.Me));
    }

    [Fact]
    public void A_capped_lab_that_earns_pays_out_through_play_and_the_books_still_balance()
    {
        Game g = Lab(Charter.Capped);
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Release(m, "game-ai");
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Weeks(60);
        Assert.True(g.Me.CapPaid >= 0 && g.Me.CapPaid <= g.C.Charter.CapAmount);
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Leaving_a_nonprofit_costs_morale_and_draws_a_challenge_and_becoming_one_lifts_morale()
    {
        Game g = Lab(Charter.Nonprofit);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        g.Do(Command.ConvertCharter(g.Id, Charter.ForProfit));
        g.Weeks(g.C.Charter.ReviewWeeks);
        Assert.Equal(Charter.ForProfit, g.Me.Charter);
        LawCase claim = g.Me.Cases.Single();
        Assert.Equal(Litigation.Charter, claim.Kind);
        Assert.Equal(g.C.Charter.ChallengeStrength, claim.Strength);
        Assert.Equal(g.C.Charter.ChallengeClaim, claim.Claim);
        Assert.All(g.Me.Staff, p => Assert.True(p.Morale < 700));
        Game h = Lab();
        foreach (StaffMember p in h.Me.Staff) p.Morale = 500;
        h.Do(Command.ConvertCharter(h.Id, Charter.Nonprofit));
        h.Weeks(h.C.Charter.ReviewWeeks);
        Assert.All(h.Me.Staff, p => Assert.True(p.Morale > 500));
        Assert.Empty(h.Me.Cases);
    }

    [Fact]
    public void A_review_in_flight_saves_loads_and_plays_on_the_same_and_old_saves_are_for_profit()
    {
        Game g = Lab(Charter.Capped);
        g.Me.CapPaid = 1234;
        g.Do(Command.ConvertCharter(g.Id, Charter.Nonprofit));
        g.Weeks(5);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(40);
        g.Weeks(40);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 82, g.Sim.Save());
        Assert.DoesNotContain("charterTo", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(Charter.ForProfit, lab.Charter);
        Assert.Equal(0, lab.CharterTo);
        Assert.Equal(0, lab.CapPaid);
    }
}
