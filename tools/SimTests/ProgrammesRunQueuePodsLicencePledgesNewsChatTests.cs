using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for research programmes, the run queue, project pods, licensing and support pledges.</summary>
public class ProgrammesRunQueuePodsLicencePledgesNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.ProgrammeStarted, EventKind.ProgrammeStageDone, EventKind.ProgrammeStageFailed, EventKind.ProgrammeEnded,
        EventKind.RunPriorityChanged, EventKind.PodFormed, EventKind.PodSwapped, EventKind.PodEnded,
        EventKind.LicenceApplied, EventKind.LicenceDecided, EventKind.LicenceStrike, EventKind.LicenceEnded,
        EventKind.SupportPledged, EventKind.SupportPledgeEnded,
    };

    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, long amount = 7000, string text = "mini")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, -1, amount, text);
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
        Assert.Equal(405, c.News.Headlines.Count);
        Assert.Equal(358, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
            if (kind != EventKind.PodSwapped && kind != EventKind.ProgrammeStarted && kind != EventKind.LicenceApplied && kind != EventKind.SupportPledged)
                Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab + " ANNOUNCES A MULTI-YEAR RESEARCH PROGRAMME ON MINI", Headline(g, EventKind.ProgrammeStarted, -1));
        Assert.StartsWith(lab + " TAKES THE RISKY ROUTE", Headline(g, EventKind.ProgrammeStageDone, 2));
        Assert.StartsWith(lab + " FINISHES A STAGE OF ITS RESEARCH PROGRAMME THE CAREFUL WAY", Headline(g, EventKind.ProgrammeStageDone, 1));
        Assert.StartsWith("A STAGE OF " + lab + "'S RESEARCH PROGRAMME FALLS THROUGH", Headline(g, EventKind.ProgrammeStageFailed, -1));
        Assert.StartsWith(lab + " COMPLETES ITS RESEARCH PROGRAMME", Headline(g, EventKind.ProgrammeEnded, 1));
        Assert.StartsWith(lab + " SHELVES ITS RESEARCH PROGRAMME", Headline(g, EventKind.ProgrammeEnded, 0));
        Assert.StartsWith(lab + " RESHUFFLES WHICH TRAINING RUN GETS THE CLUSTER", Headline(g, EventKind.RunPriorityChanged, -1));
        Assert.StartsWith(lab + " PUTS FIVE PEOPLE IN A ROOM", Headline(g, EventKind.PodFormed, -1));
        Assert.StartsWith(lab + " SWAPS SOMEONE BETWEEN PODS", Headline(g, EventKind.PodSwapped, -1));
        Assert.StartsWith(lab + " WRAPS UP A PROJECT POD ON PURPOSE", Headline(g, EventKind.PodEnded, 1));
        Assert.StartsWith("A PROJECT POD AT " + lab + " FALLS APART", Headline(g, EventKind.PodEnded, 0));
        Assert.StartsWith(lab + " FILES FOR A LICENCE", Headline(g, EventKind.LicenceApplied, 2));
        Assert.StartsWith(lab + " IS GRANTED ITS LICENCE", Headline(g, EventKind.LicenceDecided, 1));
        Assert.StartsWith(lab + " IS REFUSED ITS LICENCE", Headline(g, EventKind.LicenceDecided, 0));
        Assert.StartsWith(lab + " TAKES A STRIKE AGAINST ITS LICENCE", Headline(g, EventKind.LicenceStrike, -1));
        Assert.StartsWith(lab + " HAS ITS LICENCE SUSPENDED", Headline(g, EventKind.LicenceEnded, 2));
        Assert.StartsWith(lab + " HANDS ITS LICENCE BACK", Headline(g, EventKind.LicenceEnded, 1));
        Assert.StartsWith(lab + " LETS ITS LICENCE GO", Headline(g, EventKind.LicenceEnded, 0));
        Assert.StartsWith(lab + " PROMISES TO KEEP SUPPORTING MINI", Headline(g, EventKind.SupportPledged, -1));
        Assert.StartsWith(lab + " KEEPS ITS SUPPORT PROMISE ON MINI", Headline(g, EventKind.SupportPledgeEnded, 1));
        Assert.StartsWith(lab + " WALKS AWAY FROM ITS SUPPORT PROMISE ON MINI", Headline(g, EventKind.SupportPledgeEnded, 0));
    }

    [Fact]
    public void Chat_lines_follow_the_event()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal("programme-chat", Say(g, EventKind.ProgrammeStageDone, 1));
        Assert.Equal("programme-failed-chat", Say(g, EventKind.ProgrammeStageFailed, -1));
        Assert.Equal("programme-ended-chat", Say(g, EventKind.ProgrammeEnded, 0));
        Assert.Equal("priority-chat", Say(g, EventKind.RunPriorityChanged, -1));
        Assert.Equal("pod-formed-chat", Say(g, EventKind.PodFormed, -1));
        Assert.Equal("pod-ended-chat", Say(g, EventKind.PodEnded, 1));
        Assert.Equal("licence-decided-chat", Say(g, EventKind.LicenceDecided, 0));
        Assert.Equal("licence-strike-chat", Say(g, EventKind.LicenceStrike, -1));
        Assert.Equal("licence-ended-chat", Say(g, EventKind.LicenceEnded, 2));
        Assert.Equal("pledge-ended-chat", Say(g, EventKind.SupportPledgeEnded, 1));
    }
}
