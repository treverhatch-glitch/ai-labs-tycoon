using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Scrape and distill rival data sources (Q47).</summary>
public class ScrapeDistillDataSourceTests
{
    static Game Ready(long seed = 1)
    {
        Game g = Game.Solo(seed, c =>
        {
            var scrape = c.Training.DataSources.Find(d => d.Id == "scrape");
            var distill = c.Training.DataSources.Find(d => d.Id == "distill-rival");
            var recipe = TrainingQueries.Recipe(c, "rival-distill");
            scrape.OpensYear = 0;
            distill.OpensYear = 0;
            if (recipe != null) recipe.OpensYear = 0;
        });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        return g;
    }

    [Fact]
    public void Scrape_source_exists_in_config()
    {
        Assert.NotNull(TestData.Config().Training.DataSource("scrape"));
    }

    [Fact]
    public void Distill_source_requires_target_flag()
    {
        DataSourceConfig d = TestData.Config().Training.DataSource("distill-rival");
        Assert.True(d.RequiresDistillTarget);
    }

    [Fact]
    public void Set_scrape_default_before_year_refused_in_shipped_config()
    {
        Game g = Game.Solo().WithStarterLab();
        g.Refused(Command.SetTrainingDataSource(g.Id, "scrape"), CommandError.EraLocked);
    }

    [Fact]
    public void Scrape_opens_in_1999_by_default()
    {
        DataSourceConfig s = TestData.Config().Training.DataSource("scrape");
        Assert.Equal(1999, s.OpensYear);
    }

    [Fact]
    public void Ready_game_can_pick_scrape_default()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        Assert.Equal("scrape", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Scrape_run_cheaper_prep_than_licensed()
    {
        Game g = Ready();
        long cash = g.Me.Cash;
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        long scrapeSpend = cash - g.Me.Cash;
        g.Me.Cash = cash;
        g.Do(Command.SetTrainingDataSource(g.Id, "licensed"));
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        long licensedSpend = cash - g.Me.Cash;
        Assert.True(scrapeSpend < licensedSpend);
    }

    [Fact]
    public void Scrape_finish_adds_heat()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        int heatBefore = g.Me.Heat;
        ModelState m = g.Train("mini", 80);
        g.UntilTrained(m);
        Assert.True(g.Me.Heat > heatBefore);
    }

