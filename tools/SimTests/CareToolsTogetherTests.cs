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

/// <summary>All six care tools used together over years: crunch, offsite, insurance, red teams, interpretability and pay.</summary>
public class CareToolsTogetherTests
{
    static Game Start(int seed)
    {
        return Game.New(seed, c =>
        {
            c.RedTeam.RoleId = "";
            c.Labour.Enabled = 0;
            c.Interp.LevelTechs = new List<string> { "state-machines" };
        });
    }

    static void UseTheTools(Game g, int week)
    {
        if (week % 40 == 0) g.Try(Command.SetInsurance(g.Id, week % 80 == 0 ? 2 : 0));
        if (week % 26 == 5) g.Try(Command.SetCrunch(g.Id, true));
        if (week % 26 == 13) g.Try(Command.SetCrunch(g.Id, false));
        if (week % 26 == 20 && g.Me.Cash > 200000) g.Try(Command.HoldOffsite(g.Id));
        if (week % 30 == 3)
        {
            ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Released);
            if (m != null && g.Me.Cash > 100000) g.Try(Command.RedTeamModel(g.Id, m.Id));
        }
        if (week == 0) g.Try(Command.SetInterpretability(g.Id, 1));
        if (week % 52 == 10 && (g.Me.PayLevel > 0 || g.Me.Cash > 1_000_000)) g.Try(Command.SetPayLevel(g.Id, g.Me.PayLevel == 0 ? 1 : 0));
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
        Assert.False(g.Me.Bankrupt);
        Assert.True(Count(g, EventKind.PayChanged) > 0);
        Assert.True(Count(g, EventKind.CrunchEnded) > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_a_red_team_and_an_offsite_really_are_paid_for()
    {
        int redTeams = 0, offsites = 0;
        foreach (int seed in new[] { 1, 2, 3 })
        {
            Game g = Play(seed, 260, out long gap);
            redTeams += Count(g, EventKind.RedTeamRan);
            offsites += Count(g, EventKind.OffsiteHeld);
        }
        Assert.True(redTeams > 0);
        Assert.True(offsites > 0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void The_journal_replays_to_the_same_game(int seed)
    {
        long gap;
        Game g = Play(seed, 200, out gap);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void Morale_crunch_pay_and_red_team_weeks_stay_inside_their_limits_throughout()
    {
        long gap;
        Game g = Play(1, 0, out gap);
        for (int week = 0; week < 200 && !g.Me.Bankrupt; week++)
        {
            UseTheTools(g, week);
            g.Autopilot(1);
            Assert.InRange(g.Me.CrunchWeeks, 0, g.C.StaffPeople.CrunchMaxWeeks);
            Assert.InRange(g.Me.PayLevel, g.C.StaffPeople.PayMinLevel, g.C.StaffPeople.PayMaxLevel);
            Assert.InRange(g.Me.InterpLevel, 0, g.C.Interp.MaxLevel);
            Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
            Assert.All(g.Me.Models, m => Assert.InRange(m.RedTeamUntil, 0, g.Tick + g.C.RedTeam.Weeks));
            Assert.All(g.Me.Models, m => Assert.InRange(SafetySystem.AlignmentGap(g.C, m), 0, 1000));
        }
    }
}
