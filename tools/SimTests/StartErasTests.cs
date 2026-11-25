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

/// <summary>NEXT item 12, rules half: a new game can begin in any era, with the cast, cash, lot and techs of that era.</summary>
public class StartErasTests
{
    static readonly string[] AllEras = { "dial-up-days", "punchline", "it-sees", "it-writes", "chat-boom", "agents", "harnesses", "bodies", "upward" };

    static Simulation Start(string eraId, long seed = 1, Action<GameConfig> tweak = null, bool rivals = true)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = eraId };
        if (!rivals) options.RivalIds = new List<string>();
        return Simulation.NewGame(TestData.Config(tweak), options);
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

    // ---- the picker's list

    [Fact]
    public void Every_era_is_offered_in_order_and_the_offer_carries_what_a_picker_shows()
    {
        GameConfig c = TestData.Config();
        List<StartEraOffer> offers = StartEras.Offers(c);
        Assert.Equal(AllEras, offers.Select(o => o.EraId).ToArray());
        for (int i = 0; i < offers.Count; i++)
        {
            Assert.Equal(c.Eras[i].Name, offers[i].Name);
            Assert.Equal(c.Eras[i].StartYear, offers[i].StartYear);
            Assert.False(string.IsNullOrEmpty(offers[i].Tagline));
            Assert.False(string.IsNullOrEmpty(offers[i].ModelsCanDo));
            Assert.True(offers[i].PlayerCash > 0);
            Assert.NotNull(c.Lot(offers[i].LotId));
        }
        Assert.Equal(new[] { "bodies", "upward" }, offers.Where(o => o.Expansion).Select(o => o.EraId).ToArray());
    }

    [Fact]
    public void An_era_that_is_not_startable_is_not_offered()
    {
        GameConfig c = TestData.Config();
        c.Era("agents").Startable = false;
        Assert.DoesNotContain(StartEras.Offers(c), o => o.EraId == "agents");
        Assert.Equal(AllEras.Length - 1, StartEras.Offers(c).Count);
    }

    [Fact]
    public void Starting_cash_and_lot_grow_with_the_era()
    {
        GameConfig c = TestData.Config();
        List<StartEraOffer> offers = StartEras.Offers(c);
        for (int i = 1; i < offers.Count; i++)
        {
            Assert.True(offers[i].PlayerCash > offers[i - 1].PlayerCash || offers[i].StartYear >= 2033,
                offers[i].EraId + " starts with no more cash than " + offers[i - 1].EraId);
            Assert.True(offers[i].TechCount >= offers[i - 1].TechCount);
            Assert.True(LotArea(c, offers[i].LotId) >= LotArea(c, offers[i - 1].LotId));
        }
    }

    static int LotArea(GameConfig c, string lotId) { LotConfig l = c.Lot(lotId); return l.Width * l.Height; }

    // ---- the offer is what a new game does

    [Fact]
    public void Every_offer_matches_the_game_it_starts()
    {
        foreach (string eraId in AllEras)
        {
            Simulation sim = Start(eraId);
            GameConfig c = sim.Config;
            StartEraOffer offer = StartEras.Offer(c, c.Era(eraId));

            Assert.Equal(offer.PlayerCash, sim.Player.Cash);
            Assert.Equal(offer.GrantPerWeek, sim.Player.GrantPerWeek);
            Assert.Equal(offer.LotId, sim.Player.LotId);
            Assert.Equal(offer.TechCount, sim.Player.Techs.Count);
            Assert.Equal(c.Era(eraId).StartYear, sim.Year);

            foreach (StartCastMember member in offer.Cast)
            {
                LabState lab = sim.State.Labs.FirstOrDefault(l => l.RivalId == member.RivalId);
                if (member.PresentAtStart)
                {
                    Assert.True(lab != null, eraId + ": " + member.Name + " should be in the world");
                    Assert.Equal(member.Cash, lab.Cash);
                }
                else
                {
                    Assert.True(lab == null, eraId + ": " + member.Name + " should not be here yet");
                    Assert.Contains(member.RivalId, sim.State.PendingRivals);
                }
            }
            Assert.Equal(offer.PresentRivals + 1, sim.State.Labs.Count);
        }
    }

    [Fact]
    public void The_cast_is_whoever_has_joined_by_the_start_year()
    {
        GameConfig c = TestData.Config();
        string[] names(string era) => StartEras.Offer(c, c.Era(era)).Cast.Where(m => m.PresentAtStart).Select(m => m.RivalId).ToArray();
        Assert.Equal(new[] { "openaye", "anthoptics", "xay" }, names("dial-up-days"));
        Assert.Contains("metta", names("punchline"));
        Assert.DoesNotContain("goggle", names("punchline"));
        Assert.Contains("macrosoft", names("it-sees"));
        Assert.Contains("goggle", names("it-sees"));
        Assert.DoesNotContain("palantear", names("it-sees"));
        Assert.Contains("palantear", names("it-writes"));
        Assert.DoesNotContain("deepsneak", names("chat-boom"));
        Assert.Contains("deepsneak", names("agents"));
        Assert.DoesNotContain("successor", names("upward"));
    }

    [Fact]
    public void Founders_get_the_eras_share_and_later_joiners_start_within_half_to_double_the_players_cash()
    {
        GameConfig c = TestData.Config();
        foreach (EraConfig era in c.Eras.Skip(1))
        {
            StartEraOffer offer = StartEras.Offer(c, era);
            foreach (StartCastMember m in offer.Cast.Where(m => m.PresentAtStart))
            {
                RivalConfig r = c.Rival(m.RivalId);
                if (r.JoinsYear == 0) Assert.Equal(MathI.MulPermille(r.StartCash, era.StartCashPermille), m.Cash);
                else Assert.InRange(m.Cash, offer.PlayerCash / 2, offer.PlayerCash * 2);
            }
        }
    }

    [Fact]
    public void A_veteran_joiner_is_never_poor_beside_a_newcomer_nor_a_giant_beside_them()
    {
        GameConfig c = TestData.Config();
        EraConfig era = c.Era("chat-boom");
        long player = MathI.MulPermille(c.PlayerStart.Cash, era.StartCashPermille);
        Assert.Equal(player / 2, WorldSetup.EstablishedCash(c, c.Rival("macrosoft"), era));
        Assert.Equal(player * 2, WorldSetup.EstablishedCash(c, c.Rival("metta"), era));
        long goggle = WorldSetup.EstablishedCash(c, c.Rival("goggle"), era);
        Assert.InRange(goggle, player / 2, player * 2);
    }

    // ---- era-appropriate world

    [Fact]
    public void The_player_starts_with_every_tech_of_the_eras_before_and_nothing_of_its_own_era()
    {
        GameConfig c = TestData.Config();
        foreach (EraConfig era in c.Eras.Skip(1))
        {
            Simulation sim = Start(era.Id);
            int index = c.Eras.IndexOf(era);
            foreach (TechConfig tech in c.Research.Techs)
            {
                int techEra = c.Eras.IndexOf(c.Era(tech.Era));
                if (techEra < index) Assert.Contains(tech.Id, sim.Player.Techs);
                else Assert.DoesNotContain(tech.Id, sim.Player.Techs);
            }
        }
    }

    [Fact]
    public void Sectors_start_at_the_size_the_start_year_has_grown_them_to()
    {
        Simulation early = Start("dial-up-days");
        Simulation late = Start("agents");
        SectorState earlyGame = early.State.Sector("game-ai");
        SectorState lateGame = late.State.Sector("game-ai");
        Assert.True(lateGame.PoolPerWeek > earlyGame.PoolPerWeek);
        Assert.Equal(WorldSetup.NewSector(late.Config.Sector("game-ai"), 2024).PoolPerWeek, lateGame.PoolPerWeek);
        Assert.Equal(late.Config.Sector("coding-agents").PoolPerWeek, late.State.Sector("coding-agents").PoolPerWeek);
    }

    [Fact]
    public void The_era_banner_rules_and_the_vendors_hardware_are_those_of_the_start_year()
    {
        foreach (string eraId in new[] { "it-writes", "agents", "upward" })
        {
            Simulation sim = Start(eraId);
            EraConfig era = sim.Config.Era(eraId);
            Assert.Equal(era.Id, EraSystem.AtTick(sim.Config, sim.Tick).Id);
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(sim.Config, sim.Tick);
            Assert.True(gen.OpensYear <= era.StartYear);
            Assert.Equal(EraRules.ReleaseBar(sim.Config, sim.Tick), era.MinReleaseAlignment);
        }
    }

    [Fact]
    public void No_dated_world_news_from_before_the_start_year_is_ever_told()
    {
        Simulation sim = Start("chat-boom", rivals: false);
        sim.StepMany(48 * 3);
        GameConfig c = sim.Config;
        foreach (GameEvent e in sim.State.Events.Where(e => e.Kind == EventKind.WorldNews))
        {
            WorldEventConfig w = c.WorldEvent(e.Ref);
            if (w != null) Assert.True(w.Year >= 2022, w.Id + " from " + w.Year);
        }
    }

    // ---- refusals and the validator

    [Fact]
    public void An_unknown_era_and_one_the_picker_does_not_offer_are_refused()
    {
        GameConfig c = TestData.Config();
        c.Era("upward").Startable = false;
        Assert.Contains("stone-age", Assert.Throws<ArgumentException>(() => Simulation.NewGame(c, new NewGameOptions { Seed = 1, StartEraId = "stone-age" })).Message);
        Assert.Contains("Upward", Assert.Throws<ArgumentException>(() => Simulation.NewGame(c, new NewGameOptions { Seed = 1, StartEraId = "upward" })).Message);
    }

    [Fact]
    public void A_starting_era_with_no_cash_is_a_config_problem()
    {
        AssertProblem(Problems(c => c.Era("agents").StartCashPermille = 0), "gives no start cash");
        Assert.Empty(Problems(c => { c.Era("agents").Startable = false; c.Scenarios.Scenarios.RemoveAll(s => s.EraId == "agents"); }));
    }

    [Fact]
    public void A_starting_era_that_leaves_out_a_tech_of_the_eras_before_is_a_config_problem()
    {
        AssertProblem(Problems(c => c.Era("it-writes").StartTechs.Remove("neural-nets")), "neural-nets");
        AssertProblem(Problems(c => c.Era("upward").StartTechs.Clear()), "does not start with");
    }

    [Fact]
    public void An_era_that_is_not_startable_may_start_with_nothing()
    {
        Assert.Empty(Problems(c =>
        {
            EraConfig e = c.Era("harnesses");
            e.Startable = false;
            e.StartCashPermille = 0;
            e.StartTechs.Clear();
        }));
    }

    // ---- playing from there

    [Theory]
    [InlineData("it-sees", 1)]
    [InlineData("it-writes", 2)]
    [InlineData("chat-boom", 1)]
    [InlineData("agents", 3)]
    [InlineData("harnesses", 2)]
    [InlineData("bodies", 1)]
    [InlineData("upward", 2)]
    public void A_game_from_any_later_era_keeps_every_lab_solvent_and_cash_inside_bounds(string eraId, int seed)
    {
        int years = Math.Min(14, 2035 - TestData.Config().Era(eraId).StartYear);
        EconomyBoundsTests.AssertInBounds(EconomyBoundsTests.Live(eraId, years, seed), eraId + " seed " + seed);
    }

    [Theory]
    [InlineData("it-sees")]
    [InlineData("agents")]
    [InlineData("upward")]
    public void The_autopilot_player_releases_a_model_and_earns_from_a_later_start(string eraId)
    {
        var game = new Game(Start(eraId, seed: 4));
        game.Autopilot(48 * 6);
        Assert.Contains(game.Me.Models, m => m.Status == ModelStatus.Released);
        Assert.True(game.Me.Ledger.Lifetime.Revenue > 0);
        Assert.False(game.Me.Bankrupt);
    }

    [Fact]
    public void A_later_start_still_runs_the_era_rules_from_the_first_week()
    {
        Game early = new Game(Start("punchline", rivals: false));
        Game late = new Game(Start("agents", rivals: false));
        Assert.Equal(early.C.Era("punchline").HireFeePermille, EraRules.Value(early.C, early.Tick, EraRuleKind.HireFee));
        Assert.Equal(late.C.Era("agents").HireFeePermille, EraRules.Value(late.C, late.Tick, EraRuleKind.HireFee));
        Assert.True(late.C.Era("agents").HireFeePermille > early.C.Era("punchline").HireFeePermille);

        late.Build("research-lab", 4, 4);
        long before = late.Me.Cash;
        late.Hire("researcher", 2);
        Assert.Equal(EraRules.HireCost(late.C, late.C.Role("researcher"), 2, late.Tick), before - late.Me.Cash);
    }

    // ---- save, load, replay

    [Fact]
    public void A_game_started_in_a_later_era_saves_loads_and_replays_the_same()
    {
        var g = new Game(Start("agents", seed: 7));
        g.Autopilot(48 * 2);
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(g.Sim.Digest(), loaded.Sim.Digest());
        g.Autopilot(48);
        loaded.Autopilot(48);
        Assert.Equal(g.Sim.Digest(), loaded.Sim.Digest());

        ReplayLog log = ReplayLog.FromJson(g.Sim.Journal.ToJson());
        Assert.Equal(g.Sim.Digest(), Simulation.Replay(TestData.Config(), log).Digest());
    }

    [Fact]
    public void The_same_seed_and_era_always_start_the_same_world()
    {
        Assert.Equal(Start("chat-boom", seed: 9).Digest(), Start("chat-boom", seed: 9).Digest());
        Assert.NotEqual(Start("chat-boom", seed: 9).Digest(), Start("agents", seed: 9).Digest());
    }

    [Fact]
    public void An_old_schema_1_save_still_loads_after_the_start_era_changes()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema1Save());
        sim.StepMany(48);
        Assert.Equal(sim.Digest(), Simulation.Load(sim.Config, sim.Save()).Digest());
    }

    [Fact]
    public void The_config_round_trips_with_every_era_startable()
    {
        GameConfig again = GameConfig.FromJson(TestData.Config().ToJson());
        Assert.Equal(AllEras, StartEras.Offers(again).Select(o => o.EraId).ToArray());
        Assert.Empty(ConfigValidator.Problems(again));
    }
}
