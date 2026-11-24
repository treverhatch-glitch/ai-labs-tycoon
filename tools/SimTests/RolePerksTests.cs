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

/// <summary>The roles beyond the first four do something: each has a perk, read from the staff a lab has today.</summary>
public class RolePerksTests
{
    /// <summary>A lone lab that starts in an era, with money enough that cost is never the question.</summary>
    static Game Lab(string eraId, Action<GameConfig> tweak = null, long seed = 1)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = eraId, RivalIds = new List<string>() };
        var g = new Game(Simulation.NewGame(TestData.Config(c => { c.RoomPerks.Clear(); foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0; if (tweak != null) tweak(c); }), options));
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    static Game WithRecruiters(int recruiters, string eraId = "chat-boom", Action<GameConfig> tweak = null)
    {
        Game g = Lab(eraId, tweak);
        g.Build("recruiting-office", 6, 6);
        if (recruiters > 0) g.Hire("recruiter", recruiters);
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

    // ---- the table

    [Fact]
    public void Every_perk_names_a_real_role_and_says_what_it_does()
    {
        GameConfig c = TestData.Config();
        Assert.NotEmpty(c.RolePerks);
        foreach (RolePerkConfig perk in c.RolePerks)
        {
            Assert.NotNull(c.Role(perk.RoleId));
            Assert.False(string.IsNullOrWhiteSpace(perk.Blurb), perk.Id);
            Assert.InRange(perk.CapPermille, perk.PerStaffPermille, 900);
        }
        Assert.Equal(c.RolePerks.Count, c.RolePerks.Select(p => p.Id).Distinct().Count());
    }

    [Fact]
    public void A_role_without_a_perk_has_none_and_the_recruiter_has_the_hiring_discount()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(RolePerks.Of(c, "researcher"));
        Assert.Equal(new[] { RolePerkKind.HireDiscount }, RolePerks.Of(c, "recruiter").Select(p => p.Kind).ToArray());
    }

    // ---- the recruiter

    [Fact]
    public void With_no_recruiters_a_hire_costs_the_eras_fee()
    {
        Game g = WithRecruiters(0);
        RoleConfig role = g.C.Role("researcher");
        Assert.Equal(0, RolePerks.Permille(g.C, g.Me, RolePerkKind.HireDiscount));
        Assert.Equal(EraRules.HireFee(g.C, role, g.Tick), RolePerks.HireFee(g.C, g.Me, role, g.Tick));
    }

    [Fact]
    public void Each_recruiter_wins_back_a_share_of_every_fee()
    {
        Game g = WithRecruiters(3);
        RoleConfig role = g.C.Role("researcher");
        RolePerkConfig perk = RolePerks.Of(g.C, "recruiter")[0];
        Assert.Equal(3 * perk.PerStaffPermille, RolePerks.Permille(g.C, g.Me, RolePerkKind.HireDiscount));
        long eraFee = EraRules.HireFee(g.C, role, g.Tick);
        Assert.Equal(MathI.MulPermille(eraFee, 1000 - 3 * perk.PerStaffPermille), RolePerks.HireFee(g.C, g.Me, role, g.Tick));
    }

    [Fact]
    public void The_discount_stops_at_the_cap_however_many_recruiters_there_are()
    {
        Game g = WithRecruiters(9);
        RolePerkConfig perk = RolePerks.Of(g.C, "recruiter")[0];
        Assert.True(9 * perk.PerStaffPermille > perk.CapPermille);
        Assert.Equal(perk.CapPermille, RolePerks.Permille(g.C, g.Me, RolePerkKind.HireDiscount));
    }

    [Fact]
    public void The_hire_command_charges_the_discounted_fee()
    {
        Game g = WithRecruiters(4);
        g.Build("research-lab", 4, 4);
        long before = g.Me.Cash;
        RoleConfig role = g.C.Role("researcher");
        long expected = RolePerks.HireCost(g.C, g.Me, role, 3, g.Tick);
        g.Hire("researcher", 3);
        Assert.Equal(expected, before - g.Me.Cash);
        Assert.True(expected < EraRules.HireCost(g.C, role, 3, g.Tick));
    }

    [Fact]
    public void A_hire_is_refused_when_cash_is_one_short_of_the_discounted_fee_and_goes_through_at_it()
    {
        Game g = WithRecruiters(4);
        g.Build("research-lab", 4, 4);
        long cost = RolePerks.HireCost(g.C, g.Me, g.C.Role("researcher"), 2, g.Tick);
        g.Me.Cash = cost - 1;
        CommandResult refused = g.Try(Command.Hire(g.Id, "researcher", 2));
        Assert.False(refused.Ok);
        g.Me.Cash = cost;
        Assert.True(g.Try(Command.Hire(g.Id, "researcher", 2)).Ok);
        Assert.Equal(0, g.Me.Cash);
    }

    [Fact]
    public void Letting_the_recruiters_go_takes_the_discount_with_them_the_same_week()
    {
        Game g = WithRecruiters(5);
        RoleConfig role = g.C.Role("researcher");
        long discounted = RolePerks.HireFee(g.C, g.Me, role, g.Tick);
        g.Do(Command.Fire(g.Id, "recruiter", 5));
        Assert.Equal(0, RolePerks.Permille(g.C, g.Me, RolePerkKind.HireDiscount));
        Assert.True(RolePerks.HireFee(g.C, g.Me, role, g.Tick) > discounted);
    }

    [Fact]
    public void The_breakdown_adds_up_to_the_fee_in_every_era_and_with_or_without_recruiters()
    {
        foreach (string era in new[] { "it-sees", "chat-boom", "agents", "upward" })
        {
            foreach (int recruiters in new[] { 0, 2, 9 })
            {
                Game g = WithRecruiters(recruiters, era);
                foreach (RoleConfig role in g.C.Roles)
                {
                    var b = RolePerks.HireFeeParts(g.C, g.Me, role, g.Tick);
                    Assert.Equal(RolePerks.HireFee(g.C, g.Me, role, g.Tick), b.Total);
                    Assert.Equal(role.HireCost, b.ListPrice);
                    Assert.True(b.PerkDiscount <= 0);
                    if (recruiters == 0) Assert.Equal(0, b.PerkDiscount);
                }
            }
        }
    }

    [Fact]
    public void In_the_talent_war_nine_recruiters_win_back_a_quarter_of_the_tripled_fee()
    {
        Game g = WithRecruiters(9, "chat-boom");
        RoleConfig role = g.C.Role("engineer");
        Assert.Equal(3000, g.C.Era("chat-boom").HireFeePermille);
        var b = RolePerks.HireFeeParts(g.C, g.Me, role, g.Tick);
        Assert.Equal(role.HireCost * 2, b.EraShift);
        Assert.Equal(-(role.HireCost * 3) / 4, b.PerkDiscount);
        Assert.Equal(role.HireCost * 3 - role.HireCost * 3 / 4, b.Total);
    }

    [Fact]
    public void Two_perks_of_one_kind_add_up_but_a_fee_never_goes_below_nothing()
    {
        Game g = WithRecruiters(4, tweak: c =>
        {
            c.RolePerks.Add(new RolePerkConfig { IdValue = "second", RoleId = "recruiter", Kind = RolePerkKind.HireDiscount, PerStaffPermille = 500, CapPermille = 900, Blurb = "x" });
            c.RolePerks.Add(new RolePerkConfig { IdValue = "third", RoleId = "recruiter", Kind = RolePerkKind.HireDiscount, PerStaffPermille = 500, CapPermille = 900, Blurb = "x" });
        });
        Assert.True(RolePerks.Permille(g.C, g.Me, RolePerkKind.HireDiscount) > 1000);
        Assert.Equal(0, RolePerks.HireFee(g.C, g.Me, g.C.Role("researcher"), g.Tick));
    }

    [Fact]
    public void A_perk_is_not_saved_it_comes_back_from_the_staff_after_a_load()
    {
        Game g = WithRecruiters(6);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(RolePerks.Permille(g.C, g.Me, RolePerkKind.HireDiscount), RolePerks.Permille(again.Config, again.Player, RolePerkKind.HireDiscount));
    }

    [Fact]
    public void A_rival_plan_budgets_hires_with_the_same_discounted_fee()
    {
        var g = new Game(Simulation.NewGame(TestData.Config(), new NewGameOptions { Seed = 2, StartEraId = "chat-boom" }));
        g.Autopilot(48);
        foreach (LabState lab in g.State.Labs.Where(l => !l.Bankrupt))
            foreach (RoleConfig role in g.C.Roles)
            {
                var b = RolePerks.HireFeeParts(g.C, lab, role, g.Tick);
                Assert.Equal(RolePerks.HireFee(g.C, lab, role, g.Tick), b.Total);
            }
    }

    // ---- the red-teamer

    /// <summary>A lab selling one released model that is bound to have incidents, with a room for red-teamers and as many as asked for.</summary>
    static (Game game, ModelState model) Testing(int redTeamers, Action<GameConfig> tweak = null)
    {
        Game g = Lab("chat-boom", c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            if (tweak != null) tweak(c);
        });
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, "consumer-assistant"));
        model.Alignment = 300;
        model.DisplayedAlignment = 300;
        g.Build("sandbox-wing", 8, 6);
        if (redTeamers > 0) g.Hire("red-teamer", redTeamers);
        g.Me.Cash = 1_000_000_000_000_000;
        return (g, model);
    }

    static int Incidents(Game g) => g.State.Events.Count(e => e.Kind == EventKind.Incident);

    [Fact]
    public void Without_red_teamers_nothing_is_caught_and_the_odds_are_the_models()
    {
        var (g, model) = Testing(0);
        var odds = RolePerks.IncidentOdds(g.C, g.Me, g.Tick, model);
        Assert.Equal(0, odds.CaughtByRedTeam);
        Assert.Equal(SafetySystem.IncidentChancePermille(g.C, g.Tick, model), odds.Total);
        Assert.Equal(0, RolePerks.IncidentCatchPermille(g.C, g.Me));
    }

    [Fact]
    public void Each_red_teamer_catches_a_share_up_to_the_cap_and_the_parts_add_up()
    {
        var (g, model) = Testing(4);
        RolePerkConfig perk = RolePerks.Of(g.C, "red-teamer")[0];
        Assert.Equal(4 * perk.PerStaffPermille, RolePerks.IncidentCatchPermille(g.C, g.Me));
        var odds = RolePerks.IncidentOdds(g.C, g.Me, g.Tick, model);
        Assert.True(odds.CaughtByRedTeam < 0);
        Assert.Equal(odds.BeforeRedTeam + odds.CaughtByRedTeam, odds.Total);

        g.Hire("red-teamer", 8);
        Assert.Equal(perk.CapPermille, RolePerks.IncidentCatchPermille(g.C, g.Me));
    }

    [Fact]
    public void A_full_team_of_red_teamers_means_fewer_incidents_over_the_same_weeks()
    {
        var (bare, _) = Testing(0);
        var (guarded, _) = Testing(12);
        bare.Weeks(100);
        guarded.Weeks(100);
        int without = Incidents(bare), with = Incidents(guarded);
        Assert.True(without > 40, "expected plenty of incidents without a team, got " + without);
        Assert.InRange(with, without / 2, without * 9 / 10);
    }

    [Fact]
    public void A_lab_with_no_red_team_plays_exactly_as_it_did_before_the_perk_existed()
    {
        var (with, _) = Testing(0);
        var (without, _) = Testing(0, c => c.RolePerks.Clear());
        with.Weeks(30);
        without.Weeks(30);
        Assert.Equal(without.Sim.Digest(), with.Sim.Digest());
    }

    [Fact]
    public void Even_at_the_most_a_config_allows_some_incidents_still_get_through()
    {
        var (g, _) = Testing(1, c => { RolePerkConfig cut = c.RolePerks.First(p => p.Kind == RolePerkKind.IncidentCut); cut.PerStaffPermille = 900; cut.CapPermille = 900; });
        Assert.Equal(900, RolePerks.IncidentCatchPermille(g.C, g.Me));
        g.Weeks(200);
        Assert.InRange(Incidents(g), 1, 40);
    }

    [Fact]
    public void Letting_the_red_team_go_brings_the_odds_back()
    {
        var (g, model) = Testing(6);
        int guarded = RolePerks.IncidentOdds(g.C, g.Me, g.Tick, model).Total;
        g.Do(Command.Fire(g.Id, "red-teamer", 6));
        Assert.True(RolePerks.IncidentOdds(g.C, g.Me, g.Tick, model).Total > guarded);
    }

    [Fact]
    public void A_lab_with_nothing_on_sale_has_no_incidents_to_catch()
    {
        Game g = Lab("chat-boom", c => c.Safety.IncidentChancePermille = 1000);
        g.WithStarterLab();
        g.Build("sandbox-wing", 6, 6);
        g.Hire("red-teamer", 5);
        g.Weeks(20);
        Assert.Equal(0, Incidents(g));
    }

    [Fact]
    public void The_red_team_does_not_change_what_an_incident_costs_or_which_kind_it_is()
    {
        var (g, _) = Testing(12);
        g.Weeks(100);
        List<GameEvent> hits = g.State.Events.Where(e => e.Kind == EventKind.Incident).ToList();
        Assert.NotEmpty(hits);
        Assert.Contains(hits, e => e.RiskClass != "");
        Assert.All(hits, e => Assert.True(e.Amount > 0));
        Assert.All(hits, e => Assert.False(string.IsNullOrEmpty(EventText.Describe(g.C, g.State, e))));
    }

    [Fact]
    public void A_game_with_a_red_team_saves_and_loads_and_plays_on_the_same()
    {
        var (g, _) = Testing(8);
        g.Weeks(20);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(30);
        again.StepMany(30);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    // ---- the alignment fellow

    static (Game game, ModelState model) Aligning(int fellows, Action<GameConfig> tweak = null, int share = 120)
    {
        Game g = Lab("agents", tweak);
        g.WithStarterLab();
        g.Build("containment-wing", 6, 6);
        if (fellows > 0) g.Hire("alignment-fellow", fellows);
        ModelState model = g.Train("mini", share);
        return (g, model);
    }

    static void NoPerks(GameConfig c) => c.RolePerks.Clear();

    [Fact]
    public void Each_alignment_fellow_adds_to_the_work_a_unit_of_alignment_compute_does_beyond_being_safety_staff()
    {
        var (with, _) = Aligning(3);
        var (without, _) = Aligning(3, NoPerks);
        RolePerkConfig perk = RolePerks.Of(with.C, "alignment-fellow")[0];
        Assert.Equal(3 * perk.PerStaffPermille, RolePerks.Permille(with.C, with.Me, RolePerkKind.AlignmentWork));
        Assert.Equal(0, RolePerks.Permille(without.C, without.Me, RolePerkKind.AlignmentWork));
        Assert.Equal(TrainingSystem.AlignmentWorkPermille(without.C, without.Me) + 3 * perk.PerStaffPermille,
            TrainingSystem.AlignmentWorkPermille(with.C, with.Me));
    }

    [Fact]
    public void Fellows_stop_helping_at_the_cap()
    {
        var (g, _) = Aligning(9);
        RolePerkConfig perk = RolePerks.Of(g.C, "alignment-fellow")[0];
        Assert.Equal(9 * perk.PerStaffPermille, RolePerks.Permille(g.C, g.Me, RolePerkKind.AlignmentWork));
        g.Build("containment-wing", 8, 6);
        g.Hire("alignment-fellow", 3);
        Assert.Equal(perk.CapPermille, RolePerks.Permille(g.C, g.Me, RolePerkKind.AlignmentWork));
    }

    [Fact]
    public void A_run_with_the_perk_gathers_more_alignment_in_the_same_weeks_for_the_same_progress()
    {
        var (without, noModel) = Aligning(8, NoPerks);
        var (with, model) = Aligning(8);
        without.Weeks(6);
        with.Weeks(6);
        Assert.True(noModel.AlignmentCompute > 0);
        Assert.True(model.AlignmentCompute > noModel.AlignmentCompute);
        Assert.Equal(noModel.ProgressCompute, model.ProgressCompute);
    }

    [Fact]
    public void A_trained_model_comes_out_better_aligned_and_just_as_capable()
    {
        var (without, noModel) = Aligning(8, NoPerks, 20);
        var (with, model) = Aligning(8, null, 20);
        without.UntilTrained(noModel);
        with.UntilTrained(model);
        Assert.True(model.Alignment > noModel.Alignment, model.Alignment + " vs " + noModel.Alignment);
        Assert.Equal(noModel.CapBase + noModel.CapTech, model.CapBase + model.CapTech);
    }

    [Fact]
    public void Without_any_fellows_the_perk_changes_nothing_at_all()
    {
        var (g, _) = Aligning(0);
        var (h, _) = Aligning(0, NoPerks);
        g.Weeks(10);
        h.Weeks(10);
        Assert.Equal(h.Sim.Digest(), g.Sim.Digest());
    }

    [Fact]
    public void Letting_the_fellows_go_takes_the_perk_with_them_the_same_week()
    {
        var (g, _) = Aligning(4);
        int withFellows = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        g.Do(Command.Fire(g.Id, "alignment-fellow", 4));
        int without = TrainingSystem.AlignmentWorkPermille(g.C, g.Me);
        Assert.True(withFellows - without >= 4 * RolePerks.Of(g.C, "alignment-fellow")[0].PerStaffPermille);
        Assert.Equal(0, RolePerks.Permille(g.C, g.Me, RolePerkKind.AlignmentWork));
    }

    [Fact]
    public void The_perk_and_the_containment_wing_both_count_towards_the_work()
    {
        var (g, _) = Aligning(4);
        int wing = LabQueries.ContainmentAlignmentBonus(g.C, g.Me);
        Assert.True(wing > 0);
        int perk = RolePerks.Permille(g.C, g.Me, RolePerkKind.AlignmentWork);
        Assert.True(TrainingSystem.AlignmentWorkPermille(g.C, g.Me) >= 1000 + wing + perk);
    }

    // ---- the product lead and the specialists

    /// <summary>A lab with a released model on sale in the given sectors and a team of one role in its own room.</summary>
    static (Game game, ModelState model) Appealing(string eraId, string[] sectors, string roomId, string roleId, int team, Action<GameConfig> tweak = null, bool rivals = false)
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = eraId };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c => { c.RoomPerks.Clear(); if (tweak != null) tweak(c); }), options));
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 100_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, sectors));
        g.Me.Cash = 1_000_000_000_000_000;
        g.Build(roomId, 8, 6);
        if (team > 0) g.Hire(roleId, team);
        return (g, model);
    }

    static DisplayQueries.AppealBreakdown AppealIn(Game g, ModelState model, string sectorId)
    {
        SectorConfig sector = g.C.Sector(sectorId);
        return DisplayQueries.AppealParts(g.C, g.State, sector, g.State.Sector(sectorId), g.Me, model);
    }

    [Fact]
    public void A_lab_without_the_role_gets_no_boost_in_any_sector()
    {
        var (g, model) = Appealing("chat-boom", new[] { "consumer-assistant" }, "product-studio", "product-lead", 0);
        Assert.Equal(0, AppealIn(g, model, "consumer-assistant").RoleBoost);
        Assert.Equal(0, RolePerks.SectorAppealPermille(g.C, g.Me, "consumer-assistant"));
    }

    [Fact]
    public void Each_product_lead_raises_appeal_in_every_sector_the_lab_sells_in()
    {
        var (g, model) = Appealing("chat-boom", new[] { "consumer-assistant", "image-design" }, "product-studio", "product-lead", 3);
        RolePerkConfig perk = RolePerks.Of(g.C, "product-lead")[0];
        Assert.Equal(3 * perk.PerStaffPermille, RolePerks.SectorAppealPermille(g.C, g.Me, "consumer-assistant"));
        Assert.Equal(3 * perk.PerStaffPermille, RolePerks.SectorAppealPermille(g.C, g.Me, "image-design"));
        Assert.True(AppealIn(g, model, "consumer-assistant").RoleBoost > 0);
        Assert.True(AppealIn(g, model, "image-design").RoleBoost > 0);
    }

    [Fact]
    public void The_product_leads_boost_stops_at_the_cap()
    {
        var (g, _) = Appealing("chat-boom", new[] { "consumer-assistant" }, "product-studio", "product-lead", 9);
        RolePerkConfig perk = RolePerks.Of(g.C, "product-lead")[0];
        Assert.True(9 * perk.PerStaffPermille > perk.CapPermille);
        Assert.Equal(perk.CapPermille, RolePerks.SectorAppealPermille(g.C, g.Me, "consumer-assistant"));
    }

    [Fact]
    public void A_specialist_helps_only_in_the_sectors_of_its_perk()
    {
        var (g, model) = Appealing("agents", new[] { "coding-agents", "consumer-assistant" }, "agent-ops-room", "agent-wrangler", 4);
        RolePerkConfig perk = RolePerks.Of(g.C, "agent-wrangler")[0];
        Assert.Contains("coding-agents", perk.Sectors);
        Assert.DoesNotContain("consumer-assistant", perk.Sectors);
        Assert.True(AppealIn(g, model, "coding-agents").RoleBoost > 0);
        Assert.Equal(0, AppealIn(g, model, "consumer-assistant").RoleBoost);
    }

    [Fact]
    public void A_product_lead_and_a_specialist_add_up_in_the_specialists_sector()
    {
        var (g, _) = Appealing("agents", new[] { "coding-agents" }, "agent-ops-room", "agent-wrangler", 2);
        g.Build("product-studio", 6, 6);
        g.Hire("product-lead", 2);
        int lead = 2 * RolePerks.Of(g.C, "product-lead")[0].PerStaffPermille;
        int wrangler = 2 * RolePerks.Of(g.C, "agent-wrangler")[0].PerStaffPermille;
        Assert.Equal(lead + wrangler, RolePerks.SectorAppealPermille(g.C, g.Me, "coding-agents"));
    }

    [Fact]
    public void Every_specialist_role_has_a_sector_perk_in_sectors_that_exist()
    {
        GameConfig c = TestData.Config();
        foreach (string role in new[] { "product-lead", "agent-wrangler", "harness-operator", "roboticist", "mission-controller" })
        {
            List<RolePerkConfig> perks = RolePerks.Of(c, role);
            Assert.Single(perks);
            Assert.Equal(RolePerkKind.SectorAppeal, perks[0].Kind);
            foreach (string sector in perks[0].Sectors) Assert.NotNull(c.Sector(sector));
        }
        Assert.Empty(RolePerks.Of(c, "product-lead")[0].Sectors);
    }

    [Fact]
    public void The_appeal_parts_still_add_up_with_a_team_and_a_crown()
    {
        var (g, model) = Appealing("agents", new[] { "coding-agents" }, "agent-ops-room", "agent-wrangler", 5);
        g.State.Sector("coding-agents").ChampionLabId = g.Id;
        var parts = AppealIn(g, model, "coding-agents");
        Assert.True(parts.RoleBoost > 0);
        Assert.True(parts.ChampionBoost > 0);
        Assert.Equal(parts.Total, MarketSystem.Appeal(g.C, g.State, g.C.Sector("coding-agents"), g.State.Sector("coding-agents"), g.Me, model));
        Assert.Equal(parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.RoleBoost + parts.TrustBoost + parts.ChampionBoost, parts.Total);
    }

    [Fact]
    public void Letting_the_team_go_takes_the_boost_away_the_same_week()
    {
        var (g, model) = Appealing("agents", new[] { "coding-agents" }, "agent-ops-room", "agent-wrangler", 4);
        Assert.True(AppealIn(g, model, "coding-agents").RoleBoost > 0);
        g.Do(Command.Fire(g.Id, "agent-wrangler", 4));
        Assert.Equal(0, AppealIn(g, model, "coding-agents").RoleBoost);
    }

    [Fact]
    public void Against_rivals_a_product_team_wins_a_bigger_share_of_the_sector()
    {
        var (with, _) = Appealing("chat-boom", new[] { "voice-assistants" }, "product-studio", "product-lead", 8, rivals: true);
        var (without, _) = Appealing("chat-boom", new[] { "voice-assistants" }, "product-studio", "product-lead", 8, c => c.RolePerks.Clear(), rivals: true);
        with.Weeks(30);
        without.Weeks(30);
        SectorEntry a = with.State.Sector("voice-assistants").Entry(with.Id);
        SectorEntry b = without.State.Sector("voice-assistants").Entry(without.Id);
        Assert.NotNull(a);
        Assert.NotNull(b);
        Assert.True(with.State.Sector("voice-assistants").Entries.Count > 1, "needs a rival in the sector for share to be contested");
        Assert.True(a.SharePpm > b.SharePpm, a.SharePpm + " vs " + b.SharePpm);
    }

    [Fact]
    public void Without_the_perk_table_the_market_plays_as_before()
    {
        var (g, _) = Appealing("chat-boom", new[] { "consumer-assistant" }, "product-studio", "product-lead", 0);
        var (h, _) = Appealing("chat-boom", new[] { "consumer-assistant" }, "product-studio", "product-lead", 0, c => c.RolePerks.Clear());
        g.Weeks(20);
        h.Weeks(20);
        Assert.Equal(h.Sim.Digest(), g.Sim.Digest());
    }

    [Fact]
    public void The_role_perks_page_lists_every_perk_and_its_role()
    {
        GameConfig c = TestData.Config();
        string page = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "docs", "content", "role-perks.md"));
        foreach (RolePerkConfig perk in c.RolePerks)
        {
            Assert.Contains("`" + perk.Id + "`", page);
            Assert.Contains("`" + perk.RoleId + "`", page);
        }
    }

    // ---- the validator and the data

    [Fact]
    public void A_perk_that_could_not_work_is_a_config_problem()
    {
        AssertProblem(Problems(c => c.RolePerks[0].RoleId = "wizard"), "wizard");
        AssertProblem(Problems(c => c.RolePerks[0].PerStaffPermille = 0), "perStaffPermille");
        AssertProblem(Problems(c => c.RolePerks[0].CapPermille = 1000), "cap");
        AssertProblem(Problems(c => c.RolePerks[0].CapPermille = 1), "cap");
        AssertProblem(Problems(c => c.RolePerks[0].Blurb = ""), "blurb");
        AssertProblem(Problems(c => c.RolePerks[0].Sectors.Add("moon-mining")), "moon-mining");
        AssertProblem(Problems(c => c.RolePerks.Add(c.RolePerks[0])), "rolePerks");
        Assert.Empty(Problems(c => c.RolePerks.Clear()));
    }

    [Fact]
    public void The_perk_table_is_strict_json_and_round_trips()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(c.ToJson(), GameConfig.FromJson(c.ToJson()).ToJson());
        string missing = TestData.ConfigJson().Replace("\"rolePerks\"", "\"rolePerksX\"");
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(missing));
        string unknownKind = TestData.ConfigJson().Replace("\"HireDiscount\"", "\"Telepathy\"");
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(unknownKind));
    }
}
