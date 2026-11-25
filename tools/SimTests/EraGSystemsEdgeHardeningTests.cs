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

/// <summary>Save/load and edge coverage for tech bets, waitlist, employer brand, free tier, and launch watch (schema 100–104).</summary>
public class EraGSystemsEdgeHardeningTests
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
        c.Devices.Enabled = 0;
        c.Platforms.Enabled = 0;
        c.PlayerStart.GrantPerWeek = 0;
    }

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            Quiet(c);
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "release" });
        g.Me.Reputation = 500;
        g.Me.ResearchPoints = 500_000;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 600, Status = ModelStatus.Training, Alignment = 500, Name = "train" };
        g.Me.Models.Add(m);
        g.UntilTrained(m);
        return m;
    }

    [Fact]
    public void A_tech_bet_mid_save_load_matches_uninterrupted_timer()
    {
        Game straight = Lab(11);
        straight.Do(Command.BetOnTech(straight.Id, "regression-tests"));
        straight.Weeks(6);
        Game choppy = Lab(11);
        choppy.Do(Command.BetOnTech(choppy.Id, "regression-tests"));
        choppy.Weeks(3);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        Assert.Equal(straight.Me.TbLeft, choppy.Me.TbLeft);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_waitlist_mid_save_load_matches_hype_growth()
    {
        Game straight = Lab(12);
        ModelState m = Trained(straight);
        straight.Do(Command.OpenWaitlist(straight.Id, m.Id, 12));
        straight.Weeks(8);
        Game choppy = Lab(12);
        ModelState m2 = Trained(choppy);
        choppy.Do(Command.OpenWaitlist(choppy.Id, m2.Id, 12));
        choppy.Weeks(4);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(4);
        Assert.Equal(straight.Me.WlHype, choppy.Me.WlHype);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Employer_brand_page_mid_save_load_keeps_score()
    {
        Game g = Lab(13);
        g.Do(Command.OpenCareersPage(g.Id));
        g.Do(Command.BetOnTech(g.Id, "regression-tests"));
        g.Research("regression-tests");
        g.Weeks(2);
        int score = g.Me.BrandScore;
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(score, back.Player.BrandScore);
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true);
    }

    [Fact]
    public void A_free_tier_mid_save_load_matches_reach()
    {
        Game straight = Lab(14);
        straight.Do(Command.OpenFreeTier(straight.Id, 2));
        straight.Weeks(10);
        Game choppy = Lab(14);
        choppy.Do(Command.OpenFreeTier(choppy.Id, 2));
        choppy.Weeks(5);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(5);
        Assert.Equal(straight.Me.FtReach, choppy.Me.FtReach);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void A_launch_watch_mid_save_load_matches_desk_state()
    {
        Game straight = Lab(15);
        straight.Do(Command.OpenWatchDesk(straight.Id));
        straight.Weeks(6);
        Game choppy = Lab(15);
        choppy.Do(Command.OpenWatchDesk(choppy.Id));
        choppy.Weeks(3);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        Assert.Equal(straight.Me.LwOn, choppy.Me.LwOn);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void All_five_era_g_systems_together_survive_mid_save_load()
    {
        Game g = Lab(16);
        ModelState m = Trained(g);
        g.Do(Command.BetOnTech(g.Id, "regression-tests"));
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 10));
        g.Do(Command.OpenCareersPage(g.Id));
        g.Do(Command.OpenFreeTier(g.Id, 2));
        g.Do(Command.OpenWatchDesk(g.Id));
        string mid = g.Sim.Save();
        g.Weeks(12);
        Simulation back = Simulation.Load(g.C, mid);
        HardeningInvariants.StepWeeksChecking(back, 12, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void All_five_systems_disabled_keeps_era_g_fields_idle()
    {
        Game g = Lab(17, DisableEraG);
        g.Autopilot(80);
        Assert.Equal(0, g.Me.TbRef);
        Assert.Equal(0, g.Me.WlModel);
        Assert.Equal(0, g.Me.BrandOn);
        Assert.Equal(0, g.Me.FtLevel);
        Assert.Equal(0, g.Me.LwOn);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: false, eraCBounds: false, eraDBounds: false, eraEBounds: false, eraFBounds: false, eraGBounds: false);
    }

    static void DisableEraG(GameConfig c)
    {
        c.TechBets.Enabled = 0;
        c.Waitlist.Enabled = 0;
        c.Brand.Enabled = 0;
        c.FreeTier.Enabled = 0;
        c.LaunchWatch.Enabled = 0;
    }

    [Fact]
    public void Waitlist_refuses_without_model_and_opens_when_training()
    {
        Game empty = Lab(18);
        empty.Me.Models.Clear();
        empty.Refused(Command.OpenWaitlist(empty.Id, 99999, 10), CommandError.UnknownId);
        Game g = Lab(19);
        ModelState m = AddTraining(g);
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 8));
        Assert.True(g.Me.WlModel > 0);
        HardeningInvariants.AssertBooksBalance(g.C, g.State);
    }

    static ModelState AddTraining(Game g)
    {
        ModelState m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 600, Status = ModelStatus.Training, Alignment = 500, Name = "wait" };
        g.Me.Models.Add(m);
        return m;
    }

    [Fact]
    public void Schema_100_through_104_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(20);
        ModelState m = Trained(g);
        g.Do(Command.BetOnTech(g.Id, "regression-tests"));
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 10));
        g.Do(Command.OpenCareersPage(g.Id));
        g.Do(Command.OpenFreeTier(g.Id, 1));
        g.Do(Command.OpenWatchDesk(g.Id));
        string current = g.Sim.Save();
        for (int schema = 100; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true, eraCBounds: true, eraDBounds: true, eraEBounds: true, eraFBounds: true, eraGBounds: true);
        }
    }
}