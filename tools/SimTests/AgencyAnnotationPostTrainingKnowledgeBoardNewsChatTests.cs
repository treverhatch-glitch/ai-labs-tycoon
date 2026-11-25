using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for agent autonomy, annotation vendors, post-training, owners of models and board motions.</summary>
public class AgencyAnnotationPostTrainingKnowledgeBoardNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.AutonomySet, EventKind.AgentBlowup, EventKind.AgentStreak,
        EventKind.AnnotatorsContracted, EventKind.AnnotatorsEnded, EventKind.VendorAudited, EventKind.VendorScandal,
        EventKind.PostTrainingStarted, EventKind.PostTrainingDone, EventKind.PostTrainingAbandoned,
        EventKind.OwnerNamed, EventKind.OwnerLeft, EventKind.DocsWritten, EventKind.DocsStale,
        EventKind.MotionTabled, EventKind.MotionAnswered, EventKind.MotionIgnored, EventKind.MotionJudged, EventKind.BoardForcedVote,
    };

    static readonly EventKind[] NoChat =
    {
        EventKind.AnnotatorsContracted, EventKind.VendorAudited, EventKind.PostTrainingStarted, EventKind.PostTrainingAbandoned, EventKind.OwnerNamed, EventKind.DocsWritten, EventKind.MotionAnswered, EventKind.MotionIgnored,
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
        Assert.Equal(421, c.News.Headlines.Count);
        Assert.Equal(374, c.Chat.Lines.Count);
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
        Assert.StartsWith(lab + " GIVES THE AGENTS OF MINI MORE ROPE", Headline(g, EventKind.AutonomySet, -1));
        Assert.StartsWith("AN AGENT RUN AT " + lab + " GOES WRONG AND MINI REFUNDS", Headline(g, EventKind.AgentBlowup, -1));
        Assert.StartsWith("THE AGENTS OF MINI GO HALF A YEAR AT " + lab, Headline(g, EventKind.AgentStreak, -1));
        Assert.StartsWith(lab + " SIGNS AN ANNOTATION VENDOR", Headline(g, EventKind.AnnotatorsContracted, -1, ""));
        Assert.Contains("CONTRACT RUNS OUT", Headline(g, EventKind.AnnotatorsEnded, 1, ""));
        Assert.Contains("ENDS ITS ANNOTATION CONTRACT EARLY", Headline(g, EventKind.AnnotatorsEnded, 0, ""));
        Assert.StartsWith(lab + " AUDITS ITS ANNOTATION VENDOR", Headline(g, EventKind.VendorAudited, -1, ""));
        Assert.StartsWith("WORKERS AT " + lab + "'S ANNOTATION VENDOR GO PUBLIC", Headline(g, EventKind.VendorScandal, -1, ""));
        Assert.StartsWith(lab + " SENDS MINI INTO POST-TRAINING", Headline(g, EventKind.PostTrainingStarted, -1));
        Assert.StartsWith(lab + " FINISHES TUNING MINI ON WHAT USERS LIKED", Headline(g, EventKind.PostTrainingDone, 1));
        Assert.StartsWith(lab + " FINISHES A SAFETY TUNING OF MINI", Headline(g, EventKind.PostTrainingDone, 0));
        Assert.StartsWith(lab + " ABANDONS THE TUNING OF MINI", Headline(g, EventKind.PostTrainingAbandoned, -1));
        Assert.StartsWith(lab + " PUTS A NAME NEXT TO MINI ON THE WIKI", Headline(g, EventKind.OwnerNamed, -1));
        Assert.StartsWith("THE ONE PERSON WHO KNEW MINI LEAVES " + lab, Headline(g, EventKind.OwnerLeft, -1));
        Assert.StartsWith(lab + " WRITES DOWN HOW MINI WORKS", Headline(g, EventKind.DocsWritten, -1));
        Assert.StartsWith("THE DOCS FOR MINI AT " + lab + " ARE A YEAR OUT OF DATE", Headline(g, EventKind.DocsStale, -1));
        Assert.StartsWith("THE BOARD OF " + lab + " TABLES A MOTION", Headline(g, EventKind.MotionTabled, -1, ""));
        Assert.StartsWith(lab + " ACCEPTS A BOARD MOTION", Headline(g, EventKind.MotionAnswered, 1, ""));
        Assert.StartsWith(lab + " DECLINES A BOARD MOTION", Headline(g, EventKind.MotionAnswered, 0, ""));
        Assert.StartsWith("A BOARD MOTION AT " + lab + " GOES UNANSWERED", Headline(g, EventKind.MotionIgnored, -1, ""));
        Assert.StartsWith(lab + " KEEPS A BOARD COMMITMENT", Headline(g, EventKind.MotionJudged, 1, ""));
        Assert.StartsWith(lab + " BREAKS A BOARD COMMITMENT", Headline(g, EventKind.MotionJudged, 0, ""));
        Assert.StartsWith("THE BOARD OF " + lab + " FORCES A VOTE ON THE FOUNDERS", Headline(g, EventKind.BoardForcedVote, -1, ""));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("autonomy-set-chat", Say(g, EventKind.AutonomySet, -1));
        Assert.Equal("agent-blowup-chat", Say(g, EventKind.AgentBlowup, -1));
        Assert.Equal("agent-streak-chat", Say(g, EventKind.AgentStreak, -1));
        Assert.Equal("vendor-scandal-chat", Say(g, EventKind.VendorScandal, -1));
        Assert.Equal("annotators-ended-chat", Say(g, EventKind.AnnotatorsEnded, 1));
        Assert.Equal("post-training-done-chat", Say(g, EventKind.PostTrainingDone, 0));
        Assert.Equal("owner-left-chat", Say(g, EventKind.OwnerLeft, -1));
        Assert.Equal("docs-stale-chat", Say(g, EventKind.DocsStale, -1));
        Assert.Equal("motion-tabled-chat", Say(g, EventKind.MotionTabled, -1));
        Assert.Equal("motion-judged-chat", Say(g, EventKind.MotionJudged, 0));
        Assert.Equal("board-forced-vote-chat", Say(g, EventKind.BoardForcedVote, -1));
    }
}
