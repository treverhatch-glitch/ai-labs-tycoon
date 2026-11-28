using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchTwentyThreeNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.HearingShareEraBoosted,
        EventKind.RungSixEraCooldownShortened,
        EventKind.DataSourceAcquireEraDiscounted,
    };

    [Fact]
    public void Batch_twenty_three_headlines_and_chat_validate()
    {
        GameConfig c = TestData.Config();
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        Assert.Empty(ConfigValidator.Problems(c));
    }
}