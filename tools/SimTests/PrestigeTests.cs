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

/// <summary>Published papers: research points buy reputation and a prestige that fades, which lifts morale, cheapens hiring and helps counter-offers, and tempts rivals.</summary>
public class PrestigeTests
{
    static Game Lab(Action<GameConfig> tweak = null, int researchers = 4)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.RoomPerks.Clear();
            c.RolePerks.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: researchers, engineers: 2, gpus: 10);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        foreach (StaffMember p in g.Me.Staff) { p.TraitId = "steady"; p.Morale = 500; }
        return g;
    }

    static List<GameEvent> Papers(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.PaperPublished).ToList(); }

    // ---- nothing happens by itself

    [Fact]
    public void No_paper_is_published_and_nothing_changes_until_the_player_asks()
    {
        Game g = Lab();
        g.Weeks(30);
        Assert.Equal(0, g.Me.Prestige);
        Assert.Equal(0, Prestige.MoraleOffset(g.C, g.Me));
        Assert.Equal(0, Prestige.HireDiscountPermille(g.C, g.Me));
        Assert.Equal(0, Prestige.PoachPenaltyPermille(g.C, g.Me));
        Assert.Equal(0, Prestige.CounterBonusPermille(g.C, g.Me));
        Assert.Empty(Papers(g));
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.Prestige));
    }

    // ---- the command

    [Fact]
    public void A_paper_costs_research_points_adds_prestige_and_reputation_and_is_logged()
    {
        Game g = Lab();
        g.Me.Reputation = 300;
        long cost = Prestige.PaperCost(g.C, g.State, g.Me);
        long points = g.Me.ResearchPoints;
        g.Do(Command.PublishPaper(g.Id));
        Assert.Equal(points - cost, g.Me.ResearchPoints);
        Assert.Equal(200, g.Me.Prestige);
        Assert.Equal(308, g.Me.Reputation);
        GameEvent e = Papers(g).Single();
        Assert.Equal(200, e.SubjectId);
        Assert.Equal(cost, e.Amount);
        Assert.Contains("prestige is now 200", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_paper_costs_the_data_s_weeks_of_research_and_never_less_than_the_floor()
    {
        Game g = Lab();
        long weekly = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.True(weekly * 3 > 60);
        Assert.Equal(weekly * 3, Prestige.PaperCost(g.C, g.State, g.Me));
        Game small = Lab(c => c.Prestige.PaperResearchWeeks = 0);
        Assert.Equal(60, Prestige.PaperCost(small.C, small.State, small.Me));
    }

    [Fact]
    public void Not_enough_research_points_is_refused_and_nothing_changes()
    {
        Game g = Lab();
        g.Me.ResearchPoints = Prestige.PaperCost(g.C, g.State, g.Me) - 1;
        long points = g.Me.ResearchPoints;
        g.Refused(Command.PublishPaper(g.Id), CommandError.NotEnoughResearch);
        Assert.Equal(points, g.Me.ResearchPoints);
        Assert.Equal(0, g.Me.Prestige);
        Assert.Empty(Papers(g));
    }

    [Fact]
    public void Too_few_researchers_is_refused()
    {
        Game g = Lab(researchers: 1);
        g.Refused(Command.PublishPaper(g.Id), CommandError.NotEnoughStaff);
        Game two = Lab(researchers: 2);
        two.Do(Command.PublishPaper(two.Id));
    }

    [Fact]
    public void A_lab_already_at_the_most_cannot_publish_more()
    {
        Game g = Lab();
        for (int i = 0; i < 5; i++) g.Do(Command.PublishPaper(g.Id));
        Assert.Equal(1000, g.Me.Prestige);
        long points = g.Me.ResearchPoints;
        g.Refused(Command.PublishPaper(g.Id), CommandError.InvalidArgument);
        Assert.Equal(points, g.Me.ResearchPoints);
        Assert.Equal(5, Papers(g).Count);
    }

    [Fact]
    public void A_paper_that_would_pass_the_most_stops_at_it_and_reputation_stops_at_full()
    {
        Game g = Lab();
        g.Me.Prestige = 900;
        g.Me.Reputation = 998;
        g.Do(Command.PublishPaper(g.Id));
        Assert.Equal(1000, g.Me.Prestige);
        Assert.Equal(1000, g.Me.Reputation);
    }

    [Fact]
    public void A_rival_and_a_game_without_papers_refuse_the_command()
    {
        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.PublishPaper(rival.Id), CommandError.InvalidArgument);
        Assert.Equal(0, rival.Prestige);
        Game off = Lab(c => c.Prestige.PaperPrestige = 0);
        off.Refused(Command.PublishPaper(off.Id), CommandError.InvalidArgument);
        Game none = Lab(c => { c.Prestige.Max = 0; c.Prestige.PaperPrestige = 0; c.Prestige.DecayPerWeek = 0; });
        none.Refused(Command.PublishPaper(none.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_publish()
    {
        Game g = Lab();
        g.Me.Bankrupt = true;
        g.Refused(Command.PublishPaper(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void A_lab_with_no_cash_can_still_publish_because_a_paper_costs_only_research()
    {
        Game g = Lab();
        g.Me.Cash = 0;
        g.Do(Command.PublishPaper(g.Id));
        Assert.Equal(0, g.Me.Cash);
        Assert.Equal(200, g.Me.Prestige);
    }

    // ---- fading

    [Fact]
    public void Prestige_fades_by_the_data_s_amount_every_week_down_to_zero()
    {
        Game g = Lab();
        g.Do(Command.PublishPaper(g.Id));
        g.Weeks(10);
        Assert.Equal(160, g.Me.Prestige);
        g.Weeks(60);
        Assert.Equal(0, g.Me.Prestige);
        g.Weeks(5);
        Assert.Equal(0, g.Me.Prestige);
    }

    [Fact]
    public void Papers_add_up_until_the_most()
    {
        Game g = Lab();
        g.Do(Command.PublishPaper(g.Id));
        g.Do(Command.PublishPaper(g.Id));
        Assert.Equal(400, g.Me.Prestige);
        g.Weeks(5);
        Assert.Equal(380, g.Me.Prestige);
    }

    // ---- what it does

    [Fact]
    public void The_effects_grow_in_step_with_prestige_to_the_data_s_values_at_the_most()
    {
        Game g = Lab();
        g.Me.Prestige = 500;
        Assert.Equal(30, Prestige.MoraleOffset(g.C, g.Me));
        Assert.Equal(100, Prestige.HireDiscountPermille(g.C, g.Me));
        Assert.Equal(100, Prestige.PoachPenaltyPermille(g.C, g.Me));
        Assert.Equal(50, Prestige.CounterBonusPermille(g.C, g.Me));
        g.Me.Prestige = 1000;
        Assert.Equal(60, Prestige.MoraleOffset(g.C, g.Me));
        Assert.Equal(200, Prestige.HireDiscountPermille(g.C, g.Me));
        Assert.Equal(200, Prestige.PoachPenaltyPermille(g.C, g.Me));
        Assert.Equal(100, Prestige.CounterBonusPermille(g.C, g.Me));
    }

    [Fact]
    public void Morale_settles_higher_while_prestige_lasts_and_people_drift_there()
    {
        Game g = Lab(c => c.StaffPeople.MoraleDriftPerWeek = 50);
        int resting = Workplace.RestingMorale(g.C, g.Me);
        g.Me.Prestige = 1000;
        Assert.Equal(resting + 60, Workplace.RestingMorale(g.C, g.Me));
        g.Weeks(3);
        Assert.All(g.Me.Staff, p => Assert.True(p.Morale > resting));
    }

    [Fact]
    public void Hiring_costs_less_while_prestige_lasts_and_the_fee_breakdown_shows_it()
    {
        Game g = Lab();
        RoleConfig role = g.C.Role("researcher");
        long plain = RolePerks.HireFee(g.C, g.Me, role, g.Tick);
        g.Me.Prestige = 1000;
        g.Build("research-lab", 4, 4);
        long fee = RolePerks.HireFee(g.C, g.Me, role, g.Tick);
        Assert.Equal(MathI.MulPermille(plain, 800), fee);
        RolePerks.HireFeeBreakdown parts = RolePerks.HireFeeParts(g.C, g.Me, role, g.Tick);
        Assert.Equal(fee, parts.Total);
        Assert.Equal(fee - plain, parts.PerkDiscount);
        long cash = g.Me.Cash;
        g.Do(Command.Hire(g.Id, "researcher", 1));
        Assert.Equal(cash - fee, g.Me.Cash);
    }

    [Fact]
    public void Rivals_find_the_lab_s_people_more_tempting_while_prestige_lasts()
    {
        Game g = Lab();
        int plain = Workplace.PoachChancePermille(g.C, g.Me);
        g.Me.Prestige = 1000;
        Assert.Equal(MathI.MulPermille(g.C.StaffPeople.PoachChancePermille, 1200), Workplace.PoachChancePermille(g.C, g.Me));
        Assert.True(Workplace.PoachChancePermille(g.C, g.Me) > plain);
    }

    [Fact]
    public void Prestige_and_pay_and_rooms_add_in_the_same_limits()
    {
        Game g = Lab();
        g.Me.PayLevel = -1;
        g.Me.Prestige = 1000;
        Assert.Equal(-350, Workplace.PoachDefencePermille(g.C, g.Me));
        g.C.StaffPeople.PayPoachStepPermille = 300;
        g.Me.PayLevel = -1;
        g.C.Prestige.PoachAtMax = 500;
        Assert.Equal(-500, Workplace.PoachDefencePermille(g.C, g.Me));
    }

    [Fact]
    public void Counter_offers_work_better_by_the_data_s_points_and_stay_inside_the_limits()
    {
        Game g = Lab();
        CounterConfig mission = g.C.Talent.Counter("mission");
        StaffMember star = g.Me.Staff[0];
        int plain = TalentSystem.CounterChance(g.C, g.Me, star, mission).Total;
        g.Me.Prestige = 1000;
        int better = TalentSystem.CounterChance(g.C, g.Me, star, mission).Total;
        Assert.True(better >= plain);
        Assert.InRange(better, g.C.Talent.MinSuccessPermille, 1000);
        Assert.Equal(mission.BaseSuccessPermille + 100, TalentSystem.CounterChance(g.C, g.Me, star, mission).Base);
    }

    [Fact]
    public void A_bigger_research_team_makes_a_paper_cost_more_points()
    {
        Game g = Lab();
        long before = Prestige.PaperCost(g.C, g.State, g.Me);
        g.Build("research-lab", 4, 4);
        g.Do(Command.Hire(g.Id, "researcher", 2));
        Assert.True(Prestige.PaperCost(g.C, g.State, g.Me) > before);
    }

    // ---- saving, loading, migrating

    [Fact]
    public void A_lab_with_prestige_survives_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.PublishPaper(g.Id));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(168, back.Player.Prestige);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => { c.Prestige.PaperResearchWeeks = 0; c.Prestige.MinPoints = 1; });
        g.Autopilot(100);
        Assert.True(g.Try(Command.PublishPaper(g.Id)).Ok);
        g.Autopilot(30);
        Assert.True(g.Try(Command.PublishPaper(g.Id)).Ok);
        g.Autopilot(30);
        Assert.Equal(2, Papers(g).Count);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_prestige()
    {
        Game g = Lab();
        g.Do(Command.PublishPaper(g.Id));
        string old = SaveSchemaPretend.At(g.C, 48, g.Sim.Save());
        Assert.DoesNotContain("\"prestige\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(0, sim.Player.Prestige);
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_impossible_prestige_is_reported()
    {
        Game g = Lab();
        g.Me.Prestige = 1001;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("academic prestige"));
        g.Me.Prestige = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("academic prestige"));
        g.Me.Prestige = 1000;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("academic prestige"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1000, c.Prestige.Max);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Prestige.Max = 100;
        c.Prestige.PaperPrestige = 200;
        c.Prestige.DecayPerWeek = 200;
        c.Prestige.PaperResearchWeeks = 521;
        c.Prestige.MinPoints = -1;
        c.Prestige.MinResearchers = -1;
        c.Prestige.PaperReputation = 201;
        c.Prestige.MoraleAtMax = 301;
        c.Prestige.HireDiscountAtMax = 501;
        c.Prestige.PoachAtMax = 501;
        c.Prestige.CounterAtMax = 301;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "paperPrestige", "decayPerWeek", "paperResearchWeeks", "minPoints", "minResearchers", "paperReputation", "moraleAtMax", "hireDiscountAtMax", "poachAtMax", "counterAtMax" })
            Assert.Contains(problems, p => p.Contains("prestige." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_publish()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => Assert.Equal(0, lab.Prestige));
        Assert.Empty(Papers(g));
    }

    [Fact]
    public void An_autopilot_game_with_papers_now_and_then_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        g.Autopilot(48);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int published = 0;
        for (int i = 0; i < 150; i++)
        {
            g.Autopilot(1);
            if (i % 12 == 0 && g.Try(Command.PublishPaper(g.Id)).Ok) published++;
        }
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Assert.Equal(published, Papers(g).Count);
        Assert.All(g.Me.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        Assert.InRange(g.Me.Prestige, 0, 1000);
    }
}
