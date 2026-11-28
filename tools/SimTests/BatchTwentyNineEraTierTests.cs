using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BatchTwentyNineEraTierTests
{
    static Game Lab(System.Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(143, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        return g;
    }

    [Fact]
    public void Zero_enabled_leaves_paths_unchanged()
    {
        Game g = Lab(c =>
        {
            c.OfficeHearingEraGrace.Enabled = 0;
            c.OpenWeightsCopyEraRelief.Enabled = 0;
            c.EvalFailEraRelief.Enabled = 0;
        });
        Assert.Equal(g.C.Office.ExpireWeeks, OfficeHearingEraGrace.AnswerWeeks(g.C, g.State));
        Assert.Equal(50, OpenWeightsCopyEraRelief.CopyTrustLoss(g.C, g.State, 50));
        Assert.Equal(-40, EvalFailEraRelief.TrustLoss(g.C, g.State, -40));
    }
}