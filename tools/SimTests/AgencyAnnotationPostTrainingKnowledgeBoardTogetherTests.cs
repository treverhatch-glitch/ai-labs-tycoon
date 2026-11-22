using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class AgencyAnnotationPostTrainingKnowledgeBoardTogetherTests
{
    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.Office.ChancePermille = 0;
        c.Safety.IncidentChancePermille = 0;
        c.Safety.SectorFlavours.Clear();
        c.Chiefs.MinStaff = 0;
        c.Support.Enabled = 0;
        c.Security.Enabled = 0;
        c.Tax.Enabled = 0;
        c.Standards.Enabled = 0;
        c.Reliability.Enabled = 0;
        c.GovContracts.Enabled = 0;
            c.Shocks.Enabled = 0;
            c.AgentSandbox.Enabled = 0;
            c.RunawayPlaybook.Enabled = 0;
            c.OrchestrationBudget.Enabled = 0;
            c.TrafficRouter.Enabled = 0;
            c.PublicMetrics.Enabled = 0;
            c.Agency.MinEra = 0;
        c.PostTraining.MinEra = 0;
        foreach (FundingRoundConfig r in c.Funding.Rounds) { r.OpensYear = 1995; r.MinReputation = 0; }
    }

    static Game Lab(Action<GameConfig> more = null)
    {
        Game g = Game.Solo(4, c =>
        {
            Quiet(c);
            if (more != null) more(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 2, sales: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        g.Me.Reputation = 600;
        g.Me.BoardInvestorSeats = 1;
        return g;
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void A_knowledge_gap_makes_the_agents_of_a_model_blow_up_more_often()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        int sure = Agency.BlowupChance(g.C, g.Me, m);
        m.KnowGap = 1000;
        int gap = Agency.BlowupChance(g.C, g.Me, m);
        Assert.Equal((g.C.Agency.Blowup2 + g.C.Agency.GapBlowup) * (1000 - g.C.Agency.RoomCut) / 1000, gap);
        Assert.True(gap > sure);
        Game off = Lab(c => c.Knowledge.Enabled = 0);
        ModelState om = Selling(off);
        off.Me.Rooms.Add(new RoomState { Id = off.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        off.Do(Command.SetAutonomy(off.Id, om.Id, 2));
        om.KnowGap = 1000;
        Assert.Equal(sure, Agency.BlowupChance(off.C, off.Me, om));
    }

    [Fact]
    public void A_vendor_scandal_adds_tension_to_a_board_that_meets_and_directors_soften_it()
    {
        Game g = Lab(c => c.Annotation.ExposeDiv = 1);
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Do(Command.ContractAnnotators(g.Id, 1));
        g.Me.AnnExposure = 1000;
        Annotation.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(g.C.Annotation.ScandalTension, g.Me.BoardTension);

        Game led = Lab(c => c.Annotation.ExposeDiv = 1);
        led.Do(Command.HoldBoardMeetings(led.Id, 1));
        led.Do(Command.ContractAnnotators(led.Id, 1));
        led.Me.GovDirectors = 2;
        led.Me.AnnExposure = 1000;
        Annotation.RunWeek(led.C, led.State, led.Me);
        Assert.Equal(led.C.Annotation.ScandalTension * (1000 - 2 * led.C.BoardAgenda.DirectorCut) / 1000, led.Me.BoardTension);

        Game quiet = Lab(c => c.Annotation.ExposeDiv = 1);
        quiet.Do(Command.ContractAnnotators(quiet.Id, 1));
        quiet.Me.AnnExposure = 1000;
        Annotation.RunWeek(quiet.C, quiet.State, quiet.Me);
        Assert.Equal(1, quiet.Me.AnnStrikes);
        Assert.Equal(0, quiet.Me.BoardTension);
    }

    [Fact]
    public void An_agent_blow_up_adds_tension_to_a_board_that_meets()
    {
        Game g = Lab(c => { c.Agency.Blowup2 = 1000; c.Agency.Blowup3 = 1000; c.Agency.RoomCut = 0; });
        ModelState m = Selling(g);
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Weeks(1);
        Assert.Equal(1, m.AgentBlowups);
        Assert.Equal(g.C.BoardAgenda.BlowupTension, g.Me.BoardTension);

        Game quiet = Lab(c => { c.Agency.Blowup2 = 1000; c.Agency.Blowup3 = 1000; c.Agency.RoomCut = 0; });
        ModelState qm = Selling(quiet);
        quiet.Me.Rooms.Add(new RoomState { Id = quiet.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        quiet.Do(Command.SetAutonomy(quiet.Id, qm.Id, 2));
        quiet.Weeks(1);
        Assert.Equal(1, qm.AgentBlowups);
        Assert.Equal(0, quiet.Me.BoardTension);
    }

    [Fact]
    public void Each_level_of_documentation_takes_a_share_off_the_failures_preference_tuning_teaches()
    {
        Game g = Lab();
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.PostPref = 3;
        int plain = PostTraining.IncidentPermille(g.C, m);
        Assert.Equal(3 * g.C.PostTraining.Syco, plain);
        m.DocLevel = 2;
        Assert.Equal(plain * (1000 - 2 * g.C.PostTraining.DocsCutSyco) / 1000, PostTraining.IncidentPermille(g.C, m));
        m.DocLevel = 3;
        Assert.True(PostTraining.IncidentPermille(g.C, m) < plain);
        Game off = Lab(c => c.Knowledge.Enabled = 0);
        ModelState om = off.Train("mini", 0);
        off.UntilTrained(om);
        om.PostPref = 3;
        om.DocLevel = 2;
        Assert.Equal(plain, PostTraining.IncidentPermille(off.C, om));
    }

    [Fact]
    public void Nothing_changes_for_a_game_that_uses_none_of_them()
    {
        Game g = Game.New(5, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        Assert.Equal(0, g.Me.AnnVendor + g.Me.AnnExposure + g.Me.AnnStrikes + g.Me.AnnCleaned + g.Me.MeetingsOn + g.Me.BoardTension + g.Me.MotionsTabled + (int)g.Me.AgentEarned);
        Assert.Equal(0, g.Me.Models.Sum(m => m.AgentLevel + m.AgentBlowups + m.PostKind + m.PostPref + m.PostSafety + m.OwnerId + m.DocLevel + m.KnowGap + m.Handovers));
        foreach (EventKind k in new[] { EventKind.AutonomySet, EventKind.AgentBlowup, EventKind.AnnotatorsContracted, EventKind.VendorScandal, EventKind.PostTrainingStarted, EventKind.OwnerNamed, EventKind.OwnerLeft, EventKind.MotionTabled, EventKind.BoardForcedVote })
            Assert.Equal(0, Count(g, k));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab();
        ModelState sold = Selling(g);
        ModelState tuned = g.Train("mini", 0);
        g.UntilTrained(tuned);
        g.Me.DataClean4 = 200;
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, sold.Id, 2));
        g.Do(Command.ContractAnnotators(g.Id, 1));
        g.Do(Command.PostTrain(g.Id, tuned.Id, 1));
        StaffMember r = g.Me.Staff.First(p => p.RoleId == "researcher");
        g.Do(Command.AssignOwner(g.Id, sold.Id, r.Id));
        g.Do(Command.WriteDocs(g.Id, sold.Id));
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Weeks(g.C.BoardAgenda.MeetingWeeks);
        g.Do(Command.AnswerMotion(g.Id, 1));
        g.Weeks(4);
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        Assert.True(idx >= 0);
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(20);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_owner_who_leaves_an_agent_model_a_blow_up_and_the_board_play_out_in_order()
    {
        Game g = Lab(c => { c.Agency.Blowup2 = 0; c.Agency.RoomCut = 0; c.Agency.Cooldown = 0; });
        ModelState m = Selling(g);
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        StaffMember r = g.Me.Staff.First(p => p.RoleId == "researcher");
        g.Do(Command.AssignOwner(g.Id, m.Id, r.Id));
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Do(Command.Fire(g.Id, "researcher", g.Me.Staff.Count(p => p.RoleId == "researcher")));
        Assert.Equal(g.C.Knowledge.GapBase, m.KnowGap);
        Assert.Equal(g.C.Agency.GapBlowup * g.C.Knowledge.GapBase / 1000, Agency.BlowupChance(g.C, g.Me, m));
        Assert.True(Knowledge.Untouchable(g.C, m));
        g.Refused(Command.SetAutonomy(g.Id, m.Id, 3), CommandError.MissingPrerequisite);
        g.Do(Command.AssignOwner(g.Id, m.Id, g.Me.Staff.First(p => p.RoleId == "engineer").Id));
        g.Weeks(30);
        Assert.Equal(0, m.KnowGap);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void All_five_systems_in_a_long_scripted_game_keep_the_books_the_save_and_the_replay(int seed)
    {
        Game g = Game.New(seed, c => c.PlayerStart.Cash = 1_000_000_000_000);
        g.Autopilot(60);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, 12);
        g.State.Tick = (2025 - 1995) * g.C.Calendar.WeeksPerYear;
        g.Me.Reputation = 800;
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 40 + seed, Y = 40, Width = 4, Height = 4 });
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 50 + seed, Y = 40, Width = 4, Height = 4 });
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        var done = new Dictionary<string, int>();
        Action<string, bool> tick = (k, ok) => { if (ok) done[k] = (done.ContainsKey(k) ? done[k] : 0) + 1; };
        string[] roundIds = g.C.Funding.Rounds.Select(r => r.Id).ToArray();
        for (int week = 0; week < 300 && !g.Me.Bankrupt; week++)
        {
            g.Me.ResearchPoints += 20_000;
            if (week % 25 == 3) g.Try(Command.StartTraining(g.Id, "mini", 0));
            if (week == 4) tick("raise", g.Try(Command.RaiseFunding(g.Id, roundIds[0])).Ok);
            if (week % 6 == 1) g.Me.DataClean4 += 40;
            if (week % 6 == 2) g.Me.DataClean2 += 40;
            ModelState sold = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Released);
            ModelState fresh = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Trained);
            if (fresh != null && week % 5 == 0) tick("tune", g.Try(Command.PostTrain(g.Id, fresh.Id, 1 + week / 5 % 2)).Ok);
            if (fresh != null && week % 11 == 4) tick("release", g.Try(Command.ReleaseModel(g.Id, fresh.Id, 0, "game-ai")).Ok);
            if (sold != null && week % 13 == 3) tick("agents", g.Try(Command.SetAutonomy(g.Id, sold.Id, 1 + week / 13 % 3)).Ok);
            if (week % 17 == 6) tick("vendor", g.Try(Command.ContractAnnotators(g.Id, 1 + week / 17 % 3)).Ok);
            if (week % 19 == 8) tick("audit", g.Try(Command.AuditVendor(g.Id)).Ok);
            if (week % 47 == 9) tick("endvendor", g.Try(Command.EndAnnotators(g.Id)).Ok);
            StaffMember person = g.Me.Staff.FirstOrDefault(p => p.RoleId == "researcher" || p.RoleId == "engineer");
            ModelState any = g.Me.Models.FirstOrDefault(m => m.Status != ModelStatus.Retired);
            if (any != null && person != null && week % 9 == 2) tick("owner", g.Try(Command.AssignOwner(g.Id, any.Id, person.Id)).Ok);
            if (any != null && week % 10 == 7) tick("docs", g.Try(Command.WriteDocs(g.Id, any.Id)).Ok);
            if (week == 11) g.Me.BoardInvestorSeats = 1;
            if (week == 12) tick("meetings", g.Try(Command.HoldBoardMeetings(g.Id, 1)).Ok);
            if (g.Me.MotionPend != 0) tick("answer", g.Try(Command.AnswerMotion(g.Id, week % 3 == 0 ? 0 : 1)).Ok);
            if (week % 70 == 30) g.Try(Command.Fire(g.Id, "researcher", 1));
            g.Autopilot(1);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        foreach (string k in new[] { "tune", "agents", "vendor", "owner", "docs", "meetings" })
            Assert.True(done.ContainsKey(k), "never used: " + k + " (" + string.Join(", ", done.Select(p => p.Key + "=" + p.Value)) + ")");
    }
}
