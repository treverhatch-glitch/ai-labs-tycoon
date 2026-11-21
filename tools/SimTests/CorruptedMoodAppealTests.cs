using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q111: corruptedMoodAppealPermille in AppealParts (dedicated coverage).</summary>
public class CorruptedMoodAppealTests
{
    static Game WithReleased(out ModelState model)
    {
        Game g = Game.Solo().WithStarterLab(gpus: 12);
        model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        model.Corruption = g.C.Shady.CorruptedMood + 50;
        return g;
    }

    [Fact]
    public void Shipped_corrupted_appeal_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(700, c.Safety.CorruptedMoodAppealPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Corrupted_mood_negative_appeal_boost()
    {
        Game g = WithReleased(out ModelState model);
        Assert.Equal(ModelMood.Corrupted, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.True(parts.MoodBoost < 0);
    }

    [Fact]
    public void Corrupted_cuts_deeper_than_healthy_mood()
    {
        Game corrupt = WithReleased(out ModelState mc);
        Game healthy = Game.Solo().WithStarterLab();
        ModelState mh = healthy.Train("mini", 0);
        healthy.UntilTrained(mh);
        healthy.Release(mh, "game-ai");
        var sector = corrupt.C.Sector("game-ai");
        var corruptParts = DisplayQueries.AppealParts(corrupt.C, corrupt.State, sector, corrupt.State.Sector("game-ai"), corrupt.Me, mc);
        var healthyParts = DisplayQueries.AppealParts(healthy.C, healthy.State, sector, healthy.State.Sector("game-ai"), healthy.Me, mh);
        Assert.Equal(0, healthyParts.MoodBoost);
        Assert.True(corruptParts.MoodBoost < healthyParts.MoodBoost);
    }

    [Fact]
    public void Zero_corrupted_appeal_disables_mood_boost()
    {
        Game g = WithReleased(out ModelState model);
        g.C.Safety.CorruptedMoodAppealPermille = 1000;
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Healthy_release_no_corrupted_boost()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Appeal_breakdown_sums_with_corrupted_mood()
    {
        Game g = WithReleased(out ModelState model);
        var b = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(b.Total, DisplayQueries.Appeal(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model));
    }

    [Fact]
    public void Save_load_preserves_corrupted_appeal()
    {
        Game g = WithReleased(out ModelState model);
        long appeal = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        long again = DisplayQueries.Appeal(loaded.C, loaded.State, loaded.C.Sector("game-ai"), loaded.State.Sector("game-ai"), loaded.Me, model);
        Assert.Equal(appeal, again);
    }

    [Fact]
    public void Corrupted_quality_and_appeal_both_drop()
    {
        Game g = WithReleased(out ModelState model);
        var sector = g.C.Sector("game-ai");
        var quality = DisplayQueries.QualityParts(g.C, sector, model, g.Tick, g.Me);
        Assert.True(quality.MoodAdjustment < 0);
        Assert.True(DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model).MoodBoost < 0);
    }

    [Fact]
    public void Devices_sector_corrupted_appeal()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 16);
        ModelState device = ExpansionTestSupport.AddReleasedModel(g, 100_000, "devices");
        device.Corruption = g.C.Shady.CorruptedMood + 20;
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("devices"), g.State.Sector("devices"), g.Me, device);
        Assert.True(parts.MoodBoost < 0);
    }

    [Fact]
    public void Corrupted_permille_between_overworked_and_zero()
    {
        GameConfig c = TestData.Config();
        Assert.InRange(c.Safety.CorruptedMoodAppealPermille, 1, 1000);
        Assert.True(c.Safety.CorruptedMoodAppealPermille < c.Safety.OverworkedAppealPermille);
    }

    [Fact]
    public void Training_model_ignores_corrupted_appeal()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState training = g.Train("mini", 0);
        training.Corruption = 900;
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, training);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Workshop_boost_stacks_after_corrupted_mood()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 24);
        ModelState device = ExpansionTestSupport.AddReleasedModel(g, 120_000, "devices");
        device.Corruption = g.C.Shady.CorruptedMood + 5;
        ExpansionTestSupport.BuildExpansionRoom(g, "device-workshop");
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("devices"), g.State.Sector("devices"), g.Me, device);
        Assert.True(parts.MoodBoost < 0);
        Assert.True(parts.WorkshopBoost > 0);
    }

    [Fact]
    public void Negative_corrupted_appeal_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Safety.CorruptedMoodAppealPermille = -1));
    }
}