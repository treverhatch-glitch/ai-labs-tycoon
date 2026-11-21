using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Plan labs use Rivals collapse dice and never shady data sources (Q48–Q49).</summary>
public class PlanLabTrainingFairnessTests
{
    static Game Risky(long seed = 5) =>
        Game.New(seed, c =>
        {
            c.Training.BaseFailPermille = 950;
            c.Training.FailAfterProgressPermille = 0;
            c.Training.FailEngineerCutPermille = 0;
            c.Training.FailAlignmentCutPermille = 0;
        });

    [Fact]
    public void CollapseRng_player_uses_training_collapse_stream()
    {
        Game g = Game.New();
        Assert.Same(g.State.Rng.TrainingCollapse, TrainingSystem.CollapseRng(g.State, g.Me));
    }

    [Fact]
    public void CollapseRng_plan_lab_uses_rivals_stream()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        Assert.Same(g.State.Rng.Rivals, TrainingSystem.CollapseRng(g.State, rival));
    }

    [Fact]
    public void Scrape_and_distill_marked_player_only_in_shipped_config()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Training.DataSource("scrape").PlayerOnly);
        Assert.True(c.Training.DataSource("distill-rival").PlayerOnly);
        Assert.False(c.Training.DataSource("internal").PlayerOnly);
        Assert.False(c.Training.DataSource("licensed").PlayerOnly);
        Assert.False(c.Training.DataSource("synthetic").PlayerOnly);
    }

    [Fact]
    public void Plan_plan_data_source_strips_scrape_default()
    {
        Game g = Game.New();
        LabState rival = g.Rival("anthoptics");
        rival.TrainingDataSourceId = "scrape";
        Assert.Equal("internal", TrainingQueries.PlanDataSourceId(g.C, rival));
    }

    [Fact]
    public void Player_can_pick_scrape_when_era_open()
    {
        Game g = Game.Solo();
        g.Years(5);
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        Assert.Equal("scrape", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Plan_start_training_ignores_scrape_lab_default()
    {
        Game g = Risky();
        LabState rival = g.Rival("openaye");
        rival.TrainingDataSourceId = "scrape";
        g.Sim.Apply(Command.StartTraining(rival.Id, "mini", 300));
        ModelState run = rival.Models.Single(m => m.Status == ModelStatus.Training);
        Assert.Equal("internal", run.DataSourceId);
    }

    [Fact]
    public void Set_training_data_source_refused_for_plan_lab_scrape()
    {
        Game g = Game.New();
        LabState rival = g.Rival("xay");
        rival.TrainingDataSourceId = "internal";
        var r = g.Sim.Apply(Command.SetTrainingDataSource(rival.Id, "scrape"));
        Assert.False(r.Ok);
        Assert.Equal(CommandError.PlayerOnlyDataSource, r.Error);
    }

    [Fact]
    public void Set_distill_target_refused_for_plan_lab()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        g.Refused(Command.SetDistillTarget(rival.Id, g.Me.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Start_training_recipe_refused_for_plan_lab()
    {
        Game g = Game.New();
        LabState rival = g.Rival("openaye");
        var r = g.Sim.Apply(Command.StartTrainingRecipe(rival.Id, "diligent-mini"));
        Assert.Equal(CommandError.InvalidArgument, r.Error);
    }

    [Fact]
    public void Rival_run_can_fire_training_failed_under_crunch()
    {
        Game g = Risky();
        LabState rival = g.Rival("openaye");
        g.Sim.Apply(Command.StartTraining(rival.Id, "mini", 50));
        ModelState run = rival.Models.Single(m => m.Status == ModelStatus.Training);
        run.ProgressCompute = run.RequiredCompute - 1;
        TrainingSystem.RunWeek(g.C, g.State, rival);
        bool failed = rival.Models.Any(m => m.Status == ModelStatus.Failed)
                      || g.State.Events.Any(e => e.Kind == EventKind.TrainingFailed && e.SubjectId == rival.Id);
        Assert.True(failed);
    }

    [Fact]
    public void Rival_collapse_does_not_advance_player_training_collapse_stream()
    {
        Game g = Risky();
        long before = g.State.Rng.TrainingCollapse.State;
        LabState rival = g.Rival("openaye");
        g.Sim.Apply(Command.StartTraining(rival.Id, "mini", 50));
        ModelState run = rival.Models.Single(m => m.Status == ModelStatus.Training);
        run.ProgressCompute = run.RequiredCompute - 1;
        TrainingSystem.RunWeek(g.C, g.State, rival);
        Assert.Equal(before, g.State.Rng.TrainingCollapse.State);
    }

    [Fact]
    public void Fifteen_year_rival_sims_never_stamp_scrape_or_distill()
    {
        Game g = Game.New(3);
        g.Years(15);
        foreach (LabState lab in g.State.Labs)
        {
            if (lab.Controller != LabController.Plan) continue;
            Assert.DoesNotContain(lab.Models, m => m.DataSourceId == "scrape" || m.DataSourceId == "distill-rival");
        }
    }

    [Fact]
    public void Source_usable_rejects_scrape_for_plan_lab()
    {
        GameConfig c = TestData.Config();
        var state = Simulation.NewGame(c, 1).State;
        LabState rival = state.Labs.First(l => l.Controller == LabController.Plan);
        Assert.False(TrainingQueries.SourceUsable(c, state, rival, "scrape", "mini", out _));
        Assert.False(TrainingQueries.SourceUsable(c, state, rival, "distill-rival", "mini", out _));
        Assert.True(TrainingQueries.SourceUsable(c, state, rival, "licensed", "mini", out _));
    }

    [Fact]
    public void Plan_distill_target_is_never_set_by_commands()
    {
        Game g = Game.New();
        foreach (LabState lab in g.State.Labs)
            if (lab.Controller == LabController.Plan)
                Assert.Equal(-1, lab.DistillTargetLabId);
    }

    [Fact]
    public void Player_high_risk_still_uses_training_collapse_stream()
    {
        Game g = Risky();
        g.WithStarterLab(gpus: 8);
        long before = g.State.Rng.TrainingCollapse.State;
        g.Do(Command.SetTrainingDataSource(g.Id, "synthetic"));
        g.Do(Command.StartTraining(g.Id, "mini", 0));
        g.Weeks(6);
        Assert.NotEqual(before, g.State.Rng.TrainingCollapse.State);
    }

    [Fact]
    public void Licensed_default_still_allowed_for_plan_starts_when_set()
    {
        Game g = Risky();
        LabState rival = g.Rival("anthoptics");
        rival.TrainingDataSourceId = "licensed";
        g.Sim.Apply(Command.StartTraining(rival.Id, "mini", 400));
        Assert.Equal("licensed", rival.Models.Single(m => m.Status == ModelStatus.Training).DataSourceId);
    }

    [Fact]
    public void Plan_failed_run_cleared_next_week_by_sim_step()
    {
        Game g = Risky();
        LabState rival = g.Rival("openaye");
        g.Sim.Apply(Command.StartTraining(rival.Id, "mini", 50));
        var run = rival.Models.Single(m => m.Status == ModelStatus.Training);
        run.ProgressCompute = run.RequiredCompute - 1;
        TrainingSystem.RunWeek(g.C, g.State, rival);
        if (rival.Models.All(m => m.Status != ModelStatus.Failed)) return;
        g.Sim.Step();
        Assert.DoesNotContain(rival.Models, m => m.Status == ModelStatus.Failed);
    }
}