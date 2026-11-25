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

/// <summary>The treasury reserve, the pledge, the roadmap, mentoring and skunkworks used together over years, and where they meet the older tools.</summary>
public class ReservePledgeRoadmapMentoringSkunkworksTogetherTests
{
    static Game Start(int seed, Action<GameConfig> tweak = null)
    {
        return Game.New(seed, c =>
        {
            c.Marketing.MinSales = 0;
            c.Chiefs.MinStaff = 0;
            c.Sla.MinReputation = 0;
            c.Whistle.MoraleBelow = 1000;
            c.Whistle.WeeksToConcern = 5;
            c.Autopilot.AlignmentSharePermille = 100;
            c.Autopilot.MinReleaseAlignment = 0;
            c.PlayerStart.Cash = 5_000_000;
            if (tweak != null) tweak(c);
        });
    }

    static void Mentor(Game g, string role)
    {
        StaffMember mentor = g.Me.Staff.FirstOrDefault(s => s.RoleId == role && s.Senior == 1 && !Mentoring.IsMentor(g.Me, s));
        if (mentor == null)
        {
            StaffMember pick = g.Me.Staff.FirstOrDefault(s => s.RoleId == role && s.Senior == 0 && s.MentorId == 0);
            if (pick != null && g.Try(Command.PromoteStaff(g.Id, pick.Id)).Ok) mentor = pick;
        }
        StaffMember junior = g.Me.Staff.FirstOrDefault(s => s.RoleId == role && s.Senior == 0 && s.MentorId == 0 && s != mentor);
        if (mentor != null && junior != null) g.Try(Command.AssignMentor(g.Id, mentor.Id, junior.Id));
    }

    static void UseTheTools(Game g, int week)
    {
        if (week == 5) g.Try(Command.SetAsideReserve(g.Id, 300_000));
        if (week % 80 == 40) g.Try(Command.DrawReserve(g.Id, 100_000));
        if (week % 90 == 10) g.Try(Command.SetAsideReserve(g.Id, 150_000));
        if (week % 60 == 3) g.Try(Command.SetPledge(g.Id, 1 + week / 60 % 3));
        if (week % 70 == 12) g.Try(Command.AnnounceRoadmap(g.Id, 1 + week / 70 % 2, 20 + week % 30));
        if (week % 35 == 5) Mentor(g, week / 35 % 2 == 0 ? "researcher" : "engineer");
        if (week % 90 == 50) g.Try(Command.EndMentoring(g.Id, (g.Me.Staff.FirstOrDefault(s => s.MentorId > 0) ?? g.Me.Staff[0]).Id));
        if (week % 45 == 20) g.Try(Command.StartSkunkworks(g.Id, new[] { "research", "safety", "engineering" }[week / 45 % 3], 2, 8 + week % 12));
        if (week % 45 == 30) g.Try(Command.WrapUpSkunkworks(g.Id));
        {
            ModelState fresh = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Trained);
            if (fresh != null)
                foreach (SectorConfig s in g.C.Market.Sectors)
                    if (g.Try(Command.ReleaseModel(g.Id, fresh.Id, 1000, s.Id)).Ok) break;
        }
        if (Whistle.Open(g.Me)) g.Try(Command.AnswerWhistle(g.Id, Whistle.Clean));
        if (week % 30 == 3 && LabQueries.StaffCount(g.Me, "safety") < 2) g.Try(Command.Hire(g.Id, "safety", 1));
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
        Assert.True(g.Me.Reserve >= 0);
        Assert.InRange(g.Me.PledgeLevel, 0, g.C.Pledge.MaxLevel);
        Assert.InRange(g.Me.RoadmapLevel, 0, g.C.Roadmap.MaxLevel);
        Assert.InRange(Mentoring.Pairs(g.Me), 0, g.C.Mentoring.MaxPairs);
        Assert.All(g.Me.Staff, s => Assert.InRange(s.Morale, 0, 1000));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_every_tool_is_really_used()
    {
        int moved = 0, pledges = 0, announced = 0, settled = 0, paired = 0, parted = 0, projects = 0, paid = 0;
        foreach (int seed in new[] { 1, 2, 3 })
        {
            long gap;
            Game g = Play(seed, 260, out gap);
            moved += Count(g, EventKind.ReserveMoved);
            pledges += Count(g, EventKind.PledgeSet);
            announced += Count(g, EventKind.RoadmapAnnounced);
            settled += Count(g, EventKind.RoadmapSettled);
            paired += Count(g, EventKind.MentoringStarted);
            parted += Count(g, EventKind.MentoringEnded);
            projects += Count(g, EventKind.SkunkworksStarted);
            paid += Count(g, EventKind.SkunkworksDone);
        }
        Assert.True(moved > 0, "the reserve was never touched");
        Assert.True(pledges > 0, "no pledge was ever made");
        Assert.True(announced > 0 && settled > 0, "no roadmap was ever announced and settled (" + announced + ", " + settled + ")");
        Assert.True(paired > 0 && parted > 0, "no mentoring pair ever started and ended (" + paired + ", " + parted + ")");
        Assert.True(projects > 0 && paid > 0, "no skunkworks project ever started and paid (" + projects + ", " + paid + ")");
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
        Game on = Start(3);
        Game off = Start(3, c =>
        {
            c.Treasury.Enabled = 0;
            c.Pledge.MaxLevel = 0;
            c.Roadmap.MaxLevel = 0;
            c.Mentoring.Enabled = 0;
            c.Skunkworks.Enabled = 0;
        });
        on.Autopilot(150);
        off.Autopilot(150);
        Assert.Equal(0, on.Me.Reserve);
        Assert.Equal(0, on.Me.PledgeLevel);
        Assert.Equal(0, on.Me.RoadmapLevel);
        Assert.Equal(off.Me.Cash, on.Me.Cash);
        Assert.Equal(off.Me.Reputation, on.Me.Reputation);
        Assert.Equal(off.Me.Staff.Count, on.Me.Staff.Count);
    }

