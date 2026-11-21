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

/// <summary>Chiefs, the ethics board, inference optimisation, marketing campaigns and the on-call rota used together over years.</summary>
public class ChiefsBoardOptimiseCampaignOnCallTogetherTests
{
    static Game Start(int seed)
    {
        return Game.New(seed, c =>
        {
            c.Chiefs.MinStaff = 0;
            c.Board.SafetyStaffPerLevel = 0;
            c.Optimise.EngineersPerLevel = 0;
            c.Marketing.MinSales = 0;
            c.OnCall.StaffPerLevel = 0;
            c.RedTeam.RoleId = "";
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Scaling.Enabled = 0;
        });
    }

    static void UseTheTools(Game g, int week)
    {
        if (week == 10 || week == 90)
        {
            StaffMember r = g.Me.Staff.FirstOrDefault(x => g.C.Role(x.RoleId).Function == RoleFunction.Research);
            if (r != null && g.Me.ChiefScientistId == -1) g.Try(Command.AppointChief(g.Id, Chiefs.Scientist, r.Id));
            StaffMember s = g.Me.Staff.FirstOrDefault(x => g.C.Role(x.RoleId).Function == RoleFunction.Safety);
            if (s != null && g.Me.ChiefSafetyId == -1) g.Try(Command.AppointChief(g.Id, Chiefs.Safety, s.Id));
        }
        if (week == 20 && g.Me.Cash > 200_000) g.Try(Command.SetEthicsBoard(g.Id, 1));
        if (week == 140 && g.Me.EthicsLevel > 0) g.Try(Command.SetEthicsBoard(g.Id, 0));
        if (week == 30 && g.Me.Cash > 200_000) g.Try(Command.SetOptimisation(g.Id, 2));
        if (week % 52 == 15 && g.Me.Cash > 400_000)
        {
            SectorState sector = g.State.Sectors.FirstOrDefault(s => s.Entry(g.Id) != null);
            if (sector != null) g.Try(Command.RunCampaign(g.Id, sector.Id, 2));
        }
        if (week % 60 == 25 && g.Me.Cash > 100_000) g.Try(Command.SetOnCall(g.Id, week % 120 == 25 ? 2 : 0));
    }

    static Game Play(int seed, int weeks, out long gap)
    {
        Game g = Start(seed);
        gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Autopilot(60);
        for (int week = 0; week < weeks && !g.Me.Bankrupt; week++)
        {
            UseTheTools(g, week);
            g.Autopilot(1);
        }
        return g;
    }

    static int Count(Game g, EventKind kind) { return g.State.Events.Count(e => e.Kind == kind); }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void The_books_balance_the_save_is_sound_and_a_load_plays_on_the_same(int seed)
    {
        long gap;
        Game g = Play(seed, 260, out gap);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.InRange(g.Me.Reputation, 0, 1000);
        Assert.InRange(g.Me.EthicsLevel, 0, g.C.Board.MaxLevel);
        Assert.InRange(g.Me.OptimiseLevel, 0, g.C.Optimise.MaxLevel);
        Assert.InRange(g.Me.OnCallLevel, 0, g.C.OnCall.MaxLevel);
        Assert.InRange(g.Me.CampaignWeeks, 0, g.C.Marketing.Weeks);
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_every_tool_is_really_used()
    {
        int chiefs = 0, boards = 0, optimised = 0, campaigns = 0, rotas = 0;
        foreach (int seed in new[] { 1, 2, 3 })
        {
            long gap;
            Game g = Play(seed, 260, out gap);
            chiefs += Count(g, EventKind.ChiefChanged);
            boards += Count(g, EventKind.EthicsBoardChanged);
            optimised += Count(g, EventKind.OptimisationSet);
            campaigns += g.State.Events.Count(e => e.Kind == EventKind.CampaignRun && e.Pick == 1);
            rotas += Count(g, EventKind.OnCallSet);
        }
        Assert.True(chiefs > 0, "no chief was ever named");
        Assert.True(boards > 0, "no board was ever seated");
        Assert.True(optimised > 0, "no inference was ever optimised");
        Assert.True(campaigns > 0, "no campaign was ever run");
        Assert.True(rotas > 0, "no rota was ever set");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void The_journal_replays_to_the_same_game(int seed)
    {
        long gap;
        Game g = Play(seed, 150, out gap);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_unused_set_of_tools_changes_nothing_for_a_lab_that_plays_on_its_own()
    {
        Game plain = Game.New(3, c =>
        {
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Scaling.Enabled = 0;
        });
        Game withTools = Start(3);
        plain.Autopilot(120);
        withTools.Autopilot(120);
        Assert.Equal(-1, withTools.Me.ChiefScientistId);
        Assert.Equal(-1, withTools.Me.ChiefSafetyId);
        Assert.Equal(0, withTools.Me.EthicsLevel);
        Assert.Equal(0, withTools.Me.OptimiseLevel);
        Assert.Equal(0, withTools.Me.OnCallLevel);
        Assert.Equal("", withTools.Me.CampaignSector);
        Assert.Equal(plain.Me.Cash, withTools.Me.Cash);
        Assert.Equal(plain.Me.Reputation, withTools.Me.Reputation);
    }

    [Fact]
    public void An_incident_is_softened_by_a_chief_a_board_and_a_rota_each_in_its_own_way()
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 2, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        int plain = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, g.Me.Staff.First(p => p.RoleId == "safety").Id));
        int withChief = SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m);
        Assert.True(withChief < plain);
        g.Do(Command.SetOptimisation(g.Id, 2));
        Assert.True(SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m) > withChief);
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, g.State, g.Me, m);
        Assert.True(parts.FromChief < 0);
        Assert.True(parts.FromOptimisation > 0);
        Assert.Equal(SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m), parts.Total);
        long fineBefore = g.Me.Ledger.Lifetime.Fines;
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        g.Do(Command.SetOnCall(g.Id, 2));
        m.Alignment = 0;
        g.C.Safety.IncidentChancePermille = 5000;
        g.Me.Reputation = 600;
        int incidents = Count(g, EventKind.Incident);
        for (int i = 0; i < 40 && Count(g, EventKind.Incident) == incidents; i++) SafetySystem.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(incidents + 1, Count(g, EventKind.Incident));
        Assert.True(g.Me.Reputation < 600);
        Assert.True(g.Me.Ledger.Lifetime.Fines < fineBefore);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }
}
