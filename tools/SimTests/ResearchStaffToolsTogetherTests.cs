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

/// <summary>Audits, the research focus, hack weeks, staff equity and retraining used together over years, with the older care tools beside them.</summary>
public class ResearchStaffToolsTogetherTests
{
    static Game Start(int seed)
    {
        return Game.New(seed, c =>
        {
            c.Compliance.StaffPerLevel = 0;
            c.Audit.MinSafetyStaff = 0;
            c.Hack.MinMorale = 0;
            c.Equity.MinStaff = 0;
            c.RedTeam.RoleId = "";
        });
    }

    static readonly string[] Focuses = { "frontier", "alignment", "efficiency" };

    static void UseTheTools(Game g, int week, ref int focusTurn)
    {
        if (week % 52 == 10 && g.Me.Cash > 1_000_000)
        {
            ModelState m = g.Me.Models.FirstOrDefault(x => (x.Status == ModelStatus.Trained || x.Status == ModelStatus.Released) && !SafetyAudit.Certified(g.State, x));
            if (m != null) g.Try(Command.CommissionAudit(g.Id, m.Id));
        }
        if (week % 40 == 3 && g.Me.Cash > 500_000 && g.Try(Command.SetResearchFocus(g.Id, Focuses[focusTurn % 3])).Ok) focusTurn++;
        if (week % 30 == 12 && g.Me.Cash > 300_000) g.Try(Command.HoldHackWeek(g.Id));
        if ((week == 40 || week == 120 || week == 200) && g.Me.Cash > 200_000) g.Try(Command.GrantStaffEquity(g.Id));
        if (week % 35 == 20)
        {
            StaffMember p = g.Me.Staff.FirstOrDefault(x => x.RoleId == "engineer" && x.Morale >= 300);
            if (p != null && !g.Try(Command.RetrainStaff(g.Id, p.Id, "researcher")).Ok)
            {
                StaffMember r = g.Me.Staff.FirstOrDefault(x => x.RoleId == "researcher" && x.Morale >= 300);
                if (r != null) g.Try(Command.RetrainStaff(g.Id, r.Id, "engineer"));
            }
        }
        if (week % 13 == 0 && g.Me.ComplianceLevel == 0 && g.Me.Cash > 1_000_000) g.Try(Command.SetCompliance(g.Id, 2));
        if (week % 13 == 6 && g.Me.ComplianceLevel > 0 && g.Me.Cash < 300_000) g.Try(Command.SetCompliance(g.Id, 0));
        if (week % 26 == 20 && g.Me.Cash > 200_000) g.Try(Command.HoldOffsite(g.Id));
    }

    static Game Play(int seed, int weeks, out long gap)
    {
        Game g = Start(seed);
        gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int focusTurn = 0;
        g.Autopilot(60);
        for (int week = 0; week < weeks && !g.Me.Bankrupt; week++)
        {
            UseTheTools(g, week, ref focusTurn);
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
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.InRange(g.Me.Reputation, 0, 1000);
        Assert.InRange(g.Me.EquityLevel, 0, 3);
        Assert.True(g.Me.EquityPermille + g.Me.FounderSharePermille <= 1000);
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        Assert.All(g.Me.Staff, p => Assert.NotNull(g.C.Role(p.RoleId)));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_every_tool_is_really_used()
    {
        int audits = 0, focuses = 0, hacks = 0, grants = 0, moves = 0;
        foreach (int seed in new[] { 1, 2, 3 })
        {
            long gap;
            Game g = Play(seed, 260, out gap);
            audits += Count(g, EventKind.AuditDone);
            focuses += Count(g, EventKind.FocusChanged);
            hacks += g.State.Events.Count(e => e.Kind == EventKind.HackWeek && e.Pick == 1);
            grants += Count(g, EventKind.EquityGranted);
            moves += Count(g, EventKind.StaffRetrained);
        }
        Assert.True(audits > 0, "no model was ever audited");
        Assert.True(focuses > 0, "no focus was ever picked");
        Assert.True(hacks > 0, "no hack week was ever held");
        Assert.True(grants > 0, "no equity was ever granted");
        Assert.True(moves > 0, "no one was ever retrained");
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
        Game plain = Game.New(3);
        Game withTools = Start(3);
        plain.Autopilot(120);
        withTools.Autopilot(120);
        Assert.Equal("", withTools.Me.ResearchFocus);
        Assert.Equal(0, withTools.Me.HackWeeks);
        Assert.Equal(0, withTools.Me.EquityLevel);
        Assert.All(withTools.Me.Models, m => Assert.Equal(0, m.AuditedUntil));
        Assert.Equal(plain.Me.Cash, withTools.Me.Cash);
        Assert.Equal(plain.Me.Reputation, withTools.Me.Reputation);
    }

    [Fact]
    public void Equity_makes_the_lab_cheaper_to_run_but_the_sale_price_smaller()
    {
        Game g = Start(2);
        g.Autopilot(60);
        long salaries = LabQueries.WeeklySalaries(g.C, g.Me);
        DisplayQueries.ExitPriceBreakdown before = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        g.Do(Command.GrantStaffEquity(g.Id));
        g.Do(Command.GrantStaffEquity(g.Id));
        Assert.True(LabQueries.WeeklySalaries(g.C, g.Me) < salaries);
        DisplayQueries.ExitPriceBreakdown after = DisplayQueries.ExitPriceParts(g.C, g.State, g.Me);
        Assert.True(after.StaffShare > 0);
        Assert.True(before.Total > 0);
        Assert.True(after.Total < before.Total);
    }
}
