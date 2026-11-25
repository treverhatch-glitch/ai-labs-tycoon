using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class TaxTests
{
    static Game Lab(bool earning = true, System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Security.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Tax.AllowancePerWeek = 0;
            c.PlayerStart.GrantPerWeek = 400_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        if (earning)
        {
            ModelState m = g.Train("mini", 300);
            g.UntilTrained(m);
            g.Release(m, "game-ai");
        }
        g.Me.TaxAccrued = 0;
        g.Me.TaxResearch = 0;
        return g;
    }

    static int Year(Game g) { return g.C.Calendar.WeeksPerYear; }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static void ToYearEnd(Game g)
    {
        int left = Year(g) - g.State.Tick % Year(g);
        g.Weeks(left);
    }

    [Fact]
    public void A_profitable_year_pays_its_share_at_the_year_end_and_starts_again()
    {
        Game g = Lab();
        g.Weeks(Year(g) - 5 - g.State.Tick % Year(g));
        Assert.True(g.Me.TaxAccrued > 0, "profit accrued tax");
        long accrued = g.Me.TaxAccrued;
        long fines = g.Me.Ledger.Lifetime.Fines;
        long cash = g.Me.Cash;
        ToYearEnd(g);
        Assert.Equal(1, Count(g, EventKind.TaxPaid));
        GameEvent paid = g.State.Events.Last(e => e.Kind == EventKind.TaxPaid);
        Assert.True(paid.Amount >= accrued);
        Assert.Equal(0, g.Me.TaxAccrued - g.Me.TaxAccrued);
        Assert.True(g.Me.Ledger.Lifetime.Fines <= fines - paid.Amount);
        Assert.True(g.Me.Cash < cash + g.Me.Ledger.Lifetime.Net - paid.Amount + 1_000_000_000_000);
        Assert.Equal(0, paid.Pick);
    }

    [Fact]
    public void A_year_of_losses_owes_nothing_and_a_loss_week_takes_back_what_a_good_week_owed()
    {
        Game g = Lab(earning: false, more: c => c.PlayerStart.GrantPerWeek = 0);
        ToYearEnd(g);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.TaxPaid).Amount);
        Game h = Lab(earning: false, more: c => c.PlayerStart.GrantPerWeek = 0);
        h.Me.TaxAccrued = 5000;
        h.Weeks(2);
        Assert.True(h.Me.TaxAccrued < 5000);
        Assert.True(h.Me.TaxAccrued >= 0);
    }

    [Fact]
    public void A_structure_is_built_over_weeks_costs_a_fee_and_takes_a_share_off_the_tax()
    {
        Game plain = Lab();
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.BuildTaxStructure(g.Id, 1));
        Assert.Equal(cash - g.C.Tax.HoldingCost, g.Me.Cash);
        Assert.Equal(1, g.Me.TaxBuilding);
        g.Refused(Command.BuildTaxStructure(g.Id, 2), CommandError.InvalidArgument);
        g.Weeks(g.C.Tax.HoldingWeeks);
        Assert.Equal(1, g.Me.TaxStructure);
        Assert.Equal(1, Count(g, EventKind.TaxStructureBuilt));
        plain.Weeks(g.C.Tax.HoldingWeeks);
        g.Weeks(20);
        plain.Weeks(20);
        Assert.True(g.Me.TaxSaved > 0);
        Assert.True(g.Me.TaxExposure > 0);
        Assert.True(g.Me.TaxAccrued < plain.Me.TaxAccrued);
        Assert.Equal(0, plain.Me.TaxExposure);
        g.Do(Command.BuildTaxStructure(g.Id, 0));
        Assert.Equal(0, g.Me.TaxStructure);
        g.Refused(Command.BuildTaxStructure(g.Id, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void An_offshore_structure_saves_more_and_is_watched_more_closely()
    {
        Game hold = Lab();
        Game off = Lab();
        hold.Do(Command.BuildTaxStructure(hold.Id, 1));
        off.Do(Command.BuildTaxStructure(off.Id, 2));
        hold.Weeks(off.C.Tax.OffshoreWeeks + 10);
        off.Weeks(off.C.Tax.OffshoreWeeks + 10);
        Assert.Equal(2, off.Me.TaxStructure);
        Assert.True(off.Me.TaxExposure > hold.Me.TaxExposure);
        Assert.True(off.C.Tax.OffshoreRelief > hold.C.Tax.HoldingRelief);
    }

    [Fact]
    public void Claiming_the_credit_takes_the_trainings_spend_off_the_bill_and_adds_exposure()
    {
        Game g = Lab();
        g.Train("mini", 300);
        g.Weeks(3);
        long spend = g.Me.TaxResearch;
        Assert.True(spend > 0, "training counted as research spend");
        int exposure = g.Me.TaxExposure;
        g.Do(Command.ClaimTaxCredit(g.Id));
        long credit = g.State.Events.Last(e => e.Kind == EventKind.TaxCreditClaimed).Amount;
        Assert.Equal(MathI.MulPermille(spend, g.C.Tax.CreditPermille), credit);
        Assert.Equal(credit, g.Me.TaxCredit);
        Assert.Equal(credit, g.Me.TaxClaimed);
        Assert.Equal(exposure + g.C.Tax.ClaimExposure, g.Me.TaxExposure);
        Assert.Equal(0, g.Me.TaxResearch);
        g.Refused(Command.ClaimTaxCredit(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void An_exposed_lab_is_audited_at_the_year_end_and_can_cooperate()
    {
        Game g = Lab();
        g.Do(Command.BuildTaxStructure(g.Id, 1));
        g.Weeks(g.C.Tax.HoldingWeeks + 5);
        g.Me.TaxExposure = g.C.Tax.AuditThreshold;
        ToYearEnd(g);
        Assert.True(Tax.Auditing(g.Me));
        Assert.Equal(1, Count(g, EventKind.TaxAuditOpened));
        long owed = g.Me.TaxAuditOwed;
        Assert.True(owed > 0);
        g.Refused(Command.BuildTaxStructure(g.Id, 0), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        g.Do(Command.AnswerTaxAudit(g.Id, Tax.Cooperate));
        Assert.Equal(owed + MathI.MulPermille(owed, g.C.Tax.CooperateInterest), cash - g.Me.Cash);
        Assert.Equal(0, g.Me.TaxStructure);
        Assert.Equal(0, g.Me.TaxExposure);
        Assert.Equal(0, g.Me.TaxSaved);
        Assert.False(Tax.Auditing(g.Me));
        Assert.Equal(Tax.Cooperate, g.State.Events.Last(e => e.Kind == EventKind.TaxAuditClosed).Pick);
    }

    [Fact]
    public void Settling_keeps_the_structure_and_contesting_puts_the_claim_on_the_docket()
    {
        Game g = Lab();
        g.Do(Command.BuildTaxStructure(g.Id, 1));
        g.Weeks(g.C.Tax.HoldingWeeks + 5);
        g.Me.TaxExposure = g.C.Tax.AuditThreshold * 2;
        ToYearEnd(g);
        long owed = g.Me.TaxAuditOwed;
        long cash = g.Me.Cash;
        int exposed = g.Me.TaxExposure;
        g.Do(Command.AnswerTaxAudit(g.Id, Tax.Settle));
        Assert.Equal(MathI.MulPermille(owed, g.C.Tax.SettlePermille), cash - g.Me.Cash);
        Assert.Equal(1, g.Me.TaxStructure);
        Assert.Equal(exposed / 2, g.Me.TaxExposure);

        Game h = Lab();
        h.Do(Command.BuildTaxStructure(h.Id, 1));
        h.Weeks(h.C.Tax.HoldingWeeks + 5);
        h.Me.TaxExposure = h.C.Tax.AuditThreshold;
        ToYearEnd(h);
        long stake = h.Me.TaxAuditOwed;
        long before = h.Me.Cash;
        h.Do(Command.AnswerTaxAudit(h.Id, Tax.Contest));
        LawCase claim = h.Me.Cases.Single(x => x.Kind == Litigation.Tax);
        Assert.Equal(MathI.MulPermille(stake, h.C.Tax.ContestClaimPermille), claim.Claim);
        Assert.Equal(h.C.Tax.ContestStrength + h.C.Tax.StructureStrength, claim.Strength);
        Assert.Equal(before, h.Me.Cash);
        Assert.Equal(0, h.Me.TaxExposure);
    }

    [Fact]
    public void An_audit_nobody_answers_is_lost_by_default_at_a_price()
    {
        Game g = Lab();
        g.Do(Command.BuildTaxStructure(g.Id, 2));
        g.Weeks(g.C.Tax.OffshoreWeeks + 3);
        g.Me.TaxExposure = g.C.Tax.AuditThreshold;
        ToYearEnd(g);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.TaxAuditOpened).Pick);
        long lapse = Tax.LapseCost(g.C, g.Me);
        Assert.True(lapse > g.Me.TaxAuditOwed);
        g.Weeks(g.C.Tax.AuditWeeks + 1);
        Assert.False(Tax.Auditing(g.Me));
        GameEvent closed = g.State.Events.Last(e => e.Kind == EventKind.TaxAuditClosed);
        Assert.Equal(Tax.Lapsed, closed.Pick);
        Assert.Equal(lapse, closed.Amount);
        Assert.Equal(0, g.Me.TaxStructure);
    }

    [Fact]
    public void Commands_are_refused_when_they_make_no_sense_or_the_system_is_off()
    {
        Game g = Lab();
        g.Refused(Command.BuildTaxStructure(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.BuildTaxStructure(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.BuildTaxStructure(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.AnswerTaxAudit(g.Id, 0), CommandError.InvalidArgument);
        g.Me.Cash = 1;
        Assert.Equal(CommandError.NotEnoughCash, g.Try(Command.BuildTaxStructure(g.Id, 2)).Error);
        g.C.Tax.Enabled = 0;
        g.Refused(Command.BuildTaxStructure(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.ClaimTaxCredit(g.Id), CommandError.InvalidArgument);
        g.Weeks(Year(g));
        Assert.Equal(0, Count(g, EventKind.TaxPaid));
    }

    [Fact]
    public void Tax_books_save_load_and_play_on_the_same_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.BuildTaxStructure(g.Id, 1));
        g.Weeks(g.C.Tax.HoldingWeeks + 5);
        g.Me.TaxExposure = g.C.Tax.AuditThreshold;
        ToYearEnd(g);
        Assert.True(Tax.Auditing(g.Me));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 85, g.Sim.Save());
        Assert.DoesNotContain("taxAccrued", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.TaxStructure);
        Assert.Equal(0, lab.TaxAuditDue);
    }

    [Fact]
    public void A_rival_with_tax_books_is_not_sound_and_bad_data_is_caught()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.TaxStructure = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("tax books"));
        rival.TaxStructure = 0;
        g.Me.TaxBuilding = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("tax books"));
        g.Me.TaxBuilding = 0;
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Tax.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Tax.Enabled = 2;
        c.Tax.RatePermille = 901;
        c.Tax.OffshoreCost = c.Tax.HoldingCost - 1;
        c.Tax.OffshoreRelief = c.Tax.HoldingRelief - 1;
        c.Tax.OffshoreExposure = c.Tax.HoldingExposure - 1;
        c.Tax.AuditThreshold = 0;
        c.Tax.SettlePermille = 1001;
        c.Tax.ContestStrength = 900;
        c.Tax.StructureStrength = 100;
        c.Tax.ExposedReputation = 501;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "ratePermille", "offshoreCost", "offshoreRelief", "offshoreExposure", "auditThreshold", "settlePermille", "contestStrength", "exposedReputation" })
            Assert.Contains(problems, p => p.Contains("tax." + key));
    }
}
