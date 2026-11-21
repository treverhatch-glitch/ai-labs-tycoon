using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class EraIncidentLoadLaterTests
{
    static ModelState Released(int alignment, int corruption = 0) =>
        new ModelState { Alignment = alignment, Corruption = corruption, Status = ModelStatus.Released };

    [Fact]
    public void Agents_era_load_raises_incident_chance_versus_dial_up_days()
    {
        var g = Game.Solo();
        int dialUp = GameDate.TickOfYear(g.C.Calendar, 1996);
        int agents = GameDate.TickOfYear(g.C.Calendar, 2025);
        int low = SafetySystem.IncidentChancePermille(g.C, dialUp, Released(600));
        int high = SafetySystem.IncidentChancePermille(g.C, agents, Released(600));
        Assert.True(high > low);
        Assert.Equal(1200, g.C.Era("agents").IncidentLoadPermille);
    }

    [Fact]
    public void Zero_incident_load_in_config_defaults_to_unchanged_permille()
    {
        var g = Game.Solo(tweak: c => c.Era("dial-up-days").IncidentLoadPermille = 0);
        int tick = GameDate.TickOfYear(g.C.Calendar, 1996);
        int odds = SafetySystem.IncidentChancePermille(g.C, tick, Released(500));
        int expected = (int)MathI.MulPermille(g.C.Safety.IncidentChancePermille, MathI.Permille - 500);
        Assert.Equal(expected, odds);
    }

    [Fact]
    public void Max_alignment_models_stay_at_zero_incident_chance_in_any_era()
    {
        var g = Game.Solo();
        int agents = GameDate.TickOfYear(g.C.Calendar, 2025);
        Assert.Equal(0, SafetySystem.IncidentChancePermille(g.C, agents, Released(MathI.Permille)));
    }

    [Fact]
    public void Heavy_corruption_still_stacks_on_top_of_era_load()
    {
        var g = Game.Solo();
        int tick = GameDate.TickOfYear(g.C.Calendar, 2025);
        int clean = SafetySystem.IncidentChancePermille(g.C, tick, Released(600, 0));
        int dirty = SafetySystem.IncidentChancePermille(g.C, tick, Released(600, 400));
        Assert.True(dirty > clean);
    }

    [Fact]
    public void Incident_parts_sum_matches_incident_chance_at_tick()
    {
        var g = Game.Solo();
        var model = Released(550, 50);
        var b = DisplayQueries.IncidentParts(g.C, GameDate.TickOfYear(g.C.Calendar, 2025), model);
        Assert.Equal(b.Total, SafetySystem.IncidentChancePermille(g.C, GameDate.TickOfYear(g.C.Calendar, 2025), model));
    }

    [Fact]
    public void Crossing_an_era_boundary_changes_breakdown_config_component()
    {
        var g = Game.Solo();
        var model = Released(500);
        int dial = DisplayQueries.IncidentParts(g.C, GameDate.TickOfYear(g.C.Calendar, 1999), model).FromConfig;
        int agents = DisplayQueries.IncidentParts(g.C, GameDate.TickOfYear(g.C.Calendar, 2025), model).FromConfig;
        Assert.True(agents > dial);
    }

    [Fact]
    public void High_era_load_raises_incident_odds_versus_baseline_for_the_same_model()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Era("dial-up-days").IncidentLoadPermille = 1000;
            c.Era("agents").IncidentLoadPermille = 1500;
        });
        var model = Released(200);
        int dial = SafetySystem.IncidentChancePermille(g.C, GameDate.TickOfYear(g.C.Calendar, 1996), model);
        int agents = SafetySystem.IncidentChancePermille(g.C, GameDate.TickOfYear(g.C.Calendar, 2025), model);
        Assert.True(agents > dial);
        Assert.True(agents <= MathI.Permille);
    }

    [Fact]
    public void Bankrupt_lab_stops_incidents_on_models_that_were_retired()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            c.Era("dial-up-days").IncidentLoadPermille = 1500;
        }).WithStarterLab();
        var model = g.Train("mini", 100);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        int before = model.Incidents;
        Finance.GoBankrupt(g.State, g.Me);
        g.Weeks(8);
        Assert.Equal(before, model.Incidents);
        Assert.Equal(ModelStatus.Retired, model.Status);
    }

    [Fact]
    public void Shipped_config_lists_incident_load_on_every_era()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(9, c.Eras.Count);
        foreach (EraConfig era in c.Eras)
        {
            Assert.True(era.IncidentLoadPermille >= 1, era.Id + " missing incident load");
            if (era.Id == "agents") Assert.Equal(1200, era.IncidentLoadPermille);
        }
    }

    [Fact]
    public void Save_round_trip_preserves_incident_chance_at_same_tick()
    {
        var g = Game.Solo();
        var model = Released(450, 80);
        int tick = GameDate.TickOfYear(g.C.Calendar, 2028);
        int before = SafetySystem.IncidentChancePermille(g.C, tick, model);
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(before, SafetySystem.IncidentChancePermille(loaded.Config, tick, model));
    }

    [Fact]
    public void Schema_17_save_load_round_trip_after_era_load_uses_same_odds()
    {
        var g = Game.Solo();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2026);
        var model = Released(500);
        string old = SaveSchemaPretend.At(g.C, 16, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        SaveSchemaPretend.AssertRoundTrip(sim);
        Assert.Equal(
            SafetySystem.IncidentChancePermille(g.C, g.Tick, model),
            SafetySystem.IncidentChancePermille(sim.Config, sim.Tick, model));
    }

    [Fact]
    public void Gauntlet_reputation_prize_and_era_load_both_apply_in_the_same_year_end_week()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.Era("dial-up-days").IncidentLoadPermille = 800;
            c.Era("agents").IncidentLoadPermille = 1200;
        });
        g.Me.Models.Add(new ModelState { Id = 1, Capability = 800, Status = ModelStatus.Released, Alignment = 600, Name = "Solo" });
        int repBefore = g.Me.Reputation;
        GauntletSystem.RunYear(g.C, g.State);
        Assert.True(g.Me.Reputation > repBefore);
        var model = Released(600);
        int dial = SafetySystem.IncidentChancePermille(g.C, GameDate.TickOfYear(g.C.Calendar, 1998), model);
        int agents = SafetySystem.IncidentChancePermille(g.C, GameDate.TickOfYear(g.C.Calendar, 2028), model);
        Assert.True(agents > dial);
    }
}