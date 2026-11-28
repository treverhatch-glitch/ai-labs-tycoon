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

/// <summary>Staff traits: the player's hires can arrive with one, each does one thing for the lab, and two camps start arguing.</summary>
public class TraitsTests
{
    static void NoRandomTraits(GameConfig c)
    {
        foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
        c.RolePerks.Clear();
    }

    static Game Lab(string era = "it-sees", Action<GameConfig> tweak = null, bool rivals = false)
    {
        var options = new NewGameOptions { Seed = 3, StartEraId = era };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c => { NoRandomTraits(c); if (tweak != null) tweak(c); }), options));
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    /// <summary>Puts people on the payroll without seating them, for tests of the sums.</summary>
    static List<StaffMember> Add(Game g, string trait, int count, string role = "researcher", LabState lab = null)
    {
        lab = lab ?? g.Me;
        var added = new List<StaffMember>();
        for (int i = 0; i < count; i++)
        {
            var p = new StaffMember { Id = g.State.TakeId(), RoleId = role, TraitId = trait, Morale = g.C.StaffPeople.MoraleBaseline };
            lab.Staff.Add(p);
            added.Add(p);
        }
        return added;
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

    static StaffTraitConfig Trait(GameConfig c, string id) => c.StaffPeople.Trait(id);

    static long Research(Game g)
    {
        long before = g.Me.ResearchPoints;
        TrainingSystem.RunResearch(g.C, g.Me, g.Tick);
        return g.Me.ResearchPoints - before;
    }

    // ---- the table

    [Fact]
    public void There_are_twelve_traits_and_the_plain_one_does_nothing()
    {
        StaffPeopleConfig s = TestData.Config().StaffPeople;
        Assert.Equal(new[] { "steady", "star", "tenx", "doomer", "accelerationist", "mentor", "reads-the-logs", "interviewing", "sleeps-under-desk", "posts-through-it", "quits-by-blog-post", "only-interesting" }, s.Traits.Select(t => t.Id).ToArray());
        StaffTraitConfig steady = s.Trait("steady");
        Assert.Equal(0, steady.ResearchPermille + steady.HirePermille + steady.TrainingSpeedPermille + steady.AlignmentWorkPermille
            + steady.IncidentCatchPermille + steady.MentorPermille + steady.OfferPermille + steady.RestMoraleOffset);
        Assert.Equal("", steady.Camp);
    }

    [Fact]
    public void Only_the_star_is_a_star_and_it_keeps_the_numbers_it_always_had()
    {
        StaffPeopleConfig s = TestData.Config().StaffPeople;
        Assert.Equal(new[] { "star" }, s.Traits.Where(t => t.Star).Select(t => t.Id).ToArray());
        Assert.Equal(150, s.Trait("star").ResearchPermille);
        Assert.Equal(0, s.Trait("star").HirePermille);
        Assert.Equal(120, s.StarHirePermille);
    }

    [Fact]
    public void The_two_camps_are_the_doomer_and_the_accelerationist_and_they_pull_opposite_ways()
    {
        GameConfig c = TestData.Config();
        StaffTraitConfig doom = Trait(c, "doomer");
        StaffTraitConfig accel = Trait(c, "accelerationist");
        Assert.Equal("doom", doom.Camp);
        Assert.Equal("accel", accel.Camp);
        Assert.True(doom.AlignmentWorkPermille > 0 && doom.TrainingSpeedPermille < 0);
        Assert.True(accel.AlignmentWorkPermille < 0 && accel.TrainingSpeedPermille > 0);
        Assert.Equal(new[] { "accelerationist", "doomer" }, c.StaffPeople.Traits.Where(t => t.Camp != "").Select(t => t.Id).OrderBy(x => x).ToArray());
    }

    [Fact]
    public void Every_trait_has_a_name_and_a_blurb_and_the_hire_shares_leave_room_for_plain_hires()
    {
        StaffPeopleConfig s = TestData.Config().StaffPeople;
        foreach (StaffTraitConfig t in s.Traits)
        {
            Assert.False(string.IsNullOrWhiteSpace(t.Name), t.Id);
            Assert.False(string.IsNullOrWhiteSpace(t.Blurb), t.Id);
        }
        Assert.Equal(443, s.Traits.Sum(t => t.HirePermille));
    }

    [Fact]
    public void The_validator_keeps_each_trait_number_in_range()
    {
        Assert.Empty(Problems(c => { }));
        AssertProblem(Problems(c => Trait(c, "tenx").ResearchPermille = -901), "researchPermille");
        AssertProblem(Problems(c => Trait(c, "tenx").HirePermille = -1), "hirePermille");
        AssertProblem(Problems(c => Trait(c, "doomer").TrainingSpeedPermille = 501), "trainingSpeedPermille");
        AssertProblem(Problems(c => Trait(c, "doomer").AlignmentWorkPermille = -501), "alignmentWorkPermille");
        AssertProblem(Problems(c => Trait(c, "reads-the-logs").IncidentCatchPermille = 501), "incidentCatchPermille");
        AssertProblem(Problems(c => Trait(c, "mentor").MentorPermille = -1), "mentorPermille");
        AssertProblem(Problems(c => Trait(c, "interviewing").OfferPermille = 501), "offerPermille");
        AssertProblem(Problems(c => Trait(c, "sleeps-under-desk").RestMoraleOffset = -501), "restMoraleOffset");
        AssertProblem(Problems(c => Trait(c, "doomer").Camp = "moon"), "camp");
        AssertProblem(Problems(c => Trait(c, "mentor").Blurb = ""), "blurb");
        AssertProblem(Problems(c => Trait(c, "mentor").Name = ""), "name");
    }

    [Fact]
    public void The_validator_keeps_the_star_out_of_the_hire_roll_and_the_roll_under_a_whole()
    {
        AssertProblem(Problems(c => Trait(c, "star").HirePermille = 10), "star trait");
        AssertProblem(Problems(c => Trait(c, "steady").HirePermille = 600), "hirePermille adds up");
        Assert.DoesNotContain(Problems(c => Trait(c, "steady").HirePermille = 560), p => p.Contains("adds up"));
    }

    [Fact]
    public void The_validator_keeps_the_caps_sane()
    {
        AssertProblem(Problems(c => c.StaffPeople.MaxTraitSwingPermille = 901), "maxTraitSwingPermille");
        AssertProblem(Problems(c => c.StaffPeople.MaxTraitSwingPermille = -1), "maxTraitSwingPermille");
        AssertProblem(Problems(c => c.StaffPeople.MaxMentorPermille = 1001), "maxMentorPermille");
        AssertProblem(Problems(c => c.StaffPeople.MaxOfferBoostPermille = 2001), "maxOfferBoostPermille");
        AssertProblem(Problems(c => c.StaffPeople.FrictionPerPairPermille = 501), "frictionPerPairPermille");
        AssertProblem(Problems(c => c.StaffPeople.MaxFrictionPermille = 901), "maxFrictionPermille");
        Assert.DoesNotContain(Problems(c => c.StaffPeople.MaxFrictionPermille = 900), p => p.Contains("maxFrictionPermille"));
    }

    // ---- the sums

    [Fact]
    public void A_lab_with_only_plain_people_has_nothing_to_report()
    {
        Game g = Lab();
        Add(g, "steady", 3);
        Add(g, "star", 1);
        TraitReport r = Traits.Report(g.C, g.Me);
        Assert.Equal(0, r.TrainingSpeedPermille + r.AlignmentWorkPermille + r.IncidentCatchPermille + r.MentorPermille + r.OfferBoostPermille + r.FrictionPermille);
        Assert.Equal(1000, r.ResearchPermille);
    }

    [Fact]
    public void Two_doomers_slow_training_and_add_alignment_work()
    {
        Game g = Lab();
        Add(g, "doomer", 2);
        TraitReport r = Traits.Report(g.C, g.Me);
        Assert.Equal(-60, r.TrainingSpeedPermille);
        Assert.Equal(80, r.AlignmentWorkPermille);
        Assert.Equal(2, r.Doomers);
        Assert.Equal(0, r.Accelerationists);
    }

    [Fact]
    public void Accelerationists_speed_training_and_give_up_alignment_work()
    {
        Game g = Lab();
        Add(g, "accelerationist", 3);
        TraitReport r = Traits.Report(g.C, g.Me);
        Assert.Equal(120, r.TrainingSpeedPermille);
        Assert.Equal(-90, r.AlignmentWorkPermille);
    }

    [Fact]
    public void The_swing_stops_at_the_cap_in_both_directions()
    {
        Game g = Lab();
        Add(g, "accelerationist", 20);
        TraitReport r = Traits.Report(g.C, g.Me);
        Assert.Equal(300, r.TrainingSpeedPermille);
        Assert.Equal(-300, r.AlignmentWorkPermille);
        Game d = Lab();
        Add(d, "doomer", 20);
        Assert.Equal(300, Traits.Report(d.C, d.Me).AlignmentWorkPermille);
        Assert.Equal(-300, Traits.Report(d.C, d.Me).TrainingSpeedPermille);
    }

    [Fact]
    public void Mentors_add_up_to_a_cap()
    {
        Game g = Lab();
        Add(g, "mentor", 4);
        Assert.Equal(80, Traits.Report(g.C, g.Me).MentorPermille);
        Add(g, "mentor", 20);
        Assert.Equal(200, Traits.Report(g.C, g.Me).MentorPermille);
    }

    [Fact]
    public void Secret_interviews_add_to_the_chance_of_an_offer_up_to_a_cap()
    {
        Game g = Lab();
        Add(g, "interviewing", 3);
        Assert.Equal(300, Traits.Report(g.C, g.Me).OfferBoostPermille);
        Add(g, "interviewing", 10);
        Assert.Equal(500, Traits.Report(g.C, g.Me).OfferBoostPermille);
    }

    [Fact]
    public void The_offer_boost_is_never_negative_even_with_a_trait_that_cuts_it()
    {
        Game g = Lab(tweak: c => Trait(c, "mentor").OfferPermille = -200);
        Add(g, "mentor", 3);
        Assert.Equal(0, Traits.Report(g.C, g.Me).OfferBoostPermille);
    }

    [Fact]
    public void Friction_is_a_share_for_each_pair_of_camps_and_needs_both()
    {
        Game g = Lab();
        Add(g, "doomer", 3);
        Assert.Equal(0, Traits.Report(g.C, g.Me).FrictionPermille);
        Add(g, "accelerationist", 1);
        Assert.Equal(40, Traits.Report(g.C, g.Me).FrictionPermille);
        Add(g, "accelerationist", 4);
        Assert.Equal(120, Traits.Report(g.C, g.Me).FrictionPermille);
    }

    [Fact]
    public void Friction_stops_at_its_cap_and_a_lab_always_does_some_research()
    {
        Game g = Lab();
        Add(g, "doomer", 10);
        Add(g, "accelerationist", 10);
        TraitReport r = Traits.Report(g.C, g.Me);
        Assert.Equal(300, r.FrictionPermille);
        Assert.Equal(700, r.ResearchPermille);
    }

    [Fact]
    public void Friction_can_be_switched_off_in_the_data()
    {
        Game g = Lab(tweak: c => c.StaffPeople.FrictionPerPairPermille = 0);
        Add(g, "doomer", 5);
        Add(g, "accelerationist", 5);
        Assert.Equal(0, Traits.Report(g.C, g.Me).FrictionPermille);
    }

    // ---- the hooks

    [Fact]
    public void Training_speed_and_alignment_work_follow_the_traits()
    {
        Game plain = Lab();
        Game fast = Lab();
        Add(fast, "accelerationist", 2);
        Assert.Equal(TrainingSystem.SpeedPermille(plain.C, plain.Me) + 80, TrainingSystem.SpeedPermille(fast.C, fast.Me));
        Assert.Equal(TrainingSystem.AlignmentWorkPermille(plain.C, plain.Me, plain.State.Tick) - 60, TrainingSystem.AlignmentWorkPermille(fast.C, fast.Me, fast.State.Tick));
        Game careful = Lab();
        Add(careful, "doomer", 2);
        Assert.Equal(TrainingSystem.SpeedPermille(plain.C, plain.Me) - 60, TrainingSystem.SpeedPermille(careful.C, careful.Me));
        Assert.Equal(TrainingSystem.AlignmentWorkPermille(plain.C, plain.Me, plain.State.Tick) + 80, TrainingSystem.AlignmentWorkPermille(careful.C, careful.Me, careful.State.Tick));
    }

    [Fact]
    public void A_lab_of_accelerationists_finishes_a_run_sooner_than_a_lab_of_doomers()
    {
        Game fast = Lab("agents");
        fast.WithStarterLab();
        Game slow = Lab("agents");
        slow.WithStarterLab();
        foreach (StaffMember p in fast.Me.Staff) p.TraitId = "accelerationist";
        foreach (StaffMember p in slow.Me.Staff) p.TraitId = "doomer";
        int quick = fast.UntilTrained(fast.Train("mini", 200));
        int late = slow.UntilTrained(slow.Train("mini", 200));
        Assert.True(quick < late, quick + " weeks against " + late);
    }

    [Fact]
    public void People_who_read_the_logs_add_to_the_share_of_incidents_caught()
    {
        Game g = Lab();
        Assert.Equal(0, RolePerks.IncidentCatchPermille(g.C, g.Me));
        Add(g, "reads-the-logs", 3);
        Assert.Equal(120, RolePerks.IncidentCatchPermille(g.C, g.Me));
        Add(g, "reads-the-logs", 30);
        Assert.Equal(900, RolePerks.IncidentCatchPermille(g.C, g.Me));
    }

    [Fact]
    public void A_trait_that_loses_catches_cannot_push_the_share_under_zero_and_the_odds_breakdown_agrees()
    {
        Game g = Lab(tweak: c => Trait(c, "accelerationist").IncidentCatchPermille = -100);
        Add(g, "accelerationist", 3);
        Assert.Equal(0, RolePerks.IncidentCatchPermille(g.C, g.Me));
        Add(g, "reads-the-logs", 5);
        Assert.Equal(0, RolePerks.IncidentCatchPermille(g.C, g.Me));
        Add(g, "reads-the-logs", 3);
        Assert.Equal(20, RolePerks.IncidentCatchPermille(g.C, g.Me));
        var model = new ModelState { Alignment = 300 };
        RolePerks.IncidentOddsBreakdown b = RolePerks.IncidentOdds(g.C, g.Me, g.Tick, model);
        Assert.Equal(SafetySystem.IncidentChancePermille(g.C, g.Tick, model), b.BeforeRedTeam);
        Assert.Equal(-(int)MathI.MulPermille(b.BeforeRedTeam, 20), b.CaughtByRedTeam);
    }

    [Fact]
    public void Readers_of_logs_cut_real_incidents()
    {
        int without = Incidents(0);
        int with = Incidents(10);
        Assert.True(with < without, with + " with readers, " + without + " without");
    }

    static int Incidents(int readers)
    {
        Game g = Lab("chat-boom", c => c.Safety.IncidentChancePermille = 1000);
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        g.Do(Command.ReleaseModel(g.Id, model.Id, "consumer-assistant"));
        model.Alignment = 300;
        model.DisplayedAlignment = 300;
        g.Me.Cash = 1_000_000_000_000_000;
        Add(g, "reads-the-logs", readers, "safety");
        g.Weeks(60);
        return g.State.Events.Count(e => e.Kind == EventKind.Incident);
    }

    static Game Researchers(int count, Action<GameConfig> tweak = null)
    {
        Game g = Lab(tweak: c => { c.Research.PointsPerResearcherWeek = 100000; if (tweak != null) tweak(c); });
        g.Build("research-lab", 8, 8);
        g.Hire("researcher", count);
        foreach (StaffMember p in g.Me.Staff) p.TraitId = "steady";
        return g;
    }

    [Fact]
    public void A_10X_does_a_tenth_more_research_and_one_who_sleeps_under_the_desk_a_tenth_less()
    {
        Game g = Researchers(1);
        long normal = Research(g);
        g.Me.Staff[0].TraitId = "tenx";
        Assert.InRange(Research(g), normal * 1100 / 1000 - 2, normal * 1100 / 1000 + 2);
        g.Me.Staff[0].TraitId = "sleeps-under-desk";
        Assert.InRange(Research(g), normal * 900 / 1000 - 2, normal * 900 / 1000 + 2);
    }

    [Fact]
    public void Mentors_lift_every_researchers_output()
    {
        Game g = Researchers(4);
        long normal = Research(g);
        g.Me.Staff[0].TraitId = "mentor";
        Assert.InRange(Research(g), normal * 1020 / 1000 - 2, normal * 1020 / 1000 + 2);
        g.Me.Staff[1].TraitId = "mentor";
        Assert.InRange(Research(g), normal * 1040 / 1000 - 3, normal * 1040 / 1000 + 3);
    }

    [Fact]
    public void Arguing_costs_research_and_a_mentor_pays_part_of_it_back()
    {
        Game g = Researchers(4);
        long normal = Research(g);
        g.Me.Staff[0].TraitId = "doomer";
        g.Me.Staff[1].TraitId = "accelerationist";
        Assert.InRange(Research(g), normal * 960 / 1000 - 3, normal * 960 / 1000 + 3);
        g.Me.Staff[2].TraitId = "mentor";
        Assert.InRange(Research(g), normal * 980 / 1000 - 3, normal * 980 / 1000 + 3);
    }

    [Fact]
    public void A_lab_with_no_researchers_does_no_research_whatever_the_traits()
    {
        Game g = Lab();
        Add(g, "mentor", 5, "engineer");
        Assert.Equal(0, Research(g));
    }

    [Fact]
    public void Traits_stack_with_the_star_bonus_and_morale()
    {
        Game g = Researchers(1);
        long normal = Research(g);
        g.Me.Staff[0].TraitId = "star";
        long star = Research(g);
        Assert.InRange(star, normal * 1150 / 1000 - 2, normal * 1150 / 1000 + 2);
        g.Me.Staff[0].Morale = 1000;
        Assert.InRange(Research(g), star * 1200 / 1000 - 3, star * 1200 / 1000 + 3);
    }

    // ---- offers

    [Fact]
    public void Secret_interviews_raise_the_chance_of_an_offer_by_a_share_of_the_usual_chance()
    {
        Game g = Lab();
        Add(g, "interviewing", 2);
        Assert.Equal(80 * 1200 / 1000, Workplace.PoachChancePermille(g.C, g.Me));
    }

    [Fact]
    public void Interviews_and_a_kitchen_work_against_each_other()
    {
        Game g = Lab();
        g.Build("kitchen", 3, 3);
        Add(g, "interviewing", 2);
        Assert.Equal(80 * (1000 - 72 + 200) / 1000, Workplace.PoachChancePermille(g.C, g.Me));
    }

    static (Game game, LabState rival) Market(Action<GameConfig> tweak = null)
    {
        Game g = Lab(rivals: true, tweak: c => { c.StaffPeople.PoachChancePermille = 1000; if (tweak != null) tweak(c); });
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 100;
        return (g, rival);
    }

    [Fact]
    public void A_rival_goes_for_the_one_who_is_secretly_interviewing_when_there_is_no_star()
    {
        var (g, rival) = Market();
        Add(g, "steady", 3);
        StaffMember leaker = Add(g, "interviewing", 1)[0];
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Single(g.Me.PoachOffers);
        Assert.Equal(leaker.Id, g.Me.PoachOffers[0].StaffId);
    }

    [Fact]
    public void With_neither_a_star_nor_an_interview_nobody_is_courted()
    {
        var (g, rival) = Market();
        Add(g, "steady", 3);
        Add(g, "mentor", 2);
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Empty(g.Me.PoachOffers);
    }

    [Fact]
    public void Where_there_is_a_star_the_star_is_still_picked_first_when_morale_ties()
    {
        var (g, rival) = Market();
        StaffMember star = Add(g, "star", 1)[0];
        Add(g, "interviewing", 1);
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(star.Id, g.Me.PoachOffers[0].StaffId);
    }

    // ---- morale

    [Fact]
    public void Someone_who_sleeps_under_the_desk_settles_a_notch_happier_than_the_rest()
    {
        Game g = Lab();
        StaffMember sleeper = Add(g, "sleeps-under-desk", 1)[0];
        StaffMember plain = Add(g, "steady", 1)[0];
        Assert.Equal(560, Traits.RestingMorale(g.C, g.Me, sleeper));
        Assert.Equal(500, Traits.RestingMorale(g.C, g.Me, plain));
        g.Weeks(70);
        Assert.Equal(560, sleeper.Morale);
        Assert.Equal(500, plain.Morale);
    }

    [Fact]
    public void The_trait_offset_adds_to_the_comfort_rooms_and_stays_on_the_scale()
    {
        Game g = Lab(tweak: c => { Trait(c, "sleeps-under-desk").RestMoraleOffset = 500; });
        g.Build("kitchen", 5, 5);
        StaffMember sleeper = Add(g, "sleeps-under-desk", 1)[0];
        Assert.Equal(1000, Traits.RestingMorale(g.C, g.Me, sleeper));
        Game low = Lab(tweak: c => { Trait(c, "sleeps-under-desk").RestMoraleOffset = -500; });
        StaffMember glum = Add(low, "sleeps-under-desk", 1)[0];
        Assert.Equal(0, Traits.RestingMorale(low.C, low.Me, glum));
        low.Weeks(5);
        Assert.Equal(495, glum.Morale);
    }

    [Fact]
    public void A_hire_who_sleeps_under_the_desk_starts_at_their_own_resting_level()
    {
        Game g = Lab(tweak: c => { Trait(c, "sleeps-under-desk").HirePermille = 1000; Trait(c, "star").HirePermille = 0; c.StaffPeople.StarHirePermille = 0; });
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 2);
        Assert.All(g.Me.Staff, p => Assert.Equal("sleeps-under-desk", p.TraitId));
        Assert.All(g.Me.Staff, p => Assert.Equal(560, p.Morale));
    }

    // ---- hiring

    [Fact]
    public void With_the_hire_shares_at_zero_every_hire_is_plain_as_it_always_was()
    {
        Game g = Lab(tweak: c => c.StaffPeople.StarHirePermille = 0);
        g.Build("research-lab", 8, 8);
        g.Hire("researcher", 16);
        Assert.All(g.Me.Staff, p => Assert.Equal("steady", p.TraitId));
    }

    [Fact]
    public void The_star_roll_still_decides_the_star_and_comes_first()
    {
        Game g = Lab(tweak: c => { c.StaffPeople.StarHirePermille = 1000; Trait(c, "doomer").HirePermille = 600; });
        g.Build("research-lab", 6, 6);
        g.Hire("researcher", 9);
        Assert.All(g.Me.Staff, p => Assert.Equal("star", p.TraitId));
    }

    [Fact]
    public void A_trait_comes_from_the_hires_id_so_the_same_id_always_gets_the_same_trait()
    {
        Game g = Lab();
        for (int id = 1; id < 400; id += 7)
            Assert.Equal(Traits.RollHireTrait(g.C, g.Me, id, "steady"), Traits.RollHireTrait(g.C, g.Me, id, "steady"));
    }

    [Fact]
    public void Over_many_ids_each_trait_turns_up_about_as_often_as_its_share()
    {
        GameConfig c = TestData.Config();
        Game g = Lab();
        var counts = new Dictionary<string, int>();
        const int n = 20000;
        for (int id = 1; id <= n; id++)
        {
            string t = Traits.RollHireTrait(c, g.Me, id, "steady");
            counts[t] = counts.ContainsKey(t) ? counts[t] + 1 : 1;
        }
        foreach (StaffTraitConfig t in c.StaffPeople.Traits.Where(t => t.HirePermille > 0))
            Assert.InRange(counts[t.Id], n * t.HirePermille / 1000 * 75 / 100, n * t.HirePermille / 1000 * 125 / 100);
        Assert.InRange(counts["steady"], n * 560 / 1000 * 95 / 100, n * 560 / 1000 * 105 / 100);
        Assert.False(counts.ContainsKey("star"));
    }

    [Fact]
    public void A_plan_run_lab_never_gets_the_extra_traits()
    {
        Game g = Lab(rivals: true);
        foreach (StaffTraitConfig t in g.C.StaffPeople.Traits.Where(t => !t.Star && t.Id != "steady")) t.HirePermille = 100;
        LabState rival = g.Rival("openaye");
        for (int id = 1; id < 3000; id += 3)
            Assert.Equal("steady", Traits.RollHireTrait(g.C, rival, id, "steady"));
        Game player = Lab();
        foreach (StaffTraitConfig t in player.C.StaffPeople.Traits.Where(t => !t.Star && t.Id != "steady")) t.HirePermille = 100;
        Assert.Contains(Enumerable.Range(1, 200), id => Traits.RollHireTrait(player.C, player.Me, id, "steady") != "steady");
    }

    [Fact]
    public void Rival_hires_in_a_real_run_are_only_steady_or_star()
    {
        Game g = Game.New(5);
        g.Years(6);
        foreach (LabState lab in g.State.Labs.Where(l => l.Controller == LabController.Plan))
            Assert.All(lab.Staff, p => Assert.Contains(p.TraitId, new[] { "steady", "star" }));
    }

    [Fact]
    public void Rolling_a_trait_does_not_move_the_world_dice()
    {
        long Next(Action<GameConfig> tweak)
        {
            Game g = Lab(tweak: tweak);
            g.Build("research-lab", 6, 6);
            g.Hire("researcher", 9);
            return g.State.Rng.World.Next(1000000);
        }
        Assert.Equal(Next(c => { }), Next(c => { Trait(c, "doomer").HirePermille = 500; Trait(c, "mentor").HirePermille = 400; }));
    }

    // ---- cross-system

    [Fact]
    public void A_hired_crowd_shows_up_in_the_report_and_the_research_it_does()
    {
        Game g = Lab(tweak: c => { c.Research.PointsPerResearcherWeek = 100000; Trait(c, "doomer").HirePermille = 500; Trait(c, "accelerationist").HirePermille = 500; c.StaffPeople.StarHirePermille = 0; });
        g.Build("research-lab", 8, 8);
        g.Hire("researcher", 16);
        TraitReport r = Traits.Report(g.C, g.Me);
        Assert.Equal(16, r.Doomers + r.Accelerationists);
        int pairs = Math.Min(r.Doomers, r.Accelerationists);
        Assert.Equal(Math.Min(pairs * 40, 300), r.FrictionPermille);
        Assert.True(r.Doomers > 0 && r.Accelerationists > 0, "the id roll gave " + r.Doomers + " and " + r.Accelerationists);
    }

    [Fact]
    public void Letting_a_trait_holder_go_takes_their_effect_with_them()
    {
        Game g = Lab();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 3);
        foreach (StaffMember p in g.Me.Staff) p.TraitId = "doomer";
        Assert.Equal(120, Traits.Report(g.C, g.Me).AlignmentWorkPermille);
        g.Do(Command.Fire(g.Id, "researcher", 1));
        Assert.Equal(80, Traits.Report(g.C, g.Me).AlignmentWorkPermille);
    }

    // ---- saves and determinism

    [Fact]
    public void A_game_full_of_traits_round_trips_and_plays_on_the_same()
    {
        Game g = Lab(tweak: c => { Trait(c, "doomer").HirePermille = 300; Trait(c, "accelerationist").HirePermille = 300; Trait(c, "mentor").HirePermille = 300; });
        g.Build("research-lab", 6, 6);
        g.Hire("researcher", 9);
        g.Weeks(30);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(20);
        again.StepMany(20);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_save_from_before_the_new_traits_loads_because_steady_and_star_are_still_there()
    {
        Game g = Lab();
        g.Build("research-lab", 4, 4);
        g.Hire("researcher", 3);
        g.Me.Staff[0].TraitId = "star";
        string old = SaveSchemaPretend.At(g.C, 26, g.Sim.Save());
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
        Assert.Equal("star", loaded.State.Lab(g.Id).Staff[0].TraitId);
    }

    [Fact]
    public void A_save_naming_a_trait_the_data_does_not_have_is_a_problem()
    {
        Game g = Lab();
        Add(g, "mystery", 1);
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("unknown trait"));
    }

    [Fact]
    public void Two_runs_with_the_same_seed_end_the_same_with_traits_in_play()
    {
        string Run()
        {
            Game g = Game.New(7);
            g.Me.Cash = 1_000_000_000_000;
            g.Build("research-lab", 6, 6);
            g.Hire("researcher", 8);
            g.Years(3);
            return g.Sim.Digest();
        }
        Assert.Equal(Run(), Run());
    }

    [Fact]
    public void A_long_game_with_traits_in_play_stays_sound()
    {
        Game g = Game.New(2);
        g.Me.Cash = 1_000_000_000_000;
        g.Build("research-lab", 6, 6);
        g.Hire("researcher", 9);
        for (int year = 0; year < 6; year++)
        {
            g.Years(1);
            Assert.Empty(SaveGame.Problems(g.C, g.State));
            foreach (StaffMember p in g.Me.Staff) Assert.InRange(p.Morale, 0, 1000);
        }
    }
}
