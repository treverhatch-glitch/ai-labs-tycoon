using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class AgentSandboxTests
{
    static Game Lab(System.Action<GameConfig> tweak = null, int seed = 3)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.AgentSandbox.MinEra = 0;
            c.Agency.MinEra = 0;
            c.ToolPolicy.MinEra = 0;
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
    public void Models_that_never_sandbox_stay_ungraduated()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        g.Weeks(20);
        Assert.Equal(0, m.SandLeft + m.SandGrad);
    }

    [Fact]
    public void Starting_a_sandbox_is_checked()
    {
        Game g = Lab(c => c.AgentSandbox.Staff = 50);
        ModelState m = Agent(g);
        g.Refused(Command.StartSandbox(g.Id, m.Id), CommandError.NotEnoughStaff);
        g.Refused(Command.StartSandbox(g.Id, 999), CommandError.UnknownId);
        Game bare = Lab();
        ModelState tm = bare.Train("mini", 0);
        bare.UntilTrained(tm);
        bare.Do(Command.ReleaseModel(bare.Id, tm.Id, 0, "game-ai"));
        bare.Refused(Command.StartSandbox(bare.Id, tm.Id), CommandError.InvalidArgument);
        Game poor = Lab();
        ModelState pm = Agent(poor);
        poor.Me.Cash = g.C.AgentSandbox.Cost - 1;
        poor.Refused(Command.StartSandbox(poor.Id, pm.Id), CommandError.NotEnoughCash);
        Game early = Lab(c => c.AgentSandbox.MinEra = c.Eras.Count - 1);
        ModelState em = Agent(early);
        early.Refused(Command.StartSandbox(early.Id, em.Id), CommandError.EraLocked);
        Game off = Lab(c => c.AgentSandbox.Enabled = 0);
        off.Refused(Command.StartSandbox(off.Id, Agent(off).Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_sandbox_costs_runs_for_weeks_and_blocks_a_second_start()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        long cash = g.Me.Cash;
        g.Do(Command.StartSandbox(g.Id, m.Id));
        Assert.Equal(cash - g.C.AgentSandbox.Cost, g.Me.Cash);
        Assert.Equal(g.C.AgentSandbox.Weeks, m.SandLeft);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.SandboxStarted);
        g.Refused(Command.StartSandbox(g.Id, m.Id), CommandError.InvalidArgument);
        g.Weeks(g.C.AgentSandbox.Weeks);
        Assert.Equal(0, m.SandLeft);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SandboxFinished);
    }

    [Fact]
    public void Graduation_unlocks_code_and_data_tools()
    {
        Game g = Lab(c => c.AgentSandbox.PassBar = 1);
        ModelState m = Agent(g);
        m.Capability = 800;
        m.Alignment = 800;
        g.Refused(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Code), CommandError.MissingPrerequisite);
        g.Do(Command.StartSandbox(g.Id, m.Id));
        g.Weeks(g.C.AgentSandbox.Weeks);
        Assert.Equal(1, m.SandGrad);
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Code | ToolPolicy.Data));
        Assert.Equal(ToolPolicy.Code | ToolPolicy.Data, m.ToolMask);
    }

    [Fact]
    public void Tool_policy_during_sandbox_cannot_include_code_or_data()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        g.Do(Command.StartSandbox(g.Id, m.Id));
        g.Refused(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web | ToolPolicy.Code | ToolPolicy.Data), CommandError.MissingPrerequisite);
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web));
        Assert.Equal(ToolPolicy.Web, m.ToolMask);
    }

    [Fact]
    public void Sandbox_weeks_tick_down_each_week()
    {
        Game g = Lab();
        ModelState m = Agent(g);
        g.Do(Command.StartSandbox(g.Id, m.Id));
        int left = m.SandLeft;
        g.Weeks(1);
        Assert.Equal(left - 1, m.SandLeft);
    }

    [Fact]
    public void Only_the_player_lab_starts_sandboxes()
    {
        Game g = Lab();
        g.Refused(Command.StartSandbox(9999, 1), CommandError.UnknownLab);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_sandbox_fields()
    {
        Game g = Lab(c => c.AgentSandbox.PassBar = 1);
        ModelState m = Agent(g);
        m.Capability = 900;
        m.Alignment = 900;
        g.Do(Command.StartSandbox(g.Id, m.Id));
        g.Weeks(g.C.AgentSandbox.Weeks);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 129, g.Sim.Save()));
        Assert.All(older.State.Labs.SelectMany(l => l.Models), x => Assert.Equal(0, x.SandLeft + x.SandGrad));
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}