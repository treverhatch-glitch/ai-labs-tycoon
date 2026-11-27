using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for the compliance office, papers and the compute sublet.</summary>
public class OfficePapersSubletNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int subject, long amount, string reference)
    {
        g.State.Events.Clear();
        Events.Add(g.State, kind, g.Id, subject, amount, reference);
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind.ToString());
        return ticker[0].Text;
    }

    static Game Lab(params string[] roles)
    {
        Game g = Game.Solo();
        g.Me.Staff.Clear();
        g.State.Events.Clear();
        foreach (string role in roles) g.Me.Staff.Add(new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = "steady", Morale = 500 });
        return g;
    }

    static ChatMessage Say(Game g, EventKind kind, int pick)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, 3, "");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void Three_headlines_and_four_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(431, c.News.Headlines.Count);
        Assert.Equal(384, c.Chat.Lines.Count);
        foreach (string kind in new[] { "ComplianceLapsed", "PaperPublished", "SubletChanged" })
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind);
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void The_headlines_name_the_lab_and_the_level_or_the_money()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.Equal(lab + "'S COMPLIANCE OFFICE LOSES ITS POLICY STAFF AND DROPS FROM LEVEL 3. A SPOKESPERSON IS 'FULLY COMPLIANT WITH THE IDEA OF IT'",
            Headline(g, EventKind.ComplianceLapsed, 0, 3, ""));
        Assert.Equal(lab + " PUBLISHES A PAPER. REVIEWERS 'HAVE NOTES'", Headline(g, EventKind.PaperPublished, 200, 300, ""));
        Assert.Contains("IT NOW BRINGS IN $12K A WEEK", Headline(g, EventKind.SubletChanged, 300, 12_000, ""));
    }

    [Fact]
    public void A_paper_is_always_a_good_day_and_a_researcher_has_their_own_line()
    {
        Game g = Lab("researcher");
        Assert.Equal("paper-published-chat-research", Say(g, EventKind.PaperPublished, 1).LineId);
        Game other = Lab("sales");
        Assert.Equal("paper-published-chat", Say(other, EventKind.PaperPublished, 1).LineId);
    }

    [Fact]
    public void A_lapsed_office_and_a_changed_sublet_have_their_own_lines()
    {
        Assert.Equal("compliance-lapsed-chat", Say(Lab("sales"), EventKind.ComplianceLapsed, -1).LineId);
        Assert.Equal("sublet-changed-chat", Say(Lab("engineer"), EventKind.SubletChanged, -1).LineId);
    }

    [Fact]
    public void The_real_events_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 4, engineers: 2, gpus: 10);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        g.Do(Command.PublishPaper(g.Id));
        Assert.Equal(1, g.State.Events.Single(e => e.Kind == EventKind.PaperPublished).Pick);
        g.Do(Command.SetSublet(g.Id, 300));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.SubletChanged);
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("paper-published-chat"));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("PUBLISHES A PAPER"));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("COMPUTE IT LETS"));
    }
}
