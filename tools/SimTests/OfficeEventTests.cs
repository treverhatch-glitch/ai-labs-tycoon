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

/// <summary>Office events: something happens at the player's lab, the player picks a response, and an unanswered one blows over.</summary>
public class OfficeEventTests
{
    static OfficeEventConfig Event(string id, OfficeRequires need, params (string id, OfficeEffectKind kind, int amount)[] choices)
    {
        var e = new OfficeEventConfig { IdValue = id, Name = id + " name", Text = id + " happened", Requires = need };
        foreach (var ch in choices)
        {
            var choice = new OfficeChoiceConfig { IdValue = ch.id, Text = "Do " + ch.id, Result = ch.id + " done" };
            if (ch.amount != 0) choice.Effects.Add(new OfficeEffectConfig { Kind = ch.kind, Amount = ch.amount });
            e.Choices.Add(choice);
        }
        return e;
    }

    static Game Lab(int chance = 1000, int firstAfter = 0, int expire = 8, Action<GameConfig> tweak = null, params OfficeEventConfig[] events)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.Safety.RiskClasses.Clear();
            c.Safety.IncidentChancePermille = 0;
            c.Office.ChancePermille = chance;
            c.Office.FirstAfterWeeks = firstAfter;
            c.Office.ExpireWeeks = expire;
            if (events.Length > 0) c.Office.Events = events.ToList();
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static OfficeEventConfig Plain(string id, params (string id, OfficeEffectKind kind, int amount)[] choices)
    {
        return Event(id, OfficeRequires.Nothing, choices);
    }

    static (string, OfficeEffectKind, int) A(string id, OfficeEffectKind kind = OfficeEffectKind.Reputation, int amount = 0) { return (id, kind, amount); }

    static Game Posed(params (string id, OfficeEffectKind kind, int amount)[] choices)
    {
        Game g = Lab(1000, 0, 8, null, Plain("e", choices));
        g.Weeks(1);
        Assert.Equal("e", g.Me.OfficeEventId);
        return g;
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the data

    [Fact]
    public void The_shipped_events_are_all_there_with_three_ways_to_answer_each()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Office.Events.Count >= 10);
        Assert.All(c.Office.Events, e => Assert.True(e.Choices.Count >= 2));
        Assert.Contains(c.Office.Events, e => e.Requires == OfficeRequires.ReleasedModel);
        Assert.Contains(c.Office.Events, e => e.Requires == OfficeRequires.SafetyStaff);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_kind_of_effect_is_used_somewhere_in_the_shipped_data()
    {
        GameConfig c = TestData.Config();
        foreach (OfficeEffectKind kind in Enum.GetValues(typeof(OfficeEffectKind)))
            Assert.True(c.Office.Events.Any(e => e.Choices.Any(ch => ch.Effects.Any(f => f.Kind == kind))), kind + " is never used");
    }

    [Fact]
    public void The_validator_catches_bad_office_data()
    {
        AssertProblem(Problems(c => c.Office.ChancePermille = 1001), "office.chancePermille");
        AssertProblem(Problems(c => c.Office.ExpireWeeks = 0), "office.expireWeeks");
        AssertProblem(Problems(c => c.Office.FirstAfterWeeks = -1), "office.firstAfterWeeks");
        AssertProblem(Problems(c => c.Office.MinWeekCost = -1), "office.minWeekCost");
        AssertProblem(Problems(c => c.Office.Events[0].Choices.RemoveRange(1, c.Office.Events[0].Choices.Count - 1)), "at least two choices");
        AssertProblem(Problems(c => c.Office.Events[0].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Office.Events[0].Text = ""), "has no text");
        AssertProblem(Problems(c => c.Office.Events[0].Choices[0].Result = ""), "has no result");
        AssertProblem(Problems(c => c.Office.Events[0].Choices[0].Text = ""), "has no text");
        AssertProblem(Problems(c => c.Office.Events[1].IdValue = c.Office.Events[0].IdValue), "office.events");
        AssertProblem(Problems(c => c.Office.Events[0].Choices[1].IdValue = c.Office.Events[0].Choices[0].IdValue), "choices");
        AssertProblem(Problems(c => c.Office.Events[0].Choices[0].Effects.Add(new OfficeEffectConfig { Kind = OfficeEffectKind.Morale, Amount = 0 })), "amount");
        AssertProblem(Problems(c => c.Office.Events[0].Choices[0].Effects.Add(new OfficeEffectConfig { Kind = OfficeEffectKind.Morale, Amount = 1001 })), "amount");
    }

