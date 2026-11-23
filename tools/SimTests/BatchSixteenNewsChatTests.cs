using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchSixteenNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.MarginCall, EventKind.MarginCallSatisfied, EventKind.MarginCallDefaulted,
        EventKind.HearingShareNudged, EventKind.ScenarioGoalMet, EventKind.ScenarioGoalMissed,
        EventKind.LawTargetSet,
    };

    [Fact]
    public void Batch_sixteen_headlines_and_chat_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(407, c.News.Headlines.Count);
        Assert.Equal(360, c.Chat.Lines.Count);
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        foreach (EventKind kind in new[] { EventKind.MarginCall, EventKind.MarginCallSatisfied, EventKind.HearingShareNudged, EventKind.ScenarioGoalMet, EventKind.ScenarioGoalMissed, EventKind.LawTargetSet })
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        Assert.Empty(ConfigValidator.Problems(c));
    }
}