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

/// <summary>Rival depth: decision logs, feelings, the GPU vendor, signature behaviours, late arrivals and the old-save path to them.</summary>
public class RivalDepthTests
{
    /// <summary>The player and one rival; the log is long enough that nothing is dropped.</summary>
    static Game With(string rivalId, Action<GameConfig> tweak = null, long seed = 1)
    {
        var options = new NewGameOptions { Seed = seed, RivalIds = new List<string> { rivalId } };
        GameConfig c = TestData.Config(cfg =>
        {
            cfg.Rivalry.LogMax = 100000;
            if (tweak != null) tweak(cfg);
        });
        return new Game(Simulation.NewGame(c, options));
    }

    sealed class Order
    {
        public Command Cmd;
        public bool Ok;
    }

    /// <summary>
    /// Plays a rival's lab by hand for some years: the plan runs in the second week of each month, once the vendor has set the month's
    /// allocation, from here instead of inside the step,
    /// and every command it gives is recorded with whether it went through.
    /// </summary>
    static List<Order> Drive(Game g, LabState lab, int years, Action<int> beforeMonth = null)
    {
        var orders = new List<Order>();
        PlanConfig plan = g.C.Rival(lab.RivalId).Plan;
        int start = lab.PlanStartTick;
        lab.PlanStartTick = int.MaxValue;
        for (int w = 0; w < years * g.C.Calendar.WeeksPerYear; w++)
        {
            if (g.Tick > 0 && GameDate.IsMonthStart(g.C.Calendar, g.Tick - 1))
            {
                if (beforeMonth != null) beforeMonth(g.Tick);
                lab.PlanStartTick = start;
                LabPlanner.Run(g.C, g.State, lab, plan, cmd =>
                {
                    CommandResult r = g.Sim.Apply(cmd);
                    orders.Add(new Order { Cmd = cmd, Ok = r.Ok });
                    return r;
                });
                lab.PlanStartTick = int.MaxValue;
            }
            g.Sim.Step();
        }
        lab.PlanStartTick = start;
        return orders;
    }

    static int Count(LabState lab, DecisionAction action) { return lab.Log.Count(e => e.Action == action); }

    [Fact]
    public void Every_order_that_goes_through_leaves_one_line_and_a_refused_one_leaves_none()
    {
        Game g = With("xay");
        LabState xay = g.Rival("xay");
        List<Order> orders = Drive(g, xay, 14);

        Assert.True(orders.Count > 100);
        Assert.Equal(orders.Count(o => o.Ok && o.Cmd.Kind == CommandKind.StartTraining), Count(xay, DecisionAction.StartedRun));
        Assert.Equal(orders.Count(o => o.Ok && o.Cmd.Kind == CommandKind.ReleaseModel), Count(xay, DecisionAction.Released));
        Assert.Equal(orders.Count(o => o.Ok && o.Cmd.Kind == CommandKind.BuildHall), Count(xay, DecisionAction.BuiltHall));
        Assert.Equal(orders.Count(o => o.Ok && o.Cmd.Kind == CommandKind.BuyLand), Count(xay, DecisionAction.BoughtLand));
        Assert.True(Count(xay, DecisionAction.StartedRun) > 0);
        Assert.True(Count(xay, DecisionAction.Hired) > 0);
        Assert.True(Count(xay, DecisionAction.Released) > 0);

        // Hires below the smallest logged size never get a line; the others each carry the runway behind them.
        int minUnits = g.C.Rivalry.MinLoggedUnits;
        Assert.All(xay.Log.Where(e => e.Action == DecisionAction.Hired), e => Assert.True(e.Amount >= minUnits));
        Assert.True(orders.Count(o => o.Ok && o.Cmd.Kind == CommandKind.Hire) >= Count(xay, DecisionAction.Hired));
    }

