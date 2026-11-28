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

/// <summary>Save/load and edge coverage for integrations, conformity, org, fabric, and incident register (schema 110–114).</summary>
public class EraISystemsEdgeHardeningTests
{
    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.Office.ChancePermille = 0;
        c.Safety.IncidentChancePermille = 0;
        c.Support.Enabled = 0;
        c.Litigation.Enabled = 0;
        c.Fabric.EraA = 0;
        c.Fabric.EraB = 0;
        c.Fabric.EraC = 0;
    }

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            Quiet(c);
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.Me.ResearchPoints = 500_000;
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, 10);
        return g;
    }

    static ModelState Selling(Game g, string sector = "game-ai")
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, sector));
        m.Alignment = 800;
        return m;
    }

    static void DisableEraI(GameConfig c)
    {
        c.Integrations.Enabled = 0;
        c.Conformity.Enabled = 0;
        c.Org.Enabled = 0;
        c.Fabric.Enabled = 0;
        c.Disclosure.Enabled = 0;
    }

    [Fact]
    public void Open_api_mid_save_load_matches_integrations_growth()
    {
        Game straight = Lab(21);
        ModelState m = Selling(straight);
        straight.Do(Command.OpenApi(straight.Id, m.Id));
        straight.Weeks(6);
        Game choppy = Lab(21);
        ModelState m2 = Selling(choppy);
        choppy.Do(Command.OpenApi(choppy.Id, m2.Id));
        choppy.Weeks(3);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        ModelState sm = straight.Me.Models.First(x => x.Id == m.Id);
        ModelState cm = choppy.Me.Models.First(x => x.Id == m2.Id);
        Assert.Equal(sm.Integrations, cm.Integrations);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Conformity_dossier_mid_save_load_matches_stage()
    {
        Game g = Lab(22);
        ModelState m = Selling(g);
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        g.Do(Command.StartConformity(g.Id, m.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(2);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(2);
        ModelState after = g.Me.Models.First(x => x.Id == m.Id);
        ModelState reloaded = back.Player.Models.First(x => x.Id == m.Id);
        Assert.Equal(after.CertStage, reloaded.CertStage);
        Assert.Equal(after.CertLeft, reloaded.CertLeft);
    }

    [Fact]
    public void Org_change_mid_save_load_matches_disruption_timer()
    {
        Game straight = Lab(23);
        straight.Do(Command.SetOrg(straight.Id, Org.Functional));
        straight.Weeks(4);
        Game choppy = Lab(23);
        choppy.Do(Command.SetOrg(choppy.Id, Org.Functional));
        choppy.Weeks(2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(2);
        Assert.Equal(straight.Me.OrgDisrupt, choppy.Me.OrgDisrupt);
        Assert.Equal(straight.Me.OrgForm, choppy.Me.OrgForm);
    }

    [Fact]
    public void Fabric_build_mid_save_load_matches_lead_timer()
    {
        Game straight = Lab(24);
        straight.Do(Command.OrderFabric(straight.Id));
        straight.Weeks(5);
        Game choppy = Lab(24);
        choppy.Do(Command.OrderFabric(choppy.Id));
        choppy.Weeks(2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        Assert.Equal(straight.Me.FabLeft, choppy.Me.FabLeft);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Register_join_and_report_mid_save_load_matches_open_cases()
    {
        Game g = Lab(25);
        ModelState m = Selling(g);
        g.Do(Command.JoinRegister(g.Id));
        Disclosure.OnIncident(g.C, g.Me, m);
        g.Do(Command.FileReport(g.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(g.Me.DiscOpen, back.Player.DiscOpen);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void All_era_i_tools_disabled_autopilot_matches_digest_without_commands()
    {
        Game off = Lab(26, DisableEraI);
        off.Autopilot(48);
        Game plain = Lab(26, DisableEraI);
        plain.Autopilot(48);
        Assert.Equal(plain.Sim.Digest(), off.Sim.Digest());
        Assert.Empty(SaveGame.Problems(off.C, off.State));
    }

    [Fact]
    public void Schema_110_through_114_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(28);
        ModelState m = Selling(g);
        ModelState next = Selling(g, "game-bots");
        g.Do(Command.OpenApi(g.Id, m.Id));
        g.Do(Command.SetOrg(g.Id, Org.Squads));
        g.Do(Command.OrderFabric(g.Id));
        g.Do(Command.JoinRegister(g.Id));
        next.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        g.Do(Command.StartConformity(g.Id, next.Id, 1));
        Disclosure.OnIncident(g.C, g.Me, m);
        g.Do(Command.FileReport(g.Id, 1));
        string current = g.Sim.Save();
        for (int schema = 110; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true, eraHBounds: true, eraIBounds: true);
        }
    }
}