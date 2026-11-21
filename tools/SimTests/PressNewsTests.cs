using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines for the press, poaching and Gauntlet events the ticker had no line for.</summary>
public class PressNewsTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string> { TestData.Config().Rivals[0].Id } };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int labId, int subject, long amount, string reference, int pick = 0)
    {
        g.State.Events.Clear();
        Events.Add(g.State, kind, labId, subject, amount, reference).Pick = pick;
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind.ToString());
        return ticker[0].Text;
    }

    [Fact]
    public void A_leak_that_runs_and_a_leak_that_backfires_read_differently()
    {
        Game g = World();
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        string me = g.Me.Name.ToUpperInvariant();
        string them = rival.Name.ToUpperInvariant();
        Assert.Equal("A STORY ABOUT " + them + "'S GLIMMER 3 REACHES THE PRESS. " + them + " SUSPECTS " + me + ". " + me + " SUSPECTS A COINCIDENCE", Headline(g, EventKind.StoryLeaked, g.Id, rival.Id, 30, "Glimmer 3", 1));
        Assert.Equal(me + " LEAKS A STORY ABOUT " + them + "'S GLIMMER 3 AND IT TURNS OUT TO BE ABOUT " + me + ". NOBODY IS SURPRISED", Headline(g, EventKind.StoryLeaked, g.Id, rival.Id, 30, "Glimmer 3", 0));
    }

    [Fact]
    public void A_poached_person_names_the_hiring_lab_and_the_one_that_lost_them()
    {
        Game g = World();
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        string text = Headline(g, EventKind.StaffPoached, rival.Id, g.Id, 1, "researcher");
        Assert.Equal(rival.Name.ToUpperInvariant() + " HIRES AWAY " + g.Me.Name.ToUpperInvariant() + "'S RESEARCHER. " + g.Me.Name.ToUpperInvariant() + " WISHES THEM WELL, THROUGH A LAWYER", text);
    }

    [Fact]
    public void A_poach_by_nobody_in_particular_still_reads()
    {
        Game g = World();
        Assert.StartsWith("A LAB HIRES AWAY", Headline(g, EventKind.StaffPoached, -1, g.Id, 1, "engineer"));
    }

    [Fact]
    public void An_exposed_gauntlet_and_a_launch_buzz_name_the_lab_and_the_model()
    {
        Game g = World();
        string me = g.Me.Name.ToUpperInvariant();
        Assert.Equal(me + "'S GAUNTLET SCORES ARE EXPOSED. " + me + " CALLS IT 'A DIFFERENT METHODOLOGY'", Headline(g, EventKind.GauntletExposed, g.Id, 3, 12, ""));
        g.Me.Models.Add(new ModelState { Id = 77, Name = "Glimmer 3", LineId = "mini", Status = ModelStatus.Released });
        Assert.Equal("GLIMMER 3 LAUNCHES TO A GUST OF BUZZ. " + me + " SAYS IT PLANNED THAT", Headline(g, EventKind.LaunchBuzz, g.Id, 77, 5, "Glimmer 3"));
    }

    [Fact]
    public void The_shipped_news_validates_with_its_new_lines()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        Assert.Equal(105, c.News.Headlines.Count);
    }
}
