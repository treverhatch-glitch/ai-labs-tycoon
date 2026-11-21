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

/// <summary>
/// The economy stays in bounds over a lifetime. A rival that lost its market and could not afford its next run used to wait
/// until it was in debt to cut, then bankrupted (OpenAye in about one seed in ten, 1999 to 2002). A plan now retrenches
/// while it still has cash, and gives back only the GPUs its customers are not using.
/// </summary>
public class EconomyBoundsTests
{
    const long CashCeiling = 1_000_000_000_000L;
    const long CashFloor = -1_000_000_000L;

    static Game LossMakingLab(int rented = 8)
    {
        Game g = Game.Solo(seed: 3);
        g.Me.Cash = 5_000_000;
        g.Build("research-lab", 4, 4);
        g.Build("research-lab", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Build("engineering-bay", 4, 4);
        g.Build("safety-lab", 4, 3);
        g.Build("sales-office", 4, 3);
        g.Hire("researcher", 8);
        g.Hire("engineer", 8);
        g.Hire("safety", 2);
        g.Hire("sales", 2);
        g.Rent(rented);
        g.Me.Controller = LabController.Plan;
        return g;
    }

    static long Cost(Game g) { return LabQueries.WeeklyRunningCost(g.C, g.State, g.Me); }

    static void Plan(Game g) { g.Sim.RunPlan(g.Id, g.C.Autopilot); }

    static List<DecisionEntry> Cuts(Game g) { return g.Me.Log.Where(e => e.Action == DecisionAction.Tightened).ToList(); }

    static void LosingMoney(Game g, long weeksOfCash)
    {
        long cost = Cost(g);
        g.Me.LastWeek.Revenue = cost / 3;
        g.Me.Cash = cost * weeksOfCash;
    }

    // ---- retrenching: main paths

    [Fact]
    public void A_lab_losing_money_with_a_few_weeks_of_cash_cuts_before_it_is_in_debt()
    {
        Game g = LossMakingLab();
        int staff = LabQueries.TotalStaff(g.Me);
        LosingMoney(g, weeksOfCash: 3);

        Plan(g);

        Assert.Equal(0, g.Me.WeeksInDebt);
        Assert.Equal(staff - staff / 6, LabQueries.TotalStaff(g.Me));
        List<DecisionEntry> cuts = Cuts(g);
        Assert.NotEmpty(cuts);
        Assert.All(cuts, e => Assert.True(e.Has(ReasonCode.RunwayWeeks) && !e.Has(ReasonCode.InDebt)));
    }

    [Fact]
    public void A_retrenching_lab_starts_no_run_and_hires_nobody()
    {
        Game g = LossMakingLab();
        LosingMoney(g, 3);
        int models = g.Me.Models.Count;

        Plan(g);

        Assert.Equal(models, g.Me.Models.Count);
        Assert.DoesNotContain(g.Me.Log, e => e.Action == DecisionAction.StartedRun || e.Action == DecisionAction.Hired);
    }

    [Fact]
    public void Cuts_keep_coming_each_plan_until_the_lab_stops_losing_money_then_it_grows_again()
    {
        Game g = LossMakingLab();
        int start = LabQueries.TotalStaff(g.Me);
        LosingMoney(g, 3);
        for (int i = 0; i < 6; i++)
        {
            Plan(g);
            LosingMoney(g, 3);
        }
        Assert.True(LabQueries.TotalStaff(g.Me) < start / 2, "staff only fell to " + LabQueries.TotalStaff(g.Me));
        Assert.Equal(0, g.Me.WeeksInDebt);

        g.Me.LastWeek.Revenue = Cost(g) * 2;
        g.Me.Cash = 100_000_000;
        int before = Cuts(g).Count;
        Plan(g);
        Assert.Equal(before, Cuts(g).Count);
    }

    // ---- edges: thresholds, zero, nothing left to cut

    [Fact]
    public void A_lab_with_cash_to_spare_does_not_cut_however_badly_it_loses_money()
    {
        Game g = LossMakingLab();
        long cost = Cost(g);
        g.Me.LastWeek.Revenue = 0;
        g.Me.Cash = cost * g.C.Rivalry.RetrenchRunwayWeeks;

        Plan(g);

        Assert.Empty(Cuts(g));
    }

    [Fact]
    public void One_week_under_the_runway_cuts_and_exactly_the_runway_does_not()
    {
        Game g = LossMakingLab();
        long cost = Cost(g);
        g.Me.LastWeek.Revenue = cost / 3;
        g.Me.Cash = cost * g.C.Rivalry.RetrenchRunwayWeeks - 1;
        Plan(g);
        Assert.NotEmpty(Cuts(g));
    }

    [Fact]
    public void A_lab_whose_costs_are_just_inside_the_loss_line_does_not_cut_and_just_outside_does()
    {
        Game g = LossMakingLab();
        long cost = Cost(g);
        long line = (cost * MathI.Permille + g.C.Rivalry.RetrenchCostPermille - 1) / g.C.Rivalry.RetrenchCostPermille;

        g.Me.LastWeek.Revenue = line;
        g.Me.Cash = cost;
        Plan(g);
        Assert.Empty(Cuts(g));

        Game h = LossMakingLab();
        h.Me.LastWeek.Revenue = line - 1;
        h.Me.Cash = Cost(h);
        Plan(h);
        Assert.NotEmpty(Cuts(h));
    }

    [Fact]
    public void A_lab_with_no_revenue_and_no_cash_cuts_and_keeps_one_of_every_role()
    {
        Game g = LossMakingLab();
        string[] roles = g.C.Roles.Where(r => LabQueries.StaffCount(g.Me, r.Id) > 0).Select(r => r.Id).ToArray();
        g.Me.LastWeek.Revenue = 0;
        g.Me.Cash = 0;
        for (int i = 0; i < 20; i++) Plan(g);

        Assert.All(roles, id => Assert.Equal(1, LabQueries.StaffCount(g.Me, id)));
        Assert.Equal(roles.Length, LabQueries.TotalStaff(g.Me));
    }

    [Fact]
    public void A_lab_with_one_person_in_each_role_has_nobody_to_cut_and_the_plan_still_runs()
    {
        Game g = Game.Solo(seed: 4).WithStarterLab(researchers: 1, engineers: 1, safety: 0, sales: 0, gpus: 0);
        LosingMoney(g, 3);
        int staff = LabQueries.TotalStaff(g.Me);
        Plan(g);
        Assert.Equal(staff, LabQueries.TotalStaff(g.Me));
    }

    [Fact]
    public void A_bankrupt_lab_makes_no_plan()
    {
        Game g = LossMakingLab();
        LosingMoney(g, 0);
        g.Me.Bankrupt = true;
        string before = g.Sim.Digest();
        Plan(g);
        Assert.Equal(before, g.Sim.Digest());
        Assert.Empty(Cuts(g));
    }

    [Fact]
    public void A_lab_already_in_debt_is_still_cut_for_being_in_debt()
    {
        Game g = LossMakingLab();
        g.Me.Cash = -1000;
        g.Me.WeeksInDebt = 2;
        Plan(g);
        List<DecisionEntry> cuts = Cuts(g);
        Assert.NotEmpty(cuts);
        Assert.All(cuts, e => Assert.Equal(2, e.ReasonValue(ReasonCode.InDebt)));
    }

    // ---- GPUs and customers

    [Fact]
    public void A_lab_that_cannot_serve_its_customers_keeps_every_rented_gpu()
    {
        Game g = LossMakingLab(rented: 8);
        LosingMoney(g, 3);
        g.Me.LastWeek.ServedPermille = 500;
        g.Me.LastWeek.ServingDemand = 2_000_000;
        g.Me.LastWeek.ServingCompute = 1_000_000;

        Plan(g);

        Assert.Equal(8, g.Me.RentedGpus);
        Assert.DoesNotContain(Cuts(g), e => e.Ref == "");
    }

    [Fact]
    public void A_lab_with_idle_rented_gpus_gives_back_a_quarter_of_them_at_most()
    {
        Game g = LossMakingLab(rented: 8);
        LosingMoney(g, 3);
        g.Me.LastWeek.ServedPermille = MathI.Permille;
        g.Me.LastWeek.ServingDemand = 0;

        Plan(g);

        Assert.Equal(6, g.Me.RentedGpus);
    }

    [Fact]
    public void A_lab_using_most_of_its_compute_gives_back_only_the_idle_part()
    {
        Game g = LossMakingLab(rented: 8);
        LosingMoney(g, 3);
        g.Me.LastWeek.ServedPermille = MathI.Permille;
        g.Me.LastWeek.ServingCompute = 1000;
        g.Me.LastWeek.ServingDemand = 900;

        Plan(g);

        Assert.Equal(8, g.Me.RentedGpus);
    }

    [Fact]
    public void Cutting_never_costs_a_lab_revenue_it_was_earning_from_its_own_customers()
    {
        Game g = Game.New(seed: 8);
        LabState rival = g.Rival("openaye");
        g.Weeks(60);
        long before = rival.LastWeek.Revenue;
        rival.Cash = LabQueries.WeeklyRunningCost(g.C, g.State, rival) * 2;
        g.Sim.RunPlan(rival.Id, g.C.Rival("openaye").Plan);
        g.Weeks(1);
        Assert.True(rival.LastWeek.Revenue * 2 >= before, "revenue fell from " + before + " to " + rival.LastWeek.Revenue);
    }

    // ---- config

    [Fact]
    public void The_retrench_settings_are_validated()
    {
        AssertProblem(ConfigValidator.Problems(Damaged(c => c.Rivalry.RetrenchRunwayWeeks = 0)), "retrenchRunwayWeeks");
        AssertProblem(ConfigValidator.Problems(Damaged(c => c.Rivalry.RetrenchCostPermille = 999)), "retrenchCostPermille");
        Assert.Empty(ConfigValidator.Problems(Damaged(c => c.Rivalry.RetrenchCostPermille = 1000)));
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    static GameConfig Damaged(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return c;
    }

    [Fact]
    public void A_config_missing_a_retrench_setting_is_refused_with_the_key_named()
    {
        string json = TestData.ConfigJson().Replace("\"retrenchRunwayWeeks\"", "\"retrenchRunwayXWeeks\"");
        Exception e = Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(json));
        Assert.Contains("retrenchRunwayWeeks", e.Message);
    }

    [Fact]
    public void The_retrench_settings_survive_the_config_round_trip()
    {
        GameConfig again = GameConfig.FromJson(TestData.Config().ToJson());
        Assert.Equal(TestData.Config().Rivalry.RetrenchRunwayWeeks, again.Rivalry.RetrenchRunwayWeeks);
        Assert.Equal(TestData.Config().Rivalry.RetrenchCostPermille, again.Rivalry.RetrenchCostPermille);
    }

    // ---- save and load

    [Fact]
    public void A_game_saved_in_the_middle_of_a_retrenchment_loads_and_carries_on_the_same()
    {
        Game g = Game.New(seed: 2);
        g.Weeks(48 * 4);
        LabState rival = g.Rival("openaye");
        rival.Cash = Math.Min(rival.Cash, LabQueries.WeeklyRunningCost(g.C, g.State, rival) * 2);
        g.Sim.RunPlan(rival.Id, g.C.Rival("openaye").Plan);

        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(g.Sim.Digest(), loaded.Sim.Digest());
        g.Weeks(100);
        loaded.Weeks(100);
        Assert.Equal(g.Sim.Digest(), loaded.Sim.Digest());
    }

    [Fact]
    public void A_schema_1_save_still_loads_and_plays_with_the_retrench_rule()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema1Save());
        sim.StepMany(48 * 3);
        Assert.Equal(sim.Digest(), Simulation.Load(sim.Config, sim.Save()).Digest());
    }

    // ---- the lifetime: nobody breaks and the numbers stay in bounds

    internal sealed class Lifetime
    {
        public long MaxCash = long.MinValue;
        public long MinCash = long.MaxValue;
        public readonly List<string> Bankrupt = new List<string>();
        public long MaxRevenueOverPool;
    }

    internal static Lifetime Live(string startEra, int years, long seed)
    {
        var game = new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = seed, StartEraId = startEra }));
        GameConfig c = game.C;
        var life = new Lifetime();
        for (int week = 0; week < years * c.Calendar.WeeksPerYear; week++)
        {
            if (GameDate.IsMonthStart(c.Calendar, game.Tick)) game.Sim.RunPlan(game.Id, c.Autopilot);
            game.Sim.Step();
            if (GameDate.IsMonthStart(c.Calendar, game.Tick) || week == years * c.Calendar.WeeksPerYear - 1) Survey(game, life);
        }
        foreach (LabState lab in game.State.Labs) if (lab.Bankrupt) life.Bankrupt.Add(lab.Name);
        return life;
    }

    static void Survey(Game game, Lifetime life)
    {
        long pool = 0;
        foreach (SectorState sector in game.State.Sectors) pool += sector.PoolPerWeek;
        foreach (LabState lab in game.State.Labs)
        {
            life.MaxCash = Math.Max(life.MaxCash, lab.Cash);
            life.MinCash = Math.Min(life.MinCash, lab.Cash);
            life.MaxRevenueOverPool = Math.Max(life.MaxRevenueOverPool, lab.LastWeek.Revenue - pool);
        }
    }

    internal static void AssertInBounds(Lifetime life, string who)
    {
        Assert.True(life.Bankrupt.Count == 0, who + ": " + string.Join(", ", life.Bankrupt) + " went bankrupt");
        Assert.True(life.MaxCash < CashCeiling, who + ": cash reached " + life.MaxCash);
        Assert.True(life.MaxCash > 0, who + ": nobody ever made money");
        Assert.True(life.MinCash > CashFloor, who + ": cash fell to " + life.MinCash);
        Assert.True(life.MaxRevenueOverPool <= 0, who + ": a lab earned more in a week than every sector pays in total");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(20)]
    [InlineData(24)]
    [InlineData(27)]
    [InlineData(31)]
    public void Forty_years_from_1995_keep_every_lab_solvent_and_cash_inside_bounds(int seed)
    {
        AssertInBounds(Live("", 40, seed), "seed " + seed);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(24)]
    public void Twenty_six_years_from_2009_keep_every_lab_solvent_and_cash_inside_bounds(int seed)
    {
        AssertInBounds(Live("punchline", 26, seed), "2009 seed " + seed);
    }

    [Fact]
    public void The_era_rules_and_the_retrench_rule_together_leave_money_a_tiny_fraction_of_the_ceiling_for_an_idle_player()
    {
        var game = new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 6 }));
        game.Years(15);
        foreach (LabState lab in game.State.Labs)
            Assert.InRange(lab.Cash, CashFloor, CashCeiling / 10);
    }
}
