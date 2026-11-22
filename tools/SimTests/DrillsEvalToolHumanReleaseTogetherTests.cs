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

public class DrillsEvalToolHumanReleaseTogetherTests
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
        c.Drills.MinEra = 0;
        c.EvalHarness.MinEra = 0;
        c.ToolPolicy.MinEra = 0;
        c.HumanQueue.MinEra = 0;
        c.HumanQueue.BusyWeeks = 1;
        c.ReleaseChecklist.Enabled = 0;
        c.ToolPolicy.Enabled = 0;
        c.Agency.MinEra = 0;
        c.BoardAgenda.Enabled = 1;
    }

    static Game Lab(Action<GameConfig> more = null)
    {
        Game g = Game.Solo(12, c =>
        {
            Quiet(c);
            if (more != null) more(c);
        }).WithStarterLab(researchers: 3, engineers: 4, safety: 3, sales: 2, gpus: 6);
        g.Build("support-desk", 3, 3);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }


    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Fact]
    public void High_drill_prep_stacks_with_on_call_on_incident_reputation_loss()
    {
        Game g = Lab(c =>
        {
            c.Drills.OnCallRepStack = 200;
            c.Drills.RepCut = 0;
            c.OnCall.ReputationCutPermillePerLevel = 100;
            c.OnCall.MaxLevel = 2;
        });
        g.Do(Command.SetOnCall(g.Id, 2));
        g.Me.DrillPrep = 500;
        int stacked = OnCall.ReputationCutPermille(g.C, g.Me);
        Assert.Equal(200 + MathI.MulPermille(200, 500), stacked);
        g.Me.DrillPrep = 0;
        Assert.Equal(200, OnCall.ReputationCutPermille(g.C, g.Me));
        Game off = Lab(c => c.Drills.Enabled = 0);
        off.Do(Command.SetOnCall(off.Id, 2));
        Assert.Equal(200, OnCall.ReputationCutPermille(off.C, off.Me));
    }

    [Fact]
    public void Tool_policy_on_agents_fills_the_human_queue_and_cuts_lift_past_capacity()
    {
        Game g = Lab(c => { c.ToolPolicy.Enabled = 1; c.HumanQueue.Capacity = 5; c.HumanQueue.LiftCut = 100; c.Agency.Cooldown = 0; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Code | ToolPolicy.Data));
        g.Weeks(1);
        Assert.True(Agency.Lift(g.C, g.State, g.Me) > 0);
        g.Me.HumanQueue = 8;
        Assert.True(HumanQueue.LiftKeep(g.C, g.Me) < MathI.Permille);
        Agency.RunWeek(g.C, g.State, g.Me);
        Assert.True(g.Me.HumanQueue > 8);
    }

    [Fact]
    public void A_weak_public_eval_under_a_meeting_board_adds_tension()
    {
        Game g = Lab(c => { c.EvalHarness.BarNarrow = 900; c.EvalHarness.BarBroad = 900; c.EvalHarness.FailTension = 70; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.Capability = 100;
        m.Alignment = 100;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Me.BoardInvestorSeats = 1;
        g.Do(Command.HoldBoardMeetings(g.Id, 1));
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        g.Weeks(g.C.EvalHarness.Cadence);
        Assert.Equal(g.C.EvalHarness.FailTension, g.Me.BoardTension);
    }

    [Fact]
    public void Releasing_without_every_checklist_item_costs_trust_each_week()
    {
        Game g = Lab(c => { c.ReleaseChecklist.Enabled = 1; c.ReleaseChecklist.MissTrust = -4; c.ReleaseChecklist.MinEra = c.Eras.Count; });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.ChkMissed = ReleaseChecklist.Red | ReleaseChecklist.Eval;
        g.C.ReleaseChecklist.MinEra = 0;
        int trust = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        ReleaseChecklist.RunWeek(g.C, g.State, g.Me);
        Assert.True(TrustSystem.Of(g.C, g.Me, m.Sectors[0]) <= trust - 8);
    }

    [Fact]
    public void Nothing_changes_for_a_game_that_uses_none_of_them()
    {
        Game g = Game.New(5, c =>
        {
            c.Drills.Enabled = 0;
            c.EvalHarness.Enabled = 0;
            c.ToolPolicy.Enabled = 0;
            c.HumanQueue.Enabled = 0;
            c.ReleaseChecklist.Enabled = 0;
        });
        g.Autopilot(60);
        Assert.Equal(0, g.Me.DrillPrep + g.Me.HumanQueue + g.Me.Models.Sum(m => m.EvalSuite + m.ToolMask + m.ChkDone + m.ChkMissed));
        foreach (EventKind k in new[] { EventKind.DrillStarted, EventKind.EvalPublished, EventKind.ToolPolicySet, EventKind.HumanQueueDeep, EventKind.ChecklistSigned })
            Assert.Equal(0, Count(g, k));
    }

    [Fact]
    public void A_bankrupt_lab_that_spectates_still_saves_cleanly_with_every_new_system_in_use()
    {
        Game g = Lab(c => { c.ToolPolicy.Enabled = 1; c.ReleaseChecklist.Enabled = 1; c.ReleaseChecklist.MinEra = c.Eras.Count; });
        g.Do(Command.StartDrill(g.Id));
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        m.RedTeamUntil = g.State.Tick + g.C.RedTeam.Weeks;
        m.DocLevel = g.C.ReleaseChecklist.DocBar;
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Red));
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Docs));
        m.ToolMask = 1;
        m.AgentLevel = 1;
        g.Do(Command.SignChecklistItem(g.Id, m.Id, ReleaseChecklist.Policy));
        g.C.ReleaseChecklist.Enabled = 0;
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.C.ReleaseChecklist.Enabled = 1;
        g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 8, Y = 0, Width = 4, Height = 4 });
        g.Do(Command.SetAutonomy(g.Id, m.Id, 2));
        g.Do(Command.SetToolPolicy(g.Id, m.Id, ToolPolicy.Web));
        g.Hire("support", 2);
        g.Me.HumanQueue = 6;
        g.Do(Command.StaffHumanQueue(g.Id, 2));
        g.Me.GrantPerWeek = 0;
        g.Me.Cash = -50_000;
        g.Me.WeeksInDebt = g.C.Finance.BankruptcyGraceWeeks - 1;
        g.Weeks(1);
        int idx = g.State.Endings.FindIndex(e => e.EndingId == "bankrupt");
        Assert.True(idx >= 0);
        g.Do(Command.ContinueAfterEnding(g.Me.Id, idx, "spectate"));
        Assert.Equal(-1, g.State.PlayerLabId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void A_long_scripted_game_uses_every_system_and_keeps_the_books(int seed)
    {
        Game g = Game.New(seed, c => { c.PlayerStart.Cash = 1_000_000_000_000; Quiet(c); });
        g.Build("support-desk", 4, 4);
        g.Autopilot(40);
        g.Me.BoardInvestorSeats = 1;
        var done = new Dictionary<string, int>();
        Action<string, bool> tick = (k, ok) => { if (ok) done[k] = (done.ContainsKey(k) ? done[k] : 0) + 1; };
        g.Hire("support", 2);
        for (int week = 0; week < 200 && !g.Me.Bankrupt; week++)
        {
            g.Me.ResearchPoints += 10_000;
            if (week == 8 || week == 120) tick("drill", g.Try(Command.StartDrill(g.Id)).Ok);
            if (week % 7 == 2) g.Try(Command.StartTraining(g.Id, "mini", 0));
            ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Trained);
            ModelState sold = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Released);
            if (m != null && week % 11 == 3)
            {
                tick("eval", g.Try(Command.RegisterEval(g.Id, m.Id, 1)).Ok);
                tick("release", g.Try(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai")).Ok);
            }
            if (sold != null && week % 9 == 4)
            {
                g.Me.Rooms.Add(new RoomState { Id = g.State.TakeId(), TypeId = "containment-wing", X = 20 + week, Y = 0, Width = 4, Height = 4 });
                tick("agents", g.Try(Command.SetAutonomy(g.Id, sold.Id, 2)).Ok);
                tick("policy", g.Try(Command.SetToolPolicy(g.Id, sold.Id, ToolPolicy.Web | ToolPolicy.Data)).Ok);
            }
            if (g.Me.HumanQueue > 0 && week % 5 == 0) tick("queue", g.Try(Command.ClearHumanQueue(g.Id, 1)).Ok);
            if (week == 15) g.Try(Command.HoldBoardMeetings(g.Id, 1));
            g.Autopilot(1);
        }
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        foreach (string k in new[] { "eval", "release" })
            Assert.True(done.ContainsKey(k), "never used: " + k);
    }
}