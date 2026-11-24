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

/// <summary>The workplace: comfort rooms lift where morale settles, defend against rival offers, and morale moves research.</summary>
public class WorkplaceTests
{
    /// <summary>These tests isolate comfort and morale, not the trait roll on hire (Q184).</summary>
    static void OnlySteadyTraits(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits)
            if (t.Id != "steady") t.HirePermille = 0;
    }

    static Game Lab(string eraId = "it-sees", Action<GameConfig> tweak = null, bool rivals = false)
    {
        var options = new NewGameOptions { Seed = 3, StartEraId = eraId };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c => { c.RolePerks.Clear(); OnlySteadyTraits(c); c.Ageing.MaxMoralePenalty = 0; c.Shocks.Enabled = 0; if (tweak != null) tweak(c); }), options));
        g.Me.Cash = 1_000_000_000_000;
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

    static RoomPerkConfig Perk(GameConfig c, string id) => c.RoomPerks.First(p => p.Id == id);

    static long Research(Game g, int tick = 0)
    {
        long before = g.Me.ResearchPoints;
        TrainingSystem.RunResearch(g.C, g.Me, tick == 0 ? g.Tick : tick);
        return g.Me.ResearchPoints - before;
    }

    // ---- the table

    [Fact]
    public void There_are_three_comfort_rooms_that_seat_nobody_and_open_in_three_eras()
    {
        GameConfig c = TestData.Config();
        var comfort = c.RoomTypes.Where(r => r.Purpose == RoomPurpose.Comfort).ToList();
        Assert.Equal(new[] { "coffee-corner", "kitchen", "nap-pods" }, comfort.Select(r => r.Id).ToArray());
        Assert.Equal(new[] { "dial-up-days", "it-sees", "chat-boom" }, comfort.Select(r => r.Era).ToArray());
        foreach (RoomTypeConfig r in comfort)
        {
            Assert.Equal("", r.SeatsRole);
            Assert.Equal(0, r.GpuSlotsPerTile);
        }
    }

    [Fact]
    public void The_morale_perks_are_on_the_comfort_rooms_and_the_defence_is_on_the_kitchen_and_the_recruiting_office()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "coffee-corner", "kitchen", "nap-pods" }, c.RoomPerks.Where(p => p.Kind == RoomPerkKind.MoraleLift).Select(p => p.RoomId).OrderBy(x => x).ToArray());
        Assert.Equal(new[] { "kitchen", "recruiting-office" }, c.RoomPerks.Where(p => p.Kind == RoomPerkKind.PoachDefence).Select(p => p.RoomId).OrderBy(x => x).ToArray());
    }

    [Fact]
    public void The_three_new_settings_are_in_the_data_and_nothing_ships_with_a_free_lunch_at_the_cap()
    {
        StaffPeopleConfig s = TestData.Config().StaffPeople;
        Assert.Equal(150, s.MaxMoraleLift);
        Assert.Equal(600, s.MaxPoachDefencePermille);
        Assert.Equal(400, s.MoraleResearchSlopePermille);
        Assert.Equal(0, s.MoraleResearchLossSlopePermille);
        Assert.True(s.MoraleBaseline + s.MaxMoraleLift <= 1000);
    }

    [Fact]
    public void The_validator_keeps_the_lift_in_range_and_never_rules_an_offer_out()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => c.StaffPeople.MaxMoraleLift = -1), "maxMoraleLift");
        AssertProblem(Problems(c => c.StaffPeople.MaxMoraleLift = 501), "maxMoraleLift");
        Assert.DoesNotContain(Problems(c => c.StaffPeople.MaxMoraleLift = 500), p => p.Contains("maxMoraleLift"));
        AssertProblem(Problems(c => c.StaffPeople.MaxPoachDefencePermille = 901), "maxPoachDefencePermille");
        AssertProblem(Problems(c => c.StaffPeople.MaxPoachDefencePermille = -1), "maxPoachDefencePermille");
        Assert.DoesNotContain(Problems(c => c.StaffPeople.MaxPoachDefencePermille = 900), p => p.Contains("maxPoachDefencePermille"));
        AssertProblem(Problems(c => c.StaffPeople.MoraleResearchSlopePermille = -1), "moraleResearchSlopePermille");
        AssertProblem(Problems(c => c.StaffPeople.MoraleResearchSlopePermille = 2001), "moraleResearchSlopePermille");
        AssertProblem(Problems(c => c.StaffPeople.MoraleResearchLossSlopePermille = -1), "moraleResearchLossSlopePermille");
        AssertProblem(Problems(c => c.StaffPeople.MoraleResearchLossSlopePermille = 2001), "moraleResearchLossSlopePermille");
    }

    [Fact]
    public void A_comfort_perk_follows_the_usual_perk_rules()
    {
        AssertProblem(Problems(c => Perk(c, "kitchen-lift").CapPermille = 1), "cap");
        AssertProblem(Problems(c => Perk(c, "kitchen-stay").RoomId = "no-such-room"), "no-such-room");
        AssertProblem(Problems(c => Perk(c, "nap-pods-lift").Sectors.Add("game-ai")), "sectors");
    }

    // ---- the numbers

    [Fact]
    public void With_no_comfort_room_nothing_moves()
    {
        Game g = Lab();
        WorkplaceReport r = Workplace.Report(g.C, g.Me);
        Assert.Equal(0, r.MoraleLift);
        Assert.Equal(500, r.RestingMorale);
        Assert.Equal(0, r.PoachDefencePermille);
        Assert.Equal(g.C.StaffPeople.PoachChancePermille, r.PoachChancePermille);
        Assert.Equal(1000, r.ResearchPermille);
        Assert.Equal(0, r.ResearchMorale);
    }

    [Fact]
    public void The_lift_is_the_tiles_times_the_points_a_tile()
    {
        Game g = Lab("dial-up-days");
        g.Build("coffee-corner", 2, 2);
        Assert.Equal(12, Workplace.MoraleLift(g.C, g.Me));
        Assert.Equal(512, Workplace.RestingMorale(g.C, g.Me));
        g.Build("coffee-corner", 3, 3);
        Assert.Equal(39, Workplace.MoraleLift(g.C, g.Me));
    }

    [Fact]
    public void One_room_stops_at_its_own_cap()
    {
        Game g = Lab("dial-up-days");
        g.Build("coffee-corner", 5, 4);
        g.Build("coffee-corner", 4, 4);
        Assert.Equal(36, RoomPerks.Tiles(g.Me, "coffee-corner"));
        Assert.Equal(45, Workplace.MoraleLift(g.C, g.Me));
    }

    [Fact]
    public void Three_rooms_add_up_and_the_total_stops_at_the_game_wide_cap()
    {
        Game g = Lab("chat-boom");
        g.Build("coffee-corner", 5, 4);
        g.Build("kitchen", 5, 4);
        g.Build("nap-pods", 5, 4);
        Assert.Equal(45 + 80 + 100, RoomPerks.Permille(g.C, g.Me, RoomPerkKind.MoraleLift));
        Assert.Equal(150, Workplace.MoraleLift(g.C, g.Me));
        Assert.Equal(650, Workplace.RestingMorale(g.C, g.Me));
    }

    [Fact]
    public void The_game_wide_cap_can_be_turned_off_in_the_data()
    {
        Game g = Lab("chat-boom", c => c.StaffPeople.MaxMoraleLift = 0);
        g.Build("kitchen", 5, 4);
        Assert.Equal(80, RoomPerks.Permille(g.C, g.Me, RoomPerkKind.MoraleLift));
        Assert.Equal(0, Workplace.MoraleLift(g.C, g.Me));
        Assert.Equal(500, Workplace.RestingMorale(g.C, g.Me));
    }

    [Fact]
    public void Offer_defence_adds_up_across_the_kitchen_and_the_recruiting_office_and_cuts_the_chance()
    {
        Game g = Lab();
        g.Build("kitchen", 3, 3);
        Assert.Equal(72, Workplace.PoachDefencePermille(g.C, g.Me));
        g.Build("recruiting-office", 3, 3);
        Assert.Equal(72 + 90, Workplace.PoachDefencePermille(g.C, g.Me));
        Assert.Equal(80 * (1000 - 162) / 1000, Workplace.PoachChancePermille(g.C, g.Me));
    }

    [Fact]
    public void Offer_defence_stops_at_the_game_wide_cap_and_the_chance_never_reaches_zero()
    {
        Game g = Lab("it-sees", c => { c.StaffPeople.MaxPoachDefencePermille = 100; c.StaffPeople.PoachChancePermille = 500; });
        g.Build("kitchen", 5, 5);
        g.Build("recruiting-office", 5, 5);
        Assert.Equal(100, Workplace.PoachDefencePermille(g.C, g.Me));
        Assert.Equal(450, Workplace.PoachChancePermille(g.C, g.Me));
        Game full = Lab("it-sees", c => c.StaffPeople.MaxPoachDefencePermille = 900);
        full.Build("kitchen", 5, 5);
        full.Build("recruiting-office", 5, 5);
        Assert.Equal(450, Workplace.PoachDefencePermille(full.C, full.Me));
    }

    [Fact]
    public void Research_by_morale_rises_above_the_baseline_and_is_normal_at_and_below_it_in_the_shipped_data()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1000, Workplace.ResearchPermille(c, 500));
        Assert.Equal(1060, Workplace.ResearchPermille(c, 650));
        Assert.Equal(1200, Workplace.ResearchPermille(c, 1000));
        Assert.Equal(1000, Workplace.ResearchPermille(c, 490));
        Assert.Equal(1000, Workplace.ResearchPermille(c, 0));
    }

    [Fact]
    public void A_loss_slope_makes_unhappy_researchers_slower_on_a_straight_line()
    {
        GameConfig c = TestData.Config();
        c.StaffPeople.MoraleResearchLossSlopePermille = 400;
        Assert.Equal(1000, Workplace.ResearchPermille(c, 500));
        Assert.Equal(800, Workplace.ResearchPermille(c, 0));
        Assert.Equal(996, Workplace.ResearchPermille(c, 490));
        Assert.Equal(1200, Workplace.ResearchPermille(c, 1000));
    }

    [Fact]
    public void Research_by_morale_never_falls_under_a_tenth_and_zero_slopes_ignore_morale()
    {
        GameConfig c = TestData.Config();
        c.StaffPeople.MoraleResearchSlopePermille = 2000;
        c.StaffPeople.MoraleResearchLossSlopePermille = 2000;
        Assert.Equal(100, Workplace.ResearchPermille(c, 0));
        Assert.Equal(2000, Workplace.ResearchPermille(c, 1000));
        c.StaffPeople.MoraleResearchSlopePermille = 0;
        c.StaffPeople.MoraleResearchLossSlopePermille = 0;
        Assert.Equal(1000, Workplace.ResearchPermille(c, 0));
        Assert.Equal(1000, Workplace.ResearchPermille(c, 1000));
    }

    // ---- morale settles where the rooms say

    static Game Staffed(string era = "it-sees", Action<GameConfig> tweak = null, bool rivals = false, int researchers = 2)
    {
        Game g = Lab(era, tweak, rivals);
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", researchers);
        return g;
    }

    [Fact]
    public void A_new_hire_starts_at_the_resting_level()
    {
        Game g = Lab();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 1);
        Assert.Equal(500, g.Me.Staff[0].Morale);
        g.Build("kitchen", 3, 3);
        g.Hire("researcher", 1);
        Assert.Equal(536, g.Me.Staff[1].Morale);
    }

    [Fact]
    public void Morale_climbs_a_point_a_week_to_the_resting_level_and_stops_there()
    {
        Game g = Staffed();
        g.Build("kitchen", 3, 3);
        g.Weeks(1);
        Assert.All(g.Me.Staff, p => Assert.Equal(501, p.Morale));
        g.Weeks(35);
        Assert.All(g.Me.Staff, p => Assert.Equal(536, p.Morale));
        g.Weeks(10);
        Assert.All(g.Me.Staff, p => Assert.Equal(536, p.Morale));
    }

    [Fact]
    public void Without_a_comfort_room_morale_stays_where_it_was()
    {
        Game g = Staffed();
        g.Weeks(20);
        Assert.All(g.Me.Staff, p => Assert.Equal(500, p.Morale));
    }

    [Fact]
    public void People_above_the_resting_level_drift_down_to_it()
    {
        Game g = Staffed();
        g.Build("coffee-corner", 2, 2);
        g.Me.Staff[0].Morale = 900;
        g.Weeks(10);
        Assert.Equal(890, g.Me.Staff[0].Morale);
        g.Weeks(400);
        Assert.Equal(512, g.Me.Staff[0].Morale);
    }

    [Fact]
    public void Pulling_the_room_down_lowers_the_level_at_once_and_morale_follows_it_down()
    {
        Game g = Staffed();
        int room = g.Build("kitchen", 3, 3);
        g.Weeks(40);
        Assert.All(g.Me.Staff, p => Assert.Equal(536, p.Morale));
        g.Do(Command.DemolishRoom(g.Id, room));
        Assert.Equal(500, Workplace.RestingMorale(g.C, g.Me));
        g.Weeks(1);
        Assert.All(g.Me.Staff, p => Assert.Equal(535, p.Morale));
        g.Weeks(40);
        Assert.All(g.Me.Staff, p => Assert.Equal(500, p.Morale));
    }

    [Fact]
    public void An_incident_drops_morale_and_the_rooms_bring_it_back_to_the_lifted_level()
    {
        Game g = Staffed();
        g.Build("kitchen", 3, 3);
        g.Weeks(40);
        StaffSystem.OnIncident(g.C, g.Me);
        Assert.All(g.Me.Staff, p => Assert.Equal(536 - g.C.StaffPeople.MoraleIncidentDrop, p.Morale));
        g.Weeks(g.C.StaffPeople.MoraleIncidentDrop);
        Assert.All(g.Me.Staff, p => Assert.Equal(536, p.Morale));
    }

    [Fact]
    public void Debt_still_wears_morale_down_below_a_lifted_level()
    {
        Game g = Staffed();
        g.Build("kitchen", 3, 3);
        g.Weeks(40);
        g.Me.WeeksInDebt = 1;
        StaffSystem.RunWeek(g.C, g.Me);
        Assert.All(g.Me.Staff, p => Assert.Equal(536 - g.C.StaffPeople.MoraleDebtDriftPerWeek, p.Morale));
        StaffSystem.RunWeek(g.C, g.Me);
        Assert.All(g.Me.Staff, p => Assert.Equal(536 - g.C.StaffPeople.MoraleDebtDriftPerWeek - 1, p.Morale));
    }

    [Fact]
    public void Morale_never_leaves_the_scale_even_at_the_biggest_lift()
    {
        Game g = Staffed("chat-boom", c => { c.StaffPeople.MaxMoraleLift = 500; Perk(c, "nap-pods-lift").PerTilePermille = 100; Perk(c, "nap-pods-lift").CapPermille = 900; });
        g.Build("nap-pods", 8, 8);
        Assert.Equal(1000, Workplace.RestingMorale(g.C, g.Me));
        g.Weeks(600);
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        Assert.All(g.Me.Staff, p => Assert.Equal(1000, p.Morale));
    }

    // ---- morale moves research

    [Fact]
    public void A_lab_at_the_baseline_does_the_research_it_always_did()
    {
        Game g = Staffed();
        Game flat = Staffed(tweak: c => { c.StaffPeople.MoraleResearchSlopePermille = 0; c.StaffPeople.MoraleResearchLossSlopePermille = 0; });
        Assert.Equal(Research(flat), Research(g));
    }

    [Fact]
    public void Happier_researchers_do_more_and_unhappier_ones_do_less()
    {
        Game g = Staffed(researchers: 1, tweak: c => { c.Research.PointsPerResearcherWeek = 100000; c.StaffPeople.MoraleResearchLossSlopePermille = 400; });
        long normal = Research(g);
        Assert.True(normal > 0);
        g.Me.Staff[0].Morale = 1000;
        long happy = Research(g);
        g.Me.Staff[0].Morale = 0;
        long sad = Research(g);
        Assert.InRange(happy, normal * 1200 / 1000 - 2, normal * 1200 / 1000 + 2);
        Assert.InRange(sad, normal * 800 / 1000 - 2, normal * 800 / 1000 + 2);
    }

    [Fact]
    public void Each_researcher_counts_by_their_own_morale()
    {
        Game g = Staffed(researchers: 2, tweak: c => { c.Research.PointsPerResearcherWeek = 100000; c.StaffPeople.MoraleResearchLossSlopePermille = 400; });
        long normal = Research(g);
        g.Me.Staff[0].Morale = 1000;
        g.Me.Staff[1].Morale = 0;
        long mixed = Research(g);
        Assert.InRange(mixed, normal - 2, normal + 2);
        g.Me.Staff[1].Morale = 500;
        long oneHappy = Research(g);
        Assert.InRange(oneHappy, normal * 1100 / 1000 - 2, normal * 1100 / 1000 + 2);
    }

    [Fact]
    public void In_the_shipped_data_an_unhappy_lab_does_the_research_it_always_did()
    {
        Game g = Staffed(researchers: 2, tweak: c => c.Research.PointsPerResearcherWeek = 100000);
        long normal = Research(g);
        g.Me.Staff[0].Morale = 0;
        g.Me.Staff[1].Morale = 100;
        Assert.Equal(normal, Research(g));
        Assert.Equal(50, Workplace.Report(g.C, g.Me).ResearchMorale);
        Assert.Equal(1000, Workplace.Report(g.C, g.Me).ResearchPermille);
    }

    [Fact]
    public void Only_researchers_count_for_the_research_factor()
    {
        Game g = Staffed(researchers: 1, tweak: c => { c.Research.PointsPerResearcherWeek = 100000; c.StaffPeople.MoraleResearchLossSlopePermille = 400; });
        g.Build("engineering-bay", 3, 3);
        g.Hire("engineer", 2);
        long normal = Research(g);
        foreach (StaffMember p in g.Me.Staff.Where(p => p.RoleId == "engineer")) p.Morale = 0;
        Assert.Equal(normal, Research(g));
        Assert.Equal(500, Workplace.Report(g.C, g.Me).ResearchMorale);
    }

    [Fact]
    public void A_star_keeps_the_star_bonus_on_top_of_morale()
    {
        Game g = Staffed(researchers: 1, tweak: c => c.Research.PointsPerResearcherWeek = 100000);
        long plain = Research(g);
        g.Me.Staff[0].TraitId = "star";
        long star = Research(g);
        Assert.InRange(star, plain * 1150 / 1000 - 2, plain * 1150 / 1000 + 2);
        g.Me.Staff[0].Morale = 1000;
        long happyStar = Research(g);
        Assert.InRange(happyStar, plain * 1150 / 1000 * 1200 / 1000 - 3, plain * 1150 / 1000 * 1200 / 1000 + 3);
    }

    [Fact]
    public void A_lab_with_no_researchers_does_no_research_and_does_not_break()
    {
        Game g = Lab();
        Assert.Equal(0, Research(g));
        WorkplaceReport r = Workplace.Report(g.C, g.Me);
        Assert.Equal(1000, r.ResearchPermille);
    }

    [Fact]
    public void The_kitchen_pays_back_in_research_once_morale_has_climbed()
    {
        Game plain = Staffed(researchers: 4);
        Game fed = Staffed(researchers: 4);
        fed.Build("kitchen", 5, 5);
        plain.Weeks(150);
        fed.Weeks(150);
        Assert.All(fed.Me.Staff, p => Assert.Equal(600, p.Morale));
        Assert.True(Research(fed) > Research(plain));
        Assert.Equal(1040, Workplace.Report(fed.C, fed.Me).ResearchPermille);
    }

    // ---- report

    [Fact]
    public void The_report_matches_the_pieces()
    {
        Game g = Staffed(researchers: 2);
        g.Build("kitchen", 4, 4);
        g.Me.Staff[0].Morale = 700;
        g.Me.Staff[1].Morale = 500;
        WorkplaceReport r = Workplace.Report(g.C, g.Me);
        Assert.Equal(64, r.MoraleLift);
        Assert.Equal(564, r.RestingMorale);
        Assert.Equal(128, r.PoachDefencePermille);
        Assert.Equal(Workplace.PoachChancePermille(g.C, g.Me), r.PoachChancePermille);
        Assert.Equal(600, r.ResearchMorale);
        Assert.Equal(1040, r.ResearchPermille);
    }

    // ---- offers

    static (Game game, LabState rival) Market(Action<GameConfig> tweak = null)
    {
        Game g = Staffed(rivals: true, tweak: c => { c.StaffPeople.PoachChancePermille = 1000; if (tweak != null) tweak(c); });
        g.Me.Staff[0].TraitId = "star";
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 100;
        return (g, rival);
    }

    static int Offers(Game g, LabState rival, int tries)
    {
        int made = 0;
        for (int i = 0; i < tries; i++)
        {
            g.Me.PoachOffers.Clear();
            StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
            made += g.Me.PoachOffers.Count;
        }
        return made;
    }

    [Fact]
    public void With_no_defence_a_sure_chance_makes_an_offer_every_time()
    {
        var (g, rival) = Market();
        Assert.Equal(100, Offers(g, rival, 100));
    }

    [Fact]
    public void Defence_turns_a_sure_offer_into_a_chancy_one()
    {
        var (g, rival) = Market(c =>
        {
            c.StaffPeople.MaxPoachDefencePermille = 900;
            Perk(c, "kitchen-stay").PerTilePermille = 100;
            Perk(c, "kitchen-stay").CapPermille = 900;
        });
        g.Build("kitchen", 3, 3);
        Assert.Equal(100, Workplace.PoachChancePermille(g.C, g.Me));
        int made = Offers(g, rival, 600);
        Assert.InRange(made, 20, 120);
    }

    [Fact]
    public void The_chance_and_the_offers_line_up_across_seeds()
    {
        int plain = 0;
        int defended = 0;
        foreach (long seed in new long[] { 3, 4, 5 })
        {
            var (a, ra) = Market(c => c.StaffPeople.PoachChancePermille = 500);
            var (b, rb) = Market(c => { c.StaffPeople.PoachChancePermille = 500; c.StaffPeople.MaxPoachDefencePermille = 900; Perk(c, "kitchen-stay").PerTilePermille = 100; Perk(c, "kitchen-stay").CapPermille = 900; });
            b.Build("kitchen", 3, 3);
            plain += Offers(a, ra, 300);
            defended += Offers(b, rb, 300);
        }
        Assert.True(defended < plain, "defended " + defended + " plain " + plain);
    }

    [Fact]
    public void A_bankrupt_lab_and_a_lab_with_no_staff_are_left_alone_and_nothing_is_rolled()
    {
        var (g, rival) = Market();
        g.Build("kitchen", 3, 3);
        string digest = g.Sim.Digest();
        g.Me.Bankrupt = true;
        string bankrupt = g.Sim.Digest();
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(bankrupt, g.Sim.Digest());
        g.Me.Bankrupt = false;
        g.Me.Staff.Clear();
        string empty = g.Sim.Digest();
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(empty, g.Sim.Digest());
        Assert.NotEqual(digest, bankrupt);
    }

    [Fact]
    public void A_rival_that_is_not_feuding_never_makes_an_offer_however_good_the_kitchen_is()
    {
        var (g, rival) = Market();
        rival.FeelingToPlayer = 0;
        Assert.Equal(0, Offers(g, rival, 50));
    }

    // ---- cross-system

    [Fact]
    public void A_happier_star_is_easier_to_keep_with_a_counter_offer()
    {
        Game g = Staffed();
        g.Build("kitchen", 5, 5);
        g.Weeks(150);
        StaffMember happy = g.Me.Staff[0];
        Assert.Equal(600, happy.Morale);
        CounterConfig mission = g.C.Talent.Counter("mission");
        int fed = TalentSystem.CounterChance(g.C, g.Me, happy, mission).Total;
        happy.Morale = 500;
        int plain = TalentSystem.CounterChance(g.C, g.Me, happy, mission).Total;
        Assert.True(fed > plain, fed + " vs " + plain);
        Assert.True(TalentSystem.CounterChance(g.C, g.Me, g.Me.Staff[0], mission).MoralePart == 0);
    }

    [Fact]
    public void A_lab_that_reaches_a_comfort_room_by_a_late_era_start_gets_the_same_numbers()
    {
        Game g = Lab("upward");
        g.Build("nap-pods", 3, 3);
        g.Build("kitchen", 3, 3);
        g.Build("coffee-corner", 2, 2);
        Assert.Equal(45 + 36 + 12, Workplace.MoraleLift(g.C, g.Me));
    }

    [Fact]
    public void A_comfort_room_has_to_wait_for_its_era()
    {
        Game g = Lab("dial-up-days");
        g.Refused(Command.BuildRoom(g.Id, "kitchen", 0, 0, 3, 3), CommandError.EraLocked);
        g.Refused(Command.BuildRoom(g.Id, "nap-pods", 0, 0, 3, 3), CommandError.EraLocked);
        g.Do(Command.BuildRoom(g.Id, "coffee-corner", 0, 0, 2, 2));
    }

    [Fact]
    public void A_comfort_room_must_be_big_enough_and_costs_upkeep()
    {
        Game g = Lab("dial-up-days");
        g.Refused(Command.BuildRoom(g.Id, "coffee-corner", 0, 0, 1, 2), CommandError.TooSmall);
        long cash = g.Me.Cash;
        g.Do(Command.BuildRoom(g.Id, "coffee-corner", 0, 0, 2, 2));
        Assert.Equal(cash - 4 * 250, g.Me.Cash);
    }

    [Fact]
    public void A_comfort_room_has_no_seats_so_hiring_into_it_is_refused()
    {
        Game g = Lab();
        g.Build("kitchen", 3, 3);
        g.Refused(Command.Hire(g.Id, "researcher", 1), CommandError.NoSeats);
    }

    [Fact]
    public void A_very_unhappy_lab_with_a_big_kitchen_still_recovers()
    {
        Game g = Staffed();
        g.Build("kitchen", 5, 5);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 0;
        g.Weeks(600);
        Assert.All(g.Me.Staff, p => Assert.Equal(600, p.Morale));
    }

    // ---- saves and determinism

    [Fact]
    public void A_game_with_comfort_rooms_and_lifted_morale_round_trips()
    {
        Game g = Staffed(researchers: 3);
        g.Build("kitchen", 4, 4);
        g.Build("coffee-corner", 2, 2);
        g.Weeks(30);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(20);
        again.StepMany(20);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_save_from_before_the_workplace_loads_and_plays_on_the_same()
    {
        Game g = Staffed(researchers: 3);
        g.Weeks(10);
        string old = SaveSchemaPretend.At(g.C, 26, g.Sim.Save());
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
        g.Weeks(30);
        loaded.StepMany(30);
        Assert.Equal(g.Me.ResearchPoints, loaded.State.Lab(g.Id).ResearchPoints);
        Assert.Equal(g.Me.Staff.Select(p => p.Morale).ToArray(), loaded.State.Lab(g.Id).Staff.Select(p => p.Morale).ToArray());
    }

    [Fact]
    public void Two_runs_with_the_same_seed_and_the_same_rooms_end_the_same()
    {
        string Run()
        {
            Game g = Game.New(5);
            g.Me.Cash = 1_000_000_000_000;
            g.Build("research-lab", 4, 4);
            g.Hire("researcher", 3);
            g.Build("coffee-corner", 2, 2);
            g.Years(3);
            return g.Sim.Digest();
        }
        Assert.Equal(Run(), Run());
    }

    [Fact]
    public void A_long_game_with_comfort_rooms_and_rivals_stays_sound()
    {
        Game g = Game.New(2);
        g.Me.Cash = 1_000_000_000_000;
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 3);
        g.Build("coffee-corner", 3, 3);
        for (int year = 0; year < 6; year++)
        {
            g.Years(1);
            Assert.Empty(SaveGame.Problems(g.C, g.State));
            foreach (StaffMember p in g.Me.Staff) Assert.InRange(p.Morale, 0, 1000);
        }
    }
}
