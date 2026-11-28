using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchThirtyTwoNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.LobbyingGoalEraAmplified,
        EventKind.PublicMetricsBarEraSlacked,
        EventKind.OpenWeightsCloseEraRelieved,
    };

    [Fact]
    public void Batch_thirty_two_headlines_and_chat_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(449, c.News.Headlines.Count);
        Assert.Equal(405, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        Assert.Empty(ConfigValidator.Problems(c));
    }
}