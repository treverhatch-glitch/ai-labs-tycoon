using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchSeventeenNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.ReservedComputeSigned,
        EventKind.RivalIntentSurfaced,
        EventKind.GpuTierLocked,
        EventKind.ScenarioEndlessAck,
    };

    [Fact]
    public void Batch_seventeen_headlines_and_chat_validate()
    {
        GameConfig c = TestData.Config();
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        Assert.Empty(ConfigValidator.Problems(c));
    }
}