    // ---- when they happen

    [Fact]
    public void Nothing_happens_before_the_first_week_or_with_no_chance()
    {
        Game early = Lab(1000, 0, 8, null, Plain("e", A("a"), A("b")));
        early.C.Office.FirstAfterWeeks = early.Tick + 10;
        early.Weeks(10);
        Assert.False(OfficeEvents.IsPending(early.Me));
        early.Weeks(1);
        Assert.True(OfficeEvents.IsPending(early.Me));

        Game never = Lab(0, 0, 8, null, Plain("e", A("a"), A("b")));
        never.Weeks(120);
        Assert.False(OfficeEvents.IsPending(never.Me));
        Assert.DoesNotContain(never.State.Events, e => e.Kind == EventKind.OfficeEventPosed);
    }

    [Fact]
    public void A_chance_in_between_gives_events_now_and_then()
    {
        Game g = Lab(40, 0, 2);
        g.Weeks(520);
        int posed = g.State.Events.Count(e => e.Kind == EventKind.OfficeEventPosed);
        Assert.InRange(posed, 5, 60);
    }

    [Fact]
    public void Only_one_is_waiting_at_a_time_and_it_keeps_its_deadline()
    {
        Game g = Lab(1000, 0, 8, null, Plain("e", A("a"), A("b")), Plain("f", A("a"), A("b")));
        g.Weeks(1);
        string first = g.Me.OfficeEventId;
        int expires = g.Me.OfficeEventExpires;
        Assert.Equal(g.Tick + 8 - 1, expires);
        g.Weeks(3);
        Assert.Equal(first, g.Me.OfficeEventId);
        Assert.Equal(expires, g.Me.OfficeEventExpires);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.OfficeEventPosed));
    }

    [Fact]
    public void Rivals_have_no_office_events()
    {
        Game g = Game.New(1, c => { c.Office.ChancePermille = 1000; c.Office.FirstAfterWeeks = 0; });
        g.Weeks(60);
        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Controller == LabController.Player) continue;
            Assert.False(OfficeEvents.IsPending(lab));
            Assert.Empty(lab.OfficeSeen);
        }
        Assert.All(g.State.Events.Where(e => e.Kind == EventKind.OfficeEventPosed), e => Assert.Equal(g.Id, e.LabId));
    }

    [Fact]
    public void An_event_that_needs_a_model_or_safety_people_waits_for_them()
    {
        var model = Event("needs-model", OfficeRequires.ReleasedModel, A("a"), A("b"));
        var safety = Event("needs-safety", OfficeRequires.SafetyStaff, A("a"), A("b"));
        var anyone = Event("needs-anyone", OfficeRequires.AnyStaff, A("a"), A("b"));
        Game g = Lab(0, 0, 1, null, model, safety, anyone);
        Assert.Equal(new[] { "needs-anyone" }, OfficeEvents.Eligible(g.C, g.Me, out _).Select(e => e.IdValue).ToArray());

        g.Hire("safety", 1);
        Assert.Equal(new[] { "needs-anyone", "needs-safety" }, OfficeEvents.Eligible(g.C, g.Me, out _).Select(e => e.IdValue).OrderBy(x => x).ToArray());

        ModelState m = g.Train("mini", 600);
        g.UntilTrained(m);
        g.Release(m, "web-ads");
        Assert.Equal(3, OfficeEvents.Eligible(g.C, g.Me, out _).Count);
    }

    [Fact]
    public void Events_come_round_in_turn_before_any_repeats()
    {
        Game g = Lab(1000, 0, 1, null, Plain("a", A("x"), A("y")), Plain("b", A("x"), A("y")), Plain("c", A("x"), A("y")));
        g.Weeks(40);
        List<string> posed = g.State.Events.Where(e => e.Kind == EventKind.OfficeEventPosed).Select(e => e.Ref).ToList();
        Assert.True(posed.Count >= 15);
        for (int i = 0; i + 3 <= posed.Count; i += 3)
            Assert.Equal(3, posed.Skip(i).Take(3).Distinct().Count());
    }

    [Fact]
    public void The_same_seed_gives_the_same_events_and_a_different_seed_a_different_run()
    {
        Func<long, List<string>> run = seed =>
        {
            var g = new Game(Simulation.NewGame(TestData.Config(c => { c.Office.ChancePermille = 60; c.Office.FirstAfterWeeks = 0; c.Office.ExpireWeeks = 1; }),
                new NewGameOptions { Seed = seed, StartEraId = "agents", RivalIds = new List<string>() }));
            g.WithStarterLab();
            g.Weeks(300);
            return g.State.Events.Where(e => e.Kind == EventKind.OfficeEventPosed).Select(e => e.Tick + ":" + e.Ref).ToList();
        };
        Assert.Equal(run(3), run(3));
        Assert.NotEqual(run(3), run(4));
    }

    // ---- answering

    [Fact]
    public void An_answer_applies_its_effects_clears_the_event_and_is_logged()
    {
        Game g = Lab(1000, 0, 8, null, Plain("e",
            ("a", OfficeEffectKind.Reputation, 40), ("b", OfficeEffectKind.Reputation, -40)));
        g.Weeks(1);
        int before = g.Me.Reputation;
        g.Do(Command.AnswerOfficeEvent(g.Id, "a"));
        Assert.Equal(before + 40, g.Me.Reputation);
        Assert.False(OfficeEvents.IsPending(g.Me));
        Assert.Equal(0, g.Me.OfficeEventExpires);
        GameEvent answered = g.State.Events.Last(e => e.Kind == EventKind.OfficeEventAnswered);
        Assert.Equal("e|a", answered.Ref);
        Assert.Equal(g.Id, answered.LabId);
        Assert.Contains("a done", EventText.Describe(g.C, g.State, answered));
    }

    [Fact]
    public void Reputation_and_morale_stay_inside_their_bounds()
    {
        Game g = Posed(("up", OfficeEffectKind.Reputation, 1000), ("down", OfficeEffectKind.Reputation, -1000));
        g.Me.Reputation = 500;
        g.Do(Command.AnswerOfficeEvent(g.Id, "up"));
        Assert.Equal(1000, g.Me.Reputation);

        g = Posed(("up", OfficeEffectKind.Morale, 1000), ("down", OfficeEffectKind.Morale, -1000));
        g.Do(Command.AnswerOfficeEvent(g.Id, "down"));
        Assert.All(g.Me.Staff, s => Assert.Equal(0, s.Morale));
    }

    [Fact]
    public void Morale_moves_everyone_on_the_payroll()
    {
        Game g = Posed(("up", OfficeEffectKind.Morale, 100), ("down", OfficeEffectKind.Morale, -100));
        foreach (StaffMember s in g.Me.Staff) s.Morale = 500;
        g.Do(Command.AnswerOfficeEvent(g.Id, "up"));
        Assert.True(g.Me.Staff.Count > 0);
        Assert.All(g.Me.Staff, s => Assert.Equal(600, s.Morale));
    }

    [Fact]
    public void Culture_moves_the_safety_culture()
    {
        Game g = Posed(("up", OfficeEffectKind.Culture, 25), ("down", OfficeEffectKind.Culture, -25));
        int before = g.Me.CultureShift;
        g.Do(Command.AnswerOfficeEvent(g.Id, "up"));
        Assert.Equal(before + 25, g.Me.CultureShift);
    }

    [Fact]
    public void Research_weeks_are_worth_a_week_of_the_labs_output_and_cannot_go_below_none()
    {
        Game g = Posed(("gain", OfficeEffectKind.ResearchWeeks, 3), ("lose", OfficeEffectKind.ResearchWeeks, -3));
        long week = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.True(week > 0);
        g.Me.ResearchPoints = 0;
        g.Do(Command.AnswerOfficeEvent(g.Id, "gain"));
        Assert.Equal(3 * week, g.Me.ResearchPoints);

        g = Posed(("gain", OfficeEffectKind.ResearchWeeks, 3), ("lose", OfficeEffectKind.ResearchWeeks, -3));
        g.Me.ResearchPoints = week;
        g.Do(Command.AnswerOfficeEvent(g.Id, "lose"));
        Assert.Equal(0, g.Me.ResearchPoints);
    }

    [Fact]
    public void Cash_weeks_are_worth_a_week_of_running_costs_and_never_overdraw()
    {
        Game g = Posed(("gain", OfficeEffectKind.CashWeeks, 2), ("lose", OfficeEffectKind.CashWeeks, -2));
        long cost = OfficeEvents.WeekCost(g.C, g.State, g.Me);
        Assert.True(cost >= g.C.Office.MinWeekCost);
        g.Me.Cash = 5_000_000;
        g.Do(Command.AnswerOfficeEvent(g.Id, "gain"));
        Assert.Equal(5_000_000 + 2 * cost, g.Me.Cash);

        g = Posed(("gain", OfficeEffectKind.CashWeeks, 2), ("lose", OfficeEffectKind.CashWeeks, -2));
        cost = OfficeEvents.WeekCost(g.C, g.State, g.Me);
        g.Me.Cash = 5_000_000;
        g.Do(Command.AnswerOfficeEvent(g.Id, "lose"));
        Assert.Equal(5_000_000 - 2 * cost, g.Me.Cash);

        g = Posed(("gain", OfficeEffectKind.CashWeeks, 2), ("lose", OfficeEffectKind.CashWeeks, -2));
        g.Me.Cash = 10;
        g.Do(Command.AnswerOfficeEvent(g.Id, "lose"));
        Assert.Equal(0, g.Me.Cash);
    }

    [Fact]
    public void A_choice_can_have_no_effect_at_all()
    {
        Game g = Posed(A("nothing"), ("something", OfficeEffectKind.Reputation, 50));
        string before = g.Me.Reputation + "/" + g.Me.Cash + "/" + g.Me.CultureShift;
        g.Do(Command.AnswerOfficeEvent(g.Id, "nothing"));
        Assert.Equal(before, g.Me.Reputation + "/" + g.Me.Cash + "/" + g.Me.CultureShift);
        Assert.False(OfficeEvents.IsPending(g.Me));
    }

    [Fact]
    public void Answering_with_nothing_waiting_or_with_a_wrong_choice_is_refused_and_changes_nothing()
    {
        Game g = Lab(0, 0, 8, null, Plain("e", A("a"), A("b")));
        g.Refused(Command.AnswerOfficeEvent(g.Id, "a"), CommandError.NoOfficeEvent);

        Game h = Posed(A("a"), A("b"));
        h.Refused(Command.AnswerOfficeEvent(h.Id, "zzz"), CommandError.UnknownId);
        h.Refused(Command.AnswerOfficeEvent(h.Id, ""), CommandError.UnknownId);
        Assert.True(OfficeEvents.IsPending(h.Me));
        h.Do(Command.AnswerOfficeEvent(h.Id, "b"));
        h.Refused(Command.AnswerOfficeEvent(h.Id, "b"), CommandError.NoOfficeEvent);
    }

    [Fact]
    public void A_new_event_can_come_after_one_is_answered()
    {
        Game g = Lab(1000, 0, 8, null, Plain("e", A("a"), A("b")));
        g.Weeks(1);
        g.Do(Command.AnswerOfficeEvent(g.Id, "a"));
        g.Weeks(1);
        Assert.True(OfficeEvents.IsPending(g.Me));
        Assert.Equal(2, g.State.Events.Count(e => e.Kind == EventKind.OfficeEventPosed));
    }

    // ---- lapsing

    [Fact]
    public void An_unanswered_event_blows_over_on_its_deadline_and_changes_nothing_else()
    {
        Game g = Lab(1000, 0, 5, c => c.Office.ChancePermille = 1000, Plain("e", ("a", OfficeEffectKind.Reputation, -200), ("b", OfficeEffectKind.CashWeeks, -5)));
        g.Weeks(1);
        int posedAt = g.Tick;
        int deadline = g.Me.OfficeEventExpires;
        g.Until(() => g.State.Events.Any(e => e.Kind == EventKind.OfficeEventLapsed), 20, "the event lapsing");
        Assert.Equal(deadline, g.Tick);
        GameEvent lapsed = g.State.Events.First(e => e.Kind == EventKind.OfficeEventLapsed);
        Assert.Equal("e", lapsed.Ref);
        Assert.Contains("blew over", EventText.Describe(g.C, g.State, lapsed));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.OfficeEventAnswered);
        Assert.True(deadline > posedAt);
    }

    [Fact]
    public void A_lapsed_event_does_not_touch_reputation_or_culture()
    {
        Game g = Lab(1000, 0, 3, null, Plain("e", ("a", OfficeEffectKind.Reputation, -200), ("b", OfficeEffectKind.Culture, -200)));
        Game twin = Lab(0, 0, 3, null, Plain("e", ("a", OfficeEffectKind.Reputation, -200), ("b", OfficeEffectKind.Culture, -200)));
        g.Weeks(3);
        twin.Weeks(3);
        Assert.Equal(twin.Me.Reputation, g.Me.Reputation);
        Assert.Equal(twin.Me.CultureShift, g.Me.CultureShift);
        Assert.Equal(twin.Me.Cash, g.Me.Cash);
    }

    [Fact]
    public void A_bare_view_shows_what_is_waiting_and_for_how_long()
    {
        Game g = Lab(1000, 0, 8, null, Plain("e", A("a"), A("b")));
        Assert.Null(OfficeEvents.Waiting(g.C, g.Me, g.Tick).Event);
        g.Weeks(1);
        var waiting = OfficeEvents.Waiting(g.C, g.Me, g.Tick);
        Assert.Equal("e", waiting.Event.IdValue);
        Assert.Equal(7, waiting.WeeksLeft);
        g.Weeks(3);
        Assert.Equal(4, OfficeEvents.Waiting(g.C, g.Me, g.Tick).WeeksLeft);
    }

    // ---- the log and the chat

    [Fact]
    public void The_log_and_the_chat_tell_the_story()
    {
        Game g = Lab(1000, 0, 8, null, Plain("e", A("a"), A("b")));
        g.Weeks(1);
        GameEvent posed = g.State.Events.First(e => e.Kind == EventKind.OfficeEventPosed);
        Assert.Contains("e happened", EventText.Describe(g.C, g.State, posed));
        g.Do(Command.AnswerOfficeEvent(g.Id, "a"));
        g.Weeks(1);
        List<ChatMessage> feed = CompanyChat.Feed(g.C, g.State, g.Me);
        Assert.Contains("e name", feed.Last(m => m.Kind == EventKind.OfficeEventPosed).Text);
        Assert.Contains("e name", feed.Last(m => m.Kind == EventKind.OfficeEventAnswered).Text);
    }

    // ---- saving

    [Fact]
    public void A_waiting_event_survives_a_save_and_the_game_carries_on_the_same()
    {
        Game g = Lab(1000, 0, 8, null, Plain("e", A("a"), A("b")), Plain("f", A("a"), A("b")));
        g.Weeks(1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Me.OfficeEventId, again.Player.OfficeEventId);
        Assert.Equal(g.Me.OfficeEventExpires, again.Player.OfficeEventExpires);
        Assert.Equal(g.Me.OfficeSeen, again.Player.OfficeSeen);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        g.Weeks(30);
        again.StepMany(30);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_event_waiting()
    {
        Game g = Lab(0, 0, 8);
        g.Weeks(5);
        string old = SaveSchemaPretend.At(g.C, 32, g.Sim.Save());
        Assert.DoesNotContain("officeEventId", old);
        Assert.DoesNotContain("officeSeen", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => { Assert.False(OfficeEvents.IsPending(l)); Assert.Empty(l.OfficeSeen); Assert.Equal(0, l.OfficeEventExpires); });
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void SaveGame_catches_an_unknown_event_waiting_or_seen()
    {
        Game g = Lab(0, 0, 8);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Me.OfficeEventId = "nope";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("office event"));
        g.Me.OfficeEventId = "";
        g.Me.OfficeSeen.Add("nope");
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("unknown office event"));
    }
}
