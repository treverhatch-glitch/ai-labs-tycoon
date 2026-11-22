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

/// <summary>Save/load and edge coverage for programmes, run queue, pods, licences, and pledges (schema 105–109).</summary>
public class EraHSystemsEdgeHardeningTests
{
    const string Tech = "beowulf-clusters";

    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.Office.ChancePermille = 0;
        c.Safety.IncidentChancePermille = 0;
        c.Support.Enabled = 0;
        c.Litigation.Enabled = 0;
        c.Security.Enabled = 0;
        c.GovContracts.Enabled = 0;
        c.Scaling.Enabled = 0;
        c.Charter.Enabled = 0;
        c.Tax.Enabled = 0;
        c.Devices.Enabled = 0;
        c.TechBets.Enabled = 0;
        c.PlayerStart.GrantPerWeek = 0;
    }

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            Quiet(c);
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 40);
        g.Me.ResearchPoints = 500_000;
        g.Me.Reputation = 500;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Alignment = 800;
        g.State.Tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        return m;
    }

  static void FormPod(Game g)
    {
        StaffMember a = g.Me.Staff.First(p => p.RoleId == "researcher");
        StaffMember b = g.Me.Staff.First(p => p.RoleId != "researcher");
        g.Do(Command.FormPod(g.Id, a.Id, b.Id));
    }

    [Fact]
    public void A_programme_mid_save_load_matches_uninterrupted_stage()
    {
        Game straight = Lab(11);
        straight.Do(Command.StartProgramme(straight.Id, Tech));
        straight.Do(Command.ChooseApproach(straight.Id, 1));
        straight.Weeks(4);
        Game choppy = Lab(11);
        choppy.Do(Command.StartProgramme(choppy.Id, Tech));
        choppy.Do(Command.ChooseApproach(choppy.Id, 1));
        choppy.Weeks(2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(2);
        Assert.Equal(straight.Me.PgLeft, choppy.Me.PgLeft);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Run_priority_mid_save_load_matches_training_share()
    {
        Game g = Lab(12);
        ModelState m = g.Train("mini", 0);
        long before = m.ProgressCompute;
        g.Do(Command.SetRunPriority(g.Id, m.Id, 3));
        string mid = g.Sim.Save();
        g.Weeks(3);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(3);
        ModelState after = g.Me.Models.First(x => x.Id == m.Id);
        ModelState reloaded = back.Player.Models.First(x => x.Id == m.Id);
        Assert.Equal(after.QueueLevel, reloaded.QueueLevel);
        Assert.True(reloaded.ProgressCompute > before);
    }

    [Fact]
    public void A_pod_mid_save_load_keeps_chemistry()
    {
        Game straight = Lab(13);
        FormPod(straight);
        straight.Weeks(8);
        Game choppy = Lab(13);
        FormPod(choppy);
        choppy.Weeks(4);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(4);
        Assert.Equal(straight.Me.PodChem, choppy.Me.PodChem);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_licence_review_mid_save_load_keeps_phase()
    {
        Game straight = Lab(14);
        Selling(straight);
        straight.Do(Command.ApplyLicence(straight.Id, 2));
        straight.Weeks(4);
        Game choppy = Lab(14);
        Selling(choppy);
        choppy.Do(Command.ApplyLicence(choppy.Id, 2));
        choppy.Weeks(2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(2);
        Assert.Equal(straight.Me.LicPhase, choppy.Me.LicPhase);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_pledge_mid_save_load_matches_kept_deadline()
    {
        Game straight = Lab(15);
        ModelState m = Selling(straight);
        straight.Do(Command.PledgeSupport(straight.Id, m.Id, 52));
        straight.Weeks(10);
        Game choppy = Lab(15);
        ModelState m2 = Selling(choppy);
        choppy.Do(Command.PledgeSupport(choppy.Id, m2.Id, 52));
        choppy.Weeks(5);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(5);
        Assert.Equal(straight.Me.Models.First(x => x.Id == m.Id).LtsLeft, choppy.Me.Models.First(x => x.Id == m2.Id).LtsLeft);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void All_five_systems_disabled_keeps_era_h_fields_idle()
    {
        Game g = Lab(16, DisableEraH);
        g.Autopilot(60);
        Assert.Equal(0, g.Me.PgTech);
        Assert.Empty(g.Me.PodIds);
        Assert.Equal(0, g.Me.LicRegime);
        Assert.Equal(0, g.Me.LtsKept + g.Me.LtsBroken);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: false, eraHBounds: false);
    }

    static void DisableEraH(GameConfig c)
    {
        c.Programmes.Enabled = 0;
        c.RunQueue.Enabled = 0;
        c.Pods.Enabled = 0;
        c.Licence.Enabled = 0;
        c.Pledges.Enabled = 0;
    }

    [Fact]
    public void Programme_and_pod_together_survive_save_load()
    {
        Game g = Lab(17);
        FormPod(g);
        g.Do(Command.StartProgramme(g.Id, Tech));
        g.Do(Command.ChooseApproach(g.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(12);
        Simulation back = Simulation.Load(g.C, mid);
        HardeningInvariants.StepWeeksChecking(back, 12, eraBounds: true, eraBBounds: true, eraGBounds: true, eraHBounds: true);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Schema_105_through_109_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(18);
        FormPod(g);
        g.Do(Command.StartProgramme(g.Id, Tech));
        g.Do(Command.ChooseApproach(g.Id, 1));
        ModelState m = g.Train("mini", 0);
        g.Do(Command.SetRunPriority(g.Id, m.Id, 3));
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Alignment = 800;
        g.State.Tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        g.Do(Command.ApplyLicence(g.Id, 1));
        g.Do(Command.PledgeSupport(g.Id, m.Id, 26));
        string current = g.Sim.Save();
        for (int schema = 105; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true, eraHBounds: true);
        }
    }
}