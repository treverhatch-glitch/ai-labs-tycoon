using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class AllowanceTests
{
    static Game Lab(System.Action<GameConfig> more = null)
    {
        return Game.Solo(3, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Tax.AllowancePerWeek = 0;
            c.PlayerStart.GrantPerWeek = 400_000;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    static long Run(Game g, long spend)
    {
        g.Me.Ledger.LastWeek.Hardware = -spend;
        g.Me.Ledger.LastWeek.Construction = 0;
        long adjust = CapitalAllowances.Adjust(g.C, g.State, g.Me);
        g.Me.Ledger.LastWeek.Hardware = 0;
        return adjust;
    }

    [Fact]
    public void Expensing_is_the_default_and_changes_nothing()
    {
        Game g = Lab();
        Assert.Equal(0, g.Me.CapPolicy);
        Assert.Equal(0, Run(g, 1_000_000));
        Assert.Equal(0, g.Me.CapPool);
    }

    [Fact]
    public void Capitalising_adds_the_spend_back_and_writes_a_share_off_each_week()
    {
        Game g = Lab();
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
        long off = MathI.MulPermille(1_000_000, g.C.Allowances.WriteOffPermille);
        Assert.Equal(1_000_000 - off, Run(g, 1_000_000));
        Assert.Equal(1_000_000 - off, g.Me.CapPool);
        long pool = g.Me.CapPool;
        long next = MathI.MulPermille(pool, g.C.Allowances.WriteOffPermille);
        Assert.Equal(-next, Run(g, 0));
        Assert.Equal(pool - next, g.Me.CapPool);
        Assert.Equal(off + next, g.Me.CapWritten);
    }

    [Fact]
    public void The_enhanced_allowance_deducts_more_than_was_spent_and_adds_exposure_and_tax_saved()
    {
        Game g = Lab();
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Enhanced));
        long extra = MathI.MulPermille(1_000_000, g.C.Allowances.EnhancedPermille);
        long exposure = g.Me.TaxExposure;
        long saved = g.Me.TaxSaved;
        Run(g, 1_000_000);
        Assert.Equal(1_000_000 + extra - MathI.MulPermille(1_000_000 + extra, g.C.Allowances.WriteOffPermille), g.Me.CapPool);
        Assert.Equal(exposure + g.C.Allowances.EnhancedExposure, g.Me.TaxExposure);
        Assert.Equal(saved + MathI.MulPermille(extra, g.C.Tax.RatePermille), g.Me.TaxSaved);
        long still = g.Me.TaxExposure;
        Run(g, 0);
        Assert.Equal(still, g.Me.TaxExposure);
    }

    [Fact]
    public void Going_back_to_expensing_writes_the_whole_pool_off_at_once()
    {
        Game g = Lab(c => c.Allowances.LockWeeks = 0);
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
        Run(g, 1_000_000);
        g.Sim.Step();
        long pool = g.Me.CapPool;
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Expense));
        Assert.Equal(pool, g.State.Events.Last(e => e.Kind == EventKind.CapitalPolicySet).SubjectId);
        Assert.Equal(-pool, Run(g, 0));
        Assert.Equal(0, g.Me.CapPool);
    }

    [Fact]
    public void A_change_of_policy_is_locked_for_a_time()
    {
        Game g = Lab();
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
        g.Refused(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Expense), CommandError.InvalidArgument);
        g.Sim.StepMany(g.C.Allowances.LockWeeks - 1);
        g.Refused(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Expense), CommandError.InvalidArgument);
        g.Sim.StepMany(2);
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Expense));
        Assert.Equal(2, Count(g, EventKind.CapitalPolicySet));
    }

    [Fact]
    public void Bad_orders_are_refused_and_it_needs_the_tax()
    {
        Game g = Lab();
        g.Refused(Command.SetCapitalPolicy(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetCapitalPolicy(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetCapitalPolicy(g.Id, 0), CommandError.InvalidArgument);
        Game h = Lab(c => c.Tax.Enabled = 0);
        h.Refused(Command.SetCapitalPolicy(h.Id, 1), CommandError.InvalidArgument);
        Assert.Equal(0, Run(h, 1_000_000));
        Game k = Lab(c => c.Allowances.Enabled = 0);
        k.Refused(Command.SetCapitalPolicy(k.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Over_a_profitable_year_an_enhanced_allowance_pays_less_tax_than_expensing_and_carries_exposure()
    {
        long Paid(int policy, out long exposure)
        {
            Game g = Lab(c => c.Tax.AuditThreshold = 100000);
            if (policy != 0) g.Do(Command.SetCapitalPolicy(g.Id, policy));
            int left = g.C.Calendar.WeeksPerYear - g.Tick % g.C.Calendar.WeeksPerYear;
            g.Weeks(3);
            Finance.Spend(g.Me, LedgerLine.Hardware, 3_000_000);
            g.Weeks(left - 3);
            exposure = g.Me.TaxExposure;
            return g.State.Events.Last(e => e.Kind == EventKind.TaxPaid).Amount;
        }
        long expensed = Paid(0, out long e0);
        long enhanced = Paid(CapitalAllowances.Enhanced, out long e2);
        Assert.True(enhanced < expensed, enhanced + " vs " + expensed);
        Assert.True(e2 > e0);
    }

    [Fact]
    public void A_policy_saves_loads_and_plays_on_the_same_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.SetCapitalPolicy(g.Id, CapitalAllowances.Capitalise));
        Finance.Spend(g.Me, LedgerLine.Hardware, 500_000);
        g.Weeks(20);
        Assert.True(g.Me.CapPool > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 89, g.Sim.Save());
        Assert.DoesNotContain("capPolicy", old);
        LabState lab = Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.CapPolicy);
        Assert.Equal(0, lab.CapPool);
    }

    [Fact]
    public void A_rival_with_a_policy_is_not_sound_and_bad_data_is_caught()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.CapPool = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("capital allowance"));
        rival.CapPool = 0;
        g.Me.CapPolicy = 3;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("capital allowance"));
        g.Me.CapPolicy = 0;
        g.Me.CapPool = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("capital allowance"));
        g.Me.CapPool = 0;
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Allowances.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Allowances.Enabled = 2;
        c.Allowances.WriteOffPermille = 0;
        c.Allowances.EnhancedPermille = 2001;
        c.Allowances.EnhancedExposure = -1;
        c.Allowances.LockWeeks = 521;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "writeOffPermille", "enhancedPermille", "enhancedExposure", "lockWeeks" })
            Assert.Contains(problems, p => p.Contains("allowances." + key));
    }
}
