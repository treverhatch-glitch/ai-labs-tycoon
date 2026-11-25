using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class NotesTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(8, c =>
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
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Cash = 1_000_000_000;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        g.Me.Reputation = 600;
        return g;
    }

    static void Raise(Game g)
    {
        FundingSystem.ApplyRaise(g.C, g.State, g.Me, g.C.Funding.Round("angel"));
    }

    [Fact]
    public void A_note_pays_cash_now_and_records_the_terms_of_its_tier()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.IssueNote(g.Id, 2, 1000));
        Assert.Equal(cash + 1_000_000, g.Me.Cash);
        Assert.Equal(1_000_000, g.Me.NoteOwed);
        Assert.Equal(1_000_000, g.Me.NotePrincipal);
        Assert.Equal(g.C.ConvertibleNotes.RateB, g.Me.NoteRate);
        Assert.Equal(g.C.ConvertibleNotes.DiscountB, g.Me.NoteDiscount);
        Assert.Equal(g.C.ConvertibleNotes.WeeksB, g.Me.NoteLeft);
        Assert.Equal(2, g.State.Events.Last(e => e.Kind == EventKind.NoteIssued).SubjectId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void One_note_at_a_time_with_a_tier_and_a_sensible_amount()
    {
        Game g = Lab();
        g.Refused(Command.IssueNote(g.Id, 0, 100), CommandError.InvalidArgument);
        g.Refused(Command.IssueNote(g.Id, 4, 100), CommandError.InvalidArgument);
        g.Refused(Command.IssueNote(g.Id, 1, 0), CommandError.InvalidArgument);
        g.Refused(Command.IssueNote(g.Id, 1, g.C.ConvertibleNotes.MaxThousands + 1), CommandError.InvalidArgument);
        g.Refused(Command.RepayNote(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.ExtendNote(g.Id), CommandError.InvalidArgument);
        g.Do(Command.IssueNote(g.Id, 1, 100));
        g.Refused(Command.IssueNote(g.Id, 1, 100), CommandError.InvalidArgument);
        Game off = Lab(c => c.ConvertibleNotes.Enabled = 0);
        off.Refused(Command.IssueNote(off.Id, 1, 100), CommandError.InvalidArgument);
    }

    [Fact]
    public void Interest_compounds_each_week_and_the_term_counts_down()
    {
        Game g = Lab();
        g.Do(Command.IssueNote(g.Id, 3, 1000));
        long owed = g.Me.NoteOwed;
        Notes.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(owed + MathI.MulPermille(owed, g.C.ConvertibleNotes.RateC), g.Me.NoteOwed);
        Assert.Equal(g.C.ConvertibleNotes.WeeksC - 1, g.Me.NoteLeft);
        Assert.Equal(1_000_000, g.Me.NotePrincipal);
    }

    [Fact]
    public void Repaying_pays_the_principal_and_the_interest_and_ends_the_note()
    {
        Game g = Lab();
        g.Do(Command.IssueNote(g.Id, 1, 500));
        for (int i = 0; i < 10; i++) Notes.RunWeek(g.C, g.State, g.Me);
        long owed = g.Me.NoteOwed;
        Assert.True(owed > 500_000);
        long cash = g.Me.Cash;
        g.Me.Cash = owed - 1;
        Assert.False(g.Try(Command.RepayNote(g.Id)).Ok);
        g.Me.Cash = cash;
        g.Do(Command.RepayNote(g.Id));
        Assert.Equal(cash - owed, g.Me.Cash);
        Assert.False(Notes.Owing(g.Me));
        Assert.Equal(owed, g.State.Events.Last(e => e.Kind == EventKind.NoteRepaid).Amount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Do(Command.IssueNote(g.Id, 2, 100));
    }

    [Fact]
    public void A_funding_round_converts_the_note_at_a_discount_and_dilutes_the_founders_more()
    {
        Game g = Lab();
        Game control = Lab();
        int start = g.Me.FounderSharePermille;
        g.Do(Command.IssueNote(g.Id, 2, 200));
        long owed = g.Me.NoteOwed;
        Raise(g);
        Raise(control);
        Assert.False(Notes.Owing(g.Me));
        Assert.True(g.Me.FounderSharePermille < control.Me.FounderSharePermille);
        GameEvent converted = g.State.Events.Last(e => e.Kind == EventKind.NoteConverted);
        Assert.Equal(0, converted.Pick);
        Assert.Equal(owed, converted.Amount);
        FundingRoundConfig round = g.C.Funding.Round("angel");
        long cash = Charter.FundingCash(g.C, g.Me, FundingSystem.EffectiveCash(g.C, g.State, round));
        int dilution = FundingSystem.EffectiveDilution(g.C, g.State, round);
        int share = Notes.Share(g.C, owed, cash * 1000 / dilution, g.C.ConvertibleNotes.DiscountB);
        Assert.Equal(share, converted.SubjectId);
        int total = dilution + (int)MathI.MulPermille(1000 - dilution, share);
        Assert.Equal((int)MathI.MulPermille(start, 1000 - total), g.Me.FounderSharePermille);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_deeper_discount_converts_into_more_of_the_lab()
    {
        Game a = Lab();
        Game c = Lab();
        a.Do(Command.IssueNote(a.Id, 1, 200));
        c.Do(Command.IssueNote(c.Id, 3, 200));
        Raise(a);
        Raise(c);
        Assert.True(c.State.Events.Last(e => e.Kind == EventKind.NoteConverted).SubjectId > a.State.Events.Last(e => e.Kind == EventKind.NoteConverted).SubjectId);
        Assert.True(c.Me.FounderSharePermille < a.Me.FounderSharePermille);
    }

    [Fact]
    public void An_extension_adds_time_at_a_higher_rate_and_a_deeper_discount_once()
    {
        Game g = Lab();
        g.Do(Command.IssueNote(g.Id, 1, 100));
        int rate = g.Me.NoteRate;
        int discount = g.Me.NoteDiscount;
        int left = g.Me.NoteLeft;
        g.Do(Command.ExtendNote(g.Id));
        Assert.Equal(left + g.C.ConvertibleNotes.ExtendWeeks, g.Me.NoteLeft);
        Assert.Equal((int)MathI.MulPermille(rate, g.C.ConvertibleNotes.ExtendRate), g.Me.NoteRate);
        Assert.Equal(discount + g.C.ConvertibleNotes.ExtendDiscount, g.Me.NoteDiscount);
        g.Refused(Command.ExtendNote(g.Id), CommandError.InvalidArgument);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.NoteExtended));
    }

    [Fact]
    public void A_note_that_reaches_maturity_converts_on_the_maturity_terms()
    {
        Game g = Lab(c => c.ConvertibleNotes.WeeksA = 3);
        int founder = g.Me.FounderSharePermille;
        g.Do(Command.IssueNote(g.Id, 1, 1000));
        for (int i = 0; i < 3; i++) Notes.RunWeek(g.C, g.State, g.Me);
        Assert.False(Notes.Owing(g.Me));
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.NoteConverted);
        Assert.Equal(2, e.Pick);
        Assert.True(e.SubjectId > 0);
        Assert.True(e.SubjectId <= g.C.ConvertibleNotes.MaxConvertPermille);
        Assert.Equal((int)MathI.MulPermille(founder, 1000 - e.SubjectId), g.Me.FounderSharePermille);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_note_converts_when_the_lab_lists_and_never_for_more_than_the_cap()
    {
        Game g = Lab();
        g.Do(Command.IssueNote(g.Id, 2, 1000));
        long owed = g.Me.NoteOwed;
        g.Do(Command.ListCompany(g.Id));
        Assert.False(Notes.Owing(g.Me));
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.NoteConverted);
        Assert.Equal(1, e.Pick);
        Assert.Equal(owed, e.Amount);
        Assert.Equal(Notes.Share(g.C, owed, Listing.Value(g.C, g.Me), g.C.ConvertibleNotes.DiscountB), e.SubjectId);
        Game big = Lab(c => c.ConvertibleNotes.MaxThousands = 1_000_000);
        big.Do(Command.IssueNote(big.Id, 3, 1_000_000));
        big.Do(Command.ListCompany(big.Id));
        Assert.Equal(big.C.ConvertibleNotes.MaxConvertPermille, big.State.Events.Last(x => x.Kind == EventKind.NoteConverted).SubjectId);
    }

    [Fact]
    public void A_live_note_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.IssueNote(g.Id, 2, 800));
        g.Weeks(30);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(25);
        g.Weeks(25);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 93, g.Sim.Save());
        Assert.DoesNotContain("noteOwed", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).NoteOwed);
    }

    [Fact]
    public void A_rival_note_and_bad_data_are_caught_and_the_data_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.NoteOwed = 5;
        rival.NotePrincipal = 5;
        rival.NoteLeft = 5;
        rival.NoteTier = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("convertible note"));
        rival.NoteOwed = 0;
        rival.NotePrincipal = 0;
        rival.NoteLeft = 0;
        rival.NoteTier = 0;
        g.Me.NoteRate = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("convertible note"));
        g.Me.NoteRate = 0;
        g.Me.NoteOwed = 9;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("convertible note"));
        g.Me.NoteOwed = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.ConvertibleNotes.Enabled = 2;
        c.ConvertibleNotes.MaxThousands = 0;
        c.ConvertibleNotes.RateA = 50;
        c.ConvertibleNotes.DiscountC = 5;
        c.ConvertibleNotes.WeeksB = 0;
        c.ConvertibleNotes.ExtendRate = 500;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxThousands", "rateC", "discountC", "weeksA", "maxConvertPermille" })
            Assert.Contains(problems, p => p.Contains("convertibleNotes." + key));
    }
}
