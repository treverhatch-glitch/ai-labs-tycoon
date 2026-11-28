using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for drills, public evals, tool policy, the human queue and release checklists.</summary>
public class DrillsEvalToolHumanReleaseNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.DrillStarted, EventKind.DrillFinished,
        EventKind.EvalRegistered, EventKind.EvalPublished,
        EventKind.ToolPolicySet,
        EventKind.HumanQueueDeep, EventKind.HumanQueueCleared,
        EventKind.ChecklistSigned,
    };

    static readonly EventKind[] NoChat =
    {
        EventKind.DrillStarted, EventKind.EvalRegistered, EventKind.ToolPolicySet, EventKind.ChecklistSigned,
    };

    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, string text = "mini")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, 7000, text);
        e.Pick = pick;
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind + " " + pick);
        return ticker[0].Text;
    }

    static string Say(Game g, EventKind kind, int pick)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, 7000, "mini");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single().LineId;
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(446, c.News.Headlines.Count);
        Assert.Equal(399, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
            if (!NoChat.Contains(kind)) Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab, Headline(g, EventKind.DrillStarted, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.DrillFinished, 1));
        Assert.StartsWith(lab, Headline(g, EventKind.DrillFinished, 0));
        Assert.StartsWith(lab + " PUTS", Headline(g, EventKind.EvalRegistered, -1));
        Assert.StartsWith(lab, Headline(g, EventKind.EvalPublished, 1));
        Assert.StartsWith(lab, Headline(g, EventKind.EvalPublished, 0));
        Assert.StartsWith(lab, Headline(g, EventKind.ToolPolicySet, -1));
        Assert.Contains(lab, Headline(g, EventKind.HumanQueueDeep, -1));
        Assert.Contains(lab, Headline(g, EventKind.HumanQueueCleared, 1));
        Assert.Contains(lab, Headline(g, EventKind.HumanQueueCleared, 0));
        Assert.StartsWith(lab, Headline(g, EventKind.ChecklistSigned, -1));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("drill-finished-chat", Say(g, EventKind.DrillFinished, 1));
        Assert.Equal("eval-published-chat", Say(g, EventKind.EvalPublished, 0));
        Assert.Equal("human-queue-deep-chat", Say(g, EventKind.HumanQueueDeep, -1));
        Assert.Equal("human-queue-cleared-chat", Say(g, EventKind.HumanQueueCleared, 0));
        Assert.Equal("tool-policy-chat", Say(g, EventKind.ToolPolicySet, -1));
    }
}