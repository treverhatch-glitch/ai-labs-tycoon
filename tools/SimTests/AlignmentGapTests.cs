using System.Linq;
using AiLabs.Rules.Config;
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
        Assert.Equal(b.FromCapability + b.FromAlignment + b.SandbagBoost + b.GlitchAdjustment, b.Total);

        int odds = SafetySystem.IncidentChancePermille(g.C, model);
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, model);
        Assert.Equal(odds, parts.Total);
        Assert.Equal(parts.FromConfig + parts.FromAlignment + parts.FromCorruption, parts.Total);
        Assert.True(odds > 0);
    }

    [Fact]
    public void Low_alignment_picks_rung_two_and_gap_picks_rung_three()
    {
        var modelLow = new ModelState { Alignment = 200, DisplayedAlignment = 200, Status = ModelStatus.Released };
        var modelGap = new ModelState { Alignment = 700, DisplayedAlignment = 900, Status = ModelStatus.Released };
        var g = Game.Solo();
        Assert.Equal(2, SafetySystem.PickIncidentRung(g.C, modelLow));
        Assert.Equal(3, SafetySystem.PickIncidentRung(g.C, modelGap));
    }

    [Fact]
    public void A_sandbag_scandal_closes_the_gap_on_the_displayed_meter()
    {
        var g = Game.Solo(tweak: c => c.Safety.IncidentChancePermille = 1000).WithStarterLab(gpus: 10);
        var model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 950, "game-ai"));
        int displayedBefore = model.DisplayedAlignment;
        Assert.True(SafetySystem.AlignmentGap(g.C, model) >= g.C.Safety.SandbagGapPermille);
        g.Weeks(1);
        Assert.True(model.Incidents > 0);
        Assert.Equal(3, model.LastIncidentRung);
        Assert.True(displayedBefore > SafetySystem.EffectiveAlignment(g.C, model));
        Assert.Equal(SafetySystem.EffectiveAlignment(g.C, model), model.DisplayedAlignment);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, model));
    }

    [Fact]
    public void Schema_9_migration_copies_true_alignment_to_displayed()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        foreach (LabState lab in sim.State.Labs)
            foreach (ModelState m in lab.Models)
            {
                Assert.Equal(m.Alignment, m.DisplayedAlignment);
                Assert.Equal(0, m.LastIncidentRung);
            }
    }
}