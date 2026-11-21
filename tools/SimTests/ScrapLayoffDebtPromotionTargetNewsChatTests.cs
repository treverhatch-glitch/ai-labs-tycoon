using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Headlines and chat for scrapped runs, layoff rounds, technical debt, promotions and board targets.</summary>
public class ScrapLayoffDebtPromotionTargetNewsChatTests
{
    static Game World()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.State.Events.Clear();
        return g;
    }

    static string Headline(Game g, EventKind kind, int pick, long amount = 2_000_000, string text = "Mini")
    {
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, kind, g.Id, 2, amount, text);
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
        GameEvent e = Events.Add(g.State, kind, g.Id, 1, 3, "Mini");
        e.Pick = pick;
        return CompanyChat.Feed(g.C, g.State, g.Me, 5).Single();
    }

    [Fact]
    public void The_new_headlines_and_chat_lines_are_in_the_data_and_it_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(105, c.News.Headlines.Count);
        Assert.Equal(160, c.Chat.Lines.Count);
        foreach (string kind in new[] { "RunScrapped", "LayoffRound", "DebtPaid", "StaffPromoted", "TargetSet", "TargetSettled" })
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
        Assert.Contains(lab + " SCRAPS THE TRAINING RUN OF MINI WELL INTO IT", Headline(g, EventKind.RunScrapped, 1));
        Assert.Contains(lab + " CANCELS THE TRAINING RUN OF MINI EARLY", Headline(g, EventKind.RunScrapped, 0));
        Assert.Contains(lab + " LAYS OFF 2000000 PEOPLE IN A DEEP CUT", Headline(g, EventKind.LayoffRound, 1));
        Assert.StartsWith(lab + " LAYS OFF 6 PEOPLE.", Headline(g, EventKind.LayoffRound, 0, 6));
        Assert.StartsWith(lab + " CLEARS ITS TECHNICAL DEBT", Headline(g, EventKind.DebtPaid, 1));
        Assert.StartsWith(lab + " PAYS DOWN TECHNICAL DEBT", Headline(g, EventKind.DebtPaid, 0));
        Assert.StartsWith(lab + " PROMOTES A ENGINEER TO SENIOR", Headline(g, EventKind.StaffPromoted, -1, 2000, "engineer"));
        Assert.StartsWith(lab + " PROMISES THE BOARD $2M FOR THE QUARTER", Headline(g, EventKind.TargetSet, -1));
        Assert.StartsWith(lab + " HITS ITS REVENUE TARGET WITH $2M", Headline(g, EventKind.TargetSettled, 1));
        Assert.StartsWith(lab + " MISSES ITS REVENUE TARGET, MAKING $2M", Headline(g, EventKind.TargetSettled, 0));
    }

    [Fact]
    public void Chat_lines_follow_the_pick_and_the_role()
    {
        Assert.Equal("run-scrapped-chat-late", Say(Lab("sales"), EventKind.RunScrapped, 1).LineId);
        Assert.Equal("run-scrapped-chat-early", Say(Lab("sales"), EventKind.RunScrapped, 0).LineId);
        Assert.Equal("layoff-chat-deep", Say(Lab("sales"), EventKind.LayoffRound, 1).LineId);
        Assert.Equal("layoff-chat", Say(Lab("sales"), EventKind.LayoffRound, 0).LineId);
        Assert.Equal("debt-paid-chat-engineer", Say(Lab("engineer"), EventKind.DebtPaid, 0).LineId);
        Assert.Equal("debt-paid-chat", Say(Lab("sales"), EventKind.DebtPaid, 1).LineId);
        Assert.Equal("staff-promoted-chat", Say(Lab("sales"), EventKind.StaffPromoted, 0).LineId);
        Assert.Equal("target-set-chat-sales", Say(Lab("sales"), EventKind.TargetSet, 0).LineId);
        Assert.Equal("target-met-chat", Say(Lab("sales"), EventKind.TargetSettled, 1).LineId);
        Assert.Equal("target-missed-chat", Say(Lab("sales"), EventKind.TargetSettled, 0).LineId);
    }

    [Fact]
    public void A_target_being_set_has_a_chat_line_only_from_sales()
    {
        Game g = Lab("engineer");
        g.State.Events.Clear();
        Events.Add(g.State, EventKind.TargetSet, g.Id, 1, 5000, "").Pick = 0;
        Assert.Empty(CompanyChat.Feed(g.C, g.State, g.Me, 5));
    }

    [Fact]
    public void The_real_commands_reach_the_ticker_and_the_chat()
    {
        Game g = Game.Solo(2, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 4, engineers: 3, safety: 2, gpus: 8);
        ModelState run = g.Train("mini", 0);
        g.Weeks(3);
        g.Do(Command.ScrapRun(g.Id, run.Id));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("TRAINING RUN OF"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId.StartsWith("run-scrapped-chat"));
        g.Me.TechDebt = 100;
        g.Do(Command.PayDownDebt(g.Id, 100));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("CLEARS ITS TECHNICAL DEBT"));
        g.Do(Command.PromoteStaff(g.Id, g.Me.Staff.First(p => p.RoleId == "researcher").Id));
        Assert.Contains(NewsTicker.Headlines(g.C, g.State, 10), h => !h.IsFiller && h.Text.Contains("PROMOTES A RESEARCHER TO SENIOR"));
        Assert.Contains(CompanyChat.Feed(g.C, g.State, g.Me, 20), m => m.LineId == "staff-promoted-chat");
    }
}
