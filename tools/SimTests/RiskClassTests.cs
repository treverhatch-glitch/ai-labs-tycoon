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

/// <summary>Q109: each era brings its own kinds of trouble, and what a lab sells decides which of them can hit it.</summary>
public class RiskClassTests
{
    static GameConfig Config() => TestData.Config();

    static int TickIn(GameConfig c, int year) => GameDate.TickOfYear(c.Calendar, year);

    /// <summary>A lone lab in an era with one released model on sale in the given sector, and money enough that fines never bankrupt it.</summary>
    static (Game game, ModelState model) Selling(string eraId, string sector, long seed = 1, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = eraId, RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(tweak), options));
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, sector));
        model.Alignment = 300;
        model.DisplayedAlignment = 300;
        g.Me.Cash = 1_000_000_000_000_000;
        return (g, model);
    }

    static List<GameEvent> Incidents(Game g) => g.State.Events.Where(e => e.Kind == EventKind.Incident).ToList();

    static List<string> Problems(Action<GameConfig> damage)
    {
        GameConfig c = Config();
        damage(c);
        return ConfigValidator.Problems(c);
    }

    static void AssertProblem(List<string> problems, string fragment)
    {
        Assert.True(problems.Any(p => p.Contains(fragment)), "expected a problem containing \"" + fragment + "\" but got:\n- " + string.Join("\n- ", problems));
    }

    // ---- the table

    [Fact]
    public void There_is_a_class_of_trouble_for_every_era_from_it_sees_on_and_none_before()
    {
        GameConfig c = Config();
        Assert.Equal(new[] { "bias", "autonomous-crash", "misuse", "data-leak", "agent-mishap", "runaway-agents", "physical-harm", "loss-of-control" },
            c.Safety.RiskClasses.Select(r => r.Id).ToArray());
        Assert.Empty(RiskClasses.OpeningIn(c, c.Era("dial-up-days")));
        Assert.Empty(RiskClasses.OpeningIn(c, c.Era("punchline")));
        foreach (EraConfig era in c.Eras.Skip(2)) Assert.NotEmpty(RiskClasses.OpeningIn(c, era));
        Assert.Equal(new[] { "bias", "autonomous-crash" }, RiskClasses.OpeningIn(c, c.Era("it-sees")).Select(r => r.Id).ToArray());
    }

    [Fact]
    public void Each_class_names_real_sectors_and_a_headline_for_the_model()
    {
        GameConfig c = Config();
        foreach (RiskClassConfig r in c.Safety.RiskClasses)
        {
            Assert.NotEmpty(r.Sectors);
            foreach (string s in r.Sectors) Assert.NotNull(c.Sector(s));
            Assert.True(r.Headlines.Count >= 3, r.Id);
            Assert.All(r.Headlines, h => Assert.Contains("{model}", h));
        }
    }

    [Fact]
    public void Classes_open_with_their_era_and_stay_open()
    {
        GameConfig c = Config();
        Assert.Empty(RiskClasses.Open(c, TickIn(c, 2010)));
        Assert.Equal(new[] { "bias", "autonomous-crash" }, RiskClasses.Open(c, TickIn(c, 2013)).Select(r => r.Id).ToArray());
        Assert.Equal(new[] { "bias", "autonomous-crash", "misuse" }, RiskClasses.Open(c, TickIn(c, 2018)).Select(r => r.Id).ToArray());
        Assert.Equal(c.Safety.RiskClasses.Count, RiskClasses.Open(c, TickIn(c, 2034)).Count);
        Assert.False(RiskClasses.IsOpen(c, c.Safety.RiskClasses.First(r => r.Id == "data-leak"), TickIn(c, 2021)));
        Assert.True(RiskClasses.IsOpen(c, c.Safety.RiskClasses.First(r => r.Id == "data-leak"), TickIn(c, 2022)));
    }

    // ---- what bites

    [Fact]
    public void A_model_is_exposed_only_to_the_classes_of_the_sectors_it_is_sold_in()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant");
        GameConfig c = g.C;
        Assert.Equal(new[] { "misuse", "data-leak" }, RiskClasses.Biting(c, g.State, g.Me, model).Select(r => r.Id).ToArray());
        Assert.False(RiskClasses.Exposed(g.State, g.Me, model, c.Safety.RiskClasses.First(r => r.Id == "bias")));

        var (g2, model2) = Selling("chat-boom", "image-design");
        Assert.Equal(new[] { "misuse" }, RiskClasses.Biting(g2.C, g2.State, g2.Me, model2).Select(r => r.Id).ToArray());
    }

    [Fact]
    public void A_class_that_has_not_opened_never_bites_even_when_the_model_is_in_its_sector()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant");
        g.State.Sector("coding-agents").Entries.Add(new SectorEntry { LabId = g.Id, ModelId = model.Id, SharePpm = 1 });
        RiskClassConfig mishap = g.C.Safety.RiskClasses.First(r => r.Id == "agent-mishap");
        Assert.True(RiskClasses.Exposed(g.State, g.Me, model, mishap));
        Assert.False(RiskClasses.IsOpen(g.C, mishap, g.Tick));
        Assert.DoesNotContain("agent-mishap", RiskClasses.Biting(g.C, g.State, g.Me, model).Select(r => r.Id));
    }

    [Fact]
    public void A_model_not_on_sale_anywhere_is_never_exposed()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant");
        foreach (SectorState s in g.State.Sectors) s.Entries.RemoveAll(e => e.LabId == g.Id);
        Assert.Empty(RiskClasses.Biting(g.C, g.State, g.Me, model));
    }

    [Fact]
    public void A_class_with_no_sectors_listed_bites_anything_on_sale()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant", tweak: c => c.Safety.RiskClasses.First(r => r.Id == "misuse").Sectors.Clear());
        Assert.Contains("misuse", RiskClasses.Biting(g.C, g.State, g.Me, model).Select(r => r.Id));
        foreach (SectorState s in g.State.Sectors) s.Entries.RemoveAll(e => e.LabId == g.Id);
        Assert.Empty(RiskClasses.Biting(g.C, g.State, g.Me, model));
    }

    // ---- the draw

    [Fact]
    public void Nothing_is_drawn_from_the_random_stream_when_no_class_bites()
    {
        var g = Game.Solo().WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        g.Do(Command.ReleaseModel(g.Id, model.Id, "game-ai"));
        string before = g.Sim.Digest();
        for (int i = 0; i < 20; i++) Assert.Null(RiskClasses.Draw(g.C, g.State, g.Me, model));
        Assert.Equal(before, g.Sim.Digest());
    }

    [Fact]
    public void The_draw_gives_the_plain_mishap_and_the_class_in_the_proportion_of_their_weights()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant");
        int generic = 0, misuse = 0;
        for (int i = 0; i < 4000; i++)
        {
            RiskClassConfig drawn = RiskClasses.Draw(g.C, g.State, g.Me, model);
            if (drawn == null) generic++; else { Assert.Contains(drawn.Id, new[] { "misuse", "data-leak" }); misuse++; }
        }
        Assert.InRange(generic, 1000, 1700);
        Assert.Equal(4000, generic + misuse);
    }

    [Fact]
    public void A_heavier_class_is_drawn_more_often_and_a_zero_chance_for_the_plain_mishap_cannot_be_configured()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant", tweak: c => c.Safety.RiskClasses.First(r => r.Id == "misuse").Weight = 9000);
        int generic = 0;
        for (int i = 0; i < 4000; i++) if (RiskClasses.Draw(g.C, g.State, g.Me, model) == null) generic++;
        Assert.InRange(generic, 250, 550);
        AssertProblem(Problems(c => c.Safety.GenericRiskWeight = 0), "genericRiskWeight");
    }

    [Fact]
    public void The_same_seed_draws_the_same_trouble()
    {
        string Trail(long seed)
        {
            var (g, _) = Selling("chat-boom", "consumer-assistant", seed, c => c.Safety.IncidentChancePermille = 1000);
            g.Weeks(30);
            return string.Join(",", Incidents(g).Select(e => e.RiskClass + "/" + e.Pick + "/" + e.Amount));
        }
        Assert.Equal(Trail(5), Trail(5));
        Assert.NotEqual(Trail(5), Trail(6));
    }

    // ---- what an incident does

    [Fact]
    public void An_incident_in_a_class_sector_is_sometimes_that_class_and_always_has_a_headline_that_fits()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant", tweak: c => c.Safety.IncidentChancePermille = 1000);
        g.Weeks(40);
        List<GameEvent> incidents = Incidents(g);
        Assert.True(incidents.Count >= 20);
        Assert.Contains(incidents, e => e.RiskClass == "misuse");
        Assert.Contains(incidents, e => e.RiskClass == "");
        RiskClassConfig misuse = g.C.Safety.RiskClasses.First(r => r.Id == "misuse");
        foreach (GameEvent e in incidents.Where(e => e.RiskClass == "misuse"))
        {
            Assert.InRange(e.Pick, 0, misuse.Headlines.Count - 1);
            string line = EventText.Describe(g.C, g.State, e);
            Assert.Contains(misuse.Headlines[e.Pick].Replace("{model}", model.Name), line);
        }
        foreach (GameEvent e in incidents.Where(e => e.RiskClass == ""))
            Assert.Contains(g.C.Era("chat-boom").IncidentHeadlines[e.Pick].Replace("{model}", model.Name), EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_fine_scales_with_the_class_and_nothing_else_about_the_run_changes()
    {
        string[] Trail(int finePermille, out List<long> fines)
        {
            var (g, _) = Selling("chat-boom", "consumer-assistant", 2, c =>
            {
                c.Safety.IncidentChancePermille = 1000;
                c.Safety.RiskClasses.First(r => r.Id == "misuse").FinePermille = finePermille;
            });
            g.Weeks(25);
            fines = Incidents(g).Where(e => e.RiskClass == "misuse").Select(e => e.Amount).ToList();
            return Incidents(g).Select(e => e.RiskClass + "/" + e.Pick).ToArray();
        }

        string[] one = Trail(1000, out List<long> small);
        string[] two = Trail(2000, out List<long> big);
        Assert.Equal(one, two);
        Assert.NotEmpty(small);
        for (int i = 0; i < small.Count; i++) Assert.InRange(big[i], small[i] * 2 - 12, small[i] * 2 + 12);
    }

    [Fact]
    public void A_class_costs_more_reputation_when_its_multiplier_is_higher_and_the_plain_mishap_is_untouched()
    {
        int RepAfterFirstClassIncident(int permille)
        {
            var (g, _) = Selling("chat-boom", "consumer-assistant", 2, c =>
            {
                c.Safety.IncidentChancePermille = 1000;
                c.Safety.RiskClasses.First(r => r.Id == "misuse").ReputationPermille = permille;
            });
            for (int i = 0; i < 60 && !Incidents(g).Any(e => e.RiskClass == "misuse"); i++) g.Weeks(1);
            Assert.Contains(Incidents(g), e => e.RiskClass == "misuse");
            return g.Me.Reputation;
        }
        Assert.True(RepAfterFirstClassIncident(1000) > RepAfterFirstClassIncident(4000));
    }

    [Fact]
    public void A_class_incident_still_pays_the_eras_fine_on_top_of_the_class_fine()
    {
        Func<int, long> firstClassFine = hireFeeBlind =>
        {
            var (g, _) = Selling("chat-boom", "consumer-assistant", 2, c =>
            {
                c.Safety.IncidentChancePermille = 1000;
                c.Era("chat-boom").FinePermille = hireFeeBlind;
            });
            for (int i = 0; i < 60 && !Incidents(g).Any(e => e.RiskClass == "misuse"); i++) g.Weeks(1);
            return Incidents(g).First(e => e.RiskClass == "misuse").Amount;
        };
        long plain = firstClassFine(1000);
        long tripled = firstClassFine(3000);
        Assert.InRange(tripled, plain * 3 - 12, plain * 3 + 12);
    }

    [Fact]
    public void A_lab_with_no_model_on_sale_has_no_incidents_to_classify()
    {
        var g = Game.Solo(tweak: c => c.Safety.IncidentChancePermille = 1000).WithStarterLab();
        g.Weeks(20);
        Assert.Empty(Incidents(g));
    }

    [Fact]
    public void A_bankrupt_lab_has_no_more_incidents()
    {
        var (g, _) = Selling("chat-boom", "consumer-assistant", tweak: c => c.Safety.IncidentChancePermille = 1000);
        g.Me.Bankrupt = true;
        int before = Incidents(g).Count;
        g.Weeks(10);
        Assert.Equal(before, Incidents(g).Count);
    }

    // ---- the era banner

    [Fact]
    public void The_era_banner_announces_the_new_kinds_of_trouble_and_stays_quiet_when_there_are_none()
    {
        var g = Game.Solo();
        GameConfig c = g.C;
        string Banner(string era) => EventText.Describe(c, g.State, new GameEvent { Tick = TickIn(c, c.Era(era).StartYear), Kind = EventKind.EraBegan, Ref = era });
        string[] names(string era) => RiskClasses.OpeningIn(c, c.Era(era)).Select(r => r.Name.ToLowerInvariant()).ToArray();
        Assert.Contains("A new kind of trouble: " + string.Join(", ", names("it-sees")) + ".", Banner("it-sees"));
        Assert.Contains("A new kind of trouble: " + string.Join(", ", names("it-writes")) + ".", Banner("it-writes"));
        Assert.DoesNotContain("new kind of trouble", Banner("punchline"));
        Assert.DoesNotContain("new kind of trouble", Banner("dial-up-days"));
    }

    [Fact]
    public void An_incident_whose_class_has_gone_from_the_config_still_reads_with_the_eras_headline()
    {
        var g = Game.Solo();
        var e = new GameEvent { Tick = TickIn(g.C, 2024), Kind = EventKind.Incident, LabId = g.Id, Amount = 5, Ref = "Old", RiskClass = "retired-class", Pick = 0 };
        Assert.Contains("INCIDENT", EventText.Describe(g.C, g.State, e));
    }

    // ---- the validator

    [Fact]
    public void A_risk_class_that_could_not_work_is_a_config_problem()
    {
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].Weight = 0), "weight");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].Weight = 100001), "weight");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].FinePermille = 0), "fine");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].ReputationPermille = 20000), "reputation");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].Headlines[0] = "A model did something."), "{model}");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].Headlines.Clear()), "headline");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].Sectors.Add("moon-mining")), "moon-mining");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].Era = "stone-age"), "stone-age");
        AssertProblem(Problems(c => c.Safety.RiskClasses[0].Era = "dial-up-days"), "first era");
        AssertProblem(Problems(c => c.Safety.RiskClasses[1].IdValue = c.Safety.RiskClasses[0].IdValue), c0Id());
    }

    static string c0Id() => Config().Safety.RiskClasses[0].Id;

    [Fact]
    public void Classes_must_be_listed_oldest_era_first()
    {
        AssertProblem(Problems(c => c.Safety.RiskClasses.Reverse()), "oldest");
        Assert.Empty(Problems(c => c.Safety.RiskClasses.Clear()));
    }

    [Fact]
    public void The_extreme_weights_still_draw_without_overflowing()
    {
        var (g, model) = Selling("chat-boom", "consumer-assistant", tweak: c =>
        {
            c.Safety.GenericRiskWeight = 100000;
            c.Safety.RiskClasses.First(r => r.Id == "misuse").Weight = 100000;
        });
        for (int i = 0; i < 200; i++) RiskClasses.Draw(g.C, g.State, g.Me, model);
        Assert.Empty(ConfigValidator.Problems(g.C));
    }

    [Fact]
    public void The_risk_class_table_is_strict_json_and_round_trips()
    {
        GameConfig c = Config();
        GameConfig again = GameConfig.FromJson(c.ToJson());
        Assert.Equal(c.Safety.RiskClasses.Count, again.Safety.RiskClasses.Count);
        Assert.Equal(c.Safety.GenericRiskWeight, again.Safety.GenericRiskWeight);
        Assert.Equal(c.ToJson(), again.ToJson());

        string missing = TestData.ConfigJson().Replace("\"genericRiskWeight\"", "\"genericRiskWeightX\"");
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(missing));
    }

    // ---- saves

    [Fact]
    public void The_class_of_an_incident_survives_save_and_load()
    {
        var (g, _) = Selling("chat-boom", "consumer-assistant", tweak: c => c.Safety.IncidentChancePermille = 1000);
        g.Weeks(30);
        Assert.Contains(Incidents(g), e => e.RiskClass == "misuse");
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(Incidents(g).Select(e => e.RiskClass).ToArray(), again.State.Events.Where(e => e.Kind == EventKind.Incident).Select(e => e.RiskClass).ToArray());
    }

    [Fact]
    public void A_schema_20_save_gains_an_empty_class_on_every_event_and_plays_on()
    {
        var (g, _) = Selling("chat-boom", "consumer-assistant", tweak: c => c.Safety.IncidentChancePermille = 1000);
        g.Weeks(10);
        string old = SaveSchemaPretend.At(g.C, 20, g.Sim.Save());
        Assert.DoesNotContain("riskClass", old);

        Simulation migrated = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, migrated.State.Schema);
        Assert.All(migrated.State.Events, e => Assert.Equal("", e.RiskClass));
        Assert.Equal(g.State.Events.Count, migrated.State.Events.Count);
        migrated.StepMany(20);
        Assert.Empty(SaveGame.Problems(migrated.Config, migrated.State));
        Assert.Equal(migrated.Digest(), Simulation.Load(migrated.Config, migrated.Save()).Digest());
    }

    [Fact]
    public void The_oldest_saves_still_migrate_all_the_way_to_the_class_schema()
    {
        foreach (string save in new[] { MigrationTests.Schema1Save(), MigrationTests.Schema2Save() })
        {
            Simulation sim = Simulation.Load(Config(), save);
            Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
            Assert.All(sim.State.Events, e => Assert.Equal("", e.RiskClass));
            sim.StepMany(48);
            SaveSchemaPretend.AssertRoundTrip(sim);
        }
    }

    // ---- across systems

    [Fact]
    public void What_a_lab_sells_decides_which_trouble_it_gets()
    {
        var (leaky, _) = Selling("chat-boom", "consumer-assistant", tweak: c => c.Safety.IncidentChancePermille = 1000);
        leaky.Weeks(40);
        Assert.Contains(Incidents(leaky), e => e.RiskClass == "data-leak");
        Assert.Contains(Incidents(leaky), e => e.RiskClass == "misuse");

        var (art, _) = Selling("chat-boom", "image-design", tweak: c => c.Safety.IncidentChancePermille = 1000);
        art.Weeks(40);
        Assert.Contains(Incidents(art), e => e.RiskClass == "misuse");
        Assert.DoesNotContain(Incidents(art), e => e.RiskClass != "" && e.RiskClass != "misuse");
    }

    [Fact]
    public void Every_start_era_plays_a_year_with_classes_on_without_a_lab_failing()
    {
        foreach (string era in new[] { "it-sees", "chat-boom", "harnesses", "upward" })
        {
            var game = new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 3, StartEraId = era }));
            game.Autopilot(48 * 2);
            foreach (GameEvent e in game.State.Events.Where(e => e.Kind == EventKind.Incident))
                Assert.False(string.IsNullOrEmpty(EventText.Describe(game.C, game.State, e)));
            Assert.DoesNotContain(game.State.Labs, l => l.Bankrupt);
        }
    }
}
