using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q101: glitchingTrainingSpeedPermille when LabHasGlitchingReleased.</summary>
public class GlitchingTrainingSpeedTests
{
    static Game WithReleasedMini(out ModelState model, int gpus = 24)
    {
        Game g = Game.Solo().WithStarterLab(gpus: gpus);
        model = g.Train("mini", 300);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(1);
        return g;
    }

    static void MakeGlitching(Game g, ModelState model)
    {
        model.LastIncidentTick = g.Tick;
        model.Incidents = 1;
    }

    [Fact]
    public void Shipped_glitching_training_speed_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(900, c.Safety.GlitchingTrainingSpeedPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void No_glitching_release_full_speed_permille()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        Assert.False(TrainingSystem.LabHasGlitchingReleased(g.C, g.Me, g.Tick));
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Game plain = Game.Solo().WithStarterLab(gpus: 24);
        Assert.Equal(speed, TrainingSystem.SpeedPermille(plain.C, plain.State, plain.Me));
    }

    [Fact]
    public void Glitching_release_multiplies_speed_permille()
    {
        Game g = WithReleasedMini(out ModelState model);
        int clean = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        MakeGlitching(g, model);
        Assert.True(TrainingSystem.LabHasGlitchingReleased(g.C, g.Me, g.Tick));
        int glitched = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        Assert.True(glitched < clean);
    }

    [Fact]
    public void Glitching_multiplier_matches_config()
    {
        Game g = WithReleasedMini(out ModelState model);
        int clean = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        MakeGlitching(g, model);
        int expected = (int)AiLabs.Rules.Core.MathI.MulPermille(clean, g.C.Safety.GlitchingTrainingSpeedPermille);
        Assert.Equal(expected, TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Active_training_run_slows_with_glitching_release()
    {
        Game g = WithReleasedMini(out ModelState released);
        MakeGlitching(g, released);
        ModelState training = g.Train("mini", 0);
        g.Weeks(4);
        long withGlitch = training.ProgressCompute;
        Game clean = WithReleasedMini(out ModelState _);
        ModelState t2 = clean.Train("mini", 0);
        clean.Weeks(4);
        Assert.True(withGlitch < t2.ProgressCompute);
    }

    [Fact]
    public void Zero_or_full_permille_disables_multiplier_hook()
    {
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        int slowed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.C.Safety.GlitchingTrainingSpeedPermille = 1000;
        Assert.True(slowed < TrainingSystem.SpeedPermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void Training_only_model_not_glitching_gate()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        ModelState training = g.Train("mini", 0);
        training.LastIncidentTick = g.Tick;
        training.Incidents = 1;
        Assert.False(TrainingSystem.LabHasGlitchingReleased(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void After_glitch_window_speed_recovers()
    {
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        int slow = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        g.Weeks(g.C.Safety.GlitchWeeks + 2);
        Assert.False(LabQueries.IsGlitching(g.C, model, g.Tick));
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.State, g.Me) > slow);
    }

    [Fact]
    public void Save_load_preserves_glitching_speed()
    {
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        int speed = TrainingSystem.SpeedPermille(g.C, g.State, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(speed, TrainingSystem.SpeedPermille(loaded.C, loaded.State, loaded.Me));
    }

    [Fact]
    public void Plan_lab_same_glitching_gate()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.Models.Add(new ModelState
        {
            Status = ModelStatus.Released, LineId = "mini", Incidents = 1,
            LastIncidentTick = g.Tick, Sectors = { "game-ai" }
        });
        Assert.True(TrainingSystem.LabHasGlitchingReleased(g.C, plan, g.Tick));
    }

    [Fact]
    public void Negative_glitching_speed_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Safety.GlitchingTrainingSpeedPermille = -1));
    }

    [Fact]
    public void Sim_week_training_respects_glitching_penalty()
    {
        Game g = WithReleasedMini(out ModelState released);
        MakeGlitching(g, released);
        ModelState m = g.Train("mini", 0);
        long before = m.ProgressCompute;
        g.Weeks(1);
        Assert.True(m.ProgressCompute > before);
        Assert.True(TrainingSystem.LabHasGlitchingReleased(g.C, g.Me, g.Tick));
    }

    [Fact]
    public void Second_released_glitching_keeps_penalty()
    {
        Game g = WithReleasedMini(out ModelState a);
        MakeGlitching(g, a);
        ModelState b = g.Train("mini", 200);
        g.UntilTrained(b);
        g.Release(b, "game-ai");
        MakeGlitching(g, b);
        Assert.True(TrainingSystem.LabHasGlitchingReleased(g.C, g.Me, g.Tick));
    }
}