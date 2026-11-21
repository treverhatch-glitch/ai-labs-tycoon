using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Eras are data: a table of start years, what each opens, and what it costs to work in it.</summary>
public class EraTests
{
    static Game StartedIn(string eraId, long seed = 1, Action<GameConfig> tweak = null)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = eraId, RivalIds = new List<string>() };
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
    public void The_era_table_is_the_one_in_the_design()
    {
        GameConfig c = TestData.Config();
        var expected = new (string id, int year)[]
        {
            ("dial-up-days", 1995), ("punchline", 2009), ("it-sees", 2012), ("it-writes", 2017), ("chat-boom", 2022),
            ("agents", 2024), ("harnesses", 2026), ("bodies", 2029), ("upward", 2033),
        };
        Assert.Equal(expected.Select(e => e.id).ToArray(), c.Eras.Select(e => e.Id).ToArray());
        Assert.Equal(expected.Select(e => e.year).ToArray(), c.Eras.Select(e => e.StartYear).ToArray());
    }

    [Fact]
    public void Costs_and_research_speed_only_grow_from_era_to_era()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1500, c.Eras[0].TrainingCostPermille);
        Assert.Equal(1000, c.Eras[0].ResearchPermille);
        for (int i = 1; i < c.Eras.Count; i++)
        {
            Assert.True(c.Eras[i].TrainingCostPermille > c.Eras[i - 1].TrainingCostPermille, c.Eras[i].Id + " should cost more to train in");
            Assert.True(c.Eras[i].ResearchPermille > c.Eras[i - 1].ResearchPermille, c.Eras[i].Id + " should research faster");
        }
    }

    [Fact]
    public void The_last_two_eras_are_expansions_and_two_eras_can_be_started_in()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "bodies", "upward" }, c.Eras.Where(e => e.Expansion).Select(e => e.Id).ToArray());
        Assert.Equal(new[] { "dial-up-days", "punchline" }, c.Eras.Where(e => e.Startable).Select(e => e.Id).ToArray());
    }

    [Fact]
    public void Every_era_has_something_to_research_build_sell_and_say()
    {
        GameConfig c = TestData.Config();
        foreach (EraConfig era in c.Eras)
        {
            Assert.Contains(c.Research.Techs, t => t.Era == era.Id);
            Assert.Contains(c.Market.Sectors, s => s.Era == era.Id);
            Assert.Contains(c.RoomTypes, r => r.Era == era.Id);
            Assert.True(era.IncidentHeadlines.Count >= 5, era.Id + " needs a few incident headlines so they do not repeat at once");
            Assert.True(c.WorldEvents.Count(w => w.Year >= era.StartYear && (era.Id == c.Eras.Last().Id || w.Year < c.Eras[c.Eras.IndexOf(era) + 1].StartYear)) >= 2, era.Id + " needs world events");
        }
    }

    [Fact]
    public void Capability_bonuses_step_up_with_each_era()
    {
        GameConfig c = TestData.Config();
        long previousBonus = 0;
        foreach (EraConfig era in c.Eras)
        {
            List<TechConfig> techs = c.Research.Techs.Where(t => t.Era == era.Id).ToList();
            long bonus = techs.SelectMany(t => t.Effects).Where(e => e.Kind == TechEffectKind.CapabilityPermille).Sum(e => (long)e.Amount);
            Assert.True(bonus >= previousBonus, era.Id + " should add at least as much capability as the era before");
            previousBonus = bonus;
        }
    }

    [Fact]
    public void Every_sector_can_be_reached_with_the_techs_of_its_own_era()
    {
        // A flagship line settles near base x (1 + bonus) / (1 - carryover). A sector's bar must sit under
        // 80% of that with the techs researchable by the time the sector opens, or it would never open in time.
        GameConfig c = TestData.Config();
        int flagship = c.Line("flagship").BaseCapability;
        int carry = c.Training.CarryoverPermille;
        foreach (SectorConfig sector in c.Market.Sectors)
        {
            int eraIndex = c.Eras.IndexOf(c.Era(sector.Era));
            long bonus = c.Research.Techs.Where(t => c.Eras.IndexOf(c.Era(t.Era)) <= eraIndex)
                .SelectMany(t => t.Effects).Where(e => e.Kind == TechEffectKind.CapabilityPermille).Sum(e => (long)e.Amount);
            long plateau = flagship * (1000 + bonus) / 1000 * 1000 / (1000 - carry);
            Assert.True(sector.MinCapability * 10 <= plateau * 8, sector.Id + " asks " + sector.MinCapability + " but its era tops out near " + plateau);
            Assert.True(sector.SaturationCapability > sector.MinCapability, sector.Id + " saturates below its minimum");
        }
    }

    [Fact]
    public void Compute_rooms_pack_more_gpus_per_tile_and_get_cheaper_per_slot_as_eras_pass()
    {
        GameConfig c = TestData.Config();
        var racks = c.RoomTypes.Where(r => r.GpuSlotsPerTile > 0).OrderBy(r => c.Eras.IndexOf(c.Era(r.Era))).ThenBy(r => r.GpuSlotsPerTile).ToList();
        Assert.True(racks.Count >= 6);
        for (int i = 1; i < racks.Count; i++)
        {
            Assert.True(racks[i].GpuSlotsPerTile >= racks[i - 1].GpuSlotsPerTile, racks[i].Id + " should hold at least as many GPUs per tile as " + racks[i - 1].Id);
        }
    }

    [Fact]
    public void The_shipped_content_uses_parody_names_only()
    {
        string json = TestData.ConfigJson();
        string[] real =
        {
            "OpenAI", "Anthropic", "DeepMind", "Google", "Microsoft", "Meta ", "Nvidia", "NVIDIA", "Tesla", "SpaceX", "Amazon", "Apple",
            "Altman", "Amodei", "Musk", "Huang", "Hassabis", "Zuckerberg", "Sutskever", "Hinton", "LeCun",
            "ChatGPT", "GPT-", "Claude\"", "Gemini", "Grok\"", "Llama", "Copilot", "Midjourney", "Siri", "Alexa", "Waymo",
            "Netflix", "YouTube", "Facebook", "Twitter", "Super Bowl", "Pokemon", "Nintendo",
        };
        foreach (string name in real) Assert.DoesNotContain(name, json);
    }

    // ---- the validator

    [Fact]
    public void The_validator_catches_eras_out_of_order_or_wrongly_started()
    {
        AssertProblem(Problems(c => c.Eras[1].StartYear = c.Eras[0].StartYear), "does not start after the era before it");
        AssertProblem(Problems(c => c.Eras[0].StartYear = 1990), "calendar.startYear");
        AssertProblem(Problems(c => c.Eras[0].Startable = false), "must be startable");
        AssertProblem(Problems(c => c.Eras[0].StartTechs.Add("minimax")), "no start techs");
        AssertProblem(Problems(c => c.Eras[2].TrainingCostPermille = 10), "training cheaper");
        AssertProblem(Problems(c => c.Eras[2].ResearchPermille = 10), "research slower");
        AssertProblem(Problems(c => c.Eras[1].StartLotId = "moon-base"), "unknown lot");
        AssertProblem(Problems(c => c.Eras[1].IncidentHeadlines.Clear()), "no incident headlines");
        AssertProblem(Problems(c => c.Eras[1].IncidentHeadlines[0] = "Something happened."), "without {model}");
        AssertProblem(Problems(c => c.Eras[1].Unlocks.Clear()), "unlocks");
    }

    [Fact]
    public void The_validator_catches_content_in_the_wrong_era()
    {
        AssertProblem(Problems(c => c.Research.Techs[0].Era = "stone-age"), "unknown era");
        AssertProblem(Problems(c => c.RoomTypes[0].Era = "stone-age"), "unknown era");
        AssertProblem(Problems(c => c.Market.Sectors[0].Era = "stone-age"), "unknown era");
        AssertProblem(Problems(c => c.Tech("minimax").Requires.Add("attention-arch")), "later era");
        AssertProblem(Problems(c => c.Sector("web-ads").OpensYear = 2015), "outside its era");
        AssertProblem(Problems(c => c.Sector("web-ads").GrowthDecayPermille = 0), "growthDecayPermille");
    }

    [Fact]
    public void The_validator_catches_an_era_with_nothing_in_it()
    {
        AssertProblem(Problems(c => c.Research.Techs.RemoveAll(t => t.Era == "upward")), "has no techs");
        List<string> noSectors = Problems(c =>
        {
            c.Market.Sectors.RemoveAll(s => s.Era == "upward");
            c.WorldEvents.RemoveAll(w => w.SectorId == "machine-science" || w.SectorId == "space-compute");
        });
        AssertProblem(noSectors, "no sectors");
    }

    [Fact]
    public void The_validator_catches_a_start_kit_that_does_not_hang_together()
    {
        AssertProblem(Problems(c => c.Era("punchline").StartTechs.Remove("state-machines")), "prerequisite");
        AssertProblem(Problems(c => c.Era("punchline").StartTechs.Add("conv-nets")), "later era");
        AssertProblem(Problems(c => c.Era("punchline").StartTechs.Add("moon-math")), "moon-math");
    }

    [Fact]
    public void The_validator_catches_bad_world_events()
    {
        AssertProblem(Problems(c => c.WorldEvents[0].Headline = ""), "no headline");
        AssertProblem(Problems(c => c.WorldEvents[0].Month = 13), "month");
        AssertProblem(Problems(c => c.WorldEvents[0].Year = 1990), "before the game begins");
        AssertProblem(Problems(c => c.WorldEvents[0].PoolPermille = 500), "poolPermille must be 1000");
        AssertProblem(Problems(c => c.WorldEvent("bubble-pops").SectorId = "moon-mining"), "moon-mining");
        AssertProblem(Problems(c => c.WorldEvent("bubble-pops").PoolPermille = 5000), "100 to 3000");
        AssertProblem(Problems(c => c.WorldEvent("bubble-pops").Year = 1996), "opens");
        AssertProblem(Problems(c =>
        {
            WorldEventConfig first = c.WorldEvents[0];
            c.WorldEvents.RemoveAt(0);
            c.WorldEvents.Add(first);
        }), "date order");
    }

    // ---- era gates

    [Fact]
    public void A_tech_of_a_later_era_is_refused_until_its_era_begins()
    {
        Game g = Game.Solo();
        g.GrantResearch(1000000);
        g.Refused(Command.ResearchTech(g.Id, "big-datasets"), CommandError.EraLocked);
        Assert.False(LabQueries.CanResearch(g.C, g.Me, g.C.Tech("big-datasets"), g.Sim.Tick));
    }

    [Fact]
    public void A_tech_can_be_researched_once_its_era_has_begun()
    {
        Game g = StartedIn("punchline");
        g.GrantResearch(10000000);
        g.Research("backprop-tricks");
        g.Research("digit-readers");
        g.Research("gpu-clusters");
        g.Refused(Command.ResearchTech(g.Id, "conv-nets"), CommandError.EraLocked);

        g.Until(() => g.Sim.Era.Id == "it-sees", 4 * 48, "the It Sees era");
        Assert.Equal(2012, g.Sim.Year);
        g.Do(Command.ResearchTech(g.Id, "conv-nets"));
        Assert.Contains("conv-nets", g.Me.Techs);
    }

    [Fact]
    public void The_refusal_names_the_era_and_the_year()
    {
        Game g = Game.Solo();
        CommandResult result = g.Try(Command.BuildRoom(g.Id, "gpu-hall", 0, 0, 8, 8));
        Assert.False(result.Ok);
        Assert.Equal(CommandError.EraLocked, result.Error);
        Assert.Contains("2009", result.Message);
    }

    [Fact]
    public void A_room_type_of_a_later_era_cannot_be_built_until_its_era_begins()
    {
        Game g = Game.Solo(tweak: c => c.PlayerStart.Cash = 50000000);
        RoomTypeConfig hall = g.C.RoomType("gpu-hall");
        g.Refused(Command.BuildRoom(g.Id, hall.Id, 0, 0, hall.MinWidth, hall.MinHeight), CommandError.EraLocked);

        Game later = StartedIn("punchline");
        later.Do(Command.BuyLot(later.Id));
        Assert.True(later.Me.Cash >= hall.BuildCostPerTile * hall.MinWidth * hall.MinHeight);
        later.Build(hall.Id, hall.MinWidth, hall.MinHeight);
        Assert.Contains(later.Me.Rooms, r => r.TypeId == "gpu-hall");
    }

    [Fact]
    public void Training_costs_more_in_a_later_era()
    {
        Game early = Game.Solo(tweak: c => c.PlayerStart.Cash = 5000000);
        early.WithStarterLab();
        early.GrantResearch(0);
        ModelLineConfig mini = early.C.Line("mini");
        long before = early.Me.Cash;
        ModelState a = early.Train("mini", 0);
        long earlyCost = before - early.Me.Cash;
        EraConfig earlyEra = early.C.Eras[0];
        Assert.Equal(mini.PrepCost * earlyEra.TrainingCostPermille / 1000, earlyCost);
        Assert.Equal(mini.ComputeRequired * earlyEra.TrainingCostPermille / 1000, a.RequiredCompute);

        Game later = StartedIn("punchline");
        later.WithStarterLab();
        long laterBefore = later.Me.Cash;
        ModelState b = later.Train("mini", 0);
        EraConfig era = later.C.Era("punchline");
        Assert.Equal(mini.PrepCost * era.TrainingCostPermille / 1000, laterBefore - later.Me.Cash);
        Assert.Equal(mini.ComputeRequired * era.TrainingCostPermille / 1000, b.RequiredCompute);
    }

    [Fact]
    public void Research_runs_faster_in_a_later_era()
    {
        Game early = Game.Solo();
        early.Build("research-lab", 4, 4);
        early.Hire("researcher", 2);
        long start = early.Me.ResearchPoints;
        early.Weeks(1);
        long earlyGain = early.Me.ResearchPoints - start;

        Game later = StartedIn("punchline");
        later.Build("research-lab", 4, 4);
        later.Hire("researcher", 2);
        long laterStart = later.Me.ResearchPoints;
        later.Weeks(1);
        long laterGain = later.Me.ResearchPoints - laterStart;

        Assert.True(earlyGain > 0);
        Assert.Equal(earlyGain * later.C.Era("punchline").ResearchPermille / 1000, laterGain);
    }

    // ---- the start-era picker

    [Fact]
    public void Starting_in_the_default_era_is_the_same_game_as_naming_no_era()
    {
        string plain = Simulation.NewGame(TestData.Config(), 5).Digest();
        var named = new NewGameOptions { Seed = 5, StartEraId = "dial-up-days" };
        Assert.Equal(plain, Simulation.NewGame(TestData.Config(), named).Digest());
    }

    [Fact]
    public void A_game_started_in_2009_begins_in_2009_with_the_era_start_kit()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "punchline" };
        Simulation sim = Simulation.NewGame(TestData.Config(), options);
        GameConfig c = sim.Config;
        EraConfig era = c.Era("punchline");

        Assert.Equal(2009, sim.Year);
        Assert.Equal(era.Id, sim.Era.Id);
        Assert.Equal(EraSystem.StartTick(c, era), sim.Tick);
        Assert.Equal(c.PlayerStart.Cash * era.StartCashPermille / 1000, sim.Player.Cash);
        Assert.Equal(era.StartLotId, sim.Player.LotId);
        foreach (LabState lab in sim.State.Labs)
        {
            Assert.All(era.StartTechs, t => Assert.Contains(t, lab.Techs));
            Assert.True(lab.PlanStartTick >= sim.Tick, lab.Name + " must not act on a plan that started before the game did");
        }
        int startYear = GameDate.Year(c.Calendar, sim.Tick);
        Assert.Equal(c.Rivals.Count(r => r.JoinsYear <= startYear) + 1, sim.State.Labs.Count);
    }

    [Fact]
    public void A_start_era_game_announces_its_era_and_skips_news_from_before_it()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "punchline" };
        Simulation sim = Simulation.NewGame(TestData.Config(), options);
        sim.StepMany(60);
        Assert.Contains(sim.State.Events, e => e.Kind == EventKind.EraBegan && e.Ref == "punchline");
        Assert.DoesNotContain(sim.State.Events, e => e.Kind == EventKind.WorldNews && e.Ref == "bubble-pops");
        Assert.Contains(sim.State.Events, e => e.Kind == EventKind.WorldNews && e.Ref == "photo-benchmark");
    }

    [Fact]
    public void Sectors_that_opened_before_the_start_era_have_grown_to_that_year()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "punchline" };
        Simulation late = Simulation.NewGame(TestData.Config(), options);
        Simulation early = Simulation.NewGame(TestData.Config(), 1);
        Assert.True(late.State.Sector("spam-search").PoolPerWeek > early.State.Sector("spam-search").PoolPerWeek);
        Assert.Equal(late.Config.Sector("doc-reading").PoolPerWeek, late.State.Sector("doc-reading").PoolPerWeek);
    }

    [Fact]
    public void An_unknown_or_unstartable_era_is_refused_with_a_clear_message()
    {
        GameConfig c = TestData.Config();
        var unknown = new NewGameOptions { Seed = 1, StartEraId = "stone-age" };
        Assert.Contains("stone-age", Assert.Throws<ArgumentException>(() => Simulation.NewGame(c, unknown)).Message);
        var locked = new NewGameOptions { Seed = 1, StartEraId = "bodies" };
        Assert.Contains("Bodies", Assert.Throws<ArgumentException>(() => Simulation.NewGame(c, locked)).Message);
    }

    [Fact]
    public void A_start_era_game_saves_loads_and_replays_the_same()
    {
        var options = new NewGameOptions { Seed = 3, StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        g.Autopilot(48 * 3);

        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        loaded.StepMany(48);
        g.Weeks(48);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());

        ReplayLog log = ReplayLog.FromJson(g.Sim.Journal.ToJson());
        Assert.Equal(g.Sim.Digest(), Simulation.Replay(TestData.Config(), log).Digest());
    }

    [Fact]
    public void The_start_kit_leaves_the_eras_own_techs_to_research()
    {
        Game g = StartedIn("punchline");
        g.GrantResearch(1000000);
        TechConfig next = g.C.Research.Techs.First(t => t.Era == "punchline" && !g.Me.Techs.Contains(t.Id) && t.Requires.All(r => g.Me.Techs.Contains(r)));
        g.Research(next.Id);
        Assert.Contains(next.Id, g.Me.Techs);
    }

    // ---- sector pools

    [Fact]
    public void A_sector_pool_does_not_grow_before_it_opens_and_then_slows_every_year()
    {
        GameConfig c = TestData.Config();
        SectorConfig sector = c.Sector("web-ads");
        Assert.Equal(sector.GrowthPermillePerYear, EraSystem.SectorGrowthPermille(sector, sector.OpensYear));
        int second = EraSystem.SectorGrowthPermille(sector, sector.OpensYear + 1);
        int third = EraSystem.SectorGrowthPermille(sector, sector.OpensYear + 2);
        Assert.Equal(sector.GrowthPermillePerYear * sector.GrowthDecayPermille / 1000, second);
        Assert.True(third < second && second < sector.GrowthPermillePerYear);

        SectorState fresh = WorldSetup.NewSector(sector, sector.OpensYear);
        Assert.Equal(sector.PoolPerWeek, fresh.PoolPerWeek);
        SectorState older = WorldSetup.NewSector(sector, sector.OpensYear + 3);
        Assert.True(older.PoolPerWeek > fresh.PoolPerWeek);
    }

    [Fact]
    public void A_sector_that_has_not_opened_keeps_its_listed_pool_until_the_year_it_opens()
    {
        Game g = Game.Solo();
        SectorConfig translation = g.C.Sector("machine-translation");
        g.Until(() => g.Sim.Year == translation.OpensYear, 48 * 8, "the sector's opening year");
        Assert.Equal(translation.PoolPerWeek, g.State.Sector(translation.Id).PoolPerWeek);
    }

    [Fact]
    public void A_pool_growth_never_runs_away_over_forty_years()
    {
        Game g = Game.Solo();
        g.Years(40);
        foreach (SectorState sector in g.State.Sectors)
        {
            SectorConfig config = g.C.Sector(sector.Id);
            Assert.True(sector.PoolPerWeek >= config.PoolPerWeek / 2, sector.Id + " collapsed");
            Assert.True(sector.PoolPerWeek <= config.PoolPerWeek * 40, sector.Id + " grew past forty times its opening pool");
        }
    }

    // ---- world events

    [Fact]
    public void A_world_event_lands_once_in_its_month_and_moves_its_sector()
    {
        Game g = Game.Solo();
        WorldEventConfig pop = g.C.WorldEvent("bubble-pops");
        int tick = EraSystem.EventTick(g.C, pop);
        g.Until(() => g.Sim.Tick == tick, 48 * 8, "the month of the bubble");
        long before = g.State.Sector("spam-search").PoolPerWeek;
        g.Weeks(1);
        long after = g.State.Sector("spam-search").PoolPerWeek;
        Assert.Equal(before * pop.PoolPermille / 1000, after);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.WorldNews && e.Ref == "bubble-pops"));

        g.Weeks(60);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.WorldNews && e.Ref == "bubble-pops"));
    }

    [Fact]
    public void World_events_and_eras_read_as_sentences()
    {
        Game g = Game.Solo();
        g.Years(15);
        GameEvent news = g.State.Events.First(e => e.Kind == EventKind.WorldNews);
        string line = EventText.Describe(g.C, g.State, news);
        Assert.Contains(g.C.WorldEvent(news.Ref).Headline, line);
        GameEvent era = g.State.Events.First(e => e.Kind == EventKind.EraBegan);
        Assert.Contains(g.C.Era(era.Ref).Name, EventText.Describe(g.C, g.State, era));
    }

    [Fact]
    public void World_events_do_not_touch_the_dice()
    {
        // Only the flavour-only events differ; the rivals and the dice must not notice.
        Game with = Game.New(seed: 4);
        Game without = Game.New(seed: 4, tweak: c => c.WorldEvents.RemoveAll(w => w.SectorId == ""));
        with.Years(6);
        without.Years(6);
        Assert.Equal(with.State.Rng.Training.State, without.State.Rng.Training.State);
        Assert.Equal(with.State.Rng.Safety.State, without.State.Rng.Safety.State);
        Assert.Equal(with.State.Rng.Ranking.State, without.State.Rng.Ranking.State);
        Assert.Equal(with.State.Rng.Rivals.State, without.State.Rng.Rivals.State);
        Assert.Equal(with.Me.Cash, without.Me.Cash);
    }

    // ---- the planner and the eras

    [Fact]
    public void A_rival_that_owns_gpus_racks_them_in_the_densest_room_its_era_allows()
    {
        Action<GameConfig> startInSeesEra = c =>
        {
            EraConfig sees = c.Era("it-sees");
            sees.Startable = true;
            sees.StartLotId = "strip-mall";
            sees.StartCashPermille = 40000;
            sees.StartGrantPermille = 20000;
            sees.StartTechs.AddRange(c.Era("punchline").StartTechs);
            foreach (TechConfig t in c.Research.Techs) if (t.Era == "punchline") sees.StartTechs.Add(t.Id);
        };
        var options = new NewGameOptions { Seed = 1, StartEraId = "it-sees" };
        Simulation sim = Simulation.NewGame(TestData.Config(startInSeesEra), options);
        for (int i = 0; i < 48 * 2; i++)
        {
            if (GameDate.IsMonthStart(sim.Config.Calendar, sim.Tick)) sim.RunPlan(sim.Player.Id, sim.Config.Autopilot);
            sim.Step();
        }
        LabState xay = sim.State.Labs.First(l => l.RivalId == "xay");
        Assert.DoesNotContain(xay.Rooms, r => r.TypeId == "server-closet");
        Assert.Contains(xay.Rooms, r => sim.Config.RoomType(r.TypeId).GpuSlotsPerTile > sim.Config.RoomType("server-closet").GpuSlotsPerTile);
    }
}
