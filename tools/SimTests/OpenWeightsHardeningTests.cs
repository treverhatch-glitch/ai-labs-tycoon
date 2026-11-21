using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class OpenWeightsHardeningTests
{
    const string Sector = "game-ai";

    static Game ReleasedMini(out ModelState model)
    {
        Game g = Game.Solo().WithStarterLab();
        model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Release(model, Sector);
        return g;
    }

    [Fact]
    public void Main_path_open_weights_cut_revenue_and_raise_appeal()
    {
        Game g = ReleasedMini(out ModelState model);
        SectorState sectorState = g.State.Sector(Sector);
        SectorConfig sector = g.C.Sector(Sector);
        long qClosed = DisplayQueries.Quality(g.C, sector, model, g.State.Tick);
        g.Weeks(24);
        long revenueClosed = sectorState.Entry(g.Id).LastRevenue;
        Assert.True(revenueClosed > 0);
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        long qOpen = DisplayQueries.Quality(g.C, sector, model, g.State.Tick);
        Assert.True(qOpen > qClosed);
        g.Sim.Step();
        long revenueOpen = sectorState.Entry(g.Id).LastRevenue;
        Assert.True(revenueOpen < revenueClosed);
    }

    [Fact]
    public void Closing_open_weights_restores_revenue_permute()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        g.Do(Command.SetOpenWeights(g.Id, model.Id, false));
        Assert.False(model.OpenWeights);
    }

    [Fact]
    public void Trained_but_unreleased_model_cannot_publish_weights()
    {
        Game g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 200);
        g.UntilTrained(model);
        g.Refused(Command.SetOpenWeights(g.Id, model.Id, true), CommandError.InvalidArgument);
    }

    [Fact]
    public void Unknown_model_id_is_refused()
    {
        Game g = ReleasedMini(out _);
        g.Refused(Command.SetOpenWeights(g.Id, 99999, true), CommandError.UnknownId);
    }

    [Fact]
    public void Bankrupt_lab_cannot_toggle_open_weights()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Me.Bankrupt = true;
        g.Refused(Command.SetOpenWeights(g.Id, model.Id, true), CommandError.LabBankrupt);
    }

    [Fact]
    public void Zero_revenue_permille_config_still_validates()
    {
        var c = TestData.Config(cfg =>
        {
            cfg.Market.OpenWeightsRevenuePermille = 1;
            cfg.Market.OpenWeightsAppealBonusPermille = 0;
        });
        Assert.Empty(ConfigValidator.Problems(c));
    }

    [Fact]
    public void Max_appeal_bonus_scales_quality_before_square()
    {
        Game g = ReleasedMini(out ModelState model);
        SectorConfig sector = g.C.Sector(Sector);
        var tuned = TestData.Config(cfg => cfg.Market.OpenWeightsAppealBonusPermille = 400);
        long closed = DisplayQueries.Quality(tuned, sector, model, g.State.Tick);
        model.OpenWeights = true;
        long open = DisplayQueries.Quality(tuned, sector, model, g.State.Tick);
        Assert.Equal(MathI.MulPermille(closed, 1400), open);
    }

    [Fact]
    public void Save_load_round_trip_keeps_open_weights_flag()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.True(loaded.Me.Model(model.Id).OpenWeights);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 6);
    }

    [Theory]
    [MemberData(nameof(SimHardeningSupport.FixtureRows), MemberType = typeof(SimHardeningSupport))]
    public void Migrated_fixtures_default_open_weights_false(string file)
    {
        GameConfig c = TestData.Config();
        Simulation sim = SimHardeningSupport.LoadFixture(c, file);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        foreach (LabState lab in sim.State.Labs)
            foreach (ModelState model in lab.Models)
                Assert.False(model.OpenWeights);
        SimHardeningSupport.AssertMigratedFixtureSteps(c, file, 4);
    }

    [Fact]
    public void Sector_price_and_open_weights_stack_on_revenue()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Weeks(24);
        g.Do(Command.SetSectorPrice(g.Id, Sector, g.C.Market.SectorPriceMaxPermille));
        g.Sim.Step();
        long priced = g.State.Sector(Sector).Entry(g.Id).LastRevenue;
        Assert.True(priced > 0);
        g.Do(Command.SetOpenWeights(g.Id, model.Id, true));
        g.Sim.Step();
        long both = g.State.Sector(Sector).Entry(g.Id).LastRevenue;
        Assert.True(both < priced);
    }
}