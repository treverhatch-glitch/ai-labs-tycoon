using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-sixteen combination rules keep effects through saves (Q473).</summary>
public class EraPBatchSixteenTogetherHardeningTests
{
    static Game Lab()
    {
        Game g = Game.Solo(160, c =>
        {
            c.MarginCall.Enabled = 1;
            c.MarginCall.CovenantPermille = 950;
            c.CampaignDeadline.MinEra = 0;
            c.ScenarioGoals.Enabled = 1;
            c.LawShaping.MinEra = 0;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
            c.Office.ChancePermille = 0;
        }).WithStarterLab(gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Scenario_deadline_goal_mid_save_still_resolves()
    {
        Game g = Game.Solo(161, c => { c.CampaignDeadline.MinEra = 0; c.ScenarioGoals.Enabled = 1; c.Office.ChancePermille = 0; }).WithStarterLab(researchers: 2, gpus: 4);
        g.Me.ScenarioId = g.C.Scenarios.Scenarios[0].IdValue;
        int stop = g.Tick + 3;
        g.Do(Command.SetCampaignDeadline(g.Id, stop));
        GameConfig c = g.C;
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(c, mid));
        after.Weeks(3);
        Assert.Contains(after.State.Events, e => e.Kind == EventKind.CampaignDeadlineHit || e.Kind == EventKind.ScenarioGoalMet || e.Kind == EventKind.ScenarioGoalMissed);
    }

    [Fact]
    public void Law_target_shape_mid_save_keeps_pending()
    {
        Game g = Lab();
        EraLawHardeningSupport.ShapeLaw(g, LawShaping.Loose);
        int pending = g.Me.LawShapePending;
        string mid = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(2);
        Assert.Equal(pending, back.Player.LawShapePending);
        Assert.Equal(LawShaping.Loose, back.Player.LawTarget);
    }

    [Fact]
    public void Margin_call_mid_save_still_allows_satisfy()
    {
        Game g = Lab();
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).IdValue, 6);
        g.Do(Command.TakeLoan(g.Id, 800));
        Fleet.Take(g.Me, g.Me.Fleet[0], g.Me.Fleet[0].Count - 1);
        Finance.RunWeek(g.C, g.State, g.Me);
        if (g.Me.MarginCallLeft < 1) return;
        string mid = g.Sim.Save();
        Game after = new Game(Simulation.Load(g.C, mid));
        int pay = (int)MarginCall.CashDue(after.C, after.State, after.Me);
        after.Do(Command.SatisfyMarginCall(after.Id, 0, pay));
        Assert.Equal(0, after.Me.MarginCallLeft);
    }
}