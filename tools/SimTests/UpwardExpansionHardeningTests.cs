using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class UpwardExpansionHardeningTests
{
    const string MachineScience = "machine-science";
    const string SpaceCompute = "space-compute";
    const string Uplink = "orbital-uplink";
    const string Robotics = "robotics-industry";

    [Fact]
    public void Main_path_orbital_uplink_raises_machine_science_appeal()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, MachineScience, "Lab");
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        Assert.True(ExpansionTestSupport.ExpansionBoost(g, model, MachineScience) > 0);
        Assert.Equal(g.C.Expansions.Upward.AppealBonusPermille,
            ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, MachineScience));
    }

    [Fact]
    public void Space_compute_sector_uses_the_upward_kit()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2034);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, SpaceCompute);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        Assert.True(ExpansionTestSupport.ExpansionBoost(g, model, SpaceCompute) > 0);
    }

    [Fact]
    public void Year_before_upward_era_keeps_boost_at_zero_with_uplink_present()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2032);
        g.Me.Rooms.Add(new RoomState { TypeId = Uplink, X = 0, Y = 0, Width = 6, Height = 6, Id = g.State.TakeId() });
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, MachineScience));
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, SpaceCompute));
    }

    [Fact]
    public void At_upward_era_no_uplink_means_no_kit_bonus()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, MachineScience);
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, MachineScience));
    }

    [Fact]
    public void Uplink_does_not_apply_to_outside_sectors()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, Robotics);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        Assert.Equal(0, ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, Robotics));
    }

    [Fact]
    public void Zero_upward_bonus_permille_leaves_appeal_unchanged()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033, tweak: c => c.Expansions.Upward.AppealBonusPermille = 0);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, MachineScience);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        Assert.Equal(0, ExpansionTestSupport.ExpansionBoost(g, model, MachineScience));
    }

    [Fact]
    public void Maximum_style_bonus_permille_scales_via_mulpermille()
    {
        const int max = 800;
        Game g = ExpansionTestSupport.SoloAtYear(2033, tweak: c => c.Expansions.Upward.AppealBonusPermille = max);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, MachineScience);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        long boost = ExpansionTestSupport.ExpansionBoost(g, model, MachineScience);
        long full = DisplayQueries.Appeal(g.C, g.State, g.C.Sector(MachineScience), g.State.Sector(MachineScience), g.Me, model);
        long baseAppeal = full - boost;
        Assert.Equal(ExpansionSystem.ScaleAppeal(baseAppeal, max) - baseAppeal, boost);
    }

    [Fact]
    public void Bankrupt_player_with_uplink_still_reports_upward_kit_bonus()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        g.Me.Bankrupt = true;
        Assert.True(ExpansionSystem.AppealBonusPermille(g.C, g.State, g.Me, MachineScience) > 0);
    }

    [Fact]
    public void Save_load_round_trip_preserves_upward_boost_parts()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, MachineScience);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        long boostBefore = ExpansionTestSupport.ExpansionBoost(g, model, MachineScience);
        string save = g.Sim.Save();
        var loaded = new Game(Simulation.Load(g.C, save));
        ModelState theirs = loaded.Me.Model(model.Id);
        Assert.Equal(boostBefore, ExpansionTestSupport.ExpansionBoost(loaded, theirs, MachineScience));
        SimHardeningSupport.AssertSaveLoadContinueIdentically(g, 6);
    }

    [Theory]
    [MemberData(nameof(SimHardeningSupport.FixtureRows), MemberType = typeof(SimHardeningSupport))]
    public void Migrated_fixture_loads_and_steps_under_upward_config(string file)
    {
        GameConfig c = TestData.Config();
        SimHardeningSupport.AssertMigratedFixtureSteps(c, file, 4);
        Simulation sim = SimHardeningSupport.LoadFixture(c, file);
        sim.State.Tick = GameDate.TickOfYear(c.Calendar, 2033);
        sim.Step();
        foreach (LabState lab in sim.State.Labs)
            if (!lab.Bankrupt)
                Assert.True(ExpansionSystem.AppealBonusPermille(c, sim.State, lab, MachineScience) >= 0);
    }

    [Fact]
    public void Market_week_after_uplink_earns_at_least_as_much_for_released_model()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, MachineScience);
        long before = ExpansionTestSupport.SectorRevenueAfterWeek(g, MachineScience);
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        long after = ExpansionTestSupport.SectorRevenueAfterWeek(g, MachineScience);
        Assert.True(after >= before);
    }

    [Fact]
    public void Demolish_uplink_strips_machine_science_expansion_part()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2033);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 250_000, MachineScience);
        int roomId = ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        Assert.True(ExpansionTestSupport.ExpansionBoost(g, model, MachineScience) > 0);
        g.Do(Command.DemolishRoom(g.Id, roomId));
        Assert.Equal(0, ExpansionTestSupport.ExpansionBoost(g, model, MachineScience));
    }

    [Fact]
    public void Expansion_and_crown_both_contribute_in_breakdown_on_space_compute()
    {
        Game g = ExpansionTestSupport.SoloAtYear(2035);
        ModelState model = ExpansionTestSupport.AddReleasedModel(g, 300_000, SpaceCompute, "Crown");
        g.State.Sector(SpaceCompute).ChampionLabId = g.Id;
        ExpansionTestSupport.BuildExpansionRoom(g, Uplink);
        var parts = DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(SpaceCompute), g.State.Sector(SpaceCompute), g.Me, model);
        Assert.True(parts.ChampionBoost > 0);
        Assert.True(parts.ExpansionBoost > 0);
        Assert.Equal(parts.Total, DisplayQueries.Appeal(g.C, g.State, g.C.Sector(SpaceCompute), g.State.Sector(SpaceCompute), g.Me, model));
    }
}