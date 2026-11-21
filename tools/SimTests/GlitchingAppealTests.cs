using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q94: glitchingAppealPermille in DisplayQueries.AppealParts.</summary>
public class GlitchingAppealTests
{
    static Game WithReleasedMini(out ModelState model, int gpus = 16)
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
    public void Shipped_glitching_appeal_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(750, c.Safety.GlitchingAppealPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Healthy_released_model_has_zero_mood_boost()
    {
        Game g = WithReleasedMini(out ModelState model);
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(0, parts.MoodBoost);
        Assert.Equal(ModelMood.Healthy, LabQueries.Mood(g.C, g.Me, model, g.Tick));
    }

    [Fact]
    public void Glitching_mood_applies_appeal_permille_to_core()
    {
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        Assert.Equal(ModelMood.Glitching, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.True(parts.MoodBoost < 0);
        long coreOnly = MathI.MulPermille(parts.Core - parts.MoodBoost, g.C.Safety.GlitchingAppealPermille);
        Assert.Equal(coreOnly, parts.Core);
    }

    [Fact]
    public void Glitching_lowers_total_appeal_parts()
    {
        Game g = WithReleasedMini(out ModelState model);
        var sector = g.C.Sector("game-ai");
        var sectorState = g.State.Sector("game-ai");
        long healthy = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).Total;
        MakeGlitching(g, model);
        long glitching = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).Total;
        Assert.True(glitching < healthy);
    }

    [Fact]
    public void Zero_glitching_appeal_config_skips_mood_cut()
    {
        Game g = Game.Solo(1, c => c.Safety.GlitchingAppealPermille = 1000).WithStarterLab(gpus: 16);
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        MakeGlitching(g, model);
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Glitching_beats_overworked_permille_for_mood()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Safety.GlitchingAppealPermille < c.Safety.OverworkedAppealPermille);
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        var glitch = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        g.Rent(0);
        model.LastIncidentTick = -1;
        model.Incidents = 0;
        g.Weeks(30);
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var over = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.True(glitch.MoodBoost < over.MoodBoost);
    }

    [Fact]
    public void Appeal_parts_total_matches_market_appeal()
    {
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        var sector = g.C.Sector("game-ai");
        var sectorState = g.State.Sector("game-ai");
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.Equal(parts.Total, MarketSystem.Appeal(g.C, g.State, sector, sectorState, g.Me, model));
    }

    [Fact]
    public void Higher_config_permille_softens_glitching_cut()
    {
        Game low = Game.Solo(1, c => c.Safety.GlitchingAppealPermille = 600).WithStarterLab();
        Game high = Game.Solo(2, c => c.Safety.GlitchingAppealPermille = 900).WithStarterLab();
        ModelState mLow = low.Train("mini", 250);
        ModelState mHigh = high.Train("mini", 250);
        low.UntilTrained(mLow);
        high.UntilTrained(mHigh);
        low.Release(mLow, "game-ai");
        high.Release(mHigh, "game-ai");
        MakeGlitching(low, mLow);
        MakeGlitching(high, mHigh);
        var pLow = DisplayQueries.AppealParts(low.C, low.State, low.C.Sector("game-ai"), low.State.Sector("game-ai"), low.Me, mLow);
        var pHigh = DisplayQueries.AppealParts(high.C, high.State, high.C.Sector("game-ai"), high.State.Sector("game-ai"), high.Me, mHigh);
        Assert.True(pHigh.MoodBoost > pLow.MoodBoost);
    }

    [Fact]
    public void Save_load_preserves_glitching_appeal_parts()
    {
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        long appeal = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState again = loaded.Me.Model(model.Id);
        long after = DisplayQueries.Appeal(loaded.C, loaded.State, loaded.C.Sector("game-ai"), loaded.State.Sector("game-ai"), loaded.Me, again);
        Assert.Equal(appeal, after);
    }

    [Fact]
    public void Training_runs_never_glitching_appeal()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState training = g.Train("mini", 0);
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, training);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Glitching_appeal_on_devices_sector()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 16);
        ModelState device = ExpansionTestSupport.AddReleasedModel(g, 100_000, "devices");
        MakeGlitching(g, device);
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("devices"), g.State.Sector("devices"), g.Me, device);
        Assert.True(parts.MoodBoost < 0);
    }

    [Fact]
    public void Invalid_glitching_appeal_permille_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Safety.GlitchingAppealPermille = -1));
    }

    [Fact]
    public void Lab_queries_glitching_gate_matches_appeal_mood()
    {
        Game g = WithReleasedMini(out ModelState model);
        MakeGlitching(g, model);
        Assert.True(LabQueries.IsGlitching(g.C, model, g.Tick));
        Assert.Equal(ModelMood.Glitching, LabQueries.Mood(g.C, g.Me, model, g.Tick));
    }
}