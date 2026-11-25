using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q497 combination wiring for batch twenty-two systems.</summary>
public class BatchTwentyTwoTogetherTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(93, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Law_target_era_amplify_on_set()
    {
        Game g = Lab(c =>
        {
            c.LawTarget.Enabled = 1;
            c.LawTargetEraAmplify.Enabled = 1;
            c.LawTargetEraAmplify.MinEra = 0;
        });
        g.Do(Command.SetLawTarget(g.Id, LawShaping.Strict));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LawTargetEraAmplified);
    }

    [Fact]
    public void Specialist_branch_discount_event()
    {
        Game g = Lab(c =>
        {
            c.SpecialistLines.MinEra = 0;
            c.SpecialistBranchEraDiscount.Enabled = 1;
            c.SpecialistBranchEraDiscount.MinEra = 0;
            c.SpecialistBranchEraDiscount.DiscountPermille = 300;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.BranchSpecialist(g.Id, m.Id, "code"));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SpecialistBranchEraDiscounted);
    }

    [Fact]
    public void Margin_call_gets_era_grace_weeks()
    {
        Game g = Game.Solo(93, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 1000;
            c.MarginCall.CureWeeks = 4;
            c.MarginCallEraGrace.Enabled = 1;
            c.MarginCallEraGrace.MinEra = 0;
            c.MarginCallEraGrace.GraceWeeks = 3;
            c.Loans.LoanToValuePermille = 800;
        }).WithStarterLab(gpus: 2);
        g.Me.Cash = 500_000_000;
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 950));
        int before = g.Me.Fleet.Sum(b => b.Count);
        Fleet.Take(g.Me, g.Me.Fleet[0], before - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(7, g.Me.MarginCallLeft);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.MarginCallEraGraced);
    }
}