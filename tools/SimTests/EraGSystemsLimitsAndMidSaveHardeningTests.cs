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

/// <summary>Zero cash, max activity, and bankrupt edges for era-g systems with save/load mid-flight.</summary>
public class EraGSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null, long? cash = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Scaling.Enabled = 0;
            c.Charter.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Devices.Enabled = 0;
            c.Platforms.Enabled = 0;
            c.PlayerStart.GrantPerWeek = 0;
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 800, Status = ModelStatus.Released, Alignment = 500, Name = "limits" });
        g.Me.Reputation = 500;
        g.Me.ResearchPoints = 1_000_000;
        if (cash.HasValue) g.Me.Cash = cash.Value;
        return g;
    }

    [Fact]
    public void Free_tier_refuses_invalid_level()
    {
        Game g = Lab(10);
        g.Do(Command.OpenFreeTier(g.Id, 2));
        g.Refused(Command.OpenFreeTier(g.Id, 4), CommandError.InvalidArgument);
        g.Refused(Command.OpenFreeTier(g.Id, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_bankrupt_lab_with_every_era_g_tool_active_still_validates_save()
    {
        Game g = Lab(11);
        ModelState m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 600, Status = ModelStatus.Training, Alignment = 500, Name = "bust" };
        g.Me.Models.Add(m);
        g.UntilTrained(m);
        g.Do(Command.BetOnTech(g.Id, "regression-tests"));
        g.Do(Command.OpenWaitlist(g.Id, m.Id, 12));
        g.Do(Command.OpenCareersPage(g.Id));
        g.Do(Command.OpenFreeTier(g.Id, 1));
        g.Do(Command.OpenWatchDesk(g.Id));
        g.Me.Cash = -10_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks;
        g.Weeks(2);
        string save = g.Sim.Save();
        Simulation back = Simulation.Load(g.C, save);
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }

    [Fact]
    public void Max_free_tier_and_watch_desk_mid_save_match_digest()
    {
        Game straight = Lab(12);
        straight.Do(Command.OpenFreeTier(straight.Id, 3));
        straight.Do(Command.OpenWatchDesk(straight.Id));
        straight.Weeks(20);
        Game choppy = Lab(12);
        choppy.Do(Command.OpenFreeTier(choppy.Id, 3));
        choppy.Do(Command.OpenWatchDesk(choppy.Id));
        choppy.Weeks(10);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(10);
        Assert.Equal(straight.Me.FtReach, choppy.Me.FtReach);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Bet_and_brand_with_almost_no_cash_refuse_tier_but_keep_bet()
    {
        Game broke = Lab(13, cash: 1);
        broke.Do(Command.BetOnTech(broke.Id, "regression-tests"));
        Assert.True(broke.Me.TbRef > 0);
        broke.Do(Command.OpenFreeTier(broke.Id, 1));
        broke.Refused(Command.OpenFreeTier(broke.Id, 2), CommandError.InvalidArgument);
        Simulation back = Simulation.Load(broke.C, broke.Sim.Save());
        Assert.Equal(broke.Me.TbRef, back.Player.TbRef);
    }

    [Fact]
    public void Cross_era_e_listing_and_era_g_brand_survive_save_load()
    {
        Game g = Lab(14, c =>
        {
            c.Listing.MinWeeklyRevenue = 1;
            c.Listing.Enabled = 1;
        });
        ModelState m = g.Me.Models.First(x => x.Status == ModelStatus.Released);
        g.Me.Reputation = 700;
        g.Me.Ledger.LastWeek.Revenue = 100_000;
        g.Do(Command.ListCompany(g.Id));
        g.Do(Command.OpenCareersPage(g.Id));
        g.Do(Command.BetOnTech(g.Id, "regression-tests"));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(8);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true, eraCBounds: false, eraDBounds: false, eraEBounds: true, eraFBounds: false, eraGBounds: true);
    }
}