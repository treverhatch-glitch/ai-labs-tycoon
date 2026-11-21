using AiLabs.Rules.Config;
using AiLabs.SimTests.Support;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using Xunit;

namespace AiLabs.SimTests;

public class SpecialistLineHardeningTests
{
    const string CodeSector = "code-completion";
    const string OtherSector = "game-ai";

    static Game WithCodeSpecialist(out ModelState model)
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 5_000_000_000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2020);
        g.Me.Techs.Add("code-models");
        model = g.Train("code-specialist", 300);
        g.UntilTrained(model);
        return g;
    }

    [Fact]
    public void Main_path_specialist_beats_mini_quality_in_code_completion()
    {
        Game spec = WithCodeSpecialist(out ModelState specialist);
        Game miniLab = Game.Solo().WithStarterLab();
        miniLab.State.Tick = spec.State.Tick;
        Finance.Earn(miniLab.Me, LedgerLine.CapitalIn, 2_000_000);
        ModelState mini = miniLab.Train("mini", 300);
        miniLab.UntilTrained(mini);
        if (mini.Capability < specialist.Capability)
            specialist.Capability = mini.Capability;
        SectorConfig sector = spec.C.Sector(CodeSector);
        long qSpec = DisplayQueries.Quality(spec.C, sector, specialist, spec.State.Tick);
        long qMini = DisplayQueries.Quality(miniLab.C, sector, mini, miniLab.State.Tick);
        Assert.True(qSpec > qMini);
    }

    [Fact]
    public void Specialist_bonus_missing_outside_home_sectors()
    {
        Game g = WithCodeSpecialist(out ModelState model);
        SectorConfig home = g.C.Sector(CodeSector);
        SectorConfig away = g.C.Sector(OtherSector);
        long atHome = DisplayQueries.QualityParts(g.C, home, model, g.State.Tick).FromCapability;
        long awayCap = DisplayQueries.QualityParts(g.C, away, model, g.State.Tick).FromCapability;
        Assert.True(atHome > awayCap);
    }

    [Fact]
    public void Coding_agents_is_the_second_home_sector()
    {
        Game g = WithCodeSpecialist(out ModelState model);
        int bonus = SpecialistSystem.QualityBonusPermille(g.C, model.LineId, "coding-agents");
        Assert.Equal(g.C.Line("code-specialist").SpecialistQualityBonusPermille, bonus);
    }

    [Fact]
    public void Line_locked_without_code_models_tech()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2020);
        g.Refused(Command.StartTraining(g.Id, "code-specialist", 200), CommandError.LineLocked);
    }

    [Fact]
    public void Unknown_line_has_no_specialist_bonus()
    {
        Assert.Equal(0, SpecialistSystem.QualityBonusPermille(TestData.Config(), "mini", CodeSector));
    }

    [Fact]
    public void Zero_bonus_permille_in_config_never_scales()
    {
        var c = TestData.Config(cfg =>
        {
            ModelLineConfig line = cfg.Line("code-specialist");
            line.SpecialistQualityBonusPermille = 0;
        });
        Assert.Equal(0, SpecialistSystem.QualityBonusPermille(c, "code-specialist", CodeSector));
    }

    [Fact]
    public void Bankrupt_lab_can_still_query_quality_for_spectate()
    {
        Game g = WithCodeSpecialist(out ModelState model);
        g.Me.Bankrupt = true;
        Assert.True(DisplayQueries.Quality(g.C, g.C.Sector(CodeSector), model, g.State.Tick) > 0);
    }

    [Fact]
    public void Save_load_round_trip_keeps_line_id_for_bonus()
    {
        Game g = WithCodeSpecialist(out ModelState model);
        g.Release(model, "game-ai");
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState theirs = loaded.Me.Model(model.Id);
        Assert.Equal("code-specialist", theirs.LineId);
        Assert.True(SpecialistSystem.QualityBonusPermille(loaded.C, theirs.LineId, CodeSector) > 0);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 4);
    }

    [Theory]
    [MemberData(nameof(SimHardeningSupport.FixtureRows), MemberType = typeof(SimHardeningSupport))]
    public void Migrated_fixture_steps_with_specialist_line_in_config(string file)
    {
        GameConfig c = TestData.Config();
        Assert.NotNull(c.Line("code-specialist"));
        SimHardeningSupport.AssertMigratedFixtureSteps(c, file, 4);
    }

    [Fact]
    public void Market_release_earns_in_code_completion_after_specialist_training()
    {
        Game g = WithCodeSpecialist(out ModelState model);
        g.Release(model, "game-ai");
        g.Weeks(2);
        Assert.True(g.Me.LastWeek.Revenue > 0);
    }

    [Fact]
    public void Config_validator_requires_sectors_when_bonus_positive()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Line("code-specialist").SpecialistSectorIds.Clear();
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("code-specialist"));
    }
}