    [Fact]
    public void An_order_that_is_refused_leaves_no_line()
    {
        Game g = With("openaye");
        LabState lab = g.Rival("openaye");
        g.Years(3);
        int start = lab.PlanStartTick;
        lab.PlanStartTick = int.MaxValue;
        int before = lab.Log.Count;
        int given = 0;
        PlanConfig plan = g.C.Rival("openaye").Plan;
        for (int month = 0; month < 12; month++)
        {
            g.Weeks(g.C.Calendar.WeeksPerMonth);
            lab.PlanStartTick = start;
            LabPlanner.Run(g.C, g.State, lab, plan, cmd =>
            {
                given++;
                return CommandResult.Fail(CommandError.NotEnoughCash, "no");
            });
            lab.PlanStartTick = int.MaxValue;
        }
        Assert.True(given > 0, "the plan had things to ask for");
        var notes = new[] { DecisionAction.SkippedRun, DecisionAction.HeldBack, DecisionAction.Paused, DecisionAction.Lobbied };
        Assert.All(lab.Log.Skip(before), e => Assert.Contains(e.Action, notes));
    }

    [Fact]
    public void A_decision_not_to_act_is_written_down_once_a_quarter_per_line()
    {
        Game g = With("goggle");
        g.Years(25);
        LabState goggle = g.Rival("goggle");
        int quarters = 0;
        foreach (DecisionAction kind in new[] { DecisionAction.SkippedRun, DecisionAction.HeldBack })
        {
            var seen = new HashSet<string>();
            foreach (DecisionEntry e in goggle.Log.Where(x => x.Action == kind))
            {
                string key = e.Ref + "/" + DecisionLog.QuarterOf(g.C, e.Tick);
                Assert.True(seen.Add(key), kind + " twice for " + key);
                quarters++;
            }
        }
        Assert.True(quarters > 0, "a slow lab skips plenty of runs");
    }

    [Fact]
    public void The_log_is_bounded_at_sixty_survives_a_save_and_every_code_has_a_line_of_text()
    {
        Game g = Game.New();
        g.Years(16);
        GameConfig c = g.C;
        Assert.All(g.State.Labs, lab => Assert.True(lab.Log.Count <= c.Rivalry.LogMax));
        Assert.Contains(g.State.Labs, lab => lab.Log.Count == c.Rivalry.LogMax);
        Assert.Empty(g.Me.Log);

        string save = g.Sim.Save();
        Simulation loaded = Simulation.Load(c, save);
        Assert.Equal(save, loaded.Save());
        for (int i = 0; i < g.State.Labs.Count; i++) Assert.Equal(g.State.Labs[i].Log.Count, loaded.State.Labs[i].Log.Count);
        Assert.Equal(g.State.Vendor.Log.Count, loaded.State.Vendor.Log.Count);

        foreach (DecisionAction action in Enum.GetValues(typeof(DecisionAction)))
        {
            var e = new DecisionEntry { Tick = 100, Action = action, Ref = "", Amount = 5 };
            foreach (ReasonCode code in Enum.GetValues(typeof(ReasonCode))) e.Reasons.Add(Reason.Of(code, 123));
            string line = DecisionText.Line(c, g.State, g.State.Labs[1], e);
            Assert.DoesNotContain("{", line);
            Assert.DoesNotContain("}", line);
            foreach (ReasonCode code in Enum.GetValues(typeof(ReasonCode))) Assert.NotNull(c.Rivalry.ReasonTemplate(code.ToString()));
        }
    }

    [Fact]
    public void A_dated_line_reads_like_a_sentence()
    {
        Game g = With("anthoptics");
        LabState lab = g.Rival("anthoptics");
        var e = new DecisionEntry { Tick = 4, Action = DecisionAction.HeldBack, Ref = "Clause Mini 2", Amount = 540 };
        e.Reasons.Add(Reason.Of(ReasonCode.AlignmentBelowGate, 540));
        e.Reasons.Add(Reason.Of(ReasonCode.CultureAlignmentPermille, 250));
        string line = DecisionText.Line(g.C, g.State, lab, e);
        Assert.StartsWith("1995 M2 W1: AntHopticsAI held back Clause Mini 2", line);
        Assert.Contains("540", line);
    }

