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
    public void Line_unlocks_with_conv_nets_and_beats_mini_on_image_recognition()
    {
        Game spec = WithImageSpecialist(out ModelState specialist);
        Game miniLab = Game.Solo().WithStarterLab();
        miniLab.State.Tick = spec.State.Tick;
        Finance.Earn(miniLab.Me, LedgerLine.CapitalIn, 3_000_000);
        ModelState mini = miniLab.Train("mini", 280);
        miniLab.UntilTrained(mini);
        if (mini.Capability < specialist.Capability)
            specialist.Capability = mini.Capability;
        long qSpec = DisplayQueries.Quality(spec.C, spec.C.Sector(ImageSector), specialist, spec.State.Tick);
        long qMini = DisplayQueries.Quality(miniLab.C, miniLab.C.Sector(ImageSector), mini, miniLab.State.Tick);
        Assert.True(qSpec > qMini);
    }

    [Fact]
    public void Bonus_applies_on_image_design_not_game_ai()
    {
        Game g = WithImageSpecialist(out ModelState model);
        var home = g.C.Sector(DesignSector);
        var away = g.C.Sector(OtherSector);
        long atHome = DisplayQueries.QualityParts(g.C, home, model, g.State.Tick).FromCapability;
        long awayCap = DisplayQueries.QualityParts(g.C, away, model, g.State.Tick).FromCapability;
        Assert.True(atHome > awayCap);
    }

    [Fact]
    public void Line_locked_without_conv_nets()
    {
        Game g = Game.Solo().WithStarterLab();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2015);
        g.Refused(Command.StartTraining(g.Id, "image-specialist", 200), CommandError.LineLocked);
    }

    [Fact]
    public void Config_validator_accepts_image_specialist_line()
    {
        Assert.Empty(ConfigValidator.Problems(TestData.Config()));
    }
}