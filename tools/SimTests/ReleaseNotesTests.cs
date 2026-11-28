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

/// <summary>The first release of a model asks the player how the release notes should read.</summary>
public class ReleaseNotesTests
{
    static readonly string[] Notes = { "notes-changelog", "notes-known-issues", "notes-joke" };

    static Game Lab(Action<GameConfig> tweak = null, bool rivals = false)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents" };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static ModelState Trained(Game g)
    {
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        return m;
    }

    [Fact]
    public void The_three_release_events_ship_with_three_answers_and_the_data_is_clean()
    {
        GameConfig c = TestData.Config();
        foreach (string id in Notes)
        {
            OfficeEventConfig e = c.Office.Event(id);
            Assert.NotNull(e);
            Assert.Equal(OfficeTrigger.Release, e.Trigger);
            Assert.Equal(3, e.Choices.Count);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_first_release_of_a_model_poses_the_release_notes()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        Assert.False(OfficeEvents.IsPending(g.Me));
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        Assert.True(OfficeEvents.IsPending(g.Me));
        Assert.Contains(g.Me.OfficeEventId, Notes);
        Assert.Equal(g.Me.OfficeEventId, g.State.Events.Last(e => e.Kind == EventKind.OfficeEventPosed).Ref);
    }

    [Fact]
    public void Adding_a_sector_to_a_model_already_on_sale_poses_nothing()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        int posed = g.State.Events.Count(e => e.Kind == EventKind.OfficeEventPosed);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-bots"));
        Assert.False(OfficeEvents.IsPending(g.Me));
        Assert.Equal(posed, g.State.Events.Count(e => e.Kind == EventKind.OfficeEventPosed));
    }

    [Fact]
    public void An_event_already_waiting_is_never_replaced_by_the_notes()
    {
        Game g = Lab();
        g.Me.OfficeEventId = "open-letter";
        g.Me.OfficeEventExpires = g.State.Tick + 8;
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        Assert.Equal("open-letter", g.Me.OfficeEventId);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.OfficeEventLapsed);
    }

    [Fact]
    public void The_notes_come_round_in_turn_across_releases()
    {
        Game g = Lab();
        var seen = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            ModelState m = Trained(g);
            g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
            seen.Add(g.Me.OfficeEventId);
            g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        }
        Assert.Equal(3, seen.Distinct().Count());
    }

    [Fact]
    public void The_release_events_are_never_drawn_by_the_weekly_chance()
    {
        Game g = Lab(c => { c.Office.ChancePermille = 1000; c.Office.FirstAfterWeeks = 0; });
        List<string> ids = OfficeEvents.Eligible(g.C, g.Me, out _).Select(e => e.IdValue).ToList();
        foreach (string id in Notes) Assert.DoesNotContain(id, ids);
    }

    [Fact]
    public void Rivals_have_no_release_notes()
    {
        Game g = Lab(rivals: true);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        OfficeEvents.OnRelease(g.C, g.State, rival);
        Assert.False(OfficeEvents.IsPending(rival));
    }

    [Fact]
    public void The_honest_answer_lifts_culture_and_costs_a_little_reputation()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Me.OfficeEventId = "notes-changelog";
        int culture = SafetyCulture.Level(g.C, g.Me);
        int rep = g.Me.Reputation;
        g.Do(Command.AnswerOfficeEvent(g.Id, "honest"));
        Assert.Equal((int)MathI.Clamp((long)culture + 10, 0, 1000), SafetyCulture.Level(g.C, g.Me));
        Assert.Equal(rep - 5, g.Me.Reputation);
    }

    [Fact]
    public void Letting_the_model_write_the_notes_adds_corruption()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        g.Me.OfficeEventId = "notes-changelog";
        m.Corruption = 100;
        g.Do(Command.AnswerOfficeEvent(g.Id, "model"));
        Assert.Equal(130, m.Corruption);
    }

    [Fact]
    public void A_lapsed_note_changes_nothing_but_the_log()
    {
        Game g = Lab();
        ModelState m = Trained(g);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        int wait = (int)(g.Me.OfficeEventExpires - g.Tick + 1);
        g.Weeks(wait);
        Assert.False(OfficeEvents.IsPending(g.Me));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OfficeEventLapsed);
    }

    [Fact]
    public void Every_release_note_answer_is_accepted_and_logged()
    {
        foreach (string id in Notes)
            foreach (OfficeChoiceConfig choice in TestData.Config().Office.Event(id).Choices)
            {
                Game g = Lab();
                ModelState m = Trained(g);
                g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
                g.Me.OfficeEventId = id;
                g.Do(Command.AnswerOfficeEvent(g.Id, choice.IdValue));
                Assert.False(OfficeEvents.IsPending(g.Me), id + "/" + choice.IdValue);
                Assert.Equal(id + "|" + choice.IdValue, g.State.Events.Last(e => e.Kind == EventKind.OfficeEventAnswered).Ref);
            }
    }
}
