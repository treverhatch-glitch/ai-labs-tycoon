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

/// <summary>A layoff round: the same share of every function goes on a cheaper collective package; the people who stay and the press mind it.</summary>
public class LayoffsTests
{
    static Game Roomy(int seed, Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        });
        g.Build("research-lab", 4, 4);
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Build("safety-lab", 4, 3);
        g.Build("safety-lab", 4, 3);
        g.Build("sales-office", 4, 3);
        g.Build("sales-office", 4, 3);
        g.Hire("researcher", 8);
        g.Hire("engineer", 8);
        g.Hire("safety", 4);
        g.Hire("sales", 4);
        g.Rent(6);
        return g;
    }

    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Roomy(4, tweak);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.Reputation = 600;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        return g;
    }

    static List<GameEvent> Rounds(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.LayoffRound).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Layoffs.Enabled);
        Assert.Equal(300, c.Layoffs.MaxPermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Layoffs.Enabled = 2;
        c.Layoffs.MaxPermille = 501;
        c.Layoffs.SeverancePermille = 99;
        c.Layoffs.ReputationAtAll = -1;
        c.Layoffs.MoraleAtAll = 501;
        c.Layoffs.DeepPermille = 0;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxPermille", "severancePermille", "reputationAtAll", "moraleAtAll", "deepPermille" })
            Assert.Contains(problems, p => p.Contains("layoffs." + key));
        c.Layoffs.SeverancePermille = 1001;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("layoffs.severancePermille"));
    }

    [Fact]
    public void Nobody_is_let_go_by_itself()
    {
        Game g = Lab();
        int staff = g.Me.Staff.Count;
        g.Weeks(5);
        Assert.Equal(staff, g.Me.Staff.Count);
        Assert.Empty(Rounds(g));
    }

    [Fact]
    public void A_round_lets_go_the_same_share_of_every_function()
    {
        Game g = Lab();
        g.Do(Command.Layoff(g.Id, 250));
        Assert.Equal(6, LabQueries.StaffCount(g.Me, "researcher"));
        Assert.Equal(6, LabQueries.StaffCount(g.Me, "engineer"));
        Assert.Equal(3, LabQueries.StaffCount(g.Me, "safety"));
        Assert.Equal(3, LabQueries.StaffCount(g.Me, "sales"));
    }

    [Fact]
    public void The_package_is_cheaper_than_letting_people_go_one_by_one()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        long oneByOne = 2 * g.C.Role("researcher").Severance + 2 * g.C.Role("engineer").Severance + g.C.Role("safety").Severance + g.C.Role("sales").Severance;
        Assert.Equal(MathI.MulPermille(oneByOne, 500), Layoffs.Package(g.C, g.Me, 250));
        g.Do(Command.Layoff(g.Id, 250));
        Assert.Equal(cash - MathI.MulPermille(oneByOne, 500), g.Me.Cash);
        Assert.True(MathI.MulPermille(oneByOne, 500) < oneByOne);
    }

    [Fact]
    public void The_press_and_the_staff_who_stay_mind_in_proportion_to_the_share()
    {
        Game g = Lab();
        int share = Layoffs.StaffSharePermille(g.C, g.Me, 250);
        Assert.Equal(250, share);
        int rep = g.Me.Reputation;
        g.Do(Command.Layoff(g.Id, 250));
        Assert.Equal(rep - 10, g.Me.Reputation);
        Assert.All(g.Me.Staff, p => Assert.Equal(700 - 50, p.Morale));
        Game small = Lab();
        small.Do(Command.Layoff(small.Id, 130));
        int smallShare = Layoffs.StaffSharePermille(small.C, small.Me, 130);
        Assert.True(smallShare < share);
        Assert.True(small.Me.Staff.All(p => p.Morale > 650));
    }

    [Fact]
    public void The_event_marks_a_deep_cut()
    {
        Game g = Lab();
        g.Do(Command.Layoff(g.Id, 250));
        GameEvent e = Assert.Single(Rounds(g));
        Assert.Equal(1, e.Pick);
        Assert.Equal(6, e.Amount);
        Assert.Equal(250, e.SubjectId);
        Assert.Contains("laid off 6 people", EventText.Describe(g.C, g.State, e));
        Game mild = Lab();
        mild.Do(Command.Layoff(mild.Id, 130));
        Assert.Equal(0, Assert.Single(Rounds(mild)).Pick);
    }

    [Fact]
    public void A_single_person_function_is_never_emptied_and_a_round_that_cuts_nobody_is_refused()
    {
        Game g = Game.Solo(4, c => { c.Office.ChancePermille = 0; }).WithStarterLab(researchers: 2, engineers: 1, safety: 0, sales: 1, gpus: 6);
        string before = g.Sim.Digest();
        g.Refused(Command.Layoff(g.Id, 300), CommandError.NotEnoughStaff);
        Assert.Equal(before, g.Sim.Digest());
        Game big = Lab();
        big.Do(Command.Layoff(big.Id, 300));
        Assert.True(LabQueries.StaffCount(big.Me, "sales") >= 1);
    }

    [Fact]
    public void The_share_is_capped_and_must_be_positive()
    {
        Game g = Lab();
        string before = g.Sim.Digest();
        g.Refused(Command.Layoff(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.Layoff(g.Id, -5), CommandError.InvalidArgument);
        g.Refused(Command.Layoff(g.Id, 301), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        g.Do(Command.Layoff(g.Id, 300));
    }

    [Fact]
    public void Reputation_and_morale_never_go_below_zero()
    {
        Game g = Lab();
        g.Me.Reputation = 2;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 5;
        g.Do(Command.Layoff(g.Id, 300));
        Assert.Equal(0, g.Me.Reputation);
        Assert.All(g.Me.Staff, p => Assert.Equal(0, p.Morale));
    }

    [Fact]
    public void The_lowest_morale_people_go_first()
    {
        Game g = Lab();
        StaffMember low = g.Me.Staff.First(p => p.RoleId == "researcher");
        low.Morale = 100;
        int id = low.Id;
        g.Do(Command.Layoff(g.Id, 250));
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == id);
    }

    [Fact]
    public void The_payroll_falls_by_what_was_let_go()
    {
        Game g = Lab();
        long before = LabQueries.WeeklySalaries(g.C, g.Me);
        g.Do(Command.Layoff(g.Id, 250));
        Assert.True(LabQueries.WeeklySalaries(g.C, g.Me) < before);
    }

    [Fact]
    public void A_rival_lab_and_a_game_with_it_switched_off_cannot_run_a_round()
    {
        Game w = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.Layoff(rival.Id, 100)).Ok);
        Game off = Lab(c => c.Layoffs.Enabled = 0);
        string before = off.Sim.Digest();
        off.Refused(Command.Layoff(off.Id, 100), CommandError.InvalidArgument);
        Assert.Equal(before, off.Sim.Digest());
    }

    [Fact]
    public void Save_and_load_after_a_round_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.Layoff(g.Id, 200));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(20);
        for (int i = 0; i < 20; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Roomy(4);
        g.Weeks(4);
        g.Do(Command.Layoff(g.Id, 150));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_rounds_run_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Roomy(11);
        g.Me.Cash += 1_000_000_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int rounds = 0;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            if (i % 60 == 30 && g.Try(Command.Layoff(g.Id, 200)).Ok) rounds++;
        }
        Assert.True(rounds > 0, "staff " + g.Me.Staff.Count + " tick " + g.Sim.Tick + " cash " + g.Me.Cash);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
