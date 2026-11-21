using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Training recipes (Q46).</summary>
public class TrainingRecipeTests
{
    static Game Kitchen()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        return g;
    }

    [Fact]
    public void Recipe_stamps_line_source_and_alignment()
    {
        Game g = Kitchen();
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "diligent-mini")).CreatedId);
        Assert.Equal("mini", m.LineId);
        Assert.Equal("internal", m.DataSourceId);
        Assert.Equal(350, m.AlignmentSharePermille);
        Assert.Equal("diligent-mini", m.RecipeId);
    }

    [Fact]
    public void Fast_recipe_reduces_required_compute()
    {
        Game g = Kitchen();
        ModelLineConfig line = g.C.Line("mini");
        long full = TrainingQueries.RequiredCompute(g.C, line, g.Tick, 1000);
        long fast = TrainingQueries.RequiredCompute(g.C, line, g.Tick, 850);
        Assert.True(fast < full);
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "fast-mini")).CreatedId);
        Assert.Equal(fast, m.RequiredCompute);
    }

    [Fact]
    public void Unknown_recipe_refused()
    {
        Game g = Kitchen();
        g.Refused(Command.StartTrainingRecipe(g.Id, "void"), CommandError.UnknownId);
    }

    [Fact]
    public void Locked_recipe_refused_by_era()
    {
        Game g = Kitchen();
        g.Refused(Command.StartTrainingRecipe(g.Id, "synthetic-gamble"), CommandError.EraLocked);
    }

    [Fact]
    public void Recipe_does_not_change_lab_default_source()
    {
        Game g = Kitchen();
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        g.Do(Command.StartTrainingRecipe(g.Id, "diligent-mini"));
        Assert.Equal("licensed", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Licensed_push_recipe_uses_licensed_mix()
    {
        Game g = Kitchen();
        g.Years(3);
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "licensed-push")).CreatedId);
        Assert.Equal("licensed", m.DataSourceId);
        Assert.Equal(500, m.AlignmentSharePermille);
    }

    [Fact]
    public void Recipe_respects_max_concurrent_runs()
    {
        Game g = Kitchen();
        g.Do(Command.StartTrainingRecipe(g.Id, "diligent-mini"));
        g.Do(Command.StartTrainingRecipe(g.Id, "fast-mini"));
        g.Refused(Command.StartTrainingRecipe(g.Id, "diligent-mini"), CommandError.TooManyRuns);
    }

    [Fact]
    public void Recipe_blocked_when_home_frozen()
    {
        Game g = Kitchen();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.StartTrainingRecipe(g.Id, "diligent-mini"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Distill_recipe_requires_target()
    {
        Game g = Game.Solo();
        g.Years(10);
        g.Refused(Command.StartTrainingRecipe(g.Id, "rival-distill"), CommandError.DistillTargetRequired);
    }

    [Fact]
    public void Distill_recipe_succeeds_with_valid_target()
    {
        Game g = Game.New(7, c =>
        {
            TrainingQueries.Recipe(c, "rival-distill").OpensYear = 0;
            c.Training.DataSource("distill-rival").OpensYear = 0;
        });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        LabState rival = g.Rival("openaye");
        ModelState theirs = rival.Model(g.Try(Command.StartTraining(rival.Id, "mini", 100)).CreatedId);
        g.UntilTrained(theirs);
        g.Do(Command.ReleaseModel(rival.Id, theirs.Id, 1, "game-ai"));
        g.Me.Cash += 500_000;
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "rival-distill")).CreatedId);
        Assert.Equal("distill-rival", m.DataSourceId);
        Assert.Equal(rival.Id, m.DistillSourceLabId);
    }

    [Fact]
    public void Recipe_training_finishes()
    {
        Game g = Kitchen();
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "fast-mini")).CreatedId);
        g.UntilTrained(m);
        Assert.Equal(ModelStatus.Trained, m.Status);
    }

    [Fact]
    public void Validator_knows_every_recipe_line_and_source()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Training.Recipes.Count >= 5);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Save_round_trip_keeps_recipe_fields()
    {
        Game g = Kitchen();
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "diligent-mini")).CreatedId);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState again = loaded.Me.Model(m.Id);
        Assert.Equal("diligent-mini", again.RecipeId);
    }

    [Fact]
    public void Migrate_17_to_18_adds_recipe_fields()
    {
        Game g = Kitchen();
        string json = g.Sim.Save().Replace("\"schema\":24", "\"schema\":17");
        var loaded = new Game(Simulation.Load(g.C, json));
        Assert.Equal(24, loaded.State.Schema);
        Assert.Equal(-1, loaded.Me.DistillTargetLabId);
    }

    [Fact]
    public void Schema_22_on_new_game()
    {
        Assert.Equal(24, Game.Solo().State.Schema);
    }

    [Fact]
    public void Manual_start_still_works_after_recipes()
    {
        Game g = Kitchen();
        ModelState m = g.Train("mini", 200);
        Assert.True(string.IsNullOrEmpty(m.RecipeId));
        g.UntilTrained(m);
    }

    [Fact]
    public void Web_scrape_recipe_waits_for_1999()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        g.Refused(Command.StartTrainingRecipe(g.Id, "web-scrape-sprint"), CommandError.EraLocked);
        g.Years(5);
        g.Do(Command.StartTrainingRecipe(g.Id, "web-scrape-sprint"));
    }

    [Fact]
    public void Recipe_refused_when_line_locked()
    {
        Game g = Game.Solo(1, c =>
        {
            var r = c.Training.Recipes.Find(x => x.Id == "diligent-mini");
            r.LineId = "flagship";
        });
        g.WithStarterLab();
        g.Refused(Command.StartTrainingRecipe(g.Id, "diligent-mini"), CommandError.LineLocked);
    }

    [Fact]
    public void Recipe_prep_cost_uses_recipe_source()
    {
        Game g = Kitchen();
        g.Years(3);
        long before = g.Me.Cash;
        g.Do(Command.StartTrainingRecipe(g.Id, "licensed-push"));
        long licensed = before - g.Me.Cash;
        g.Me.Cash = before;
        g.Do(Command.StartTrainingRecipe(g.Id, "diligent-mini"));
        long internalCost = before - g.Me.Cash;
        Assert.True(licensed > internalCost);
    }

    [Fact]
    public void Set_distill_target_clears_with_negative_id()
    {
        Game g = Game.New(3);
        g.Do(Command.SetDistillTarget(g.Id, g.Rival("xay").Id));
        g.Do(Command.SetDistillTarget(g.Id, -1));
        Assert.Equal(-1, g.Me.DistillTargetLabId);
    }

    [Fact]
    public void Set_distill_target_emits_event()
    {
        Game g = Game.New(4);
        g.Do(Command.SetDistillTarget(g.Id, g.Rival("anthoptics").Id));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DistillTargetSet);
    }

    [Fact]
    public void Distill_target_blocked_when_frozen()
    {
        Game g = Game.New(5);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetDistillTarget(g.Id, g.Rival("xay").Id), CommandError.PermitFrozen);
    }

    [Fact]
    public void Distill_unusable_when_player_already_leads_on_line()
    {
        GameConfig c = TestData.Config();
        var player = new LabState();
        var rival = new LabState { Id = 2 };
        player.Models.Add(new ModelState { LineId = "mini", Status = ModelStatus.Released, Capability = 220 });
        rival.Models.Add(new ModelState { LineId = "mini", Status = ModelStatus.Released, Capability = 180 });
        player.DistillTargetLabId = rival.Id;
        Assert.Null(TrainingQueries.BestReleasedAbove(c, player, rival, "mini"));
        Assert.False(TrainingQueries.SourceUsable(c, new GameState(), player, "distill-rival", "mini", out _));
    }

    [Fact]
    public void Training_queries_recipe_open_check()
    {
        Game g = Kitchen();
        TrainingRecipeConfig r = TrainingQueries.Recipe(g.C, "synthetic-gamble");
        Assert.False(TrainingQueries.RecipeOpen(g.C, r, g.Tick));
        g.Years(12);
        Assert.True(TrainingQueries.RecipeOpen(g.C, r, g.Tick));
    }

    [Fact]
    public void Scrap_failed_recipe_run_allows_retry()
    {
        Game g = Game.Solo(99, c =>
        {
            c.Training.BaseFailPermille = 900;
            c.Training.FailAfterProgressPermille = 0;
        });
        g.WithStarterLab(gpus: 24);
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "fast-mini")).CreatedId);
        g.Until(() => m.Status == ModelStatus.Failed, 12, "fail");
        g.Do(Command.ScrapFailedRun(g.Id, m.Id));
        g.Do(Command.StartTrainingRecipe(g.Id, "diligent-mini"));
    }

    [Fact]
    public void Recipe_refused_without_cash()
    {
        Game g = Kitchen();
        g.Me.Cash = 0;
        g.Refused(Command.StartTrainingRecipe(g.Id, "diligent-mini"), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Event_text_for_distill_target()
    {
        Game g = Game.New(8);
        g.Do(Command.SetDistillTarget(g.Id, g.Rival("xay").Id));
        var e = g.State.Events.Last(ev => ev.Kind == EventKind.DistillTargetSet);
        string line = EventText.Describe(g.C, g.State, e);
        Assert.Contains("distill", line.ToLowerInvariant());
    }

    [Fact]
    public void Two_different_recipes_produce_different_alignment()
    {
        Game g = Kitchen();
        ModelState a = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "diligent-mini")).CreatedId);
        ModelState b = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "fast-mini")).CreatedId);
        Assert.NotEqual(a.AlignmentSharePermille, b.AlignmentSharePermille);
    }

    [Fact]
    public void Recipe_id_empty_for_hand_started_run()
    {
        Game g = Kitchen();
        Assert.True(string.IsNullOrEmpty(g.Train("mini", 150).RecipeId));
    }

    [Fact]
    public void Deterministic_recipe_start_same_seed()
    {
        Game a = Kitchen();
        Game b = Game.Solo(1).WithStarterLab(gpus: 24);
        ModelState ma = a.Me.Model(a.Do(Command.StartTrainingRecipe(a.Id, "fast-mini")).CreatedId);
        ModelState mb = b.Me.Model(b.Do(Command.StartTrainingRecipe(b.Id, "fast-mini")).CreatedId);
        Assert.Equal(ma.RequiredCompute, mb.RequiredCompute);
        Assert.Equal(ma.AlignmentSharePermille, mb.AlignmentSharePermille);
    }

    [Fact]
    public void Config_has_distill_min_lead()
    {
        Assert.True(TestData.Config().Training.DistillMinLeadPermille > 0);
    }
}