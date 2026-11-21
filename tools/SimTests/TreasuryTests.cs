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

/// <summary>The treasury reserve: cash set aside earns a yield, comes back for a fee, and covers a debt the weekly bill would leave.</summary>
public class TreasuryTests
{
    static Game Lab(long cash = 5_000_000, Action<GameConfig> tweak = null, int seed = 3)
    {
        return Game.Solo(seed, c =>
        {
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = cash;
            if (tweak != null) tweak(c);
        });
    }

    static Game Burning(long reserve)
    {
        Game g = Lab(5_000_000, c => c.PlayerStart.GrantPerWeek = 0).WithStarterLab(researchers: 4, engineers: 3, safety: 2, sales: 1, gpus: 6);
        g.Me.Cash = 400_000;
        if (reserve > 0) g.Do(Command.SetAsideReserve(g.Id, (int)reserve));
        return g;
    }

    static List<GameEvent> Moves(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.ReserveMoved).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Treasury.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Treasury.Enabled = 2;
        c.Treasury.MinMove = 0;
        c.Treasury.WeeklyYieldPpm = 5001;
        c.Treasury.DrawFeePermille = 501;
        c.Treasury.CoverFeePermille = 100;
        c.Treasury.DrawFeePermille = 200;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "minMove", "weeklyYieldPpm", "coverFeePermille" })
            Assert.Contains(problems, p => p.Contains("treasury." + key));
        c.Treasury.DrawFeePermille = 501;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("treasury.drawFeePermille"));
    }

    [Fact]
    public void A_lab_with_nothing_set_aside_is_exactly_as_it_was()
    {
        Game g = Lab();
        g.Weeks(30);
        Assert.Equal(0, g.Me.Reserve);
        Assert.Equal(0, Treasury.WeeklyYield(g.C, g.Me));
        Assert.Empty(Moves(g));
        Assert.Equal(0, g.Me.Ledger.Lifetime.CapitalIn);
    }

    [Fact]
    public void Setting_cash_aside_moves_it_out_of_the_lab_and_onto_the_capital_line()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.SetAsideReserve(g.Id, 1_000_000));
        Assert.Equal(1_000_000, g.Me.Reserve);
        Assert.Equal(cash - 1_000_000, g.Me.Cash);
        Assert.Equal(-1_000_000, g.Me.Ledger.Lifetime.CapitalIn);
        GameEvent e = Moves(g).Single();
        Assert.Equal(1_000_000, e.Amount);
        Assert.Equal(1, e.Pick);
        Assert.Contains("set $1000000 aside", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_reserve_earns_its_yield_every_week()
    {
        Game g = Lab();
        g.Do(Command.SetAsideReserve(g.Id, 1_000_000));
        Assert.Equal(800, Treasury.WeeklyYield(g.C, g.Me));
        long capital = g.Me.Ledger.Lifetime.CapitalIn;
        g.Weeks(10);
        Assert.Equal(capital + 8000, g.Me.Ledger.Lifetime.CapitalIn);
        Assert.Equal(1_000_000, g.Me.Reserve);
    }

    [Fact]
    public void A_planned_draw_costs_its_fee_and_leaves_the_rest_with_the_lab()
    {
        Game g = Lab();
        g.Do(Command.SetAsideReserve(g.Id, 1_000_000));
        long cash = g.Me.Cash;
        g.Do(Command.DrawReserve(g.Id, 400_000));
        Assert.Equal(600_000, g.Me.Reserve);
        Assert.Equal(cash + 388_000, g.Me.Cash);
        GameEvent e = Moves(g).Last();
        Assert.Equal(0, e.Pick);
        Assert.Contains("took $400000 back", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void Refused_moves_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab(2_000_000);
        string d = g.Sim.Digest();
        g.Refused(Command.SetAsideReserve(g.Id, 9999), CommandError.InvalidArgument);
        g.Refused(Command.SetAsideReserve(g.Id, -5), CommandError.InvalidArgument);
        g.Refused(Command.SetAsideReserve(g.Id, 900_000_000), CommandError.NotEnoughCash);
        g.Refused(Command.DrawReserve(g.Id, 20_000), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());
        g.Do(Command.SetAsideReserve(g.Id, 100_000));
        d = g.Sim.Digest();
        g.Refused(Command.DrawReserve(g.Id, 100_001), CommandError.InvalidArgument);
        g.Refused(Command.DrawReserve(g.Id, 9999), CommandError.InvalidArgument);
        Assert.Equal(d, g.Sim.Digest());

        Game off = Lab(5_000_000, c => c.Treasury.Enabled = 0);
        off.Refused(Command.SetAsideReserve(off.Id, 100_000), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.SetAsideReserve(rival.Id, 100_000)).Ok);
    }

    [Fact]
    public void A_debt_is_covered_from_the_reserve_at_the_steeper_fee_and_the_lab_stays_open()
    {
        Game g = Burning(350_000);
        for (int i = 0; i < 80 && !Moves(g).Any(e => e.Pick == 2); i++) g.Weeks(1);
        GameEvent cover = Moves(g).First(e => e.Pick == 2);
        Assert.True(cover.Amount > 0);
        Assert.True(g.Me.Cash >= 0, "cash " + g.Me.Cash);
        Assert.Equal(0, g.Me.WeeksInDebt);
        Assert.False(g.Me.Bankrupt);
        Assert.Contains("to cover a debt", EventText.Describe(g.C, g.State, cover));
    }

    [Fact]
    public void A_lab_without_a_reserve_goes_into_debt_where_one_with_a_reserve_stays_afloat()
    {
        Game without = Burning(0);
        Game with = Burning(350_000);
        without.Weeks(60);
        with.Weeks(60);
        Assert.True(without.Me.Bankrupt || without.Me.WeeksInDebt > 0);
        int debtWeeks = with.Me.WeeksInDebt;
        Assert.True(with.Me.Reserve < 350_000);
        Assert.True(debtWeeks < without.Me.WeeksInDebt || without.Me.Bankrupt);
    }

    [Fact]
    public void The_reserve_counts_in_what_the_lab_would_sell_for()
    {
        Game g = Lab();
        long before = EndingSystem.SellPrice(g.C, g.State, g.Me);
        g.Do(Command.SetAsideReserve(g.Id, 1_000_000));
        long after = EndingSystem.SellPrice(g.C, g.State, g.Me);
        Assert.Equal(before, after);
    }

    [Fact]
    public void Save_and_load_keep_the_reserve_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetAsideReserve(g.Id, 1_000_000));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(1_000_000, back.State.Lab(g.Id).Reserve);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(15);
        for (int i = 0; i < 15; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_nothing_set_aside()
    {
        Game g = Lab();
        g.Do(Command.SetAsideReserve(g.Id, 1_000_000));
        string old = SaveSchemaPretend.At(g.C, 66, g.Sim.Save());
        Assert.DoesNotContain("\"reserve\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Equal(0, lab.Reserve));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_reserve()
    {
        Game g = Lab();
        g.Me.Reserve = 5000;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("reserve"));
        g.Me.Reserve = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("reserve"));
        Game w = Game.New(5);
        w.State.Labs.First(l => l.Id != w.State.PlayerLabId).Reserve = 1;
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("reserve"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab(400_000);
        g.Do(Command.SetAsideReserve(g.Id, 300_000));
        g.Weeks(30);
        g.Try(Command.DrawReserve(g.Id, 20_000));
        g.Weeks(30);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_a_reserve_stays_sound_and_the_books_balance()
    {
        Game g = Lab(3_000_000);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 200; i++)
        {
            g.Weeks(1);
            if (i % 25 == 0) g.Try(Command.SetAsideReserve(g.Id, 200_000));
            if (i % 60 == 30) g.Try(Command.DrawReserve(g.Id, 100_000));
        }
        Assert.NotEmpty(Moves(g));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
