using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchTwentySixNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.HearingShareEraDampened,
        EventKind.OfficeReleaseEraGraced,
        EventKind.DataSourceAcquireEraWeeksBonused,
    };

    [Fact]
    public void Batch_twenty_six_headlines_and_chat_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(449, c.News.Headlines.Count);
        Assert.Equal(402, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        Assert.Empty(ConfigValidator.Problems(c));
    }
}