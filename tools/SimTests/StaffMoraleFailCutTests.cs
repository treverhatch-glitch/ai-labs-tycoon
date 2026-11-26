using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q88: high staff morale cuts weekly training-failure risk.</summary>
public class StaffMoraleFailCutTests
{
    static ModelState TrainingModel(Game g, string source = "internal") =>
        new ModelState { LineId = "mini", DataSourceId = source, AlignmentSharePermille = 100 };

    [Fact]
    public void Shipped_morale_fail_cut_config_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.StaffPeople.MoraleFailCutPermillePerPoint);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Baseline_morale_zero_fail_cut()
    {
        Game g = Game.Solo().WithStarterLab();
        Assert.Equal(0, StaffSystem.MoraleFailCutPermille(g.C, g.Me));
    }

    [Fact]
    public void High_morale_lowers_weekly_fail_parts()
    {
        Game sad = Game.Solo();
        Game happy = Game.Solo();
        sad.C.Training.DataSource("scrape").OpensYear = 0;
        happy.C.Training.DataSource("scrape").OpensYear = 0;
        sad.WithStarterLab();
        happy.WithStarterLab();
        foreach (StaffMember p in happy.Me.Staff) p.Morale = happy.C.StaffPeople.MoraleBaseline + 50;
        ModelState m = TrainingModel(happy, "scrape");
        var high = TrainingQueries.WeeklyFailParts(happy.C, happy.State, happy.Me, m);
        var low = TrainingQueries.WeeklyFailParts(sad.C, sad.State, sad.Me, m);
        Assert.True(high.MoraleReduction > 0);
        Assert.True(high.Effective < low.Effective);
    }

    [Fact]
    public void Unhappy_staff_no_fail_cut()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = g.C.StaffPeople.MoraleBaseline - 30;
        Assert.Equal(0, StaffSystem.MoraleFailCutPermille(g.C, g.Me));
    }

    [Fact]
    public void Zero_scale_config_disables_fail_cut()
    {
        Game g = Game.Solo().WithStarterLab();
        g.C.StaffPeople.MoraleFailCutPermillePerPoint = 0;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        Assert.Equal(0, StaffSystem.MoraleFailCutPermille(g.C, g.Me));
    }

    [Fact]
    public void Weekly_fail_permille_matches_parts()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 650;
        ModelState m = TrainingModel(g, "synthetic");
        var parts = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m);
        Assert.Equal(parts.Effective, TrainingSystem.WeeklyFailPermille(g.C, g.State, g.Me, m));
    }

    [Fact]
    public void Synthetic_lab_stacks_with_morale_cut()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028).WithStarterLab(gpus: 16);
        ExpansionTestSupport.BuildExpansionRoom(g, "synthetic-data-lab");
        foreach (StaffMember p in g.Me.Staff) p.Morale = 700;
        ModelState m = TrainingModel(g, "synthetic");
        var parts = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m);
        Assert.True(parts.MoraleReduction > 0);
        Assert.True(parts.DataSource <= g.C.Training.DataSource("synthetic").WeeklyFailRiskPermille);
    }

    [Fact]
    public void No_staff_zero_cut()
    {
        Game g = Game.Solo();
        Assert.Equal(0, StaffSystem.MoraleFailCutPermille(g.C, g.Me));
    }

    [Fact]
    public void Engineers_and_morale_both_reduce_risk()
    {
        Game g = Game.Solo(1, c =>
        {
            c.Training.DataSource("scrape").OpensYear = 0;
            c.Training.DataSource("scrape").WeeklyFailRiskPermille = 120;
        });
        g.WithStarterLab(engineers: 0);
        int happy = g.C.StaffPeople.MoraleBaseline + 60;
        foreach (StaffMember p in g.Me.Staff) p.Morale = happy;
        ModelState m = TrainingModel(g, "scrape");
        int bare = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m).Effective;
        for (int i = 0; i < 4; i++)
            g.Me.Staff.Add(new StaffMember { RoleId = "engineer", Morale = happy, TraitId = "steady" });
        int staffed = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m).Effective;
        Assert.True(staffed < bare);
    }

    [Fact]
    public void Save_load_preserves_fail_parts()
    {
        Game g = Game.Solo().WithStarterLab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 720;
        ModelState m = TrainingModel(g);
        int eff = TrainingSystem.WeeklyFailPermille(g.C, g.State, g.Me, m);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        foreach (StaffMember p in loaded.Me.Staff) p.Morale = 720;
        Assert.Equal(eff, TrainingSystem.WeeklyFailPermille(loaded.C, loaded.State, loaded.Me, m));
    }

    [Fact]
    public void Morale_cut_clamped_with_other_reductions()
    {
        Game g = Game.Solo(1, c => c.Training.DataSource("scrape").OpensYear = 0);
        g.WithStarterLab(engineers: 2, safety: 2);
        for (int i = 0; i < 3; i++)
            g.Me.Staff.Add(new StaffMember { RoleId = "engineer", Morale = 900, TraitId = "steady" });
        foreach (StaffMember p in g.Me.Staff) p.Morale = 900;
        ModelState m = TrainingModel(g, "scrape");
        m.AlignmentSharePermille = 800;
        var parts = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m);
        Assert.InRange(parts.Effective, 0, 950);
        Assert.True(parts.MoraleReduction > 0);
    }

    [Fact]
    public void Plan_lab_uses_same_morale_cut()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        plan.Staff.Add(new StaffMember { RoleId = "researcher", Morale = 800, TraitId = g.C.StaffPeople.Traits[0].Id });
        Assert.True(StaffSystem.MoraleFailCutPermille(g.C, plan) > 0);
    }

    [Fact]
    public void Formula_uses_points_above_baseline_only()
    {
        GameConfig c = TestData.Config();
        var lab = new LabState();
        lab.Staff.Add(new StaffMember { Morale = c.StaffPeople.MoraleBaseline + 25 });
        Assert.Equal(25 * c.StaffPeople.MoraleFailCutPermillePerPoint, StaffSystem.MoraleFailCutPermille(c, lab));
    }
}