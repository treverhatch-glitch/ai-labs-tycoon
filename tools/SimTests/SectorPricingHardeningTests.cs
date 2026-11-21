using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class SectorPricingHardeningTests
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
    public void Release_leaves_sector_price_at_list_permille()
    {
        Game g = ReleasedMini(out ModelState model);
        SectorEntry entry = g.State.Sector(Sector).Entry(g.Id);
        Assert.Equal(MathI.Permille, entry.PricePermille);
    }

    [Fact]
    public void Lower_price_scales_week_revenue_without_changing_appeal()
    {
        Game g = ReleasedMini(out ModelState model);
        SectorConfig sector = g.C.Sector(Sector);
        SectorState sectorState = g.State.Sector(Sector);
        long appeal = DisplayQueries.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        g.Sim.Step();
        long listRevenue = sectorState.Entry(g.Id).LastRevenue;
        g.Do(Command.SetSectorPrice(g.Id, Sector, g.C.Market.SectorPriceMinPermille));
        g.Sim.Step();
        long cheapRevenue = sectorState.Entry(g.Id).LastRevenue;
        Assert.True(cheapRevenue < listRevenue);
        Assert.Equal(appeal, DisplayQueries.Appeal(g.C, g.State, sector, sectorState, g.Me, model));
    }

    [Fact]
    public void Maximum_price_permille_increases_revenue_for_the_same_share()
    {
        Game g = ReleasedMini(out ModelState model);
        SectorState sectorState = g.State.Sector(Sector);
        g.Do(Command.SetSectorPrice(g.Id, Sector, g.C.Market.SectorPriceMaxPermille));
        g.Sim.Step();
        long high = sectorState.Entry(g.Id).LastRevenue;
        g.Do(Command.SetSectorPrice(g.Id, Sector, MathI.Permille));
        g.Sim.Step();
        long list = sectorState.Entry(g.Id).LastRevenue;
        Assert.True(high > list);
    }

    [Fact]
    public void Zero_price_is_refused_at_config_minimum()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Refused(Command.SetSectorPrice(g.Id, Sector, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void Above_maximum_price_is_refused()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Refused(Command.SetSectorPrice(g.Id, Sector, g.C.Market.SectorPriceMaxPermille + 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Repricing_without_a_listing_is_refused()
    {
        Game g = Game.Solo();
        g.Refused(Command.SetSectorPrice(g.Id, Sector, 800), CommandError.InvalidArgument);
    }

    [Fact]
    public void Unknown_sector_id_is_refused()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Refused(Command.SetSectorPrice(g.Id, "moon-mining", 800), CommandError.UnknownId);
    }

    [Fact]
    public void Bankrupt_lab_cannot_reprice()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Me.Bankrupt = true;
        g.Refused(Command.SetSectorPrice(g.Id, Sector, 800), CommandError.LabBankrupt);
    }

    [Fact]
    public void Save_load_round_trip_keeps_sector_price()
    {
        Game g = ReleasedMini(out ModelState model);
        g.Do(Command.SetSectorPrice(g.Id, Sector, 750));
        int before = g.State.Sector(Sector).Entry(g.Id).PricePermille;
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(before, loaded.State.Sector(Sector).Entry(loaded.Id).PricePermille);
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 8);
    }

    [Theory]
    [MemberData(nameof(SimHardeningSupport.FixtureRows), MemberType = typeof(SimHardeningSupport))]
    public void Migrated_fixtures_default_sector_prices_to_list_permille(string file)
    {
        GameConfig c = TestData.Config();
        Simulation sim = SimHardeningSupport.LoadFixture(c, file);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        foreach (SectorState sector in sim.State.Sectors)
        {
            foreach (SectorEntry entry in sector.Entries)
                Assert.Equal(MathI.Permille, entry.PricePermille);
        }
        SimHardeningSupport.AssertMigratedFixtureSteps(c, file, 6);
    }

    [Fact]
    public void Schema_11_fixture_gains_price_fields_on_load()
    {
        var c = TestData.Config();
        Simulation sim = SimHardeningSupport.LoadFixture(c, "schema4-save.json");
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Contains(sim.State.Sectors, s => s.Entries.Count > 0);
        foreach (SectorState sector in sim.State.Sectors)
            foreach (SectorEntry entry in sector.Entries)
                Assert.Equal(MathI.Permille, entry.PricePermille);
    }

    [Fact]
    public void Expansion_appeal_boost_is_unchanged_when_price_drops()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, "robotics-industry");
        ExpansionTestSupport.BuildExpansionRoom(g, "robotics-arena");
        long boostBefore = ExpansionTestSupport.ExpansionBoost(g, model, "robotics-industry");
        g.Do(Command.SetSectorPrice(g.Id, "robotics-industry", g.C.Market.SectorPriceMinPermille));
        long boostAfter = ExpansionTestSupport.ExpansionBoost(g, model, "robotics-industry");
        Assert.Equal(boostBefore, boostAfter);
    }
}