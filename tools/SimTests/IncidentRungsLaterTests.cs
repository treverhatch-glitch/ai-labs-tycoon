using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class IncidentRungsLaterTests
{
    [Fact]
    public void Corruption_picks_rung_four_before_sandbagging()
    {
        var g = Game.Solo();
        var model = new ModelState
        {
            Alignment = 900, DisplayedAlignment = 900, Corruption = 300, Status = ModelStatus.Released
        };
        Assert.Equal(4, SafetySystem.PickIncidentRung(g.C, model));
    }

    [Fact]
    public void Corruption_picks_rung_five_in_the_mid_band()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 800, DisplayedAlignment = 800, Corruption = 500, Status = ModelStatus.Released };
        Assert.Equal(5, SafetySystem.PickIncidentRung(g.C, model));
    }

    [Fact]
    public void Corruption_picks_rung_six_at_the_top()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 700, DisplayedAlignment = 700, Corruption = 800, Status = ModelStatus.Released };
        Assert.Equal(6, SafetySystem.PickIncidentRung(g.C, model));
    }

    [Fact]
    public void Sandbagging_still_beats_rung_four_when_the_gap_is_wide()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 400, DisplayedAlignment = 950, Corruption = 100, Status = ModelStatus.Released };
        Assert.Equal(3, SafetySystem.PickIncidentRung(g.C, model));
    }

    [Fact]
    public void Rung_six_incidents_use_the_configured_fine_multiplier()
    {
        var g = Game.Solo(tweak: c => c.Safety.IncidentChancePermille = 1000).WithStarterLab();
        var model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        model.Corruption = g.C.Safety.Rung6CorruptionMin;
        int repBefore = g.Me.Reputation;
        IncidentRungConfig six = g.C.Safety.IncidentRung(6);
        for (int i = 0; i < 12 && model.LastIncidentRung != 6; i++) g.Weeks(1);
        Assert.Equal(6, model.LastIncidentRung);
        Assert.Equal(repBefore - six.ReputationLoss, g.Me.Reputation);
    }

    [Fact]
    public void Rung_six_does_not_close_a_sandbag_gap()
    {
        IncidentRungConfig six = TestData.Config().Safety.IncidentRung(6);
        Assert.False(six.ClosesGap);
    }

    [Fact]
    public void Config_lists_rungs_one_through_six()
    {
        GameConfig c = TestData.Config();
        for (int r = 1; r <= 6; r++) Assert.NotNull(c.Safety.IncidentRung(r));
    }

    [Fact]
    public void Validator_rejects_corruption_thresholds_out_of_order()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Safety.Rung4CorruptionMin = 600;
        c.Safety.Rung5CorruptionMin = 400;
        var problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("rung4/5/6"));
    }

    [Fact]
    public void Low_corruption_stays_on_rung_one_for_honest_models()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 800, DisplayedAlignment = 800, Corruption = 50, Status = ModelStatus.Released };
        Assert.Equal(1, SafetySystem.PickIncidentRung(g.C, model));
    }

    [Fact]
    public void Rung_two_still_wins_over_rung_four_at_low_corruption()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 100, DisplayedAlignment = 100, Corruption = 50, Status = ModelStatus.Released };
        Assert.Equal(2, SafetySystem.PickIncidentRung(g.C, model));
    }

    [Fact]
    public void Last_incident_rung_survives_save()
    {
        var g = Game.Solo().WithStarterLab();
        var model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        model.LastIncidentRung = 6;
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        ModelState again = Simulation.Load(g.C, g.Sim.Save()).Player.Model(model.Id);
        Assert.Equal(6, again.LastIncidentRung);
    }

    [Fact]
    public void High_corruption_raises_incident_odds_without_forcing_rung_three()
    {
        var g = Game.Solo();
        var honest = new ModelState { Alignment = 900, DisplayedAlignment = 900, Corruption = 0, Status = ModelStatus.Released };
        var dirty = new ModelState { Alignment = 900, DisplayedAlignment = 900, Corruption = 200, Status = ModelStatus.Released };
        Assert.True(SafetySystem.IncidentChancePermille(g.C, dirty) > SafetySystem.IncidentChancePermille(g.C, honest));
        Assert.Equal(1, SafetySystem.PickIncidentRung(g.C, dirty));
    }
}