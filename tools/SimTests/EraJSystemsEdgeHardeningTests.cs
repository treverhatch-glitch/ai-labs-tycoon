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

/// <summary>Save/load and edge coverage for datasets, user data, governance, buyback votes, and spot capacity (schema 115–119).</summary>
public class EraJSystemsEdgeHardeningTests
{
    static void OpenFunding(GameConfig c)
    {
        foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
    }

    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            OpenFunding(c);
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 500_000_000;
        g.Me.Reputation = 600;
        g.Me.ResearchPoints = 500_000;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    static void DisableEraJ(GameConfig c)
    {
        c.Datasets.Enabled = 0;
        c.UserData.Enabled = 0;
        c.Governance.Enabled = 0;
        c.Spot.Enabled = 0;
    }

    [Fact]
    public void Data_cleaning_job_mid_save_load_matches_timer()
    {
        Game straight = Lab(31);
        straight.Do(Command.AcquireData(straight.Id, 1, 50));
        straight.Do(Command.CleanData(straight.Id, 1, 40));
        straight.Weeks(4);
        Game choppy = Lab(31);
        choppy.Do(Command.AcquireData(choppy.Id, 1, 50));
        choppy.Do(Command.CleanData(choppy.Id, 1, 40));
        choppy.Weeks(2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(2);
        Assert.Equal(straight.Me.DataJobLeft, choppy.Me.DataJobLeft);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void User_data_policy_mid_save_load_matches_banked_units()
    {
        Game g = Lab(32);
        Selling(g);
        g.Do(Command.SetUserData(g.Id, 2));
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        Assert.Equal(g.Me.UserPolicy, back.Player.UserPolicy);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Governance_choice_mid_save_load_matches_form()
    {
        Game g = Lab(33);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.SetGovernance(g.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(g.Me.GovForm, back.Player.GovForm);
    }

    [Fact]
    public void Spot_share_mid_save_load_matches_permille()
    {
        Game straight = Lab(34);
        straight.Do(Command.SetSpotShare(straight.Id, 400));
        straight.Weeks(5);
        Game choppy = Lab(34);
        choppy.Do(Command.SetSpotShare(choppy.Id, 400));
        choppy.Weeks(2);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(3);
        Assert.Equal(straight.Me.SpotPermille, choppy.Me.SpotPermille);
    }

    [Fact]
    public void Round_buyback_vote_mid_save_load_keeps_board_counters()
    {
        Game g = Lab(35);
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Do(Command.RaiseFunding(g.Id, "series-a"));
        g.Do(Command.BuyBackRound(g.Id, "angel"));
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        Assert.Equal(g.Me.BoardWon, back.Player.BoardWon);
    }

    [Fact]
    public void All_era_j_tools_disabled_autopilot_matches_digest_without_commands()
    {
        Game off = Lab(36, DisableEraJ);
        off.Autopilot(48);
        Game plain = Lab(36, DisableEraJ);
        plain.Autopilot(48);
        Assert.Equal(plain.Sim.Digest(), off.Sim.Digest());
        Assert.Empty(SaveGame.Problems(off.C, off.State));
    }

    [Fact]
    public void Schema_115_through_119_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(37);
        Selling(g);
        g.Do(Command.AcquireData(g.Id, 1, 60));
        g.Do(Command.CleanData(g.Id, 1, 40));
        g.Do(Command.SetUserData(g.Id, 2));
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.SetGovernance(g.Id, 1));
        g.Do(Command.SetSpotShare(g.Id, 300));
        string current = g.Sim.Save();
        for (int schema = 115; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            sim.StepMany(20);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            if (schema >= 115) HardeningInvariants.AssertEraJStateBounds(g.C, sim.Player);
        }
    }
}