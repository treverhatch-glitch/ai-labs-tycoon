using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ListingTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Reputation = 600;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        return g;
    }

    static Game Listed(System.Action<GameConfig> more = null)
    {
        Game g = Lab(more);
        g.Do(Command.ListCompany(g.Id));
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void Report(Game g, int level, long revenue)
    {
        g.Me.Ledger.LastWeek.Revenue = 0;
        g.Me.ListNext = g.State.Tick + 1;
        g.Me.ListGuidance = level;
        g.Me.ListTarget = level == 0 ? 0 : 1_000_000;
        g.Me.ListQuarter = revenue;
        Listing.RunWeek(g.C, g.State, g.Me);
    }

    [Fact]
    public void Listing_sells_a_slice_for_cash_less_a_fee_and_dilutes_the_founders()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        int founder = g.Me.FounderSharePermille;
        long gross = Listing.Proceeds(g.C, g.Me, g.C.Listing.FloatPermille);
        long fee = MathI.MulPermille(gross, g.C.Listing.ListingFeePermille);
        g.Do(Command.ListCompany(g.Id));
        Assert.True(Listing.Listed(g.Me));
        Assert.Equal(cash + gross - fee, g.Me.Cash);
        Assert.Equal(1000, g.Me.ListPrice);
        Assert.Equal(MathI.MulPermille(founder, 1000 - g.C.Listing.FloatPermille), g.Me.FounderSharePermille);
        Assert.Equal(1000 - g.C.Listing.FloatPermille, g.Me.ListKeep);
        Assert.Equal(g.State.Tick + g.C.Listing.QuarterWeeks, g.Me.ListNext);
        Assert.Equal(gross - fee, g.State.Events.Last(e => e.Kind == EventKind.CompanyListed).Amount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Only_a_lab_with_the_reputation_and_revenue_that_is_not_a_nonprofit_can_list()
    {
        Game g = Lab();
        g.Me.Reputation = g.C.Listing.MinReputation - 1;
        g.Refused(Command.ListCompany(g.Id), CommandError.MissingPrerequisite);
        g.Me.Reputation = 600;
        g.Me.Ledger.LastWeek.Revenue = g.C.Listing.MinWeeklyRevenue - 1;
        g.Refused(Command.ListCompany(g.Id), CommandError.MissingPrerequisite);
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        g.Me.Charter = Charter.Nonprofit;
        g.Refused(Command.ListCompany(g.Id), CommandError.InvalidArgument);
        g.Me.Charter = Charter.ForProfit;
        g.Do(Command.ListCompany(g.Id));
        g.Refused(Command.ListCompany(g.Id), CommandError.InvalidArgument);
        Game off = Lab(c => c.Listing.Enabled = 0);
        off.Refused(Command.ListCompany(off.Id), CommandError.InvalidArgument);
        Assert.False(Listing.Listed(off.Me));
    }

    [Fact]
    public void Guidance_is_set_once_in_the_first_weeks_of_a_quarter()
    {
        Game g = Listed();
        g.Refused(Command.SetGuidance(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SetGuidance(g.Id, 4), CommandError.InvalidArgument);
        g.Do(Command.SetGuidance(g.Id, 3));
        Assert.Equal(3, g.Me.ListGuidance);
        Assert.Equal(MathI.MulPermille(g.Me.ListBase * g.C.Listing.QuarterWeeks, g.C.Listing.Stretch), g.Me.ListTarget);
        g.Refused(Command.SetGuidance(g.Id, 1), CommandError.InvalidArgument);
        Game late = Listed();
        late.Sim.StepMany(late.C.Listing.GuidanceWindow + 1);
        late.Refused(Command.SetGuidance(late.Id, 2), CommandError.InvalidArgument);
        Game unlisted = Lab();
        unlisted.Refused(Command.SetGuidance(unlisted.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_quarter_that_beats_meets_or_misses_guidance_moves_the_price_by_the_level()
    {
        Game beat = Listed();
        int price = beat.Me.ListPrice;
        Report(beat, 3, 1_100_000);
        Assert.Equal(price + MathI.MulPermille(price, beat.C.Listing.BeatMove * 3), beat.Me.ListPrice);
        Assert.Equal(2, beat.State.Events.Last(e => e.Kind == EventKind.EarningsReported).Pick);
        Assert.Equal(1, beat.Me.ListStreak);
        Assert.Equal(0, beat.Me.ListGuidance);

        Game met = Listed();
        Report(met, 2, 1_000_000);
        Assert.Equal(1000 + MathI.MulPermille(1000, met.C.Listing.MeetMove), met.Me.ListPrice);
        Assert.Equal(1, met.State.Events.Last(e => e.Kind == EventKind.EarningsReported).Pick);

        Game miss = Listed();
        Report(miss, 3, 900_000);
        Assert.Equal(1000 - MathI.MulPermille(1000, miss.C.Listing.MissMove * 3), miss.Me.ListPrice);
        Assert.Equal(0, miss.State.Events.Last(e => e.Kind == EventKind.EarningsReported).Pick);
        Assert.Equal(0, miss.Me.ListStreak);
    }

    [Fact]
    public void A_quarter_with_no_guidance_costs_the_price_and_the_next_quarter_starts()
    {
        Game g = Listed();
        int next = g.Me.ListNext;
        Report(g, 0, 5_000_000);
        Assert.Equal(1000 - MathI.MulPermille(1000, g.C.Listing.NoGuidanceMove), g.Me.ListPrice);
        Assert.True(g.Me.ListNext > next);
        Assert.Equal(0, g.Me.ListQuarter);
        Assert.Equal(1, Count(g, EventKind.EarningsReported));
    }

    [Fact]
    public void Reporting_costs_every_week_once_listed_and_the_price_stays_within_its_bounds()
    {
        Game g = Listed(c => c.Listing.PriceCap = 1100);
        long before = g.Me.Ledger.Lifetime.Insurance;
        Listing.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - g.C.Listing.ReportingCost, g.Me.Ledger.Lifetime.Insurance);
        for (int i = 0; i < 5; i++) Report(g, 3, 9_000_000);
        Assert.Equal(1100, g.Me.ListPrice);
        for (int i = 0; i < 100; i++) Report(g, 0, 0);
        Assert.Equal(g.C.Listing.PriceFloor, g.Me.ListPrice);
    }

    [Fact]
    public void A_follow_on_sells_more_at_the_price_and_waits_before_the_next()
    {
        Game g = Listed();
        g.Me.ListPrice = 2000;
        g.Refused(Command.FollowOn(g.Id), CommandError.InvalidArgument);
        g.Sim.StepMany(g.C.Listing.FollowOnWeeks);
        long cash = g.Me.Cash;
        long gross = Listing.Proceeds(g.C, g.Me, g.C.Listing.FollowOnPermille);
        int founder = g.Me.FounderSharePermille;
        g.Do(Command.FollowOn(g.Id));
        Assert.Equal(cash + gross, g.Me.Cash);
        Assert.Equal(MathI.MulPermille(founder, 1000 - g.C.Listing.FollowOnPermille), g.Me.FounderSharePermille);
        Assert.Equal(1, Count(g, EventKind.FollowOnSold));
        g.Refused(Command.FollowOn(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_buyback_lifts_the_price_and_gives_the_founders_back_some_of_the_lab()
    {
        Game g = Listed();
        int price = g.Me.ListPrice;
        int founder = g.Me.FounderSharePermille;
        int keep = g.Me.ListKeep;
        long cash = g.Me.Cash;
        long value = Listing.Value(g.C, g.Me);
        g.Do(Command.BuyBackShares(g.Id, 100));
        Assert.Equal(cash - 100_000, g.Me.Cash);
        Assert.True(g.Me.ListPrice > price);
        Assert.True(g.Me.FounderSharePermille > founder);
        Assert.True(g.Me.ListKeep > keep);
        Assert.True(g.Me.ListKeep <= 1000);
        Assert.Equal(1, Count(g, EventKind.SharesBoughtBack));
        g.Me.Cash = 10;
        Assert.False(g.Try(Command.BuyBackShares(g.Id, 100)).Ok);
        Assert.True(value > 0);
        g.Refused(Command.BuyBackShares(g.Id, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_low_price_brings_an_activist_closer_and_a_good_quarter_sends_it_back()
    {
        Game g = Listed(c => c.Listing.ActivistBelow = 990);
        int rep = g.Me.Reputation;
        Report(g, 3, 0);
        Assert.Equal(1, g.Me.ListActivist);
        Assert.True(g.Me.Reputation < rep);
        Assert.Equal(1, Count(g, EventKind.ActivistArrived));
        for (int i = 0; i < 6; i++) Report(g, 3, 0);
        Assert.Equal(g.C.Listing.MaxActivist, g.Me.ListActivist);
        g.Me.ListPrice = 5000;
        Report(g, 2, 2_000_000);
        Assert.Equal(g.C.Listing.MaxActivist - 1, g.Me.ListActivist);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Taking_the_lab_private_buys_the_public_out_and_clears_the_listing()
    {
        Game g = Listed();
        int founder = g.Me.FounderSharePermille;
        long cost = Listing.TakePrivateCost(g.C, g.Me);
        long cash = g.Me.Cash;
        g.Do(Command.TakePrivate(g.Id));
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.False(Listing.Listed(g.Me));
        Assert.Equal(0, g.Me.ListPrice);
        Assert.Equal(0, g.Me.ListKeep);
        Assert.True(g.Me.FounderSharePermille > founder);
        Assert.Equal(1, Count(g, EventKind.CompanyDelisted));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Refused(Command.TakePrivate(g.Id), CommandError.InvalidArgument);
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        g.Do(Command.ListCompany(g.Id));
        Assert.True(Listing.Listed(g.Me));
    }

    [Fact]
    public void A_listing_saves_loads_and_plays_on_the_same_and_an_older_save_has_none()
    {
        Game g = Listed();
        g.Do(Command.SetGuidance(g.Id, 2));
        g.Weeks(30);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 89, g.Sim.Save());
        Assert.DoesNotContain("listPrice", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.Listed);
        Assert.Equal(0, lab.ListPrice);
    }

    [Fact]
    public void A_rival_that_is_listed_is_not_sound_and_bad_data_is_caught()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.Listed = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("is listed"));
        rival.Listed = 0;
        g.Me.ListPrice = 900;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("is listed"));
        g.Me.ListPrice = 0;
        g.Me.Listed = 1;
        g.Me.ListPrice = 1000;
        g.Me.ListKeep = 800;
        g.Me.ListNext = 5;
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Me.ListGuidance = 4;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("is listed"));
        g.Me.ListGuidance = 0;
        g.Me.Listed = 0;
        g.Me.ListPrice = 0;
        g.Me.ListKeep = 0;
        g.Me.ListNext = 0;
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Listing.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Listing.Enabled = 2;
        c.Listing.MinReputation = -1;
        c.Listing.FloatPermille = 0;
        c.Listing.QuarterWeeks = 3;
        c.Listing.Conservative = 50;
        c.Listing.BeatMargin = -1;
        c.Listing.PriceFloor = 0;
        c.Listing.FollowOnWeeks = -1;
        c.Listing.MaxActivist = 0;
        c.Listing.TakePrivatePermille = 999;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minReputation", "floatPermille", "quarterWeeks", "stretch", "beatMargin", "priceFloor", "followOnWeeks", "activistBelow", "takePrivatePermille" })
            Assert.Contains(problems, p => p.Contains("listing." + key));
    }
}
