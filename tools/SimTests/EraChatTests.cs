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

/// <summary>Company chat lines that belong to a stretch of years, so the same event reads differently in different eras.</summary>
public class EraChatTests
{
    static Game Lab()
    {
        Game g = Game.Solo();
        g.Me.Staff.Clear();
        g.State.Events.Clear();
        g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "researcher", TraitId = "steady", Morale = 500 });
        g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = "sales", TraitId = "steady", Morale = 500 });
        return g;
    }

    static string Say(Game g, EventKind kind, int year, string reference = "researcher")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, 3, reference);
        e.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single().LineId;
    }

    [Fact]
    public void Seven_era_lines_ship_and_the_older_ones_have_no_years()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(7, c.Chat.Lines.Count(l => l.FromYear > 0 || l.ToYear > 0));
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void A_hire_is_welcomed_differently_in_each_era()
    {
        Game g = Lab();
        Assert.Equal("hired-dialup", Say(g, EventKind.Hired, 1999));
        Assert.Equal("hired", Say(g, EventKind.Hired, 2012));
        Assert.Equal("hired-chat-era", Say(g, EventKind.Hired, 2024));
        Assert.Equal("hired-upward", Say(g, EventKind.Hired, 2040));
    }

    [Fact]
    public void A_release_is_greeted_by_sales_in_the_words_of_its_time()
    {
        Game g = Lab();
        Assert.Equal("released-early", Say(g, EventKind.ModelReleased, 2005, "x"));
        Assert.Equal("released", Say(g, EventKind.ModelReleased, 2018, "x"));
        Assert.Equal("released-agents", Say(g, EventKind.ModelReleased, 2025, "x"));
    }

    [Fact]
    public void A_finished_model_is_met_by_researchers_in_the_words_of_its_time()
    {
        Game g = Lab();
        Assert.Equal("trained-early", Say(g, EventKind.TrainingFinished, 2010, "x"));
        Assert.Equal("trained", Say(g, EventKind.TrainingFinished, 2020, "x"));
        Assert.Equal("trained-harness", Say(g, EventKind.TrainingFinished, 2027, "x"));
    }

    [Fact]
    public void A_trait_line_still_beats_an_era_line()
    {
        Game g = Lab();
        g.Me.Staff[0].TraitId = "sleeps-under-desk";
        Assert.Equal("hired-sleeper", Say(g, EventKind.Hired, 1999));
    }

    [Fact]
    public void The_year_is_the_year_of_the_event_not_of_today()
    {
        Game g = Lab();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2040);
        Assert.Equal("hired-dialup", Say(g, EventKind.Hired, 1999));
    }

    [Fact]
    public void Years_that_do_not_make_a_range_are_caught_by_the_validator()
    {
        GameConfig c = TestData.Config();
        ChatLineConfig line = c.Chat.Line("hired-dialup");
        line.FromYear = 2010;
        line.ToYear = 2000;
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("years that do not make a range"));
    }
}
