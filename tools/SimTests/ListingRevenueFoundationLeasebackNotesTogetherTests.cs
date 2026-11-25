using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ListingRevenueFoundationLeasebackNotesTogetherTests
{
    static Game Lab(Action<GameConfig> more = null, int gpus = 0)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Tax.Enabled = 0;
            c.PlayerStart.GrantPerWeek = 400_000;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        if (gpus > 0) Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, gpus);
        g.Me.Reputation = 600;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void A_note_that_converts_at_the_listing_leaves_nothing_for_a_later_round()
    {
        Game g = Lab();
        int start = g.Me.FounderSharePermille;
        g.Do(Command.IssueNote(g.Id, 2, 200));
        g.Do(Command.ListCompany(g.Id));
        Assert.Equal(1, Count(g, EventKind.NoteConverted));
        int afterListing = g.Me.FounderSharePermille;
        Assert.True(afterListing < (int)MathI.MulPermille(start, 1000 - g.C.Listing.FloatPermille));
        FundingSystem.ApplyRaise(g.C, g.State, g.Me, g.C.Funding.Round("angel"));
        Assert.Equal(1, Count(g, EventKind.NoteConverted));
        Assert.Equal((int)MathI.MulPermille(afterListing, 1000 - FundingSystem.EffectiveDilution(g.C, g.State, g.C.Funding.Round("angel"))), g.Me.FounderSharePermille);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_revenue_advance_counts_as_debt_in_the_loan_cover()
    {
        Game g = Lab(c => c.Covenants.CureWeeks = 100, gpus: 200);
        g.Do(Command.TakeLoan(g.Id, 1000));
        long cover = Covenants.Cover(g.C, g.State, g.Me);
        long cure = Covenants.CureAmount(g.C, g.State, g.Me);
        g.Do(Command.TakeRevenueAdvance(g.Id, 1, 1000));
        Assert.True(Covenants.Cover(g.C, g.State, g.Me) < cover);
        g.C.Covenants.MinCoverPermille = (int)cover - 1;
        Assert.True(Covenants.InBreach(g.C, g.State, g.Me));
        Assert.True(Covenants.CureAmount(g.C, g.State, g.Me) > cure);
        long pay = Covenants.CureAmount(g.C, g.State, g.Me);
        g.Do(Command.CureCovenant(g.Id));
        Assert.False(Covenants.InBreach(g.C, g.State, g.Me));
        Assert.True(pay > 0);
    }

    [Fact]
    public void Leasing_chips_back_cuts_the_collateral_and_can_breach_the_covenant()
    {
        Game g = Lab(c => c.Covenants.CureWeeks = 100, gpus: 200);
        g.Do(Command.TakeLoan(g.Id, 1000));
        long cover = Covenants.Cover(g.C, g.State, g.Me);
        g.C.Covenants.MinCoverPermille = (int)cover - 1;
        Assert.False(Covenants.InBreach(g.C, g.State, g.Me));
        g.Do(Command.SaleLeaseback(g.Id, 100));
        Assert.True(Covenants.Cover(g.C, g.State, g.Me) < cover);
        Assert.True(Covenants.InBreach(g.C, g.State, g.Me));
        g.Do(Command.BuyBackLease(g.Id));
        Assert.False(Covenants.InBreach(g.C, g.State, g.Me));
        g.Do(Command.SaleLeaseback(g.Id, 100));
        Covenants.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, Count(g, EventKind.CovenantBreached));
    }

    [Fact]
    public void A_safety_foundation_softens_the_hit_of_an_activist()
    {
        Game plain = Lab(c => c.Listing.ActivistBelow = 990);
        Game giving = Lab(c => c.Listing.ActivistBelow = 990);
        giving.Do(Command.EndowFoundation(giving.Id, 5000, Foundation.Safety));
        giving.Do(Command.SetFoundationPayout(giving.Id, 3));
        for (int i = 0; i < 100; i++) Foundation.RunWeek(giving.C, giving.State, giving.Me);
        Assert.True(Foundation.Damping(giving.C, giving.Me) > 0);
        foreach (Game g in new[] { plain, giving })
        {
            g.Do(Command.ListCompany(g.Id));
            g.Me.Reputation = 500;
            g.Me.ListNext = g.State.Tick + 1;
            g.Me.ListPrice = 500;
            Listing.RunWeek(g.C, g.State, g.Me);
            Assert.Equal(1, g.Me.ListActivist);
        }
        Assert.True(giving.Me.Reputation > plain.Me.Reputation);
    }

    [Fact]
    public void The_listing_the_gift_and_the_advance_stay_out_of_the_tax_base_but_the_costs_come_off_it()
    {
        Game g = Lab(c => c.Tax.Enabled = 1);
        Game control = Lab(c => c.Tax.Enabled = 1);
        g.Me.Ledger.LastWeek.CapitalIn = 7_000_000;
        Tax.RunWeek(g.C, g.State, g.Me);
        Tax.RunWeek(control.C, control.State, control.Me);
        Assert.Equal(control.Me.TaxAccrued, g.Me.TaxAccrued);
        Assert.True(control.Me.TaxAccrued > 0);
        g.Me.Ledger.LastWeek.Insurance = -20_000;
        g.Me.Ledger.LastWeek.GpuRent = -15_000;
        Tax.RunWeek(g.C, g.State, g.Me);
        Tax.RunWeek(control.C, control.State, control.Me);
        Assert.True(g.Me.TaxAccrued < control.Me.TaxAccrued);
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab(gpus: 30);
        g.Do(Command.IssueNote(g.Id, 2, 100));
        g.Do(Command.ListCompany(g.Id));
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        g.Do(Command.TakeRevenueAdvance(g.Id, 2, 300));
        g.Do(Command.EndowFoundation(g.Id, 500, Foundation.OpenScience));
        g.Do(Command.SetFoundationPayout(g.Id, 2));
        g.Do(Command.SaleLeaseback(g.Id, 10));
        g.Weeks(5);
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        Assert.True(idx >= 0);
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(20);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay(int seed)
    {
        Game g = Game.New(seed, tweak: c =>
        {
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Scaling.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
        });
        g.Autopilot(60);
        g.C.Listing.MinReputation = 0;
        g.C.Listing.MinWeeklyRevenue = 1;
        g.C.Rbf.MinWeeklyRevenue = 1;
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, 20);
        g.Me.Cash += 20_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int week = 0; week < 330 && !g.Me.Bankrupt; week++)
        {
            if (week % 40 == 5) g.Try(Command.ListCompany(g.Id));
            if (Listing.Listed(g.Me) && week % 3 == 0) g.Try(Command.SetGuidance(g.Id, 1 + week / 3 % 3));
            if (week % 60 == 30) g.Try(Command.FollowOn(g.Id));
            if (week % 55 == 9) g.Try(Command.BuyBackShares(g.Id, 50));
            if (week % 200 == 150) g.Try(Command.TakePrivate(g.Id));
            if (week % 70 == 10) g.Try(Command.TakeRevenueAdvance(g.Id, 1 + week / 70 % 3, 400));
            if (week % 70 == 60) g.Try(Command.BuyOutAdvance(g.Id));
            if (week % 90 == 12) g.Try(Command.EndowFoundation(g.Id, 500, 1 + week / 90 % 3));
            if (week % 30 == 14) g.Try(Command.SetFoundationPayout(g.Id, week / 30 % 4));
            if (week % 130 == 100) g.Try(Command.SetFoundationFocus(g.Id, 1 + week / 130 % 3));
            if (week % 260 == 240) g.Try(Command.WindUpFoundation(g.Id));
            if (week % 100 == 20) g.Try(Command.SaleLeaseback(g.Id, 5));
            if (week % 100 == 50) g.Try(Command.ExtendLease(g.Id));
            if (week % 100 == 70) g.Try(Command.BuyBackLease(g.Id));
            if (week % 300 == 280) g.Try(Command.ReturnLease(g.Id));
            if (week % 80 == 25) g.Try(Command.IssueNote(g.Id, 1 + week / 80 % 3, 200));
            if (week % 80 == 45) g.Try(Command.ExtendNote(g.Id));
            if (week % 80 == 70) g.Try(Command.RepayNote(g.Id));
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.True(Count(g, EventKind.CompanyListed) > 0, "the lab never listed");
        Assert.True(Count(g, EventKind.RevenueAdvanceTaken) > 0, "no revenue advance was ever taken");
        Assert.True(Count(g, EventKind.FoundationEndowed) > 0, "no foundation was ever endowed");
        Assert.True(Count(g, EventKind.GpusLeasedBack) > 0, "no GPUs were ever leased back");
        Assert.True(Count(g, EventKind.NoteIssued) > 0, "no note was ever issued");
    }
}
