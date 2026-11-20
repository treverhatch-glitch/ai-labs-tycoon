using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class BodiesExpansionHardeningTests
{
    const string Robotics = "robotics-industry";
    const string Household = "household-robots";
    const string Arena = "robotics-arena";

    [Fact]
    public void Main_path_robotics_arena_raises_appeal_and_breakdown_matches_total()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics, "Robo");
        SectorConfig sector = g.C.Sector(Robotics);
        SectorState sectorState = g.State.Sector(Robotics);
        long before = DisplayQueries.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long after = DisplayQueries.Appeal(g.C, g.State, sector, sectorState, g.Me, model);
        var parts = DisplayQueries.AppealParts(g.C, g.State, sector, sectorState, g.Me, model);
        Assert.True(parts.ExpansionBoost > 0);
        Assert.Equal(before + parts.ExpansionBoost, after);
        Assert.Equal(parts.Total, after);
    }

    [Fact]
    public void Household_robots_sector_uses_the_bodies_kit()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2030);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Household);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Assert.True(ExpansionTestSupport.ExpansionBoost(g, model, Household) > 0);
        Assert.Equal(g.C.Expansions.Bodies.AppealBonusPermille,
            ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, Household));
    }

    [Fact]
    public void Before_the_bodies_era_the_kit_bonus_is_zero_even_if_the_room_exists()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2028);
        g.Me.Rooms.Add(new RoomState { TypeId = Arena, X = 0, Y = 0, Width = 6, Height = 6, Id = g.State.TakeId() });
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, Robotics));
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, Household));
    }

    [Fact]
    public void After_bodies_begins_a_lab_without_arena_gets_no_sector_bonus()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics);
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, Robotics));
        Assert.Equal(0, ExpansionTestSupport.ExpansionBoost(g, model, Robotics));
    }

    [Fact]
    public void Arena_does_not_boost_unrelated_sectors()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 50_000, "game-ai");
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, "game-ai"));
    }

    [Fact]
    public void Zero_bonus_permille_in_config_never_scales_appeal()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029, tweak: c => c.Expansions.Bodies.AppealBonusPermille = 0);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, Robotics));
        Assert.Equal(0, ExpansionTestSupport.ExpansionBoost(g, model, Robotics));
    }

    [Fact]
    public void Large_configured_bonus_matches_scale_appeal_math()
    {
        const int big = 450;
        long core = 1_000_000;
        long scaled = ExpansionSystem.ScaleAppeal(core, big);
        Assert.Equal(MathI.MulPermille(core, MathI.Permille + big), scaled);
        Game g = ExpansionTestSupport.SoloAtYear(2029, tweak: c => c.Expansions.Bodies.AppealBonusPermille = big);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long boost = ExpansionTestSupport.ExpansionBoost(g, model, Robotics);
        long baseAppeal = DisplayQueries.Appeal(g.C, g.State, g.C.Sector(Robotics), g.State.Sector(Robotics), g.Me, model) - boost;
        Assert.Equal(ExpansionSystem.ScaleAppeal(baseAppeal, big) - baseAppeal, boost);
    }

    [Fact]
    public void Bankrupt_lab_still_gets_display_bonus_when_room_and_sector_match()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        g.Me.Bankrupt = true;
        Assert.True(ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, Robotics) > 0);
    }

    [Fact]
    public void Save_load_round_trip_preserves_expansion_appeal()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long boostBefore = ExpansionTestSupport.ExpansionBoost(g, model, Robotics);
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        ModelState theirs = loaded.Me.Model(model.Id);
        long boostAfter = ExpansionTestSupport.ExpansionBoost(loaded, theirs, Robotics);
        Assert.Equal(boostBefore, boostAfter);
        loaded.Autopilot(8);
        g.Autopilot(8);
        Assert.Equal(g.Sim.Digest(), loaded.Sim.Digest());
    }

    [Theory]
    [MemberData(nameof(SimHardeningSupport.FixtureRows), MemberType = typeof(SimHardeningSupport))]
    public void Migrated_fixture_steps_with_bodies_expansion_config_in_ledger_balance(string file)
    {
        GameConfig c = TestData.Config();
        SimHardeningSupport.AssertMigratedFixtureSteps(c, file, 8);
        Simulation sim = SimHardeningSupport.LoadFixture(c, file);
        sim.State.Tick = GameDate.TickOfYear(c.Calendar, 2030);
        foreach (LabState lab in sim.State.Labs)
            Assert.Equal(0, ExpansionSystem.AppealBonusPermille(c, sim.State, lab, Robotics));
    }

    [Fact]
    public void Market_week_revenue_rises_after_robotics_arena_goes_up()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics);
        long week1 = ExpansionTestSupport.SectorRevenueAfterWeek(g, Robotics);
        ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        long week2 = ExpansionTestSupport.SectorRevenueAfterWeek(g, Robotics);
        Assert.True(week2 >= week1);
        if (week1 > 0) Assert.True(week2 > week1, "appeal boost should pull more revenue when the pool is positive");
    }

    [Fact]
    public void Demolishing_arena_removes_expansion_boost()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2029);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 200_000, Robotics);
        int roomId = ExpansionTestSupport.BuildExpansionRoom(g, Arena);
        Assert.True(ExpansionTestSupport.ExpansionBoost(g, model, Robotics) > 0);
        g.Do(Command.DemolishRoom(g.Id, roomId));
        Assert.False(ExpansionSystem.HasRoom(g.Me, Arena));
        Assert.Equal(0, ExpansionTestSupport.ExpansionBoost(g, model, Robotics));
    }

    [Fact]
    public void Expansions_block_in_default_config_passes_validator()
    {
        Assert.Empty(ConfigValidator.Problems(TestData.Config()));
    }

    [Fact]
    public void Bodies_world_event_raises_robotics_pool_per_world_event_data()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2030, 5);
        WorldEventConfig ev = g.C.WorldEvent("four-thousand-robots");
        Assert.Equal(Robotics, ev.SectorId);
        Assert.True(ev.PoolPermille > MathI.Permille);
        g.Sim.Step();
        while (g.State.Tick < GameDate.TickOfYear(g.C.Calendar, 2030) + ev.Month * g.C.Calendar.WeeksPerMonth)
            g.Sim.Step();
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.WorldNews && e.Ref == ev.Id);
    }
}