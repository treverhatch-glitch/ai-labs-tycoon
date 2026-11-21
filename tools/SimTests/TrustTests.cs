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

/// <summary>Sector trust: slow to win, fast to lose, carried between sectors of a family, worth appeal, and the customer desk that opens when it is lost.</summary>
public class TrustTests
{
    const string Chat = "consumer-assistant";   // family web
    const string Ads = "web-ads";               // family web
    const string Support = "customer-support";  // family language
    const string Drive = "autonomous-driving";  // family physical

    static Game Stage(Action<GameConfig> tweak = null, bool rivals = false, long seed = 7)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "chat-boom" };
        if (!rivals) options.RivalIds = new List<string>();
        var g = new Game(Simulation.NewGame(TestData.Config(c =>
        {
            c.RolePerks.Clear();
            c.Gauntlet.JitterPermille = 0;
            if (tweak != null) tweak(c);
        }), options));
        g.Me.Cash = 1_000_000_000_000;
        foreach (LabState lab in g.State.Labs) lab.Models.Clear();
        foreach (SectorState s in g.State.Sectors) s.Entries.Clear();
        return g;
    }

    /// <summary>A released model on sale in the named sectors. Alignment 1000 never has an incident, alignment 0 has one every week once the incident chance is 1000.</summary>
    static ModelState Sold(Game g, LabState lab, int alignment, params string[] sectors)
    {
        var m = new ModelState
        {
            Id = 9000 + lab.Id * 10 + lab.Models.Count,
            LineId = "mini",
            Version = 1,
            Name = lab.Name + " Mk" + lab.Models.Count,
            Status = ModelStatus.Released,
            Capability = 1000,
            Alignment = alignment,
            DisplayedAlignment = alignment,
            ReleasedTick = g.Tick,
            LastIncidentTick = -1
        };
        lab.Models.Add(m);
        foreach (string id in sectors)
        {
            m.Sectors.Add(id);
            g.State.Sector(id).Entries.Add(new SectorEntry { LabId = lab.Id, ModelId = m.Id });
        }
        return m;
    }

    static void Troubled(GameConfig c) { c.Safety.IncidentChancePermille = 1000; }

    static void NoGain(GameConfig c) { c.Trust.GainPerWeek = 0; }

    static void SetTrust(LabState lab, string sector, int trust)
    {
        SectorTrust t = lab.SectorTrust.FirstOrDefault(x => x.SectorId == sector);
        if (t == null) { t = new SectorTrust { SectorId = sector }; lab.SectorTrust.Add(t); }
        t.Trust = trust;
    }

    static int Trust(Game g, LabState lab, string sector) => TrustSystem.Of(g.C, lab, sector);

    static int Count(Game g, EventKind kind) => g.State.Events.Count(e => e.Kind == kind);

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

    /// <summary>One incident in the model's sectors, with trust gain off so the figures are exact.</summary>
    static Game Hit(Action<GameConfig> tweak, params string[] sectors)
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); if (tweak != null) tweak(c); });
        Sold(g, g.Me, 0, sectors);
        g.Weeks(1);
        return g;
    }

    // ---- the table

    [Fact]
    public void The_default_trust_table_is_valid_and_every_sector_belongs_to_a_family()
    {
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        Assert.All(c.Market.Sectors, s => Assert.False(string.IsNullOrEmpty(s.Family), s.Id));
        Assert.Equal(new[] { "refund", "patch", "apology" }, c.Trust.Responses.Select(r => r.Id).ToArray());
        Assert.Equal(500, c.Trust.BaselineTrust);
        Assert.Equal(9, c.Market.Sectors.Select(s => s.Family).Distinct().Count());
        Assert.Equal("web", c.Sector(Chat).Family);
        Assert.Equal("web", c.Sector(Ads).Family);
        Assert.Equal("language", c.Sector(Support).Family);
    }

    [Fact]
    public void Bad_trust_numbers_are_caught_by_the_validator()
    {
        AssertProblem(Problems(c => c.Trust.BaselineTrust = 1001), "baselineTrust");
        AssertProblem(Problems(c => c.Trust.GainPerWeek = -1), "gainPerWeek");
        AssertProblem(Problems(c => c.Trust.IdleDecayPerWeek = -1), "idleDecayPerWeek");
        AssertProblem(Problems(c => c.Trust.IncidentDrop = 1001), "incidentDrop");
        AssertProblem(Problems(c => c.Trust.BracketWinPoints = -1), "bracketWinPoints");
        AssertProblem(Problems(c => c.Trust.ExposedDrop = -1), "exposedDrop");
        AssertProblem(Problems(c => c.Trust.CarryPermille = 1001), "carryPermille");
        AssertProblem(Problems(c => c.Trust.AppealSlopePermille = 1801), "appealSlopePermille");
        AssertProblem(Problems(c => c.Trust.ComplaintWeeks = 0), "complaintWeeks");
        AssertProblem(Problems(c => c.Trust.MaxOpenComplaints = -1), "maxOpenComplaints");
        AssertProblem(Problems(c => c.Trust.MinSuccessPermille = 0), "minSuccessPermille");
        AssertProblem(Problems(c => c.Trust.Responses.Clear()), "responses is empty");
        AssertProblem(Problems(c => c.Trust.Responses[0].Name = ""), "has no name");
        AssertProblem(Problems(c => c.Trust.Responses[1].BaseSuccessPermille = 1001), "baseSuccessPermille");
        AssertProblem(Problems(c => c.Trust.Responses[1].RestorePermille = 2001), "restorePermille");
        AssertProblem(Problems(c => c.Trust.Responses[1].BackfirePermille = -1), "backfirePermille");
        AssertProblem(Problems(c => c.Trust.Responses[1].CostRevenuePermille = 5001), "costRevenuePermille");
        AssertProblem(Problems(c => c.Trust.Responses.Add(new ComplaintResponseConfig { IdValue = "refund", Name = "x", Blurb = "x" })), "\"refund\" twice");
        AssertProblem(Problems(c => c.Market.Sectors[0].Family = ""), "has no family");
    }

    // ---- reading trust and what it does to appeal

    [Fact]
    public void A_sector_with_no_record_sits_at_the_baseline_and_changes_nothing()
    {
        Game g = Stage();
        Assert.Equal(500, Trust(g, g.Me, Chat));
        Assert.Equal(1000, TrustSystem.AppealPermille(g.C, g.Me, Chat));
    }

    [Fact]
    public void Appeal_moves_by_the_slope_times_the_distance_from_the_baseline()
    {
        Game g = Stage();
        SetTrust(g.Me, Chat, 1000);
        Assert.Equal(1150, TrustSystem.AppealPermille(g.C, g.Me, Chat));
        SetTrust(g.Me, Chat, 0);
        Assert.Equal(850, TrustSystem.AppealPermille(g.C, g.Me, Chat));
        SetTrust(g.Me, Chat, 750);
        Assert.Equal(1075, TrustSystem.AppealPermille(g.C, g.Me, Chat));
        g.C.Trust.AppealSlopePermille = 0;
        Assert.Equal(1000, TrustSystem.AppealPermille(g.C, g.Me, Chat));
    }

    [Fact]
    public void The_appeal_slope_at_its_top_limit_never_takes_appeal_to_zero()
    {
        Game g = Stage(c => c.Trust.AppealSlopePermille = 1800);
        SetTrust(g.Me, Chat, 0);
        Assert.Equal(100, TrustSystem.AppealPermille(g.C, g.Me, Chat));
    }

    [Fact]
    public void Trust_shows_up_as_its_own_part_of_the_appeal_and_the_parts_still_add_up()
    {
        Game g = Stage();
        ModelState m = Sold(g, g.Me, 1000, Chat);
        SectorConfig sector = g.C.Sector(Chat);
        SectorState state = g.State.Sector(Chat);
        var neutral = DisplayQueries.AppealParts(g.C, g.State, sector, state, g.Me, m);
        Assert.Equal(0, neutral.TrustBoost);

        SetTrust(g.Me, Chat, 900);
        var high = DisplayQueries.AppealParts(g.C, g.State, sector, state, g.Me, m);
        Assert.True(high.TrustBoost > 0);
        SetTrust(g.Me, Chat, 100);
        var low = DisplayQueries.AppealParts(g.C, g.State, sector, state, g.Me, m);
        Assert.True(low.TrustBoost < 0);
        foreach (var parts in new[] { neutral, high, low })
            Assert.Equal(parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.RoleBoost + parts.TrustBoost + parts.ChampionBoost, parts.Total);
        Assert.True(high.Total > neutral.Total && neutral.Total > low.Total);
    }

    [Fact]
    public void The_bracket_champion_bonus_is_worked_out_on_the_trusted_appeal()
    {
        Game g = Stage();
        ModelState m = Sold(g, g.Me, 1000, Chat);
        SectorConfig sector = g.C.Sector(Chat);
        SectorState state = g.State.Sector(Chat);
        SetTrust(g.Me, Chat, 1000);
        var plain = DisplayQueries.AppealParts(g.C, g.State, sector, state, g.Me, m);
        state.ChampionLabId = g.Id;
        var champion = DisplayQueries.AppealParts(g.C, g.State, sector, state, g.Me, m);
        long beforeCrown = plain.Total;
        Assert.Equal(beforeCrown + beforeCrown * g.C.Gauntlet.BracketWinnerAppealPermille / 1000, champion.Total);
    }

    [Fact]
    public void A_lab_the_customers_trust_wins_a_bigger_share_of_a_shared_sector_a_cross_system_check()
    {
        Game g = Stage(c => NoGain(c), rivals: true);
        LabState rival = g.Rival("openaye");
        ModelState mine = Sold(g, g.Me, 1000, Chat);
        ModelState theirs = Sold(g, rival, 1000, Chat);
        g.Me.Reputation = rival.Reputation;
        SectorConfig sector = g.C.Sector(Chat);
        SectorState state = g.State.Sector(Chat);
        long even = MarketSystem.Appeal(g.C, g.State, sector, state, g.Me, mine);
        Assert.Equal(even, MarketSystem.Appeal(g.C, g.State, sector, state, rival, theirs) * MarketSystem.SalesPermille(g.C, g.Me) / MarketSystem.SalesPermille(g.C, rival));
        SetTrust(g.Me, Chat, 900);
        Assert.True(MarketSystem.Appeal(g.C, g.State, sector, state, g.Me, mine) > even);
    }

    // ---- earning it slowly

    [Fact]
    public void A_lab_with_a_model_on_sale_gains_trust_every_week_up_to_the_ceiling()
    {
        Game g = Stage();
        Sold(g, g.Me, 1000, Chat);
        g.Weeks(10);
        Assert.Equal(500 + 10 * g.C.Trust.GainPerWeek, Trust(g, g.Me, Chat));
        SetTrust(g.Me, Chat, 999);
        g.Weeks(3);
        Assert.Equal(1000, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void A_lab_that_sells_nowhere_has_no_trust_records_at_all()
    {
        Game g = Stage();
        g.Weeks(5);
        Assert.Empty(g.Me.SectorTrust);
    }

    [Fact]
    public void Trust_is_slower_to_win_than_to_lose_with_the_default_numbers()
    {
        TrustConfig t = TestData.Config().Trust;
        Assert.True(t.IncidentDrop >= 20 * t.GainPerWeek, "one incident should undo many weeks of gains");
        Assert.True(t.IncidentDrop > t.BracketWinPoints);
    }

    [Fact]
    public void Trust_in_a_sector_the_lab_has_left_drifts_back_to_the_baseline_and_the_record_goes()
    {
        Game g = Stage();
        SetTrust(g.Me, Chat, 503);
        SetTrust(g.Me, Support, 497);
        g.Weeks(1);
        Assert.Equal(502, Trust(g, g.Me, Chat));
        Assert.Equal(498, Trust(g, g.Me, Support));
        g.Weeks(2);
        Assert.Equal(500, Trust(g, g.Me, Chat));
        Assert.Equal(500, Trust(g, g.Me, Support));
        Assert.Empty(g.Me.SectorTrust);
    }

    [Fact]
    public void Idle_decay_of_zero_leaves_old_trust_where_it_was()
    {
        Game g = Stage(c => c.Trust.IdleDecayPerWeek = 0);
        SetTrust(g.Me, Chat, 900);
        g.Weeks(20);
        Assert.Equal(900, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void A_bankrupt_lab_neither_gains_nor_loses_trust_through_the_weeks()
    {
        Game g = Stage();
        Sold(g, g.Me, 1000, Chat);
        SetTrust(g.Me, Chat, 600);
        g.Me.Bankrupt = true;
        g.Weeks(3);
        Assert.Equal(600, Trust(g, g.Me, Chat));
    }

    // ---- losing it fast

    [Fact]
    public void An_incident_costs_trust_in_every_sector_the_model_is_on_sale_in()
    {
        Game g = Hit(c => c.Safety.SectorFlavours.Clear(), Chat, Support);
        Assert.Equal(1, Count(g, EventKind.Incident));
        Assert.Equal(500 - 60, Trust(g, g.Me, Chat));
        Assert.Equal(500 - 60, Trust(g, g.Me, Support));
        var lost = g.State.Events.Where(e => e.Kind == EventKind.TrustLost).ToList();
        Assert.Equal(2, lost.Count);
        Assert.All(lost, e => Assert.Equal(60, e.Amount));
        Assert.Equal(new[] { Chat, Support }.OrderBy(x => x), lost.Select(e => e.Ref).OrderBy(x => x));
    }

    [Fact]
    public void An_incident_in_a_model_sold_nowhere_costs_no_trust()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); });
        Sold(g, g.Me, 0);
        TrustSystem.OnIncident(g.C, g.State, g.Me, g.Me.Models[0]);
        Assert.Empty(g.Me.SectorTrust);
        Assert.Empty(g.Me.Complaints);
    }

    [Fact]
    public void Part_of_the_loss_carries_to_other_sectors_of_the_same_family_where_the_lab_sells()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); });
        Sold(g, g.Me, 0, Chat);
        Sold(g, g.Me, 1000, Ads);
        Sold(g, g.Me, 1000, Support);
        g.Weeks(1);
        Assert.Equal(440, Trust(g, g.Me, Chat));
        Assert.Equal(500 - 60 * 300 / 1000, Trust(g, g.Me, Ads));
        Assert.Equal(500, Trust(g, g.Me, Support));
    }

    [Fact]
    public void Nothing_carries_to_a_sector_of_the_family_where_the_lab_does_not_sell()
    {
        Game g = Hit(null, Chat);
        Assert.Equal(440, Trust(g, g.Me, Chat));
        Assert.Equal(500, Trust(g, g.Me, Ads));
        Assert.DoesNotContain(g.Me.SectorTrust, t => t.SectorId == Ads);
    }

    [Fact]
    public void No_carry_when_the_share_is_zero()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); c.Trust.CarryPermille = 0; });
        Sold(g, g.Me, 0, Chat);
        Sold(g, g.Me, 1000, Ads);
        g.Weeks(1);
        Assert.Equal(500, Trust(g, g.Me, Ads));
    }

    [Fact]
    public void Trust_cannot_fall_below_zero_and_only_what_was_lost_is_counted()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); });
        SetTrust(g.Me, Chat, 50);
        Sold(g, g.Me, 0, Chat);
        g.Weeks(1);
        Assert.Equal(0, Trust(g, g.Me, Chat));
        Assert.Equal(50, g.State.Events.Single(e => e.Kind == EventKind.TrustLost).Amount);
        Assert.Equal(50, g.Me.Complaints.Single().Stake);
    }

    [Fact]
    public void A_sector_with_no_trust_left_to_lose_writes_no_complaint()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); });
        SetTrust(g.Me, Chat, 0);
        Sold(g, g.Me, 0, Chat);
        g.Weeks(1);
        Assert.Empty(g.Me.Complaints);
        Assert.Equal(0, Count(g, EventKind.CustomerComplaint));
    }

    [Fact]
    public void A_rivals_incident_costs_its_trust_but_logs_nothing_and_writes_no_complaint()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); }, rivals: true);
        LabState rival = g.Rival("openaye");
        Sold(g, rival, 0, Chat);
        g.Weeks(1);
        Assert.Equal(440, Trust(g, rival, Chat));
        Assert.Equal(0, Count(g, EventKind.TrustLost));
        Assert.Empty(rival.Complaints);
        Assert.Empty(g.Me.Complaints);
    }

    [Fact]
    public void An_incident_with_a_zero_drop_changes_nothing_and_asks_nothing()
    {
        Game g = Hit(c => c.Trust.IncidentDrop = 0, Chat);
        Assert.Equal(500, Trust(g, g.Me, Chat));
        Assert.Empty(g.Me.Complaints);
    }

    // ---- the customer desk

    [Fact]
    public void An_incident_writes_a_complaint_whose_stake_is_the_trust_it_cost()
    {
        Game g = Hit(null, Chat);
        Complaint complaint = g.Me.Complaints.Single();
        Assert.Equal(Chat, complaint.SectorId);
        Assert.Equal(60, complaint.Stake);
        Assert.Equal(g.Me.Models[0].Id, complaint.ModelId);
        Assert.Equal(g.Me.Models[0].Name, complaint.Ref);
        Assert.Equal(g.Tick - 1 + g.C.Trust.ComplaintWeeks, complaint.ExpiresTick);
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.CustomerComplaint);
        Assert.Equal(complaint.Id, e.SubjectId);
        Assert.Equal(60, e.Amount);
    }

    [Fact]
    public void One_complaint_per_incident_comes_from_the_sector_where_the_model_earns_most()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); });
        Sold(g, g.Me, 0, Chat, Support, Drive);
        g.State.Sector(Support).Entry(g.Id).LastRevenue = 9_000;
        g.State.Sector(Chat).Entry(g.Id).LastRevenue = 100;
        g.State.Sector(Drive).Entry(g.Id).LastRevenue = 100;
        TrustSystem.OnIncident(g.C, g.State, g.Me, g.Me.Models[0]);
        Assert.Equal(Support, g.Me.Complaints.Single().SectorId);
        Assert.Equal(3, g.State.Events.Count(e => e.Kind == EventKind.TrustLost));
    }

    [Fact]
    public void When_every_sector_earns_the_same_the_first_one_listed_wins_the_tie()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); });
        Sold(g, g.Me, 0, Support, Chat);
        TrustSystem.OnIncident(g.C, g.State, g.Me, g.Me.Models[0]);
        Assert.Equal(Support, g.Me.Complaints.Single().SectorId);
    }

    [Fact]
    public void The_desk_holds_only_so_many_complaints_but_trust_is_lost_all_the_same()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); c.Trust.ComplaintWeeks = 52; });
        Sold(g, g.Me, 0, Chat);
        g.Weeks(5);
        Assert.Equal(g.C.Trust.MaxOpenComplaints, g.Me.Complaints.Count);
        Assert.Equal(5, Count(g, EventKind.Incident));
        Assert.Equal(5, Count(g, EventKind.TrustLost));
        Assert.Equal(500 - 5 * 60, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void A_desk_size_of_zero_means_no_complaints_are_ever_written()
    {
        Game g = Hit(c => c.Trust.MaxOpenComplaints = 0, Chat);
        Assert.Empty(g.Me.Complaints);
        Assert.Equal(440, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void An_unanswered_complaint_lapses_and_nothing_else_changes()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); c.Trust.IdleDecayPerWeek = 0; });
        ModelState m = Sold(g, g.Me, 0, Chat);
        g.Weeks(1);
        Assert.Single(g.Me.Complaints);
        m.Alignment = 1000;
        m.DisplayedAlignment = 1000;
        int trust = Trust(g, g.Me, Chat);
        g.Weeks(g.C.Trust.ComplaintWeeks - 1);
        Assert.Single(g.Me.Complaints);
        g.Weeks(1);
        Assert.Empty(g.Me.Complaints);
        Assert.Equal(trust, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void A_bankrupt_player_gets_no_complaints()
    {
        Game g = Stage(c => { Troubled(c); NoGain(c); });
        Sold(g, g.Me, 0, Chat);
        g.Me.Bankrupt = true;
        TrustSystem.OnIncident(g.C, g.State, g.Me, g.Me.Models[0]);
        Assert.Empty(g.Me.Complaints);
        Assert.Equal(440, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void An_incident_also_asks_the_press_and_the_two_desks_are_independent_a_cross_system_check()
    {
        Game g = Hit(null, Chat);
        Assert.Single(g.Me.Complaints);
        Assert.Contains(g.Me.PressInquiries, q => q.BeatId == "incident");
        g.Do(Command.AnswerComplaint(g.Id, g.Me.Complaints[0].Id, "apology"));
        Assert.Empty(g.Me.Complaints);
        Assert.Contains(g.Me.PressInquiries, q => q.BeatId == "incident");
    }

    // ---- answering

    static Complaint Waiting(Game g, out ModelState model)
    {
        model = Sold(g, g.Me, 0, Chat);
        g.Weeks(1);
        return g.Me.Complaints.Single();
    }

    static Game Desk(Action<GameConfig> tweak = null, long seed = 7)
    {
        return Stage(c => { Troubled(c); NoGain(c); c.Trust.IdleDecayPerWeek = 0; if (tweak != null) tweak(c); }, seed: seed);
    }

    [Fact]
    public void A_refund_costs_fees_and_a_week_of_the_sectors_takings_and_always_wins_back_most_of_the_loss()
    {
        Game g = Desk();
        Complaint complaint = Waiting(g, out ModelState model);
        g.State.Sector(Chat).Entry(g.Id).LastRevenue = 40_000;
        long fee = EraRules.HireFee(g.C, g.C.Role("sales"), g.Tick);
        long expected = fee * 3 + 40_000;
        Assert.Equal(expected, TrustSystem.AnswerCost(g.C, g.State, g.Me, complaint, g.C.Trust.Response("refund")));

        long cash = g.Me.Cash;
        int trust = Trust(g, g.Me, Chat);
        g.Do(Command.AnswerComplaint(g.Id, complaint.Id, "refund"));
        Assert.Equal(cash - expected, g.Me.Cash);
        Assert.Equal(trust + 60 * 700 / 1000, Trust(g, g.Me, Chat));
        Assert.Empty(g.Me.Complaints);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.ComplaintAnswered);
        Assert.Equal(1, e.Pick);
        Assert.Equal(42, e.Amount);
        Assert.Equal("refund|" + Chat, e.Ref);
    }

    [Fact]
    public void An_apology_is_free_and_wins_back_a_little()
    {
        Game g = Desk();
        Complaint complaint = Waiting(g, out _);
        long cash = g.Me.Cash;
        int trust = Trust(g, g.Me, Chat);
        g.Do(Command.AnswerComplaint(g.Id, complaint.Id, "apology"));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(trust + 9, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void A_patch_that_lands_wins_back_trust_and_one_that_does_not_loses_more()
    {
        Game sure = Desk(c => c.Trust.Responses.First(r => r.Id == "patch").BaseSuccessPermille = 1000);
        Complaint a = Waiting(sure, out _);
        int before = Trust(sure, sure.Me, Chat);
        long cash = sure.Me.Cash;
        sure.Do(Command.AnswerComplaint(sure.Id, a.Id, "patch"));
        Assert.Equal(before + 36, Trust(sure, sure.Me, Chat));
        Assert.Equal(cash - EraRules.HireFee(sure.C, sure.C.Role("sales"), sure.Tick) * 4, sure.Me.Cash);

        Game doomed = Desk(c =>
        {
            ComplaintResponseConfig patch = c.Trust.Responses.First(r => r.Id == "patch");
            patch.BaseSuccessPermille = 0;
            patch.ReputationSlopePermille = 0;
            c.Trust.MinSuccessPermille = 1;
        });
        Complaint b = Waiting(doomed, out _);
        int low = Trust(doomed, doomed.Me, Chat);
        doomed.Do(Command.AnswerComplaint(doomed.Id, b.Id, "patch"));
        GameEvent e = doomed.State.Events.Last(x => x.Kind == EventKind.ComplaintAnswered);
        Assert.Equal(0, e.Pick);
        Assert.Equal(-18, e.Amount);
        Assert.Equal(low - 18, Trust(doomed, doomed.Me, Chat));
    }

    [Fact]
    public void The_chance_of_an_answer_follows_reputation_and_stays_between_the_floor_and_certainty()
    {
        Game g = Stage();
        ComplaintResponseConfig patch = g.C.Trust.Response("patch");
        g.Me.Reputation = 500;
        var even = TrustSystem.AnswerChance(g.C, g.Me, patch);
        Assert.Equal(600, even.Base);
        Assert.Equal(0, even.ReputationPart);
        Assert.Equal(600, even.Total);
        g.Me.Reputation = 800;
        Assert.Equal(690, TrustSystem.AnswerChance(g.C, g.Me, patch).Total);
        g.Me.Reputation = 1000;
        g.C.Trust.Response("patch").BaseSuccessPermille = 1000;
        var sure = TrustSystem.AnswerChance(g.C, g.Me, patch);
        Assert.Equal(1000, sure.Total);
        Assert.True(sure.Clamp < 0);
        g.Me.Reputation = 0;
        g.C.Trust.Response("patch").BaseSuccessPermille = 0;
        Assert.Equal(g.C.Trust.MinSuccessPermille, TrustSystem.AnswerChance(g.C, g.Me, patch).Total);
    }

    [Fact]
    public void Answering_restores_trust_in_that_sector_only_and_never_past_the_ceiling()
    {
        Game g = Desk();
        Sold(g, g.Me, 1000, Ads);
        Complaint complaint = Waiting(g, out _);
        int ads = Trust(g, g.Me, Ads);
        SetTrust(g.Me, Chat, 990);
        g.Do(Command.AnswerComplaint(g.Id, complaint.Id, "refund"));
        Assert.Equal(1000, Trust(g, g.Me, Chat));
        Assert.Equal(10, g.State.Events.Last(e => e.Kind == EventKind.ComplaintAnswered).Amount);
        Assert.Equal(ads, Trust(g, g.Me, Ads));
    }

    [Fact]
    public void A_refund_for_a_model_no_longer_on_sale_costs_the_fees_alone()
    {
        Game g = Desk();
        Complaint complaint = Waiting(g, out _);
        g.State.Sector(Chat).Entries.Clear();
        long fee = EraRules.HireFee(g.C, g.C.Role("sales"), g.Tick);
        Assert.Equal(fee * 3, TrustSystem.AnswerCost(g.C, g.State, g.Me, complaint, g.C.Trust.Response("refund")));
    }

    [Fact]
    public void A_lab_with_exactly_the_cost_can_answer_and_one_coin_short_cannot()
    {
        Game g = Desk();
        Complaint complaint = Waiting(g, out _);
        long cost = TrustSystem.AnswerCost(g.C, g.State, g.Me, complaint, g.C.Trust.Response("refund"));
        Assert.True(cost > 0);
        g.Me.Cash = cost - 1;
        g.Refused(Command.AnswerComplaint(g.Id, complaint.Id, "refund"), CommandError.NotEnoughCash);
        Assert.Single(g.Me.Complaints);
        g.Me.Cash = cost;
        g.Do(Command.AnswerComplaint(g.Id, complaint.Id, "refund"));
        Assert.Equal(0, g.Me.Cash);
    }

    [Fact]
    public void Refused_answers_change_nothing()
    {
        Game g = Desk();
        Complaint complaint = Waiting(g, out _);
        g.Refused(Command.AnswerComplaint(g.Id, complaint.Id, "sue"), CommandError.UnknownId);
        g.Refused(Command.AnswerComplaint(g.Id, 424242, "refund"), CommandError.ComplaintGone);
        g.Do(Command.AnswerComplaint(g.Id, complaint.Id, "apology"));
        g.Refused(Command.AnswerComplaint(g.Id, complaint.Id, "apology"), CommandError.ComplaintGone);
    }

    [Fact]
    public void A_complaint_that_has_lapsed_cannot_be_answered()
    {
        Game g = Desk();
        Complaint complaint = Waiting(g, out ModelState m);
        m.Alignment = 1000;
        m.DisplayedAlignment = 1000;
        g.Weeks(g.C.Trust.ComplaintWeeks);
        g.Refused(Command.AnswerComplaint(g.Id, complaint.Id, "apology"), CommandError.ComplaintGone);
    }

    [Fact]
    public void An_answer_that_wins_back_nothing_still_pays_and_clears_the_desk()
    {
        Game g = Desk(c => c.Trust.Responses.First(r => r.Id == "apology").RestorePermille = 0);
        Complaint complaint = Waiting(g, out _);
        int trust = Trust(g, g.Me, Chat);
        g.Do(Command.AnswerComplaint(g.Id, complaint.Id, "apology"));
        Assert.Equal(trust, Trust(g, g.Me, Chat));
        Assert.Empty(g.Me.Complaints);
    }

    // ---- the Gauntlet

    static ModelState Entrant(Game g, LabState lab, int capability, params string[] sectors)
    {
        ModelState m = Sold(g, lab, 1000, sectors);
        m.Capability = capability;
        return m;
    }

    [Fact]
    public void Winning_a_bracket_earns_trust_in_that_sector_and_the_loser_gets_none()
    {
        Game g = Stage(c => { c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; }, rivals: true);
        LabState rival = g.Rival("openaye");
        Entrant(g, g.Me, 1000, Chat, Ads);
        Entrant(g, rival, 500, Chat);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(g.Id, g.State.Sector(Chat).ChampionLabId);
        Assert.Equal(500 + 40 + 12, Trust(g, g.Me, Chat));
        Assert.Equal(500 + 40 + 12, Trust(g, g.Me, Ads));
        Assert.Equal(500, Trust(g, rival, Chat));
    }

    [Fact]
    public void A_bracket_win_carries_part_of_its_trust_to_family_sectors_the_lab_also_sells_in()
    {
        Game g = Stage(c => { c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; });
        Entrant(g, g.Me, 1000, Chat);
        Entrant(g, g.Me, 1000, Ads);
        Entrant(g, g.Me, 1000, Support);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(552, Trust(g, g.Me, Chat));
        Assert.Equal(552, Trust(g, g.Me, Ads));
        Assert.Equal(540, Trust(g, g.Me, Support));
    }

    [Fact]
    public void A_bracket_win_cannot_push_trust_past_the_ceiling()
    {
        Game g = Stage(c => { c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; });
        Entrant(g, g.Me, 1000, Chat);
        SetTrust(g.Me, Chat, 990);
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(1000, Trust(g, g.Me, Chat));
    }

    [Fact]
    public void A_bracket_win_by_a_bankrupt_lab_earns_nothing()
    {
        Game g = Stage(c => { c.Gauntlet.DemoMemeFromPermille = 0; c.Gauntlet.DemoTriumphBelowPermille = 0; });
        Entrant(g, g.Me, 1000, Support);
        g.Me.Bankrupt = true;
        GauntletSystem.RunYear(g.C, g.State);
        Assert.Equal(500, Trust(g, g.Me, Support));
    }

    [Fact]
    public void Being_exposed_costs_trust_in_every_tuned_bracket_and_a_win_that_stands_earns_it()
    {
        int exposed = 0, stood = 0;
        for (long seed = 1; seed <= 60; seed++)
        {
            Game g = Stage(c =>
            {
                c.Gauntlet.ExposeChancePerTuningPermille = 3500;
                c.Gauntlet.DemoMemeFromPermille = 0;
                c.Gauntlet.DemoTriumphBelowPermille = 0;
            }, seed: seed);
            Entrant(g, g.Me, 1000, Support);
            g.Do(Command.SetGauntletTuning(g.Id, 300));
            GauntletSystem.RunYear(g.C, g.State);
            bool wasExposed = g.State.Events.Any(e => e.Kind == EventKind.GauntletExposed);
            if (wasExposed)
            {
                exposed++;
                Assert.Equal(500 - 100 + 40, Trust(g, g.Me, Support));
            }
            else
            {
                stood++;
                Assert.Equal(540, Trust(g, g.Me, Support));
            }
        }
        Assert.True(exposed > 0 && stood > 0);
    }

    [Fact]
    public void A_year_of_play_with_a_win_leaves_the_lab_with_more_trust_and_so_more_appeal_end_to_end()
    {
        Game g = Game.Solo(seed: 3).WithStarterLab(gpus: 8);
        ModelState mini = g.Train("mini", 300);
        g.UntilTrained(mini);
        g.Release(mini, "game-ai");
        int before = TrustSystem.Of(g.C, g.Me, "game-ai");
        g.Until(() => g.State.Gauntlets.Count == 1, 60, "the first Gauntlet");
        Assert.True(TrustSystem.Of(g.C, g.Me, "game-ai") > before + g.C.Trust.BracketWinPoints);
        Assert.True(TrustSystem.AppealPermille(g.C, g.Me, "game-ai") > 1000);
    }

    // ---- save and load

    [Fact]
    public void Trust_and_complaints_save_and_load_to_the_same_digest()
    {
        Game g = Desk();
        Waiting(g, out _);
        SetTrust(g.Me, Support, 640);
        string digest = g.Sim.Digest();
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(digest, again.Digest());
        Assert.Equal(g.Me.SectorTrust.Count, again.Player.SectorTrust.Count);
        Assert.Equal(640, TrustSystem.Of(g.C, again.Player, Support));
        Assert.Equal(g.Me.Complaints[0].Stake, again.Player.Complaints[0].Stake);
        Assert.Equal(g.Me.Complaints[0].Ref, again.Player.Complaints[0].Ref);
    }

    [Fact]
    public void A_schema_26_save_loads_with_baseline_trust_and_no_complaints_and_plays_on()
    {
        Game g = Desk();
        Waiting(g, out _);
        string old = SaveSchemaPretend.At(g.C, 26, g.Sim.Save());
        Assert.DoesNotContain("sectorTrust", old);
        Assert.DoesNotContain("complaints", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.True(GameState.CurrentSchema >= 27);
        Assert.Empty(loaded.Player.SectorTrust);
        Assert.Empty(loaded.Player.Complaints);
        Assert.Equal(500, TrustSystem.Of(loaded.Config, loaded.Player, Chat));
        loaded.StepMany(8);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }

    [Fact]
    public void A_schema_19_save_still_migrates_through_the_whole_chain()
    {
        Game g = Stage();
        string old = SaveSchemaPretend.At(g.C, 19, g.Sim.Save());
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.Empty(loaded.Player.SectorTrust);
        loaded.StepMany(4);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }

    [Fact]
    public void A_save_with_bad_trust_or_complaints_is_flagged()
    {
        Game g = Stage();
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        g.Me.SectorTrust.Add(new SectorTrust { SectorId = "atlantis", Trust = 500 });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("unknown sector \"atlantis\""));
        g.Me.SectorTrust.Clear();
        g.Me.SectorTrust.Add(new SectorTrust { SectorId = Chat, Trust = 1001 });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("outside 0 to 1000"));
        g.Me.SectorTrust[0].Trust = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("outside 0 to 1000"));
        g.Me.SectorTrust[0].Trust = 500;
        g.Me.SectorTrust.Add(new SectorTrust { SectorId = Chat, Trust = 500 });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("two trust records"));
        g.Me.SectorTrust.Clear();
        g.Me.Complaints.Add(new Complaint { Id = 1, SectorId = "atlantis", Stake = 5 });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("complaint from the unknown sector"));
        g.Me.Complaints.Clear();
        for (int i = 0; i <= g.C.Trust.MaxOpenComplaints; i++) g.Me.Complaints.Add(new Complaint { Id = 10 + i, SectorId = Chat, Stake = 5 });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("more open complaints"));
    }

    // ---- reading it

    [Fact]
    public void Every_trust_event_reads_as_a_sentence()
    {
        Game g = Desk(c => c.Trust.Responses.First(r => r.Id == "patch").BaseSuccessPermille = 1000);
        Complaint complaint = Waiting(g, out _);
        g.Do(Command.AnswerComplaint(g.Id, complaint.Id, "patch"));
        var events = new List<GameEvent>(g.State.Events);
        events.Add(new GameEvent { Kind = EventKind.ComplaintAnswered, LabId = g.Id, SubjectId = 1, Amount = -36, Pick = 0, Ref = "patch|" + Chat });
        foreach (EventKind kind in new[] { EventKind.TrustLost, EventKind.CustomerComplaint, EventKind.ComplaintAnswered })
        {
            Assert.Contains(events, e => e.Kind == kind);
            foreach (GameEvent e in events.Where(x => x.Kind == kind))
            {
                string text = EventText.Describe(g.C, g.State, e);
                Assert.False(string.IsNullOrWhiteSpace(text), kind.ToString());
                Assert.DoesNotContain("{", text);
            }
        }
        Assert.Contains("went down badly", EventText.Describe(g.C, g.State, events.Last()));
    }

    [Fact]
    public void Long_runs_keep_every_trust_inside_its_bounds_and_save_cleanly()
    {
        Game g = Game.New(seed: 5);
        g.Autopilot(4 * g.C.Calendar.WeeksPerYear);
        foreach (LabState lab in g.State.Labs)
        {
            Assert.All(lab.SectorTrust, t => Assert.InRange(t.Trust, 0, 1000));
            Assert.True(lab.Complaints.Count <= g.C.Trust.MaxOpenComplaints);
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Contains(g.State.Labs, l => l.SectorTrust.Count > 0);
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
    }
}