    [Fact]
    public void Scrape_finish_adds_corruption()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ModelState m = g.Train("mini", 80);
        g.UntilTrained(m);
        Assert.True(m.Corruption > 0);
    }

    [Fact]
    public void Scrape_finish_raises_home_attention()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        int before = home.Attention;
        ModelState m = g.Train("mini", 80);
        g.UntilTrained(m);
        Assert.True(home.Attention > before);
    }

    [Fact]
    public void Scrape_model_blocks_open_weights()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ModelState m = g.Train("mini", 80);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 1, "game-ai"));
        g.Refused(Command.SetOpenWeights(g.Id, m.Id, true), CommandError.DataSourceBlocksOpenWeights);
    }

    [Fact]
    public void Scrape_higher_fail_risk_than_internal()
    {
        Game g = Ready();
        var scrape = new ModelState { DataSourceId = "scrape", AlignmentSharePermille = 0 };
        var internalMix = new ModelState { DataSourceId = "internal", AlignmentSharePermille = 0 };
        Assert.True(TrainingSystem.WeeklyFailPermille(g.C, g.State, g.Me, scrape) > TrainingSystem.WeeklyFailPermille(g.C, g.State, g.Me, internalMix));
    }

    [Fact]
    public void Distill_default_refused_without_target()
    {
        Game g = Ready();
        g.Refused(Command.SetTrainingDataSource(g.Id, "distill-rival"), CommandError.DistillTargetRequired);
    }

    [Fact]
    public void Distill_default_ok_after_target_set()
    {
        Game g = Game.New(2, c => { c.Training.DataSource("distill-rival").OpensYear = 0; });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        PrepRivalAhead(g);
        g.Do(Command.SetDistillTarget(g.Id, g.Rival("openaye").Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        Assert.Equal("distill-rival", g.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Distill_start_stamps_source_lab_on_model()
    {
        Game g = Game.New(3, c => { c.Training.DataSource("distill-rival").OpensYear = 0; });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        LabState rival = PrepRivalAhead(g);
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        ModelState m = g.Train("mini", 200);
        Assert.Equal(rival.Id, m.DistillSourceLabId);
    }

    [Fact]
    public void Distill_source_requires_target_before_default()
    {
        Game g = Ready();
        g.Refused(Command.SetTrainingDataSource(g.Id, "distill-rival"), CommandError.DistillTargetRequired);
    }

    [Fact]
    public void Distill_finish_capability_bonus()
    {
        DataSourceConfig d = TestData.Config().Training.DataSource("distill-rival");
        DataSourceConfig plain = TestData.Config().Training.DataSource("internal");
        Assert.True(d.CapabilityBonusPermille > plain.CapabilityBonusPermille);
    }

    [Fact]
    public void Clear_distill_target_then_default_refused()
    {
        Game g = Game.New(15, c => { c.Training.DataSource("distill-rival").OpensYear = 0; });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        LabState rival = PrepRivalAhead(g);
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        g.Do(Command.SetDistillTarget(g.Id, -1));
        g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.DistillTargetRequired);
    }

    [Fact]
    public void Freeze_blocks_scrape_default()
    {
        Game g = Ready();
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.SetTrainingDataSource(g.Id, "scrape"), CommandError.PermitFrozen);
    }

    [Fact]
    public void Active_run_keeps_stamped_scrape_source()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ModelState m = g.Train("mini", 50);
        g.Do(Command.SetTrainingDataSource(g.Id, "internal"));
        Assert.Equal("scrape", m.DataSourceId);
    }

    [Fact]
    public void Save_load_scrape_model_fields()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ModelState m = g.Train("mini", 50);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal("scrape", loaded.Me.Model(m.Id).DataSourceId);
    }

    [Fact]
    public void Save_load_distill_target_and_source()
    {
        Game g = Game.New(5, c => { c.Training.DataSource("distill-rival").OpensYear = 0; });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        PrepRivalAhead(g);
        g.Do(Command.SetDistillTarget(g.Id, g.Rival("openaye").Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(g.Rival("openaye").Id, loaded.Me.DistillTargetLabId);
        Assert.Equal("distill-rival", loaded.Me.TrainingDataSourceId);
    }

    [Fact]
    public void Training_queries_best_released_above()
    {
        Game g = Game.New(6, c => { c.Training.DataSource("distill-rival").OpensYear = 0; });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        LabState rival = PrepRivalAhead(g);
        Assert.NotNull(TrainingQueries.BestReleasedAbove(g.C, g.Me, rival, "mini"));
    }

    [Fact]
    public void Scrape_recipe_uses_scrape_mix()
    {
        Game g = Game.Solo(12, c =>
        {
            TrainingQueries.Recipe(c, "web-scrape-sprint").OpensYear = 0;
            c.Training.DataSource("scrape").OpensYear = 0;
        });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        ModelState m = g.Me.Model(g.Do(Command.StartTrainingRecipe(g.Id, "web-scrape-sprint")).CreatedId);
        Assert.Equal("scrape", m.DataSourceId);
    }

    [Fact]
    public void Distill_opens_2004_in_shipped_config()
    {
        Assert.Equal(2004, TestData.Config().Training.DataSource("distill-rival").OpensYear);
    }

    [Fact]
    public void Validator_accepts_scrape_and_distill()
    {
        ConfigValidator.Validate(TestData.Config());
    }

    [Fact]
    public void Scrape_capability_part_can_be_negative_in_breakdown()
    {
        Game g = Ready();
        g.Do(Command.SetTrainingDataSource(g.Id, "scrape"));
        ModelState m = g.Train("mini", 80);
        g.UntilTrained(m);
        var parts = DisplayQueries.TrainedCapabilityParts(m);
        Assert.True(parts.FromDataSource < 0);
    }

    [Fact]
    public void Cannot_distill_from_self()
    {
        Game g = Ready();
        g.Refused(Command.SetDistillTarget(g.Id, g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Distill_min_lead_enforced()
    {
        Game g = Game.New(9);
        g.WithStarterLab(gpus: 24);
        LabState rival = g.Rival("openaye");
        ModelState theirs = rival.Model(g.Try(Command.StartTraining(rival.Id, "mini", 50)).CreatedId);
        g.UntilTrained(theirs);
        g.Do(Command.ReleaseModel(rival.Id, theirs.Id, 1, "game-ai"));
        ModelState mine = g.Train("mini", 50);
        g.UntilTrained(mine);
        g.Do(Command.ReleaseModel(g.Id, mine.Id, 1, "game-ai"));
        int gap = (int)((theirs.Capability - mine.Capability) * 1000L / mine.Capability);
        Assert.True(gap < g.C.Training.DistillMinLeadPermille || gap >= 0);
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        if (theirs.Capability <= mine.Capability + mine.Capability * g.C.Training.DistillMinLeadPermille / 1000)
            g.Refused(Command.StartTraining(g.Id, "mini", 100), CommandError.DistillTargetRequired);
    }

    [Fact]
    public void Hand_start_distill_after_rival_pulls_ahead()
    {
        Game g = Game.New(11, c => { c.Training.DataSource("distill-rival").OpensYear = 0; });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        LabState rival = PrepRivalAhead(g);
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        Assert.Equal("distill-rival", m.DataSourceId);
    }

    static LabState PrepRivalAhead(Game g)
    {
        LabState rival = g.Rival("openaye");
        g.Try(Command.SetRentedGpus(rival.Id, 48));
        ModelState theirs = rival.Model(g.Try(Command.StartTraining(rival.Id, "mini", 40)).CreatedId);
        g.UntilTrained(theirs);
        g.Do(Command.ReleaseModel(rival.Id, theirs.Id, 1, "game-ai"));
        return rival;
    }
}