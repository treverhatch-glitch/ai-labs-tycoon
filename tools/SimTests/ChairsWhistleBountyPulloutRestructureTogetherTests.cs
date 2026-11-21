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

/// <summary>Chairs, whistleblowers, the bug bounty, sector pullouts and loan restructures used together over years, with the older tools.</summary>
public class ChairsWhistleBountyPulloutRestructureTogetherTests
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

    static void UseTheTools(Game g, int week)
    {
        if (week == 15 || week == 90 || week == 170) g.Try(Command.EndowChair(g.Id));
        if (week % 60 == 5) g.Try(Command.SetBugBounty(g.Id, 1 + week / 60 % 3));
        if (week % 60 == 40 && g.Me.BountyLevel > 0 && week > 150) g.Try(Command.SetBugBounty(g.Id, 0));
        if (week == 5 && LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 4, out int x, out int y))
        {
            g.Try(Command.BuildRoom(g.Id, "server-closet", x, y, 4, 4));
            g.Try(Command.BuyGpus(g.Id, 6, HardwareBatch.Hq));
        }
        if (week % 70 == 20 && g.Me.Loans.Count == 0) g.Try(Command.TakeLoan(g.Id, 400));
        if (week % 70 == 24)
        {
            Loan loan = g.Me.Loans.FirstOrDefault();
            if (loan != null) g.Try(Command.RestructureLoan(g.Id, loan.Id));
        }
        if (week % 50 == 25)
        {
            ModelState m = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Released && x.Sectors.Count > 1);
            if (m != null) g.Try(Command.PullOutOfSector(g.Id, m.Id, m.Sectors[m.Sectors.Count - 1]));
        }
        {
            ModelState fresh = g.Me.Models.FirstOrDefault(x => x.Status == ModelStatus.Trained);
            if (fresh != null)
                foreach (SectorConfig s in g.C.Market.Sectors)
                    if (g.Try(Command.ReleaseModel(g.Id, fresh.Id, 1000, s.Id)).Ok) break;
        }
        if (Whistle.Open(g.Me)) g.Try(Command.AnswerWhistle(g.Id, g.Me.Silenced < 1 ? Whistle.Silence : Whistle.Clean));
        if (week % 30 == 3 && LabQueries.StaffCount(g.Me, "safety") < 2) g.Try(Command.Hire(g.Id, "safety", 1));
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
        Assert.InRange(g.Me.Chairs, 0, g.C.Chairs.MaxChairs);
        Assert.InRange(g.Me.BountyLevel, 0, g.C.BugBounty.MaxLevel);
        Assert.All(g.Me.Loans, l => Assert.True(l.Payment > 0 && l.Balance > 0));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Autopilot(20);
        for (int i = 0; i < 20; i++) { if (GameDate.IsMonthStart(back.Config.Calendar, back.Tick)) back.RunPlan(back.Player.Id, back.Config.Autopilot); back.Step(); }
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void Across_the_seeds_every_tool_is_really_used()
    {
        int chairs = 0, raised = 0, answered = 0, bounties = 0, pullouts = 0, restructures = 0;
        string seen = "";
        foreach (int seed in new[] { 1, 2, 3 })
        {
            long gap;
            Game g = Play(seed, 260, out gap);
            seen += " seed " + seed + " tick " + g.State.Tick + " bankrupt " + g.Me.Bankrupt + " loans " + Count(g, EventKind.LoanTaken) + ";";
            chairs += Count(g, EventKind.ChairEndowed);
            raised += Count(g, EventKind.WhistleRaised);
            answered += Count(g, EventKind.WhistleAnswered);
            bounties += Count(g, EventKind.BountySet);
            pullouts += Count(g, EventKind.SectorPulledOut);
            restructures += Count(g, EventKind.LoanRestructured);
        }
        Assert.True(chairs > 0, "no chair was ever endowed");
        Assert.True(bounties > 0, "no bounty was ever set");
        Assert.True(restructures > 0, "no loan was ever restructured" + seen);
        Assert.True(raised > 0 && answered > 0, "no concern was ever raised and answered (" + raised + ", " + answered + ")");
        Assert.True(pullouts > 0, "nobody ever pulled out of a sector");
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
            c.Chairs.MaxChairs = 0;
            c.Whistle.Enabled = 0;
            c.BugBounty.MaxLevel = 0;
            c.Pullout.Enabled = 0;
            c.Restructure.Enabled = 0;
        });
        on.Autopilot(150);
        off.Autopilot(150);
        Assert.Equal(0, on.Me.Chairs);
        Assert.Equal(0, on.Me.BountyLevel);
        Assert.Equal(0, on.Me.WhistleDue);
        Assert.Equal(off.Me.Cash, on.Me.Cash);
        Assert.Equal(off.Me.Reputation, on.Me.Reputation);
        Assert.Equal(off.Me.Staff.Count, on.Me.Staff.Count);
    }

    static Game Shipped()
    {
        Game g = Game.Solo(6, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Whistle.MoraleBelow = 1000;
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, sales: 1, gpus: 8);
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 950, "game-ai", "game-bots"));
        return g;
    }

    [Fact]
    public void Pulling_out_of_every_sector_ends_the_bounty_bill_and_a_concern_about_the_model()
    {
        Game g = Shipped();
        ModelState model = g.Me.Models.First(m => m.Status == ModelStatus.Released);
        g.Do(Command.SetBugBounty(g.Id, 2));
        Assert.True(BugBounty.WeeklyCost(g.C, g.Me) > 0);
        g.Weeks(g.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(g.Me));
        g.Do(Command.PullOutOfSector(g.Id, model.Id, "game-ai"));
        g.Do(Command.PullOutOfSector(g.Id, model.Id, "game-bots"));
        Assert.Equal(ModelStatus.Retired, model.Status);
        Assert.Equal(0, BugBounty.WeeklyCost(g.C, g.Me));
        Assert.Equal(2, g.Me.BountyLevel);
        g.Weeks(1);
        Assert.False(Whistle.Open(g.Me));
        g.Weeks(30);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.WhistleLeaked);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_chair_a_bounty_and_a_restructured_loan_each_land_on_their_own_line_of_the_books()
    {
        Game g = Shipped();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.EndowChair(g.Id));
        g.Do(Command.SetBugBounty(g.Id, 1));
        g.Build("server-closet", 4, 4);
        g.Do(Command.BuyGpus(g.Id, 6, HardwareBatch.Hq));
        gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.TakeLoan(g.Id, 500));
        Loan loan = g.Me.Loans.Single();
        long payment = loan.Payment;
        g.Do(Command.RestructureLoan(g.Id, loan.Id));
        Assert.True(loan.Payment < payment);
        g.Weeks(60);
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Equal(1, g.Me.Chairs);
        Assert.True(Prestige.Level(g.C, g.Me) >= g.C.Chairs.PrestigeFloor);
    }

    [Fact]
    public void Coming_clean_then_a_chair_floor_keep_reputation_and_prestige_where_they_should_be()
    {
        Game g = Shipped();
        g.Weeks(g.C.Whistle.WeeksToConcern);
        Assert.True(Whistle.Open(g.Me));
        g.Do(Command.AnswerWhistle(g.Id, Whistle.Clean));
        ModelState model = g.Me.Models.First(m => m.Status == ModelStatus.Released);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, model));
        g.Do(Command.EndowChair(g.Id));
        g.Weeks(100);
        Assert.Equal(g.C.Chairs.PrestigeFloor, Prestige.Level(g.C, g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.WhistleLeaked);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }
}
