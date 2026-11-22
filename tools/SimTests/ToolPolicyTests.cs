using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ToolPolicyTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(8, c =>
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
            c.Agency.MinEra = 0;
            c.ToolPolicy.MinEra = 0;
            c.HumanQueue.Enabled = 0;
            c.AgentSandbox.Enabled = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 3, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Agent(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        return m;
    }

    [Fact]
    public void A_model_with_no_tool_policy_changes_nothing()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        Assert.Equal(MathI.Permille, ToolPolicy.LiftBonus(g.C, m));
        Assert.Equal(0, ToolPolicy.IncidentExtra(g.C, m));
        Assert.Equal(0, ToolPolicy.BlowupExtra(g.C, m));
        Assert.Equal(0, ToolPolicy.QueueAccrue(g.C, m));
    }

    [Fact]
    public void Set_tool_policy_is_checked_and_costs_cash()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        g.Refused(Command.SetToolPolicy(g.Id, m.Id, 8), CommandError.InvalidArgument);
        g.Refused(Command.SetToolPolicy(g.Id, 999, 1), CommandError.UnknownId);
        Game bare = Lab();
        ModelState tm = bare.Train("mini", 0);
        bare.UntilTrained(tm);
        bare.Do(Command.ReleaseModel(bare.Id, tm.Id, 0, "game-ai"));
        bare.Refused(Command.SetToolPolicy(bare.Id, tm.Id, 1), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web | ToolPolicy.Code));
        Assert.Equal(cash - g.C.ToolPolicy.SetupCost, g.Me.Cash);
        Assert.Equal(ToolPolicy.Web | ToolPolicy.Code, m.ToolMask);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ToolPolicySet);
        Game poor = Lab();
        ModelState pm = Agent(poor);
        poor.Me.Cash = g.C.ToolPolicy.SetupCost - 1;
        poor.Refused(Command.SetToolPolicy(poor.Id, pm.Id, 1), CommandError.NotEnoughCash);
        Game early = Lab(c => c.ToolPolicy.MinEra = c.Eras.Count - 1);
        ModelState em = Agent(early);
        early.Refused(Command.SetToolPolicy(early.Id, em.Id, 1), CommandError.EraLocked);
        Game off = Lab(c => c.ToolPolicy.Enabled = 0);
        ModelState om = Agent(off);
        off.Refused(Command.SetToolPolicy(off.Id, om.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Each_allowed_tool_lifts_revenue_and_adds_incident_risk()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        int baseInc = Agency.IncidentPermille(g.C, m);
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web));
        Assert.Equal(MathI.Permille + g.C.ToolPolicy.LiftWeb, ToolPolicy.LiftBonus(g.C, m));
        Assert.Equal(baseInc + g.C.ToolPolicy.IncidentWeb, Agency.IncidentPermille(g.C, m));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web | ToolPolicy.Code | ToolPolicy.Data));
        Assert.Equal(MathI.Permille + g.C.ToolPolicy.LiftWeb + g.C.ToolPolicy.LiftCode + g.C.ToolPolicy.LiftData, ToolPolicy.LiftBonus(g.C, m));
        Assert.Equal(baseInc + g.C.ToolPolicy.IncidentWeb + g.C.ToolPolicy.IncidentCode + g.C.ToolPolicy.IncidentData, Agency.IncidentPermille(g.C, m));
    }

    [Fact]
    public void Blow_up_risk_and_queue_accrue_need_delegation_and_a_mask()
    {
        Game g = Lab(c => c.Agency.Cooldown = 0);
        ModelState m = Agent(g);
        Assert.Equal(0, ToolPolicy.BlowupExtra(g.C, m));
        Assert.Equal(0, ToolPolicy.QueueAccrue(g.C, m));
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Code));
        Assert.Equal(g.C.ToolPolicy.BlowupCode, ToolPolicy.BlowupExtra(g.C, m));
        Assert.Equal(g.C.ToolPolicy.QueueCode, ToolPolicy.QueueAccrue(g.C, m));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, 0));
        Assert.Equal(0, ToolPolicy.QueueAccrue(g.C, m));
    }

    [Fact]
    public void Agent_runs_accrue_the_human_queue_when_tools_are_on()
    {
        Game g = Lab(c => { c.HumanQueue.Enabled = 1; c.HumanQueue.MinEra = 0; c.Agency.Cooldown = 0; });
        ModelState m = Agent(g);
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Data));
        Agency.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.ToolPolicy.QueueData, g.Me.HumanQueue);
    }

    [Fact]
    public void Lift_bonus_is_capped_and_zero_mask_uses_the_baseline()
    {
        Game g = Lab(c =>
        {
            c.ToolPolicy.LiftWeb = 900;
            c.ToolPolicy.LiftCode = 900;
            c.ToolPolicy.LiftData = 900;
        });
        ModelState m = Agent(g);
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web | ToolPolicy.Code | ToolPolicy.Data));
        Assert.Equal(2500, ToolPolicy.LiftBonus(g.C, m));
        m.ToolMask = 0;
        Assert.Equal(MathI.Permille, ToolPolicy.LiftBonus(g.C, m));
    }

    [Fact]
    public void A_model_that_loses_its_agents_can_keep_a_mask_saved_but_it_adds_nothing()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 0));
        Assert.Equal(0, m.AgentLevel);
        Assert.Equal(g.C.ToolPolicy.IncidentWeb, ToolPolicy.IncidentExtra(g.C, m));
        m.ToolMask = 0;
        Assert.Equal(0, ToolPolicy.IncidentExtra(g.C, m));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_tool_mask()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 126, g.Sim.Save()));
        Assert.All(older.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.ToolMask));
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}