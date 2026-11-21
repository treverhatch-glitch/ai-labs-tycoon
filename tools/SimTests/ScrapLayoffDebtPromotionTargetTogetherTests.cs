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

/// <summary>Scrapped runs, layoff rounds, technical debt, promotions and board targets used together over years, with the older tools.</summary>
public class ScrapLayoffDebtPromotionTargetTogetherTests
{
    static Game Start(int seed, Action<GameConfig> tweak = null)
    {
        return Game.New(seed, c =>
        {
            c.Marketing.MinSales = 0;
            c.Chiefs.MinStaff = 0;
            c.Sla.MinReputation = 0;
            if (tweak != null) tweak(c);
        });
    }

    static void UseTheTools(Game g, int week)
    {
        if (week % 90 == 40)
        {
            ModelState run = g.Me.Models.FirstOrDefault(m => m.Status == ModelStatus.Training);
            if (run != null) g.Try(Command.ScrapRun(g.Id, run.Id));
        }
        if (week % 110 == 70 && g.Me.Staff.Count >= 8) g.Try(Command.Layoff(g.Id, 150));
        if (week % 45 == 10) g.Try(Command.SetCrunch(g.Id, true));
        if (week % 45 == 18) g.Try(Command.SetCrunch(g.Id, false));
        if (week % 30 == 12 && g.Me.TechDebt > 0 && g.Me.Cash > 400_000) g.Try(Command.PayDownDebt(g.Id, (int)MathI.Min(60, g.Me.TechDebt)));
        if (week % 25 == 7)
        {
            StaffMember next = g.Me.Staff.FirstOrDefault(p => p.Senior == 0 && p.Morale >= g.C.Seniority.MinMorale);
            if (next != null && g.Me.Cash > 200_000) g.Try(Command.PromoteStaff(g.Id, next.Id));
        }
        if (week % 20 == 3 && g.Me.Cash > 100_000) g.Try(Command.SetRevenueTarget(g.Id, 1 + week / 20 % 3));
        if (week % 35 == 3) g.Try(Command.PublishPaper(g.Id));
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
        Assert.InRange(g.Me.TechDebt, 0, 1000);
        Assert.InRange(g.Me.RevenueTargetLevel, 0, g.C.RevenueTargets.MaxLevel);
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Senior, 0, 1));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_every_tool_is_really_used()
    {
        int scrapped = 0, rounds = 0, debtPaid = 0, promoted = 0, set = 0, settled = 0;
        foreach (int seed in new[] { 1, 2, 3 })
        {
            long gap;
            Game g = Play(seed, 260, out gap);
            scrapped += Count(g, EventKind.RunScrapped);
            rounds += Count(g, EventKind.LayoffRound);
            debtPaid += Count(g, EventKind.DebtPaid);
            promoted += Count(g, EventKind.StaffPromoted);
            set += Count(g, EventKind.TargetSet);
            settled += Count(g, EventKind.TargetSettled);
        }
        Assert.True(promoted > 0, "nobody was ever promoted");
        Assert.True(set > 0, "no target was ever set");
        Assert.True(settled > 0, "no target was ever judged");
        Assert.True(scrapped + rounds + debtPaid > 0, "none of the scrap, layoff or debt tools was ever used");
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
            c.Abandon.Enabled = 0;
            c.Layoffs.Enabled = 0;
            c.TechDebt.Enabled = 0;
            c.Seniority.Enabled = 0;
            c.RevenueTargets.MaxLevel = 0;
        });
        on.Autopilot(150);
        off.Autopilot(150);
        Assert.Equal(0, on.Me.TechDebt);
        Assert.Equal(0, on.Me.RevenueTargetLevel);
        Assert.All(on.Me.Staff, p => Assert.Equal(0, p.Senior));
        Assert.Equal(off.Me.Cash, on.Me.Cash);
        Assert.Equal(off.Me.Reputation, on.Me.Reputation);
        Assert.Equal(off.Me.Staff.Count, on.Me.Staff.Count);
    }

    [Fact]
    public void Debt_a_promotion_and_a_layoff_pull_the_right_ways_in_one_lab()
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
        });
        g.Build("research-lab", 4, 4);
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Hire("researcher", 8);
        g.Hire("engineer", 8);
        g.Rent(6);
        long research = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        g.Do(Command.PromoteStaff(g.Id, g.Me.Staff.First(p => p.RoleId == "researcher").Id));
        long promotedResearch = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        Assert.True(promotedResearch > research);
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(5);
        Assert.True(g.Me.TechDebt > 0);
        long salaries = LabQueries.WeeklySalaries(g.C, g.Me);
        g.Do(Command.Layoff(g.Id, 250));
        Assert.True(LabQueries.WeeklySalaries(g.C, g.Me) < salaries);
        Assert.Equal(1, g.Me.Staff.Count(p => p.Senior == 1));
        int debt = g.Me.TechDebt;
        g.Do(Command.PayDownDebt(g.Id, debt));
        Assert.Equal(0, g.Me.TechDebt);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }
}