    [Fact]
    public void AntHoptics_never_releases_a_model_below_its_gate_and_does_release_eventually()
    {
        Game g = With("anthoptics");
        LabState lab = g.Rival("anthoptics");
        int gate = g.C.Rival("anthoptics").Plan.MinReleaseAlignment;
        Assert.Equal(550, gate);
        g.Years(15);
        List<ModelState> released = lab.Models.Where(m => m.ReleasedTick >= 0).ToList();
        Assert.NotEmpty(released);
        Assert.All(released, m => Assert.True(m.Alignment >= gate, m.Name + " went out at " + m.Alignment));
    }

    [Fact]
    public void A_gate_that_holds_models_back_raises_the_alignment_share_until_one_passes()
    {
        GameConfig c = TestData.Config();
        PlanConfig plan = c.Rival("anthoptics").Plan;
        var lab = new LabState();
        Assert.Equal(plan.AlignmentSharePermille, LabPlanner.RunAlignmentShare(c, lab, plan));
        lab.Models.Add(new ModelState { Status = ModelStatus.Trained, Alignment = plan.MinReleaseAlignment - 100 });
        Assert.Equal(plan.AlignmentSharePermille + c.Rivalry.GateBoostPermille, LabPlanner.RunAlignmentShare(c, lab, plan));
        lab.Models.Add(new ModelState { Status = ModelStatus.Trained, Alignment = plan.MinReleaseAlignment - 1 });
        Assert.Equal(plan.AlignmentSharePermille + 2 * c.Rivalry.GateBoostPermille, LabPlanner.RunAlignmentShare(c, lab, plan));
        lab.Models.Add(new ModelState { Status = ModelStatus.Released, Alignment = plan.MinReleaseAlignment });
        Assert.Equal(plan.AlignmentSharePermille, LabPlanner.RunAlignmentShare(c, lab, plan));
    }

    [Fact]
    public void A_lab_that_publishes_its_safety_research_makes_the_tech_cheaper_for_everyone_else()
    {
        Game g = With("anthoptics");
        LabState anth = g.Rival("anthoptics");
        TechConfig tech = g.C.Tech("regression-tests");
        g.Me.Techs.Remove(tech.Id);
        if (!anth.Techs.Contains(tech.Id)) anth.Techs.Add(tech.Id);
        Assert.Equal(tech.Cost * 700 / 1000, LabQueries.TechCost(g.C, g.State, g.Me, tech));
        Assert.Equal(tech.Cost, LabQueries.TechCost(g.C, g.State, anth, tech));

        g.Me.ResearchPoints = tech.Cost;
        g.Do(Command.ResearchTech(g.Id, tech.Id));
        Assert.Equal(tech.Cost - tech.Cost * 700 / 1000, g.Me.ResearchPoints);

        TechConfig other = g.C.Tech("opening-books");
        Assert.Equal(other.Cost, LabQueries.TechCost(g.C, g.State, g.Me, other));
    }

    [Fact]
    public void The_cheaper_tech_needs_a_lab_that_publishes()
    {
        Game g = With("openaye");
        TechConfig tech = g.C.Tech("regression-tests");
        g.Me.Techs.Remove(tech.Id);
        g.Rival("openaye").Techs.Add(tech.Id);
        Assert.Equal(tech.Cost, LabQueries.TechCost(g.C, g.State, g.Me, tech));
    }

