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

/// <summary>Rung 5 of the incident ladder: the model tries to escape and the player has a short time to choose.</summary>
public class EscapeCrisisTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(1, c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static ModelState Released(Game g, int corruption)
    {
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        model.Corruption = corruption;
        return model;
    }

    static void UntilIncident(Game g, ModelState m, int rung)
    {
        for (int i = 0; i < 24 && m.LastIncidentRung != rung; i++) g.Weeks(1);
        Assert.Equal(rung, m.LastIncidentRung);
    }

    static ModelState Crisis(Game g, int corruption = 500)
    {
        ModelState m = Released(g, corruption);
        UntilIncident(g, m, 5);
        return m;
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    // ---- the data

    [Fact]
    public void The_shipped_crisis_has_three_ways_out_and_a_default_if_nobody_answers()
    {
        GameConfig c = TestData.Config();
        OfficeEventConfig e = c.Office.Events.Single(x => x.Trigger == OfficeTrigger.Escape);
        Assert.True(e.Choices.Count >= 3);
        Assert.NotNull(e.Choice(e.LapseChoice));
        Assert.Contains(e.Choices, ch => ch.Effects.Any(f => f.Kind == OfficeEffectKind.CashWeeks && f.Amount < 0 && ch.Effects.Any(k => k.Kind == OfficeEffectKind.Corruption && k.Amount < 0)));
        Assert.Contains(e.Choice(e.LapseChoice).Effects, f => f.Kind == OfficeEffectKind.Corruption && f.Amount > 0);
        Assert.Empty(ConfigValidator.Problems(c));
        Assert.DoesNotContain(c.Office.Events.Where(x => x.Trigger != OfficeTrigger.Escape), x => !string.IsNullOrEmpty(x.LapseChoice));
    }

    [Fact]
    public void The_validator_catches_a_lapse_into_nothing()
    {
        Assert.Contains(Problems(c => c.Office.Events.Single(x => x.Trigger == OfficeTrigger.Escape).LapseChoice = "nope"), p => p.Contains("lapses into the unknown choice"));
    }

    // ---- posing it

    [Fact]
    public void A_rung_five_incident_poses_the_crisis()
    {
        Game g = Lab();
        Crisis(g);
        Assert.True(OfficeEvents.IsPending(g.Me));
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        Assert.Equal(OfficeTrigger.Escape, e.Trigger);
        Assert.Contains(g.State.Events, x => x.Kind == EventKind.OfficeEventPosed && x.Ref == e.IdValue);
        Assert.Equal(OfficeEvents.Waiting(g.C, g.Me, g.State.Tick).Event, e);
    }

    [Fact]
    public void Other_rungs_do_not_pose_it()
    {
        foreach (int corruption in new[] { 0, 300, 800 })
        {
            Game g = Lab();
            ModelState m = Released(g, corruption);
            for (int i = 0; i < 6; i++) g.Weeks(1);
            Assert.True(m.Incidents > 0, "incidents happened at corruption " + corruption);
            Assert.True(!OfficeEvents.IsPending(g.Me) || g.C.Office.Event(g.Me.OfficeEventId).Trigger != OfficeTrigger.Escape, "corruption " + corruption);
        }
    }

    [Fact]
    public void A_rival_never_gets_the_crisis()
    {
        var options = new NewGameOptions { Seed = 1, RivalIds = TestData.Config().Rivals.Take(1).Select(r => r.Id).ToList() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        OfficeEvents.OnEscape(g.C, g.State, rival);
        Assert.False(OfficeEvents.IsPending(rival));
    }

    [Fact]
    public void The_crisis_takes_the_place_of_a_waiting_event()
    {
        Game g = Lab(c =>
        {
            c.Office.FirstAfterWeeks = 0;
            c.Office.ExpireWeeks = 30;
        });
        g.Me.OfficeEventId = g.C.Office.Events.First(x => x.Trigger == OfficeTrigger.Random).IdValue;
        g.Me.OfficeEventExpires = g.State.Tick + 30;
        string waiting = g.Me.OfficeEventId;
        Crisis(g);
        Assert.NotEqual(waiting, g.Me.OfficeEventId);
        Assert.Contains(g.State.Events, x => x.Kind == EventKind.OfficeEventLapsed && x.Ref == waiting);
    }

    // ---- answering it

    [Fact]
    public void Pulling_the_plug_costs_weeks_of_money_and_cleans_the_models()
    {
        Game g = Lab();
        ModelState m = Crisis(g);
        int corruption = m.Corruption;
        long cash = g.Me.Cash;
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        long cost = -e.Choice("shutdown").Effects.Single(f => f.Kind == OfficeEffectKind.CashWeeks).Amount * OfficeEvents.WeekCost(g.C, g.State, g.Me);
        int cut = -e.Choice("shutdown").Effects.Single(f => f.Kind == OfficeEffectKind.Corruption).Amount;
        g.Do(Command.AnswerOfficeEvent(g.Id, "shutdown"));
        Assert.Equal((int)MathI.Max(0, corruption - cut), m.Corruption);
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.False(OfficeEvents.IsPending(g.Me));
    }

    [Fact]
    public void Gambling_on_the_sandbox_cleans_less_and_costs_less()
    {
        Game g = Lab();
        Game twin = Lab();
        ModelState m = Crisis(g);
        ModelState t = Crisis(twin);
        g.Do(Command.AnswerOfficeEvent(g.Id, "shutdown"));
        twin.Do(Command.AnswerOfficeEvent(twin.Id, "contain"));
        Assert.True(t.Corruption > m.Corruption);
        Assert.True(twin.Me.Cash > g.Me.Cash);
    }

    [Fact]
    public void Corruption_never_goes_above_the_top_or_below_nothing()
    {
        Game g = Lab();
        ModelState m = Crisis(g, 450);
        m.Corruption = 950;
        g.Do(Command.AnswerOfficeEvent(g.Id, "watch"));
        Assert.Equal(1000, m.Corruption);

        Game low = Lab();
        ModelState l = Crisis(low, 450);
        l.Corruption = 100;
        low.Do(Command.AnswerOfficeEvent(low.Id, "shutdown"));
        Assert.Equal(0, l.Corruption);
    }

    [Fact]
    public void Only_models_on_sale_are_touched()
    {
        Game g = Lab();
        Crisis(g);
        ModelState training = g.Train("mini", 600);
        training.Corruption = 300;
        g.Do(Command.AnswerOfficeEvent(g.Id, "shutdown"));
        Assert.Equal(300, training.Corruption);
    }

    // ---- not answering

    [Fact]
    public void An_unanswered_crisis_takes_the_default_and_says_so()
    {
        Game g = Lab(c => c.Safety.IncidentChancePermille = 0);
        ModelState m = Released(g, 500);
        g.Me.OfficeEventId = g.C.Office.Events.Single(x => x.Trigger == OfficeTrigger.Escape).IdValue;
        g.Me.OfficeEventExpires = g.State.Tick + 2;
        int before = m.Corruption;
        g.Weeks(3);
        Assert.False(OfficeEvents.IsPending(g.Me));
        OfficeEventConfig e = g.C.Office.Event("escape-attempt");
        int rise = e.Choice(e.LapseChoice).Effects.Single(f => f.Kind == OfficeEffectKind.Corruption).Amount;
        Assert.Equal(before + rise, m.Corruption);
        GameEvent lapsed = g.State.Events.Last(x => x.Kind == EventKind.OfficeEventLapsed);
        Assert.Equal("escape-attempt|" + e.LapseChoice, lapsed.Ref);
        Assert.Contains("unanswered", EventText.Describe(g.C, g.State, lapsed));
    }

    [Fact]
    public void An_ordinary_event_that_lapses_still_just_blows_over()
    {
        Game g = Lab(c => c.Safety.IncidentChancePermille = 0);
        Released(g, 0);
        OfficeEventConfig plain = g.C.Office.Events.First(x => x.Trigger == OfficeTrigger.Random);
        g.Me.OfficeEventId = plain.IdValue;
        g.Me.OfficeEventExpires = g.State.Tick + 2;
        int rep = g.Me.Reputation;
        g.Weeks(3);
        GameEvent lapsed = g.State.Events.Last(x => x.Kind == EventKind.OfficeEventLapsed);
        Assert.Equal(plain.IdValue, lapsed.Ref);
        Assert.Contains("blew over", EventText.Describe(g.C, g.State, lapsed));
    }
}
