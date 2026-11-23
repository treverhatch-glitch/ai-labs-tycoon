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

/// <summary>Edge coverage for drills, eval harness, tool policy, human queue, and release checklist (schema 125–129).</summary>
public class EraLSystemsEdgeHardeningTests
{
    static Game Lab(long seed = 8, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Drills.MinEra = 0;
            c.EvalHarness.MinEra = 0;
            c.ToolPolicy.MinEra = 0;
            c.HumanQueue.MinEra = 0;
            c.Agency.MinEra = 0;
            c.ReleaseChecklist.MinEra = 0;
            c.ReleaseChecklist.Enabled = 0;
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 500_000_000_000;
        g.Me.ResearchPoints = 500_000;
        return g;
    }

    static ModelState Sold(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    static ModelState Agent(Game g)
    {
        ModelState m = Sold(g);
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        return m;
    }

    static void DisableEraL(GameConfig c)
    {
        c.Drills.Enabled = 0;
        c.EvalHarness.Enabled = 0;
        c.ToolPolicy.Enabled = 0;
        c.HumanQueue.Enabled = 0;
        c.ReleaseChecklist.Enabled = 0;
    }

    [Fact]
    public void Drill_mid_save_load_matches_prep_and_left()
    {
        Game g = Lab(51);
        g.Do(Command.StartDrill(g.Id));
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        Assert.Equal(g.Me.DrillLeft, back.Player.DrillLeft);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Tool_policy_mid_save_load_matches_mask()
    {
        Game g = Lab(52, c => c.AgentSandbox.Enabled = 0);
        ModelState m = Agent(g);
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web | ToolPolicy.Code));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(m.ToolMask, back.Player.Models[0].ToolMask);
    }

    [Fact]
    public void Eval_registration_mid_save_load_matches_suite()
    {
        Game g = Lab(53);
        ModelState m = Sold(g);
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(8);
        Assert.Equal(m.EvalSuite, back.Player.Models[0].EvalSuite);
    }

    [Fact]
    public void Checklist_item_mid_save_load_matches_done_count()
    {
        Game g = Lab(54, c => c.ReleaseChecklist.Enabled = 1);
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        m.DocLevel = g.C.ReleaseChecklist.DocBar;
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Docs));
        string mid = g.Sim.Save();
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(3);
        Assert.Equal(m.ChkDone, back.Player.Models[0].ChkDone);
    }

    [Fact]
    public void All_era_l_tools_disabled_autopilot_matches_digest_without_commands()
    {
        Game off = Lab(55, DisableEraL);
        off.Autopilot(48);
        Game plain = Lab(55, DisableEraL);
        plain.Autopilot(48);
        Assert.Equal(plain.Sim.Digest(), off.Sim.Digest());
        Assert.Empty(SaveGame.Problems(off.C, off.State));
    }

    [Fact]
    public void Schema_125_through_129_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(56, c => c.ReleaseChecklist.Enabled = 1);
        g.Do(Command.StartDrill(g.Id));
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        m.DocLevel = g.C.ReleaseChecklist.DocBar;
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        m.ToolMask = ToolPolicy.Code;
        m.AgentLevel = 1;
        foreach (int item in new[] { ReleaseChecklist.Red, ReleaseChecklist.Docs, ReleaseChecklist.Eval, ReleaseChecklist.Policy })
            g.Do(Command.SignChecklistItem(g.Id, m.Id, item));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        string current = g.Sim.Save();
        for (int schema = 125; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            sim.StepMany(20);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            if (schema >= 120) HardeningInvariants.AssertEraKStateBounds(g.C, sim.Player);
            if (schema >= 125) HardeningInvariants.AssertEraLStateBounds(g.C, sim.Player);
        }
    }

    [Fact]
    public void Released_model_with_eval_and_policy_survives_save_load()
    {
        Game g = Lab(57);
        ModelState m = Sold(g);
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Do(Command.SetAutonomy(g.Id, m.Id, 1));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web));
        string mid = g.Sim.Save();
        g.Weeks(10);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(10);
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }
}