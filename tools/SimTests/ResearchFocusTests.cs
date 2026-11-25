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

/// <summary>Research focus: one direction costs less research, the others cost more, and changing it costs cash and time.</summary>
public class ResearchFocusTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, gpus: 4);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static TechConfig T(Game g, string id) { return g.C.Tech(id); }

    static long Cost(Game g, string id) { return LabQueries.TechCost(g.C, g.State, g.Me, T(g, id)); }

    // ---- nothing happens by itself

    [Fact]
    public void Every_tech_costs_what_it_says_until_the_player_picks_a_focus()
    {
        Game g = Lab();
        Assert.Equal("", g.Me.ResearchFocus);
        foreach (TechConfig tech in g.C.Research.Techs) Assert.Equal(tech.Cost, LabQueries.TechCost(g.C, g.State, g.Me, tech));
        g.Weeks(30);
        Assert.Equal("", g.Me.ResearchFocus);
        Assert.Null(ResearchFocus.Current(g.C, g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.FocusChanged);
    }

    // ---- the command

    [Fact]
    public void Picking_a_focus_costs_the_data_s_cash_locks_it_and_logs_it()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        long training = g.Me.Ledger.Lifetime.Training;
        g.Do(Command.SetResearchFocus(g.Id, "alignment"));
        Assert.Equal("alignment", g.Me.ResearchFocus);
        Assert.Equal(g.Tick + 26, g.Me.FocusLockedUntil);
        Assert.Equal(cash - 50_000, g.Me.Cash);
        Assert.Equal(training - 50_000, g.Me.Ledger.Lifetime.Training);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.FocusChanged);
        Assert.Equal(1, e.Pick);
        Assert.Equal(50_000, e.Amount);
        Assert.Contains("Alignment first", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void An_unknown_focus_the_same_focus_and_a_change_inside_the_lock_are_refused()
    {
        Game g = Lab();
        g.Refused(Command.SetResearchFocus(g.Id, "vibes"), CommandError.UnknownId);
        g.Refused(Command.SetResearchFocus(g.Id, ""), CommandError.InvalidArgument);
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        long cash = g.Me.Cash;
        g.Refused(Command.SetResearchFocus(g.Id, "frontier"), CommandError.InvalidArgument);
        g.Refused(Command.SetResearchFocus(g.Id, "efficiency"), CommandError.FocusLocked);
        g.Refused(Command.SetResearchFocus(g.Id, ""), CommandError.FocusLocked);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal("frontier", g.Me.ResearchFocus);
        g.Weeks(25);
        g.Refused(Command.SetResearchFocus(g.Id, "efficiency"), CommandError.FocusLocked);
        g.Weeks(1);
        g.Do(Command.SetResearchFocus(g.Id, "efficiency"));
        Assert.Equal("efficiency", g.Me.ResearchFocus);
    }

    [Fact]
    public void Dropping_the_focus_costs_the_same_and_puts_every_tech_back_to_its_price()
    {
        Game g = Lab(c => c.Focus.LockWeeks = 0);
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        Assert.NotEqual(T(g, "minimax").Cost, Cost(g, "minimax"));
        long cash = g.Me.Cash;
        g.Do(Command.SetResearchFocus(g.Id, ""));
        Assert.Equal(cash - 50_000, g.Me.Cash);
        Assert.Equal(T(g, "minimax").Cost, Cost(g, "minimax"));
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.FocusChanged).Pick);
        Assert.Contains("dropped", EventText.Describe(g.C, g.State, g.State.Events.Last(e => e.Kind == EventKind.FocusChanged)));
    }

    [Fact]
    public void With_no_lock_the_focus_can_change_every_week()
    {
        Game g = Lab(c => c.Focus.LockWeeks = 0);
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        g.Do(Command.SetResearchFocus(g.Id, "alignment"));
        Assert.Equal("alignment", g.Me.ResearchFocus);
    }

    [Fact]
    public void Too_little_cash_is_refused_and_changes_nothing()
    {
        Game g = Lab(rich: false);
        g.Me.Cash = 49_999;
        g.Refused(Command.SetResearchFocus(g.Id, "frontier"), CommandError.NotEnoughCash);
        Assert.Equal("", g.Me.ResearchFocus);
        Assert.Equal(49_999, g.Me.Cash);
        Assert.Equal(0, g.Me.FocusLockedUntil);
    }

    [Fact]
    public void A_free_change_works_with_no_cash_at_all()
    {
        Game g = Lab(c => c.Focus.SwitchCost = 0, rich: false);
        g.Me.Cash = 0;
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        Assert.Equal("frontier", g.Me.ResearchFocus);
    }

    [Fact]
    public void A_rival_a_bankrupt_lab_and_a_game_without_options_refuse_it()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.SetResearchFocus(rival.Id, "frontier"), CommandError.InvalidArgument);
        Game broke = Lab();
        broke.Me.Bankrupt = true;
        broke.Refused(Command.SetResearchFocus(broke.Id, "frontier"), CommandError.LabBankrupt);
        Game off = Lab(c => c.Focus.Options.Clear());
        off.Refused(Command.SetResearchFocus(off.Id, "frontier"), CommandError.InvalidArgument);
        Assert.False(ResearchFocus.Enabled(off.C));
    }

    // ---- what it does to the price

    [Fact]
    public void A_tech_in_the_picked_direction_costs_a_fifth_less_and_the_others_a_tenth_more()
    {
        Game g = Lab();
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        Assert.Equal(T(g, "minimax").Cost * 800 / 1000, Cost(g, "minimax"));
        Assert.Equal(T(g, "regression-tests").Cost * 1100 / 1000, Cost(g, "regression-tests"));
        Assert.Equal(T(g, "beowulf-clusters").Cost * 1100 / 1000, Cost(g, "beowulf-clusters"));
    }

    [Fact]
    public void Each_direction_discounts_its_own_techs()
    {
        foreach (var pair in new[] { ("frontier", "minimax"), ("alignment", "regression-tests"), ("efficiency", "beowulf-clusters") })
        {
            Game g = Lab();
            g.Do(Command.SetResearchFocus(g.Id, pair.Item1));
            Assert.Equal(T(g, pair.Item2).Cost * 800 / 1000, Cost(g, pair.Item2));
            foreach (var other in new[] { "minimax", "regression-tests", "beowulf-clusters" }.Where(x => x != pair.Item2))
                Assert.Equal(T(g, other).Cost * 1100 / 1000, Cost(g, other));
        }
    }

    [Fact]
    public void A_tech_in_two_directions_is_discounted_under_either()
    {
        TechConfig both = TestData.Config().Tech("rlhf");
        Assert.Contains(both.Effects, e => e.Kind == TechEffectKind.CapabilityPermille);
        Assert.Contains(both.Effects, e => e.Kind == TechEffectKind.AlignmentPermille);
        foreach (string focus in new[] { "frontier", "alignment" })
        {
            Game g = Lab();
            g.Do(Command.SetResearchFocus(g.Id, focus));
            Assert.Equal(both.Cost * 800 / 1000, Cost(g, "rlhf"));
        }
        Game eff = Lab();
        eff.Do(Command.SetResearchFocus(eff.Id, "efficiency"));
        Assert.Equal(both.Cost * 1100 / 1000, Cost(eff, "rlhf"));
    }

    [Fact]
    public void A_tech_with_no_direction_never_changes_price()
    {
        Game g = Lab();
        foreach (string focus in new[] { "frontier", "alignment", "efficiency" })
        {
            g.Me.FocusLockedUntil = 0;
            g.Do(Command.SetResearchFocus(g.Id, focus));
            Assert.Equal(T(g, "hbm-memory").Cost, Cost(g, "hbm-memory"));
        }
    }

    [Fact]
    public void Researching_spends_the_focused_price_and_a_lab_can_afford_one_tech_but_not_the_other()
    {
        Game g = Lab();
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        long dear = Cost(g, "regression-tests");
        long cheap = Cost(g, "minimax");
        Assert.True(cheap < T(g, "minimax").Cost && dear > T(g, "regression-tests").Cost);
        g.Me.ResearchPoints = dear - 1;
        g.Refused(Command.ResearchTech(g.Id, "regression-tests"), CommandError.NotEnoughResearch);
        g.Me.ResearchPoints = cheap;
        g.Do(Command.ResearchTech(g.Id, "minimax"));
        Assert.Equal(0, g.Me.ResearchPoints);
        Assert.Contains("minimax", g.Me.Techs);
    }

    [Fact]
    public void The_focus_stacks_with_the_cheaper_tech_a_publishing_rival_gives()
    {
        var options = new NewGameOptions { Seed = 1, RivalIds = new List<string> { "anthoptics" } };
        Game g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState anth = g.State.Labs.First(l => l.RivalId == "anthoptics");
        TechConfig tech = g.C.Tech("regression-tests");
        g.Me.Techs.Remove(tech.Id);
        if (!anth.Techs.Contains(tech.Id)) anth.Techs.Add(tech.Id);
        long published = tech.Cost * 700 / 1000;
        Assert.Equal(published, LabQueries.TechCost(g.C, g.State, g.Me, tech));
        g.Me.Cash = 1_000_000_000;
        g.Do(Command.SetResearchFocus(g.Id, "alignment"));
        Assert.Equal(published * 800 / 1000, LabQueries.TechCost(g.C, g.State, g.Me, tech));
    }

    [Fact]
    public void The_price_never_falls_below_one_point()
    {
        Game g = Lab(c =>
        {
            c.Focus.DiscountPermille = 900;
            c.Research.Techs.First(t => t.Id == "minimax").Cost = 5;
        });
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        Assert.Equal(1, Cost(g, "minimax"));
    }

    [Fact]
    public void A_focus_the_data_no_longer_has_changes_nothing()
    {
        Game g = Lab();
        g.Me.ResearchFocus = "gone";
        Assert.Null(ResearchFocus.Current(g.C, g.Me));
        Assert.Equal(T(g, "minimax").Cost, Cost(g, "minimax"));
    }

    [Fact]
    public void A_rival_with_a_tech_to_buy_pays_full_price_whatever_the_player_picks()
    {
        Game g = Game.New(2);
        g.Me.Cash = 1_000_000_000;
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        foreach (TechConfig t in g.C.Research.Techs) Assert.Equal(t.Cost, LabQueries.TechCost(g.C, g.State, rival, t));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_focus_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.SetResearchFocus(g.Id, "efficiency"));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal("efficiency", back.Player.ResearchFocus);
        Assert.Equal(g.Me.FocusLockedUntil, back.Player.FocusLockedUntil);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => c.Focus.SwitchCost = 1_000);
        g.Autopilot(20);
        Assert.True(g.Try(Command.SetResearchFocus(g.Id, "alignment")).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal("alignment", again.Player.ResearchFocus);
    }

    [Fact]
    public void An_older_save_loads_with_no_focus()
    {
        Game g = Lab();
        g.Do(Command.SetResearchFocus(g.Id, "frontier"));
        string old = SaveSchemaPretend.At(g.C, 51, g.Sim.Save());
        Assert.DoesNotContain("researchFocus", old);
        Assert.DoesNotContain("focusLockedUntil", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal("", lab.ResearchFocus); Assert.Equal(0, lab.FocusLockedUntil); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_a_focus_the_data_does_not_have_or_a_negative_lock_is_reported()
    {
        Game g = Lab();
        g.Me.ResearchFocus = "gone";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research focus"));
        g.Me.ResearchFocus = "frontier";
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("research focus"));
        g.Me.FocusLockedUntil = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("research focus"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(3, c.Focus.Options.Count);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Focus.DiscountPermille = 901;
        c.Focus.PenaltyPermille = 1001;
        c.Focus.SwitchCost = -1;
        c.Focus.LockWeeks = 521;
        c.Focus.Options.Add(new FocusOptionConfig { Id = "frontier", Name = "Again", Kinds = new List<string> { "CapabilityPermille" } });
        c.Focus.Options.Add(new FocusOptionConfig { Id = "empty", Name = "Empty", Kinds = new List<string>() });
        c.Focus.Options.Add(new FocusOptionConfig { Id = "odd", Name = "Odd", Kinds = new List<string> { "Vibes" } });
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "discountPermille", "penaltyPermille", "switchCost", "lockWeeks" })
            Assert.Contains(problems, p => p.Contains("focus." + key));
        Assert.Contains(problems, p => p.Contains("repeats one"));
        Assert.Contains(problems, p => p.Contains("covers no tech effect"));
        Assert.Contains(problems, p => p.Contains("unknown tech effect"));
    }

    [Fact]
    public void Every_tech_effect_that_costs_research_is_in_some_direction()
    {
        GameConfig c = TestData.Config();
        foreach (TechConfig tech in c.Research.Techs)
        {
            if (tech.Effects.Count == 0) continue;
            Assert.True(ResearchFocus.InAnyDirection(c, tech), tech.Id + " is in no direction");
        }
    }

    // ---- whole games

    [Fact]
    public void The_autopilot_and_the_rivals_never_pick_a_focus()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal("", lab.ResearchFocus));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.FocusChanged);
    }

    [Fact]
    public void A_focused_autopilot_game_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        g.Autopilot(40);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        string[] focuses = { "frontier", "alignment", "efficiency" };
        int changes = 0;
        for (int i = 0; i < 240; i++)
        {
            g.Autopilot(1);
            if (i % 30 == 0 && g.Me.Cash > 200_000 && g.Try(Command.SetResearchFocus(g.Id, focuses[changes % 3])).Ok) changes++;
        }
        Assert.True(changes >= 2);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Equal(changes, g.State.Events.Count(e => e.Kind == EventKind.FocusChanged));
    }
}
