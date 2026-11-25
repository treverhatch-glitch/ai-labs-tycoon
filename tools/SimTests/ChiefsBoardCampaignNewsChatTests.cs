using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for chiefs, the ethics board, marketing campaigns, inference optimisation and the on-call rota.</summary>
public class ChiefsBoardCampaignNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick)
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, 2, 0, "");
        e.Pick = pick;
        List<Headline> ticker = NewsTicker.Headlines(g.C, g.State, 1);
        Assert.False(ticker[0].IsFiller, kind + " " + pick);
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
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, 3, "game-ai");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(334, c.News.Headlines.Count);
        Assert.Equal(316, c.Chat.Lines.Count);
        foreach (string kind in new[] { "ChiefChanged", "ChiefLeft", "EthicsBoardChanged", "EthicsBoardResigned", "CampaignRun", "OptimisationSet", "OnCallSet" })
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind);
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind);
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Every_pick_of_the_new_events_has_a_headline_that_names_the_lab()
    {
        Game g = World();
        string lab = g.Me.Name.ToUpperInvariant();
        Assert.StartsWith(lab + " NAMES A CHIEF", Headline(g, EventKind.ChiefChanged, 1));
        Assert.StartsWith(lab + " LETS A CHIEF STEP BACK", Headline(g, EventKind.ChiefChanged, 0));
        Assert.StartsWith(lab + "'S CHIEF WALKS OUT", Headline(g, EventKind.ChiefLeft, 0));
        Assert.StartsWith(lab + " SEATS AN ETHICS BOARD", Headline(g, EventKind.EthicsBoardChanged, 1));
        Assert.StartsWith(lab + " STEPS ITS ETHICS BOARD DOWN", Headline(g, EventKind.EthicsBoardChanged, 0));
        Assert.StartsWith(lab + "'S ETHICS BOARD RESIGNS", Headline(g, EventKind.EthicsBoardResigned, 0));
        Assert.StartsWith(lab + " LAUNCHES A MARKETING CAMPAIGN", Headline(g, EventKind.CampaignRun, 1));
        Assert.StartsWith(lab + "'S MARKETING CAMPAIGN WRAPS UP", Headline(g, EventKind.CampaignRun, 0));
        Assert.StartsWith(lab + " OPTIMISES ITS INFERENCE", Headline(g, EventKind.OptimisationSet, 1));
        Assert.StartsWith(lab + " PUTS ITS TEAM ON CALL", Headline(g, EventKind.OnCallSet, 1));
    }

    [Fact]
    public void Chat_lines_follow_the_pick_and_the_role()
    {
        Assert.Equal("chief-named-chat-research", Say(Lab("researcher"), EventKind.ChiefChanged, 1).LineId);
        Assert.Equal("chief-named-chat", Say(Lab("sales"), EventKind.ChiefChanged, 1).LineId);
        Assert.Equal("chief-left-chat", Say(Lab("sales"), EventKind.ChiefLeft, 0).LineId);
        Assert.Equal("ethics-board-chat-safety", Say(Lab("safety"), EventKind.EthicsBoardChanged, 1).LineId);
        Assert.Equal("ethics-board-chat", Say(Lab("sales"), EventKind.EthicsBoardChanged, 1).LineId);
        Assert.Equal("ethics-board-resigned-chat", Say(Lab("sales"), EventKind.EthicsBoardResigned, 0).LineId);
        Assert.Equal("campaign-chat-sales", Say(Lab("sales"), EventKind.CampaignRun, 1).LineId);
        Assert.Equal("campaign-chat", Say(Lab("engineer"), EventKind.CampaignRun, 1).LineId);
        Assert.Equal("campaign-over-chat", Say(Lab("sales"), EventKind.CampaignRun, 0).LineId);
        Assert.Equal("optimisation-chat-engineer", Say(Lab("engineer"), EventKind.OptimisationSet, 1).LineId);
        Assert.Equal("on-call-chat", Say(Lab("sales"), EventKind.OnCallSet, 1).LineId);
    }

    [Fact]
    public void A_collapsed_campaign_is_left_to_the_incident_s_own_headline_and_chat()
    {
        Game g = Lab("sales");
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, EventKind.CampaignRun, g.Id, 1, 0, "game-ai");
        e.Pick = 2;
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
        Assert.True(NewsTicker.Headlines(g.C, g.State, 1)[0].IsFiller);
    }

    [Fact]
    public void Stepping_down_an_optimisation_or_a_rota_has_a_chat_line_only_if_the_data_has_one()
    {
        Game g = Lab("engineer");
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, EventKind.OnCallSet, g.Id, 0, 0, "");
        e.Pick = 0;
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
    }

    [Fact]
    public void The_real_commands_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo().WithStarterLab(researchers: 3, engineers: 4, safety: 2, gpus: 8);
        g.Me.Cash = 1_000_000_000_000;
        g.Do(Command.SetOnCall(g.Id, 1));
        g.Do(Command.SetOptimisation(g.Id, 1));
        g.Do(Command.SetEthicsBoard(g.Id, 1));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("PUTS ITS TEAM ON CALL"));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("OPTIMISES ITS INFERENCE"));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("SEATS AN ETHICS BOARD"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("on-call-chat"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("ethics-board-chat"));
    }
}
