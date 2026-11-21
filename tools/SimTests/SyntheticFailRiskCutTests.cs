using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q112: syntheticDataLab.failRiskCutPermille on weekly collapse odds.</summary>
public class SyntheticFailRiskCutTests
{
    const string Room = "synthetic-data-lab";
    const string Synthetic = "synthetic";

    static Game Harness(int year = 2028) => ExpansionTestSupport.SoloAtYear(year).WithStarterLab(gpus: 24);

  [Fact]
    public void Shipped_fail_risk_cut_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(400, c.Training.SyntheticDataLab.FailRiskCutPermille);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Inactive_lab_leaves_source_risk_unchanged()
    {
        Game g = Harness();
        var source = g.C.Training.DataSource(Synthetic);
        Assert.Equal(source.WeeklyFailRiskPermille,
            SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source));
    }

    [Fact]
    public void Active_synthetic_room_cuts_fail_risk()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        var source = g.C.Training.DataSource(Synthetic);
        int bare = source.WeeklyFailRiskPermille;
        int cut = SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source);
        Assert.True(cut < bare);
    }

    [Fact]
    public void Internal_mix_unaffected_by_fail_cut()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        var internalMix = g.C.Training.DataSource("internal");
        Assert.Equal(internalMix.WeeklyFailRiskPermille,
            SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, internalMix));
    }

    [Fact]
    public void Zero_cut_config_disables_hook()
    {
        Game g = Harness();
        g.C.Training.SyntheticDataLab.FailRiskCutPermille = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        var source = g.C.Training.DataSource(Synthetic);
        Assert.Equal(source.WeeklyFailRiskPermille,
            SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source));
    }

    [Fact]
    public void Cut_flows_into_weekly_fail_parts()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        g.Do(Command.SetTrainingDataSource(g.Id, Synthetic));
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        ModelState m = new ModelState { LineId = "mini", DataSourceId = Synthetic };
        var parts = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m);
        Game bare = Harness();
        bare.Do(Command.SetTrainingDataSource(bare.Id, Synthetic));
        var bareParts = TrainingQueries.WeeklyFailParts(bare.C, bare.State, bare.Me, m);
        Assert.True(parts.Effective < bareParts.Effective);
    }

    [Fact]
    public void Bankruptcy_turns_fail_cut_off()
    {
        Game g = Harness();
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        g.Me.Bankrupt = true;
        var source = g.C.Training.DataSource(Synthetic);
        Assert.Equal(source.WeeklyFailRiskPermille,
            SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source));
    }

    [Fact]
    public void Before_harnesses_era_no_fail_cut()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2024).WithStarterLab(gpus: 24);
        var source = g.C.Training.DataSource(Synthetic);
        Assert.Equal(source.WeeklyFailRiskPermille,
            SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source));
    }

    [Fact]
    public void Stacks_with_morale_fail_cut()
    {
        Game g = Harness();
        g.C.Training.DataSource("scrape").OpensYear = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 800;
        ModelState m = new ModelState { LineId = "mini", DataSourceId = "scrape" };
        var parts = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m);
        Assert.True(parts.MoraleReduction >= 0);
    }

    [Fact]
    public void Save_load_preserves_fail_risk_formula()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        var source = g.C.Training.DataSource(Synthetic);
        int risk = SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, g.Me, source);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(risk, SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(loaded.C, loaded.State, loaded.Me, source));
    }

    [Fact]
    public void Negative_fail_cut_rejected()
    {
        Assert.ThrowsAny<System.Exception>(() =>
            TestData.Config(c => c.Training.SyntheticDataLab.FailRiskCutPermille = -1));
    }

    [Fact]
    public void Plan_lab_never_gets_synthetic_fail_cut()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        var source = g.C.Training.DataSource(Synthetic);
        Assert.Equal(source.WeeklyFailRiskPermille,
            SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(g.C, g.State, plan, source));
    }

    [Fact]
    public void Training_speed_bonus_independent_of_fail_cut()
    {
        Game g = Harness();
        g.C.Training.DataSource(Synthetic).OpensYear = 0;
        ExpansionTestSupport.BuildExpansionRoom(g, Room);
        Assert.True(SyntheticDataLabSystem.Active(g.C, g.State, g.Me, Synthetic));
        Assert.True(g.C.Training.SyntheticDataLab.TrainingSpeedBonusPermille > 0);
        Assert.True(g.C.Training.SyntheticDataLab.FailRiskCutPermille > 0);
    }
}