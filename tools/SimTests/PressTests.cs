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

/// <summary>The press: press rooms dampen a scandal and sharpen a launch, journalists ask about the beats, the player answers, and a lab can leak a rival's incident.</summary>
public class PressTests
{
    static Game Desk(string era = "chat-boom", Action<GameConfig> tweak = null, bool rivals = true, long seed = 7)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = era };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c => { c.RolePerks.Clear(); if (tweak != null) tweak(c); }), options));
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    /// <summary>A working lab with one trained model that has not been released.</summary>
    static ModelState Trained(Game g)
    {
        g.WithStarterLab();
        ModelState model = g.Train("mini", 600);
        g.UntilTrained(model);
        model.Capability = 1_000_000;
        return model;
    }

    static void Launch(Game g, ModelState model)
    {
        g.Do(Command.ReleaseModel(g.Id, model.Id, "consumer-assistant"));
    }

    static PressInquiry Ask(Game g, string beat, int points = 50, string what = "Mini")
    {
        int before = g.Me.PressInquiries.Count;
        PressSystem.OnBeat(g.C, g.State, g.Me, beat, points, what);
        Assert.Equal(before + 1, g.Me.PressInquiries.Count);
        return g.Me.PressInquiries.Last();
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
    public void There_are_four_beats_three_journalists_and_three_answers_and_every_beat_has_a_reporter()
    {
        PressConfig p = TestData.Config().Press;
        Assert.Equal(new[] { "launch", "crown", "incident", "hearing" }, p.Beats.Select(b => b.Id).ToArray());
        Assert.Equal(3, p.Journalists.Count);
        Assert.Equal(new[] { "honest", "spin", "exclusive" }, p.Responses.Select(r => r.Id).ToArray());
        foreach (BeatConfig b in p.Beats) Assert.Contains(p.Journalists, j => j.Beats.Contains(b.Id));
        Assert.True(p.Beat("launch").Good);
        Assert.True(p.Beat("crown").Good);
        Assert.False(p.Beat("incident").Good);
        Assert.False(p.Beat("hearing").Good);
    }

    [Fact]
    public void Honest_is_free_and_sure_and_wins_back_the_least()
    {
        PressConfig p = TestData.Config().Press;
        PressResponseConfig honest = p.Response("honest");
        Assert.Equal(0, honest.CostHireFees);
        Assert.Equal(1000, honest.BaseSuccessPermille);
        Assert.Equal(0, honest.BackfirePermille);
        foreach (PressResponseConfig other in p.Responses.Where(r => r.Id != "honest"))
        {
            Assert.True(other.RecoverPermille > honest.RecoverPermille, other.Id);
            Assert.True(other.CostHireFees > 0, other.Id);
        }
    }

    [Fact]
    public void The_press_room_opens_in_the_punchline_and_the_auditorium_in_the_chat_boom_and_neither_seats_anyone()
    {
        GameConfig c = TestData.Config();
        RoomTypeConfig press = c.RoomType("press-room"), hall = c.RoomType("auditorium");
        Assert.Equal("punchline", press.Era);
        Assert.Equal("chat-boom", hall.Era);
        Assert.Equal(RoomPurpose.Press, press.Purpose);
        Assert.Equal(RoomPurpose.Press, hall.Purpose);
        Assert.True(string.IsNullOrEmpty(press.SeatsRole));
        Assert.True(string.IsNullOrEmpty(hall.SeatsRole));
        Assert.True(hall.BuildCostPerTile > press.BuildCostPerTile);
    }

    [Fact]
    public void The_press_rooms_cannot_be_built_before_their_era()
    {
        Game early = Desk("dial-up-days", rivals: false);
        int x = 0, y = 0;
        Assert.True(LotGrid.FindFreeRect(early.C.Lot(early.Me.LotId), early.Me.Rooms, 3, 3, out x, out y));
        early.Refused(Command.BuildRoom(early.Id, "press-room", x, y, 3, 3), CommandError.EraLocked);
    }

    // ---- the validator

    [Fact]
    public void The_default_press_table_is_valid()
    {
        Assert.Empty(ConfigValidator.Problems(TestData.Config()));
    }

    [Fact]
    public void The_validator_catches_a_beat_nobody_covers_and_a_journalist_with_an_unknown_beat()
    {
        AssertProblem(Problems(c => c.Press.Journalists.ForEach(j => j.Beats.Remove("crown"))), "covered by no journalist");
        AssertProblem(Problems(c => c.Press.Journalists[0].Beats.Add("gossip")), "unknown beat");
    }

    [Fact]
    public void The_validator_catches_a_bad_stake_and_empty_tables_and_wild_numbers()
    {
        AssertProblem(Problems(c => c.Press.Beat("incident").StakePoints = 10), "stakePoints must be 0");
        AssertProblem(Problems(c => c.Press.Responses.Clear()), "press.responses is empty");
        AssertProblem(Problems(c => c.Press.Journalists.Clear()), "press.journalists is empty");
        AssertProblem(Problems(c => c.Press.Beats.Clear()), "press.beats is empty");
        AssertProblem(Problems(c => c.Press.MaxDampingPermille = 1000), "no lab is immune");
        AssertProblem(Problems(c => c.Press.InquiryWeeks = 0), "inquiryWeeks");
        AssertProblem(Problems(c => c.Press.MinSuccessPermille = 0), "no answer is hopeless");
        AssertProblem(Problems(c => c.Press.Response("spin").BaseSuccessPermille = 1001), "baseSuccessPermille");
        AssertProblem(Problems(c => c.Press.Responses.Add(c.Press.Response("spin"))), "\"spin\" twice");
        AssertProblem(Problems(c => c.Press.Journalists[0].BackfirePermille = -1), "backfirePermille");
    }

    [Fact]
    public void The_validator_wants_the_sales_role_the_costs_are_priced_in()
    {
        AssertProblem(Problems(c => c.Roles.RemoveAll(r => r.Id == "sales")), "\"sales\"");
    }

    // ---- press rooms

    [Fact]
    public void Press_room_tiles_add_damping_and_buzz_under_their_caps()
    {
        Game g = Desk("chat-boom", rivals: false);
        Assert.Equal(0, PressSystem.Damping(g.C, g.Me));
        Assert.Equal(0, PressSystem.Buzz(g.C, g.Me));
        g.Build("press-room", 3, 3);
        Assert.Equal(9 * 10, PressSystem.Damping(g.C, g.Me));
        Assert.Equal(9 * 12, PressSystem.Buzz(g.C, g.Me));
        g.Build("auditorium", 5, 4);
        Assert.Equal(9 * 10 + 20 * 8, PressSystem.Damping(g.C, g.Me));
        Assert.Equal(9 * 12 + 20 * 15, PressSystem.Buzz(g.C, g.Me));
    }

    [Fact]
    public void Together_the_press_rooms_never_pass_the_press_caps()
    {
        Game g = Desk("chat-boom", c => { c.Press.MaxDampingPermille = 150; c.Press.MaxBuzzPermille = 200; }, rivals: false);
        g.Build("press-room", 6, 5);
        g.Build("auditorium", 6, 4);
        Assert.True(RoomPerks.Permille(g.C, g.Me, RoomPerkKind.ScandalDamping) > 150);
        Assert.Equal(150, PressSystem.Damping(g.C, g.Me));
        Assert.Equal(200, PressSystem.Buzz(g.C, g.Me));
    }

    [Fact]
    public void Damping_takes_a_share_off_a_loss_and_leaves_zero_and_no_room_alone()
    {
        Game g = Desk("chat-boom", rivals: false);
        Assert.Equal(100, PressSystem.Damp(g.C, g.Me, 100));
        g.Build("press-room", 6, 5);
        Assert.Equal(300, PressSystem.Damping(g.C, g.Me));
        Assert.Equal(70, PressSystem.Damp(g.C, g.Me, 100));
        Assert.Equal(0, PressSystem.Damp(g.C, g.Me, 0));
        Assert.Equal(-5, PressSystem.Damp(g.C, g.Me, -5));
    }

    [Fact]
    public void Pulling_the_press_room_down_ends_the_damping_the_same_week()
    {
        Game g = Desk("chat-boom", rivals: false);
        int room = g.Build("press-room", 6, 5);
        Assert.Equal(300, PressSystem.Damping(g.C, g.Me));
        g.Do(Command.DemolishRoom(g.Id, room));
        Assert.Equal(0, PressSystem.Damping(g.C, g.Me));
        Assert.Equal(0, PressSystem.Buzz(g.C, g.Me));
    }

    // ---- launch

    [Fact]
    public void A_launch_with_a_press_room_earns_the_buzz_and_says_so()
    {
        Game g = Desk(rivals: false);
        g.Build("press-room", 6, 5);
        ModelState model = Trained(g);
        g.Me.Reputation = 500;
        int points = PressSystem.LaunchPoints(g.C, g.Me);
        Assert.Equal((int)MathI.MulPermille(g.C.Press.LaunchBuzzPoints, 30 * 12), points);
        Assert.True(points > 0);
        Launch(g, model);
        Assert.Equal(500 + points, g.Me.Reputation);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LaunchBuzz);
        Assert.Equal(g.Id, e.LabId);
        Assert.Equal(model.Id, e.SubjectId);
        Assert.Equal(points, e.Amount);
    }

    [Fact]
    public void A_launch_without_a_press_room_earns_no_buzz_and_changes_no_reputation()
    {
        Game g = Desk(rivals: false);
        ModelState model = Trained(g);
        g.Me.Reputation = 500;
        Launch(g, model);
        Assert.Equal(0, Events(g, EventKind.LaunchBuzz));
        Assert.Equal(500, g.Me.Reputation);
    }

    [Fact]
    public void A_second_release_of_the_same_model_earns_no_second_buzz()
    {
        Game g = Desk(rivals: false);
        g.Build("press-room", 6, 5);
        ModelState model = Trained(g);
        Launch(g, model);
        Assert.Equal(1, Events(g, EventKind.LaunchBuzz));
        int inquiries = g.Me.PressInquiries.Count;
        g.Do(Command.ReleaseModel(g.Id, model.Id, new[] { "consumer-assistant", "writing-tools" }));
        Assert.Equal(1, Events(g, EventKind.LaunchBuzz));
        Assert.Equal(inquiries, g.Me.PressInquiries.Count);
    }

    [Fact]
    public void A_launch_brings_a_journalist_who_covers_launches()
    {
        Game g = Desk(rivals: false);
        ModelState model = Trained(g);
        Launch(g, model);
        PressInquiry q = Assert.Single(g.Me.PressInquiries);
        Assert.Equal("launch", q.BeatId);
        Assert.Equal(g.C.Press.Beat("launch").StakePoints, q.Stake);
        Assert.Equal(model.Name, q.Ref);
        Assert.Contains("launch", g.C.Press.Journalist(q.JournalistId).Beats);
        Assert.Equal(g.Tick + g.C.Press.InquiryWeeks, q.ExpiresTick);
        Assert.Equal(1, Events(g, EventKind.PressInquiry));
    }

    [Fact]
    public void A_rivals_launch_is_not_the_players_story()
    {
        Game g = Desk();
        LabState rival = g.Rival("openaye");
        var model = new ModelState { Id = 900, Name = "Rival One", Status = ModelStatus.Released };
        PressSystem.OnLaunch(g.C, g.State, rival, model);
        Assert.Empty(g.Me.PressInquiries);
        Assert.Empty(rival.PressInquiries);
    }

    // ---- scandal

    /// <summary>A lab with one released, misaligned model and a certain incident each week, and a press room of the given side (none for 0).</summary>
    static Game Scandal(int roomSide, Action<GameConfig> tweak = null)
    {
        Game g = Desk("chat-boom", c =>
        {
            c.Safety.IncidentChancePermille = 1000;
            if (tweak != null) tweak(c);
        }, rivals: false, seed: 5);
        if (roomSide > 0) g.Build("press-room", roomSide, roomSide);
        ModelState model = Trained(g);
        Launch(g, model);
        g.Me.PressInquiries.Clear();
        model.Alignment = 300;
        model.DisplayedAlignment = 300;
        g.Me.Reputation = 800;
        return g;
    }

    [Fact]
    public void An_incident_brings_a_question_whose_stake_is_what_the_incident_cost()
    {
        Game g = Scandal(0);
        g.Weeks(1);
        Assert.True(Events(g, EventKind.Incident) >= 1);
        PressInquiry q = g.Me.PressInquiries.First(x => x.BeatId == "incident");
        Assert.True(q.Stake > 0);
        Assert.Contains("incident", g.C.Press.Journalist(q.JournalistId).Beats);
    }

    [Fact]
    public void A_press_room_cuts_what_an_incident_costs_and_the_question_stakes_the_cut_figure()
    {
        Game bare = Scandal(0);
        Game roomed = Scandal(6);
        bare.Weeks(1);
        roomed.Weeks(1);
        Assert.Equal(1, Events(bare, EventKind.Incident));
        Assert.Equal(1, Events(roomed, EventKind.Incident));
        int bareStake = bare.Me.PressInquiries.Single(x => x.BeatId == "incident").Stake;
        int roomedStake = roomed.Me.PressInquiries.Single(x => x.BeatId == "incident").Stake;
        Assert.Equal(PressSystem.Damp(roomed.C, roomed.Me, bareStake), roomedStake);
        Assert.True(roomedStake < bareStake);
        Assert.True(roomed.Me.Reputation > bare.Me.Reputation);
    }

    [Fact]
    public void A_hearing_costs_less_with_a_press_room_and_asks_a_question_about_what_it_cost()
    {
        Game g = Desk("chat-boom", rivals: false);
        g.Build("press-room", 6, 5);
        g.Me.Reputation = 500;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        g.Me.Ledger.ThisMonth.Revenue = 0;
        g.Me.Ledger.LastWeek.Revenue = 0;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        int cut = PressSystem.Damp(g.C, g.Me, -g.C.World.Attention.HearingReputation);
        Assert.Equal(500 - cut, g.Me.Reputation);
        Assert.True(cut < -g.C.World.Attention.HearingReputation);
        PressInquiry q = g.Me.PressInquiries.Single(x => x.BeatId == "hearing");
        Assert.Equal(cut, q.Stake);
    }

    [Fact]
    public void A_hearing_the_lab_prepared_for_is_good_news_and_asks_nothing()
    {
        Game g = Desk("chat-boom", rivals: false);
        g.Build("policy-office", 4, 4);
        g.Hire("policy", g.C.World.Attention.PreparedPolicyStaff);
        g.Me.Reputation = 500;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        Assert.Equal(500 + g.C.World.Attention.HearingReputationPrepared, g.Me.Reputation);
        Assert.Empty(g.Me.PressInquiries);
    }

    [Fact]
    public void Winning_the_crown_asks_for_a_profile_staking_what_the_crown_paid()
    {
        Game g = Desk("chat-boom", rivals: false);
        g.Me.Models.Add(new ModelState { Id = 1, Capability = 950, Status = ModelStatus.Released, Alignment = 800, Name = "Ace" });
        GauntletSystem.RunYear(g.C, g.State);
        PressInquiry q = Assert.Single(g.Me.PressInquiries);
        Assert.Equal("crown", q.BeatId);
        Assert.Equal(g.C.Gauntlet.CrownReputation, q.Stake);
        Assert.Equal("Ace", q.Ref);
    }

    // ---- the desk

    [Fact]
    public void The_desk_holds_a_few_questions_and_a_beat_that_arrives_when_it_is_full_goes_unasked()
    {
        Game g = Desk(rivals: false);
        for (int i = 0; i < g.C.Press.MaxOpenInquiries; i++) Ask(g, "incident");
        int events = Events(g, EventKind.PressInquiry);
        PressSystem.OnBeat(g.C, g.State, g.Me, "incident", 50, "Mini");
        Assert.Equal(g.C.Press.MaxOpenInquiries, g.Me.PressInquiries.Count);
        Assert.Equal(events, Events(g, EventKind.PressInquiry));
    }

    [Fact]
    public void A_beat_that_moved_nothing_a_bankrupt_lab_and_an_unknown_beat_ask_nothing()
    {
        Game g = Desk(rivals: false);
        PressSystem.OnBeat(g.C, g.State, g.Me, "incident", 0, "Mini");
        PressSystem.OnBeat(g.C, g.State, g.Me, "gossip", 50, "Mini");
        Assert.Empty(g.Me.PressInquiries);
        g.Me.Bankrupt = true;
        PressSystem.OnBeat(g.C, g.State, g.Me, "incident", 50, "Mini");
        Assert.Empty(g.Me.PressInquiries);
    }

    [Fact]
    public void Journalists_take_the_beats_they_cover_in_turn_by_the_week()
    {
        Game g = Desk(rivals: false);
        var seen = new HashSet<string>();
        for (int week = 0; week < 6; week++)
        {
            g.State.Tick = 100 + week;
            PressInquiry q = Ask(g, "incident");
            Assert.Contains("incident", g.C.Press.Journalist(q.JournalistId).Beats);
            seen.Add(q.JournalistId);
            g.Me.PressInquiries.Clear();
        }
        Assert.Equal(g.C.Press.Journalists.Count(j => j.Beats.Contains("incident")), seen.Count);
        foreach (string id in seen) Assert.DoesNotContain("crown-only", id);
    }

    [Fact]
    public void A_question_nobody_answers_lapses_when_its_time_is_up_and_changes_nothing()
    {
        Game g = Desk(rivals: false);
        PressInquiry q = Ask(g, "incident");
        int rep = g.Me.Reputation;
        long cash = g.Me.Cash;
        g.State.Tick = q.ExpiresTick - 1;
        PressSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Single(g.Me.PressInquiries);
        g.State.Tick = q.ExpiresTick;
        PressSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.PressInquiries);
        Assert.Equal(rep, g.Me.Reputation);
        Assert.Equal(cash, g.Me.Cash);
    }

    // ---- answering

    [Fact]
    public void Telling_the_truth_is_free_and_wins_back_a_quarter_of_a_lost_stake()
    {
        Game g = Desk(rivals: false);
        g.Me.Reputation = 400;
        PressInquiry q = Ask(g, "incident", 80);
        long cash = g.Me.Cash;
        g.Do(Command.AnswerPress(g.Id, q.Id, "honest"));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(400 + 20, g.Me.Reputation);
        Assert.Empty(g.Me.PressInquiries);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.PressAnswered);
        Assert.Equal(1, e.Pick);
        Assert.Equal(20, e.Amount);
    }

    [Fact]
    public void Spin_costs_the_eras_sales_fees_and_when_it_lands_wins_most_of_the_stake()
    {
        Game g = Desk(rivals: false, tweak: c => c.Press.Response("spin").BaseSuccessPermille = 1000);
        g.Me.Reputation = 400;
        PressInquiry q = Ask(g, "incident", 100);
        PressResponseConfig spin = g.C.Press.Response("spin");
        long cost = EraRules.HireFee(g.C, g.C.Role("sales"), g.Tick) * spin.CostHireFees;
        Assert.Equal(cost, PressSystem.AnswerCost(g.C, g.Tick, spin));
        long cash = g.Me.Cash;
        g.Do(Command.AnswerPress(g.Id, q.Id, "spin"));
        Assert.Equal(cash - cost, g.Me.Cash);
        Assert.Equal(400 + 70, g.Me.Reputation);
    }

    [Fact]
    public void Spin_that_does_not_land_costs_a_share_of_the_stake_scaled_by_the_journalist()
    {
        Game g = Desk(rivals: false, tweak: c => { c.Press.MinSuccessPermille = 1; c.Press.Response("spin").BaseSuccessPermille = 0; c.Press.Response("spin").ReputationSlopePermille = 0; });
        foreach (JournalistConfig j in g.C.Press.Journalists) j.ChancePermille = 0;
        g.Me.Reputation = 600;
        PressInquiry q = Ask(g, "incident", 100);
        q.JournalistId = "nell-hype";
        int loss = PressSystem.BackfirePoints(g.C, q, g.C.Press.Response("spin"));
        Assert.Equal(100 * 600 / 1000 * 1500 / 1000, loss);
        g.Do(Command.AnswerPress(g.Id, q.Id, "spin"));
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.PressAnswered);
        Assert.Equal(0, e.Pick);
        Assert.Equal(600 - loss, g.Me.Reputation);
        Assert.Equal(-loss, e.Amount);
    }

    [Fact]
    public void A_tabloid_makes_a_gaffe_bigger_than_a_trade_writer_does()
    {
        GameConfig c = TestData.Config();
        var q = new PressInquiry { Stake = 100 };
        q.JournalistId = "nell-hype";
        int tabloid = PressSystem.BackfirePoints(c, q, c.Press.Response("spin"));
        q.JournalistId = "ravi-benchmark";
        int trade = PressSystem.BackfirePoints(c, q, c.Press.Response("spin"));
        Assert.True(tabloid > trade, tabloid + " vs " + trade);
        q.JournalistId = "ada-fine-print";
        int fair = PressSystem.BackfirePoints(c, q, c.Press.Response("spin"));
        Assert.InRange(fair, trade, tabloid);
    }

    [Fact]
    public void The_chance_is_taken_apart_and_a_press_room_a_good_name_and_a_friendly_writer_each_raise_it()
    {
        Game g = Desk("chat-boom", rivals: false);
        PressResponseConfig spin = g.C.Press.Response("spin");
        PressInquiry q = Ask(g, "incident");
        q.JournalistId = "ada-fine-print";
        g.Me.Reputation = 500;
        PressSystem.ChanceParts plain = PressSystem.AnswerChance(g.C, g.Me, q, spin);
        Assert.Equal(spin.BaseSuccessPermille, plain.Total);
        Assert.Equal(plain.Base + plain.JournalistPart + plain.RoomPart + plain.ReputationPart + plain.Clamp, plain.Total);

        g.Build("press-room", 6, 5);
        Assert.Equal(300 * spin.RoomSlopePermille / 1000, PressSystem.AnswerChance(g.C, g.Me, q, spin).RoomPart);
        Assert.True(PressSystem.AnswerChance(g.C, g.Me, q, spin).Total > plain.Total);

        g.Me.Reputation = 900;
        Assert.Equal(400 * spin.ReputationSlopePermille / 1000, PressSystem.AnswerChance(g.C, g.Me, q, spin).ReputationPart);

        g.Me.Reputation = 500;
        int ada = PressSystem.AnswerChance(g.C, g.Me, q, spin).Total;
        q.JournalistId = "ravi-benchmark";
        Assert.True(PressSystem.AnswerChance(g.C, g.Me, q, spin).Total > ada);
        q.JournalistId = "nell-hype";
        Assert.True(PressSystem.AnswerChance(g.C, g.Me, q, spin).Total < ada);
    }

    [Fact]
    public void No_answer_is_ever_hopeless_or_certain_beyond_the_ceiling()
    {
        Game g = Desk("chat-boom", c => { c.Press.Response("spin").BaseSuccessPermille = 0; c.Press.Response("spin").ReputationSlopePermille = 0; }, rivals: false);
        PressInquiry q = Ask(g, "incident");
        q.JournalistId = "nell-hype";
        PressSystem.ChanceParts low = PressSystem.AnswerChance(g.C, g.Me, q, g.C.Press.Response("spin"));
        Assert.Equal(g.C.Press.MinSuccessPermille, low.Total);
        Assert.True(low.Clamp > 0);
        g.C.Press.Response("exclusive").BaseSuccessPermille = 1000;
        q.JournalistId = "ravi-benchmark";
        Assert.Equal(1000, PressSystem.AnswerChance(g.C, g.Me, q, g.C.Press.Response("exclusive")).Total);
    }

    [Fact]
    public void An_answer_that_lands_cannot_push_reputation_over_a_thousand_and_a_gaffe_cannot_take_it_below_zero()
    {
        Game g = Desk(rivals: false, tweak: c => { c.Press.Response("spin").BaseSuccessPermille = 1000; });
        g.Me.Reputation = 990;
        PressInquiry q = Ask(g, "launch", 200);
        g.Do(Command.AnswerPress(g.Id, q.Id, "spin"));
        Assert.Equal(1000, g.Me.Reputation);
        Assert.Equal(10, g.State.Events.Last(x => x.Kind == EventKind.PressAnswered).Amount);

        Game h = Desk(rivals: false, tweak: c => { c.Press.MinSuccessPermille = 1; c.Press.Response("spin").BaseSuccessPermille = 0; c.Press.Response("spin").ReputationSlopePermille = 0; });
        foreach (JournalistConfig j in h.C.Press.Journalists) j.ChancePermille = 0;
        h.Me.Reputation = 5;
        PressInquiry r = Ask(h, "incident", 300);
        h.Do(Command.AnswerPress(h.Id, r.Id, "spin"));
        Assert.Equal(0, h.Me.Reputation);
    }

    [Fact]
    public void Answering_is_refused_for_an_unknown_answer_a_closed_question_and_an_empty_purse_and_changes_nothing()
    {
        Game g = Desk(rivals: false);
        PressInquiry q = Ask(g, "incident");
        g.Refused(Command.AnswerPress(g.Id, q.Id, "gossip"), CommandError.UnknownId);
        g.Refused(Command.AnswerPress(g.Id, q.Id + 1000, "honest"), CommandError.InquiryGone);
        g.Me.Cash = 0;
        g.Refused(Command.AnswerPress(g.Id, q.Id, "exclusive"), CommandError.NotEnoughCash);
        Assert.Single(g.Me.PressInquiries);
        g.Do(Command.AnswerPress(g.Id, q.Id, "honest"));
        g.Refused(Command.AnswerPress(g.Id, q.Id, "honest"), CommandError.InquiryGone);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_answer()
    {
        Game g = Desk(rivals: false);
        PressInquiry q = Ask(g, "incident");
        g.Me.Bankrupt = true;
        Assert.False(g.Try(Command.AnswerPress(g.Id, q.Id, "honest")).Ok);
    }

    // ---- leaking

    /// <summary>A rival that has just had an incident with one of its models.</summary>
    static void Troubled(Game g, LabState rival, int weeksAgo = 2)
    {
        rival.Models.Clear();
        rival.Models.Add(new ModelState { Id = 901, Name = "Rival Oops", Status = ModelStatus.Released, Incidents = 1, LastIncidentTick = g.Tick - weeksAgo });
    }

    [Fact]
    public void A_leak_that_runs_costs_the_rival_reputation_and_goodwill_and_the_leaker_the_fee()
    {
        Game g = Desk(tweak: c => c.Press.LeakBaseSuccessPermille = 1000);
        LabState rival = g.Rival("openaye");
        Troubled(g, rival);
        rival.Reputation = 700;
        rival.FeelingToPlayer = 100;
        long fee = PressSystem.LeakFee(g.C, g.Tick);
        Assert.True(fee > 0);
        long cash = g.Me.Cash;
        CommandResult r = g.Do(Command.LeakStory(g.Id, rival.Id));
        Assert.Equal(rival.Id, r.CreatedId);
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Equal(700 - g.C.Press.LeakHitPoints, rival.Reputation);
        Assert.Equal(100 - g.C.Press.LeakFeelingHit, rival.FeelingToPlayer);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.StoryLeaked);
        Assert.Equal(1, e.Pick);
        Assert.Equal(g.C.Press.LeakHitPoints, e.Amount);
        Assert.Equal("Rival Oops", e.Ref);
    }

    [Fact]
    public void A_leak_that_is_traced_costs_the_leaker_reputation_and_the_rival_a_little_goodwill()
    {
        Game g = Desk(tweak: c => { c.Press.MinSuccessPermille = 1; c.Press.LeakBaseSuccessPermille = 0; c.Press.LeakReputationSlopePermille = 0; });
        LabState rival = g.Rival("openaye");
        Troubled(g, rival);
        rival.Reputation = 700;
        rival.FeelingToPlayer = 100;
        g.Me.Reputation = 600;
        long cash = g.Me.Cash;
        long fee = PressSystem.LeakFee(g.C, g.Tick);
        g.Do(Command.LeakStory(g.Id, rival.Id));
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.StoryLeaked);
        Assert.Equal(0, e.Pick);
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Equal(600 - g.C.Press.LeakBackfirePoints, g.Me.Reputation);
        Assert.Equal(700, rival.Reputation);
        Assert.Equal(100 - g.C.Press.LeakFailFeelingHit, rival.FeelingToPlayer);
        Assert.Equal(g.C.Press.LeakBackfirePoints, e.Amount);
    }

    [Fact]
    public void A_traced_leak_hurts_less_for_a_lab_with_a_press_room()
    {
        Action<GameConfig> sure = c => { c.Press.MinSuccessPermille = 1; c.Press.LeakBaseSuccessPermille = 0; c.Press.LeakReputationSlopePermille = 0; c.Press.LeakRoomSlopePermille = 0; };
        Game bare = Desk(tweak: sure);
        Game roomed = Desk(tweak: sure);
        roomed.Build("press-room", 6, 5);
        foreach (Game g in new[] { bare, roomed })
        {
            Troubled(g, g.Rival("openaye"));
            g.Me.Reputation = 600;
            g.Do(Command.LeakStory(g.Id, g.Rival("openaye").Id));
        }
        Assert.Equal(600 - 40, bare.Me.Reputation);
        Assert.Equal(600 - 28, roomed.Me.Reputation);
    }

    [Fact]
    public void A_press_room_makes_a_leak_likelier_to_land()
    {
        Game g = Desk(rivals: false);
        PressSystem.ChanceParts plain = PressSystem.LeakChance(g.C, g.Me);
        Assert.Equal(g.C.Press.LeakBaseSuccessPermille, plain.Total);
        g.Build("press-room", 6, 5);
        PressSystem.ChanceParts roomed = PressSystem.LeakChance(g.C, g.Me);
        Assert.Equal(300 * g.C.Press.LeakRoomSlopePermille / 1000, roomed.RoomPart);
        Assert.Equal(roomed.Base + roomed.RoomPart + roomed.ReputationPart + roomed.Clamp, roomed.Total);
        Assert.True(roomed.Total > plain.Total);
    }

    [Fact]
    public void A_leak_needs_news_so_a_rival_without_a_recent_incident_is_refused()
    {
        Game g = Desk();
        LabState rival = g.Rival("openaye");
        rival.Models.Clear();
        g.Refused(Command.LeakStory(g.Id, rival.Id), CommandError.TargetNotEligible);
        Troubled(g, rival, weeksAgo: g.C.Press.LeakWindowWeeks + 1);
        Assert.False(PressSystem.IsLeakable(g.C, g.State, rival));
        g.Refused(Command.LeakStory(g.Id, rival.Id), CommandError.TargetNotEligible);
        Troubled(g, rival, weeksAgo: g.C.Press.LeakWindowWeeks);
        Assert.True(PressSystem.IsLeakable(g.C, g.State, rival));
        Assert.True(g.Try(Command.LeakStory(g.Id, rival.Id)).Ok);
    }

    [Fact]
    public void A_leak_is_refused_for_yourself_an_unknown_lab_a_bankrupt_or_absorbed_rival_and_an_empty_purse()
    {
        Game g = Desk();
        LabState rival = g.Rival("openaye");
        Troubled(g, rival);
        g.Refused(Command.LeakStory(g.Id, g.Id), CommandError.UnknownLab);
        g.Refused(Command.LeakStory(g.Id, 99999), CommandError.UnknownLab);
        rival.Bankrupt = true;
        g.Refused(Command.LeakStory(g.Id, rival.Id), CommandError.TargetNotEligible);
        rival.Bankrupt = false;
        rival.AbsorbedByLabId = g.Id;
        g.Refused(Command.LeakStory(g.Id, rival.Id), CommandError.TargetNotEligible);
        rival.AbsorbedByLabId = -1;
        g.Me.Cash = 0;
        g.Refused(Command.LeakStory(g.Id, rival.Id), CommandError.NotEnoughCash);
    }

    [Fact]
    public void A_rivals_own_press_room_cuts_what_a_leak_does_to_it()
    {
        Game g = Desk(tweak: c => c.Press.LeakBaseSuccessPermille = 1000);
        LabState rival = g.Rival("openaye");
        Troubled(g, rival);
        rival.Reputation = 700;
        rival.Rooms.Add(new RoomState { Id = 800, TypeId = "press-room", X = 0, Y = 0, Width = 6, Height = 5 });
        g.Do(Command.LeakStory(g.Id, rival.Id));
        Assert.Equal(700 - PressSystem.Damp(g.C, rival, g.C.Press.LeakHitPoints), rival.Reputation);
        Assert.True(rival.Reputation > 700 - g.C.Press.LeakHitPoints);
    }

    [Fact]
    public void A_leak_at_a_rival_pushes_it_toward_a_feud_with_the_leaker()
    {
        Game g = Desk(tweak: c => c.Press.LeakBaseSuccessPermille = 1000);
        LabState rival = g.Rival("openaye");
        Troubled(g, rival);
        rival.FeelingToPlayer = g.C.Rivalry.Feelings.FeudBelow + 50;
        Assert.False(RivalSystem.IsFeud(g.C, rival));
        g.Do(Command.LeakStory(g.Id, rival.Id));
        Assert.True(RivalSystem.IsFeud(g.C, rival));
    }

    // ---- saves, events, the whole game

    [Fact]
    public void Open_questions_survive_a_save_round_trip()
    {
        Game g = Desk(rivals: false);
        Ask(g, "incident", 77, "Mini");
        Ask(g, "launch", 40, "Maxi");
        string digest = g.Sim.Digest();
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(digest, again.Digest());
        Assert.Equal(2, again.Player.PressInquiries.Count);
        Assert.Equal(77, again.Player.PressInquiries[0].Stake);
        Assert.Equal("Maxi", again.Player.PressInquiries[1].Ref);
    }

    [Fact]
    public void A_schema_23_save_loads_with_no_questions_and_plays_on()
    {
        Game g = Desk(rivals: false);
        Ask(g, "incident");
        string old = SaveSchemaPretend.At(g.C, 23, g.Sim.Save());
        Assert.DoesNotContain("pressInquiries", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.True(GameState.CurrentSchema >= 25);
        Assert.Empty(loaded.Player.PressInquiries);
        loaded.StepMany(8);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }

    [Fact]
    public void A_save_with_a_question_about_an_unknown_beat_or_journalist_or_too_many_questions_is_flagged()
    {
        Game g = Desk(rivals: false);
        PressInquiry q = Ask(g, "incident");
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        q.BeatId = "gossip";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("unknown beat"));
        q.BeatId = "incident";
        q.JournalistId = "nobody";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("unknown journalist"));
        q.JournalistId = "nell-hype";
        for (int i = 0; i < g.C.Press.MaxOpenInquiries; i++) g.Me.PressInquiries.Add(new PressInquiry { Id = 500 + i, JournalistId = "nell-hype", BeatId = "incident", Stake = 5 });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("more open questions"));
    }

    [Fact]
    public void Every_press_event_reads_as_a_sentence()
    {
        Game g = Desk(tweak: c => c.Press.LeakBaseSuccessPermille = 1000);
        g.Build("press-room", 6, 5);
        LabState rival = g.Rival("openaye");
        Troubled(g, rival);
        ModelState model = Trained(g);
        Launch(g, model);
        PressInquiry q = g.Me.PressInquiries.First();
        g.Do(Command.AnswerPress(g.Id, q.Id, "honest"));
        g.Do(Command.LeakStory(g.Id, rival.Id));
        foreach (EventKind kind in new[] { EventKind.LaunchBuzz, EventKind.PressInquiry, EventKind.PressAnswered, EventKind.StoryLeaked })
        {
            GameEvent e = g.State.Events.Last(x => x.Kind == kind);
            string text = EventText.Describe(g.C, g.State, e);
            Assert.False(string.IsNullOrWhiteSpace(text), kind.ToString());
            Assert.DoesNotContain("|", text);
        }
        Assert.Contains("Rival Oops", EventText.Describe(g.C, g.State, g.State.Events.Last(x => x.Kind == EventKind.StoryLeaked)));
    }

    [Fact]
    public void A_lab_on_autopilot_never_answers_so_a_scandalous_year_leaves_the_desk_within_its_cap_and_the_save_clean()
    {
        Game g = Desk("chat-boom", c => c.Safety.IncidentChancePermille = 400, rivals: true, seed: 21);
        g.WithStarterLab();
        g.Autopilot(60);
        Assert.True(g.Me.PressInquiries.Count <= g.C.Press.MaxOpenInquiries);
        Assert.All(g.Me.PressInquiries, q => Assert.True(q.ExpiresTick > g.Tick - 1));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }

    [Fact]
    public void A_scandal_with_a_press_room_leaves_the_lab_with_more_appeal_than_without()
    {
        Game bare = Scandal(0);
        Game roomed = Scandal(6);
        bare.Weeks(1);
        roomed.Weeks(1);
        Assert.True(roomed.Me.Reputation > bare.Me.Reputation);
        var sector = bare.C.Sector("consumer-assistant");
        ModelState a = bare.Me.Models.First(m => m.Status == ModelStatus.Released);
        ModelState b = roomed.Me.Models.First(m => m.Status == ModelStatus.Released);
        long bareBoost = DisplayQueries.AppealParts(bare.C, bare.State, sector, bare.State.Sector(sector.Id), bare.Me, a).ReputationBoost;
        long roomedBoost = DisplayQueries.AppealParts(roomed.C, roomed.State, sector, roomed.State.Sector(sector.Id), roomed.Me, b).ReputationBoost;
        Assert.True(roomedBoost > bareBoost, roomedBoost + " vs " + bareBoost);
    }

    // ---- the page

    [Fact]
    public void The_press_page_lists_every_beat_journalist_response_and_room()
    {
        GameConfig c = TestData.Config();
        string page = System.IO.File.ReadAllText(System.IO.Path.Combine(TestData.RepoRoot(), "docs", "content", "press.md"));
        foreach (BeatConfig b in c.Press.Beats) Assert.Contains("`" + b.Id + "`", page);
        foreach (JournalistConfig j in c.Press.Journalists) Assert.Contains("`" + j.Id + "`", page);
        foreach (PressResponseConfig r in c.Press.Responses) Assert.Contains("`" + r.Id + "`", page);
        Assert.Contains("`press-room`", page);
        Assert.Contains("`auditorium`", page);
    }
}
