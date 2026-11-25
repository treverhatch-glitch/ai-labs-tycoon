using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>A lab's own chips: the design screen's rules, tape-out, the foundry, how chips feed compute, selling them and Envydia's answer.</summary>
public class ChipTests
{
    const string Inference = "inference";
    const string Training = "training";

    static readonly string[] ChipTechs =
    {
        "chip-team", "hbm-memory", "training-silicon", "stacked-hbm", "reticle-dies", "low-precision", "liquid-cooling",
    };

    /// <summary>
    /// The player (and, if asked, every rival) in the given year, knowing the chip techs, with a design lab seating some
    /// architects and a lot of cash. The calendar is moved by hand: the sim has no other way to reach 2023 quickly.
    /// </summary>
    internal static Game Lab(int year, int architects = 4, int engineers = 0, bool rivals = false, Action<GameConfig> tweak = null, long cash = 20_000_000_000L)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "punchline" };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(Cfg(tweak), options));
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, year);
        foreach (LabState lab in g.State.Labs) Equip(g, lab, architects, engineers, cash);
        return g;
    }

    static GameConfig Cfg(Action<GameConfig> tweak)
    {
        return TestData.Config(c =>
        {
            c.World.Attention.DominanceFromPpm = MathI.Ppm;
            if (tweak != null) tweak(c);
        });
    }

    static void Equip(Game g, LabState lab, int architects, int engineers, long cash)
    {
        foreach (string tech in ChipTechs) if (!lab.Techs.Contains(tech)) lab.Techs.Add(tech);
        Finance.Earn(lab, LedgerLine.CapitalIn, cash);
        if (architects > 0)
        {
            LotGrid.FindFreeRect(g.C.Lot(lab.LotId), lab.Rooms, 4, Math.Max(3, architects), out int x, out int y);
            Assert.True(g.Try(Command.BuildRoom(lab.Id, "chip-design-lab", x, y, 4, Math.Max(3, architects))).Ok);
            Assert.True(g.Try(Command.Hire(lab.Id, "chip-architect", architects)).Ok);
        }
        if (engineers > 0)
        {
            for (int bays = (engineers + 3) / 4; bays > 0; bays--)
            {
                LotGrid.FindFreeRect(g.C.Lot(lab.LotId), lab.Rooms, 4, 4, out int x, out int y);
                Assert.True(g.Try(Command.BuildRoom(lab.Id, "engineering-bay", x, y, 4, 4)).Ok);
            }
            Assert.True(g.Try(Command.Hire(lab.Id, "engineer", engineers)).Ok);
        }
    }

    internal static int Hall(Game g, LabState lab, string siteId = "us-oregon", int kw = 1_000_000)
    {
        int id = g.Do(Command.BuyLand(lab.Id, siteId)).CreatedId;
        lab.Holding(id).HallKw = kw;
        return id;
    }

    static Command Design(int labId, string target = Inference, string node = "n28", string die = "small", string format = "fp16", string memory = "ddr", string cooling = "air")
    {
        return Command.StartChipDesign(labId, target, node, die, format, memory, cooling);
    }

    /// <summary>Starts a design and makes it Ready without waiting: the points are done and the tape-out has come back.</summary>
    internal static ChipDesign Ready(Game g, LabState lab, string target = Inference, string node = "n28", string die = "small", string format = "fp16", string memory = "ddr", string cooling = "air", int yield = -1)
    {
        var d = new ChipDesign();
        ChipSpec spec = ChipQueries.Derive(g.C, target, node, die, format, memory, cooling);
        d.Id = g.State.TakeId();
        d.Version = ChipQueries.NextVersion(lab);
        d.Name = lab.FamilyName + " Silicon " + d.Version;
        d.Target = target;
        d.NodeId = node;
        d.DieId = die;
        d.FormatId = format;
        d.MemoryId = memory;
        d.CoolingId = cooling;
        d.PointsRequired = spec.Points;
        d.PointsDone = spec.Points;
        d.Status = ChipDesignStatus.Ready;
        d.YieldPermille = yield < 0 ? spec.InitialYieldPermille : yield;
        d.StageEndsTick = -1;
        d.ReadyTick = g.Tick;
        lab.ChipDesigns.Add(d);
        if (lab.SoftwarePermille == 0) lab.SoftwarePermille = g.C.Chips.SoftwareStart;
        return d;
    }

    internal static void NoLuck(GameConfig c)
    {
        c.Chips.YieldCrashPermille = 0;
        c.Chips.SnubChancePermille = 0;
        foreach (ChipNodeConfig n in c.Chips.Nodes) n.RespinPermille = 0;
    }

    // 1
    [Fact]
    public void The_derived_numbers_of_the_two_worked_designs_are_exactly_the_documented_ones()
    {
        GameConfig c = TestData.Config();
        ChipSpec cheap = ChipQueries.Derive(c, Inference, "n28", "small", "fp16", "ddr", "air");
        Assert.True(cheap.Valid);
        Assert.Equal(99, cheap.Compute);
        Assert.Equal(1, cheap.Kw);
        Assert.Equal(1260, cheap.Points);
        Assert.Equal(3600, cheap.WaferCost);
        Assert.Equal(500, cheap.InitialYieldPermille);
        Assert.Equal(950, cheap.MaxYieldPermille);

        ChipSpec big = ChipQueries.Derive(c, Training, "n5", "medium", "fp8", "hbm-stacked", "liquid");
        Assert.Equal(4118, big.Compute);
        Assert.Equal(3, big.Kw);
        Assert.Equal(15444, big.Points);
        Assert.Equal(64350, big.WaferCost);
        Assert.Equal(300 - 50, big.InitialYieldPermille);
        Assert.Equal(920 - 50, big.MaxYieldPermille);
        Assert.True(big.Trains);
        Assert.Equal(5, big.Tier);

        Assert.False(ChipQueries.Derive(c, Inference, "n28", "tiny", "fp16", "ddr", "air").Valid);
    }

    [Fact]
    public void Kilowatts_are_rounded_up_once_and_never_below_one()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, ChipQueries.Derive(c, Inference, "n28", "small", "fp16", "ddr", "air").Kw);
        Assert.Equal(4, ChipQueries.Derive(c, Training, "n3", "medium", "fp16", "ddr", "air").Kw);
        Assert.Equal(11, ChipQueries.Derive(c, Training, "n3", "reticle", "fp16", "hbm-stacked", "air").Kw);
        Assert.Equal(2, ChipQueries.Derive(c, Inference, "n7", "small", "fp16", "ddr", "air").Kw);
        Assert.Equal(4, ChipQueries.Derive(c, Training, "n3", "medium", "fp16", "ddr", "liquid").Kw);
    }

    // 2
    [Fact]
    public void A_design_is_refused_for_unknown_ids_closed_nodes_and_unknown_techs_and_only_one_runs_at_a_time()
    {
        Game g = Lab(2013);
        g.Refused(Command.StartChipDesign(g.Id, "toaster", "n28", "small", "fp16", "ddr", "air"), CommandError.UnknownId);
        g.Refused(Command.StartChipDesign(g.Id, Inference, "n1", "small", "fp16", "ddr", "air"), CommandError.UnknownId);
        g.Refused(Command.StartChipDesign(g.Id, Inference, "n28", "tiny", "fp16", "ddr", "air"), CommandError.UnknownId);
        g.Refused(Command.StartChipDesign(g.Id, Inference, "n28", "small", "fp16", "ddr", "plasma"), CommandError.UnknownId);
        g.Refused(Design(g.Id, node: "n3"), CommandError.EraLocked);
        g.Refused(Design(g.Id, node: "n16"), CommandError.EraLocked);

        g.Me.Techs.Remove("hbm-memory");
        g.Refused(Design(g.Id, memory: "hbm"), CommandError.MissingPrerequisite);
        g.Me.Techs.Remove("training-silicon");
        g.Refused(Design(g.Id, target: Training), CommandError.MissingPrerequisite);
        g.Me.Techs.Remove("chip-team");
        g.Refused(Design(g.Id), CommandError.MissingPrerequisite);
        g.Me.Techs.Add("chip-team");

        CommandResult first = g.Do(Design(g.Id));
        Assert.Equal(g.C.Chips.SoftwareStart, g.Me.SoftwarePermille);
        g.Refused(Design(g.Id), CommandError.InvalidArgument);
        g.Do(Command.CancelChipDesign(g.Id, first.CreatedId));
        Assert.Equal(ChipDesignStatus.Cancelled, g.Me.Design(first.CreatedId).Status);
        g.Do(Design(g.Id));
        Assert.Equal(2, g.Me.Design(g.Me.ChipDesigns.Last().Id).Version);
        Assert.Equal(g.Me.FamilyName + " Silicon 2", g.Me.ChipDesigns.Last().Name);
    }

    [Fact]
    public void A_design_needs_an_architect_and_the_lab_rooms_need_the_tech()
    {
        Game g = Lab(2013, architects: 0);
        g.Refused(Design(g.Id), CommandError.NotEnoughStaff);

        g.Me.Techs.Remove("chip-team");
        int x, y;
        LotGrid.FindFreeRect(g.C.Lot(g.Me.LotId), g.Me.Rooms, 4, 4, out x, out y);
        g.Refused(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 4), CommandError.MissingPrerequisite);
        g.Refused(Command.BuildRoom(g.Id, "hw-test-lab", x, y, 3, 3), CommandError.MissingPrerequisite);
        g.Me.Techs.Add("chip-team");
        g.Do(Command.BuildRoom(g.Id, "chip-design-lab", x, y, 4, 4));
        g.Do(Command.Hire(g.Id, "chip-architect", 4));
        Assert.Equal(4, LabQueries.Seats(g.C, g.Me, "chip-architect"));

        Game early = Lab(2009, architects: 0);
        early.Me.Techs.Add("chip-team");
        LotGrid.FindFreeRect(early.C.Lot(early.Me.LotId), early.Me.Rooms, 4, 4, out x, out y);
        early.Refused(Command.BuildRoom(early.Id, "chip-design-lab", x, y, 4, 4), CommandError.EraLocked);
    }

    // 3
    [Fact]
    public void Four_architects_finish_a_1260_point_design_in_32_weeks()
    {
        Game g = Lab(2013, architects: 4);
        ChipDesign d = g.Me.Design(g.Do(Design(g.Id)).CreatedId);
        Assert.Equal(1260, d.PointsRequired);
        g.Weeks(31);
        Assert.Equal(1240, d.PointsDone);
        g.Refused(Command.TapeOut(g.Id, d.Id), CommandError.DesignNotReady);
        g.Weeks(1);
        Assert.Equal(1260, d.PointsDone);
        Assert.Equal(32, ChipQueries.WeeksToFinishDesign(g.C, g.Me, g.Me.Design(d.Id)) + 32);
        g.Do(Command.TapeOut(g.Id, d.Id));
    }

    [Fact]
    public void Software_matures_with_the_architects_up_to_the_base_cap_and_techs_add_on_top()
    {
        Game g = Lab(2013, architects: 4);
        g.Do(Design(g.Id));
        Assert.Equal(300, g.Me.SoftwarePermille);
        g.Weeks(100);
        Assert.Equal(300 + 100 * 4 * 2 / 10, g.Me.SoftwarePermille);
        g.Weeks(600);
        Assert.Equal(g.C.Chips.SoftwareBaseCap, g.Me.SoftwarePermille);
        Assert.Equal(700, ChipQueries.EffectiveSoftware(g.C, g.Me));
        g.Me.Techs.Add("compiler-stack-1");
        g.Me.Techs.Add("compiler-stack-2");
        Assert.Equal(900, ChipQueries.EffectiveSoftware(g.C, g.Me));
        g.Me.Techs.Add("compiler-stack-3");
        g.Me.Techs.Add("optical-interconnect");
        Assert.Equal(1000, ChipQueries.EffectiveSoftware(g.C, g.Me));
    }

    // 4
    [Fact]
    public void Respin_odds_are_the_nodes_own_less_what_engineers_take_off()
    {
        foreach (int engineers in new[] { 0, 10 })
        {
            Game g = Lab(2023, architects: 0, engineers: engineers, tweak: c => c.Chips.YieldCrashPermille = 0, cash: 200_000_000_000_000L);
            ChipNodeConfig n5 = g.C.Chips.Node("n5");
            int chance = ChipQueries.RespinChancePermille(g.C, g.Me, n5);
            Assert.Equal(250 - 10 * engineers, chance);

            const int Runs = 10000;
            for (int i = 0; i < Runs; i++)
            {
                ChipDesign d = Ready(g, g.Me, Training, "n5", "medium", "fp8", "hbm", "air");
                d.Status = ChipDesignStatus.TapingOut;
                d.StageEndsTick = g.Tick;
                d.YieldPermille = 0;
            }
            g.Weeks(1);
            int respun = g.Me.ChipDesigns.Count(d => d.Respins == 1);
            int working = g.Me.ChipDesigns.Count(d => d.Status == ChipDesignStatus.Ready);
            Assert.Equal(Runs, respun + working);
            Assert.InRange(respun, (chance - 10) * Runs / 1000, (chance + 10) * Runs / 1000);
        }
    }

    [Fact]
    public void A_respin_costs_forty_percent_of_the_tape_out_and_twelve_more_weeks_and_a_hit_is_a_ready_design()
    {
        Game g = Lab(2013, architects: 4, tweak: c => c.Chips.Node("n28").RespinPermille = 1000);
        ChipDesign d = g.Me.Design(g.Do(Design(g.Id)).CreatedId);
        g.Weeks(32);
        long before = g.Me.Cash;
        g.Do(Command.TapeOut(g.Id, d.Id));
        Assert.Equal(before - 1_500_000, g.Me.Cash);
        Assert.Equal(g.Tick + 8, d.StageEndsTick);
        int taped = g.Tick;
        g.Until(() => d.Respins == 1, 12, "the first respin");
        Assert.Equal(ChipDesignStatus.TapingOut, d.Status);
        Assert.Equal(taped + 20, d.StageEndsTick);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ChipRespin && e.Amount == 600_000);

        g.C.Chips.Node("n28").RespinPermille = 0;
        g.Until(() => d.Status == ChipDesignStatus.Ready, 20, "the working chip");
        Assert.Equal(500, d.YieldPermille);
        Assert.Equal(0, d.Respins - 1);
    }

    [Fact]
    public void The_first_chip_has_its_own_line_of_news_and_the_text_reads_well()
    {
        Game g = Lab(2013, architects: 4, tweak: NoLuck);
        ChipDesign d = g.Me.Design(g.Do(Design(g.Id)).CreatedId);
        g.Weeks(32);
        g.Do(Command.TapeOut(g.Id, d.Id));
        g.Until(() => d.Status == ChipDesignStatus.Ready, 12, "the working chip");
        GameEvent ready = g.State.Events.Single(e => e.Kind == EventKind.ChipReady);
        Assert.Equal(1, ready.Pick);
        Assert.Contains("runs Minesweeper at 4,000 frames per second", EventText.Describe(g.C, g.State, ready));
        Assert.Contains("Respin", EventText.Describe(g.C, g.State, new GameEvent { Tick = 1, Kind = EventKind.ChipRespin, LabId = g.Id, Amount = 5, Ref = "X" }));
        Assert.Contains("Yield crash", EventText.Describe(g.C, g.State, new GameEvent { Tick = 1, Kind = EventKind.ChipYieldCrash, LabId = g.Id, Amount = 5, Ref = "X" }));
    }

    // 5
    [Fact]
    public void Yield_ramps_by_the_formula_and_stops_at_the_maximum_and_a_test_lab_doubles_the_ramp()
    {
        Game plain = Lab(2013, architects: 0);
        ChipDesign a = Ready(plain, plain.Me);
        Assert.Equal(500, a.YieldPermille);
        plain.Weeks(10);
        Assert.Equal(600, a.YieldPermille);
        plain.Weeks(100);
        Assert.Equal(950, a.YieldPermille);

        Game staffed = Lab(2013, architects: 0, engineers: 3);
        ChipDesign b = Ready(staffed, staffed.Me);
        staffed.Weeks(10);
        Assert.Equal(500 + 10 * 13, b.YieldPermille);

        Game tested = Lab(2013, architects: 0, engineers: 3);
        ChipDesign c = Ready(tested, tested.Me);
        tested.Build("hw-test-lab", 3, 3);
        tested.Weeks(10);
        Assert.Equal(500 + 10 * 26, c.YieldPermille);

        Game many = Lab(2013, architects: 0, engineers: 12);
        ChipDesign d = Ready(many, many.Me);
        many.Weeks(1);
        Assert.Equal(500 + 20, d.YieldPermille);
    }

    // 6
    [Fact]
    public void The_foundry_gives_each_month_room_to_every_lab_together_and_the_later_order_moves_on()
    {
        Func<List<int>> run = () =>
        {
            Game g = Lab(2013, rivals: true, tweak: c => { NoLuck(c); c.Chips.FoundryVendorSharePermille = 999; });
            LabState me = g.Me;
            LabState other = g.State.Labs[1];
            Assert.Equal(5, g.C.Chips.FoundryRoom(g.C.Chips.Node("n28")));
            ChipDesign mine = Ready(g, me);
            ChipDesign theirs = Ready(g, other);
            int h1 = Hall(g, me);
            int h2 = Hall(g, other, "us-texas");
            int now = ChipQueries.MonthIndex(g.C, g.Tick);

            g.Do(Command.OrderChips(me.Id, mine.Id, h1, 3));
            Assert.Equal(3, ChipQueries.BookedIn(g.State, "n28", now));
            g.Do(Command.OrderChips(other.Id, theirs.Id, h2, 3));
            Assert.Equal(3, ChipQueries.BookedIn(g.State, "n28", now));
            Assert.Equal(3, ChipQueries.BookedIn(g.State, "n28", now + 1));
            g.Do(Command.OrderChips(me.Id, mine.Id, h1, 2));
            Assert.Equal(5, ChipQueries.BookedIn(g.State, "n28", now));
            g.Refused(Command.OrderChips(me.Id, mine.Id, h1, 6), CommandError.FoundryFull);

            Assert.Equal(ChipQueries.DeliveryTickOf(g.C, g.C.Chips.Node("n28"), now), me.ChipOrders[0].DeliveryTick);
            Assert.Equal(ChipQueries.DeliveryTickOf(g.C, g.C.Chips.Node("n28"), now + 1), other.ChipOrders[0].DeliveryTick);
            Assert.Equal(now * g.C.Calendar.WeeksPerMonth + 12, me.ChipOrders[0].DeliveryTick);
            return me.ChipOrders.Concat(other.ChipOrders).Select(o => o.FoundryMonth).ToList();
        };
        Assert.Equal(run(), run());
    }

    [Fact]
    public void Ordering_costs_the_wafers_over_the_yield_reserves_the_power_and_needs_cash_and_a_hall()
    {
        Game g = Lab(2013, tweak: NoLuck);
        ChipDesign d = Ready(g, g.Me, yield: 800);
        g.Refused(Command.OrderChips(g.Id, d.Id, HardwareBatch.Hq, 10), CommandError.UnknownId);
        int hall = Hall(g, g.Me, kw: 100);
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 101), CommandError.NotEnoughPower);
        g.Refused(Command.OrderChips(g.Id, 424242, hall, 10), CommandError.UnknownId);

        long before = g.Me.Cash;
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 60));
        Assert.Equal(before - 60 * 3600 * 1000 / 800, g.Me.Cash);
        Assert.Equal(60, ChipQueries.ReservedKw(g.Me, hall));
        Assert.Equal(40, WorldQueries.FreeKw(g.C, g.State, g.Me, g.Me.Holding(hall)));
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 41), CommandError.NotEnoughPower);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 40));
        Assert.Equal(0, WorldQueries.FreeKw(g.C, g.State, g.Me, g.Me.Holding(hall)));

        ChipDesign unfinished = g.Me.Design(g.Do(Design(g.Id, die: "medium")).CreatedId);
        g.Refused(Command.OrderChips(g.Id, unfinished.Id, hall, 1), CommandError.DesignNotReady);

        Game poor = Lab(2013, tweak: NoLuck, cash: 100_000_000);
        ChipDesign pd = Ready(poor, poor.Me, yield: 800);
        int h = Hall(poor, poor.Me);
        poor.Me.Cash = 0;
        poor.Refused(Command.OrderChips(poor.Id, pd.Id, h, 10), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Chips_arrive_in_the_hall_at_the_delivery_week_and_a_yield_crash_loses_most_of_them()
    {
        Game g = Lab(2013, tweak: NoLuck);
        ChipDesign d = Ready(g, g.Me, yield: 800);
        int hall = Hall(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 100));
        ChipOrder order = g.Me.ChipOrders.Single();
        g.Until(() => g.Tick >= order.DeliveryTick, 200, "the delivery week");
        g.Weeks(1);
        Assert.Empty(g.Me.ChipOrders);
        HardwareBatch batch = g.Me.Fleet.Single();
        Assert.Equal(100, batch.Count);
        Assert.Equal(d.Id, batch.ChipDesignId);
        Assert.Equal(g.Id, batch.DesignLabId);
        Assert.Equal(hall, batch.HoldingId);
        Assert.Equal(100, WorldQueries.InstalledKw(g.C, g.State, g.Me, hall));

        Game crash = Lab(2013, tweak: c => { NoLuck(c); c.Chips.YieldCrashPermille = 1000; });
        ChipDesign cd = Ready(crash, crash.Me, yield: 800);
        int ch = Hall(crash, crash.Me);
        crash.Do(Command.OrderChips(crash.Id, cd.Id, ch, 100));
        crash.Weeks(60);
        Assert.Equal(30, crash.Me.Fleet.Single().Count);
        Assert.Contains(crash.State.Events, e => e.Kind == EventKind.ChipYieldCrash && e.Amount == 30);
    }

    // 7
    [Fact]
    public void The_export_rules_stop_a_tier_five_chip_going_to_china_for_a_us_lab_but_not_for_deepsneaks_own_design()
    {
        Game g = Lab(2023, rivals: true, tweak: NoLuck);
        g.State.PendingRivals.Remove("deepsneak");
        LabState sneak = WorldSetup.AddRival(g.C, g.State, g.C.Rival("deepsneak"), EraSystem.AtTick(g.C, g.Tick), true);
        Equip(g, sneak, 0, 0, 10_000_000_000L);
        Assert.Equal("china", sneak.HomeBlocId);

        ChipDesign mine = Ready(g, g.Me, Training, "n5", "medium", "fp8", "hbm", "air", yield: 800);
        ChipDesign theirs = Ready(g, sneak, Training, "n5", "medium", "fp8", "hbm", "air", yield: 800);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int usHall = Hall(g, g.Me, chinaSite);
        int cnHall = Hall(g, sneak, chinaSite);

        g.Refused(Command.OrderChips(g.Id, mine.Id, usHall, 2), CommandError.ExportBlocked);
        g.Do(Command.OrderChips(sneak.Id, theirs.Id, cnHall, 2));

        ChipDesign tier4 = Ready(g, g.Me, Inference, "n7", "medium", "fp16", "ddr", "air", yield: 800);
        g.Do(Command.OrderChips(g.Id, tier4.Id, usHall, 2));
    }

    [Fact]
    public void A_frozen_permit_stops_an_order_and_a_chip_batch_cannot_be_moved_or_sold_like_a_gpu()
    {
        Game g = Lab(2013, tweak: NoLuck);
        ChipDesign d = Ready(g, g.Me, yield: 800);
        int hall = Hall(g, g.Me);
        int other = Hall(g, g.Me, "us-texas");
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 10);
        HardwareBatch batch = g.Me.Fleet.Single();

        g.Refused(Command.MoveHardware(g.Id, batch.Id, other, 5), CommandError.InvalidArgument);
        g.Refused(Command.SellGpus(g.Id, 5, hall), CommandError.NotEnoughGpus);
        g.Refused(Command.SellLand(g.Id, hall), CommandError.HoldingNotEmpty);

        g.Do(Command.OrderChips(g.Id, d.Id, other, 5));
        g.Refused(Command.SellLand(g.Id, other), CommandError.HoldingNotEmpty);
        g.Refused(Command.LeaseColo(g.Id, "us-texas", 0), CommandError.InvalidArgument);

        g.Me.Bloc(g.C.World.Site("us-oregon").BlocId).Level = BlocStanding.Freeze;
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 1), CommandError.PermitFrozen);
    }

    // 8
    [Fact]
    public void Inference_chips_never_add_to_training_compute_and_the_training_share_applies_to_general_compute_only()
    {
        Game g = Lab(2023, tweak: NoLuck);
        g.Me.Techs.Add("quantised-serving");
        g.Rent(10);
        ChipDesign inf = Ready(g, g.Me, Inference, "n5", "medium", "fp16", "hbm", "air", yield: 900);
        g.Me.SoftwarePermille = 1000;
        int hall = Hall(g, g.Me);
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, inf.Id, 50);

        long gpu = 10L * LabQueries.CurrentGeneration(g.C, g.Tick).ComputePerGpuWeek;
        ChipSpec spec = ChipQueries.Spec(g.C, inf);
        long chips = 50 * spec.Compute;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal(gpu, split.General);
        Assert.Equal(chips, split.Inference);
        Assert.Equal(0, split.OwnChips);
        Assert.Equal(gpu + chips, LabQueries.WeeklyCompute(g.C, g.State, g.Me));

        g.Me.TrainingSharePermille = 500;
        g.Do(Command.StartTraining(g.Id, g.C.Training.Lines.First(l => !l.Locked).Id, 0));
        Assert.Equal(gpu / 2, TrainingSystem.TrainingCompute(g.C, g.State, g.Me));
        g.Weeks(1);
        Assert.Equal(gpu / 2, g.Me.LastWeek.TrainingCompute);
        ComputeSplit served = LabQueries.Split(g.C, g.State, g.Me, true);
        Assert.Equal(served.General - served.General / 2 + served.Inference, g.Me.LastWeek.ServingCompute);
        Assert.True(g.Me.LastWeek.ServingCompute > g.Me.LastWeek.TrainingCompute);
    }

    // 9
    [Fact]
    public void The_in_house_bonus_applies_to_own_training_chips_and_not_to_bought_ones()
    {
        Game g = Lab(2023, rivals: true, tweak: NoLuck);
        LabState other = g.State.Labs[1];
        ChipDesign own = Ready(g, g.Me, Training, "n5", "medium", "fp8", "hbm", "air", yield: 900);
        ChipDesign foreign = Ready(g, other, Training, "n5", "medium", "fp8", "hbm", "air", yield: 900);
        g.Me.SoftwarePermille = 1000;
        other.SoftwarePermille = 1000;
        int hall = Hall(g, g.Me);
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, own.Id, 10);
        ChipSystem.AddChips(g.State, g.Me, hall, other.Id, foreign.Id, 10);
        g.Me.TrainingSharePermille = 1000;

        long per = ChipQueries.Spec(g.C, own).Compute * 10;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        Assert.Equal(2 * per, split.General);
        Assert.Equal(per, split.OwnChips);
        long expected = per + MathI.MulPermille(per, 1000 + g.C.Chips.InHouseTrainingBonusPermille);
        Assert.Equal(expected, LabQueries.TrainingShareOf(g.C, g.Me, split));

        g.Me.TrainingSharePermille = 500;
        Assert.Equal(per / 2 + MathI.MulPermille(per / 2, 1150), LabQueries.TrainingShareOf(g.C, g.Me, split));
    }

    // 10
    [Fact]
    public void A_lab_at_software_300_gets_30_percent_of_paper_compute_and_the_parts_sum_up()
    {
        Game g = Lab(2023, tweak: NoLuck);
        ChipDesign d = Ready(g, g.Me, Training, "n5", "medium", "fp8", "hbm", "air");
        Assert.Equal(300, g.Me.SoftwarePermille);
        int hall = Hall(g, g.Me);
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 100);
        g.Rent(3);

        long paper = 100 * ChipQueries.Spec(g.C, d).Compute;
        ComputeSplit split = LabQueries.Split(g.C, g.State, g.Me, false);
        long rented = 3L * LabQueries.CurrentGeneration(g.C, g.Tick).ComputePerGpuWeek;
        Assert.Equal(MathI.MulPermille(paper, 300), split.General - rented);
        Assert.Equal(split.General + split.Inference, split.Total);
        Assert.Equal(split.Total, LabQueries.WeeklyCompute(g.C, g.State, g.Me));

        g.Me.Techs.Add("compiler-stack-1");
        Assert.Equal(MathI.MulPermille(paper, 400), LabQueries.Split(g.C, g.State, g.Me, false).General - rented);
    }

    [Fact]
    public void Chips_at_a_site_lose_compute_to_heatwaves_and_to_latency_for_customers_and_buyers_run_the_sellers_software()
    {
        Game g = Lab(2023, rivals: true, tweak: NoLuck);
        LabState seller = g.State.Labs[1];
        ChipDesign d = Ready(g, seller, Inference, "n5", "medium", "fp16", "hbm", "air");
        seller.SoftwarePermille = 700;
        g.Me.SoftwarePermille = 0;
        string siteId = "us-oregon";
        int hall = Hall(g, g.Me, siteId);
        ChipSystem.AddChips(g.State, g.Me, hall, seller.Id, d.Id, 100);

        long paper = 100 * ChipQueries.Spec(g.C, d).Compute;
        long running = MathI.MulPermille(paper, 700);
        Assert.Equal(running, LabQueries.Split(g.C, g.State, g.Me, false).Inference);
        SiteConfig site = g.C.World.Site(siteId);
        Assert.Equal(MathI.MulPermille(running, 1000 - site.LatencyPermille), LabQueries.Split(g.C, g.State, g.Me, true).Inference);

        g.State.Site(siteId).HeatwaveUntilTick = g.Tick + 3;
        int throttle = WorldQueries.HeatwaveThrottle(g.C, g.Me);
        Assert.Equal(MathI.MulPermille(running, 1000 - throttle), LabQueries.Split(g.C, g.State, g.Me, false).Inference);
    }

    // 11
    [Fact]
    public void A_lab_that_runs_only_its_own_chips_pays_less_per_unit_of_training_compute_than_a_gpu_only_lab()
    {
        Game chipLab = Lab(2023, architects: 4, tweak: NoLuck);
        Game gpuLab = Lab(2023, architects: 0, tweak: NoLuck);
        ChipDesign d = Ready(chipLab, chipLab.Me, Training, "n5", "medium", "fp8", "hbm", "air", yield: 800);
        chipLab.Me.SoftwarePermille = 600;
        int chipHall = Hall(chipLab, chipLab.Me);
        long chipSpend = chipLab.Me.Cash;
        chipLab.Do(Command.OrderChips(chipLab.Id, d.Id, chipHall, 200));
        chipSpend -= chipLab.Me.Cash;
        chipLab.Weeks(80);
        Assert.Equal(200, chipLab.Me.Fleet.Single().Count);

        GpuGenerationConfig gen = LabQueries.CurrentGeneration(gpuLab.C, gpuLab.Tick);
        int gpuHall = Hall(gpuLab, gpuLab.Me);
        gpuLab.Weeks(1);
        long gpuSpend = gpuLab.Me.Cash;
        int n = 200;
        gpuLab.Do(Command.BuyGpus(gpuLab.Id, n, gpuHall));
        gpuSpend -= gpuLab.Me.Cash;

        foreach (Game g in new[] { chipLab, gpuLab }) g.Me.TrainingSharePermille = 1000;
        chipLab.Me.Models.Add(new ModelState { Status = ModelStatus.Training, LineId = chipLab.C.Training.Lines.First().Id });
        gpuLab.Me.Models.Add(new ModelState { Status = ModelStatus.Training, LineId = gpuLab.C.Training.Lines.First().Id });
        long chipTraining = TrainingSystem.TrainingCompute(chipLab.C, chipLab.State, chipLab.Me);
        long gpuTraining = TrainingSystem.TrainingCompute(gpuLab.C, gpuLab.State, gpuLab.Me);
        Assert.True(chipTraining > 0 && gpuTraining > 0);
        Assert.True(chipSpend * gpuTraining < gpuSpend * chipTraining,
            "chips cost " + chipSpend + " for " + chipTraining + " compute, GPUs " + gpuSpend + " for " + gpuTraining + " (" + gen.Name + ")");
    }

    // 12
    [Fact]
    public void A_rival_buys_a_listing_under_ninety_percent_of_the_vendors_price_per_compute_and_not_above_and_the_ledgers_balance()
    {
        Func<double, Tuple<Game, LabState, int>> setup = factor =>
        {
            Game g = Lab(2013, rivals: true, tweak: c =>
            {
                NoLuck(c);
                PlanConfig p = c.Rival("deepsneak").Plan;
                p.OwnGpus = true;
                p.SiteAppetitePermille = 400;
                p.BuysListedChips = true;
                p.ChipProgramYear = 2013;
                p.Gpus = 400;
            });
            g.State.PendingRivals.Remove("deepsneak");
            LabState buyer = WorldSetup.AddRival(g.C, g.State, g.C.Rival("deepsneak"), EraSystem.AtTick(g.C, g.Tick), true);
            Equip(g, buyer, 0, 0, 5_000_000_000L);
            buyer.PlanStartTick = 0;
            int hall = Hall(g, buyer, g.C.World.Sites.First(s => s.BlocId == "china").Id);
            buyer.HomeBlocId = "china";

            ChipDesign d = Ready(g, g.Me, Inference, "n28", "medium", "fp16", "ddr", "air", yield: 900);
            g.Me.SoftwarePermille = 1000;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
            long vendorPrice = VendorSystem.PricePerGpu(g.C, g.State, buyer, gen);
            long effective = MathI.MulPermille(ChipQueries.Spec(g.C, d).Compute, 1000);
            long price = (long)(vendorPrice * (double)effective / gen.ComputePerGpuWeek * factor);
            g.Do(Command.SetChipListing(g.Id, d.Id, (int)price, 500));
            return Tuple.Create(g, buyer, hall);
        };

        var cheap = setup(0.85);
        long sellerBase = cheap.Item1.Me.Cash - cheap.Item1.Me.Ledger.Lifetime.Net;
        long buyerBase = cheap.Item2.Cash - cheap.Item2.Ledger.Lifetime.Net;
        LabPlanner.Run(cheap.Item1.C, cheap.Item1.State, cheap.Item2, cheap.Item1.C.Rival("deepsneak").Plan, cmd => cheap.Item1.Try(cmd));
        Game g1 = cheap.Item1;
        Assert.Contains(cheap.Item2.Log, e => e.Action == DecisionAction.BoughtChips);
        ChipOrder order = cheap.Item2.ChipOrders.Single();
        long listed = g1.Me.Listing(order.DesignId).PricePerChip;
        Assert.Equal(order.Count, g1.Me.Listing(order.DesignId).SoldThisMonth);
        Assert.Equal(g1.Me.Ledger.Lifetime.ChipSales, -cheap.Item2.Ledger.Lifetime.Hardware);
        Assert.Equal(order.Count * listed, g1.Me.Ledger.Lifetime.ChipSales);
        Assert.True(g1.Me.Ledger.Lifetime.ChipDevelopment < 0, "the seller pays the foundry");
        Assert.Equal(sellerBase, g1.Me.Cash - g1.Me.Ledger.Lifetime.Net);
        Assert.Equal(buyerBase, cheap.Item2.Cash - cheap.Item2.Ledger.Lifetime.Net);

        var dear = setup(0.95);
        LabPlanner.Run(dear.Item1.C, dear.Item1.State, dear.Item2, dear.Item1.C.Rival("deepsneak").Plan, cmd => dear.Item1.Try(cmd));
        Assert.DoesNotContain(dear.Item2.Log, e => e.Action == DecisionAction.BoughtChips);
        Assert.Empty(dear.Item2.ChipOrders);
    }

    [Fact]
    public void Buying_a_listed_chip_checks_the_listing_the_buyer_and_the_seller_and_warms_the_rival_up()
    {
        Game g = Lab(2013, rivals: true, tweak: NoLuck);
        LabState seller = g.State.Labs[1];
        LabState buyer = g.Me;
        ChipDesign d = Ready(g, seller, Inference, "n28", "medium", "fp16", "ddr", "air", yield: 800);
        seller.SoftwarePermille = 700;
        int hall = Hall(g, buyer);
        g.Refused(Command.BuyListedChips(g.Id, seller.Id, d.Id, hall, 5), CommandError.UnknownId);
        g.Do(Command.SetChipListing(seller.Id, d.Id, 4000, 100));
        g.Refused(Command.BuyListedChips(g.Id, seller.Id, d.Id, hall, 101), CommandError.AllocationExhausted);
        g.Refused(Command.BuyListedChips(g.Id, g.Id, d.Id, hall, 1), CommandError.UnknownLab);
        g.Refused(Command.BuyListedChips(g.Id, seller.Id, d.Id, HardwareBatch.Hq, 1), CommandError.UnknownId);
        g.Refused(Command.BuyListedChips(g.Id, seller.Id, d.Id, hall, 0), CommandError.InvalidArgument);

        seller.Cash = 100;
        g.Refused(Command.BuyListedChips(g.Id, seller.Id, d.Id, hall, 10), CommandError.NotEnoughCash);
        seller.Cash = 10_000_000;
        buyer.Cash = 1000;
        g.Refused(Command.BuyListedChips(g.Id, seller.Id, d.Id, hall, 10), CommandError.NotEnoughCash);
        buyer.Cash = 100_000;

        int feeling = seller.FeelingToPlayer;
        long sellerBefore = seller.Cash;
        g.Do(Command.BuyListedChips(g.Id, seller.Id, d.Id, hall, 10));
        Assert.Equal(feeling + g.C.Rivalry.Feelings.ChipOrderPermille, seller.FeelingToPlayer);
        Assert.Equal(100_000 - 40_000, buyer.Cash);
        Assert.Equal(sellerBefore + 40_000 - ChipQueries.OrderCost(ChipQueries.Spec(g.C, d), 800, 10), seller.Cash);
        Assert.Equal(10, seller.Listing(d.Id).SoldThisMonth);
        Assert.Equal(seller.Id, buyer.ChipOrders.Single().DesignLabId);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ChipsBought && e.Amount == 10 && e.SubjectId == seller.Id);
        Assert.Contains("bought 10", EventText.Describe(g.C, g.State, g.State.Events.Last(e => e.Kind == EventKind.ChipsBought)));
    }

    [Fact]
    public void A_listing_resets_each_month_and_cannot_be_made_for_an_unfinished_design()
    {
        Game g = Lab(2013, tweak: NoLuck);
        ChipDesign unfinished = g.Me.Design(g.Do(Design(g.Id)).CreatedId);
        g.Refused(Command.SetChipListing(g.Id, unfinished.Id, 100, 10), CommandError.DesignNotReady);
        ChipDesign d = Ready(g, g.Me);
        g.Refused(Command.SetChipListing(g.Id, d.Id, 0, 10), CommandError.InvalidArgument);
        g.Refused(Command.SetChipListing(g.Id, d.Id, -5, 10), CommandError.InvalidArgument);
        g.Refused(Command.SetChipListing(g.Id, d.Id, 100, 0), CommandError.InvalidArgument);
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 10));
        g.Do(Command.SetChipListing(g.Id, d.Id, 6000, 20));
        Assert.Equal(6000, g.Me.Listing(d.Id).PricePerChip);
        g.Me.Listing(d.Id).SoldThisMonth = 9;
        g.Weeks(g.C.Calendar.WeeksPerMonth);
        Assert.Equal(0, g.Me.Listing(d.Id).SoldThisMonth);
        g.Do(Command.SetChipListing(g.Id, d.Id, 0, 0));
        Assert.Null(g.Me.Listing(d.Id));
    }

    // 13
    [Fact]
    public void Envydia_takes_standing_for_the_first_training_chip_each_delivery_and_the_first_listing_and_charges_more()
    {
        Game g = Lab(2023, architects: 4, tweak: c =>
        {
            NoLuck(c);
            c.Vendor.DriftPerMonth = 0;
            c.Vendor.GpusPerStanding = 1_000_000;
            foreach (EraConfig era in c.Eras) era.ChipPricePermille = 1000;
        });
        VendorStanding standing = g.State.Vendor.Standing(g.Id);
        int start = standing.Standing;

        ChipDesign d = g.Me.Design(g.Do(Design(g.Id, Training, "n5", "medium", "fp8", "hbm", "air")).CreatedId);
        d.PointsDone = d.PointsRequired;
        g.Do(Command.TapeOut(g.Id, d.Id));
        Assert.Equal(start, standing.Standing);
        g.Until(() => d.Status == ChipDesignStatus.Ready, 12, "the training chip");
        Assert.Equal(start - 150, standing.Standing);

        ChipDesign second = g.Me.Design(g.Do(Design(g.Id, Training, "n5", "medium", "fp8", "hbm", "air")).CreatedId);
        second.PointsDone = second.PointsRequired;
        g.Do(Command.TapeOut(g.Id, second.Id));
        g.Until(() => second.Status == ChipDesignStatus.Ready, 12, "the second training chip");
        Assert.Equal(start - 150, standing.Standing);

        int hall = Hall(g, g.Me);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        long listPrice = VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 10));
        g.Weeks(100);
        Assert.Equal(10, g.Me.Fleet.Single(b => b.ChipDesignId >= 0).Count);
        Assert.Equal(start - 200, standing.Standing);
        Assert.True(ChipQueries.RunsOwnTrainingChips(g.C, g.State, g.Me));
        Assert.Equal(listPrice + gen.BuyPrice / 10, VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen));

        g.Do(Command.SetChipListing(g.Id, d.Id, 100000, 10));
        Assert.Equal(start - 300, standing.Standing);
        g.Do(Command.SetChipListing(g.Id, d.Id, 90000, 10));
        Assert.Equal(start - 300, standing.Standing);
    }

    [Fact]
    public void An_inference_chip_does_not_raise_envydias_price_and_a_training_one_does_only_once_it_runs()
    {
        Game g = Lab(2023, tweak: NoLuck);
        int hall = Hall(g, g.Me);
        ChipDesign inf = Ready(g, g.Me, Inference, "n5", "medium", "fp16", "hbm", "air");
        ChipDesign train = Ready(g, g.Me, Training, "n5", "medium", "fp8", "hbm", "air");
        int before = VendorSystem.PricePermille(g.C, g.State, g.Me);
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, inf.Id, 5);
        Assert.Equal(before, VendorSystem.PricePermille(g.C, g.State, g.Me));
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, train.Id, 5);
        Assert.Equal(before + g.C.Vendor.OwnChipsMarkupPermille, VendorSystem.PricePermille(g.C, g.State, g.Me));
    }

    [Fact]
    public void The_keynote_upstages_a_chip_that_came_out_just_before_and_courting_follows_a_big_chip_share()
    {
        int year = 2017;
        Game g = Lab(year, rivals: true, tweak: c =>
        {
            NoLuck(c);
            c.Vendor.DriftPerMonth = 0;
            c.Vendor.GpusPerStanding = 1_000_000;
        });
        int keynoteTick = GameDate.TickOfYear(g.C.Calendar, year) + (g.C.Vendor.KeynoteMonth - 1) * g.C.Calendar.WeeksPerMonth;
        g.State.Tick = keynoteTick;
        LabState recent = g.Me;
        LabState old = g.State.Labs[1];
        ChipDesign a = Ready(g, recent);
        a.ReadyTick = keynoteTick - g.C.Vendor.AmbushLookbackWeeks;
        ChipDesign b = Ready(g, old);
        b.ReadyTick = keynoteTick - g.C.Vendor.AmbushLookbackWeeks - 1;
        g.State.Vendor.Standing(g.State.Labs[2].Id).Standing = 500;
        int rivalStanding = g.State.Vendor.Standing(g.State.Labs[2].Id).Standing;

        int hall = Hall(g, recent);
        ChipDesign train = Ready(g, recent, Training, "n5", "medium", "fp8", "hbm", "air", yield: 900);
        recent.SoftwarePermille = 1000;
        ChipSystem.AddChips(g.State, recent, hall, recent.Id, train.Id, 1000);
        Assert.True(ChipQueries.OwnChipsShare(g.C, g.State, recent) >= g.C.Vendor.CourtingSharePermille);

        g.Sim.Step();
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.Keynote);
        Assert.Equal(keynoteTick + g.C.Vendor.AmbushWeeks, recent.ChipAmbushUntilTick);
        Assert.True(old.ChipAmbushUntilTick < keynoteTick);

        VendorStanding courted = g.State.Vendor.Standing(g.State.Labs[2].Id);
        Assert.True(courted.Standing >= rivalStanding + g.C.Vendor.CourtingStandingBoost, "the vendor courts everyone but the chip maker");
        Assert.Contains(g.State.Vendor.Log, e => e.Action == DecisionAction.Courted && e.Ref == recent.Id.ToString());
        int courtLines = g.State.Vendor.Log.Count(e => e.Action == DecisionAction.Courted);
        g.Weeks(g.C.Calendar.WeeksPerMonth);
        Assert.Equal(courtLines + 1, g.State.Vendor.Log.Count(e => e.Action == DecisionAction.Courted));
        g.Weeks(g.C.Calendar.WeeksPerMonth);
        Assert.Equal(courtLines + 1, g.State.Vendor.Log.Count(e => e.Action == DecisionAction.Courted));
    }

    // 14
    [Fact]
    public void A_save_in_the_middle_of_a_tape_out_and_a_delivery_loads_into_the_same_game()
    {
        Game g = Lab(2013, architects: 4, rivals: true, tweak: c => { c.Chips.Node("n28").RespinPermille = 500; c.Chips.YieldCrashPermille = 500; });
        int hall = Hall(g, g.Me);
        ChipDesign ready = Ready(g, g.Me, yield: 600);
        g.Do(Command.OrderChips(g.Id, ready.Id, hall, 40));
        ChipDesign d = g.Me.Design(g.Do(Design(g.Id, die: "medium")).CreatedId);
        d.PointsDone = d.PointsRequired;
        g.Do(Command.TapeOut(g.Id, d.Id));
        g.Weeks(3);
        Assert.Equal(ChipDesignStatus.TapingOut, d.Status);
        Assert.Single(g.Me.ChipOrders);

        string save = g.Sim.Save();
        Simulation loaded = Simulation.Load(Cfg(c => { c.Chips.Node("n28").RespinPermille = 500; c.Chips.YieldCrashPermille = 500; }), save);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        g.Weeks(60);
        loaded.StepMany(60);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        Assert.Contains(g.Me.Fleet, b => b.ChipDesignId == ready.Id);
    }

    [Fact]
    public void A_journal_with_chip_commands_replays_into_the_same_game()
    {
        Game g = Lab(2013, architects: 4);
        int hall = Hall(g, g.Me);
        g.Sim.Journal.Entries.Clear();
        g.Sim.Journal.StartSave = g.Sim.Save();
        ChipDesign d = g.Me.Design(g.Do(Design(g.Id, die: "medium")).CreatedId);
        g.Weeks(60);
        g.Do(Command.TapeOut(g.Id, d.Id));
        g.Weeks(30);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 20));
        g.Do(Command.SetChipListing(g.Id, d.Id, 9000, 50));
        g.Weeks(60);
        Simulation again = Simulation.Replay(Cfg(null), g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_lab_that_goes_bankrupt_loses_its_orders_listings_and_the_foundry_room_they_held()
    {
        Game g = Lab(2013, rivals: true, tweak: NoLuck);
        LabState lab = g.Me;
        ChipDesign d = Ready(g, lab);
        int hall = Hall(g, lab);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 30));
        g.Do(Command.SetChipListing(g.Id, d.Id, 5000, 10));
        int month = lab.ChipOrders.Single().FoundryMonth;
        Assert.Equal(30, ChipQueries.BookedIn(g.State, "n28", month));
        Finance.GoBankrupt(g.C, g.State, lab);
        Assert.Empty(lab.ChipOrders);
        Assert.Empty(lab.ChipListings);
        Assert.Equal(0, ChipQueries.BookedIn(g.State, "n28", month));
        Assert.True(lab.Bankrupt);
    }

    [Fact]
    public void The_foundry_books_a_bounded_number_of_months_ahead_and_drops_the_months_gone_by()
    {
        Game g = Lab(2013, tweak: c => { NoLuck(c); c.Chips.FoundryVendorSharePermille = 999; });
        ChipDesign d = Ready(g, g.Me);
        int hall = Hall(g, g.Me);
        int now = ChipQueries.MonthIndex(g.C, g.Tick);
        for (int m = 0; m < g.C.Chips.BookAheadMonths; m++) g.Do(Command.OrderChips(g.Id, d.Id, hall, 5));
        Assert.Equal(g.C.Chips.BookAheadMonths, g.State.Foundry.Count);
        g.Refused(Command.OrderChips(g.Id, d.Id, hall, 1), CommandError.FoundryFull);
        g.Weeks(g.C.Calendar.WeeksPerMonth * 3);
        Assert.Equal(now + 3, ChipQueries.MonthIndex(g.C, g.Tick));
        Assert.DoesNotContain(g.State.Foundry, m => m.MonthIndex < now + 2);
    }

    // planner
    [Fact]
    public void A_plan_run_lab_staffs_designs_tapes_out_and_orders_chips_and_says_why()
    {
        var options = new NewGameOptions { Seed = 3, RivalIds = new List<string> { "xay" } };
        GameConfig c = TestData.Config(cfg => cfg.Rivalry.LogMax = 100000);
        var g = new Game(Simulation.NewGame(c, options));
        for (int w = 0; w < 36 * c.Calendar.WeeksPerYear; w++)
        {
            if (GameDate.IsMonthStart(c.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, c.Autopilot);
            g.Sim.Step();
        }
        LabState xay = g.Rival("xay");
        Assert.False(xay.Bankrupt);
        Assert.Contains(xay.ChipDesigns, d => d.Status == ChipDesignStatus.Ready);
        Assert.Contains(xay.Fleet, b => b.ChipDesignId >= 0 && b.DesignLabId == xay.Id);
        foreach (DecisionAction action in new[] { DecisionAction.StartedChip, DecisionAction.TapedOut, DecisionAction.OrderedChips })
        {
            DecisionEntry e = xay.Log.First(x => x.Action == action);
            string line = DecisionText.Line(c, g.State, xay, e);
            Assert.DoesNotContain("{", line);
            Assert.True(e.Reasons.Count >= 2, action + " should carry its numbers");
        }
        Assert.Equal(0, xay.ChipOrders.Count(o => o.BuyerLabId != xay.Id));
        Assert.All(xay.Holdings, h => Assert.True(WorldQueries.FreeKw(c, g.State, xay, h) >= 0));
    }

    [Fact]
    public void A_lab_that_only_rents_has_no_chip_programme()
    {
        var options = new NewGameOptions { Seed = 3, RivalIds = new List<string> { "openaye" } };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        for (int w = 0; w < 31 * g.C.Calendar.WeeksPerYear; w++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        Assert.Empty(g.Rival("openaye").ChipDesigns);
    }

    // saves
    [Fact]
    public void A_schema_four_save_loads_with_an_empty_chip_programme_and_plays_on()
    {
        string json = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema4-save.json"));
        Simulation sim = Simulation.Load(TestData.Config(), json);
        Assert.Equal(GameState.CurrentSchema, JsonSchema(sim.Save()));
        Assert.Empty(sim.State.Foundry);
        Assert.All(sim.State.Labs, l =>
        {
            Assert.Empty(l.ChipDesigns);
            Assert.Empty(l.ChipOrders);
            Assert.Empty(l.ChipListings);
            Assert.Equal(0, l.SoftwarePermille);
        });
        Assert.All(sim.State.Vendor.Standings, s => Assert.False(s.ListingSeen));
        sim.StepMany(60);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    static long JsonSchema(string save)
    {
        return Json.Parse(save).Get("schema").Int;
    }

    [Fact]
    public void A_broken_chip_reference_is_a_problem_when_loading()
    {
        Game g = Lab(2013, tweak: NoLuck);
        int hall = Hall(g, g.Me);
        ChipDesign d = Ready(g, g.Me);
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 5));
        g.Do(Command.SetChipListing(g.Id, d.Id, 100, 10));
        Assert.Empty(SaveGame.Problems(g.C, g.State));

        g.Me.ChipOrders[0].DesignId = 99999;
        g.Me.ChipListings[0].DesignId = 99998;
        d.DieId = "gigantic";
        g.State.Foundry[0].NodeId = "n1";
        g.Me.Fleet.Add(new HardwareBatch { Id = 1, HoldingId = hall, GpuGenId = "", ChipDesignId = 77777, DesignLabId = g.Id, Count = 1 });
        List<string> problems = SaveGame.Problems(g.C, g.State);
        Assert.Contains(problems, p => p.Contains("on order of a design that does not exist"));
        Assert.Contains(problems, p => p.Contains("lists chips of a design that does not exist"));
        Assert.Contains(problems, p => p.Contains("made of something the game does not know"));
        Assert.Contains(problems, p => p.Contains("foundry has a booking"));
        Assert.Contains(problems, p => p.Contains("has chips of a design that does not exist"));
    }

    [Fact]
    public void The_new_ledger_stays_balanced_through_the_whole_chip_chain()
    {
        Game g = Lab(2013, architects: 4);
        long start = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int hall = Hall(g, g.Me);
        ChipDesign d = g.Me.Design(g.Do(Design(g.Id, die: "medium")).CreatedId);
        g.Weeks(70);
        g.Do(Command.TapeOut(g.Id, d.Id));
        g.Until(() => d.Status == ChipDesignStatus.Ready, 200, "the tape-out");
        g.Do(Command.OrderChips(g.Id, d.Id, hall, 30));
        g.Weeks(70);
        Assert.Equal(g.Me.Cash, start + g.Me.Ledger.Lifetime.Net);
        Assert.True(g.Me.Ledger.Lifetime.ChipDevelopment < 0);
    }

    [Fact]
    public void The_config_validator_refuses_a_broken_chips_section()
    {
        Assert.Throws<ConfigException>(() => TestData.Config(c => c.Chips.Nodes[1].OpensYear = 2000));
        Assert.Throws<ConfigException>(() => TestData.Config(c => c.Chips.Options.RemoveAll(o => o.Group == ChipOptionGroup.Die && string.IsNullOrEmpty(o.RequiresTech))));
        Assert.Throws<ConfigException>(() => TestData.Config(c => c.Chips.ArchitectRoleId = "researcher"));
        Assert.Throws<ConfigException>(() => TestData.Config(c => c.Chips.FoundryVendorSharePermille = 1000));
        Assert.Throws<ConfigException>(() => TestData.Config(c => c.Chips.Options[0].RequiresTech = "nothing"));
        Assert.Throws<ConfigException>(() => TestData.Config(c => c.Research.Techs.First(t => t.Id == "chip-team").Effects[0].Id = "toaster"));
        Assert.Throws<ConfigException>(() => TestData.Config(c => c.RoomType("hw-test-lab").RequiresTech = "nothing"));
    }
}
