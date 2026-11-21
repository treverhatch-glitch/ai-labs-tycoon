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

/// <summary>A hearing at home asks the player a question; the answer moves heat, trust and the lab's standing.</summary>
public class HearingQuestionTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        });
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static void Hearing(Game g, string bloc = "us")
    {
        g.Me.Bloc(bloc).Level = BlocStanding.Letter;
        g.Me.Bloc(bloc).Attention = g.C.World.Attention.HearingAt + 80;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        g.Me.Ledger.LastWeek.Revenue = 0;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        Assert.Equal(BlocStanding.Hearing, g.Me.Bloc(bloc).Level);
    }

    static void HearingAgain(Game g)
    {
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        Assert.Equal(BlocStanding.Hearing, g.Me.Bloc("us").Level);
    }

    [Fact]
    public void The_shipped_data_has_hearing_questions_the_weekly_chance_never_draws()
    {
        GameConfig c = TestData.Config();
        List<OfficeEventConfig> questions = OfficeEvents.HearingQuestions(c);
        Assert.True(questions.Count >= 4);
        Assert.All(questions, q => Assert.True(q.Choices.Count >= 3));
        Game g = Lab();
        g.Me.OfficeSeen.Clear();
        List<OfficeEventConfig> pool = OfficeEvents.Eligible(g.C, g.Me, out _);
        Assert.NotEmpty(pool);
        Assert.DoesNotContain(pool, e => e.Trigger == OfficeTrigger.Hearing);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void A_hearing_at_home_poses_a_question_and_logs_it()
    {
        Game g = Lab();
        Hearing(g);
        Assert.True(OfficeEvents.IsPending(g.Me));
        OfficeEventConfig q = g.C.Office.Event(g.Me.OfficeEventId);
        Assert.Equal(OfficeTrigger.Hearing, q.Trigger);
        GameEvent posed = g.State.Events.Last(e => e.Kind == EventKind.OfficeEventPosed);
        Assert.Equal(q.IdValue, posed.Ref);
        Assert.Equal(g.State.Tick + g.C.Office.ExpireWeeks, g.Me.OfficeEventExpires);
    }

    [Fact]
    public void The_automatic_reputation_hit_of_the_hearing_is_unchanged()
    {
        Game g = Lab();
        Game twin = Lab(c => c.Office.Events.RemoveAll(e => e.Trigger == OfficeTrigger.Hearing));
        Hearing(g);
        Hearing(twin);
        Assert.False(OfficeEvents.IsPending(twin.Me));
        Assert.Equal(twin.Me.Reputation, g.Me.Reputation);
        Assert.Equal(twin.Me.Bloc("us").Attention, g.Me.Bloc("us").Attention);
    }

    [Fact]
    public void The_answer_moves_what_it_says_and_clears_the_question()
    {
        Game g = Lab();
        Hearing(g);
        OfficeEventConfig q = g.C.Office.Event(g.Me.OfficeEventId);
        OfficeChoiceConfig choice = q.Choices[0];
        int rep = g.Me.Reputation;
        int attention = g.Me.Bloc("us").Attention;
        int repChange = choice.Effects.Where(f => f.Kind == OfficeEffectKind.Reputation).Sum(f => f.Amount);
        int attentionChange = choice.Effects.Where(f => f.Kind == OfficeEffectKind.Attention).Sum(f => f.Amount);
        Assert.True(repChange != 0 || attentionChange != 0, "the first answer should move trust or heat");
        g.Do(Command.AnswerOfficeEvent(g.Id, choice.IdValue));
        Assert.False(OfficeEvents.IsPending(g.Me));
        Assert.Equal((int)MathI.Clamp((long)rep + repChange, 0, 1000), g.Me.Reputation);
        Assert.Equal((int)MathI.Clamp((long)attention + attentionChange, 0, 1000), g.Me.Bloc("us").Attention);
        Assert.Equal(q.IdValue + "|" + choice.IdValue, g.State.Events.Last(e => e.Kind == EventKind.OfficeEventAnswered).Ref);
    }

    [Fact]
    public void An_unanswered_question_lapses_and_changes_nothing()
    {
        Game g = Lab();
        Hearing(g);
        string id = g.Me.OfficeEventId;
        int rep = g.Me.Reputation;
        g.Weeks(g.C.Office.ExpireWeeks + 1);
        Assert.False(OfficeEvents.IsPending(g.Me));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OfficeEventLapsed && e.Ref == id);
        Assert.True(g.Me.Reputation <= rep + g.C.Office.ExpireWeeks + 1, "nothing but the usual drift");
    }

    [Fact]
    public void A_hearing_takes_the_place_of_a_waiting_office_event_which_lapses()
    {
        Game g = Lab(c =>
        {
            c.Office.ChancePermille = 1000;
            c.Office.FirstAfterWeeks = 0;
        });
        g.Weeks(1);
        string waiting = g.Me.OfficeEventId;
        Assert.NotEmpty(waiting);
        Assert.Equal(OfficeTrigger.Random, g.C.Office.Event(waiting).Trigger);
        Hearing(g);
        Assert.NotEqual(waiting, g.Me.OfficeEventId);
        Assert.Equal(OfficeTrigger.Hearing, g.C.Office.Event(g.Me.OfficeEventId).Trigger);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.OfficeEventLapsed && e.Ref == waiting);
    }

    [Fact]
    public void The_questions_rotate_before_one_is_asked_again()
    {
        Game g = Lab();
        int n = OfficeEvents.HearingQuestions(g.C).Count;
        var asked = new List<string>();
        for (int i = 0; i < n; i++)
        {
            HearingAgain(g);
            asked.Add(g.Me.OfficeEventId);
            g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        }
        Assert.Equal(n, asked.Distinct().Count());
        HearingAgain(g);
        Assert.Equal(OfficeTrigger.Hearing, g.C.Office.Event(g.Me.OfficeEventId).Trigger);
    }

    [Fact]
    public void A_hearing_abroad_or_at_a_rival_asks_nothing()
    {
        Game g = Lab();
        Assert.NotEqual("eu", g.Me.HomeBlocId);
        Hearing(g, "eu");
        Assert.False(OfficeEvents.IsPending(g.Me));

        var options = new NewGameOptions { Seed = 1, RivalIds = g.C.Rivals.Take(1).Select(r => r.Id).ToList() };
        Game withRival = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = withRival.State.Labs.First(l => l.Controller != LabController.Player);
        rival.Bloc(rival.HomeBlocId).Attention = withRival.C.World.Attention.HearingAt + 80;
        rival.Bloc(rival.HomeBlocId).Level = BlocStanding.Letter;
        GovernmentSystem.CloseMonth(withRival.C, withRival.State, rival);
        Assert.False(OfficeEvents.IsPending(rival));
    }

    [Fact]
    public void With_no_questions_in_the_data_the_hearing_asks_nothing()
    {
        Game g = Lab(c => c.Office.Events.RemoveAll(e => e.Trigger == OfficeTrigger.Hearing));
        Hearing(g);
        Assert.False(OfficeEvents.IsPending(g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.OfficeEventPosed);
    }

    [Fact]
    public void Every_answer_to_every_question_runs_and_leaves_the_books_balanced()
    {
        GameConfig probe = TestData.Config();
        foreach (OfficeEventConfig q in OfficeEvents.HearingQuestions(probe))
            for (int k = 0; k < q.Choices.Count; k++)
            {
                Game g = Lab();
                g.Me.OfficeEventId = q.IdValue;
                g.Me.OfficeEventExpires = g.State.Tick + 8;
                g.Do(Command.AnswerOfficeEvent(g.Id, q.Choices[k].IdValue));
                Assert.False(OfficeEvents.IsPending(g.Me));
                Assert.InRange(g.Me.Reputation, 0, 1000);
                Assert.InRange(g.Me.Bloc("us").Attention, 0, 1000);
            }
    }
}
