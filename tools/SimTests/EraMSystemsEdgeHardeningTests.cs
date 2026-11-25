using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Edge coverage for sandbox, playbook, orchestration, routing, and public metrics (schema 130–134).</summary>
public class EraMSystemsEdgeHardeningTests
{
    static Game Lab(long seed = 70, System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.AgentSandbox.MinEra = 0;
            c.RunawayPlaybook.MinEra = 0;
            c.OrchestrationBudget.MinEra = 0;
            c.TrafficRouter.MinEra = 0;
            c.PublicMetrics.MinEra = 0;
            c.Agency.MinEra = 0;
            c.ToolPolicy.MinEra = 0;
            c.PlayerStart.Cash = 50_000_000;
            tweak?.Invoke(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 3, sales: 2, gpus: 8);
        g.Me.Cash = 500_000_000_000;
        g.Me.ResearchPoints = 500_000;
        return g;
    }

    static ModelState AgentSold(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        return m;
    }

    static void DisableEraM(GameConfig c)
    {
        c.AgentSandbox.Enabled = 0;
        c.RunawayPlaybook.Enabled = 0;
        c.OrchestrationBudget.Enabled = 0;
        c.TrafficRouter.Enabled = 0;
        c.PublicMetrics.Enabled = 0;
    }

    [Fact]
    public void Sandbox_mid_save_load_matches_graduation_fields()
    {
        Game g = Lab(70);
        ModelState m = AgentSold(g);
        g.Do(Command.StartSandbox(g.Id, m.Id));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(m.SandLeft, back.Player.Models[0].SandLeft);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Playbook_mid_save_load_matches_level_and_armed()
    {
        Game g = Lab(71);
        g.Do(Command.SetPlaybook(g.Id, 2));
        g.Do(Command.ArmPlaybook(g.Id));
        string mid = g.Sim.Save();
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(3);
        Assert.Equal(g.Me.PlayLevel, back.Player.PlayLevel);
        Assert.Equal(g.Me.PlayArmed, back.Player.PlayArmed);
    }

    [Fact]
    public void Orchestration_mid_save_load_matches_level_and_overrun()
    {
        Game g = Lab(72);
        g.Do(Command.SetOrchBudget(g.Id, 3));
        g.Me.OrchOver = 2;
        string mid = g.Sim.Save();
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(2);
        Assert.Equal(g.Me.OrchLevel, back.Player.OrchLevel);
        Assert.Equal(g.Me.OrchOver, back.Player.OrchOver);
    }

    [Fact]
    public void Route_share_mid_save_load_matches_ppm()
    {
        Game g = Lab(73);
        ModelState m = AgentSold(g);
        g.Do(Command.SetRouteShare(g.Id, m.Id, MathI.Ppm / 2));
        string mid = g.Sim.Save();
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(2);
        Assert.Equal(m.RoutePpm, back.Player.Models[0].RoutePpm);
    }

    [Fact]
    public void Public_metrics_mid_save_load_matches_pub_fields()
    {
        Game g = Lab(74);
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        Assert.Equal(1, m.PubOn);
        string mid = g.Sim.Save();
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(2);
        Assert.Equal(m.PubOn, back.Player.Models[0].PubOn);
    }

    [Fact]
    public void All_era_m_tools_disabled_autopilot_matches_digest_without_commands()
    {
        Game off = Lab(75, DisableEraM);
        off.Autopilot(48);
        Game plain = Lab(75, DisableEraM);
        plain.Autopilot(48);
        Assert.Equal(plain.Sim.Digest(), off.Sim.Digest());
        Assert.Empty(SaveGame.Problems(off.C, off.State));
    }

    [Fact]
    public void Schema_130_through_134_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(76);
        ModelState m = AgentSold(g);
        g.Do(Command.StartSandbox(g.Id, m.Id));
        g.Weeks(g.C.AgentSandbox.Weeks);
        if (m.SandGrad == 0) { m.SandGrad = 1; m.SandLeft = 0; }
        g.Do(Command.SetPlaybook(g.Id, 2));
        g.Do(Command.SetOrchBudget(g.Id, 2));
        g.Do(Command.SetRouteShare(g.Id, m.Id, MathI.Ppm / 2));
        g.Do(Command.PublishMetrics(g.Id, m.Id));
        string current = g.Sim.Save();
        for (int schema = 130; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            sim.StepMany(20);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            if (schema >= 125) HardeningInvariants.AssertEraLStateBounds(g.C, sim.Player);
            if (schema >= 130) HardeningInvariants.AssertEraMStateBounds(g.C, sim.Player);
        }
    }
}