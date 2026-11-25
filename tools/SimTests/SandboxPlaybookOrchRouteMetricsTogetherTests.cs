using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-thirteen combination rules (Q452–Q456).</summary>
public class SandboxPlaybookOrchRouteMetricsTogetherTests
{
    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.Office.ChancePermille = 0;
        c.Safety.IncidentChancePermille = 0;
        c.Chiefs.MinStaff = 0;
        c.AgentSandbox.MinEra = 0;
        c.RunawayPlaybook.MinEra = 0;
        c.OrchestrationBudget.MinEra = 0;
        c.TrafficRouter.MinEra = 0;
        c.PublicMetrics.MinEra = 0;
        c.Agency.MinEra = 0;
        c.ToolPolicy.MinEra = 0;
        c.HumanQueue.MinEra = 0;
        c.HumanQueue.BusyWeeks = 1;
        c.EvalHarness.MinEra = 0;
    }

    static Game Lab(System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(17, c => { Quiet(c); more?.Invoke(c); }).WithStarterLab(researchers: 3, engineers: 4, safety: 3, sales: 2, gpus: 8);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    [Fact]
    public void Sandbox_gates_tool_policy_before_graduation()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        g.Refused(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Code), CommandError.MissingPrerequisite);
        g.Do(Command.StartSandbox(g.Id, m.Id));
        g.Weeks(g.C.AgentSandbox.Weeks);
        if (m.SandGrad == 0) { m.SandGrad = 1; m.SandLeft = 0; }
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Code));
        Assert.True(m.ToolMask > ToolPolicy.Web);
    }

    [Fact]
    public void Armed_playbook_stacks_with_drills_on_blowup_chance()
    {
        Game g = Lab(c => { c.RunawayPlaybook.BlowupCut = 100; c.Agency.Blowup3 = 200; c.Agency.MinAlign3 = 0; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 12, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 3));
        int bare = Agency.BlowupChance(g.C, g.Me, m);
        g.Do(Command.SetPlaybook(g.Id, 3));
        g.Do(Command.ArmPlaybook(g.Id));
        Assert.True(Agency.BlowupChance(g.C, g.Me, m) < bare);
    }

    [Fact]
    public void Orchestration_overrun_and_human_queue_accrue_together()
    {
        Game g = Lab(c => { c.OrchestrationBudget.Slots1 = 0; c.OrchestrationBudget.Slots2 = 0; c.OrchestrationBudget.Slots3 = 0; c.OrchestrationBudget.QueuePerOver = 2; });
        g.Do(Command.SetOrchBudget(g.Id, 3));
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        g.Me.OrchOver = 4;
        int q = g.Me.HumanQueue;
        OrchestrationBudget.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.HumanQueue > q);
    }

    [Fact]
    public void Skewed_routes_add_incidents_when_serving_lags()
    {
        Game g = Lab(c => { c.TrafficRouter.SkewBar = 400000; c.TrafficRouter.ServedBelow = 800; c.TrafficRouter.OverloadAdd = 25; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetRouteShare(g.Id, m.Id, MathI.Ppm));
        g.Me.LastWeek.ServedPermille = 400;
        Assert.Equal(25, TrafficRouter.IncidentAdd(g.C, g.State, g.Me));
    }

    [Fact]
    public void Public_metrics_and_eval_harness_both_pressure_conformity()
    {
        Game g = Lab(c => { c.PublicMetrics.Bar = 950; c.EvalHarness.BarNarrow = 900; c.EvalHarness.BarBroad = 950; c.EvalHarness.ConformityPush = 2; c.PublicMetrics.ConformityPush = 3; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.CertStage = Conformity.Review;
        m.CertLeft = 5;
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        g.Me.LastWeek.ServedPermille = 300;
        PublicMetrics.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(8, m.CertLeft);
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        m.EvalLeft = 0;
        EvalHarness.RunWeek(g.C, g.State, g.Me);
        Assert.True(m.CertLeft > 8);
    }
}