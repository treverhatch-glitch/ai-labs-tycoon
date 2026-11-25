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

public class TaxStandardsReliabilityCovenantsAllowancesTogetherTests
{
    static Game Lab(Action<GameConfig> more = null, int gpus = 0)
    {
        Game g = Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.BaseFailPpm = 0;
            c.Reliability.AgePpm = 0;
            c.Tax.AllowancePerWeek = 0;
            c.PlayerStart.GrantPerWeek = 400_000;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        if (gpus > 0) Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, gpus);
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static int ToYearEnd(Game g) { return g.C.Calendar.WeeksPerYear - g.Tick % g.C.Calendar.WeeksPerYear; }

    [Fact]
    public void An_enhanced_allowance_makes_an_audit_that_a_contest_puts_on_the_docket()
    {
        Game g = Lab(c => { c.Tax.AuditThreshold = 10; c.Allowances.EnhancedExposure = 50; });
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Enhanced));
        int left = ToYearEnd(g);
        g.Weeks(3);
        Finance.Spend(g.Me, LedgerLine.Hardware, 3_000_000);
        g.Weeks(left - 3);
        Assert.True(g.Me.TaxSaved > 0 || Tax.Auditing(g.Me));
        Assert.Equal(1, Count(g, EventKind.TaxAuditOpened));
        Assert.True(Tax.Auditing(g.Me));
        g.Do(Command.AnswerTaxAudit(g.Id, Tax.Contest));
        LawCase claim = g.Me.Cases.Single();
        Assert.Equal(Litigation.Tax, claim.Kind);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Failed_chips_shrink_the_collateral_until_the_lender_calls_a_breach()
    {
        Game g = Lab(c =>
        {
            c.Reliability.BaseFailPpm = 25_000;
            c.Covenants.CureWeeks = 100;
        }, gpus: 200);
        g.Do(Command.TakeLoan(g.Id, 1000));
        long cover = Covenants.Cover(g.C, g.State, g.Me);
        g.C.Covenants.MinCoverPermille = (int)cover - 1;
        Assert.False(Covenants.InBreach(g.C, g.State, g.Me));
        g.Weeks(3);
        Assert.True(g.Me.GpuFailures >= 5);
        Assert.True(g.Me.CovBreach >= 1);
        Assert.Equal(1, Count(g, EventKind.CovenantBreached));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_waiver_fee_is_deductible_and_paying_a_loan_down_is_not()
    {
        Game Run(string how)
        {
            Game g = Lab(c => c.Covenants.CureWeeks = 100, gpus: 100);
            g.Do(Command.TakeLoan(g.Id, 1000));
            g.C.Covenants.MinCoverPermille = 5000;
            g.Weeks(1);
            if (how == "waive") g.Do(Command.WaiveCovenant(g.Id));
            if (how == "cure") g.Do(Command.CureCovenant(g.Id));
            g.Weeks(2);
            return g;
        }
        Game none = Run("none");
        Game waive = Run("waive");
        Game cure = Run("cure");
        long fee = MathI.MulPermille(Loans.Outstanding(none.Me) - MathI.MulPermille(Loans.Outstanding(none.Me), none.C.Covenants.SurchargePermille * 2), none.C.Covenants.WaiverFeePermille);
        long saved = none.Me.TaxAccrued - waive.Me.TaxAccrued;
        Assert.True(saved > 0, "a waiver fee should lower the tax accrued");
        Assert.True(saved <= MathI.MulPermille(fee * 2, none.C.Tax.RatePermille) + 2, saved + " against a fee of about " + fee);
        Assert.Equal(none.Me.TaxAccrued, cure.Me.TaxAccrued);
    }

    [Fact]
    public void Spares_bought_under_a_capitalising_policy_go_into_the_pool()
    {
        Game Run(bool buy)
        {
            Game g = Lab(c => c.Allowances.LockWeeks = 0);
            g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
            if (buy) g.Do(Command.StockSpares(g.Id, 4));
            g.Weeks(2);
            return g;
        }
        Game with = Run(true);
        Game without = Run(false);
        long price = HardwareReliability.SparePrice(with.C, with.State) * 4;
        long kept = price - MathI.MulPermille(price, with.C.Allowances.WriteOffPermille);
        Assert.InRange(with.Me.CapPool - without.Me.CapPool, kept - 2, kept + 2);
        Assert.Equal(4, with.Me.Spares);
        with.Do(Command.SetCapitalPolicy(with.Id, CapitalAllowances.Expense));
        with.Weeks(1);
        Assert.Equal(0, with.Me.CapPool);
    }

    [Fact]
    public void Standards_adopted_ease_the_scaling_policy_and_the_tax_audit_lapses_when_ignored()
    {
        Game g = Lab(c =>
        {
            c.Standards.Enabled = 1;
            c.Scaling.SafetyPerTier = 6;
            c.Tax.AuditThreshold = 1;
            c.Tax.AuditWeeks = 4;
        });
        g.Me.SecurityTier = 1;
        Assert.False(ScalingPolicy.Safeguarded(g.C, g.Me, 1));
        g.Me.StdLevel = 1;
        g.Do(Command.AdoptStandards(g.Id));
        g.C.Standards.SafetyRelief = 3;
        Assert.True(ScalingPolicy.Safeguarded(g.C, g.Me, 1));

        g.Me.TaxExposure = 500;
        g.Me.TaxSaved = 10_000;
        int left = ToYearEnd(g);
        g.Weeks(left);
        Assert.True(Tax.Auditing(g.Me));
        g.Weeks(g.C.Tax.AuditWeeks + 2);
        Assert.False(Tax.Auditing(g.Me));
        Assert.Equal(Tax.Lapsed, g.State.Events.Last(e => e.Kind == EventKind.TaxAuditClosed).Pick);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab(c => c.Standards.Enabled = 1, gpus: 20);
        g.Do(Command.SetWarranty(g.Id, 1));
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
        g.Me.StdLevel = 1;
        g.Me.TaxStructure = 1;
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
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay(int seed)
    {
        Game g = Game.New(seed);
        g.Autopilot(60);
        g.Me.Cash += 5_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int week = 0; week < 330 && !g.Me.Bankrupt; week++)
        {
            if (week % 40 == 3) g.Try(Command.BuildTaxStructure(g.Id, (week / 40) % 3));
            if (week % 52 == 10) g.Try(Command.ClaimTaxCredit(g.Id));
            if (Tax.Auditing(g.Me)) g.Try(Command.AnswerTaxAudit(g.Id, week % 3));
            if (week % 7 == 0) g.Try(Command.TakeStandardsRole(g.Id, (week / 7) % 4));
            if (week % 60 == 20) g.Try(Command.AdoptStandards(g.Id));
            if (week % 130 == 100) g.Try(Command.DropStandards(g.Id));
            if (week % 50 == 7) g.Try(Command.SetWarranty(g.Id, (week / 50) % 3));
            if (week % 64 == 5) g.Try(Command.StockSpares(g.Id, 2));
            if (g.Me.RmaUnits > 0 && week % 2 == 0) g.Try(Command.ExpediteReplacements(g.Id));
            if (week % 80 == 30) g.Try(Command.TakeLoan(g.Id, 500));
            if (Covenants.InBreach(g.C, g.State, g.Me)) g.Try(week % 2 == 0 ? Command.WaiveCovenant(g.Id) : Command.CureCovenant(g.Id));
            if (week % 60 == 15) g.Try(Command.SetCapitalPolicy(g.Id, (week / 60) % 3));
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.True(Count(g, EventKind.TaxPaid) > 0, "no tax was ever paid");
        Assert.True(Count(g, EventKind.StandardDrafted) > 0, "no standard was ever drafted");
        Assert.True(Count(g, EventKind.WarrantyChanged) + Count(g, EventKind.SparesStocked) + Count(g, EventKind.CapitalPolicySet) > 0, "no cover or policy was ever set");
    }
}
