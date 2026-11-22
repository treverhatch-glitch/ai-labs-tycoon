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

/// <summary>Save/load and edge coverage for devices, platforms, creed, shocks, and replication (schema 95–99).</summary>
public class EraFSystemsEdgeHardeningTests
{
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
        c.Standards.Enabled = 0;
        c.Listing.Enabled = 0;
        c.Rbf.Enabled = 0;
        c.Foundation.Enabled = 0;
        c.Leaseback.Enabled = 0;
        c.ConvertibleNotes.Enabled = 0;
        c.PlayerStart.GrantPerWeek = 0;
    }

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null, int year = 2027)
    {
        Game g = Game.Solo(seed, c =>
        {
            Quiet(c);
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        g.State.Tick = (year - 1995) * g.C.Calendar.WeeksPerYear;
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "edge" });
        g.Me.Reputation = 600;
        g.Me.ResearchPoints = 500_000;
        return g;
    }

    static void Strike(Game g, string id)
    {
        int idx = g.C.WorldEvents.FindIndex(e => e.Id == id);
        if (idx >= 0) Shocks.OnWorldEvent(g.C, g.State, idx);
    }

    [Fact]
    public void A_device_mid_save_load_matches_uninterrupted_design()
    {
        Game straight = Lab(11);
        straight.Do(Command.DesignDevice(straight.Id, 1, 1));
        straight.Weeks(6);
        Game choppy = Lab(11);
        choppy.Do(Command.DesignDevice(choppy.Id, 1, 1));
        choppy.Weeks(3);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        Assert.Equal(straight.Me.DevStage, choppy.Me.DevStage);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_platform_deal_mid_save_load_matches_renewal_week()
    {
        Game straight = Lab(12, c => c.Platforms.TermWeeks = 48);
        straight.Do(Command.SignPlatform(straight.Id, 1));
        straight.Weeks(12);
        Game choppy = Lab(12, c => c.Platforms.TermWeeks = 48);
        choppy.Do(Command.SignPlatform(choppy.Id, 1));
        choppy.Weeks(6);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(6);
        Assert.Equal(straight.Me.PlatLeft, choppy.Me.PlatLeft);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Creed_integrity_survives_save_load()
    {
        Game g = Lab(13);
        g.Do(Command.SetCreed(g.Id, Creed.Safety, Creed.Profit));
        g.Do(Command.DesignDevice(g.Id, 1, 0));
        g.Weeks(4);
        int integrity = g.Me.CreedIntegrity;
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(integrity, back.Player.CreedIntegrity);
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true);
    }

    [Fact]
    public void A_shock_mid_save_load_matches_answered_outcome()
    {
        Game straight = Lab(14);
        Strike(straight, "talent-war");
        straight.Do(Command.AnswerShock(straight.Id, 2));
        straight.Weeks(4);
        Game choppy = Lab(14);
        Strike(choppy, "talent-war");
        choppy.Do(Command.AnswerShock(choppy.Id, 2));
        choppy.Weeks(2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(2);
        Assert.Equal(straight.Me.ShockKind, choppy.Me.ShockKind);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_replication_run_mid_save_load_matches_release()
    {
        Game straight = Lab(15);
        straight.Me.ReplPending = 1;
        straight.Do(Command.ReleaseCode(straight.Id, 2));
        straight.Weeks(6);
        Game choppy = Lab(15);
        choppy.Me.ReplPending = 1;
        choppy.Do(Command.ReleaseCode(choppy.Id, 2));
        choppy.Weeks(3);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        Assert.Equal(straight.Me.ReplLeft, choppy.Me.ReplLeft);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void All_five_systems_disabled_keeps_era_f_fields_idle()
    {
        Game g = Lab(16, DisableEraF);
        g.Autopilot(80);
        Assert.Equal(0, g.Me.DevLine);
        Assert.Equal(0, g.Me.PlatPlat);
        Assert.Equal(0, g.Me.CreedA);
        Assert.Equal(0, g.Me.ShockKind);
        Assert.Equal(0, g.Me.ReplPending);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: false, eraCBounds: false, eraDBounds: false, eraEBounds: false, eraFBounds: false);
    }

    static void DisableEraF(GameConfig c)
    {
        c.Devices.Enabled = 0;
        c.Platforms.Enabled = 0;
        c.Creed.Enabled = 0;
        c.Shocks.Enabled = 0;
        c.Replication.Enabled = 0;
    }

    [Fact]
    public void Platform_refuses_without_reputation_and_signs_when_rich()
    {
        Game broke = Lab(17, c => c.Platforms.MinReputation = 900);
        broke.Me.Reputation = 100;
        broke.Refused(Command.SignPlatform(broke.Id, 1), CommandError.MissingPrerequisite);
        Game rich = Lab(18);
        rich.Do(Command.SignPlatform(rich.Id, 1));
        Assert.Equal(1, rich.Me.PlatPlat);
        HardeningInvariants.AssertBooksBalance(rich.C, rich.State);
    }

    [Fact]
    public void Schema_95_through_99_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(19);
        g.Do(Command.SetCreed(g.Id, Creed.Profit, Creed.Openness));
        g.Do(Command.SignPlatform(g.Id, 1));
        g.Do(Command.DesignDevice(g.Id, 1, 0));
        Strike(g, "pause-letter");
        if (Shocks.Pending(g.Me)) g.Do(Command.AnswerShock(g.Id, 2));
        g.Me.ReplPending = 1;
        g.Do(Command.ReleaseCode(g.Id, 0));
        string current = g.Sim.Save();
        for (int schema = 95; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true);
        }
    }
}