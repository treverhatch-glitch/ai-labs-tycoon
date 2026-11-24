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

/// <summary>NEXT item 13: each era turns a few rules (hiring fees, chip prices, appetite, fines, the release law), all read from the era table.</summary>
public class EraRulesTests
{
    static int Tick(GameConfig c, int year) { return GameDate.TickOfYear(c.Calendar, year); }

    static Game StartedIn(string eraId, long seed = 1, bool solo = true, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = eraId };
        if (solo) options.RivalIds = new List<string>();
        return new Game(Simulation.NewGame(TestData.Config(tweak), options));
    }

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = TestData.Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the table

    [Fact]
    public void The_first_era_leaves_every_rule_at_its_baseline()
    {
        EraConfig first = TestData.Config().Eras[0];
        Assert.Equal(1000, first.HireFeePermille);
        Assert.Equal(1000, first.ChipPricePermille);
        Assert.Equal(1000, first.AdoptionPermille);
        Assert.Equal(1000, first.FinePermille);
        Assert.Equal(0, first.MinReleaseAlignment);
    }

    [Fact]
    public void The_eras_tell_a_story_the_talent_war_the_chip_shortage_and_the_law()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Era("it-sees").HireFeePermille > c.Era("punchline").HireFeePermille, "the talent war begins with It Sees");
        int peakShortage = c.Eras.Max(e => e.ChipPricePermille);
        Assert.Equal("chat-boom", c.Eras.First(e => e.ChipPricePermille == peakShortage).Id);
        Assert.True(c.Era("harnesses").ChipPricePermille < peakShortage, "the shortage eases");
        Assert.Equal(0, c.Era("it-writes").MinReleaseAlignment);
        Assert.True(c.Era("chat-boom").MinReleaseAlignment > 0, "the first serious rules arrive with the Chat Boom");
        Assert.True(c.Era("chat-boom").FinePermille > c.Era("it-writes").FinePermille);
        for (int i = 1; i < c.Eras.Count; i++)
            Assert.True(c.Eras[i].MinReleaseAlignment >= c.Eras[i - 1].MinReleaseAlignment, "a law stays on the books");
    }

    [Fact]
    public void The_briefing_lists_every_rule_with_the_era_before_and_the_first_era()
    {
        GameConfig c = TestData.Config();
        List<EraRuleLine> lines = EraRules.Briefing(c, Tick(c, 2022));
        Assert.Equal(Enum.GetValues(typeof(EraRuleKind)).Length, lines.Count);
        EraRuleLine hire = lines.First(l => l.Kind == EraRuleKind.HireFee);
        Assert.Equal(c.Era("chat-boom").HireFeePermille, hire.Value);
        Assert.Equal(c.Era("it-writes").HireFeePermille, hire.Previous);
        Assert.Equal(1000, hire.Baseline);
        EraRuleLine bar = lines.First(l => l.Kind == EraRuleKind.ReleaseBar);
        Assert.Equal(0, bar.Previous);
        Assert.True(bar.Shifted);
    }

    [Fact]
    public void The_briefing_for_the_first_era_has_nothing_to_compare_and_nothing_shifted()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(EraRules.Shifts(c, 0));
        Assert.All(EraRules.Briefing(c, 0), l => Assert.Equal(l.Previous, l.Value));
    }

    [Fact]
    public void Shifts_lists_only_what_the_era_changed()
    {
        GameConfig c = TestData.Config();
        List<EraRuleLine> shifts = EraRules.Shifts(c, Tick(c, 2012));
        Assert.Contains(shifts, l => l.Kind == EraRuleKind.HireFee);
        Assert.DoesNotContain(shifts, l => l.Kind == EraRuleKind.Fine);
        Assert.All(shifts, l => Assert.NotEqual(l.Previous, l.Value));
    }

    [Fact]
    public void Rule_text_reads_as_a_multiplier_or_as_the_law()
    {
        Assert.Equal("x1", EraRules.Text(EraRuleKind.HireFee, 1000));
        Assert.Equal("x1.5", EraRules.Text(EraRuleKind.HireFee, 1500));
        Assert.Equal("x1.25", EraRules.Text(EraRuleKind.Adoption, 1250));
        Assert.Equal("x3", EraRules.Text(EraRuleKind.Fine, 3000));
        Assert.Equal("no rule", EraRules.Text(EraRuleKind.ReleaseBar, 0));
        Assert.Equal("at least 80", EraRules.Text(EraRuleKind.ReleaseBar, 80));
        foreach (EraRuleKind kind in Enum.GetValues(typeof(EraRuleKind))) Assert.False(string.IsNullOrEmpty(EraRules.Label(kind)));
    }

    [Fact]
    public void The_era_banner_names_the_rules_that_shift()
    {
        Game g = StartedIn("punchline");
        g.Until(() => g.Sim.Era.Id == "it-sees", 4 * 48, "the It Sees era");
        g.Weeks(1);
        GameEvent banner = g.State.Events.Last(e => e.Kind == EventKind.EraBegan && e.Ref == "it-sees");
        string text = EventText.Describe(g.C, g.State, banner);
        Assert.Contains("The rules shift", text);
        Assert.Contains("hiring fees x1.5", text);
    }

    [Fact]
    public void An_era_that_changes_nothing_adds_no_rules_line_to_its_banner()
    {
        Game g = StartedIn("punchline", tweak: c =>
        {
            foreach (EraConfig e in c.Eras)
            {
                e.HireFeePermille = 1000; e.ChipPricePermille = 1000; e.AdoptionPermille = 1000; e.FinePermille = 1000; e.MinReleaseAlignment = 0;
                e.IncidentLoadPermille = 1000; e.TrainingCostPermille = 1000; e.ResearchPermille = 1000;
            }
        });
        g.Weeks(1);
        GameEvent banner = g.State.Events.First(e => e.Kind == EventKind.EraBegan);
        Assert.DoesNotContain("The rules shift", EventText.Describe(g.C, g.State, banner));
    }

    // ---- the validator

    [Fact]
    public void The_validator_holds_the_first_era_to_baseline_and_the_range_of_every_rule()
    {
        AssertProblem(Problems(c => c.Eras[0].HireFeePermille = 1500), "first era");
        AssertProblem(Problems(c => c.Eras[0].MinReleaseAlignment = 10), "first era");
        AssertProblem(Problems(c => c.Era("agents").ChipPricePermille = 100), "chipPricePermille");
        AssertProblem(Problems(c => c.Era("agents").AdoptionPermille = 9000), "adoptionPermille");
        AssertProblem(Problems(c => c.Era("agents").FinePermille = 0), "finePermille");
        AssertProblem(Problems(c => c.Era("agents").HireFeePermille = -5), "hireFeePermille");
        AssertProblem(Problems(c => c.Era("upward").MinReleaseAlignment = 1001), "minReleaseAlignment");
        AssertProblem(Problems(c => c.Era("upward").MinReleaseAlignment = -1), "minReleaseAlignment");
    }

    [Fact]
    public void The_validator_refuses_a_law_that_is_repealed()
    {
        AssertProblem(Problems(c => c.Era("harnesses").MinReleaseAlignment = 10), "stays on the books");
    }

    [Fact]
    public void A_config_missing_an_era_rule_is_refused_with_the_key_named()
    {
        string json = TestData.ConfigJson().Replace("\"finePermille\"", "\"fineXPermille\"");
        Exception e = Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(json));
        Assert.Contains("fine", e.Message);
    }

    [Fact]
    public void The_era_rules_survive_the_config_round_trip()
    {
        GameConfig c = TestData.Config();
        GameConfig again = GameConfig.FromJson(c.ToJson());
        for (int i = 0; i < c.Eras.Count; i++)
        {
            foreach (EraRuleKind kind in Enum.GetValues(typeof(EraRuleKind)))
                Assert.Equal(EraRules.Value(c.Eras[i], kind), EraRules.Value(again.Eras[i], kind));
        }
    }

    // ---- the talent war

    [Fact]
    public void The_hiring_fee_follows_the_era()
    {
        GameConfig c = TestData.Config(x => x.Era("punchline").HireFeePermille = 2000);
        RoleConfig role = c.Role("researcher");
        Assert.Equal(role.HireCost, EraRules.HireFee(c, role, Tick(c, 1996)));
        Assert.Equal(role.HireCost * 2, EraRules.HireFee(c, role, Tick(c, 2009)));
        Assert.Equal(role.HireCost * 2 * 3, EraRules.HireCost(c, role, 3, Tick(c, 2010)));
    }

    [Fact]
    public void Hiring_in_a_dearer_era_costs_the_dearer_fee_and_leaves_the_salary_alone()
    {
        Game cheap = StartedIn("punchline");
        Game dear = StartedIn("punchline", tweak: c => c.Era("punchline").HireFeePermille = 3000);
        foreach (Game g in new[] { cheap, dear }) g.Build("research-lab", 4, 4);
        long cheapBefore = cheap.Me.Cash, dearBefore = dear.Me.Cash;
        cheap.Hire("researcher", 2);
        dear.Hire("researcher", 2);
        RoleConfig role = cheap.C.Role("researcher");
        Assert.Equal(role.HireCost * 2, cheapBefore - cheap.Me.Cash);
        Assert.Equal(role.HireCost * 2 * 3, dearBefore - dear.Me.Cash);
        Assert.Equal(LabQueries.WeeklySalaries(cheap.C, cheap.Me), LabQueries.WeeklySalaries(dear.C, dear.Me));
    }

    [Fact]
    public void A_lab_that_cannot_afford_the_dearer_fee_is_refused_and_nothing_changes()
    {
        Game g = StartedIn("punchline", tweak: c => c.Era("punchline").HireFeePermille = 5000);
        g.Build("research-lab", 4, 4);
        RoleConfig role = g.C.Role("researcher");
        g.Me.Cash = role.HireCost * 4;
        g.Refused(Command.Hire(g.Id, "researcher", 1), CommandError.NotEnoughCash);
    }

    [Fact]
    public void A_lab_with_no_cash_cannot_hire_in_any_era_and_one_with_no_seats_is_told_so()
    {
        Game g = StartedIn("punchline");
        g.Refused(Command.Hire(g.Id, "researcher", 1), CommandError.NoSeats);
        g.Build("research-lab", 4, 4);
        g.Me.Cash = 0;
        g.Refused(Command.Hire(g.Id, "researcher", 1), CommandError.NotEnoughCash);
        g.Refused(Command.Hire(g.Id, "researcher", 0), CommandError.InvalidArgument);
    }

    // ---- chip prices

    [Fact]
    public void The_vendors_price_follows_the_eras_shortage()
    {
        Game plain = StartedIn("punchline");
        Game short_ = StartedIn("punchline", tweak: c => c.Era("punchline").ChipPricePermille = 1500);
        GpuGenerationConfig gen = plain.C.Compute.GpuGenerations.First();
        long a = VendorSystem.PricePerGpu(plain.C, plain.State, plain.Me, gen);
        long b = VendorSystem.PricePerGpu(short_.C, short_.State, short_.Me, gen);
        Assert.Equal(a * 3 / 2, b);
        Assert.True(a > 0);
    }

    [Fact]
    public void Buying_a_gpu_in_a_shortage_charges_the_shortage_price()
    {
        Game g = StartedIn("punchline", tweak: c => c.Era("punchline").ChipPricePermille = 2000);
        g.Build("server-closet", 4, 4);
        GpuGenerationConfig gen = LabQueries.CurrentGeneration(g.C, g.Tick);
        long price = VendorSystem.PricePerGpu(g.C, g.State, g.Me, gen);
        long before = g.Me.Cash;
        g.Do(Command.BuyGpus(g.Id, 1));
        Assert.Equal(price, before - g.Me.Cash);
        Assert.True(price >= gen.BuyPrice * 2, "a price doubled by the shortage");
    }

    // ---- appetite

    [Fact]
    public void Adoption_is_scaled_by_the_era_and_never_passes_the_whole_sector()
    {
        GameConfig c = TestData.Config();
        int chat = Tick(c, 2022);
        Assert.Equal(0, EraRules.Adoption(c, 0, chat));
        Assert.Equal(c.Era("chat-boom").AdoptionPermille / 2, EraRules.Adoption(c, 500, chat));
        Assert.Equal(1000, EraRules.Adoption(c, 1000, chat));
        Assert.Equal(1000, EraRules.Adoption(c, 900, chat));
        Assert.Equal(500, EraRules.Adoption(c, 500, Tick(c, 1996)));
    }

    [Fact]
    public void A_hungrier_era_raises_a_sectors_adoption_for_the_same_model()
    {
        Func<int, int> adoptionIn2009 = permille =>
        {
            Game g = StartedIn("punchline", tweak: c => c.Era("punchline").AdoptionPermille = permille);
            g.WithStarterLab(gpus: 12);
            ModelState model = g.Train("mini", 0);
            g.UntilTrained(model, 400);
            g.Release(model, "game-ai");
            g.Weeks(8);
            return g.State.Sector("game-ai").AdoptionPermille;
        };
        int base_ = adoptionIn2009(1000);
        int hungry = adoptionIn2009(1250);
        Assert.True(base_ > 0);
        Assert.True(hungry >= base_);
        Assert.True(hungry <= 1000);
    }

    // ---- the law

    static ModelState Trained(int alignment, int displayed = -1)
    {
        return new ModelState { Name = "Test Mini", Alignment = alignment, DisplayedAlignment = displayed < 0 ? alignment : displayed, Status = ModelStatus.Trained };
    }

    [Fact]
    public void A_model_clears_the_law_at_the_bar_and_not_below_it()
    {
        GameConfig c = TestData.Config();
        int tick = Tick(c, 2022);
        int bar = EraRules.ReleaseBar(c, tick);
        Assert.True(bar > 0);
        Assert.True(EraRules.MeetsReleaseBar(c, Trained(bar), tick));
        Assert.False(EraRules.MeetsReleaseBar(c, Trained(bar - 1), tick));
        Assert.True(EraRules.MeetsReleaseBar(c, Trained(1000), tick));
        Assert.True(EraRules.MeetsReleaseBar(c, Trained(0), Tick(c, 1996)), "no law in the first era");
    }

    [Fact]
    public void A_bar_of_1000_lets_only_a_perfect_model_through_and_a_bar_of_0_everything()
    {
        GameConfig strict = TestData.Config(c => { foreach (EraConfig e in c.Eras.Skip(1)) e.MinReleaseAlignment = 1000; });
        int tick = Tick(strict, 2012);
        Assert.False(EraRules.MeetsReleaseBar(strict, Trained(999), tick));
        Assert.True(EraRules.MeetsReleaseBar(strict, Trained(1000), tick));
        Assert.True(EraRules.MeetsReleaseBar(TestData.Config(), Trained(0), tick));
    }

    [Fact]
    public void Releasing_a_model_below_the_law_is_refused_with_the_reason_and_changes_nothing()
    {
        Game g = StartedIn("punchline", tweak: c => { foreach (EraConfig e in c.Eras.Skip(1)) e.MinReleaseAlignment = 600; });
        g.WithStarterLab(gpus: 12);
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model, 400);
        Assert.True(model.Alignment < 600);
        g.Refused(Command.ReleaseModel(g.Id, model.Id, new[] { "game-ai" }), CommandError.BelowEraSafetyBar);
        CommandResult result = g.Try(Command.ReleaseModel(g.Id, model.Id, new[] { "game-ai" }));
        Assert.Contains("600", result.Message);
        Assert.Contains(g.Sim.Era.Name, result.Message);
        Assert.Equal(ModelStatus.Trained, model.Status);
    }

    [Fact]
    public void A_model_with_enough_alignment_work_goes_on_sale_under_the_law()
    {
        Game g = StartedIn("punchline", tweak: c => { foreach (EraConfig e in c.Eras.Skip(1)) e.MinReleaseAlignment = 600; });
        g.WithStarterLab(gpus: 12);
        ModelState model = g.Train("mini", 300);
        g.UntilTrained(model, 600);
        Assert.True(model.Alignment >= 600);
        g.Release(model, "game-ai");
        Assert.Equal(ModelStatus.Released, model.Status);
    }

    [Fact]
    public void The_law_reads_the_alignment_a_lab_can_show_so_sandbagging_passes_on_paper()
    {
        GameConfig c = TestData.Config();
        int tick = Tick(c, 2022);
        int bar = EraRules.ReleaseBar(c, tick);
        ModelState sandbagger = Trained(alignment: 10, displayed: bar + 50);
        Assert.True(EraRules.MeetsReleaseBar(c, sandbagger, tick));
        ModelState honest = Trained(alignment: 10);
        Assert.False(EraRules.MeetsReleaseBar(c, honest, tick));
    }

    [Fact]
    public void A_model_already_on_sale_keeps_its_customers_when_the_law_arrives()
    {
        Game g = StartedIn("punchline", tweak: c =>
        {
            c.Era("chat-boom").MinReleaseAlignment = 900;
            c.Era("agents").MinReleaseAlignment = 900;
            c.Era("harnesses").MinReleaseAlignment = 900;
            c.Era("bodies").MinReleaseAlignment = 900;
            c.Era("upward").MinReleaseAlignment = 900;
        });
        g.WithStarterLab(gpus: 12);
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model, 400);
        g.Release(model, "game-ai");
        g.Until(() => g.Sim.Era.Id == "chat-boom", 48 * 14, "the Chat Boom");
        Assert.Equal(ModelStatus.Released, model.Status);
        Assert.NotNull(g.State.Sector("game-ai").Entry(g.Id));
    }

    // ---- fines

    [Fact]
    public void The_fine_is_scaled_by_the_era_and_by_nothing_else()
    {
        GameConfig c = TestData.Config(x => x.Era("punchline").FinePermille = 2500);
        Assert.Equal(1000, EraRules.Fine(c, 1000, Tick(c, 1996)));
        Assert.Equal(2500, EraRules.Fine(c, 1000, Tick(c, 2009)));
        Assert.Equal(0, EraRules.Fine(c, 0, Tick(c, 2009)));
    }

    [Fact]
    public void The_same_incidents_cost_three_times_the_fine_under_a_law_that_bites_three_times_as_hard()
    {
        Func<int, (long fines, int incidents)> run = fine =>
        {
            Game g = StartedIn("punchline", seed: 4, tweak: c =>
            {
                c.Era("punchline").FinePermille = fine;
                foreach (StaffTraitConfig t in c.StaffPeople.Traits)
                    if (t.Id != "steady") t.HirePermille = 0;
            });
            g.WithStarterLab(gpus: 12);
            ModelState model = g.Train("mini", 0);
            g.UntilTrained(model, 400);
            g.Release(model, "game-ai", "game-bots");
            g.Years(3);
            return (g.State.Events.Where(e => e.Kind == EventKind.Incident).Sum(e => e.Amount), model.Incidents);
        };
        var one = run(1000);
        var three = run(3000);
        Assert.True(one.incidents > 0, "a reckless model has incidents");
        Assert.Equal(one.incidents, three.incidents);
        Assert.Equal(one.fines * 3, three.fines);
    }

    // ---- save, load, migration

    [Fact]
    public void Era_rules_are_config_only_and_never_written_into_a_save()
    {
        Game g = StartedIn("punchline");
        Assert.DoesNotContain("hireFeePermille", g.Sim.Save());
        Assert.DoesNotContain("minReleaseAlignment", g.Sim.Save());
    }

    [Fact]
    public void A_game_saved_before_an_era_turns_its_rules_loads_and_plays_the_same_across_the_turn()
    {
        Game original = StartedIn("punchline", solo: false, seed: 5);
        original.Autopilot(48 * 2);
        string save = original.Sim.Save();
        var loaded = new Game(Simulation.Load(original.C, save));
        Assert.Equal(original.Sim.Digest(), loaded.Sim.Digest());

        int toTheTurn = Tick(original.C, 2012) - original.Tick + 4;
        original.Weeks(toTheTurn);
        loaded.Weeks(toTheTurn);
        Assert.Equal("it-sees", original.Sim.Era.Id);
        Assert.Equal(original.Sim.Digest(), loaded.Sim.Digest());
    }

    [Fact]
    public void An_old_schema_1_save_still_loads_and_runs_under_the_era_rules()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema1Save());
        Assert.Equal("dial-up-days", sim.Era.Id);
        Assert.Equal(0, EraRules.ReleaseBar(sim.Config, sim.Tick));
        sim.StepMany(48 * 3);
        Assert.Equal(sim.Digest(), Simulation.Load(sim.Config, sim.Save()).Digest());
    }

    [Fact]
    public void A_game_started_in_2009_replays_through_the_talent_war_to_the_same_digest()
    {
        Game g = StartedIn("punchline", solo: false, seed: 3);
        g.Autopilot(48 * 5);
        ReplayLog log = ReplayLog.FromJson(g.Sim.Journal.ToJson());
        Assert.Equal(g.Sim.Digest(), Simulation.Replay(TestData.Config(), log).Digest());
    }

    // ---- across systems

    [Fact]
    public void A_rival_that_cannot_meet_the_law_holds_its_models_back_and_says_why_while_a_careful_one_ships()
    {
        Game g = StartedIn("punchline", solo: false, seed: 2, tweak: c => { foreach (EraConfig e in c.Eras.Skip(1)) e.MinReleaseAlignment = 900; });
        g.Years(3);
        LabState careless = g.Rival("openaye");
        LabState careful = g.Rival("anthoptics");
        Assert.DoesNotContain(careless.Models, m => m.Status == ModelStatus.Released);
        Assert.Contains(careless.Log, e => e.Action == DecisionAction.HeldBack && e.Reasons.Any(r => r.Code == ReasonCode.ReleaseGate && r.Value == 900));
        Assert.Contains(careful.Models, m => m.Status == ModelStatus.Released);
        Assert.All(careful.Models.Where(m => m.Status == ModelStatus.Released), m => Assert.True(m.Alignment >= 900));
    }

    [Fact]
    public void The_planner_hires_with_the_dearer_fee_in_mind_so_a_talent_war_slows_a_labs_growth()
    {
        Func<int, int> staffAfterTwoYears = fee =>
        {
            Game g = StartedIn("punchline", solo: false, seed: 6, tweak: c => { foreach (EraConfig e in c.Eras.Skip(1)) e.HireFeePermille = fee; });
            g.Years(2);
            return LabQueries.TotalStaff(g.Rival("xay"));
        };
        int cheap = staffAfterTwoYears(1000);
        int dear = staffAfterTwoYears(5000);
        Assert.True(cheap > 0);
        Assert.True(dear <= cheap, "a lab does not out-hire itself when hiring is dearer: " + dear + " vs " + cheap);
    }

    [Fact]
    public void Forty_years_of_play_never_trip_the_law_into_a_stuck_economy()
    {
        Game g = Game.New(seed: 3);
        g.Autopilot(48 * 40);
        Assert.Empty(ConfigValidator.Problems(g.C));
        Assert.True(g.Me.Techs.Count > 40);
        Assert.Equal("upward", g.Sim.Era.Id);
    }
}