    static Game Shipped(Action<GameConfig> tweak = null, int claim = 950)
    {
        Game g = Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Whistle.MoraleBelow = 1000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, sales: 1, gpus: 8);
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, claim, "game-ai"));
        return g;
    }

    [Fact]
    public void A_pledge_kept_by_overstating_is_the_whistleblowers_business()
    {
        Game g = Shipped(c => { c.Pledge.BaseFloor = 1000; c.Pledge.FloorStep = 0; }, 1000);
        g.Do(Command.SetPledge(g.Id, 1));
        ModelState second = g.Train("mini", 100);
        g.UntilTrained(second);
        g.Do(Command.ReleaseModel(g.Id, second.Id, 1000, "game-bots"));
        Assert.Equal(1, g.Me.PledgeLevel);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.PledgeBroken);
        g.Weeks(g.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(g.Me));
        g.Do(Command.AnswerWhistle(g.Id, Whistle.Clean));
        Assert.Equal(second.Alignment, SafetySystem.DisplayedAlignment(second));
        Assert.True(SafetySystem.DisplayedAlignment(second) < 1000);
    }

    [Fact]
    public void Mentoring_an_unhappy_safety_junior_stops_a_concern_from_building()
    {
        Action<GameConfig> tweak = c =>
        {
            c.Whistle.MoraleBelow = 550;
            c.Seniority.MaxSharePermille = 500;
            c.Mentoring.MoralePerWeek = 50;
            c.Mentoring.MoraleCeiling = 900;
        };
        Game plain = Shipped(tweak);
        Game paired = Shipped(tweak);
        foreach (Game g in new[] { plain, paired })
        {
            foreach (StaffMember s in g.Me.Staff.Where(x => x.RoleId == "safety")) s.Morale = 800;
            g.Me.Staff.Last(x => x.RoleId == "safety").Morale = 400;
        }
        StaffMember mentor = paired.Me.Staff.First(x => x.RoleId == "safety");
        paired.Do(Command.PromoteStaff(paired.Id, mentor.Id));
        paired.Do(Command.AssignMentor(paired.Id, mentor.Id, paired.Me.Staff.Last(x => x.RoleId == "safety").Id));
        plain.Weeks(plain.C.Whistle.WeeksToConcern);
        paired.Weeks(paired.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(plain.Me));
        Assert.False(Whistle.Open(paired.Me));
    }

    [Fact]
    public void A_launch_can_deliver_a_roadmap_and_break_a_pledge_in_the_same_week()
    {
        Game g = Shipped(c => { c.Pledge.BaseFloor = 1000; c.Pledge.FloorStep = 0; });
        g.Do(Command.SetPledge(g.Id, 1));
        ModelState second = g.Train("mini", 100);
        g.Do(Command.AnnounceRoadmap(g.Id, 1, 40));
        g.UntilTrained(second);
        g.Do(Command.ReleaseModel(g.Id, second.Id, "game-bots"));
        Assert.Equal(0, g.Me.PledgeLevel);
        g.Weeks(1);
        Assert.Single(g.State.Events, e => e.Kind == EventKind.PledgeBroken);
        GameEvent settled = g.State.Events.Single(e => e.Kind == EventKind.RoadmapSettled);
        Assert.Equal(1, settled.Pick);
    }

    [Fact]
    public void A_project_pays_in_a_lump_at_the_end_and_leaves_the_lab_ahead_of_one_that_did_nothing()
    {
        Game worked = Shipped(c => c.Skunkworks.MinPeople = 1, 0);
        Game idle = Shipped(null, 0);
        worked.Do(Command.StartSkunkworks(worked.Id, "research", 1, 12));
        long start = worked.Me.ResearchPoints;
        worked.Weeks(11);
        idle.Weeks(11);
        Assert.True(worked.Me.ResearchPoints - start < idle.Me.ResearchPoints - start, "before the payout the team is missed");
        worked.Weeks(1);
        idle.Weeks(1);
        Assert.True(worked.Me.ResearchPoints > idle.Me.ResearchPoints, "after it the lump puts the lab ahead: " + worked.Me.ResearchPoints + " against " + idle.Me.ResearchPoints);
    }

    [Fact]
    public void A_safety_team_away_on_a_project_still_counts_for_the_whistleblower()
    {
        Game g = Shipped(c => { c.Whistle.MoraleBelow = 550; c.Skunkworks.MinPeople = 1; });
        g.Me.Staff.First(x => x.RoleId == "safety").Morale = 300;
        g.Do(Command.StartSkunkworks(g.Id, "safety", 1, 20));
        Assert.True(Skunkworks.Away(g.C, g.Me, g.Me.Staff.First(x => x.RoleId == "safety")));
        g.Weeks(g.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(g.Me));
    }

    [Fact]
    public void A_senior_mentoring_is_one_of_the_people_a_project_can_take_and_both_costs_show_in_the_count()
    {
        Game g = Shipped(c => { c.Seniority.MaxSharePermille = 500; c.Skunkworks.MinPeople = 1; });
        StaffMember mentor = g.Me.Staff.First(x => x.RoleId == "researcher");
        g.Do(Command.PromoteStaff(g.Id, mentor.Id));
        g.Do(Command.AssignMentor(g.Id, mentor.Id, g.Me.Staff.Where(x => x.RoleId == "researcher").Skip(1).First().Id));
        long before = LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research);
        Assert.Equal(1000 + 800 + 1000, before);
        g.Do(Command.StartSkunkworks(g.Id, "research", 1, 8));
        Assert.True(Skunkworks.Away(g.C, g.Me, mentor));
        Assert.Equal(800 + 1000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Research));
    }

    [Fact]
    public void The_reserve_covers_the_pay_while_a_team_is_away_and_the_books_still_balance()
    {
        Game g = Game.Solo(3, c =>
        {
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.GrantPerWeek = 0;
            c.PlayerStart.Cash = 5_000_000;
        }).WithStarterLab(researchers: 4, engineers: 3, safety: 2, sales: 1, gpus: 6);
        g.Me.Cash = 600_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.SetAsideReserve(g.Id, 400_000));
        g.Do(Command.StartSkunkworks(g.Id, "research", 2, 10));
        for (int i = 0; i < 60 && !g.Me.Bankrupt; i++) g.Weeks(1);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReserveCovered);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SkunkworksDone);
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }
}
