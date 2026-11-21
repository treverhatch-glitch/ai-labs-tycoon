using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DataSourceFinishRiskTests
{
    [Fact]
    public void Synthetic_adds_corruption_when_a_run_finishes()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.True(model.Corruption >= g.C.Training.DataSource("synthetic").FinishCorruption);
    }

    [Fact]
    public void Synthetic_raises_lab_heat_on_finish()
    {
        Game g = new Game(Simulation.NewGame(TestData.Config(c => c.Shady.HeatDecayPerWeek = 0), new NewGameOptions()));
        g.WithStarterLab(gpus: 16);
        int before = g.Me.Heat;
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Equal(before + g.C.Training.DataSource("synthetic").FinishHeat, g.Me.Heat);
    }

    [Fact]
    public void Licensed_raises_home_attention_when_finish()
    {
        Game g = new Game(Simulation.NewGame(TestData.Config(c => c.World.Attention.DecayPermillePerMonth = 0), new NewGameOptions()));
        g.WithStarterLab(gpus: 16);
        int before = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Equal(before + g.C.Training.DataSource("licensed").HomeAttentionPermille, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Internal_finish_has_no_extra_risk()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        int heat = g.Me.Heat;
        int attention = g.Me.Bloc(g.Me.HomeBlocId).Attention;
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Equal(0, model.Corruption);
        Assert.Equal(heat, g.Me.Heat);
        Assert.Equal(attention, g.Me.Bloc(g.Me.HomeBlocId).Attention);
    }

    [Fact]
    public void Synthetic_finish_increases_incident_chance_after_release()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        model.Alignment = 400;
        int syntheticChance = SafetySystem.IncidentChancePermille(g.C, model);
        model.Corruption = 0;
        int cleanChance = SafetySystem.IncidentChancePermille(g.C, model);
        Assert.True(syntheticChance > cleanChance);
    }

    [Fact]
    public void Licensed_emits_attention_rise_event()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AttentionRose && e.Ref == g.Me.HomeBlocId);
    }

    [Fact]
    public void Finish_risk_survives_save_load_on_model()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        int corruption = model.Corruption;
        int heat = g.Me.Heat;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState after = loaded.Me.Models.Find(m => m.Id == model.Id);
        Assert.Equal(corruption, after.Corruption);
        Assert.Equal(heat, loaded.Me.Heat);
    }

    [Fact]
    public void Zero_risk_config_leaves_heat_and_corruption()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 16);
        g.C.Training.DataSource("synthetic").FinishCorruption = 0;
        g.C.Training.DataSource("synthetic").FinishHeat = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Equal(0, model.Corruption);
        Assert.Equal(0, g.Me.Heat);
    }

    [Fact]
    public void Heat_and_corruption_cap_at_permille()
    {
        Game g = new Game(Simulation.NewGame(TestData.Config(c => c.Shady.HeatDecayPerWeek = 0), new NewGameOptions()));
        g.WithStarterLab(gpus: 16);
        g.C.Training.DataSource("synthetic").FinishCorruption = 900;
        g.C.Training.DataSource("synthetic").FinishHeat = 900;
        g.Me.Heat = MathI.Permille - 50;
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        ModelState model = g.Train("mini", 100);
        model.Corruption = MathI.Permille - 50;
        g.UntilTrained(model);
        Assert.Equal(MathI.Permille, model.Corruption);
        Assert.Equal(MathI.Permille, g.Me.Heat);
    }
}