using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q90: Overworked and Corrupted mascots cut appeal via MoodBoost.</summary>
public class ModelMoodAppealTests
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
    public void Shipped_mood_appeal_permilles_validate()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(880, c.Safety.OverworkedAppealPermille);
        Assert.Equal(700, c.Safety.CorruptedMoodAppealPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Healthy_mood_leaves_appeal_mood_boost_zero()
    {
        Game g = WithReleasedMini(out ModelState model);
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Overworked_lab_cuts_appeal_parts()
    {
        Game starved = WithReleasedMini(out ModelState model, 8);
        starved.Weeks(30);
        starved.Rent(0);
        starved.Weeks(1);
        var sector = starved.C.Sector("game-ai");
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(starved.C, starved.Me, model, starved.Tick));
        Assert.True(DisplayQueries.AppealParts(starved.C, starved.State, sector, starved.State.Sector("game-ai"), starved.Me, model).MoodBoost < 0);
    }

    [Fact]
    public void Overworked_lowers_total_appeal()
    {
        Game g = WithReleasedMini(out ModelState model, 8);
        g.Weeks(25);
        var sector = g.C.Sector("game-ai");
        long healthy = DisplayQueries.Appeal(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model);
        g.Rent(0);
        g.Weeks(1);
        long starved = DisplayQueries.Appeal(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model);
        Assert.True(starved < healthy);
    }

    [Fact]
    public void Corrupted_mood_cuts_appeal_more_than_overworked()
    {
        Game g = WithReleasedMini(out ModelState model);
        model.Corruption = g.C.Shady.CorruptedMood + 10;
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(ModelMood.Corrupted, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        Assert.True(parts.MoodBoost < 0);
    }

    [Fact]
    public void Appeal_breakdown_sums_with_mood_boost()
    {
        Game g = WithReleasedMini(out ModelState model, 1);
        g.Weeks(20);
        var sector = g.C.Sector("game-ai");
        var b = DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(b.Total, DisplayQueries.Appeal(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model));
    }

    [Fact]
    public void Zero_overworked_appeal_config_disables_mood_boost()
    {
        Game g = Game.Solo(1, c => c.Safety.OverworkedAppealPermille = 1000).WithStarterLab(gpus: 1);
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Release(model, "game-ai");
        g.Weeks(20);
        var sector = g.C.Sector("game-ai");
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Glitching_does_not_double_apply_mood_on_appeal()
    {
        Game g = WithReleasedMini(out ModelState model);
        model.LastIncidentTick = g.Tick;
        model.Incidents = 1;
        Assert.Equal(ModelMood.Glitching, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Save_load_preserves_overworked_appeal()
    {
        Game g = WithReleasedMini(out ModelState model, 1);
        g.Weeks(18);
        long appeal = DisplayQueries.Appeal(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, model);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        loaded.Weeks(1);
        long again = DisplayQueries.Appeal(loaded.C, loaded.State, loaded.C.Sector("game-ai"), loaded.State.Sector("game-ai"), loaded.Me, model);
        Assert.Equal(appeal, again);
    }

    [Fact]
    public void Released_only_mood_gate()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState training = g.Train("mini", 0);
        Assert.Equal(ModelMood.Healthy, LabQueries.Mood(g.C, g.Me, training, g.Tick));
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("game-ai"), g.State.Sector("game-ai"), g.Me, training);
        Assert.Equal(0, parts.MoodBoost);
    }

    [Fact]
    public void Market_core_unchanged_when_mood_healthy()
    {
        Game a = WithReleasedMini(out ModelState ma, 20);
        Game b = WithReleasedMini(out ModelState mb, 20);
        a.Weeks(2);
        b.Weeks(2);
        var sector = a.C.Sector("game-ai");
        var pa = DisplayQueries.AppealParts(a.C, a.State, sector, a.State.Sector("game-ai"), a.Me, ma);
        var pb = DisplayQueries.AppealParts(b.C, b.State, sector, b.State.Sector("game-ai"), b.Me, mb);
        Assert.Equal(pa.MoodBoost, pb.MoodBoost);
    }

    [Fact]
    public void Mood_appeal_permille_in_shipped_range()
    {
        GameConfig c = TestData.Config();
        Assert.InRange(c.Safety.OverworkedAppealPermille, 1, 1000);
        Assert.InRange(c.Safety.CorruptedMoodAppealPermille, 1, 1000);
        Assert.True(c.Safety.CorruptedMoodAppealPermille < c.Safety.OverworkedAppealPermille);
    }

    [Fact]
    public void Overworked_cuts_appeal_on_devices_sector_too()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 8);
        ModelState device = ExpansionTestSupport.AddReleasedModel(g, 120_000, "devices");
        g.Weeks(30);
        g.Rent(0);
        g.Weeks(1);
        Assert.Equal(ModelMood.Overworked, LabQueries.Mood(g.C, g.Me, device, g.Tick));
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector("devices"), g.State.Sector("devices"), g.Me, device);
        Assert.True(parts.MoodBoost < 0);
    }
}