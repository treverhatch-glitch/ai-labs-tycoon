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

/// <summary>Office events that belong to a stretch of years: a modem, a demo day, a muffin, a sentence, a flight and a weekend.</summary>
public class EraOfficeEventsTests
{
    static readonly string[] Ids = { "modem-handshake", "demo-day", "muffin-label", "finishes-sentence", "phantom-flights", "robot-weekends" };

    static Game Lab()
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
        }).WithStarterLab();
        ModelState m = g.Train("mini", 100);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, "game-ai"));
        if (OfficeEvents.IsPending(g.Me)) g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        return g;
    }

    static List<string> In(Game g, int year)
    {
        int tick = GameDate.TickOfYear(g.C.Calendar, year);
        return OfficeEvents.Eligible(g.C, g.Me, out _, tick).Select(e => e.IdValue).ToList();
    }

    [Fact]
    public void The_six_events_ship_with_years_and_three_answers_and_the_data_is_clean()
    {
        GameConfig c = TestData.Config();
        foreach (string id in Ids)
        {
            OfficeEventConfig e = c.Office.Event(id);
            Assert.NotNull(e);
            Assert.Equal(3, e.Choices.Count);
            Assert.True(e.FromYear > 0 || e.ToYear > 0, id);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_older_event_has_no_years_and_can_happen_at_any_date()
    {
        GameConfig c = TestData.Config();
        foreach (OfficeEventConfig e in c.Office.Events.Where(e => !Ids.Contains(e.IdValue)))
        {
            Assert.Equal(0, e.FromYear);
            Assert.Equal(0, e.ToYear);
        }
    }

    [Fact]
    public void An_event_waits_for_its_first_year_and_stops_after_its_last()
    {
        Game g = Lab();
        Assert.DoesNotContain("modem-handshake", In(g, 2005));
        Assert.Contains("modem-handshake", In(g, 1999));
        Assert.Contains("modem-handshake", In(g, 2003));
        Assert.DoesNotContain("modem-handshake", In(g, 2004));
        Assert.DoesNotContain("phantom-flights", In(g, 2023));
        Assert.Contains("phantom-flights", In(g, 2024));
        Assert.Contains("phantom-flights", In(g, 2060));
    }

    [Fact]
    public void Each_era_sees_its_own_events_and_the_timeless_ones()
    {
        Game g = Lab();
        List<string> y1999 = In(g, 1999);
        Assert.Contains("modem-handshake", y1999);
        Assert.DoesNotContain("robot-weekends", y1999);
        Assert.DoesNotContain("phantom-flights", y1999);
        Assert.Contains("ceo-friday", y1999);

        List<string> y2031 = In(g, 2031);
        Assert.Contains("robot-weekends", y2031);
        Assert.Contains("phantom-flights", y2031);
        Assert.DoesNotContain("modem-handshake", y2031);
        Assert.DoesNotContain("demo-day", y2031);
    }

    [Fact]
    public void Without_a_date_the_years_are_ignored()
    {
        Game g = Lab();
        List<string> all = OfficeEvents.Eligible(g.C, g.Me, out _).Select(e => e.IdValue).ToList();
        foreach (string id in Ids) Assert.Contains(id, all);
    }

    [Fact]
    public void The_weekly_draw_respects_the_years()
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 1000;
            c.Office.FirstAfterWeeks = 0;
            c.Office.Events.RemoveAll(e => e.FromYear == 0 && e.ToYear == 0);
            c.TakeoffCrisis.Enabled = 0;
            c.RungSixCrisis.Enabled = 0;
            c.Safety.IncidentChancePermille = 0;
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000_000;
        int year = GameDate.Year(g.C.Calendar, g.Tick);
        var allowed = g.C.Office.Events.Where(e => e.Trigger == OfficeTrigger.Random && (e.FromYear <= 0 || year >= e.FromYear) && (e.ToYear <= 0 || year <= e.ToYear)).Select(e => e.IdValue).ToHashSet();
        for (int i = 0; i < 6; i++)
        {
            g.Weeks(1);
            if (OfficeEvents.IsPending(g.Me))
            {
                Assert.Contains(g.Me.OfficeEventId, allowed);
                g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
            }
        }
    }

    [Fact]
    public void Years_that_do_not_make_a_range_are_caught_by_the_validator()
    {
        GameConfig c = TestData.Config();
        c.Office.Event("demo-day").FromYear = 2020;
        c.Office.Event("demo-day").ToYear = 2010;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("years that do not make a range"));
        c.Office.Event("demo-day").FromYear = -1;
        c.Office.Event("demo-day").ToYear = 0;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("years that do not make a range"));
    }

    [Fact]
    public void Every_answer_is_accepted_and_logged()
    {
        foreach (string id in Ids)
            foreach (OfficeChoiceConfig choice in TestData.Config().Office.Event(id).Choices)
            {
                Game g = Lab();
                g.Me.OfficeEventId = id;
                g.Me.OfficeEventExpires = g.State.Tick + 8;
                g.Do(Command.AnswerOfficeEvent(g.Id, choice.IdValue));
                Assert.False(OfficeEvents.IsPending(g.Me), id + "/" + choice.IdValue);
                Assert.Equal(id + "|" + choice.IdValue, g.State.Events.Last(e => e.Kind == EventKind.OfficeEventAnswered).Ref);
            }
    }

    [Fact]
    public void A_phantom_city_adds_corruption_to_the_model_on_sale()
    {
        Game g = Lab();
        ModelState m = g.Me.Models.First(x => x.Status == ModelStatus.Released);
        m.Corruption = 10;
        g.Me.OfficeEventId = "phantom-flights";
        g.Me.OfficeEventExpires = g.State.Tick + 8;
        g.Do(Command.AnswerOfficeEvent(g.Id, "feature"));
        Assert.Equal(50, m.Corruption);
    }
}
