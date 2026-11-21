using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ImageSpecialistLineHardeningTests
{
    const string ImageSector = "image-recognition";
    const string DesignSector = "image-design";
    const string OtherSector = "game-ai";

    static Game WithImageSpecialist(out ModelState model)
    {
        Game g = Game.Solo().WithStarterLab(gpus: 48);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 8_000_000_000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2015);
        g.Me.Techs.Add("conv-nets");
        model = g.Train("image-specialist", 280);
        g.UntilTrained(model);
        return g;
    }

    [Fact]
    public void Conv_nets_unlocks_image_specialist_training()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 5_000_000_000);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2015);
        g.Me.Techs.Add("conv-nets");
        ModelState model = g.Train("image-specialist", 250);
        Assert.Equal(ModelStatus.Training, model.Status);
    }

    [Fact]
    public void Line_locked_without_conv_nets_tech()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2015);
        g.Refused(Command.StartTraining(g.Id, "image-specialist", 200), CommandError.LineLocked);
    }

    [Fact]
    public void Specialist_beats_mini_quality_on_image_recognition()
    {
        Game spec = WithImageSpecialist(out ModelState specialist);
        Game miniLab = Game.Solo().WithStarterLab(gpus: 32);
        miniLab.State.Tick = spec.State.Tick;
        Finance.Earn(miniLab.Me, LedgerLine.CapitalIn, 5_000_000_000);
        ModelState mini = miniLab.Train("mini", 280);
        miniLab.UntilTrained(mini);
        if (mini.Capability < specialist.Capability)
            specialist.Capability = mini.Capability;
        long qSpec = DisplayQueries.Quality(spec.C, spec.C.Sector(ImageSector), specialist, spec.State.Tick);
        long qMini = DisplayQueries.Quality(miniLab.C, miniLab.C.Sector(ImageSector), mini, miniLab.State.Tick);
        Assert.True(qSpec > qMini);
    }

    [Fact]
    public void Bonus_applies_on_both_home_sectors_not_game_ai()
    {
        Game g = WithImageSpecialist(out ModelState model);
        long rec = SpecialistSystem.QualityBonusPermille(g.C, model.LineId, ImageSector);
        long design = SpecialistSystem.QualityBonusPermille(g.C, model.LineId, DesignSector);
        Assert.Equal(g.C.Line("image-specialist").SpecialistQualityBonusPermille, rec);
        Assert.Equal(rec, design);
        Assert.Equal(0, SpecialistSystem.QualityBonusPermille(g.C, model.LineId, OtherSector));
    }

    [Fact]
    public void Quality_breakdown_sums_on_image_design_after_specialist_train()
    {
        Game g = WithImageSpecialist(out ModelState model);
        var sector = g.C.Sector(DesignSector);
        var parts = DisplayQueries.QualityParts(g.C, sector, model, g.Tick);
        Assert.Equal(parts.Total, DisplayQueries.Quality(g.C, sector, model, g.Tick));
        Assert.True(parts.FromCapability > 0);
    }

    [Fact]
    public void Zero_config_bonus_never_scales_quality()
    {
        var c = TestData.Config(cfg => cfg.Line("image-specialist").SpecialistQualityBonusPermille = 0);
        Assert.Equal(0, SpecialistSystem.QualityBonusPermille(c, "image-specialist", ImageSector));
    }

    [Fact]
    public void Bankrupt_lab_cannot_start_image_specialist_run()
    {
        Game g = Game.Solo().WithStarterLab(gpus: 32);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2015);
        g.Me.Techs.Add("conv-nets");
        g.Me.Bankrupt = true;
        g.Refused(Command.StartTraining(g.Id, "image-specialist", 200), CommandError.LabBankrupt);
    }

    [Fact]
    public void Bankrupt_lab_still_queries_specialist_quality_for_spectate()
    {
        Game g = WithImageSpecialist(out ModelState model);
        g.Me.Bankrupt = true;
        Assert.True(DisplayQueries.Quality(g.C, g.C.Sector(ImageSector), model, g.State.Tick) > 0);
    }

    [Fact]
    public void Trained_image_specialist_capability_breakdown_matches_market_quality()
    {
        Game g = WithImageSpecialist(out ModelState model);
        var sector = g.C.Sector(ImageSector);
        Assert.Equal(model.Capability, DisplayQueries.TrainedCapabilityParts(model).Total);
        Assert.True(DisplayQueries.Quality(g.C, sector, model, g.Tick) > 0);
        model.Capability = 8_000;
        g.Release(model, ImageSector);
        g.Weeks(4);
        Assert.True(g.Me.LastWeek.Revenue >= 0);
    }

    [Fact]
    public void Save_load_round_trip_keeps_line_and_specialist_bonus()
    {
        Game g = WithImageSpecialist(out ModelState model);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        ModelState theirs = loaded.Me.Model(model.Id);
        Assert.Equal("image-specialist", theirs.LineId);
        Assert.True(SpecialistSystem.QualityBonusPermille(loaded.C, theirs.LineId, ImageSector) > 0);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 4);
    }

    [Fact]
    public void Config_validator_requires_sectors_when_image_bonus_positive()
    {
        GameConfig c = GameConfig.FromJson(TestData.ConfigJson());
        c.Line("image-specialist").SpecialistSectorIds.Clear();
        Assert.Contains(ConfigValidator.Problems(c), p => p.Contains("image-specialist"));
    }
}