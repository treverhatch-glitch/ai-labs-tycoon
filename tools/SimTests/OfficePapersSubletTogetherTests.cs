using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>The compliance office, papers and the compute sublet used together over years, with the earlier care tools running beside them.</summary>
public class OfficePapersSubletTogetherTests
{
    static Game Start(int seed)
    {
        return Game.New(seed, c =>
        {
            c.Compliance.StaffPerLevel = 0;
            c.Prestige.PaperResearchWeeks = 1;
            c.Prestige.MinPoints = 1;
            c.RedTeam.RoleId = "";
        });
    }

    static void UseTheTools(Game g, int week)
    {
        if (week % 13 == 0 && g.Me.ComplianceLevel == 0 && g.Me.Cash > 1_000_000) g.Try(Command.SetCompliance(g.Id, 2));
        if (week % 13 == 6 && g.Me.ComplianceLevel > 0 && g.Me.Cash < 300_000) g.Try(Command.SetCompliance(g.Id, 0));
        if (week % 30 == 7) g.Try(Command.PublishPaper(g.Id));
        if (week % 26 == 5 && g.Me.Cash > 500_000) g.Try(Command.SetSublet(g.Id, 200));
        if (week % 26 == 18) g.Try(Command.SetSublet(g.Id, 0));
        if (week % 52 == 10 && g.Me.Cash > 1_000_000) g.Try(Command.SetPayLevel(g.Id, g.Me.PayLevel == 0 ? 1 : 0));
        if (week % 26 == 20 && g.Me.Cash > 200_000) g.Try(Command.HoldOffsite(g.Id));
    }

    static Game Play(int seed, int weeks, out long gap, out int topCompliance)
    {
        Game g = Start(seed);
        gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        topCompliance = 0;
        g.Autopilot(60);
        for (int week = 0; week < weeks && !g.Me.Bankrupt; week++)
        {
            UseTheTools(g, week);
            g.Autopilot(1);
            topCompliance = System.Math.Max(topCompliance, g.Me.ComplianceLevel);
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
        int top;
        Game g = Play(seed, 260, out gap, out top);
        Assert.False(g.Me.Bankrupt);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.InRange(g.Me.Prestige, 0, 1000);
        Assert.InRange(g.Me.Reputation, 0, 1000);
        Assert.InRange(g.Me.ComplianceLevel, 0, 3);
        Assert.True(g.Me.Ledger.Lifetime.Sublet >= 0);
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_papers_are_published_and_compute_is_really_let()
    {
        int papers = 0;
        long sublet = 0;
        int fees = 0;
        int tops = 0;
        foreach (int seed in new[] { 1, 2, 3 })
        {
            long gap;
            Game g = Play(seed, 260, out gap, out int top);
            tops = System.Math.Max(tops, top);
            papers += Count(g, EventKind.PaperPublished);
            sublet += g.Me.Ledger.Lifetime.Sublet;
            fees += Count(g, EventKind.SubletChanged);
        }
        Assert.True(papers > 0, "no paper was ever published");
        Assert.True(sublet > 0, "no compute was ever let");
        Assert.True(fees > 0);
        Assert.True(tops > 0, "no compliance office was ever opened");
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    public void The_journal_replays_to_the_same_game(int seed)
    {
        long gap;
        Game g = Play(seed, 150, out gap, out int top);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_unused_set_of_tools_changes_nothing_for_a_lab_that_plays_on_its_own()
    {
        Game plain = Game.New(3);
        Game withTools = Start(3);
        plain.Autopilot(120);
        withTools.Autopilot(120);
        Assert.Equal(0, withTools.Me.ComplianceLevel);
        Assert.Equal(0, withTools.Me.Prestige);
        Assert.Equal(0, withTools.Me.SubletPermille);
        Assert.Equal(plain.Me.Cash, withTools.Me.Cash);
        Assert.Equal(plain.Me.Reputation, withTools.Me.Reputation);
    }
}
