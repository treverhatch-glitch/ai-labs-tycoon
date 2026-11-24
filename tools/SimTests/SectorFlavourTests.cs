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

/// <summary>Sector incident flavours: each sector fails in its own way, and an incident there costs what that way costs.</summary>
public class SectorFlavourTests
{
    static GameConfig Config(Action<GameConfig> tweak = null) => TestData.Config(tweak);

    static void Quiet(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.RolePerks.Clear();
        c.Safety.RiskClasses.Clear();
        c.Safety.IncidentChancePermille = 1000;
    }

    /// <summary>A lone lab with one released model on sale in the given sectors, money enough that nothing bankrupts it, and no risk classes.</summary>
    static (Game game, ModelState model) Selling(string[] sectors, Action<GameConfig> tweak = null, bool plain = false, bool risks = false)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents", RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            Quiet(c);
            if (plain) c.Safety.SectorFlavours.Clear();
            if (tweak != null) tweak(c);
        }), options));
        if (risks) Assert.NotEmpty(g.C.Safety.RiskClasses);
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        g.Release(model, sectors);
        model.Alignment = 300;
        model.DisplayedAlignment = 300;
        g.Me.Cash = 1_000_000_000_000_000;
        return (g, model);
    }

    static void Takes(Game g, string sector, long weekly) => g.State.Sector(sector).Entry(g.Id).LastRevenue = weekly;

    /// <summary>One week of incidents for the lab, nothing else, so the test sees only what an incident does.</summary>
    static (long cost, int reputation, long amount) Incident(Game g)
    {
        long cash = g.Me.Cash;
        int rep = g.Me.Reputation;
        int events = g.State.Events.Count;
        SafetySystem.RunWeek(g.C, g.State, g.Me);
        GameEvent e = g.State.Events.Skip(events).First(x => x.Kind == EventKind.Incident);
        return (cash - g.Me.Cash, rep - g.Me.Reputation, e.Amount);
    }

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

    static string Pct(int permille) => (permille / 10.0).ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) + "%";

    static SectorFlavourConfig Flavour(GameConfig c, string sector) => c.Safety.SectorFlavour(sector);

    // ---- the table

    [Fact]
    public void Eighteen_sectors_fail_in_their_own_way_and_the_rest_fail_plainly()
    {
        GameConfig c = Config();
        Assert.Equal(18, c.Safety.SectorFlavours.Count);
        Assert.Empty(ConfigValidator.Problems(c));
        Assert.Equal(c.Safety.SectorFlavours.Count, c.Safety.SectorFlavours.Select(f => f.Sector).Distinct().Count());
        foreach (SectorFlavourConfig f in c.Safety.SectorFlavours)
        {
            Assert.NotNull(c.Sector(f.Sector));
            Assert.False(string.IsNullOrEmpty(f.Failure));
        }
        Assert.Null(Flavour(c, "web-ads"));
        Assert.Equal("", SectorFlavours.Failure(c, "web-ads"));
        Assert.Equal("", SectorFlavours.Failure(c, "no-such-sector"));
    }

    [Fact]
    public void The_two_named_in_the_bible_are_there()
    {
        GameConfig c = Config();
        Assert.Contains("refund policy you do not have", SectorFlavours.Failure(c, "customer-support"));
        Assert.Contains("cases that do not exist", SectorFlavours.Failure(c, "legal-research"));
        Assert.True(Flavour(c, "customer-support").MakeGoodWeeks > 0, "the support bot's refunds are the cost beyond reputation");
        Assert.True(Flavour(c, "legal-research").ReputationPermille > 1000);
    }

    [Fact]
    public void Where_trust_is_the_product_an_incident_hurts_more_than_where_nobody_expects_better()
    {
        GameConfig c = Config();
        Assert.True(Flavour(c, "companionship").TrustPermille > Flavour(c, "consumer-assistant").TrustPermille);
        Assert.True(Flavour(c, "legal-research").TrustPermille > Flavour(c, "code-completion").TrustPermille);
        Assert.True(Flavour(c, "game-bots").TrustPermille < 1000);
    }

    [Fact]
    public void Grounded_and_unsafe_sectors_stay_down_longer_than_the_lab_wide_glitch()
    {
        GameConfig c = Config();
        foreach (string s in new[] { "autonomous-driving", "legal-research", "science-medicine", "household-robots", "government-services" })
            Assert.True(SectorFlavours.GlitchWeeks(c, s) > c.Safety.GlitchWeeks, s);
        Assert.True(SectorFlavours.GlitchWeeks(c, "game-bots") < c.Safety.GlitchWeeks);
    }

    [Fact]
    public void Every_shipped_flavour_is_inside_the_validated_ranges()
    {
        foreach (SectorFlavourConfig f in Config().Safety.SectorFlavours)
        {
            Assert.InRange(f.FinePermille, 250, 5000);
            Assert.InRange(f.ReputationPermille, 250, 5000);
            Assert.InRange(f.TrustPermille, 250, 5000);
            Assert.InRange(f.MakeGoodWeeks, 0, 26);
            Assert.InRange(f.GlitchWeeks, 0, 52);
            Assert.InRange(f.GlitchQualityPermille, 0, 1000);
        }
    }

    // ---- the validator

    [Fact]
    public void The_validator_names_each_way_a_flavour_can_be_wrong()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => Flavour(c, "legal-research").Sector = "no-such-sector"), "unknown sector");
        AssertProblem(Problems(c => Flavour(c, "legal-research").Sector = "customer-support"), "second flavour of sector customer-support");
        AssertProblem(Problems(c => c.Safety.SectorFlavours.Add(c.Safety.SectorFlavours[0])), "twice");
        AssertProblem(Problems(c => Flavour(c, "legal-research").Failure = ""), "no failure line");
        AssertProblem(Problems(c => Flavour(c, "legal-research").FinePermille = 249), "finePermille");
        AssertProblem(Problems(c => Flavour(c, "legal-research").FinePermille = 5001), "finePermille");
        AssertProblem(Problems(c => Flavour(c, "legal-research").ReputationPermille = 249), "reputationPermille");
        AssertProblem(Problems(c => Flavour(c, "legal-research").ReputationPermille = 5001), "reputationPermille");
        AssertProblem(Problems(c => Flavour(c, "legal-research").TrustPermille = 249), "trustPermille");
        AssertProblem(Problems(c => Flavour(c, "legal-research").TrustPermille = 5001), "trustPermille");
        AssertProblem(Problems(c => Flavour(c, "legal-research").MakeGoodWeeks = -1), "makeGoodWeeks");
        AssertProblem(Problems(c => Flavour(c, "legal-research").MakeGoodWeeks = 27), "makeGoodWeeks");
        AssertProblem(Problems(c => Flavour(c, "legal-research").GlitchWeeks = -1), "glitchWeeks");
        AssertProblem(Problems(c => Flavour(c, "legal-research").GlitchWeeks = 53), "glitchWeeks");
        AssertProblem(Problems(c => Flavour(c, "legal-research").GlitchQualityPermille = -1), "glitchQualityPermille");
        AssertProblem(Problems(c => Flavour(c, "legal-research").GlitchQualityPermille = 1001), "glitchQualityPermille");
    }

    [Fact]
    public void The_edges_of_every_range_are_allowed()
    {
        Assert.Empty(Problems(c =>
        {
            SectorFlavourConfig f = Flavour(c, "legal-research");
            f.FinePermille = 250; f.ReputationPermille = 250; f.TrustPermille = 250;
            f.MakeGoodWeeks = 0; f.GlitchWeeks = 0; f.GlitchQualityPermille = 0;
        }));
        Assert.Empty(Problems(c =>
        {
            SectorFlavourConfig f = Flavour(c, "legal-research");
            f.FinePermille = 5000; f.ReputationPermille = 5000; f.TrustPermille = 5000;
            f.MakeGoodWeeks = 26; f.GlitchWeeks = 52; f.GlitchQualityPermille = 1000;
        }));
    }

    [Fact]
    public void A_game_with_no_flavours_at_all_is_valid()
    {
        Assert.Empty(Problems(c => c.Safety.SectorFlavours.Clear()));
    }

    // ---- the glitch

    [Fact]
    public void A_sector_can_set_how_long_the_glitch_lasts_and_how_deep()
    {
        GameConfig c = Config();
        Assert.Equal(12, SectorFlavours.GlitchWeeks(c, "legal-research"));
        Assert.Equal(600, SectorFlavours.GlitchQualityPermille(c, "legal-research"));
        Assert.Equal(c.Safety.GlitchWeeks, SectorFlavours.GlitchWeeks(c, "consumer-assistant"));
        Assert.Equal(c.Safety.GlitchQualityPermille, SectorFlavours.GlitchQualityPermille(c, "consumer-assistant"));
        Assert.Equal(c.Safety.GlitchWeeks, SectorFlavours.GlitchWeeks(c, "no-such-sector"));
    }

    [Fact]
    public void A_zero_in_the_data_means_the_lab_wide_value()
    {
        GameConfig c = Config(x => { Flavour(x, "legal-research").GlitchWeeks = 0; Flavour(x, "legal-research").GlitchQualityPermille = 0; });
        Assert.Equal(c.Safety.GlitchWeeks, SectorFlavours.GlitchWeeks(c, "legal-research"));
        Assert.Equal(c.Safety.GlitchQualityPermille, SectorFlavours.GlitchQualityPermille(c, "legal-research"));
    }

    [Fact]
    public void The_glitch_ends_on_each_sectors_own_week()
    {
        GameConfig c = Config();
        var model = new ModelState { Incidents = 1, LastIncidentTick = 100 };
        Assert.True(SectorFlavours.IsGlitching(c, model, "legal-research", 100 + 11));
        Assert.False(SectorFlavours.IsGlitching(c, model, "legal-research", 100 + 12));
        Assert.True(SectorFlavours.IsGlitching(c, model, "consumer-assistant", 100 + c.Safety.GlitchWeeks - 1));
        Assert.False(SectorFlavours.IsGlitching(c, model, "consumer-assistant", 100 + c.Safety.GlitchWeeks));
        Assert.True(SectorFlavours.IsGlitching(c, model, "autonomous-driving", 100 + 15));
        Assert.False(SectorFlavours.IsGlitching(c, model, "autonomous-driving", 100 + 16));
    }

    [Fact]
    public void A_model_that_never_had_an_incident_never_glitches()
    {
        GameConfig c = Config();
        var model = new ModelState { Incidents = 0, LastIncidentTick = 0 };
        Assert.False(SectorFlavours.IsGlitching(c, model, "legal-research", 0));
    }

    [Fact]
    public void Quality_in_a_sector_stays_down_by_that_sectors_flavour()
    {
        GameConfig c = Config();
        var model = new ModelState { Capability = 100_000, Alignment = 500, DisplayedAlignment = 500, Incidents = 1, LastIncidentTick = 100 };
        SectorConfig legal = c.Sector("legal-research");
        SectorConfig plain = c.Sector("consumer-assistant");

        DisplayQueries.QualityBreakdown atLegal = DisplayQueries.QualityParts(c, legal, model, 108);
        long beforeLegal = atLegal.FromCapability + atLegal.FromAlignment + atLegal.SandbagBoost;
        Assert.Equal(beforeLegal * 600 / 1000 - beforeLegal, atLegal.GlitchAdjustment);

        Assert.Equal(0, DisplayQueries.QualityParts(c, plain, model, 108).GlitchAdjustment);
        Assert.True(DisplayQueries.QualityParts(c, plain, model, 103).GlitchAdjustment < 0);
        Assert.Equal(0, DisplayQueries.QualityParts(c, legal, model, 112).GlitchAdjustment);
    }

    [Fact]
    public void Without_flavours_the_quality_dip_is_the_old_one()
    {
        GameConfig c = Config(x => x.Safety.SectorFlavours.Clear());
        var model = new ModelState { Capability = 100_000, Alignment = 500, DisplayedAlignment = 500, Incidents = 1, LastIncidentTick = 100 };
        SectorConfig legal = c.Sector("legal-research");
        DisplayQueries.QualityBreakdown inside = DisplayQueries.QualityParts(c, legal, model, 100 + c.Safety.GlitchWeeks - 1);
        long before = inside.FromCapability + inside.FromAlignment + inside.SandbagBoost;
        Assert.Equal(MathI.MulPermille(before, c.Safety.GlitchQualityPermille) - before, inside.GlitchAdjustment);
        Assert.Equal(0, DisplayQueries.QualityParts(c, legal, model, 100 + c.Safety.GlitchWeeks).GlitchAdjustment);
    }

    // ---- the fine and the reputation

    [Fact]
    public void A_flavoured_sector_scales_the_fine_and_the_reputation_lost()
    {
        var plain = Selling(new[] { "legal-research" }, plain: true);
        var flavoured = Selling(new[] { "legal-research" });
        Takes(plain.game, "legal-research", 1_000_000);
        Takes(flavoured.game, "legal-research", 1_000_000);
        var a = Incident(plain.game);
        var b = Incident(flavoured.game);
        Assert.True(a.cost > 0 && a.reputation > 0);
        Assert.InRange(b.cost, a.cost * 3 / 2 - 1, a.cost * 3 / 2 + 1);
        Assert.InRange(b.reputation, a.reputation * 18 / 10 - 1, a.reputation * 18 / 10 + 1);
        Assert.Equal(b.cost, b.amount);
    }

    [Fact]
    public void A_sector_with_no_flavour_is_priced_exactly_as_before()
    {
        var plain = Selling(new[] { "web-ads" }, plain: true);
        var flavoured = Selling(new[] { "web-ads" });
        Takes(plain.game, "web-ads", 1_000_000);
        Takes(flavoured.game, "web-ads", 1_000_000);
        Assert.Equal(Incident(plain.game), Incident(flavoured.game));
        Assert.Null(SectorFlavours.Leading(flavoured.game.C, flavoured.game.State, flavoured.game.Me, flavoured.model));
        Assert.Equal(0, SectorFlavours.MakeGood(flavoured.game.C, flavoured.game.State, flavoured.game.Me, flavoured.model));
    }

    [Fact]
    public void A_cheaper_flavour_lowers_the_bill()
    {
        var plain = Selling(new[] { "code-completion" }, plain: true);
        var cheap = Selling(new[] { "code-completion" });
        Takes(plain.game, "code-completion", 1_000_000);
        Takes(cheap.game, "code-completion", 1_000_000);
        Assert.True(Incident(cheap.game).cost < Incident(plain.game).cost);
    }

    [Fact]
    public void With_no_takings_there_is_nothing_to_refund_and_the_fine_is_the_minimum()
    {
        var plain = Selling(new[] { "customer-support" }, plain: true);
        var s = Selling(new[] { "customer-support" });
        Takes(plain.game, "customer-support", 0);
        Takes(s.game, "customer-support", 0);
        var cost = Incident(s.game);
        Assert.Equal(Incident(plain.game).cost, cost.cost);
        Assert.True(cost.cost >= s.game.C.Safety.IncidentFineMin);
    }

    // ---- the refunds

    [Fact]
    public void The_support_bot_refunds_weeks_of_takings_on_top_of_the_fine()
    {
        var plain = Selling(new[] { "customer-support" }, plain: true);
        var flavoured = Selling(new[] { "customer-support" });
        Takes(plain.game, "customer-support", 500_000);
        Takes(flavoured.game, "customer-support", 500_000);
        long fineOnly = Incident(plain.game).cost;
        var b = Incident(flavoured.game);
        Assert.Equal(2, Flavour(flavoured.game.C, "customer-support").MakeGoodWeeks);
        Assert.InRange(b.cost - fineOnly, 1_000_000 - 2, 1_000_000 + 2);
        Assert.Equal(b.cost, b.amount);
    }

    [Fact]
    public void Refunds_are_booked_with_the_fines_and_come_out_of_cash()
    {
        var s = Selling(new[] { "customer-support" });
        Takes(s.game, "customer-support", 500_000);
        long fines = s.game.Me.Ledger.ThisWeek.Fines;
        var cost = Incident(s.game);
        Assert.Equal(cost.cost, fines - s.game.Me.Ledger.ThisWeek.Fines);
    }

    [Fact]
    public void A_model_on_sale_in_two_flavoured_sectors_pays_both_refunds()
    {
        var s = Selling(new[] { "customer-support", "coding-agents" });
        Takes(s.game, "customer-support", 100_000);
        Takes(s.game, "coding-agents", 300_000);
        long expected = 100_000L * Flavour(s.game.C, "customer-support").MakeGoodWeeks + 300_000L * Flavour(s.game.C, "coding-agents").MakeGoodWeeks;
        Assert.True(Flavour(s.game.C, "coding-agents").MakeGoodWeeks > 0);
        Assert.Equal(expected, SectorFlavours.MakeGood(s.game.C, s.game.State, s.game.Me, s.model));
    }

    [Fact]
    public void Refunds_follow_the_model_that_has_the_sector_not_a_rival_model_in_it()
    {
        var s = Selling(new[] { "customer-support" });
        Takes(s.game, "customer-support", 500_000);
        s.game.State.Sector("customer-support").Entry(s.game.Id).ModelId = s.model.Id + 999;
        Assert.Equal(0, SectorFlavours.MakeGood(s.game.C, s.game.State, s.game.Me, s.model));
    }

    // ---- two sectors, one model

    [Fact]
    public void The_sector_where_the_model_takes_the_most_sets_the_fine()
    {
        var a = Selling(new[] { "legal-research", "code-completion" });
        Takes(a.game, "legal-research", 900_000);
        Takes(a.game, "code-completion", 100_000);
        Assert.Equal("legal-research", SectorFlavours.Leading(a.game.C, a.game.State, a.game.Me, a.model).Sector);

        var b = Selling(new[] { "legal-research", "code-completion" });
        Takes(b.game, "legal-research", 100_000);
        Takes(b.game, "code-completion", 900_000);
        Assert.Equal("code-completion", SectorFlavours.Leading(b.game.C, b.game.State, b.game.Me, b.model).Sector);
    }

    [Fact]
    public void A_tie_goes_to_the_sector_the_model_was_released_into_first()
    {
        var s = Selling(new[] { "legal-research", "code-completion" });
        Takes(s.game, "legal-research", 0);
        Takes(s.game, "code-completion", 0);
        Assert.Equal("legal-research", SectorFlavours.Leading(s.game.C, s.game.State, s.game.Me, s.model).Sector);
    }

    [Fact]
    public void A_plain_sector_with_the_biggest_takings_does_not_hide_a_flavoured_one()
    {
        var s = Selling(new[] { "web-ads", "legal-research" });
        Takes(s.game, "web-ads", 5_000_000);
        Takes(s.game, "legal-research", 1);
        Assert.Equal("legal-research", SectorFlavours.Leading(s.game.C, s.game.State, s.game.Me, s.model).Sector);
    }

    // ---- trust

    [Fact]
    public void An_incident_costs_the_sectors_customers_by_that_sectors_trust_multiplier()
    {
        var s = Selling(new[] { "legal-research" });
        Takes(s.game, "legal-research", 100_000);
        int before = TrustSystem.Of(s.game.C, s.game.Me, "legal-research");
        Incident(s.game);
        int drop = before - TrustSystem.Of(s.game.C, s.game.Me, "legal-research");
        Assert.Equal((int)MathI.MulPermille(s.game.C.Trust.IncidentDrop, Flavour(s.game.C, "legal-research").TrustPermille), drop);
        Assert.True(drop > s.game.C.Trust.IncidentDrop);
    }

    [Fact]
    public void A_plain_sector_loses_the_old_amount_of_trust()
    {
        var s = Selling(new[] { "web-ads" });
        Takes(s.game, "web-ads", 100_000);
        int before = TrustSystem.Of(s.game.C, s.game.Me, "web-ads");
        Incident(s.game);
        Assert.Equal(s.game.C.Trust.IncidentDrop, before - TrustSystem.Of(s.game.C, s.game.Me, "web-ads"));
    }

    [Fact]
    public void Trust_cannot_fall_below_zero_however_sharp_the_flavour()
    {
        var s = Selling(new[] { "legal-research" }, tweak: c => { Flavour(c, "legal-research").TrustPermille = 5000; c.Trust.IncidentDrop = 1000; });
        Takes(s.game, "legal-research", 100_000);
        Incident(s.game);
        Assert.Equal(0, TrustSystem.Of(s.game.C, s.game.Me, "legal-research"));
    }

    [Fact]
    public void The_sharper_fall_reaches_the_customer_desk_and_the_appeal()
    {
        var legal = Selling(new[] { "legal-research" });
        var plain = Selling(new[] { "legal-research" }, plain: true);
        Takes(legal.game, "legal-research", 100_000);
        Takes(plain.game, "legal-research", 100_000);
        Incident(legal.game);
        Incident(plain.game);
        Assert.True(TrustSystem.AppealPermille(legal.game.C, legal.game.Me, "legal-research") < TrustSystem.AppealPermille(plain.game.C, plain.game.Me, "legal-research"));
        Assert.Equal(legal.game.Me.Complaints[0].Stake, (int)MathI.MulPermille(legal.game.C.Trust.IncidentDrop, 1800));
    }

    // ---- the zero and the maximum

    [Fact]
    public void Flavours_at_the_bottom_of_their_range_still_leave_the_minimum_fine()
    {
        var s = Selling(new[] { "legal-research" }, tweak: c =>
        {
            SectorFlavourConfig f = Flavour(c, "legal-research");
            f.FinePermille = 250; f.ReputationPermille = 250; f.TrustPermille = 250;
        });
        Takes(s.game, "legal-research", 0);
        var cost = Incident(s.game);
        Assert.True(cost.cost > 0);
    }

    [Fact]
    public void Flavours_at_the_top_of_their_range_still_fit_in_the_books()
    {
        var s = Selling(new[] { "customer-support" }, tweak: c =>
        {
            SectorFlavourConfig f = Flavour(c, "customer-support");
            f.FinePermille = 5000; f.ReputationPermille = 5000; f.TrustPermille = 5000; f.MakeGoodWeeks = 26;
        });
        Takes(s.game, "customer-support", 1_000_000_000_000);
        s.game.Me.Reputation = 5;
        var cost = Incident(s.game);
        Assert.True(cost.cost > 26 * 1_000_000_000_000L);
        Assert.Equal(0, s.game.Me.Reputation);
    }

    [Fact]
    public void A_bankrupt_lab_still_has_its_incident_and_goes_deeper_into_debt()
    {
        var s = Selling(new[] { "customer-support" });
        Takes(s.game, "customer-support", 500_000);
        s.game.Me.Cash = -1_000;
        var cost = Incident(s.game);
        Assert.True(s.game.Me.Cash < -1_000_000);
        Assert.True(cost.cost > 1_000_000);
    }

    // ---- other systems

    [Fact]
    public void A_risk_class_and_a_flavour_multiply_each_other()
    {
        Func<bool, (long, int)> run = withFlavour =>
        {
            var s = Selling(new[] { "legal-research" }, plain: !withFlavour, tweak: c =>
            {
                c.Safety.RiskClasses.Add(TestData.Config().Safety.RiskClasses.First(r => r.Id == "data-leak"));
                c.Safety.GenericRiskWeight = 1;
                c.Safety.RiskClasses[0].Weight = 100000;
            });
            Takes(s.game, "legal-research", 1_000_000);
            var r = Incident(s.game);
            return (r.cost, r.reputation);
        };
        var plain = run(false);
        var both = run(true);
        Assert.InRange(both.Item1, plain.Item1 * 3 / 2 - 1, plain.Item1 * 3 / 2 + 1);
        Assert.InRange(both.Item2, plain.Item2 * 18 / 10 - 1, plain.Item2 * 18 / 10 + 1);
    }

    [Fact]
    public void A_law_that_bites_three_times_as_hard_multiplies_the_fine_but_not_the_refunds()
    {
        Func<int, long> run = law =>
        {
            var s = Selling(new[] { "customer-support" }, tweak: c => c.Era("agents").FinePermille = law);
            Takes(s.game, "customer-support", 500_000);
            return Incident(s.game).cost;
        };
        long one = run(1000);
        long three = run(3000);
        long refunds = 1_000_000;
        Assert.InRange(three - refunds, (one - refunds) * 3 - 3, (one - refunds) * 3 + 3);
    }

    [Fact]
    public void A_catch_by_the_staff_leaves_no_incident_and_no_refund()
    {
        var s = Selling(new[] { "customer-support" }, tweak: c => c.RolePerks.Clear());
        Takes(s.game, "customer-support", 500_000);
        long cash = s.game.Me.Cash;
        s.game.Me.Staff.Clear();
        s.game.Me.Staff.Add(new StaffMember { Id = s.game.State.TakeId(), RoleId = "safety", TraitId = "reads-the-logs", Morale = 500 });
        for (int i = 0; i < 12; i++) s.game.Me.Staff.Add(new StaffMember { Id = s.game.State.TakeId(), RoleId = "safety", TraitId = "reads-the-logs", Morale = 500 });
        Assert.True(RolePerks.IncidentCatchPermille(s.game.C, s.game.Me) > 0);
        int incidents = 0;
        for (int i = 0; i < 40; i++)
        {
            int before = s.model.Incidents;
            SafetySystem.RunWeek(s.game.C, s.game.State, s.game.Me);
            if (s.model.Incidents > before) incidents++;
        }
        Assert.True(incidents < 40, "a trait that reads the logs catches some");
        Assert.Equal(incidents, s.game.State.Events.Count(e => e.Kind == EventKind.Incident));
        Assert.True(cash - s.game.Me.Cash >= incidents * 1_000_000L);
    }

    [Fact]
    public void The_incident_line_of_the_log_carries_the_whole_bill()
    {
        var s = Selling(new[] { "customer-support" });
        Takes(s.game, "customer-support", 500_000);
        SafetySystem.RunWeek(s.game.C, s.game.State, s.game.Me);
        GameEvent e = s.game.State.Events.Last(x => x.Kind == EventKind.Incident);
        Assert.Contains("$" + e.Amount, EventText.Describe(s.game.C, s.game.State, e));
    }

    // ---- rivals

    [Fact]
    public void Rivals_fail_the_plain_way_while_the_data_says_so()
    {
        var game = new Game(Simulation.NewGame(Config(c => c.Safety.SectorFlavoursForRivals = false), new NewGameOptions { Seed = 1, StartEraId = "agents" }));
        LabState rival = game.State.Labs.First(l => l.Id != game.Id);
        var model = new ModelState { Id = 7, Sectors = new List<string> { "customer-support", "legal-research" } };
        Assert.False(SectorFlavours.Pays(game.C, game.State, rival));
        Assert.True(SectorFlavours.Pays(game.C, game.State, game.Me));
        Assert.Null(SectorFlavours.Leading(game.C, game.State, rival, model));
        Assert.Equal(0, SectorFlavours.MakeGood(game.C, game.State, rival, model));
        Assert.Equal(1000, SectorFlavours.TrustPermille(game.C, game.State, rival, "legal-research"));
        Assert.Equal(1800, SectorFlavours.TrustPermille(game.C, game.State, game.Me, "legal-research"));
    }

    [Fact]
    public void The_shipped_data_has_rivals_paying_what_a_flavour_costs_too()
    {
        var game = new Game(Simulation.NewGame(Config(), new NewGameOptions { Seed = 1, StartEraId = "agents" }));
        LabState rival = game.State.Labs.First(l => l.Id != game.Id);
        Assert.True(game.C.Safety.SectorFlavoursForRivals);
        Assert.True(SectorFlavours.Pays(game.C, game.State, rival));
    }

    [Fact]
    public void A_rival_with_takings_in_a_flavoured_sector_owes_refunds_when_rivals_pay()
    {
        var game = new Game(Simulation.NewGame(Config(), new NewGameOptions { Seed = 1, StartEraId = "agents" }));
        LabState rival = game.State.Labs.First(l => l.Id != game.Id);
        var model = new ModelState { Id = 7, Sectors = new List<string> { "customer-support" } };
        game.State.Sector("customer-support").Entries.Add(new SectorEntry { LabId = rival.Id, ModelId = 7, LastRevenue = 10_000 });
        Assert.Equal(20_000, SectorFlavours.MakeGood(game.C, game.State, rival, model));
        game.C.Safety.SectorFlavoursForRivals = false;
        Assert.Equal(0, SectorFlavours.MakeGood(game.C, game.State, rival, model));
    }

    [Fact]
    public void Turning_the_switch_on_makes_rivals_pay_what_a_flavour_costs_too()
    {
        var game = new Game(Simulation.NewGame(Config(c => c.Safety.SectorFlavoursForRivals = true), new NewGameOptions { Seed = 1, StartEraId = "agents" }));
        LabState rival = game.State.Labs.First(l => l.Id != game.Id);
        var model = new ModelState { Id = 7, Sectors = new List<string> { "legal-research" } };
        Assert.True(SectorFlavours.Pays(game.C, game.State, rival));
        Assert.Equal("legal-research", SectorFlavours.Leading(game.C, game.State, rival, model).Sector);
        Assert.Equal(1800, SectorFlavours.TrustPermille(game.C, game.State, rival, "legal-research"));
    }

    [Fact]
    public void The_glitch_curve_is_a_quality_curve_and_belongs_to_the_sector_for_every_lab()
    {
        GameConfig c = Config();
        var model = new ModelState { Capability = 100_000, Alignment = 500, DisplayedAlignment = 500, Incidents = 1, LastIncidentTick = 100 };
        Assert.True(DisplayQueries.QualityParts(c, c.Sector("legal-research"), model, 108).GlitchAdjustment < 0);
    }

    // ---- the page

    [Fact]
    public void The_page_lists_every_flavour_with_its_failure_line_and_numbers()
    {
        string page = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "docs", "content", "sector-flavours.md"));
        GameConfig c = Config();
        foreach (SectorFlavourConfig f in c.Safety.SectorFlavours)
        {
            Assert.Contains("`" + f.Sector + "`", page);
            Assert.Contains(f.Failure, page);
            string row = page.Split('\n').First(l => l.StartsWith("| `" + f.Sector + "`"));
            Assert.Contains("| " + Pct(f.FinePermille) + " | " + Pct(f.ReputationPermille) + " | " + Pct(f.TrustPermille) + " |", row);
        }
    }

    // ---- save and load

    [Fact]
    public void A_save_taken_mid_glitch_loads_with_the_same_dip()
    {
        var s = Selling(new[] { "legal-research" });
        Takes(s.game, "legal-research", 100_000);
        Incident(s.game);
        s.game.Weeks(3);
        Assert.Empty(SaveGame.Problems(s.game.C, s.game.State));
        SaveSchemaPretend.AssertRoundTrip(s.game.Sim);
        Simulation again = Simulation.Load(s.game.C, s.game.Sim.Save());
        ModelState loaded = again.State.Lab(s.game.Id).Model(s.model.Id);
        SectorConfig legal = s.game.C.Sector("legal-research");
        Assert.Equal(DisplayQueries.QualityParts(s.game.C, legal, s.model, s.game.State.Tick).GlitchAdjustment,
            DisplayQueries.QualityParts(s.game.C, legal, loaded, again.State.Tick).GlitchAdjustment);
        Assert.True(DisplayQueries.QualityParts(s.game.C, legal, loaded, again.State.Tick).GlitchAdjustment < 0);
        for (int i = 0; i < 20; i++) { s.game.Sim.Step(); again.Step(); }
        Assert.Equal(s.game.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_old_save_loads_unchanged_because_the_flavours_are_not_in_it()
    {
        var s = Selling(new[] { "legal-research" });
        string save = s.game.Sim.Save();
        Assert.DoesNotContain("sectorFlavours", save);
        Assert.DoesNotContain("makeGood", save);
        Assert.Equal(GameState.CurrentSchema, Simulation.Load(s.game.C, save).State.Schema);
    }

    [Fact]
    public void The_flavours_live_in_the_config_and_survive_its_round_trip()
    {
        GameConfig c = Config();
        GameConfig again = GameConfig.FromJson(c.ToJson());
        Assert.Equal(c.Safety.SectorFlavours.Count, again.Safety.SectorFlavours.Count);
        Assert.Equal("customer-support", again.Safety.SectorFlavours.First(f => f.Id == "customer-support-flavour").Sector);
        Assert.Equal(2, again.Safety.SectorFlavour("customer-support").MakeGoodWeeks);
    }

    [Fact]
    public void Two_runs_of_the_same_game_land_on_the_same_digest()
    {
        Func<string> run = () =>
        {
            var s = Selling(new[] { "customer-support", "legal-research" });
            Takes(s.game, "customer-support", 300_000);
            Takes(s.game, "legal-research", 200_000);
            s.game.Weeks(30);
            return s.game.Sim.Digest();
        };
        Assert.Equal(run(), run());
    }

    [Fact]
    public void A_long_game_with_flavours_keeps_the_books_and_the_save_valid()
    {
        Game g = Game.Solo(seed: 5).WithStarterLab(gpus: 12);
        ModelState model = g.Train("mini", 0);
        g.UntilTrained(model, 400);
        g.Release(model, "game-ai", "game-bots");
        g.Years(4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.All(g.State.Labs, l => Assert.True(l.Ledger.Lifetime.Fines <= 0));
    }
}
