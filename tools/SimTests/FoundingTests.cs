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

/// <summary>Who the player is and how the lab is good: a founder, a style and a home base, chosen when the game is made.</summary>
public class FoundingTests
{
    static NewGameOptions Options(string era = "", string founder = "", string style = "", string home = "", bool rivals = false)
    {
        var o = new NewGameOptions { Seed = 1, StartEraId = era, FounderId = founder, StyleId = style, HomeBlocId = home };
        if (!rivals) o.RivalIds = new List<string>();
        return o;
    }

    static Game Start(NewGameOptions options, Action<GameConfig> tweak = null)
    {
        var g = new Game(Simulation.NewGame(TestData.Config(c => { c.RolePerks.Clear(); c.RoomPerks.Clear(); if (tweak != null) tweak(c); }), options));
        return g;
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

    // ---- the tables

    [Fact]
    public void There_are_four_founders_and_three_styles_each_with_a_name_and_a_blurb()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "professor", "dropout", "defector", "heir" }, c.Founders.Select(f => f.Id).ToArray());
        Assert.Equal(new[] { "sniper", "shotgun", "wildcard" }, c.Styles.Select(s => s.Id).ToArray());
        foreach (FounderConfig f in c.Founders) { Assert.False(string.IsNullOrWhiteSpace(f.Name)); Assert.False(string.IsNullOrWhiteSpace(f.Blurb)); }
        foreach (StyleConfig s in c.Styles)
        {
            Assert.False(string.IsNullOrWhiteSpace(s.Name));
            Assert.False(string.IsNullOrWhiteSpace(s.Blurb));
            foreach (string sector in s.FocusSectors) Assert.NotNull(c.Sector(sector));
        }
    }

    [Fact]
    public void A_sniper_and_a_shotgun_do_not_share_a_focus_sector()
    {
        GameConfig c = TestData.Config();
        foreach (StyleConfig a in c.Styles)
            foreach (StyleConfig b in c.Styles)
                if (a != b) Assert.Empty(a.FocusSectors.Intersect(b.FocusSectors));
    }

    [Fact]
    public void A_choice_that_could_not_work_is_a_config_problem()
    {
        AssertProblem(Problems(c => c.Founders[0].Name = ""), "founder professor has no name");
        AssertProblem(Problems(c => c.Founders[0].Blurb = ""), "no blurb");
        AssertProblem(Problems(c => c.Founders[0].CashPermille = 0), "cashPermille");
        AssertProblem(Problems(c => c.Founders[0].GrantPermille = -1), "grantPermille");
        AssertProblem(Problems(c => c.Founders[0].ReputationDelta = 900), "reputationDelta");
        AssertProblem(Problems(c => c.Founders[0].ResearchPermille = 900), "researchPermille");
        AssertProblem(Problems(c => c.Founders[0].AppealPermille = -900), "appealPermille");
        AssertProblem(Problems(c => c.Founders[0].HireFeePermille = 5000), "hireFeePermille");
        AssertProblem(Problems(c => c.Founders[0].EnemyRivalId = "nobody"), "unknown enemy rival");
        AssertProblem(Problems(c => c.Founders[0].EnemyFeeling = -100), "no enemy");
        AssertProblem(Problems(c => c.Founders[2].EnemyFeeling = 100), "enemyFeeling");
        AssertProblem(Problems(c => c.Styles[0].FocusSectors.Clear()), "no focus sectors");
        AssertProblem(Problems(c => c.Styles[0].FocusSectors.Add("moon-mining")), "moon-mining");
        AssertProblem(Problems(c => c.Styles[0].FocusAppealPermille = 900), "focusAppealPermille");
        AssertProblem(Problems(c => c.Styles[0].OtherAppealPermille = 10), "otherAppealPermille");
        AssertProblem(Problems(c => c.Styles.Add(c.Styles[0])), "styles");
        AssertProblem(Problems(c => c.Founders.Add(c.Founders[0])), "founders");
        Assert.Empty(Problems(c => { c.Founders.Clear(); c.Styles.Clear(); }));
    }

    [Fact]
    public void An_unknown_founder_style_or_home_base_is_refused_when_the_game_is_made()
    {
        GameConfig c = TestData.Config();
        Assert.Throws<ArgumentException>(() => Simulation.NewGame(c, Options(founder: "wizard")));
        Assert.Throws<ArgumentException>(() => Simulation.NewGame(c, Options(style: "wizard")));
        Assert.Throws<ArgumentException>(() => Simulation.NewGame(c, Options(home: "atlantis")));
    }

    // ---- no choice at all

    [Fact]
    public void With_no_choice_the_lab_is_as_it_always_was_and_every_reading_is_zero()
    {
        Game g = Start(Options(rivals: true));
        Assert.Equal("", g.Me.FounderId);
        Assert.Equal("", g.Me.StyleId);
        Assert.Null(Founding.FounderOf(g.C, g.Me));
        Assert.Null(Founding.StyleOf(g.C, g.Me));
        Assert.Equal(0, Founding.ResearchPermille(g.C, g.Me));
        Assert.Equal(0, Founding.HireFeePermille(g.C, g.Me));
        Assert.Equal(0, Founding.AppealPermille(g.C, g.Me, "consumer-assistant"));
        Assert.Equal(g.C.PlayerStart.Cash, g.Me.Cash);
        Assert.Equal(g.C.PlayerStart.Reputation, g.Me.Reputation);
        Assert.All(g.State.Labs.Where(l => l.Controller != LabController.Player), l => { Assert.Equal("", l.FounderId); Assert.Equal("", l.StyleId); });
    }

    // ---- founders: the start

    [Theory]
    [InlineData("professor")]
    [InlineData("dropout")]
    [InlineData("defector")]
    [InlineData("heir")]
    public void A_founder_changes_the_start_by_what_the_table_says(string id)
    {
        Game plain = Start(Options());
        Game g = Start(Options(founder: id));
        FounderConfig f = g.C.Founder(id);
        Assert.Equal(id, g.Me.FounderId);
        Assert.Equal(MathI.MulPermille(plain.Me.Cash, f.CashPermille), g.Me.Cash);
        Assert.Equal(g.Me.Cash, g.Me.Ledger.StartCash);
        Assert.Equal(Math.Max(0, Math.Min(1000, plain.Me.Reputation + f.ReputationDelta)), g.Me.Reputation);
        Assert.Equal(MathI.MulPermille(plain.Me.GrantPerWeek, f.GrantPermille), g.Me.GrantPerWeek);
    }

    [Fact]
    public void A_founder_cannot_push_hype_past_the_ends_of_the_scale()
    {
        Game high = Start(Options(founder: "dropout"), c => { c.Founders[1].ReputationDelta = 500; c.PlayerStart.Reputation = 900; });
        Assert.Equal(1000, high.Me.Reputation);
        Game low = Start(Options(founder: "heir"), c => { c.Founders[3].ReputationDelta = -500; c.PlayerStart.Reputation = 100; });
        Assert.Equal(0, low.Me.Reputation);
    }

    [Fact]
    public void The_heir_can_buy_compute_on_day_one_and_the_professor_cannot()
    {
        Game heir = Start(Options(founder: "heir"));
        Game professor = Start(Options(founder: "professor"));
        Assert.True(heir.Me.Cash >= 3 * professor.Me.Cash - 1);
        heir.Rent(60);
        Assert.Equal(60, heir.Me.RentedGpus);
    }

    // ---- the defector's enemy

    [Fact]
    public void The_defector_starts_in_a_feud_with_the_enemy_and_with_nobody_else()
    {
        Game g = Start(Options(founder: "defector", rivals: true));
        LabState enemy = g.Rival("openaye");
        Assert.Equal(-600, enemy.FeelingToPlayer);
        Assert.True(RivalSystem.IsFeud(g.C, enemy));
        Assert.All(g.State.Labs.Where(l => l.Controller != LabController.Player && l.RivalId != "openaye"), l => Assert.Equal(0, l.FeelingToPlayer));
    }

    [Fact]
    public void An_enemy_that_is_not_in_the_game_is_skipped_not_an_error()
    {
        Game g = Start(Options(founder: "defector"));
        Assert.Equal("defector", g.Me.FounderId);
        Game later = Start(Options(era: "chat-boom", founder: "defector", rivals: true));
        Assert.Equal(-600, later.Rival("openaye").FeelingToPlayer);
    }

    [Fact]
    public void The_feud_fades_with_the_months_like_any_other_but_does_not_vanish_in_a_year()
    {
        Game g = Start(Options(founder: "defector", rivals: true));
        g.Weeks(52);
        int feeling = g.Rival("openaye").FeelingToPlayer;
        Assert.InRange(feeling, -599, -1);
    }

    // ---- founders: the lasting edge and cost

    static long ResearchInAWeek(Game g)
    {
        g.Me.Cash = 1_000_000_000;
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 4);
        g.Weeks(1);
        return g.Me.LastWeek.ResearchGained;
    }

    [Fact]
    public void The_professors_researchers_earn_ten_percent_more_and_the_dropouts_five_percent_fewer()
    {
        long baseline = ResearchInAWeek(Start(Options()));
        Assert.True(baseline > 0);
        Assert.Equal(MathI.MulPermille(baseline, 1100), ResearchInAWeek(Start(Options(founder: "professor"))));
        Assert.Equal(MathI.MulPermille(baseline, 950), ResearchInAWeek(Start(Options(founder: "dropout"))));
    }

    [Fact]
    public void A_founders_research_edge_adds_to_the_research_floors()
    {
        Game g = Start(Options(era: "it-sees", founder: "professor"), c => c.RoomPerks.AddRange(TestData.Config().RoomPerks));
        g.Me.Cash = 1_000_000_000;
        g.Build("research-floor", 4, 4);
        int room = RoomPerks.Permille(g.C, g.Me, RoomPerkKind.ResearchSpeed);
        Assert.True(room > 0);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 4);
        g.Weeks(1);
        long boosted = g.Me.LastWeek.ResearchGained;
        Game plain = Start(Options(era: "it-sees"));
        long baseline = ResearchInAWeek(plain);
        Assert.Equal(MathI.MulPermille(baseline, 1000 + room + 100), boosted);
    }

    [Fact]
    public void The_heirs_hires_cost_fifteen_percent_more_and_the_breakdown_still_adds_up()
    {
        Game plain = Start(Options());
        Game heir = Start(Options(founder: "heir"));
        RoleConfig role = plain.C.Role("researcher");
        long baseFee = RolePerks.HireFee(plain.C, plain.Me, role, plain.Tick);
        long fee = RolePerks.HireFee(heir.C, heir.Me, role, heir.Tick);
        Assert.Equal(MathI.MulPermille(baseFee, 1150), fee);
        var parts = RolePerks.HireFeeParts(heir.C, heir.Me, role, heir.Tick);
        Assert.Equal(fee, parts.ListPrice + parts.EraShift + parts.PerkDiscount);
        long before = heir.Me.Cash;
        heir.Build("research-lab", 4, 4);
        long afterBuild = heir.Me.Cash;
        heir.Hire("researcher", 2);
        Assert.Equal(2 * fee, afterBuild - heir.Me.Cash);
        Assert.True(before > afterBuild);
    }

    // ---- appeal: founders and styles

    /// <summary>A lab with one released model on sale in the given sectors, in an era where they are open.</summary>
    static (Game game, ModelState model) Selling(string founder, string style, params string[] sectors)
    {
        Game g = Start(Options(era: "it-sees", founder: founder, style: style));
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 100_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, sectors));
        return (g, model);
    }

    static DisplayQueries.AppealBreakdown AppealIn(Game g, ModelState model, string sectorId)
    {
        return DisplayQueries.AppealParts(g.C, g.State, g.C.Sector(sectorId), g.State.Sector(sectorId), g.Me, model);
    }

    [Fact]
    public void The_professor_is_terrible_at_sales_and_the_dropout_a_little_better_than_most()
    {
        var (prof, pm) = Selling("professor", "", "doc-reading");
        var (drop, dm) = Selling("dropout", "", "doc-reading");
        var (none, nm) = Selling("", "", "doc-reading");
        Assert.True(AppealIn(prof, pm, "doc-reading").RoleBoost < 0);
        Assert.True(AppealIn(drop, dm, "doc-reading").RoleBoost > 0);
        Assert.Equal(0, AppealIn(none, nm, "doc-reading").RoleBoost);
    }

    [Fact]
    public void A_sniper_wins_in_its_focus_and_loses_everywhere_else()
    {
        var (g, model) = Selling("", "sniper", "doc-reading", "image-recognition");
        Assert.True(AppealIn(g, model, "doc-reading").RoleBoost > 0);
        Assert.True(AppealIn(g, model, "image-recognition").RoleBoost < 0);
        Assert.Equal(250, Founding.AppealPermille(g.C, g.Me, "doc-reading"));
        Assert.Equal(-80, Founding.AppealPermille(g.C, g.Me, "image-recognition"));
    }

    [Fact]
    public void A_shotgun_gets_a_small_lift_across_chat_image_and_voice_and_a_small_cost_elsewhere()
    {
        var (g, model) = Selling("", "shotgun", "doc-reading", "image-recognition");
        Assert.Equal(80, Founding.AppealPermille(g.C, g.Me, "image-recognition"));
        Assert.Equal(-20, Founding.AppealPermille(g.C, g.Me, "doc-reading"));
        Assert.True(AppealIn(g, model, "image-recognition").RoleBoost > 0);
        Assert.True(AppealIn(g, model, "doc-reading").RoleBoost < 0);
    }

    [Fact]
    public void A_wildcards_focus_is_narrow_and_strong()
    {
        var (g, model) = Selling("", "wildcard", "fraud-detection", "doc-reading");
        Assert.Equal(350, Founding.AppealPermille(g.C, g.Me, "fraud-detection"));
        Assert.Equal(-100, Founding.AppealPermille(g.C, g.Me, "doc-reading"));
        Assert.True(AppealIn(g, model, "fraud-detection").RoleBoost > AppealIn(g, model, "doc-reading").RoleBoost);
    }

    [Fact]
    public void A_founder_and_a_style_add_up_and_the_sum_never_falls_below_minus_half()
    {
        Game g = Start(Options(founder: "professor", style: "wildcard"));
        Assert.Equal(-100 + -100, Founding.AppealPermille(g.C, g.Me, "doc-reading"));
        Assert.Equal(-100 + 350, Founding.AppealPermille(g.C, g.Me, "cybersecurity"));
        Game harsh = Start(Options(founder: "professor", style: "wildcard"), c =>
        {
            c.Founders[0].AppealPermille = -400;
            c.Styles[2].OtherAppealPermille = -400;
        });
        Assert.Equal(Founding.AppealFloorPermille, Founding.AppealPermille(harsh.C, harsh.Me, "doc-reading"));
    }

    [Fact]
    public void Founding_appeal_is_the_role_boost_and_stacks_with_the_role_and_room_perks()
    {
        Game g = Start(Options(era: "chat-boom", style: "sniper"), c =>
        {
            c.RolePerks.AddRange(TestData.Config().RolePerks);
            c.RoomPerks.AddRange(TestData.Config().RoomPerks);
        });
        g.WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 100_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, "doc-reading"));
        long alone = AppealIn(g, model, "doc-reading").RoleBoost;
        g.Build("product-studio", 6, 6);
        g.Hire("product-lead", 4);
        long with = AppealIn(g, model, "doc-reading").RoleBoost;
        Assert.True(with > alone && alone > 0);
        var parts = AppealIn(g, model, "doc-reading");
        Assert.Equal(parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.RoleBoost + parts.TrustBoost + parts.ChampionBoost + parts.NamingConfusion, parts.Total);
    }

    // ---- home base

    [Fact]
    public void The_home_base_option_picks_the_bloc_and_empty_keeps_the_default()
    {
        Assert.Equal(TestData.Config().PlayerStart.HomeBlocId, Start(Options()).Me.HomeBlocId);
        Assert.Equal("nordics", Start(Options(home: "nordics")).Me.HomeBlocId);
        Assert.Equal("thailand", Start(Options(home: "thailand", founder: "heir")).Me.HomeBlocId);
    }

    // ---- saves

    [Fact]
    public void A_founder_and_a_style_save_and_load_and_play_on_the_same()
    {
        Game g = Start(Options(era: "chat-boom", founder: "defector", style: "sniper", rivals: true));
        g.WithStarterLab();
        g.Weeks(20);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        string json = g.Sim.Save();
        Assert.Contains("\"founderId\":\"defector\"", json.Replace(" ", ""));
        Simulation again = Simulation.Load(g.C, json);
        Assert.Equal("defector", again.Player.FounderId);
        Assert.Equal("sniper", again.Player.StyleId);
        g.Weeks(30);
        again.StepMany(30);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_save_from_before_the_choices_loads_with_neither_and_plays_on()
    {
        Game g = Start(Options(era: "chat-boom", rivals: true));
        g.WithStarterLab();
        g.Weeks(10);
        g.Me.CultureShift = 0;
        foreach (RoomState room in g.Me.Rooms) room.EraAge = 0;
        string old = SaveSchemaPretend.At(g.C, 21, g.Sim.Save());
        Assert.DoesNotContain("founderId", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => { Assert.Equal("", l.FounderId); Assert.Equal("", l.StyleId); });
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void The_config_with_its_founders_and_styles_is_strict_json_and_round_trips()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(c.ToJson(), GameConfig.FromJson(c.ToJson()).ToJson());
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(TestData.ConfigJson().Replace("\"founders\"", "\"foundersX\"")));
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(TestData.ConfigJson().Replace("\"enemyFeeling\"", "\"enemyFeelings\"")));
    }

    // ---- selling the lab

    [Fact]
    public void A_lab_founded_again_keeps_its_founder_and_style_and_the_sold_lab_loses_them()
    {
        Game g = Start(Options(founder: "professor", style: "wildcard"), c => c.Endings.ExitRevenueYears = 1);
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        int sold = g.Id;
        g.Do(Command.SellLab(g.Id, "found-again"));
        LabState fresh = g.State.Lab(g.State.PlayerLabId);
        Assert.NotEqual(sold, fresh.Id);
        Assert.Equal("professor", fresh.FounderId);
        Assert.Equal("wildcard", fresh.StyleId);
        LabState old = g.State.Lab(sold);
        Assert.Equal("", old.FounderId);
        Assert.Equal("", old.StyleId);
    }

    [Fact]
    public void A_lab_sold_to_spectate_hands_over_the_choices_too()
    {
        Game g = Start(Options(founder: "heir", style: "shotgun"), c => c.Endings.ExitRevenueYears = 1);
        g.Me.Ledger.ThisMonth.Revenue = 2_000_000;
        int sold = g.Id;
        g.Do(Command.SellLab(sold, "spectate"));
        Assert.Equal("", g.State.Lab(sold).FounderId);
        Assert.Equal("", g.State.Lab(sold).StyleId);
    }

    // ---- whole games

    [Fact]
    public void Every_founder_and_style_pairing_plays_three_years_with_a_valid_save()
    {
        GameConfig c = TestData.Config();
        foreach (FounderConfig f in c.Founders)
            foreach (StyleConfig s in c.Styles)
            {
                var g = new Game(Simulation.NewGame(TestData.Config(), Options(founder: f.Id, style: s.Id, rivals: true)));
                g.Autopilot(156);
                Assert.Empty(SaveGame.Problems(g.C, g.State));
                SaveSchemaPretend.AssertRoundTrip(g.Sim);
            }
    }

    [Fact]
    public void A_founder_game_goes_a_different_way_from_the_plain_one_from_the_same_seed()
    {
        var plain = new Game(Simulation.NewGame(TestData.Config(), Options(rivals: true)));
        var heir = new Game(Simulation.NewGame(TestData.Config(), Options(founder: "heir", rivals: true)));
        plain.Weeks(10);
        heir.Weeks(10);
        Assert.NotEqual(plain.Sim.Digest(), heir.Sim.Digest());
    }

    [Fact]
    public void A_founder_start_in_a_later_era_keeps_the_era_cash_and_cast()
    {
        var g = new Game(Simulation.NewGame(TestData.Config(), Options(era: "agents", founder: "heir", style: "sniper", rivals: true)));
        Game plain = new Game(Simulation.NewGame(TestData.Config(), Options(era: "agents", rivals: true)));
        Assert.Equal(MathI.MulPermille(plain.Me.Cash, 3000), g.Me.Cash);
        Assert.Equal(plain.State.Labs.Count, g.State.Labs.Count);
        g.Weeks(52);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    // ---- the page

    [Fact]
    public void The_founding_page_lists_every_founder_and_style()
    {
        GameConfig c = TestData.Config();
        string page = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "docs", "content", "founding.md"));
        foreach (FounderConfig f in c.Founders) Assert.Contains("`" + f.Id + "`", page);
        foreach (StyleConfig s in c.Styles)
        {
            Assert.Contains("`" + s.Id + "`", page);
            foreach (string sector in s.FocusSectors) Assert.Contains("`" + sector + "`", page);
        }
    }
}
