using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchFifteenNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.CampaignDeadlineSet, EventKind.CampaignDeadlineHit,
        EventKind.LawShaped, EventKind.ScenarioBaggagePicked,
    };

    [Fact]
    public void Batch_fifteen_headlines_and_chat_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(418, c.News.Headlines.Count);
        Assert.Equal(371, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
        {
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        }
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Headlines_name_the_lab()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        Game g = new Game(Simulation.NewGame(TestData.Config(), options));
        string lab = g.Me.Name.ToUpperInvariant();
        g.State.Events.Clear();
        GameEvent e = Events.Add(g.State, EventKind.CampaignDeadlineSet, g.Id, 10, 0, "on");
        e.Pick = -1;
        string text = NewsTicker.Headlines(g.C, g.State, 1)[0].Text;
        Assert.StartsWith(lab, text.ToUpperInvariant());
    }
}