    [Fact]
    public void Allocation_adds_up_to_the_supply_and_a_lab_past_its_share_is_refused()
    {
        Game g = Game.New(1, c => { foreach (YearAmountConfig y in c.Vendor.SupplyByYear) y.Amount = 40; });
        VendorState vendor = g.State.Vendor;
        Assert.Equal(40, vendor.SupplyThisMonth);
        Assert.Equal(vendor.SupplyThisMonth, vendor.Standings.Sum(s => s.AllocationThisMonth));
        for (int month = 0; month < 30; month++)
        {
            g.Weeks(4);
            Assert.Equal(vendor.SupplyThisMonth, vendor.Standings.Where(s => !g.State.Lab(s.LabId).Bankrupt).Sum(s => s.AllocationThisMonth));
        }

        g.Build("server-closet", 3, 3);
        g.Build("server-closet", 3, 3);
        g.Build("server-closet", 3, 3);
        g.Build("server-closet", 3, 3);
        g.Me.Cash += 100000000;
        int left = VendorSystem.AllocationLeft(g.State, g.Me);
        Assert.InRange(left, 1, 40);
        g.Refused(Command.BuyGpus(g.Id, left + 1), CommandError.AllocationExhausted);
        g.Do(Command.BuyGpus(g.Id, left));
        Assert.Equal(0, VendorSystem.AllocationLeft(g.State, g.Me));
        g.Refused(Command.BuyGpus(g.Id, 1), CommandError.AllocationExhausted);
    }

    [Fact]
    public void Standing_raises_a_labs_share_and_buying_raises_standing()
    {
        Game g = With("openaye");
        VendorState vendor = g.State.Vendor;
        VendorStanding me = vendor.Standing(g.Id);
        VendorStanding rival = vendor.Standing(g.Rival("openaye").Id);
        VendorSystem.RunMonth(g.C, g.State);
        int even = me.AllocationThisMonth;
        Assert.InRange(me.AllocationThisMonth - rival.AllocationThisMonth, -1, 1);

        me.Standing = 900;
        rival.Standing = 100;
        VendorSystem.RunMonth(g.C, g.State);
        Assert.True(me.AllocationThisMonth > even);
        Assert.True(me.AllocationThisMonth > rival.AllocationThisMonth);
        Assert.Equal(vendor.SupplyThisMonth, me.AllocationThisMonth + rival.AllocationThisMonth);

        int before = me.Standing;
        VendorSystem.OnBought(g.C, g.State, g.Me, g.C.Vendor.GpusPerStanding * 3);
        Assert.Equal(before + 3, me.Standing);
    }

