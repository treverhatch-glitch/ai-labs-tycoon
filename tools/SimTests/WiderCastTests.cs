using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Metta, Macrosoft and Palantear arrive with the eras like Goggle and DeepSneak.</summary>
public class WiderCastTests
{
    static int FirstWeekOfYear(GameConfig c, int year) => (year - c.Calendar.StartYear) * c.Calendar.WeeksPerYear;

    static LabState LabByRival(Game g, string rivalId) => g.State.Labs.Single(l => l.RivalId == rivalId);

    [Fact]
    public void A_new_game_queues_the_late_cast_in_join_year_order_in_the_config()
    {
        Game g = Game.New();
        Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray());
        Assert.DoesNotContain(g.State.Labs, l => l.RivalId == "metta");
    }

    [Fact]
    public void Metta_arrives_in_the_first_week_of_2007_with_its_start_cash_and_standing()
    {
        Game g = Game.New();
        GameConfig c = g.C;
        int tick = FirstWeekOfYear(c, 2007);
        g.Sim.StepMany(tick);
        Assert.DoesNotContain(g.State.Labs, l => l.RivalId == "metta");
        int labs = g.State.Labs.Count;
        g.Sim.Step();
        LabState metta = LabByRival(g, "metta");
        Assert.Equal(labs + 1, g.State.Labs.Count);
        Assert.Equal(c.Rival("metta").StartCash, metta.Ledger.StartCash);
        Assert.Equal("Facehook", metta.FamilyName);
        Assert.Equal(500, g.State.Vendor.Standing(metta.Id).Standing);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RivalJoined && e.Ref == "metta" && e.Tick == tick);
    }

    [Fact]
    public void Macrosoft_arrives_in_2012_and_builds_as_an_owner_not_a_renter()
    {
        Game g = Game.New();
        int joinTick = FirstWeekOfYear(g.C, 2012);
        g.Sim.StepMany(joinTick + 1);
        LabState macro = LabByRival(g, "macrosoft");
        Assert.True(macro.PlanStartTick >= joinTick);
        Assert.True(g.C.Rival("macrosoft").Plan.OwnGpus);
        Assert.Equal("DeskPilot", macro.FamilyName);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RivalJoined && e.Ref == "macrosoft");
    }

    [Fact]
    public void Palantear_arrives_in_2017_with_a_release_gate_and_publishes_safety_research()
    {
        Game g = Game.New();
        g.Sim.StepMany(FirstWeekOfYear(g.C, 2017) + 1);
        PlanConfig plan = g.C.Rival("palantear").Plan;
        LabState pal = LabByRival(g, "palantear");
        Assert.Equal(280, plan.MinReleaseAlignment);
        Assert.True(plan.PublishesSafetyResearch);
        Assert.Equal("Gotham", pal.FamilyName);
    }

    [Fact]
    public void Goggle_still_arrives_in_2010_even_with_three_labs_before_it_in_the_config()
    {
        Game g = Game.New();
        g.Sim.StepMany(FirstWeekOfYear(g.C, 2010) + 1);
        Assert.NotNull(LabByRival(g, "goggle"));
        Assert.NotNull(LabByRival(g, "metta"));
        Assert.NotNull(LabByRival(g, "goggle"));
        Assert.DoesNotContain(g.State.Labs, l => l.RivalId == "macrosoft");
        Assert.DoesNotContain(g.State.Labs, l => l.RivalId == "palantear");
    }

    [Fact]
    public void DeepSneak_is_still_the_last_scheduled_arrival_in_2023()
    {
        Game g = Game.New();
        g.Sim.StepMany(FirstWeekOfYear(g.C, 2023) + 1);
        LabState sneak = LabByRival(g, "deepsneak");
        Assert.Equal("china", sneak.HomeBlocId);
        Assert.Empty(g.State.PendingRivals);
        Assert.Equal(g.State.Labs.Count, g.State.Vendor.Standings.Count);
    }

    [Fact]
    public void A_late_arrival_keeps_the_techs_of_eras_before_its_year()
    {
        Game g = Game.New();
        g.Sim.StepMany(FirstWeekOfYear(g.C, 2012) + 1);
        LabState macro = LabByRival(g, "macrosoft");
        Assert.True(macro.Techs.Count > 8);
        Assert.All(macro.Techs, t => Assert.True(EraSystem.TechOpen(g.C, g.C.Tech(t), g.Tick)));
    }

    [Fact]
    public void Metta_plan_stays_rent_heavy_with_no_chip_programme()
    {
        PlanConfig plan = TestData.Config().Rival("metta").Plan;
        Assert.False(plan.OwnGpus);
        Assert.False(plan.BuysListedChips);
        Assert.Equal(0, plan.ChipProgramYear);
        Assert.False(plan.UsesCloudDeals);
    }

    [Fact]
    public void Macrosoft_plan_owns_metal_and_may_buy_listings()
    {
        PlanConfig plan = TestData.Config().Rival("macrosoft").Plan;
        Assert.True(plan.OwnGpus);
        Assert.True(plan.BuysListedChips);
        Assert.True(plan.SiteAppetitePermille >= 800);
    }

    [Fact]
    public void Palantear_stays_out_of_consumer_fluff_sectors()
    {
        PlanConfig plan = TestData.Config().Rival("palantear").Plan;
        Assert.Contains("government-services", plan.Sectors);
        Assert.DoesNotContain(plan.Sectors, s => s == "companionship" || s == "music-audio");
    }

    [Fact]
    public void The_successor_plan_still_never_spawns_when_other_arrivals_are_present()
    {
        Game g = Game.New();
        Assert.DoesNotContain(g.State.PendingRivals, id => id == "successor");
        g.Years(30);
        Assert.DoesNotContain(g.State.Labs, l => l.RivalId == "successor");
    }

    [Fact]
    public void Skipping_a_custom_rival_list_omits_the_wider_cast_too()
    {
        var options = new NewGameOptions { Seed = 3, RivalIds = new List<string> { "openaye", "xay" } };
        Game g = new Game(Simulation.NewGame(TestData.Config(), options));
        Assert.Equal(new[] { "openaye", "xay" }, g.State.Labs.Select(l => l.RivalId).Where(id => id != "").ToArray());
        Assert.Empty(g.State.PendingRivals);
    }
}