using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q57: synthetic-data-lab room softens synthetic-mix training risk.</summary>
public class SyntheticDataLabTests
{
    const string Room = "synthetic-data-lab";
    const string Synthetic = "synthetic";

    static Game Harness(int year = 2028)
    {
        Game g = ExpansionTestSupport.SoloAtYear(year);
        g.WithStarterLab(gpus: 24);
        return g;
    }

    [Fact]
    public void Shipped_synthetic_data_lab_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(Room, c.Training.SyntheticDataLab.RoomTypeId);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Inactive_before_harnesses_era()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024);
        g.Me.Rooms.Add(new RoomState { TypeId = Room, X = 0, Y = 0, Width = 4, Height = 4, Id = g.State.TakeId() });
        Assert.False(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Room_without_synthetic_source_inactive()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.False(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, "internal"));
    }

    [Fact]
    public void Synthetic_source_without_room_inactive()
    {
        Game g = Harness();
        Assert.False(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Active_with_room_era_and_synthetic_mix()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.True(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Bankruptcy_turns_hook_off()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Me.Bankrupt = true;
        Assert.False(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
    }

    [Fact]
    public void Finish_corruption_cut_matches_config()
    {
        Game g = Harness();
        DataSourceConfig source = g.C.Training.DataSource(Synthetic);
        int raw = source.FinishCorruption;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        int cut = SyntheticDataLabSystem.EffectiveFinishCorruption(g.C, g.State, g.Me, source);
        Assert.Equal(MathI.MulPermille(raw, MathI.Permille - g.C.Training.SyntheticDataLab.CorruptionCutPermille), cut);
        Assert.True(cut < raw);
    }

    [Fact]
    public void Zero_corruption_cut_keeps_finish_damage()
    {
        Game g = Harness();
        g.C.Training.SyntheticDataLab.CorruptionCutPermille = 0;
        DataSourceConfig source = g.C.Training.DataSource(Synthetic);
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.Equal(source.FinishCorruption, SyntheticDataLabSystem.EffectiveFinishCorruption(g.C, g.State, g.Me, source));
    }

    [Fact]
    public void Max_corruption_cut_zeros_finish_corruption()
    {
        Game g = Harness();
        g.C.Training.SyntheticDataLab.CorruptionCutPermille = 1000;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.Equal(0, SyntheticDataLabSystem.EffectiveFinishCorruption(g.C, g.State, g.Me, g.C.Training.DataSource(Synthetic)));
    }

    [Fact]
    public void Weekly_fail_risk_drops_with_room()
    {
        Game g = Harness();
        ModelState m = new ModelState { LineId = "mini", DataSourceId = Synthetic, AlignmentSharePermille = 200 };
        DataSourceConfig source = g.C.Training.DataSource(Synthetic);
        int bare = SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source);
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        int withRoom = SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source);
        Assert.True(withRoom < bare);
        Assert.Equal(
            TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m).Effective,
            TrainingSystem.WeeklyFailPermille(g.C, g.State, g.Me, m));
    }

    [Fact]
    public void Training_finish_applies_reduced_corruption()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.True(model.Corruption < 45);
        Assert.True(model.Corruption > 0);
    }

    [Fact]
    public void Without_room_synthetic_finish_keeps_full_corruption()
    {
        Game g = Harness();
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Equal(45, model.Corruption);
    }

    [Fact]
    public void Alignment_share_still_cuts_fail_risk_with_synthetic_lab()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        var low = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me,
            new ModelState { LineId = "mini", DataSourceId = Synthetic, AlignmentSharePermille = 0 });
        var high = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me,
            new ModelState { LineId = "mini", DataSourceId = Synthetic, AlignmentSharePermille = 500 });
        Assert.True(high.AlignmentReduction > low.AlignmentReduction);
        Assert.True(high.Effective <= low.Effective);
    }

    [Fact]
    public void Save_load_keeps_synthetic_lab_hook()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        bool on = SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(on, SyntheticDataLabSystem.Active(loaded.C, loaded.State, loaded.Me, Synthetic));
    }

    [Fact]
    public void Internal_source_ignores_synthetic_lab()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        DataSourceConfig internalMix = g.C.Training.DataSource("internal");
        Assert.Equal(0, SyntheticDataLabSystem.EffectiveFinishCorruption(g.C, g.State, g.Me, internalMix));
    }

    [Fact]
    public void Scrape_distill_unaffected_by_synthetic_lab()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        DataSourceConfig scrape = g.C.Training.DataSource("scrape");
        Assert.Equal(scrape.WeeklyFailRiskPermille,
            SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, scrape));
    }

    [Fact]
    public void Heat_still_applies_on_synthetic_finish()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028, tweak: c => c.Shady.HeatDecayPerWeek = 0);
        g.WithStarterLab(gpus: 24);
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        int heatBefore = g.Me.Heat;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ModelState model = g.Train("mini", 100);
        g.UntilTrained(model);
        Assert.Equal(heatBefore + g.C.Training.DataSource(Synthetic).FinishHeat, g.Me.Heat);
    }
}