    [Fact]
    public void A_lab_in_good_standing_pays_less_and_one_in_bad_standing_pays_more()
    {
        Game g = Game.Solo();
        GameConfig c = g.C;
        VendorStanding me = g.State.Vendor.Standing(g.Id);
        me.Standing = 500;
        Assert.Equal(1000, VendorSystem.PricePermille(c, g.State, g.Me));
        me.Standing = c.Vendor.LowStandingBelow - 1;
        Assert.Equal(1000 + c.Vendor.LowStandingMarkupPermille, VendorSystem.PricePermille(c, g.State, g.Me));
        me.Standing = c.Vendor.HighStandingAbove + 1;
        Assert.Equal(1000 - c.Vendor.HighStandingDiscountPermille, VendorSystem.PricePermille(c, g.State, g.Me));
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, g.Tick);
        Assert.Equal(gen.BuyPrice * 950 / 1000, VendorSystem.PricePerGpu(c, g.State, g.Me, gen));
    }

    [Fact]
    public void The_keynote_halves_that_months_supply_and_leaves_an_event()
    {
        Game g = Game.Solo();
        GameConfig c = g.C;
        var supply = new Dictionary<int, int>();
        int target = (2003 - c.Calendar.StartYear + 1) * c.Calendar.WeeksPerYear;
        while (g.Tick < target)
        {
            int month = GameDate.Month(c.Calendar, g.Tick);
            int year = GameDate.Year(c.Calendar, g.Tick);
            bool start = GameDate.IsMonthStart(c.Calendar, g.Tick);
            g.Sim.Step();
            if (start && year == 2003) supply[month] = g.State.Vendor.SupplyThisMonth;
        }
        int full = c.Vendor.SupplyInYear(2003);
        Assert.Equal(full, supply[2]);
        Assert.Equal(full * c.Vendor.KeynoteSupplyPermille / 1000, supply[c.Vendor.KeynoteMonth]);
        Assert.Equal(full, supply[4]);
        GameEvent keynote = g.State.Events.Single(e => e.Kind == EventKind.Keynote && GameDate.Year(c.Calendar, e.Tick) == 2003);
        Assert.Equal("geforks", keynote.Ref);
        Assert.Contains("GeForks 2", EventText.Describe(c, g.State, keynote));
    }

    [Fact]
    public void Goggle_arrives_in_the_first_week_of_2010_and_not_before_and_DeepSneak_in_2023()
    {
        Game g = Game.New();
        GameConfig c = g.C;
        Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, g.State.PendingRivals.ToArray());
        Assert.DoesNotContain(g.State.Labs, l => l.RivalId == "goggle");

        int firstWeek2010 = (2010 - c.Calendar.StartYear) * c.Calendar.WeeksPerYear;
        g.Sim.StepMany(firstWeek2010);
        Assert.DoesNotContain(g.State.Labs, l => l.RivalId == "goggle");
        int labs = g.State.Labs.Count;
        g.Sim.Step();
        Assert.Equal(labs + 1, g.State.Labs.Count);
        LabState goggle = g.State.Labs.Last();
        Assert.Equal("goggle", goggle.RivalId);
        Assert.Equal("Goggle DeepMined", goggle.Name);
        Assert.Equal(c.Rival("goggle").StartCash, goggle.Ledger.StartCash);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RivalJoined && e.Ref == "goggle" && e.Tick == firstWeek2010);
        Assert.NotNull(g.State.Vendor.Standing(goggle.Id));
        Assert.Equal(new[] { "macrosoft", "palantear", "deepsneak" }, g.State.PendingRivals.ToArray());
        Assert.Equal(g.Me, g.State.Labs[0]);

        g.Sim.StepMany((2023 - 2010) * c.Calendar.WeeksPerYear);
        LabState sneak = g.State.Labs.Last();
        Assert.Equal("deepsneak", sneak.RivalId);
        Assert.Equal("china", sneak.HomeBlocId);
        Assert.Empty(g.State.PendingRivals);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.RivalJoined && e.Ref == "deepsneak");
        Assert.Equal(g.State.Labs.Count, g.State.Vendor.Standings.Count);
    }

    [Fact]
    public void A_late_lab_joins_with_the_techs_of_the_eras_before_and_a_full_plan()
    {
        Game g = Game.New();
        g.Sim.StepMany((2010 - g.C.Calendar.StartYear) * g.C.Calendar.WeeksPerYear + 1);
        LabState goggle = g.State.Labs.Last();
        Assert.True(goggle.Techs.Count > 10);
        Assert.All(goggle.Techs, t => Assert.True(EraSystem.TechOpen(g.C, g.C.Tech(t), g.State.Tick)));
        g.Years(3);
        Assert.False(goggle.Bankrupt);
        Assert.True(goggle.Models.Count > 0);
    }

    [Fact]
    public void A_game_without_rivals_gets_no_late_arrivals_and_a_successor_never_spawns()
    {
        Game g = Game.Solo();
        Assert.Empty(g.State.PendingRivals);
        g.Years(16);
        Assert.Single(g.State.Labs);
        Game all = Game.New();
        Assert.DoesNotContain("successor", all.State.PendingRivals);
        all.Years(40);
        Assert.DoesNotContain(all.State.Labs, l => l.RivalId == "successor");
    }

    [Fact]
    public void A_board_with_nothing_to_fear_still_has_fits_and_the_plan_waits_through_them()
    {
        Game g = With("openaye", c => c.Rival("openaye").Plan.DramaPermillePerYear = 1000, 1);
        LabState lab = g.Rival("openaye");
        g.Until(() => lab.PlanPausedUntilTick > g.Tick, 12 * g.C.Calendar.WeeksPerYear, "a board fit");
        int pausedUntil = lab.PlanPausedUntilTick;
        Assert.Equal(g.Tick - 1 + g.C.Rivalry.BoardDramaWeeks, pausedUntil);
        Assert.Contains(lab.Log, e => e.Action == DecisionAction.Paused && e.Reasons.Any(r => r.Code == ReasonCode.BoardDrama));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.BoardDrama && e.LabId == lab.Id);

        int commands = 0;
        PlanConfig plan = g.C.Rival("openaye").Plan;
        for (int w = g.Tick; w < pausedUntil; w++)
        {
            g.State.Tick = w;
            LabPlanner.Run(g.C, g.State, lab, plan, cmd => { commands++; return CommandResult.Done(-1); });
        }
        Assert.Equal(0, commands);
    }

    [Fact]
    public void A_board_that_never_has_fits_never_pauses()
    {
        Game g = With("anthoptics");
        g.Years(10);
        Assert.Equal(-1, g.Rival("anthoptics").PlanPausedUntilTick);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.BoardDrama);
    }

    [Fact]
    public void Winning_the_crown_sours_a_rival_that_was_beaten_by_thirty()
    {
        Game g = With("openaye");
        LabState rival = g.Rival("openaye");
        rival.PlanStartTick = int.MaxValue;
        g.WithStarterLab(gpus: 8);
        ModelState mini = g.Train("mini", 300);
        g.UntilTrained(mini);
        g.Release(mini, "game-ai");
        g.Until(() => g.State.Gauntlets.Count == 1, 60, "the first Gauntlet");
        Assert.Equal(g.Id, g.State.Gauntlets[0].CrownLabId);
        Assert.Equal(g.C.Rivalry.Feelings.CrownPermille, rival.FeelingToPlayer);
        Assert.Equal(-30, rival.FeelingToPlayer);
        Assert.Equal(0, g.Me.FeelingToPlayer);
    }

    [Fact]
    public void Feelings_move_with_wins_drift_back_and_stay_inside_the_scale()
    {
        Game g = With("openaye");
        LabState rival = g.Rival("openaye");
        FeelingsConfig f = g.C.Rivalry.Feelings;
        Assert.Equal(0, rival.FeelingToPlayer);
        RivalSystem.Feel(g.C, rival, f.CrownPermille);
        Assert.Equal(-30, rival.FeelingToPlayer);
        RivalSystem.Feel(g.C, rival, f.SecondPlacePermille);
        Assert.Equal(-90, rival.FeelingToPlayer);
        RivalSystem.Feel(g.C, g.Me, -500);
        Assert.Equal(0, g.Me.FeelingToPlayer);
        RivalSystem.Feel(g.C, rival, -5000);
        Assert.Equal(-1000, rival.FeelingToPlayer);
        RivalSystem.Feel(g.C, rival, 5000);
        Assert.Equal(1000, rival.FeelingToPlayer);

        rival.FeelingToPlayer = -60;
        for (int i = 0; i < 12; i++) RivalSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, rival.FeelingToPlayer);
        rival.FeelingToPlayer = 40;
        for (int i = 0; i < 12; i++) RivalSystem.RunMonth(g.C, g.State);
        Assert.Equal(0, rival.FeelingToPlayer);
    }

    [Fact]
    public void A_feud_lobbies_the_players_home_government_a_little_every_month()
    {
        Game g = With("openaye");
        LabState rival = g.Rival("openaye");
        BlocStanding home = g.Me.Bloc(g.Me.HomeBlocId);
        int before = home.Attention;
        rival.FeelingToPlayer = -400;
        RivalSystem.RunMonth(g.C, g.State);
        Assert.Equal(before, home.Attention);

        rival.FeelingToPlayer = -800;
        Assert.True(RivalSystem.IsFeud(g.C, rival));
        RivalSystem.RunMonth(g.C, g.State);
        Assert.Equal(before + g.C.Rivalry.Feelings.FeudAttentionPerMonth, home.Attention);
        DecisionEntry entry = rival.Log.Last(e => e.Action == DecisionAction.Lobbied);
        Assert.Equal(g.Me.HomeBlocId, entry.Ref);
        Assert.True(entry.Has(ReasonCode.FeelingTowardPlayer));
        RivalSystem.RunMonth(g.C, g.State);
        Assert.Equal(before + 2 * g.C.Rivalry.Feelings.FeudAttentionPerMonth, home.Attention);
    }

    [Fact]
    public void Old_hardware_is_swapped_for_the_current_generation_and_only_the_buying_is_logged()
    {
        Game g = With("xay", c => c.World.Attention.DominanceFromPpm = MathI.Ppm);
        LabState xay = g.Rival("xay");
        List<Order> orders = Drive(g, xay, 18);
        Assert.True(Count(xay, DecisionAction.Replaced) > 0, "xAy never swapped old hardware");
        Assert.Contains(orders, o => o.Ok && o.Cmd.Kind == CommandKind.SellGpus);
        Assert.All(xay.Log.Where(e => e.Action == DecisionAction.Replaced), e => Assert.True(e.ReasonValue(ReasonCode.GenerationsBehind) >= g.C.Rivalry.ReplaceTierGap));
        GpuGenerationConfig now = LabQueries.CurrentGeneration(g.C, g.Tick);
        Assert.All(xay.Fleet, b => Assert.True(now.Tier - Fleet.TierOf(g.C, b) <= 3, "a batch of " + b.GpuGenId + " is still in the racks"));
    }

    [Fact]
    public void Forty_worlds_leave_nearly_every_rival_standing_through_2010()
    {
        int pairs = 0;
        int bankrupt = 0;
        for (long seed = 1; seed <= 40; seed++)
        {
            Game g = Game.New(seed);
            g.Years(15);
            foreach (LabState lab in g.State.Labs.Skip(1))
            {
                pairs++;
                if (lab.Bankrupt) bankrupt++;
            }
        }
        Assert.True(pairs >= 40 * 3, pairs + " rivals in 40 worlds");
        Assert.True(bankrupt * 100 <= pairs * 5, bankrupt + " of " + pairs + " rivals went under");
    }

    [Fact]
    public void A_schema_3_save_moves_up_with_a_vendor_logs_and_the_rivals_still_to_come()
    {
        GameConfig c = TestData.Config();
        string json = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema3-save.json"));
        Simulation sim = Simulation.Load(c, json);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(89, sim.Tick);
        Assert.Equal(550058, sim.Player.Cash);
        Assert.Empty(SaveGame.Problems(c, sim.State));
        Assert.Equal(new[] { "metta", "macrosoft", "palantear", "goggle", "deepsneak" }, sim.State.PendingRivals.ToArray());
        Assert.Equal(4, sim.State.Vendor.Standings.Count);
        Assert.Equal(sim.State.Vendor.SupplyThisMonth, sim.State.Vendor.Standings.Sum(s => s.AllocationThisMonth));
        Assert.All(sim.State.Labs, lab =>
        {
            Assert.Empty(lab.Log);
            Assert.Equal(0, lab.FeelingToPlayer);
            Assert.Equal(-1, lab.PlanPausedUntilTick);
            Assert.Equal(-1, lab.ChipAmbushUntilTick);
        });

        sim.StepMany((2010 - 1995) * c.Calendar.WeeksPerYear - sim.Tick);
        Assert.Equal(5, sim.State.Labs.Count);
        Assert.Equal("metta", sim.State.Labs.Last().RivalId);
        sim.Step();
        Assert.Equal(6, sim.State.Labs.Count);
        Assert.Equal("goggle", sim.State.Labs.Last().RivalId);
        Assert.Equal(sim.Save(), Simulation.Load(c, sim.Save()).Save());
        foreach (LabState lab in sim.State.Labs) Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
    }

    [Fact]
    public void A_schema_3_save_replays_from_its_journal_to_the_same_game()
    {
        GameConfig c = TestData.Config();
        string json = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "tools", "SimTests", "Fixtures", "schema3-save.json"));
        Simulation sim = Simulation.Load(c, json);
        sim.Apply(Command.SetRentedGpus(sim.Player.Id, 3));
        sim.StepMany(3 * c.Calendar.WeeksPerYear);
        ReplayLog log = ReplayLog.FromJson(sim.Journal.ToJson());
        Assert.Equal(sim.Digest(), Simulation.Replay(c, log).Digest());
    }
}
