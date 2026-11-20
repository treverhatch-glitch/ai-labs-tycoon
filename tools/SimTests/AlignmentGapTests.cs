using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class AlignmentGapTests
{
    [Fact]
    public void Sandbag_release_inflates_displayed_quality_but_incidents_still_use_the_true_meter()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Equal(model.Alignment, model.DisplayedAlignment);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 950, "game-ai"));
        Assert.True(model.DisplayedAlignment > model.Alignment);
        Assert.True(SafetySystem.AlignmentGap(g.C, model) >= g.C.Safety.SandbagGapPermille);

        var sector = g.C.Sector("game-ai");
        DisplayQueries.QualityBreakdown b = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.True(b.SandbagBoost > 0);
        Assert.Equal(b.Total, LabQueries.Quality(g.C, sector, model, g.Tick));

        int odds = SafetySystem.IncidentChancePermille(g.C, model);
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, model);
        Assert.Equal(odds, parts.Total);
        Assert.True(odds > 0);
    }

    [Fact]
    public void Low_alignment_picks_rung_two_and_gap_picks_rung_three()
    {
        var modelLow = new ModelState { Alignment = 200, DisplayedAlignment = 200, Status = ModelStatus.Released };
        var modelGap = new ModelState { Alignment = 700, DisplayedAlignment = 900, Status = ModelStatus.Released };
        var g = Game.Solo();
        Assert.Equal(2, PickRungForTest(g.C, modelLow));
        Assert.Equal(3, PickRungForTest(g.C, modelGap));
    }

    [Fact]
    public void Schema_8_models_gain_displayed_alignment()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        foreach (LabState lab in sim.State.Labs)
            foreach (ModelState m in lab.Models)
                Assert.Equal(m.Alignment, m.DisplayedAlignment);
    }

    static int PickRungForTest(AiLabs.Rules.Config.GameConfig c, ModelState model)
    {
        if (SafetySystem.AlignmentGap(c, model) >= c.Safety.SandbagGapPermille) return 3;
        if (model.Alignment < c.Safety.Rung2BelowAlignment) return 2;
        return 1;
    }
}