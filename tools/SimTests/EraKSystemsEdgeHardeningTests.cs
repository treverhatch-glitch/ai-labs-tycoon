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

/// <summary>Save/load and edge coverage for annotation, post-training, knowledge, board agenda, and agency (schema 120–124).</summary>
public class EraKSystemsEdgeHardeningTests
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
            c.PostTraining.MinEra = 0;
            c.Agency.MinEra = 0;
            OpenFunding(c);
            c.PlayerStart.Cash = 50_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 500_000_000;
        g.Me.Reputation = 600;
        g.Me.BoardInvestorSeats = 1;
        g.Me.DataClean1 = 200;
        g.Me.DataClean4 = 500;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        return m;
    }

    static int Clean(Game g, int units)
    {
        g.Do(Command.AcquireData(g.Id, 1, units));
        g.Do(Command.CleanData(g.Id, 1, units));
        g.Weeks(g.C.Datasets.CleanWeeks);
        return g.Me.DataClean1;
    }

    static void DisableEraK(GameConfig c)
    {
        c.Annotation.Enabled = 0;
        c.PostTraining.Enabled = 0;
        c.Knowledge.Enabled = 0;
        c.BoardAgenda.Enabled = 0;
        c.Agency.Enabled = 0;
    }

    [Fact]
    public void Annotator_contract_mid_save_load_matches_vendor_and_cleaned_units()
    {
        Game g = Lab(41);
        Clean(g, 60);
        g.Do(Command.ContractAnnotators(g.Id, 2));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(4);
        Assert.Equal(g.Me.AnnVendor, back.Player.AnnVendor);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Post_training_mid_save_load_matches_kind_and_counters()
    {
        Game g = Lab(42);
        ModelState m = Trained(g);
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(g.C.PostTraining.Weeks);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(g.C.PostTraining.Weeks);
        Assert.Equal(g.Me.Models[0].PostPref, back.Player.Models[0].PostPref);
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Owner_and_docs_mid_save_load_match_model_fields()
    {
        Game g = Lab(43);
        ModelState m = Trained(g);
        StaffMember r = g.Me.Staff.First(s => s.RoleId == "researcher");
        g.Do(Command.AssignOwner(g.Id, m.Id, r.Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        string mid = g.Sim.Save();
        g.Weeks(6);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(6);
        Assert.Equal(m.DocLevel, back.Player.Models[0].DocLevel);
        Assert.Equal(m.OwnerId, back.Player.Models[0].OwnerId);
    }

    [Fact]
    public void Board_meetings_mid_save_load_matches_agenda_counters()
    {
        Game g = Lab(44);
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(8);
        Assert.Equal(g.Me.MeetingsOn, back.Player.MeetingsOn);
        Assert.Equal(g.Me.BoardTension, back.Player.BoardTension);
    }

    [Fact]
    public void All_era_k_tools_disabled_autopilot_matches_digest_without_commands()
    {
        Game off = Lab(45, DisableEraK);
        off.Autopilot(48);
        Game plain = Lab(45, DisableEraK);
        plain.Autopilot(48);
        Assert.Equal(plain.Sim.Digest(), off.Sim.Digest());
        Assert.Empty(SaveGame.Problems(off.C, off.State));
    }

    [Fact]
    public void Schema_120_through_124_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab(46);
        Clean(g, 70);
        ModelState m = Trained(g);
        g.Do(Command.ContractAnnotators(g.Id, 1));
        g.Do(Command.PostTrain(g.Id, m.Id, 1));
        g.Weeks(g.C.PostTraining.Weeks);
        StaffMember r = g.Me.Staff.First(s => s.RoleId == "researcher");
        g.Do(Command.AssignOwner(g.Id, m.Id, r.Id));
        g.Do(Command.WriteDocs(g.Id, m.Id));
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.RaiseFunding(g.Id, "angel"));
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        string current = g.Sim.Save();
        for (int schema = 120; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            sim.StepMany(20);
            Assert.Empty(SaveGame.Problems(g.C, sim.State));
            if (schema >= 115) HardeningInvariants.AssertEraJStateBounds(g.C, sim.Player);
            if (schema >= 120) HardeningInvariants.AssertEraKStateBounds(g.C, sim.Player);
        }
    }

    [Fact]
    public void Released_model_with_post_training_survives_save_load()
    {
        Game g = Lab(47);
        ModelState m = Trained(g);
        g.Do(Command.PostTrain(g.Id, m.Id, 2));
        g.Weeks(g.C.PostTraining.Weeks);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        back.StepMany(8);
        Assert.Equal(g.Me.Models[0].PostPref, back.Player.Models[0].PostPref);
        Assert.Empty(SaveGame.Problems(g.C, back.State));
    }
}