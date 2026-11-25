using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q473 combination wiring for batch sixteen systems.</summary>
public class BatchSixteenTogetherTests
{
    static Game Lab()
    {
        Game g = Game.Solo(59, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 950;
            c.HearingSharePrice.Enabled = 1;
            c.RungSixCrisis.MinEra = 0;
            c.RungSixCrisis.CorruptionMin = 100;
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.CampaignDeadline.MinEra = 0;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Margin_call_with_loan_repay()
    {
        Game g = Lab();
        Fleet.AddGpus(g.State, g.Me, 0, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 4);
        g.Do(Command.TakeLoan(g.Id, 950));
        int before = g.Me.Fleet.Sum(b => b.Count);
        Fleet.Take(g.Me, g.Me.Fleet[0], before - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.MarginCallLeft > 0 || g.State.Events.Any(e => e.Kind == EventKind.MarginCall));
    }

    [Fact]
    public void Law_target_with_shape_law()
    {
        Game g = Lab();
        g.Do(Command.SetLawTarget(g.Id, LawShaping.Loose));
        g.Do(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Loose));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LawTargetSet);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LawShaped);
    }

    [Fact]
    public void Scenario_deadline_with_goal()
    {
        Game g = Game.Solo(59, c => { c.CampaignDeadline.MinEra = 0; }).WithStarterLab(researchers: 2, gpus: 4);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        g.Do(Command.SetCampaignDeadline(g.Id, g.Tick + 3));
        g.Weeks(3);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.CampaignDeadlineHit);
    }
}