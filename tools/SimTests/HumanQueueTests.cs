using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class HumanQueueTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(9, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Shocks.Enabled = 0;
            c.AgentSandbox.Enabled = 0;
            c.RunawayPlaybook.Enabled = 0;
            c.OrchestrationBudget.Enabled = 0;
            c.TrafficRouter.Enabled = 0;
            c.PublicMetrics.Enabled = 0;
            c.Agency.MinEra = 0;
            c.ToolPolicy.MinEra = 0;
            c.HumanQueue.MinEra = 0;
            c.HumanQueue.BusyWeeks = 1;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 6);
        g.Build("support-desk", 3, 3);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static void Support(Game g, int count)
    {
        g.Hire("support", count);
    }

    static ModelState AgentWithTools(Game g, int mask)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, mask));
        return m;
    }

    [Fact]
    public void An_empty_queue_changes_nothing_for_a_lab_on_its_own()
    {
        Game g = Lab();
        Assert.Equal(MathI.Permille, HumanQueue.LiftKeep(g.C, g.Me));
        Assert.Equal(0, HumanQueue.IncidentAdd(g.C, g.Me));
        g.Weeks(10);
        Assert.Equal(0, g.Me.HumanQueue);
    }

    [Fact]
    public void Agent_tools_accrue_items_and_a_deep_queue_fires_an_event()
    {
        Game g = Lab(c => { c.HumanQueue.Capacity = 5; c.ToolPolicy.QueueCode = 3; });
        AgentWithTools(g, ToolPolicy.Code);
        g.Me.HumanQueue = 4;
        Agency.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(7, g.Me.HumanQueue);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.HumanQueueDeep);
        Agency.RunWeek(g.C, g.State, g.Me);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.HumanQueueDeep);
    }

    [Fact]
    public void A_queue_past_capacity_cuts_agent_lift()
    {
        Game g = Lab(c => { c.HumanQueue.Capacity = 10; c.HumanQueue.LiftCut = 50; });
        g.Me.HumanQueue = 12;
        Assert.Equal(MathI.Permille - 100, HumanQueue.LiftKeep(g.C, g.Me));
        g.Me.HumanQueue = 10;
        Assert.Equal(MathI.Permille, HumanQueue.LiftKeep(g.C, g.Me));
    }

    [Fact]
    public void Queued_items_raise_incident_risk_on_agent_models()
    {
        Game g = Lab(c => c.HumanQueue.IncidentPer = 20);
        g.Me.HumanQueue = 5;
        Assert.Equal(100, HumanQueue.IncidentAdd(g.C, g.Me));
        g.Me.HumanQueue = 0;
        Assert.Equal(0, HumanQueue.IncidentAdd(g.C, g.Me));
    }

    [Fact]
    public void Paying_clears_items_and_costs_cash()
    {
        Game g = Lab(c => c.HumanQueue.ClearCost = 10);
        g.Me.HumanQueue = 8;
        long cash = g.Me.Cash;
        g.Do(Command.ClearHumanQueue(g.Id, 3));
        Assert.Equal(5, g.Me.HumanQueue);
        Assert.Equal(3, g.Me.HumanCleared);
        Assert.Equal(cash - 30, g.Me.Cash);
        GameEvent e = Assert.Single(g.State.Events, x => x.Kind == EventKind.HumanQueueCleared);
        Assert.Equal(0, e.Pick);
        g.Refused(Command.ClearHumanQueue(g.Id, 0), CommandError.InvalidArgument);
        Game poor = Lab();
        poor.Me.HumanQueue = 5;
        poor.Me.Cash = 1;
        poor.Refused(Command.ClearHumanQueue(poor.Id, 2), CommandError.NotEnoughCash);
    }

    [Fact]
    public void StaffHumanQueue_needs_support_people_and_busy_weeks()
    {
        Game g = Lab(c => c.HumanQueue.StaffNeed = 2);
        g.Me.HumanQueue = 6;
        g.Refused(Command.StaffHumanQueue(g.Id, 2), CommandError.NotEnoughStaff);
        Support(g, 2);
        g.Do(Command.StaffHumanQueue(g.Id, 4));
        Assert.Equal(2, g.Me.HumanQueue);
        Assert.Equal(1, g.Me.HumanBusy);
        Assert.Equal(4, g.Me.HumanCleared);
        GameEvent e = Assert.Single(g.State.Events, x => x.Kind == EventKind.HumanQueueCleared);
        Assert.Equal(1, e.Pick);
        g.Refused(Command.StaffHumanQueue(g.Id, 1), CommandError.InvalidArgument);
        g.Weeks(1);
        g.Do(Command.StaffHumanQueue(g.Id, 1));
    }

    [Fact]
    public void Support_staff_drain_the_queue_each_week()
    {
        Game g = Lab(c => { c.HumanQueue.StaffNeed = 1; c.HumanQueue.Capacity = 100; });
        Support(g, 2);
        g.Me.HumanQueue = 5;
        HumanQueue.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(3, g.Me.HumanQueue);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.HumanQueueCleared && e.Pick == 1);
    }

    [Fact]
    public void Commands_are_checked_and_off_when_disabled()
    {
        Game g = Lab();
        g.Me.HumanQueue = 3;
        g.Refused(Command.StaffHumanQueue(g.Id, 1), CommandError.NotEnoughStaff);
        Game early = Lab(c => c.HumanQueue.MinEra = c.Eras.Count - 1);
        early.Me.HumanQueue = 3;
        Support(early, 2);
        early.Refused(Command.StaffHumanQueue(early.Id, 1), CommandError.EraLocked);
        Game off = Lab(c => c.HumanQueue.Enabled = 0);
        off.Me.HumanQueue = 3;
        off.Refused(Command.ClearHumanQueue(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_human_queue()
    {
        Game g = Lab();
        g.Me.HumanQueue = 12;
        g.Me.HumanCleared = 4;
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 127, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.HumanQueue + lab.HumanBusy + lab.HumanCleared);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}