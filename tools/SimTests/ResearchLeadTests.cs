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

/// <summary>Research leads: a run that comes out far better than aimed for leaves a mystery that can be followed for research, a lasting speed-up, or nothing.</summary>
public class ResearchLeadTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(tweak: c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.RolePerks.Clear();
            c.RoomPerks.Clear();
            if (tweak != null) tweak(c);
        });
        g.Me.Cash = 1_000_000_000_000_000;
        return g;
    }

    static Game Staffed(Action<GameConfig> tweak = null)
    {
        Game g = Lab(tweak);
        g.WithStarterLab(researchers: 3);
        return g;
    }

    static ModelState Model(Game g)
    {
        var m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Name = "Fable Mini " + (g.Me.Models.Count + 1), Status = ModelStatus.Trained };
        g.Me.Models.Add(m);
        return m;
    }

    static ResearchLead Found(Game g, int luck = 70)
    {
        ModelState m = Model(g);
        ResearchLeads.OnTrained(g.C, g.State, g.Me, m, luck);
        return g.Me.Leads.Last();
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
    public void The_table_has_five_outcomes_and_validates()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(new[] { "real-insight", "training-trick", "data-quirk", "the-cat", "tuesday" }, c.Leads.Outcomes.Select(o => o.IdValue).ToArray());
        Assert.Equal(60, c.Leads.MinLuckPermille);
        Assert.True(c.Leads.MinLuckPermille <= c.Training.LuckPermille);
        Assert.Empty(ConfigValidator.Problems(c));
    }

    // ---- finding a lead

    [Fact]
    public void A_run_that_lands_well_above_its_aim_leaves_a_lead_on_the_desk()
    {
        Game g = Lab();
        ModelState m = Model(g);
        ResearchLeads.OnTrained(g.C, g.State, g.Me, m, 60);
        ResearchLead lead = Assert.Single(g.Me.Leads);
        Assert.Equal(m.Id, lead.ModelId);
        Assert.Equal(60, lead.Luck);
        Assert.Equal(m.Name, lead.Ref);
        Assert.Equal(g.State.Tick + g.C.Leads.ExpireWeeks, lead.ExpiresTick);
        Assert.False(lead.Following);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.LeadFound);
        Assert.Equal(m.Id, e.SubjectId);
        Assert.Equal(60, e.Amount);
    }

    [Fact]
    public void A_run_that_is_only_a_little_lucky_leaves_nothing()
    {
        Game g = Lab();
        ResearchLeads.OnTrained(g.C, g.State, g.Me, Model(g), 59);
        Assert.Empty(g.Me.Leads);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.LeadFound);
    }

    [Fact]
    public void A_full_desk_finds_no_more()
    {
        Game g = Lab();
        for (int i = 0; i < g.C.Leads.MaxOpen + 2; i++) Found(g);
        Assert.Equal(g.C.Leads.MaxOpen, g.Me.Leads.Count);
    }

    [Fact]
    public void Rivals_never_find_leads()
    {
        Game g = Game.New(seed: 3);
        LabState rival = g.State.Labs.First(l => l.Controller != LabController.Player);
        var m = new ModelState { Id = g.State.TakeId(), LineId = "mini", Name = "Rival 1", Status = ModelStatus.Trained };
        rival.Models.Add(m);
        ResearchLeads.OnTrained(g.C, g.State, rival, m, 80);
        Assert.Empty(rival.Leads);
    }

    [Fact]
    public void A_real_training_run_with_the_bar_at_the_floor_leaves_a_lead_when_it_finishes()
    {
        int withLead = 0;
        for (int seed = 1; seed <= 12; seed++)
        {
            Game g = Game.Solo(seed, c => c.Leads.MinLuckPermille = 1).WithStarterLab(researchers: 3);
            g.Me.Cash = 1_000_000_000_000_000;
            ModelState m = g.Train("mini", 300);
            g.UntilTrained(m);
            ResearchLead lead = g.Me.Leads.FirstOrDefault(l => l.ModelId == m.Id);
            if (lead == null) { Assert.True(m.CapLuck <= 0); continue; }
            withLead++;
            Assert.True(m.CapLuck >= 0);
            Assert.Equal(m.Name, lead.Ref);
        }
        Assert.InRange(withLead, 1, 12);
    }

    // ---- going cold

    [Fact]
    public void An_unfollowed_lead_goes_cold_after_the_wait_and_says_so()
    {
        Game g = Lab();
        ResearchLead lead = Found(g);
        g.Weeks(g.C.Leads.ExpireWeeks - 1);
        Assert.Single(g.Me.Leads);
        g.Weeks(1);
        Assert.Empty(g.Me.Leads);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LeadWentCold && e.SubjectId == lead.Id);
    }

    // ---- following a lead

    [Fact]
    public void Following_a_lead_costs_weeks_of_research_and_starts_the_clock()
    {
        Game g = Staffed();
        g.Weeks(10);
        ResearchLead lead = Found(g);
        long cost = ResearchLeads.FollowCost(g.C, g.Me, g.State.Tick);
        Assert.True(cost >= g.C.Leads.MinCost);
        Assert.Equal(Math.Max(g.C.Leads.MinCost, g.C.Leads.FollowCostWeeks * TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick)), cost);
        g.Me.ResearchPoints = cost + 50;
        g.Do(Command.FollowLead(g.Id, lead.Id));
        Assert.Equal(50, g.Me.ResearchPoints);
        Assert.True(lead.Following);
        Assert.Equal(g.State.Tick + g.C.Leads.FollowWeeks, lead.DoneTick);
    }

    [Fact]
    public void The_cost_is_never_below_the_floor_even_for_a_lab_with_no_researchers()
    {
        Game g = Lab();
        Assert.Equal(g.C.Leads.MinCost, ResearchLeads.FollowCost(g.C, g.Me, g.State.Tick));
    }

    [Fact]
    public void Following_is_refused_without_the_points_or_for_a_lead_that_is_not_there_or_a_second_one()
    {
        Game g = Lab();
        ResearchLead one = Found(g);
        ResearchLead two = Found(g);
        g.Me.ResearchPoints = 0;
        g.Refused(Command.FollowLead(g.Id, one.Id), CommandError.NotEnoughResearch);
        g.Refused(Command.FollowLead(g.Id, 99999), CommandError.LeadGone);
        g.Me.ResearchPoints = 10_000;
        g.Do(Command.FollowLead(g.Id, one.Id));
        g.Refused(Command.FollowLead(g.Id, one.Id), CommandError.InvalidArgument);
        g.Refused(Command.FollowLead(g.Id, two.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_lead_being_followed_does_not_go_cold()
    {
        Game g = Lab(c => { c.Leads.FollowWeeks = 100; c.Leads.ExpireWeeks = 5; });
        ResearchLead lead = Found(g);
        g.Me.ResearchPoints = 10_000;
        g.Do(Command.FollowLead(g.Id, lead.Id));
        g.Weeks(20);
        Assert.Single(g.Me.Leads);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.LeadWentCold);
    }

    // ---- what comes of it

    [Fact]
    public void The_same_lead_always_ends_the_same_way_and_every_outcome_can_happen()
    {
        GameConfig c = TestData.Config();
        var seen = new HashSet<string>();
        for (int id = 1; id <= 400; id++)
        {
            var lead = new ResearchLead { Id = id * 7, ModelId = id * 3, FoundTick = id * 11, Luck = 60 + id % 20 };
            LeadOutcomeConfig first = ResearchLeads.OutcomeFor(c, lead);
            Assert.Same(first, ResearchLeads.OutcomeFor(c, lead));
            seen.Add(first.IdValue);
        }
        Assert.Equal(c.Leads.Outcomes.Count, seen.Count);
    }

    [Fact]
    public void Outcomes_come_in_roughly_the_weights_the_data_gives()
    {
        GameConfig c = TestData.Config();
        int insight = 0;
        for (int id = 1; id <= 2000; id++)
            if (ResearchLeads.OutcomeFor(c, new ResearchLead { Id = id * 13, ModelId = id, FoundTick = id * 5, Luck = 70 }).IdValue == "real-insight") insight++;
        Assert.InRange(insight, 2000 * 4 / 11 - 300, 2000 * 4 / 11 + 300);
    }

    static Game Resolved(string outcome, out ResearchLead lead, out long perWeek)
    {
        Game g = Staffed(c =>
        {
            foreach (LeadOutcomeConfig o in c.Leads.Outcomes) o.Weight = o.IdValue == outcome ? 10 : 1;
            c.Leads.FollowWeeks = 2;
        });
        g.Weeks(5);
        lead = null;
        for (int tries = 0; tries < 200; tries++)
        {
            lead = Found(g);
            if (ResearchLeads.OutcomeFor(g.C, lead).IdValue == outcome) break;
            g.Me.Leads.Remove(lead);
            lead = null;
        }
        Assert.NotNull(lead);
        g.Me.ResearchPoints = 1_000_000;
        g.Do(Command.FollowLead(g.Id, lead.Id));
        perWeek = 0;
        return g;
    }

    [Fact]
    public void A_real_insight_pays_weeks_of_research()
    {
        Game g = Resolved("real-insight", out ResearchLead lead, out long ignored);
        long before = g.Me.ResearchPoints;
        g.Weeks(2);
        Assert.Empty(g.Me.Leads);
        GameEvent done = g.State.Events.Last(e => e.Kind == EventKind.LeadResolved);
        Assert.Equal("real-insight", done.Ref);
        Assert.Equal(1, done.Pick);
        Assert.Equal(20 * TrainingSystem.ResearchPerWeek(g.C, g.Me, done.Tick), done.Amount);
        Assert.True(g.Me.ResearchPoints > before);
    }

    [Fact]
    public void A_training_trick_adds_lasting_speed_up_to_the_cap()
    {
        Game g = Resolved("training-trick", out ResearchLead lead, out long ignored);
        int plain = TrainingSystem.SpeedPermille(g.C, g.Me);
        g.Weeks(2);
        Assert.Equal(30, g.Me.LeadSpeedPermille);
        Assert.Equal(plain + 30, TrainingSystem.SpeedPermille(g.C, g.Me));
        g.Me.LeadSpeedPermille = 140;
        ResearchLead again = Found(g);
        while (ResearchLeads.OutcomeFor(g.C, again).IdValue != "training-trick") { g.Me.Leads.Remove(again); again = Found(g); }
        g.Me.ResearchPoints = 1_000_000;
        g.Do(Command.FollowLead(g.Id, again.Id));
        g.Weeks(2);
        Assert.Equal(150, g.Me.LeadSpeedPermille);
        Assert.Equal(10, g.State.Events.Last(e => e.Kind == EventKind.LeadResolved).Amount);
    }

    [Fact]
    public void The_cat_gives_nothing_and_says_so()
    {
        Game g = Resolved("the-cat", out ResearchLead lead, out long ignored);
        long points = g.Me.ResearchPoints;
        int speed = g.Me.LeadSpeedPermille;
        g.Weeks(2);
        GameEvent done = g.State.Events.Last(e => e.Kind == EventKind.LeadResolved);
        Assert.Equal("the-cat", done.Ref);
        Assert.Equal(0, done.Pick);
        Assert.Equal(0, done.Amount);
        Assert.Equal(speed, g.Me.LeadSpeedPermille);
        Assert.Contains("office cat", EventText.Describe(g.C, g.State, done));
    }

    [Fact]
    public void A_training_speed_that_was_learned_survives_and_applies_to_every_run()
    {
        Game g = Staffed();
        Game plain = Staffed();
        g.Me.LeadSpeedPermille = 90;
        Assert.Equal(TrainingSystem.SpeedPermille(plain.C, plain.Me) + 90, TrainingSystem.SpeedPermille(g.C, g.Me));
    }

    // ---- the desk and the words

    [Fact]
    public void The_report_lists_each_lead_with_what_it_would_cost_and_how_long_is_left()
    {
        Game g = Staffed();
        g.Weeks(10);
        ResearchLead one = Found(g);
        ResearchLead two = Found(g);
        g.Me.ResearchPoints = 100_000;
        g.Do(Command.FollowLead(g.Id, two.Id));
        List<ResearchLeads.Row> rows = ResearchLeads.Rows(g.C, g.Me, g.State.Tick);
        Assert.Equal(2, rows.Count);
        Assert.False(rows[0].Following);
        Assert.Equal(g.C.Leads.ExpireWeeks, rows[0].WeeksLeft);
        Assert.Equal(ResearchLeads.FollowCost(g.C, g.Me, g.State.Tick), rows[0].FollowCost);
        Assert.True(rows[1].Following);
        Assert.Equal(g.C.Leads.FollowWeeks, rows[1].WeeksLeft);
        Assert.Equal(0, rows[1].FollowCost);
        Assert.Equal(one.Id, rows[0].LeadId);
    }

    [Fact]
    public void The_log_and_the_chat_put_a_lead_in_plain_words()
    {
        Game g = Staffed();
        ResearchLead lead = Found(g);
        GameEvent found = g.State.Events.Last(e => e.Kind == EventKind.LeadFound);
        Assert.Contains("better than anyone aimed for", EventText.Describe(g.C, g.State, found));
        Assert.Contains(lead.Ref, CompanyChat.Feed(g.C, g.State, g.Me).Last(m => m.Kind == EventKind.LeadFound).Text);
        g.Weeks(g.C.Leads.ExpireWeeks);
        GameEvent cold = g.State.Events.Last(e => e.Kind == EventKind.LeadWentCold);
        Assert.Contains("has gone cold", EventText.Describe(g.C, g.State, cold));
    }

    [Fact]
    public void The_chat_has_a_line_for_a_good_ending_and_one_for_nothing()
    {
        Game g = Resolved("real-insight", out ResearchLead lead, out long ignored);
        g.Weeks(2);
        Assert.Contains("writing it down", CompanyChat.Feed(g.C, g.State, g.Me).Last(m => m.Kind == EventKind.LeadResolved).Text);
        Game h = Resolved("tuesday", out ResearchLead lead2, out long ignored2);
        h.Weeks(2);
        Assert.Contains("vibes", CompanyChat.Feed(h.C, h.State, h.Me).Last(m => m.Kind == EventKind.LeadResolved).Text);
    }

    // ---- saves, determinism, validation

    [Fact]
    public void A_lab_with_leads_and_a_lesson_saves_loads_and_plays_on_the_same()
    {
        Game g = Staffed();
        g.Weeks(5);
        ResearchLead one = Found(g);
        Found(g);
        g.Me.ResearchPoints = 100_000;
        g.Do(Command.FollowLead(g.Id, one.Id));
        g.Me.LeadSpeedPermille = 60;
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        SaveSchemaPretend.AssertRoundTrip(g.Sim);
        Simulation again = Simulation.Load(g.C, g.Sim.Save());
        g.Weeks(60);
        again.StepMany(60);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_leads_and_no_lesson()
    {
        Game g = Staffed();
        g.Weeks(5);
        string old = SaveSchemaPretend.At(g.C, 31, g.Sim.Save());
        Assert.DoesNotContain("researchLeads", old);
        Assert.DoesNotContain("leadSpeedPermille", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        Assert.All(loaded.State.Labs, l => { Assert.Empty(l.Leads); Assert.Equal(0, l.LeadSpeedPermille); });
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void SaveGame_catches_a_lead_about_a_missing_model_and_a_speed_out_of_range()
    {
        Game g = Staffed();
        ResearchLead lead = Found(g);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        lead.ModelId = 99999;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("missing model"));
        lead.ModelId = g.Me.Models[0].Id;
        g.Me.LeadSpeedPermille = g.C.Leads.MaxSpeedPermille + 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("lead speed"));
    }

    [Fact]
    public void The_validator_catches_a_broken_table()
    {
        AssertProblem(Problems(c => c.Leads.MinLuckPermille = 0), "leads.minLuckPermille");
        AssertProblem(Problems(c => c.Leads.MinLuckPermille = c.Training.LuckPermille + 1), "no run could ever leave a lead");
        AssertProblem(Problems(c => c.Leads.MaxOpen = 0), "leads.maxOpen");
        AssertProblem(Problems(c => c.Leads.ExpireWeeks = 0), "leads.expireWeeks");
        AssertProblem(Problems(c => c.Leads.FollowWeeks = 0), "leads.followWeeks");
        AssertProblem(Problems(c => c.Leads.MaxSpeedPermille = -1), "leads.maxSpeedPermille");
        AssertProblem(Problems(c => c.Leads.Outcomes.Clear()), "leads.outcomes is empty");
        AssertProblem(Problems(c => c.Leads.Outcomes[0].Weight = 0), "needs a weight");
        AssertProblem(Problems(c => c.Leads.Outcomes[0].Amount = 0), "does something but gives nothing");
        AssertProblem(Problems(c => c.Leads.Outcomes[3].Amount = 5), "does nothing but has an amount");
    }
}
