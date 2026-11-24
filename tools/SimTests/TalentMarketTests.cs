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

/// <summary>The talent market: a rival's offer waits for an answer, the answer is a counter, and the player can poach back.</summary>
public class TalentMarketTests
{
    /// <summary>A game with rivals, a research lab with three researchers (the first a star), and a rival that dislikes the player.</summary>
    static (Game game, LabState rival) Market(string era = "", Action<GameConfig> tweak = null, int researchers = 3, long seed = 7)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = era };
        var g = new Game(Simulation.NewGame(TestData.Config(c => { c.StaffPeople.PoachChancePermille = 1000; foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0; if (tweak != null) tweak(c); }), options));
        g.Me.Cash = 1_000_000_000;
        g.Build("research-lab", 5, 4);
        g.Hire("researcher", researchers);
        g.Me.Staff[0].TraitId = "star";
        LabState rival = g.Rival("openaye");
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow - 100;
        return (g, rival);
    }

    static PoachOffer MakeOffer(Game g, LabState rival)
    {
        int before = g.Me.PoachOffers.Count;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(before + 1, g.Me.PoachOffers.Count);
        return g.Me.PoachOffers.Last();
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

    static int Events(Game g, EventKind kind) => g.State.Events.Count(e => e.Kind == kind);

    // ---- the table

    [Fact]
    public void There_are_four_counters_and_the_mission_is_the_cheapest_and_the_easiest_to_lose()
    {
        TalentConfig t = TestData.Config().Talent;
        Assert.Equal(new[] { "money", "compute", "title", "mission" }, t.Counters.Select(k => k.Id).ToArray());
        Assert.Equal(1000, t.Counter("money").BaseSuccessPermille);
        Assert.Equal(0, t.Counter("mission").CostPermille);
        Assert.True(t.Counters.All(k => k.CostPermille >= t.Counter("mission").CostPermille));
        Assert.True(t.Counter("mission").MoraleSlopePermille > t.Counter("money").MoraleSlopePermille);
        Assert.Null(t.Counter("bribe"));
    }

    [Fact]
    public void A_talent_market_that_could_not_work_is_a_config_problem()
    {
        AssertProblem(Problems(c => c.Talent.OfferWeeks = -1), "offerWeeks");
        AssertProblem(Problems(c => c.Talent.MaxOpenOffers = 0), "maxOpenOffers");
        AssertProblem(Problems(c => c.Talent.PackageMultiple = 0), "packageMultiple");
        AssertProblem(Problems(c => c.Talent.StarPackagePermille = 900), "starPackagePermille");
        AssertProblem(Problems(c => c.Talent.MinSuccessPermille = 0), "minSuccessPermille");
        AssertProblem(Problems(c => c.Talent.PoachBackFeePermille = -1), "poachBackFeePermille");
        AssertProblem(Problems(c => c.Talent.PoachBackBasePermille = 1500), "poachBackBasePermille");
        AssertProblem(Problems(c => c.Talent.PoachBackFailFeelingHit = -1), "FeelingHit");
        AssertProblem(Problems(c => c.Talent.Counters.Clear()), "counters is empty");
        AssertProblem(Problems(c => c.Talent.Counters.Add(c.Talent.Counters[0])), "talent.counters");
        AssertProblem(Problems(c => c.Talent.Counters[0].Name = ""), "no name");
        AssertProblem(Problems(c => c.Talent.Counters[0].Blurb = ""), "no blurb");
        AssertProblem(Problems(c => c.Talent.Counters[1].CostPermille = 9000), "costPermille");
        AssertProblem(Problems(c => c.Talent.Counters[1].BaseSuccessPermille = 1200), "baseSuccessPermille");
        AssertProblem(Problems(c => c.Talent.Counters[1].MoraleGain = 900), "moraleGain");
    }

    [Fact]
    public void The_talent_config_is_strict_json_and_round_trips()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(c.ToJson(), GameConfig.FromJson(c.ToJson()).ToJson());
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(TestData.ConfigJson().Replace("\"offerWeeks\"", "\"offerWeek\"")));
        Assert.ThrowsAny<Exception>(() => GameConfig.FromJson(TestData.ConfigJson().Replace("\"baseSuccessPermille\"", "\"baseSuccess\"")));
    }

    // ---- offers

    [Fact]
    public void A_feuding_rival_makes_an_offer_instead_of_taking_the_star_at_once()
    {
        var (g, rival) = Market();
        StaffMember star = g.Me.Staff[0];
        int staff = g.Me.Staff.Count;
        PoachOffer offer = MakeOffer(g, rival);
        Assert.Equal(star.Id, offer.StaffId);
        Assert.Equal(rival.Id, offer.RivalLabId);
        Assert.Equal(g.Tick + g.C.Talent.OfferWeeks, offer.ExpiresTick);
        Assert.Equal(staff, g.Me.Staff.Count);
        Assert.Equal(1, Events(g, EventKind.PoachOffered));
        Assert.Equal(0, Events(g, EventKind.StaffPoached));
        Assert.Equal(offer.Package, g.State.Events.Last(e => e.Kind == EventKind.PoachOffered).Amount);
    }

    [Fact]
    public void An_offer_nobody_answers_is_taken_when_its_time_is_up()
    {
        var (g, rival) = Market();
        PoachOffer offer = MakeOffer(g, rival);
        int moraleBefore = g.Me.Staff[1].Morale;
        g.Weeks(g.C.Talent.OfferWeeks);
        Assert.Contains(g.Me.Staff, p => p.TraitId == "star");
        Assert.Single(g.Me.PoachOffers);
        g.Weeks(1);
        Assert.DoesNotContain(g.Me.Staff, p => p.TraitId == "star");
        Assert.Empty(g.Me.PoachOffers);
        GameEvent lost = g.State.Events.Last(e => e.Kind == EventKind.StaffPoached);
        Assert.Equal(rival.Id, lost.LabId);
        Assert.Equal(g.Id, lost.SubjectId);
        Assert.True(g.Me.Staff[0].Morale <= moraleBefore);
    }

    [Fact]
    public void With_no_waiting_time_the_star_goes_at_once_as_it_always_did()
    {
        var (g, rival) = Market(tweak: c => c.Talent.OfferWeeks = 0);
        int staff = g.Me.Staff.Count;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(staff - 1, g.Me.Staff.Count);
        Assert.Empty(g.Me.PoachOffers);
        Assert.Equal(1, Events(g, EventKind.StaffPoached));
        Assert.Equal(0, Events(g, EventKind.PoachOffered));
    }

    [Fact]
    public void Only_so_many_offers_wait_at_once_and_nobody_gets_two()
    {
        var (g, rival) = Market(researchers: 4, tweak: c => c.Talent.MaxOpenOffers = 2);
        g.Me.Staff[1].TraitId = "star";
        g.Me.Staff[2].TraitId = "star";
        MakeOffer(g, rival);
        MakeOffer(g, rival);
        Assert.Equal(2, g.Me.PoachOffers.Count);
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Equal(2, g.Me.PoachOffers.Count);
        Assert.Equal(2, g.Me.PoachOffers.Select(o => o.StaffId).Distinct().Count());
    }

    [Fact]
    public void A_rival_with_no_one_left_to_tempt_makes_no_offer()
    {
        var (g, rival) = Market();
        g.Me.Staff[0].TraitId = "steady";
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Empty(g.Me.PoachOffers);
    }

    [Fact]
    public void A_package_is_the_eras_hiring_fee_times_the_multiple_and_double_for_a_star()
    {
        var (g, rival) = Market();
        StaffMember star = g.Me.Staff[0], plain = g.Me.Staff[1];
        RoleConfig role = g.C.Role("researcher");
        long fee = EraRules.HireFee(g.C, role, g.Tick);
        Assert.Equal(fee * g.C.Talent.PackageMultiple, TalentSystem.Package(g.C, plain, g.Tick));
        Assert.Equal(MathI.MulPermille(fee * g.C.Talent.PackageMultiple, g.C.Talent.StarPackagePermille), TalentSystem.Package(g.C, star, g.Tick));
    }

    [Fact]
    public void A_talent_war_makes_everyone_dearer_in_the_chat_boom_than_in_dial_up_days()
    {
        var (early, _) = Market();
        var (boom, _) = Market(era: "chat-boom");
        long early1 = TalentSystem.Package(early.C, early.Me.Staff[1], early.Tick);
        long boom1 = TalentSystem.Package(boom.C, boom.Me.Staff[1], boom.Tick);
        Assert.True(boom1 > early1, boom1 + " vs " + early1);
    }

    // ---- counters

    [Fact]
    public void Matching_the_money_always_keeps_the_person_and_costs_the_whole_package()
    {
        var (g, rival) = Market();
        PoachOffer offer = MakeOffer(g, rival);
        StaffMember star = g.Me.Staff[0];
        star.Morale = 300;
        long cash = g.Me.Cash;
        CommandResult r = g.Do(Command.CounterPoach(g.Id, offer.Id, "money"));
        Assert.Equal(star.Id, r.CreatedId);
        Assert.Equal(cash - offer.Package, g.Me.Cash);
        Assert.Equal(300 + g.C.Talent.Counter("money").MoraleGain, star.Morale);
        Assert.Empty(g.Me.PoachOffers);
        Assert.Contains(star, g.Me.Staff);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.PoachCountered);
        Assert.Equal(1, e.Pick);
        Assert.Equal("money", e.Ref);
        Assert.Equal(offer.Package, e.Amount);
        Assert.Equal(0, Events(g, EventKind.StaffPoached));
    }

    [Fact]
    public void A_counter_costs_its_share_of_the_package_win_or_lose()
    {
        foreach (string id in new[] { "compute", "title", "mission" })
        {
            var (g, rival) = Market();
            PoachOffer offer = MakeOffer(g, rival);
            long cash = g.Me.Cash;
            g.Do(Command.CounterPoach(g.Id, offer.Id, id));
            Assert.Equal(TalentSystem.CounterCost(g.C, g.C.Talent.Counter(id), offer.Package), cash - g.Me.Cash);
        }
    }

    [Fact]
    public void The_mission_is_free_and_only_works_while_people_believe_in_the_lab()
    {
        var (g, rival) = Market();
        StaffMember star = g.Me.Staff[0];
        CounterConfig mission = g.C.Talent.Counter("mission");
        star.Morale = 900;
        g.Me.Reputation = 900;
        int happy = TalentSystem.CounterChance(g.C, g.Me, star, mission).Total;
        star.Morale = 100;
        g.Me.Reputation = 100;
        int sour = TalentSystem.CounterChance(g.C, g.Me, star, mission).Total;
        Assert.True(happy > sour + 300, happy + " vs " + sour);
        PoachOffer offer = MakeOffer(g, rival);
        long cash = g.Me.Cash;
        g.Do(Command.CounterPoach(g.Id, offer.Id, "mission"));
        Assert.Equal(cash, g.Me.Cash);
    }

    [Fact]
    public void The_chance_is_shown_in_parts_that_add_up_including_the_floor_and_the_ceiling()
    {
        var (g, _) = Market();
        StaffMember star = g.Me.Staff[0];
        foreach (CounterConfig k in g.C.Talent.Counters)
            foreach (int morale in new[] { 0, 250, 500, 800, 1000 })
                foreach (int rep in new[] { 0, 500, 1000 })
                {
                    star.Morale = morale;
                    g.Me.Reputation = rep;
                    var parts = TalentSystem.CounterChance(g.C, g.Me, star, k);
                    Assert.Equal(parts.Base + parts.MoralePart + parts.ReputationPart + parts.Clamp, parts.Total);
                    Assert.InRange(parts.Total, g.C.Talent.MinSuccessPermille, 1000);
                }
        star.Morale = 0;
        g.Me.Reputation = 0;
        var worst = TalentSystem.CounterChance(g.C, g.Me, star, g.C.Talent.Counter("mission"));
        Assert.Equal(g.C.Talent.MinSuccessPermille, worst.Total);
        Assert.True(worst.Clamp > 0);
        star.Morale = 1000;
        Assert.Equal(1000, TalentSystem.CounterChance(g.C, g.Me, star, g.C.Talent.Counter("money")).Total);
    }

    [Fact]
    public void A_counter_that_fails_costs_the_money_and_the_person_leaves_anyway()
    {
        var (g, rival) = Market(tweak: c =>
        {
            c.Talent.MinSuccessPermille = 1;
            c.Talent.Counters.First(k => k.Id == "title").BaseSuccessPermille = 0;
            c.Talent.Counters.First(k => k.Id == "title").MoraleSlopePermille = 0;
            c.Talent.Counters.First(k => k.Id == "title").ReputationSlopePermille = 0;
        });
        PoachOffer offer = MakeOffer(g, rival);
        int staff = g.Me.Staff.Count;
        long cash = g.Me.Cash;
        CommandResult r = g.Do(Command.CounterPoach(g.Id, offer.Id, "title"));
        Assert.Equal(-1, r.CreatedId);
        Assert.Equal(staff - 1, g.Me.Staff.Count);
        Assert.True(cash > g.Me.Cash);
        Assert.Empty(g.Me.PoachOffers);
        Assert.Equal(0, g.State.Events.Last(e => e.Kind == EventKind.PoachCountered).Pick);
        Assert.Equal(1, Events(g, EventKind.StaffPoached));
    }

    [Fact]
    public void A_counter_nobody_can_pay_for_a_missing_offer_or_an_unknown_counter_is_refused()
    {
        var (g, rival) = Market();
        PoachOffer offer = MakeOffer(g, rival);
        g.Refused(Command.CounterPoach(g.Id, offer.Id, "bribe"), CommandError.UnknownId);
        g.Refused(Command.CounterPoach(g.Id, offer.Id + 999, "money"), CommandError.OfferGone);
        g.Me.Cash = offer.Package - 1;
        g.Refused(Command.CounterPoach(g.Id, offer.Id, "money"), CommandError.NotEnoughCash);
        Assert.Single(g.Me.PoachOffers);
        g.Me.Cash = 0;
        g.Do(Command.CounterPoach(g.Id, offer.Id, "mission"));
        Assert.Empty(g.Me.PoachOffers);
    }

    [Fact]
    public void An_answered_offer_cannot_be_answered_twice_and_the_rival_can_try_again_later()
    {
        var (g, rival) = Market();
        PoachOffer offer = MakeOffer(g, rival);
        g.Do(Command.CounterPoach(g.Id, offer.Id, "money"));
        g.Refused(Command.CounterPoach(g.Id, offer.Id, "money"), CommandError.OfferGone);
        PoachOffer again = MakeOffer(g, rival);
        Assert.NotEqual(offer.Id, again.Id);
    }

    [Fact]
    public void Letting_the_person_go_yourself_closes_the_offer_and_the_save_is_still_valid()
    {
        var (g, rival) = Market(researchers: 1);
        MakeOffer(g, rival);
        g.Do(Command.Fire(g.Id, "researcher", 1));
        Assert.Empty(g.Me.PoachOffers);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Weeks(8);
        Assert.Equal(0, Events(g, EventKind.StaffPoached));
    }

    [Fact]
    public void An_offer_to_someone_who_has_gone_is_dropped_without_a_second_loss()
    {
        var (g, rival) = Market();
        MakeOffer(g, rival);
        g.Me.Staff.RemoveAt(0);
        g.Weeks(1);
        Assert.Empty(g.Me.PoachOffers);
        Assert.Equal(0, Events(g, EventKind.StaffPoached));
    }

    // ---- poaching back

    static StaffMember AddRivalStaff(LabState rival, string role, string trait, int morale = 500)
    {
        var p = new StaffMember { Id = 90_000 + rival.Staff.Count, RoleId = role, TraitId = trait, Morale = morale };
        rival.Staff.Add(p);
        return p;
    }

    static (Game game, LabState rival) Raid(Action<GameConfig> tweak = null)
    {
        var (g, rival) = Market(tweak: c =>
        {
            c.Talent.PoachBackBasePermille = 1000;
            if (tweak != null) tweak(c);
        }, researchers: 1);
        rival.Staff.RemoveAll(p => p.RoleId == "researcher");
        rival.FeelingToPlayer = 0;
        g.Me.Reputation = 500;
        return (g, rival);
    }

    [Fact]
    public void A_successful_poach_moves_the_best_person_for_the_headhunter_and_the_package()
    {
        var (g, rival) = Raid();
        StaffMember star = AddRivalStaff(rival, "researcher", "star", morale: 500);
        AddRivalStaff(rival, "researcher", "steady");
        int mine = g.Me.Staff.Count;
        long package = TalentSystem.Package(g.C, star, g.Tick);
        long fee = MathI.MulPermille(package, g.C.Talent.PoachBackFeePermille);
        long cash = g.Me.Cash;
        CommandResult r = g.Do(Command.PoachStaff(g.Id, rival.Id, "researcher"));
        Assert.Equal(star.Id, r.CreatedId);
        Assert.Equal(mine + 1, g.Me.Staff.Count);
        Assert.Contains(g.Me.Staff, p => p.Id == star.Id && p.TraitId == "star");
        Assert.DoesNotContain(rival.Staff, p => p.Id == star.Id);
        Assert.Equal(1, rival.Staff.Count(p => p.RoleId == "researcher"));
        Assert.Equal(cash - fee - package, g.Me.Cash);
        Assert.Equal(g.C.StaffPeople.MoraleBaseline, star.Morale);
        Assert.Equal(-g.C.Talent.PoachBackFeelingHit, rival.FeelingToPlayer);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.PoachBackTried);
        Assert.Equal(1, e.Pick);
        Assert.Equal(fee + package, e.Amount);
        Assert.Equal(rival.Id, e.SubjectId);
    }

    [Fact]
    public void A_poach_that_fails_only_costs_the_headhunter_and_annoys_the_rival_a_little()
    {
        var (g, rival) = Raid(c => { c.Talent.PoachBackBasePermille = 0; c.Talent.PoachBackMoraleSlopePermille = 0; c.Talent.PoachBackReputationSlopePermille = 0; c.Talent.MinSuccessPermille = 1; });
        StaffMember target = AddRivalStaff(rival, "researcher", "steady");
        long package = TalentSystem.Package(g.C, target, g.Tick);
        long fee = MathI.MulPermille(package, g.C.Talent.PoachBackFeePermille);
        long cash = g.Me.Cash;
        int mine = g.Me.Staff.Count;
        CommandResult r = g.Do(Command.PoachStaff(g.Id, rival.Id, "researcher"));
        Assert.Equal(-1, r.CreatedId);
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Equal(mine, g.Me.Staff.Count);
        Assert.Contains(rival.Staff, p => p.Id == target.Id);
        Assert.Equal(-g.C.Talent.PoachBackFailFeelingHit, rival.FeelingToPlayer);
        Assert.Equal(0, g.State.Events.Last(x => x.Kind == EventKind.PoachBackTried).Pick);
    }

    [Fact]
    public void A_poach_takes_a_star_before_anyone_else_and_the_unhappiest_of_the_rest()
    {
        var (g, rival) = Raid();
        AddRivalStaff(rival, "researcher", "steady", morale: 100);
        StaffMember star = AddRivalStaff(rival, "researcher", "star", morale: 900);
        Assert.Same(star, TalentSystem.BestTarget(g.C, rival, "researcher"));
        rival.Staff.Remove(star);
        StaffMember sad = AddRivalStaff(rival, "researcher", "steady", morale: 50);
        Assert.Same(sad, TalentSystem.BestTarget(g.C, rival, "researcher"));
        Assert.Null(TalentSystem.BestTarget(g.C, rival, "engineer-nobody"));
    }

    [Fact]
    public void An_unhappy_target_and_a_famous_lab_make_a_poach_likelier_and_the_parts_add_up()
    {
        var (g, rival) = Raid();
        StaffMember target = AddRivalStaff(rival, "researcher", "steady", morale: 500);
        g.C.Talent.PoachBackBasePermille = 400;
        g.Me.Reputation = 500;
        var mid = TalentSystem.PoachBackChance(g.C, g.Me, target);
        target.Morale = 100;
        var sad = TalentSystem.PoachBackChance(g.C, g.Me, target);
        g.Me.Reputation = 900;
        var famous = TalentSystem.PoachBackChance(g.C, g.Me, target);
        Assert.True(sad.Total > mid.Total);
        Assert.True(famous.Total > sad.Total);
        foreach (var parts in new[] { mid, sad, famous })
            Assert.Equal(parts.Base + parts.MoralePart + parts.ReputationPart + parts.Clamp, parts.Total);
    }

    [Fact]
    public void A_poach_that_cannot_happen_is_refused_and_costs_nothing()
    {
        var (g, rival) = Raid();
        long cash = g.Me.Cash;
        g.Refused(Command.PoachStaff(g.Id, rival.Id, "wizard"), CommandError.UnknownId);
        g.Refused(Command.PoachStaff(g.Id, 9999, "researcher"), CommandError.UnknownLab);
        g.Refused(Command.PoachStaff(g.Id, g.Id, "researcher"), CommandError.UnknownLab);
        g.Refused(Command.PoachStaff(g.Id, rival.Id, "researcher"), CommandError.NotEnoughStaff);
        AddRivalStaff(rival, "researcher", "steady");
        g.Hire("researcher", 4);
        g.Refused(Command.PoachStaff(g.Id, rival.Id, "researcher"), CommandError.NoSeats);
        g.Do(Command.Fire(g.Id, "researcher", 1));
        long cashAfter = g.Me.Cash;
        g.Me.Cash = 10;
        g.Refused(Command.PoachStaff(g.Id, rival.Id, "researcher"), CommandError.NotEnoughCash);
        g.Me.Cash = cashAfter;
        rival.Bankrupt = true;
        g.Refused(Command.PoachStaff(g.Id, rival.Id, "researcher"), CommandError.TargetNotEligible);
        Assert.True(cash >= cashAfter);
    }

    [Fact]
    public void Poaching_over_and_over_turns_a_neutral_rival_into_a_feud_that_makes_offers_back()
    {
        var (g, rival) = Raid(c => c.Talent.PoachBackFeelingHit = 200);
        for (int i = 0; i < 3; i++)
        {
            AddRivalStaff(rival, "researcher", "steady");
            g.Me.Staff.RemoveAll(p => p.RoleId == "researcher" && g.Me.Staff.Count(q => q.RoleId == "researcher") > 1);
            g.Do(Command.PoachStaff(g.Id, rival.Id, "researcher"));
            g.Do(Command.Fire(g.Id, "researcher", 1));
        }
        Assert.True(RivalSystem.IsFeud(g.C, rival), "feeling " + rival.FeelingToPlayer);
        g.Hire("researcher", 1);
        g.Me.Staff.Last().TraitId = "star";
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        Assert.Single(g.Me.PoachOffers);
    }

    [Fact]
    public void A_poached_star_earns_the_labs_research_bonus_straight_away()
    {
        var (g, rival) = Raid();
        long Research(Game game)
        {
            game.Weeks(1);
            return game.Me.LastWeek.ResearchGained;
        }
        g.Me.Staff[0].TraitId = "steady";
        long before = Research(g);
        g.Do(Command.Fire(g.Id, "researcher", 1));
        AddRivalStaff(rival, "researcher", "star");
        g.Do(Command.Hire(g.Id, "researcher", 0 + 1));
        g.Me.Staff.RemoveAll(p => p.RoleId == "researcher");
        g.Do(Command.PoachStaff(g.Id, rival.Id, "researcher"));
        long after = Research(g);
        Assert.True(after > before, after + " vs " + before);
    }

    // ---- saves and whole games

    [Fact]
    public void A_game_with_an_open_offer_saves_and_loads_and_plays_on_the_same()
    {
        var (g, rival) = Market();
        PoachOffer offer = MakeOffer(g, rival);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        string json = g.Sim.Save();
        Simulation again = Simulation.Load(g.C, json);
        Assert.Single(again.Player.PoachOffers);
        Assert.Equal(offer.Package, again.Player.PoachOffers[0].Package);
        g.Do(Command.CounterPoach(g.Id, offer.Id, "compute"));
        again.Apply(Command.CounterPoach(again.Player.Id, offer.Id, "compute"));
        g.Weeks(30);
        again.StepMany(30);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_save_from_before_the_talent_market_loads_with_no_offers_and_plays_on()
    {
        var (g, _) = Market();
        g.Weeks(5);
        g.Me.CultureShift = 0;
        g.Me.Alumni.Clear();
        string old = SaveSchemaPretend.At(g.C, 22, g.Sim.Save());
        Assert.DoesNotContain("poachOffers", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => Assert.Empty(l.PoachOffers));
        loaded.StepMany(10);
        g.Weeks(10);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void A_save_that_names_an_offer_for_nobody_is_a_problem()
    {
        var (g, rival) = Market();
        PoachOffer offer = MakeOffer(g, rival);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        offer.StaffId = 12345;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("not on the payroll"));
        offer.StaffId = g.Me.Staff[0].Id;
        offer.RivalLabId = 9999;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("unknown lab"));
    }

    [Fact]
    public void A_replayed_game_with_counters_and_poaches_comes_out_the_same()
    {
        var (g, rival) = Raid();
        AddRivalStaff(rival, "researcher", "star");
        g.Do(Command.PoachStaff(g.Id, rival.Id, "researcher"));
        g.Me.Staff.Last().TraitId = "star";
        rival.FeelingToPlayer = -900;
        StaffSystem.TryPoach(g.C, g.State, rival, g.Me);
        if (g.Me.PoachOffers.Count > 0) g.Do(Command.CounterPoach(g.Id, g.Me.PoachOffers[0].Id, "mission"));
        g.Weeks(40);
        Simulation copy = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(20);
        copy.StepMany(20);
        Assert.Equal(g.Sim.Digest(), copy.Digest());
    }

    [Fact]
    public void An_autopilot_that_never_answers_still_plays_five_years_with_a_valid_save_against_a_hostile_rival()
    {
        var g = Game.New(seed: 21, tweak: c => c.StaffPeople.PoachChancePermille = 400);
        g.Me.Staff.Clear();
        g.Rival("openaye").FeelingToPlayer = -900;
        g.Autopilot(260);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Assert.True(g.State.Events.Any(e => e.Kind == EventKind.PoachOffered) || g.Me.Staff.Count > 0);
    }

    [Fact]
    public void Every_new_event_reads_as_a_sentence_for_the_screen()
    {
        var (g, rival) = Market();
        PoachOffer offer = MakeOffer(g, rival);
        g.Do(Command.CounterPoach(g.Id, offer.Id, "money"));
        AddRivalStaff(rival, "researcher", "star");
        g.C.Talent.PoachBackBasePermille = 1000;
        g.Do(Command.Fire(g.Id, "researcher", 1));
        g.Do(Command.PoachStaff(g.Id, rival.Id, "researcher"));
        foreach (EventKind kind in new[] { EventKind.PoachOffered, EventKind.PoachCountered, EventKind.PoachBackTried })
        {
            GameEvent e = g.State.Events.Last(x => x.Kind == kind);
            string text = EventText.Describe(g.C, g.State, e);
            Assert.False(string.IsNullOrWhiteSpace(text), kind.ToString());
            Assert.DoesNotContain("{", text);
        }
    }

    [Fact]
    public void The_talent_page_lists_every_counter()
    {
        GameConfig c = TestData.Config();
        string page = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "docs", "content", "talent-market.md"));
        foreach (CounterConfig k in c.Talent.Counters) Assert.Contains("`" + k.Id + "`", page);
    }
}
