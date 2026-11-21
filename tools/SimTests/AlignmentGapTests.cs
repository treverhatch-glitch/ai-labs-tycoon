using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
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

        SectorConfig sector = g.C.Sector("game-ai");
        DisplayQueries.QualityBreakdown b = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.True(b.SandbagBoost > 0);
        Assert.True(LabQueries.Quality(g.C, sector, model, g.Tick) > LabQueries.Quality(g.C, sector,
            new ModelState { Capability = model.Capability, Alignment = model.Alignment, Corruption = 400, DisplayedAlignment = model.Alignment, Status = ModelStatus.Released }, g.Tick));
        int odds = SafetySystem.IncidentChancePermille(g.C, model);
        Assert.True(odds > 0);
    }

    [Fact]
    public void Rung_one_when_honest_and_rung_two_when_alignment_is_low()
    {
        var honest = new ModelState { Alignment = 900, DisplayedAlignment = 900, Status = ModelStatus.Released };
        var low = new ModelState { Alignment = 100, DisplayedAlignment = 100, Status = ModelStatus.Released };
        var g = Game.Solo();
        Assert.Equal(1, SafetySystem.PickIncidentRung(g.C, honest));
        Assert.Equal(2, SafetySystem.PickIncidentRung(g.C, low));
    }

    [Fact]
    public void A_large_gap_picks_rung_three_even_when_true_alignment_is_high()
    {
        var modelGap = new ModelState { Alignment = 700, DisplayedAlignment = 900, Status = ModelStatus.Released };
        var g = Game.Solo();
        Assert.True(SafetySystem.AlignmentGap(g.C, modelGap) >= g.C.Safety.SandbagGapPermille);
        Assert.Equal(3, SafetySystem.PickIncidentRung(g.C, modelGap));
    }

    [Fact]
    public void Honest_release_matches_the_training_meter_when_claim_is_zero()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.DisplayedAlignment = 100;
        g.Do(Command.ReleaseModel(g.Id, model.Id, 0, "game-ai"));
        Assert.Equal(model.Alignment, model.DisplayedAlignment);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, model));
    }

    [Fact]
    public void Claimed_alignment_is_capped_at_permille()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 50);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 1500, "game-ai"));
        Assert.Equal(MathI.Permille, model.DisplayedAlignment);
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
        Assert.True(displayedBefore > model.Alignment);
        Assert.Equal(SafetySystem.EffectiveAlignment(g.C, model), model.DisplayedAlignment);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, model));
    }

    [Fact]
    public void A_rung_two_incident_applies_that_rungs_reputation_loss()
    {
        var g = Game.Solo(tweak: c => c.Safety.IncidentChancePermille = 1000).WithStarterLab();
        var model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        int repBefore = g.Me.Reputation;
        IncidentRungConfig rung2 = g.C.Safety.IncidentRung(2);
        g.Weeks(1);
        Assert.Equal(2, model.LastIncidentRung);
        Assert.Equal(repBefore - rung2.ReputationLoss, g.Me.Reputation);
    }

    [Fact]
    public void Honest_corruption_does_not_trigger_sandbag_rung_three()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 900, DisplayedAlignment = 900, Corruption = 400, Status = ModelStatus.Released };
        Assert.True(SafetySystem.IncidentChancePermille(g.C, model) > 0);
        Assert.Equal(0, SafetySystem.AlignmentGap(g.C, model));
        Assert.Equal(1, SafetySystem.PickIncidentRung(g.C, model));
    }

    [Fact]
    public void Corruption_shrinks_effective_alignment_used_for_gap_and_odds()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 800, DisplayedAlignment = 800, Corruption = 400, Status = ModelStatus.Released };
        int effective = SafetySystem.EffectiveAlignment(g.C, model);
        Assert.True(effective < model.Alignment);
        Assert.True(SafetySystem.IncidentChancePermille(g.C, model) > SafetySystem.IncidentChancePermille(g.C,
            new ModelState { Alignment = 800, DisplayedAlignment = 800, Corruption = 0, Status = ModelStatus.Released }));
    }

    [Fact]
    public void A_bankrupt_lab_stops_having_safety_incidents()
    {
        var g = Game.Solo(tweak: c => c.Safety.IncidentChancePermille = 1000).WithStarterLab();
        var model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        int incBefore = model.Incidents;
        Finance.GoBankrupt(g.C, g.State, g.Me);
        g.Weeks(8);
        Assert.Equal(incBefore, model.Incidents);
        Assert.Equal(ModelStatus.Retired, model.Status);
    }

    [Fact]
    public void Sandbag_state_survives_a_save_round_trip()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 80);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 950, "game-ai"));
        int gap = SafetySystem.AlignmentGap(g.C, model);
        Assert.True(gap > 0);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        ModelState loaded = Simulation.Load(g.C, g.Sim.Save()).Player.Model(model.Id);
        Assert.Equal(gap, SafetySystem.AlignmentGap(g.C, loaded));
        Assert.Equal(model.DisplayedAlignment, loaded.DisplayedAlignment);
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

    [Fact]
    public void Loading_from_schema_8_restores_displayed_fields_without_changing_alignment()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 60);
        g.UntilTrained(model);
        Assert.True(model.Alignment < 500);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 900, "game-ai"));
        int align = model.Alignment;
        string at8 = SaveSchemaPretend.At(g.C, 8, g.Sim.Save());
        Simulation loaded = Simulation.Load(g.C, at8);
        ModelState again = loaded.Player.Model(model.Id);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Equal(align, again.Alignment);
        Assert.Equal(align, again.DisplayedAlignment);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }

    [Fact]
    public void Repeated_scandals_can_drive_bankrupt_and_record_the_ending_without_freezing_the_world()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            c.Safety.IncidentFineMin = 50_000;
            c.Finance.BankruptcyGraceWeeks = 1;
        }).WithStarterLab(gpus: 12);
        g.Me.Cash = 80_000;
        g.Me.GrantPerWeek = 0;
        var model = g.Train("mini", 50);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, 980, "game-ai"));
        g.Until(() => g.Me.Bankrupt, 20, "bankruptcy from incident fines");
        Assert.Contains(g.State.Endings, e => e.EndingId == "bankrupt" && e.Choice == "");
        int tick = g.Tick;
        g.Weeks(10);
        Assert.True(g.Tick > tick);
    }
}