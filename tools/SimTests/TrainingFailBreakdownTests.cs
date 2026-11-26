using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q60: TrainingQueries.WeeklyFailParts decomposes collapse dice.</summary>
public class TrainingFailBreakdownTests
{
    static ModelState Mini(string source = "internal", int align = 200) =>
        new ModelState { LineId = "mini", DataSourceId = source, AlignmentSharePermille = align };

    [Fact]
    public void Parts_effective_matches_training_system()
    {
        Game g = Game.Solo();
        ModelState m = Mini();
        var p = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m);
        Assert.Equal(p.Effective, TrainingSystem.WeeklyFailPermille(g.C, g.State, g.Me, m));
    }

    [Fact]
    public void Internal_source_uses_base_only()
    {
        Game g = Game.Solo();
        var p = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini("internal"));
        Assert.Equal(g.C.Training.BaseFailPermille, p.Base);
        Assert.Equal(0, p.DataSource);
    }

    [Fact]
    public void Synthetic_adds_data_source_risk()
    {
        Game g = Game.Solo();
        var syn = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini("synthetic"));
        var lic = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini("licensed"));
        Assert.True(syn.DataSource > lic.DataSource);
        Assert.True(syn.Effective > lic.Effective);
    }

    [Fact]
    public void Engineers_reduce_effective_risk()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 4);
        ModelState m = Mini("synthetic");
        var p = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m);
        int engineers = LabQueries.StaffByFunction(g.C, g.Me, RoleFunction.Engineering);
        Assert.Equal(engineers * g.C.Training.FailEngineerCutPermille, p.EngineerReduction);
        Assert.True(p.EngineerReduction > 0);
    }

    [Fact]
    public void Alignment_share_cuts_risk()
    {
        Game g = Game.Solo();
        int low = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini(align: 50)).Effective;
        int high = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini(align: 800)).Effective;
        Assert.True(high < low);
    }

    [Fact]
    public void Effective_clamped_at_950()
    {
        Game g = Game.Solo(1, c =>
        {
            c.Training.BaseFailPermille = 900;
            foreach (var s in c.Training.DataSources)
                if (s.Id == "scrape") s.WeeklyFailRiskPermille = 200;
        });
        Assert.Equal(950, TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini("scrape")).Effective);
    }

    [Fact]
    public void Zero_risk_when_fully_mitigated()
    {
        Game g = Game.Solo(1, c =>
        {
            c.Training.BaseFailPermille = 2;
            foreach (var s in c.Training.DataSources)
                if (s.Id == "internal") s.WeeklyFailRiskPermille = 0;
            c.Training.FailEngineerCutPermille = 10;
        });
        g.WithStarterLab(gpus: 4);
        g.Hire("engineer", 1);
        ModelState m = Mini(align: 500);
        Assert.Equal(0, TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m).Effective);
    }

    [Fact]
    public void Synthetic_lab_only_shrinks_synthetic_data_line()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028);
        g.WithStarterLab(gpus: 8);
        ModelState m = Mini("synthetic");
        int before = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m).DataSource;
        ExpansionTestSupport.BuildExpansionRoom(g, "synthetic-data-lab");
        int after = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m).DataSource;
        Assert.True(after < before);
        int internalRisk = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini("internal")).DataSource;
        Assert.Equal(0, internalRisk);
    }

    [Fact]
    public void Bankrupt_lab_parts_still_compute()
    {
        Game g = Game.Solo();
        g.Me.Bankrupt = true;
        Assert.True(TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini()).Effective >= 0);
    }

    [Fact]
    public void Save_load_keeps_fail_parts()
    {
        Game g = Game.Solo();
        ModelState m = Mini("licensed");
        int eff = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, m).Effective;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(eff, TrainingQueries.WeeklyFailParts(loaded.C, loaded.State, loaded.Me, m).Effective);
    }

    [Fact]
    public void Scrape_player_only_still_has_breakdown_risk()
    {
        Game g = Game.Solo();
        var p = TrainingQueries.WeeklyFailParts(g.C, g.State, g.Me, Mini("scrape"));
        Assert.True(p.DataSource > p.Base);
    }

    [Fact]
    public void Plan_lab_uses_same_formula()
    {
        Game g = Game.New();
        LabState plan = g.Rival("openaye");
        ModelState m = Mini("internal");
        var p = TrainingQueries.WeeklyFailParts(g.C, g.State, plan, m);
        Assert.Equal(TrainingSystem.WeeklyFailPermille(g.C, g.State, plan, m), p.Effective);
    }
}