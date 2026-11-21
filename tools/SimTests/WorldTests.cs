using System;
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

/// <summary>The world map: land, halls, colo, power, heatwaves, export rules and what governments make of a big lab.</summary>
public class WorldTests
{
    /// <summary>
    /// One lab with plenty of cash, no town votes and attention that cannot move the vote: tests that are not about
    /// those things should not depend on a dice roll.
    /// </summary>
    public static Game Rich(string eraId = null, Action<GameConfig> tweak = null, long seed = 1)
    {
        var options = new NewGameOptions { Seed = seed, RivalIds = new List<string>(), StartEraId = eraId };
        GameConfig c = TestData.Config(cfg =>
        {
            cfg.PlayerStart.Cash = 500000000;
            cfg.World.Attention.DominanceFromPpm = MathI.Ppm;
            cfg.World.Halls.TownVoteAttentionDivisor = 1000000;
            foreach (SiteConfig site in cfg.World.Sites) site.TownVotePermille = 0;
            if (tweak != null) tweak(cfg);
        });
        return new Game(Simulation.NewGame(c, options));
    }

    /// <summary>The player and one rival, both with plenty of cash.</summary>
    static Game Duo(Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = 1, RivalIds = new List<string> { "openaye" } };
        GameConfig c = TestData.Config(cfg =>
        {
            cfg.PlayerStart.Cash = 50000000;
            cfg.Rival("openaye").StartCash = 50000000;
            cfg.World.Halls.TownVoteAttentionDivisor = 1000000;
            foreach (SiteConfig site in cfg.World.Sites) site.TownVotePermille = 0;
            if (tweak != null) tweak(cfg);
        });
        return new Game(Simulation.NewGame(c, options));
    }

    static Action<GameConfig> Startable(string eraId)
    {
        return c =>
        {
            EraConfig era = c.Eras.Single(e => e.Id == eraId);
            era.Startable = true;
            era.StartCashPermille = 1000;
        };
    }

    public static Holding Hall(Game g, string siteId, int kw, LabState lab = null)
    {
        lab = lab ?? g.Me;
        int id = g.Do(Command.BuyLand(lab.Id, siteId)).CreatedId;
        g.Do(Command.BuildHall(lab.Id, id, kw));
        Holding holding = lab.Holding(id);
        g.Until(() => holding.HallKw >= kw, 200, "the hall at " + siteId);
        return holding;
    }

    static long Power(LabState lab) { return -lab.Ledger.Lifetime.Power; }

    // ---- land --------------------------------------------------------------------------------------------------

    [Fact]
    public void Land_is_refused_at_a_closed_or_unknown_site_a_full_site_a_second_plot_and_in_a_frozen_bloc()
    {
        Game g = Game.New(1, c => c.World.Site("us-oregon").Plots = 1);
        g.Refused(Command.BuyLand(g.Id, "us-tennessee"), CommandError.SiteClosed);
        g.Refused(Command.BuyLand(g.Id, "nowhere"), CommandError.UnknownId);

        LabState rival = g.Rival("openaye");
        g.Do(Command.BuyLand(rival.Id, "us-oregon"));
        g.Refused(Command.BuyLand(g.Id, "us-oregon"), CommandError.NoFreePlot);

        g.Me.Bloc("us").Level = BlocStanding.Freeze;
        g.Refused(Command.BuyLand(g.Id, "us-virginia"), CommandError.PermitFrozen);
        g.Me.Bloc("us").Level = 0;

        long before = g.Me.Cash;
        g.Do(Command.BuyLand(g.Id, "us-virginia"));
        Assert.Equal(before - g.C.World.Site("us-virginia").LandPrice, g.Me.Cash);
        g.Refused(Command.BuyLand(g.Id, "us-virginia"), CommandError.NoFreePlot);
    }

    [Fact]
    public void Land_cannot_be_bought_without_the_cash_and_a_refusal_changes_nothing()
    {
        Game g = Game.Solo(1, c => c.PlayerStart.Cash = 1000);
        g.Refused(Command.BuyLand(g.Id, "us-oregon"), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Selling_land_refunds_part_of_the_price_and_needs_an_empty_finished_plot()
    {
        Game g = Rich();
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Do(Command.BuildHall(g.Id, land, 100));
        g.Refused(Command.SellLand(g.Id, land), CommandError.BuildRunning);
        Holding holding = g.Me.Holding(land);
        g.Until(() => holding.HallKw == 100, 100, "the hall");

        g.Do(Command.BuyGpus(g.Id, 4, land));
        g.Refused(Command.SellLand(g.Id, land), CommandError.HoldingNotEmpty);
        g.Do(Command.SellGpus(g.Id, 4, land));

        long before = g.Me.Cash;
        g.Do(Command.SellLand(g.Id, land));
        Assert.Equal(before + g.C.World.Site("us-oregon").LandPrice * g.C.World.Halls.LandRefundPermille / 1000, g.Me.Cash);
        Assert.Null(g.Me.Holding(land));
        g.Refused(Command.SellLand(g.Id, land), CommandError.UnknownId);
        Assert.Equal(g.Me.Ledger.StartCash + g.Me.Ledger.Lifetime.Net, g.Me.Cash);
    }

    // ---- the grid ----------------------------------------------------------------------------------------------

    [Fact]
    public void Two_labs_share_one_grid_and_a_power_deal_lets_one_go_past_it()
    {
        Game g = Duo(c => c.World.Site("us-oregon").GridKw = 800);
        LabState rival = g.Rival("openaye");

        int mine = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Do(Command.BuildHall(g.Id, mine, 200));
        int theirs = g.Do(Command.BuyLand(rival.Id, "us-oregon")).CreatedId;
        g.Refused(Command.BuildHall(rival.Id, theirs, 700), CommandError.GridFull);
        g.Do(Command.BuildHall(rival.Id, theirs, 500));
        Assert.Equal(700, WorldQueries.SiteGridUsed(g.State, "us-oregon"));

        Holding hall = g.Me.Holding(mine);
        g.Until(() => hall.HallKw == 200, 100, "my hall");
        g.Refused(Command.BuildHall(g.Id, mine, 200), CommandError.GridFull);

        g.Do(Command.AddPowerDeal(g.Id, mine, "grid-upgrade"));
        g.Refused(Command.AddPowerDeal(g.Id, mine, "grid-upgrade"), CommandError.BuildRunning);
        g.Until(() => hall.PrivateKw == 500, 50, "the grid upgrade");
        g.Do(Command.BuildHall(g.Id, mine, 200));
        g.Until(() => hall.HallKw == 400, 100, "the second extension");

        Assert.True(g.Me.Holding(mine).HallKw + g.Rival("openaye").Holding(theirs).HallKw > g.C.World.Site("us-oregon").GridKw);
        Assert.Equal(0, WorldQueries.GridDrawKw(hall));
    }

    [Fact]
    public void A_power_deal_must_be_on_offer_and_the_hall_must_be_one_the_lab_owns()
    {
        Game g = Rich();
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Refused(Command.AddPowerDeal(g.Id, land, "reactor-restart"), CommandError.EraLocked);
        g.Refused(Command.AddPowerDeal(g.Id, land, "no-such-deal"), CommandError.UnknownId);
        g.Refused(Command.AddPowerDeal(g.Id, 9999, "grid-upgrade"), CommandError.UnknownId);
    }

    // ---- halls -------------------------------------------------------------------------------------------------

    [Fact]
    public void Infrastructure_staff_shorten_construction_but_never_below_the_floor()
    {
        Game plain = Rich();
        int a = plain.Do(Command.BuyLand(plain.Id, "us-oregon")).CreatedId;
        plain.Do(Command.BuildHall(plain.Id, a, 400));
        Assert.Equal(16 + 400 / 250, plain.Me.Holding(a).HallDoneTick - plain.Tick);

        Game staffed = Rich();
        staffed.Build("ops-centre", 10, 8);
        staffed.Hire("infra", 20);
        int b = staffed.Do(Command.BuyLand(staffed.Id, "us-oregon")).CreatedId;
        staffed.Do(Command.BuildHall(staffed.Id, b, 400));
        int weeks = staffed.Me.Holding(b).HallDoneTick - staffed.Tick;
        Assert.Equal(WorldQueries.BuildWeeks(staffed.C, staffed.Me, 400), weeks);
        Assert.True(weeks < 16 + 400 / 250);
        Assert.True(weeks >= staffed.C.World.Halls.MinBuildWeeks);
        Assert.Equal(6, weeks);
    }

    [Fact]
    public void A_town_vote_adds_weeks_and_the_same_seed_gives_the_same_delay()
    {
        Action<GameConfig> always = c =>
        {
            c.World.Halls.TownVoteCapPermille = 1000;
            c.World.Site("us-oregon").TownVotePermille = 1000;
        };
        Game a = Rich(null, always, 5);
        Game b = Rich(null, always, 5);
        int ha = a.Do(Command.BuyLand(a.Id, "us-oregon")).CreatedId;
        int hb = b.Do(Command.BuyLand(b.Id, "us-oregon")).CreatedId;
        a.Do(Command.BuildHall(a.Id, ha, 100));
        b.Do(Command.BuildHall(b.Id, hb, 100));
        Assert.Equal(16 + 8, a.Me.Holding(ha).HallDoneTick - a.Tick);
        Assert.Equal(a.Me.Holding(ha).HallDoneTick, b.Me.Holding(hb).HallDoneTick);
        Assert.Contains(a.State.Events, e => e.Kind == EventKind.TownVoteDelayed && e.Amount == 8);
        Assert.Equal(a.Sim.Digest(), b.Sim.Digest());

        Game none = Rich();
        int hn = none.Do(Command.BuyLand(none.Id, "us-oregon")).CreatedId;
        none.Do(Command.BuildHall(none.Id, hn, 100));
        Assert.Equal(16, none.Me.Holding(hn).HallDoneTick - none.Tick);
        Assert.DoesNotContain(none.State.Events, e => e.Kind == EventKind.TownVoteDelayed);
    }

    [Fact]
    public void A_hall_is_refused_when_too_small_already_building_or_unaffordable_and_pays_when_it_starts()
    {
        Game g = Rich(null, c => c.PlayerStart.Cash = 300000);
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Refused(Command.BuildHall(g.Id, land, 10), CommandError.TooSmall);
        g.Refused(Command.BuildHall(g.Id, land, 1000), CommandError.NotEnoughCash);
        long before = g.Me.Cash;
        g.Do(Command.BuildHall(g.Id, land, 100));
        Assert.Equal(before - 100 * g.C.World.Halls.CostPerKw, g.Me.Cash);
        Assert.Equal(-100 * g.C.World.Halls.CostPerKw, g.Me.Ledger.Lifetime.SiteBuild);
        g.Refused(Command.BuildHall(g.Id, land, 100), CommandError.BuildRunning);
        Assert.Equal(g.Me.Ledger.StartCash + g.Me.Ledger.Lifetime.Net, g.Me.Cash);
    }

    [Fact]
    public void Hardware_goes_only_where_there_is_a_hall_with_the_power_free()
    {
        Game g = Rich();
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Refused(Command.BuyGpus(g.Id, 1, land), CommandError.NotEnoughPower);
        g.Refused(Command.BuyGpus(g.Id, 1, 9999), CommandError.UnknownId);
        Holding hall = Hall(g, "us-virginia", 50);
        g.Refused(Command.BuyGpus(g.Id, 51, hall.Id), CommandError.NotEnoughPower);
        g.Do(Command.BuyGpus(g.Id, 50, hall.Id));
        Assert.Equal(0, WorldQueries.FreeKw(g.C, g.State, g.Me, hall));
        g.Refused(Command.BuyGpus(g.Id, 1, hall.Id), CommandError.NotEnoughPower);
        Assert.Equal(50, LabQueries.OwnedUnits(g.Me, hall.Id));
        Assert.Equal(0, LabQueries.HqGpus(g.Me));
    }

    // ---- colo --------------------------------------------------------------------------------------------------

    [Fact]
    public void Colo_is_leased_by_the_kilowatt_grown_shrunk_and_ended_and_its_markup_is_on_the_bill()
    {
        Game g = Rich();
        Assert.Equal(0, WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me));
        g.Refused(Command.LeaseColo(g.Id, "us-oregon", 100000), CommandError.InvalidArgument);
        g.Refused(Command.LeaseColo(g.Id, "us-oregon", 0), CommandError.InvalidArgument);
        g.Refused(Command.LeaseColo(g.Id, "us-tennessee", 100), CommandError.SiteClosed);

        int colo = g.Do(Command.LeaseColo(g.Id, "us-oregon", 100)).CreatedId;
        Assert.Equal(HoldingKind.Colo, g.Me.Holding(colo).Kind);
        g.Do(Command.BuyGpus(g.Id, 100, colo));
        PowerBill bill = WorldQueries.BillFor(g.C, g.State, g.Me, g.Me.Holding(colo));
        Assert.Equal(100 * g.C.World.Site("us-oregon").PowerPricePerKwWeek, bill.GridCost);
        Assert.Equal(100 * g.C.World.Colo.MarkupPerKwWeek, bill.ColoMarkup);
        Assert.Equal(bill.GridCost + bill.PrivateCost + bill.Cooling + bill.ColoMarkup, bill.Total);

        g.Refused(Command.LeaseColo(g.Id, "us-oregon", 50), CommandError.NotEnoughPower);
        g.Do(Command.LeaseColo(g.Id, "us-oregon", 200));
        Assert.Equal(200, g.Me.Holding(colo).HallKw);
        g.Refused(Command.LeaseColo(g.Id, "us-oregon", 0), CommandError.NotEnoughPower);
        g.Do(Command.SellGpus(g.Id, 100, colo));
        g.Do(Command.LeaseColo(g.Id, "us-oregon", 0));
        Assert.Null(g.Me.Holding(colo));
        Assert.Equal(0, WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me));
    }

    [Fact]
    public void Colo_leases_come_out_of_the_same_grid_as_halls()
    {
        Game g = Rich(null, c =>
        {
            c.World.Site("us-oregon").GridKw = 300;
            c.World.Colo.MaxKwPerLabPerSite = 1000;
        });
        g.Do(Command.LeaseColo(g.Id, "us-oregon", 200));
        g.Refused(Command.LeaseColo(g.Id, "us-oregon", 301), CommandError.GridFull);
        int land = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Refused(Command.BuildHall(g.Id, land, 150), CommandError.GridFull);
        g.Do(Command.BuildHall(g.Id, land, 100));
    }

    // ---- moving hardware ---------------------------------------------------------------------------------------

    [Fact]
    public void Hardware_moves_between_places_for_a_fee_and_keeps_its_generation()
    {
        Game g = Rich();
        g.Build("server-closet", 4, 3);
        g.Do(Command.BuyGpus(g.Id, 10));
        HardwareBatch batch = g.Me.Fleet.Single();
        Holding hall = Hall(g, "us-oregon", 100);

        g.Refused(Command.MoveHardware(g.Id, batch.Id, hall.Id, 11), CommandError.InvalidArgument);
        g.Refused(Command.MoveHardware(g.Id, batch.Id, HardwareBatch.Hq, 1), CommandError.InvalidArgument);
        g.Refused(Command.MoveHardware(g.Id, batch.Id, 9999, 1), CommandError.UnknownId);
        g.Refused(Command.MoveHardware(g.Id, 9999, hall.Id, 1), CommandError.UnknownId);

        long cash = g.Me.Cash;
        g.Do(Command.MoveHardware(g.Id, batch.Id, hall.Id, 4));
        Assert.Equal(cash - 4 * g.C.Compute.MoveCostPerUnit, g.Me.Cash);
        Assert.Equal(6, LabQueries.HqGpus(g.Me));
        Assert.Equal(4, LabQueries.OwnedUnits(g.Me, hall.Id));
        Assert.All(g.Me.Fleet, b => Assert.Equal("voodude", b.GpuGenId));

        g.Do(Command.MoveHardware(g.Id, g.Me.Fleet.Single(b => b.HoldingId == hall.Id).Id, HardwareBatch.Hq, 4));
        Assert.Equal(10, LabQueries.HqGpus(g.Me));
        Assert.Single(g.Me.Fleet);
        Assert.Equal(g.Me.Ledger.StartCash + g.Me.Ledger.Lifetime.Net, g.Me.Cash);
    }

    // ---- power, cooling and the payback ------------------------------------------------------------------------

    [Fact]
    public void The_power_bill_is_the_sum_of_its_parts_and_is_what_the_ledger_pays_each_week()
    {
        Game g = Rich("punchline");
        Holding hall = Hall(g, "us-texas", 100);
        g.Do(Command.BuyGpus(g.Id, 100, hall.Id));

        PowerBill bill = WorldQueries.BillFor(g.C, g.State, g.Me, hall);
        Assert.Equal(100, bill.InstalledKw);
        Assert.Equal(100 * 16, bill.GridCost);
        Assert.Equal(0, bill.PrivateCost);
        Assert.Equal(1600 * 300 / 1000, bill.Cooling);
        Assert.Equal(1600 + 480, bill.Total);
        Assert.Equal(bill.Total, WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me));

        long before = Power(g.Me);
        g.Weeks(1);
        Assert.Equal(bill.Total, Power(g.Me) - before);
    }

    [Fact]
    public void Cooling_tech_shrinks_only_the_part_of_the_site_factor_above_1000()
    {
        Game g = Rich("punchline");
        Holding texas = Hall(g, "us-texas", 100);
        g.Do(Command.BuyGpus(g.Id, 100, texas.Id));
        PowerBill hot = WorldQueries.BillFor(g.C, g.State, g.Me, texas);

        Holding iceland = Hall(g, "nordics-iceland", 100);
        g.Do(Command.BuyGpus(g.Id, 100, iceland.Id));
        PowerBill cool = WorldQueries.BillFor(g.C, g.State, g.Me, iceland);
        Assert.Equal(100 * 8 * 850 / 1000, cool.GridCost + cool.Cooling);

        g.Me.Techs.Add("liquid-cooling");
        Assert.Equal(1000 + 300 * (1000 - 300) / 1000, WorldQueries.EffectiveCooling(g.C, g.Me, g.C.World.Site("us-texas")));
        Assert.Equal(850, WorldQueries.EffectiveCooling(g.C, g.Me, g.C.World.Site("nordics-iceland")));
        Assert.Equal(1000, WorldQueries.EffectiveCooling(g.C, g.Me, g.C.World.Site("us-oregon")));

        PowerBill hotNow = WorldQueries.BillFor(g.C, g.State, g.Me, texas);
        Assert.Equal(hot.GridCost, hotNow.GridCost);
        Assert.True(hotNow.Cooling < hot.Cooling);
        Assert.Equal(1600 * 210 / 1000, hotNow.Cooling);
        PowerBill coolNow = WorldQueries.BillFor(g.C, g.State, g.Me, iceland);
        Assert.Equal(cool.Total, coolNow.Total);
    }

    [Fact]
    public void A_hall_at_a_cheap_site_pays_for_itself_in_about_a_year()
    {
        Game g = Rich();
        g.Build("server-closet", 10, 5);
        g.Do(Command.BuyGpus(g.Id, 100));
        long atHome = LabQueries.WeeklyGpuUpkeep(g.C, g.Me);
        g.Do(Command.SellGpus(g.Id, 100));
        Assert.Equal(0, LabQueries.WeeklyGpuUpkeep(g.C, g.Me));

        Holding hall = Hall(g, "us-oregon", 100);
        long invested = g.C.World.Site("us-oregon").LandPrice + 100 * g.C.World.Halls.CostPerKw;
        g.Do(Command.BuyGpus(g.Id, 100, hall.Id));
        long atTheSite = WorldQueries.WeeklyPowerBill(g.C, g.State, g.Me) + WorldQueries.WeeklyHallUpkeep(g.C, g.Me);

        long weeks = invested / (atHome - atTheSite);
        Assert.InRange(weeks, 50, 56);
    }

    // ---- heatwaves ---------------------------------------------------------------------------------------------

    [Fact]
    public void A_heatwave_throttles_compute_at_that_site_only_and_cooling_tech_halves_it()
    {
        Game g = Rich();
        g.Build("server-closet", 4, 3);
        g.Do(Command.BuyGpus(g.Id, 20));
        Holding oregon = Hall(g, "us-oregon", 100);
        g.Do(Command.BuyGpus(g.Id, 100, oregon.Id));
        Holding virginia = Hall(g, "us-virginia", 50);
        g.Do(Command.BuyGpus(g.Id, 50, virginia.Id));

        long normal = LabQueries.WeeklyCompute(g.C, g.State, g.Me);
        Assert.Equal(170 * 10, normal);
        g.State.Site("us-oregon").HeatwaveUntilTick = g.Tick + 4;
        Assert.Equal(normal - 100 * 10 * 400 / 1000, LabQueries.WeeklyCompute(g.C, g.State, g.Me));

        g.Me.Techs.Add("liquid-cooling");
        Assert.Equal(normal - 100 * 10 * 200 / 1000, LabQueries.WeeklyCompute(g.C, g.State, g.Me));
        g.Weeks(4);
        Assert.False(WorldQueries.InHeatwave(g.State, "us-oregon"));
        Assert.Equal(normal, LabQueries.WeeklyCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void Summer_brings_heatwaves_to_the_sites_a_lab_uses_and_to_no_others()
    {
        Game g = Rich(null, c =>
        {
            c.World.Site("us-oregon").SummerHeatwavePermille = 1000;
            c.World.Site("us-virginia").SummerHeatwavePermille = 1000;
            c.World.Heatwave.RollDivisor = 1;
        });
        Hall(g, "us-oregon", 50);
        g.Until(() => g.State.Events.Any(e => e.Kind == EventKind.Heatwave), 60, "a heatwave");
        Assert.True(WorldQueries.InHeatwave(g.State, "us-oregon"));
        Assert.False(WorldQueries.InHeatwave(g.State, "us-virginia"));
        GameEvent e0 = g.State.Events.First(e => e.Kind == EventKind.Heatwave);
        Assert.Equal("us-oregon", e0.Ref);
        Assert.Equal(g.C.World.Heatwave.Weeks, e0.Amount);
        int months = GameDate.Month(g.C.Calendar, g.Tick);
        Assert.InRange(months, g.C.World.Heatwave.FromMonth, g.C.World.Heatwave.ToMonth);
        g.Weeks(g.C.World.Heatwave.Weeks);
        Assert.True(g.State.Site("us-oregon").HeatwaveUntilTick <= g.Tick || g.State.Events.Count(e => e.Kind == EventKind.Heatwave) > 1);
    }

    // ---- latency -----------------------------------------------------------------------------------------------

    [Fact]
    public void A_far_site_slows_serving_customers_but_not_training()
    {
        Game g = Rich("punchline");
        Holding hall = Hall(g, "nordics-iceland", 100);
        g.Do(Command.BuyGpus(g.Id, 100, hall.Id));
        Assert.Equal(100 * 60, LabQueries.WeeklyCompute(g.C, g.State, g.Me));
        Assert.Equal(100 * 60 * (1000 - 150) / 1000, LabQueries.WeeklyServingBase(g.C, g.State, g.Me));
    }

    // ---- export rules ------------------------------------------------------------------------------------------

    [Fact]
    public void Export_rules_stop_a_home_lab_buying_or_moving_top_tier_hardware_into_a_rival_bloc()
    {
        Game g = Rich("harnesses", Startable("harnesses"));
        Holding oregon = Hall(g, "us-oregon", 100);
        Holding china = Hall(g, "china-guizhou", 100);
        Holding gulf = Hall(g, "gulf-desert", 100);

        g.Refused(Command.BuyGpus(g.Id, 1, china.Id), CommandError.ExportBlocked);
        g.Refused(Command.BuyGpus(g.Id, 1, gulf.Id), CommandError.ExportBlocked);
        g.Do(Command.BuyGpus(g.Id, 2, oregon.Id));
        g.Refused(Command.MoveHardware(g.Id, g.Me.Fleet.Single().Id, china.Id, 1), CommandError.ExportBlocked);
        g.Refused(Command.MoveHardware(g.Id, g.Me.Fleet.Single().Id, gulf.Id, 1), CommandError.ExportBlocked);
    }

    [Fact]
    public void A_lab_based_in_china_may_buy_there_and_hardware_already_placed_keeps_running()
    {
        Game g = Rich("harnesses", Startable("harnesses"));
        Holding china = Hall(g, "china-guizhou", 100);
        g.Me.HomeBlocId = "china";
        g.Do(Command.BuyGpus(g.Id, 5, china.Id));
        long compute = LabQueries.WeeklyCompute(g.C, g.State, g.Me);
        Assert.True(compute > 0);

        g.Me.HomeBlocId = "us";
        g.Refused(Command.BuyGpus(g.Id, 1, china.Id), CommandError.ExportBlocked);
        Assert.Equal(compute, LabQueries.WeeklyCompute(g.C, g.State, g.Me));
        g.Weeks(4);
        Assert.Equal(5, LabQueries.OwnedUnits(g.Me, china.Id));
        Assert.Equal(compute, LabQueries.WeeklyCompute(g.C, g.State, g.Me));
    }

    [Fact]
    public void An_older_generation_is_not_blocked_by_a_rule_that_is_not_yet_in_force()
    {
        Game g = Rich("punchline");
        Assert.DoesNotContain(g.C.World.ExportRules, r => r.FromYear <= g.Sim.Year);
        Holding china = Hall(g, "china-guizhou", 100);
        g.Do(Command.BuyGpus(g.Id, 5, china.Id));
        Assert.True(LabQueries.ExportAllowed(g.C, g.State, g.Me, 6, "gulf"));
    }

    // ---- government attention ----------------------------------------------------------------------------------

    static Action<GameConfig> NoticeAt(int kw)
    {
        return c =>
        {
            c.World.Attention.NoticeKwByYear = new List<NoticeKwConfig> { new NoticeKwConfig { Year = 1995, Kw = kw } };
        };
    }

    [Fact]
    public void A_steady_footprint_settles_attention_near_ten_months_of_gain()
    {
        Game g = Rich(null, NoticeAt(400));
        Holding hall = Hall(g, "us-oregon", 200);
        g.Do(Command.BuyGpus(g.Id, 200, hall.Id));

        AttentionGain gain = WorldQueries.GainFor(g.C, g.State, g.Me, "us", 0);
        Assert.Equal(200, gain.FootprintKw);
        Assert.Equal(50, gain.Size);
        Assert.Equal(50, gain.Total);

        g.Years(4);
        int attention = g.Me.Bloc("us").Attention;
        Assert.InRange(attention, 500 * 95 / 100, 500 * 105 / 100);
        Assert.Equal(BlocStanding.Letter, g.Me.Bloc("us").Level);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.AttentionRose));
        Assert.Equal(0, g.Me.Bloc("eu").Attention);
    }

    [Fact]
    public void Policy_staff_take_a_share_off_the_gain_and_the_parts_add_up()
    {
        Game g = Rich(null, NoticeAt(400));
        Holding hall = Hall(g, "us-oregon", 200);
        g.Do(Command.BuyGpus(g.Id, 200, hall.Id));
        g.Me.IncidentsThisMonth = 2;

        AttentionGain plain = WorldQueries.GainFor(g.C, g.State, g.Me, "us", 0);
        Assert.Equal(50 + 2 * g.C.World.Attention.IncidentGain, plain.Total);
        Assert.Equal(0, plain.PolicySaving);
        Assert.Equal(plain.FromFootprint + plain.FromIncidents + plain.FromDominance + plain.PolicySaving, plain.Total);

        g.Build("policy-office", 5, 5);
        g.Hire("policy", 5);
        AttentionGain staffed = WorldQueries.GainFor(g.C, g.State, g.Me, "us", 0);
        Assert.True(staffed.PolicySaving < 0);
        Assert.Equal((50 + 40) * (1000 - 200) / 1000, staffed.Total);
        Assert.Equal(plain.FromFootprint, staffed.FromFootprint);

        Assert.Equal(0, WorldQueries.GainFor(g.C, g.State, g.Me, "uk", 0).FromIncidents);
    }

    [Fact]
    public void Dominating_the_market_raises_attention_in_the_home_bloc_only()
    {
        Game g = Rich(null, c => c.World.Attention.DominanceFromPpm = 300000);
        AttentionGain gain = WorldQueries.GainFor(g.C, g.State, g.Me, "us", 800000);
        Assert.Equal((800000 - 300000) / g.C.World.Attention.DominanceDivisor, gain.FromDominance);
        Assert.Equal(0, WorldQueries.GainFor(g.C, g.State, g.Me, "us", 200000).FromDominance);
        Assert.Equal(0, WorldQueries.GainFor(g.C, g.State, g.Me, "eu", 800000).FromDominance);
    }

    [Fact]
    public void Each_step_of_scrutiny_fires_once_on_the_way_up_and_a_freeze_blocks_buying_until_it_lifts()
    {
        Game g = Rich(null, NoticeAt(10));
        Holding hall = Hall(g, "us-oregon", 100);
        g.Do(Command.BuyGpus(g.Id, 100, hall.Id));

        g.Until(() => g.Me.Bloc("us").Level == BlocStanding.Freeze, 48 * 5, "a freeze");
        var levels = g.State.Events.Where(e => e.Kind == EventKind.AttentionRose).Select(e => (int)e.SubjectId).ToList();
        Assert.Equal(new[] { 1, 2, 3, 4 }, levels);
        GameEvent hearing = g.State.Events.Single(e => e.Kind == EventKind.AttentionRose && e.SubjectId == BlocStanding.Hearing);
        Assert.Equal(g.C.World.Attention.HearingReputation, hearing.Amount);

        Assert.True(WorldQueries.IsFrozen(g.Me, "us"));
        g.Refused(Command.BuyLand(g.Id, "us-virginia"), CommandError.PermitFrozen);
        g.Refused(Command.BuildHall(g.Id, hall.Id, 100), CommandError.PermitFrozen);
        g.Refused(Command.LeaseColo(g.Id, "us-virginia", 50), CommandError.PermitFrozen);
        g.Refused(Command.BuyGpus(g.Id, 1, hall.Id), CommandError.PermitFrozen);
        g.Refused(Command.BuyGpus(g.Id, 1), CommandError.PermitFrozen);
        g.Do(Command.LeaseColo(g.Id, "eu-ireland", 50));

        g.Do(Command.SellGpus(g.Id, 100, hall.Id));
        g.Until(() => !WorldQueries.IsFrozen(g.Me, "us"), 48, "the freeze to lift");
        Assert.True(g.Me.Bloc("us").Attention < g.C.World.Attention.FreezeExitBelow);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.AttentionFell);
        Assert.Equal(4, g.State.Events.Count(e => e.Kind == EventKind.AttentionRose));
    }

    [Fact]
    public void A_hearing_costs_reputation_unless_the_lab_has_policy_staff_ready()
    {
        Game g = Rich(null, NoticeAt(10));
        Holding hall = Hall(g, "us-oregon", 100);
        g.Do(Command.BuyGpus(g.Id, 100, hall.Id));
        g.Build("policy-office", 5, 5);
        g.Hire("policy", g.C.World.Attention.PreparedPolicyStaff);

        g.Until(() => g.Me.Bloc("us").Level >= BlocStanding.Hearing, 48 * 3, "a hearing");
        GameEvent hearing = g.State.Events.Single(e => e.Kind == EventKind.AttentionRose && e.SubjectId == BlocStanding.Hearing);
        Assert.Equal(g.C.World.Attention.HearingReputationPrepared, hearing.Amount);
    }

    // ---- bankruptcy and saves ----------------------------------------------------------------------------------

    [Fact]
    public void A_bankrupt_lab_gives_up_its_sites_and_the_plots_come_free_again()
    {
        Game g = Duo(c =>
        {
            c.World.Site("us-oregon").Plots = 1;
            c.PlayerStart.Cash = 400000;
        });
        Holding hall = Hall(g, "us-oregon", 100);
        g.Do(Command.BuyGpus(g.Id, 5, hall.Id));
        LabState rival = g.Rival("openaye");
        g.Refused(Command.BuyLand(rival.Id, "us-oregon"), CommandError.NoFreePlot);

        Finance.Spend(g.Me, LedgerLine.Fines, g.Me.Cash + 100000);
        g.Until(() => g.Me.Bankrupt, 48 * 3, "bankruptcy");
        Assert.Empty(g.Me.Holdings);
        Assert.DoesNotContain(g.Me.Fleet, b => b.HoldingId != HardwareBatch.Hq);
        Assert.Equal(0, WorldQueries.SiteGridUsed(g.State, "us-oregon"));
        g.Do(Command.BuyLand(rival.Id, "us-oregon"));
    }

    [Fact]
    public void A_world_with_halls_heatwaves_and_attention_saves_loads_and_replays_to_the_same_game()
    {
        Game g = Rich(null, c =>
        {
            NoticeAt(10)(c);
            c.World.Site("us-oregon").SummerHeatwavePermille = 1000;
            c.World.Heatwave.RollDivisor = 1;
        });
        Holding hall = Hall(g, "us-oregon", 100);
        g.Do(Command.BuyGpus(g.Id, 100, hall.Id));
        g.Do(Command.LeaseColo(g.Id, "us-virginia", 80));
        g.Years(2);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.Heatwave);
        Assert.True(g.Me.Bloc("us").Level > 0);

        string saved = g.Sim.Save();
        Simulation loaded = Simulation.Load(g.C, saved);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        loaded.StepMany(48);
        g.Weeks(48);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());

        ReplayLog log = ReplayLog.FromJson(g.Sim.Journal.ToJson());
        Assert.Equal(g.Sim.Digest(), Simulation.Replay(g.C, log).Digest());
    }

    // ---- the planner -------------------------------------------------------------------------------------------

    [Fact]
    public void A_lab_that_owns_its_gpus_and_has_site_appetite_builds_a_data_centre_in_its_first_years()
    {
        Game g = Game.New(1);
        LabState xay = g.Rival("xay");
        g.Years(8);
        Assert.NotEmpty(xay.Holdings);
        Assert.Contains(xay.Holdings, h => h.Kind == HoldingKind.Owned && h.HallKw > 0);
        Assert.Contains(xay.Fleet, b => b.HoldingId != HardwareBatch.Hq);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.HallBuilt && e.LabId == xay.Id);
        Assert.All(xay.Holdings, h => Assert.Contains(g.C.World.Site(h.SiteId).BlocId, g.C.Rival("xay").Plan.SiteBlocs));
    }

    [Fact]
    public void A_lab_with_no_site_appetite_never_buys_land_and_neither_does_one_that_rents()
    {
        Game g = Game.New(1, c => c.Rival("xay").Plan.SiteAppetitePermille = 0);
        g.Years(12);
        foreach (LabState lab in g.State.Labs.Where(l => !string.IsNullOrEmpty(l.RivalId)))
            Assert.Empty(lab.Holdings);
    }

    [Fact]
    public void Every_lab_keeps_a_sound_world_through_a_long_game()
    {
        Game g = Game.New(3);
        for (int year = 0; year < 12; year++)
        {
            g.Years(1);
            Assert.Empty(SaveGame.Problems(g.C, g.State));
            foreach (LabState lab in g.State.Labs)
            {
                Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
                foreach (Holding h in lab.Holdings) Assert.True(WorldQueries.FreeKw(g.C, g.State, lab, h) >= 0, "a hall holds more than its power");
            }
            foreach (SiteConfig site in g.C.World.Sites)
            {
                Assert.True(WorldQueries.SiteGridUsed(g.State, site.Id) <= site.GridKw + g.State.Labs.Sum(l => l.Holdings.Where(h => h.SiteId == site.Id).Sum(h => (long)h.PrivateKw)));
                Assert.True(WorldQueries.PlotsTaken(g.State, site.Id) <= site.Plots);
            }
        }
    }

    // ---- the config --------------------------------------------------------------------------------------------

    [Fact]
    public void The_shipped_world_validates_and_the_validator_names_every_world_mistake_at_once()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));

        c.World.Sites[0].BlocId = "atlantis";
        c.World.Sites[1].Plots = 0;
        c.World.PowerDeals[0].AddsKw = 0;
        c.World.Halls.MinKw = 0;
        c.World.Attention.LetterAt = 900;
        c.World.ExportRules[0].DestBlocs = new List<string> { "atlantis" };
        c.PlayerStart.HomeBlocId = "nowhere";
        List<string> problems = ConfigValidator.Problems(c);
        Assert.Contains(problems, p => p.Contains("atlantis"));
        Assert.Contains(problems, p => p.Contains("plot"));
        Assert.Contains(problems, p => p.Contains("nowhere"));
        Assert.True(problems.Count >= 6, string.Join("\n", problems));
    }

    [Fact]
    public void The_world_is_in_the_data_with_the_spec_s_shape()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(9, c.World.Blocs.Count);
        Assert.Equal(11, c.World.Sites.Count);
        Assert.Equal(4, c.World.PowerDeals.Count);
        Assert.Equal(3, c.World.ExportRules.Count);
        Assert.Equal(new[] { 1000, 1100, 1300 }, new[] { "us-oregon", "us-virginia", "us-texas" }.Select(id => c.World.Site(id).CoolingPermille));
        Assert.Contains(c.Research.Techs, t => t.Id == "liquid-cooling" && t.Effects.Any(e => e.Kind == TechEffectKind.CoolingPermille));
        Assert.Contains(c.Roles, r => r.Id == "infra" && r.Function == RoleFunction.Infrastructure);
        Assert.Contains(c.Roles, r => r.Id == "policy" && r.Function == RoleFunction.Policy);
        Assert.Contains(c.RoomTypes, r => r.Id == "ops-centre" && r.Purpose == RoomPurpose.Infrastructure);
        Assert.Contains(c.RoomTypes, r => r.Id == "policy-office" && r.Purpose == RoomPurpose.Policy);
    }
}
