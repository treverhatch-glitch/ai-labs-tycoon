using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Crunch time, the offsite and incident insurance used together over years: the books balance, the save is sound, and the replay agrees.</summary>
public class StaffCareToolsTogetherTests
{
    static void UseTheTools(Game g, int week)
    {
        if (week % 40 == 0) g.Try(Command.SetInsurance(g.Id, week % 80 == 0 ? 2 : 0));
        if (week % 26 == 5) g.Try(Command.SetCrunch(g.Id, true));
        if (week % 26 == 13) g.Try(Command.SetCrunch(g.Id, false));
        if (week % 26 == 20 && g.Me.Cash > 200000) g.Try(Command.HoldOffsite(g.Id));
    }

    static Game Play(int seed, int weeks, out long gap)
    {
        Game g = Game.New(seed);
        gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Autopilot(60);
        for (int week = 0; week < weeks && !g.Me.Bankrupt; week++)
        {
            UseTheTools(g, week);
            g.Autopilot(1);
        }
        return g;
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void The_books_balance_the_save_is_sound_and_a_load_plays_on_the_same(int seed)
    {
        long gap;
        Game g = Play(seed, 260, out gap);
        Assert.False(g.Me.Bankrupt);
        Assert.True(g.Tick >= 260);
        Assert.True(g.State.Events.Count(e => e.Kind == EventKind.CrunchEnded) > 0);
        Assert.True(g.State.Events.Count(e => e.Kind == EventKind.OffsiteHeld) > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
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
    public void Morale_stays_on_the_scale_and_crunch_weeks_inside_the_limit_throughout()
    {
        long gap;
        Game g = Play(1, 0, out gap);
        for (int week = 0; week < 200 && !g.Me.Bankrupt; week++)
        {
            UseTheTools(g, week);
            g.Autopilot(1);
            Assert.InRange(g.Me.CrunchWeeks, 0, g.C.StaffPeople.CrunchMaxWeeks);
            Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
            Assert.InRange(g.Me.InsuranceClaims, 0, g.C.Finance.InsuranceDropClaims);
        }
    }
}
