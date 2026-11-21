using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q77: Overworked and Corrupted mascots cut market quality (not just presentation).</summary>
public class ModelMoodQualityTests
{
    static Game WithReleasedMini(out ModelState model, int gpus = 8)
    {
        Game g = Game.Solo().WithStarterLab(gpus: gpus);
        model = g.Train("mini", 300);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(1);
        return g;
    }

    [Fact]
    public void Shipped_mood_quality_permilles_validate()
    {
        GameConfig c = TestData.Config();
        Assert.InRange(c.Safety.OverworkedQualityPermille, 1, 1000);
        Assert.InRange(c.Safety.CorruptedMoodQualityPermille, 1, 1000);
        Assert.True(c.Safety.OverworkedQualityPermille < 1000);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Healthy_mood_leaves_quality_unchanged()
    {
        Game g = WithReleasedMini(out ModelState model);
        var sector = g.C.Sector("game-ai");
        Assert.Equal(ModelMood.Healthy, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick, g.Me);
        Assert.Equal(0, parts.MoodAdjustment);
    }

    [Fact]
    public void Overworked_lab_cuts_quality_parts()
    {
        Game roomy = WithReleasedMini(out ModelState a);
        Game starved = WithReleasedMini(out ModelState b);
        roomy.Weeks(30);
        starved.Weeks(30);
        var sector = starved.C.Sector("game-ai");
        long healthyQ = DisplayQueries.Quality(starved.C, sector, b, starved.Tick, starved.Me);
        starved.Rent(0);
        roomy.Weeks(1);
        starved.Weeks(1);
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(starved.C, starved.Me, b, starved.Tick));
        long overQ = DisplayQueries.Quality(starved.C, sector, b, starved.Tick, starved.Me);
        Assert.True(overQ < healthyQ);
        Assert.True(DisplayQueries.QualityParts(starved.C, sector, b, starved.Tick, starved.Me).MoodAdjustment < 0);
    }

    [Fact]
    public void Omitting_lab_skips_mood_penalty_for_queries()
    {
        Game g = WithReleasedMini(out ModelState model);
        g.Weeks(30);
        g.Rent(0);
        g.Weeks(1);
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var sector = g.C.Sector("game-ai");
        long withLab = DisplayQueries.Quality(g.C, sector, model, g.Tick, g.Me);
        long bare = DisplayQueries.Quality(g.C, sector, model, g.Tick);
        Assert.True(withLab < bare);
    }

    [Fact]
    public void Corrupted_mood_cuts_quality()
    {
        Game g = Game.Solo().WithStarterLab();
        var model = new ModelState
        {
            Status = ModelStatus.Released, Capability = 20_000, CapBase = 20_000,
            Alignment = 500, Corruption = g.C.Shady.CorruptedMood,
        };
        g.Me.Models.Add(model);
        model.Sectors.Add("game-ai");
        var sector = g.C.Sector("game-ai");
        Assert.Equal(ModelMood.Corrupted, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick, g.Me);
        Assert.True(parts.MoodAdjustment < 0);
        long noMood = DisplayQueries.Quality(g.C, sector, model, g.Tick);
        Assert.True(parts.Total < noMood);
    }

    [Fact]
    public void Zero_overworked_permille_disables_penalty()
    {
        Game g = WithReleasedMini(out ModelState model, gpus: 8);
        GameConfig c = TestData.Config(tweak: cfg => cfg.Safety.OverworkedQualityPermille = 1000);
        var sector = c.Sector("game-ai");
        g.Rent(0);
        g.Weeks(1);
        var parts = DisplayQueries.QualityParts(c, sector, model, g.Tick, g.Me);
        Assert.Equal(0, parts.MoodAdjustment);
    }

    [Fact]
    public void Glitching_uses_glitch_path_not_overworked_mood()
    {
        Game g = WithReleasedMini(out ModelState model);
        model.Incidents = 1;
        model.LastIncidentTick = g.Tick;
        g.Rent(0);
        g.Me.LastWeek.ServingDemand = 100;
        g.Me.LastWeek.ServedPermille = 0;
        Assert.Equal(ModelMood.Glitching, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick, g.Me);
        Assert.True(parts.GlitchAdjustment < 0);
        Assert.Equal(0, parts.MoodAdjustment);
    }

    [Fact]
    public void Mood_breakdown_sums_to_total_quality()
    {
        Game g = WithReleasedMini(out ModelState model);
        g.Weeks(30);
        g.Rent(0);
        g.Weeks(1);
        var sector = g.C.Sector("game-ai");
        var b = DisplayQueries.QualityParts(g.C, sector, model, g.Tick, g.Me);
        Assert.Equal(b.Total, LabQueries.Quality(g.C, sector, model, g.Tick, g.Me));
        Assert.Equal(b.FromCapability + b.FromAlignment + b.SandbagBoost + b.GlitchAdjustment + b.MoodAdjustment + b.OpenWeightsBoost, b.Total);
    }

    [Fact]
    public void Overworked_lowers_market_appeal_core()
    {
        Game g = WithReleasedMini(out ModelState model);
        g.Weeks(30);
        var sector = g.C.Sector("game-ai");
        var sectorState = g.State.Sector("game-ai");
        long fed = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).Core;
        g.Rent(0);
        g.Weeks(1);
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        long starved = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model).Core;
        Assert.True(starved < fed);
    }

    [Fact]
    public void Save_load_preserves_overworked_quality_delta()
    {
        Game g = WithReleasedMini(out ModelState model);
        g.Weeks(30);
        g.Rent(0);
        g.Weeks(1);
        var sector = g.C.Sector("game-ai");
        long before = DisplayQueries.Quality(g.C, sector, model, g.Tick, g.Me);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState again = loaded.Me.Models.Find(m => m.Id == model.Id);
        long after = DisplayQueries.Quality(loaded.C, loaded.C.Sector("game-ai"), again, loaded.State.Tick, loaded.Me);
        Assert.Equal(before, after);
    }

    [Fact]
    public void Bankrupt_lab_still_computes_mood_quality_when_called()
    {
        Game g = WithReleasedMini(out ModelState model);
        g.Me.LastWeek.ServingDemand = 50;
        g.Me.LastWeek.ServedPermille = 0;
        g.Me.Bankrupt = true;
        var sector = g.C.Sector("game-ai");
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick, g.Me);
        Assert.True(parts.MoodAdjustment < 0);
    }

    [Fact]
    public void Gauntlet_bracket_score_reflects_overworked_quality()
    {
        Game g = WithReleasedMini(out ModelState model);
        g.Weeks(30);
        var sector = g.C.Sector("game-ai");
        long healthy = LabQueries.Quality(g.C, sector, model, g.Tick, g.Me);
        g.Rent(0);
        g.Weeks(1);
        long tired = LabQueries.Quality(g.C, sector, model, g.Tick, g.Me);
        Assert.True(tired < healthy);
    }

    [Fact]
    public void Max_corrupted_permille_one_is_floor_not_skip()
    {
        Game g = Game.Solo().WithStarterLab();
        var model = new ModelState
        {
            Status = ModelStatus.Released, Capability = 10_000, CapBase = 10_000,
            Alignment = 500, Corruption = g.C.Shady.CorruptedMood,
        };
        g.Me.Models.Add(model);
        model.Sectors.Add("game-ai");
        GameConfig c = TestData.Config(tweak: cfg => cfg.Safety.CorruptedMoodQualityPermille = 1);
        var sector = c.Sector("game-ai");
        ConfigValidator.Validate(c);
        Assert.True(DisplayQueries.Quality(c, sector, model, g.Tick, g.Me) >= 1);
    }
}