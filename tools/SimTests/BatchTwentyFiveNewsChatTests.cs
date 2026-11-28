using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchTwentyFiveNewsChatTests
{
    static readonly EventKind[] Kinds =
    {
        EventKind.ComputeEquipmentUpgraded,
        EventKind.ExitDebtEraHaircutted,
        EventKind.TakeoffAnswerEraCooldownShortened,
        EventKind.ScenarioGoalEraTrusted,
    };

    [Fact]
    public void Batch_twenty_five_headlines_and_chat_validate()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.News.Headlines.Count >= 449);
        Assert.True(c.Chat.Lines.Count >= 405);
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.News.Headlines, h => h.Kind == kind.ToString());
        foreach (EventKind kind in Kinds)
            Assert.Contains(c.Chat.Lines, l => l.Kind == kind.ToString());
        Assert.Empty(ConfigValidator.Problems(c));
    }
}