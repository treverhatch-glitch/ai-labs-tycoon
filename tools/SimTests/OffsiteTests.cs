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

/// <summary>The offsite: a week of salaries buys morale for everyone with room to rise, up to a little above where morale settles.</summary>
public class OffsiteTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(1, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 2, gpus: 10);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) p.Morale = 500;
        return g;
    }

    static List<GameEvent> Held(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.OffsiteHeld).ToList(); }

    [Fact]
    public void Nothing_happens_until_the_player_takes_the_team_away()
    {
        Game g = Lab();
        long hiring = g.Me.Ledger.Lifetime.Hiring;
        g.Weeks(30);
        Assert.Empty(Held(g));
        Assert.Equal(hiring, g.Me.Ledger.Lifetime.Hiring);
    }

    [Fact]
    public void An_offsite_lifts_everyone_costs_a_week_of_salaries_and_adds_culture()
    {
        Game g = Lab();
        long salaries = LabQueries.WeeklySalaries(g.C, g.Me);
        long cash = g.Me.Cash;
        long hiring = g.Me.Ledger.ThisWeek.Hiring;
        int culture = SafetyCulture.Level(g.C, g.Me);
        g.Do(Command.HoldOffsite(g.Id));
        Assert.All(g.Me.Staff, p => Assert.Equal(580, p.Morale));
        Assert.Equal(cash - salaries, g.Me.Cash);
        Assert.Equal(hiring - salaries, g.Me.Ledger.ThisWeek.Hiring);
        Assert.Equal(culture + 15, SafetyCulture.Level(g.C, g.Me));
        GameEvent e = Held(g).Single();
        Assert.Equal(g.Me.Staff.Count, e.SubjectId);
        Assert.Equal(salaries, e.Amount);
        Assert.Contains("on an offsite for $" + salaries, EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_second_offsite_gives_only_what_room_is_left_and_a_third_is_refused()
    {
        Game g = Lab();
        g.Do(Command.HoldOffsite(g.Id));
        g.Do(Command.HoldOffsite(g.Id));
        Assert.All(g.Me.Staff, p => Assert.Equal(600, p.Morale));
        g.Refused(Command.HoldOffsite(g.Id), CommandError.InvalidArgument);
        Assert.Single(Held(g).Skip(1));
        Assert.Equal(2, Held(g).Count);
    }

    [Fact]
    public void People_already_at_their_ceiling_stay_put_and_are_not_counted()
    {
        Game g = Lab();
        g.Me.Staff[0].Morale = 600;
        g.Do(Command.HoldOffsite(g.Id));
        Assert.Equal(600, g.Me.Staff[0].Morale);
        Assert.Equal(g.Me.Staff.Count - 1, Held(g).Single().SubjectId);
    }

    [Fact]
    public void Morale_never_goes_over_the_scale()
    {
        Game g = Lab(c => c.StaffPeople.OffsiteMaxAboveResting = 1000);
        foreach (StaffMember p in g.Me.Staff) p.Morale = 990;
        g.Do(Command.HoldOffsite(g.Id));
        Assert.All(g.Me.Staff, p => Assert.Equal(1000, p.Morale));
        g.Refused(Command.HoldOffsite(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_person_whose_trait_rests_higher_has_a_higher_ceiling()
    {
        Game g = Lab();
        StaffTraitConfig low = g.C.StaffPeople.Traits.First(t => t.RestMoraleOffset > 0);
        g.Me.Staff[0].TraitId = low.Id;
        int ceiling = Offsite.Ceiling(g.C, g.Me, g.Me.Staff[0]);
        Assert.Equal(Traits.RestingMorale(g.C, g.Me, g.Me.Staff[0]) + 100, ceiling);
        Assert.Equal(ceiling - low.RestMoraleOffset, Offsite.Ceiling(g.C, g.Me, g.Me.Staff[1]));
    }

    // ---- refusals

    [Fact]
    public void A_lab_with_no_staff_cannot_go()
    {
        Game g = Lab();
        g.Me.Staff.Clear();
        g.Refused(Command.HoldOffsite(g.Id), CommandError.NotEnoughStaff);
        Assert.Empty(Held(g));
    }

    [Fact]
    public void A_lab_that_cannot_pay_cannot_go_and_one_that_can_just_pay_goes()
    {
        Game g = Lab(rich: false);
        long cost = Offsite.Cost(g.C, g.Me);
        Assert.True(cost > 0);
        g.Me.Cash = cost - 1;
        g.Refused(Command.HoldOffsite(g.Id), CommandError.NotEnoughCash);
        Assert.All(g.Me.Staff, p => Assert.Equal(500, p.Morale));
        g.Me.Cash = cost;
        g.Do(Command.HoldOffsite(g.Id));
        Assert.Equal(0, g.Me.Cash);
    }

    [Fact]
    public void A_bankrupt_lab_cannot_go()
    {
        Game g = Lab();
        g.Me.Bankrupt = true;
        g.Refused(Command.HoldOffsite(g.Id), CommandError.LabBankrupt);
    }

    [Fact]
    public void Only_the_players_lab_can_go()
    {
        var options = new NewGameOptions { Seed = 1, StartEraId = "agents" };
        var g = new Game(Simulation.NewGame(TestData.Config(), options));
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        long cash = rival.Cash;
        Assert.False(g.Try(Command.HoldOffsite(rival.Id)).Ok);
        Assert.Equal(cash, rival.Cash);
    }

    [Fact]
    public void A_game_with_no_offsites_in_the_data_refuses_them()
    {
        Game g = Lab(c => c.StaffPeople.OffsiteMoraleLift = 0);
        g.Refused(Command.HoldOffsite(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_free_offsite_still_works_when_salaries_are_zero()
    {
        Game g = Lab(c => c.StaffPeople.OffsiteSalaryPermille = 0);
        long cash = g.Me.Cash;
        g.Do(Command.HoldOffsite(g.Id));
        Assert.Equal(cash, g.Me.Cash);
        Assert.All(g.Me.Staff, p => Assert.Equal(580, p.Morale));
    }

    // ---- other systems

    [Fact]
    public void Culture_stays_on_the_scale_at_the_top()
    {
        Game g = Lab();
        g.Me.CultureShift = 1000;
        g.Do(Command.HoldOffsite(g.Id));
        Assert.Equal(1000, SafetyCulture.Level(g.C, g.Me));
    }

    [Fact]
    public void An_offsite_lets_a_tired_team_crunch_again()
    {
        Game g = Lab();
        foreach (StaffMember p in g.Me.Staff) p.Morale = 440;
        g.Refused(Command.SetCrunch(g.Id, true), CommandError.TeamTooTired);
        g.Do(Command.HoldOffsite(g.Id));
        g.Do(Command.SetCrunch(g.Id, true));
        Assert.True(Crunch.Active(g.Me));
    }

    [Fact]
    public void An_offsite_during_crunch_lifts_morale_and_the_crunch_goes_on()
    {
        Game g = Lab();
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(10);
        int before = g.Me.Staff[0].Morale;
        g.Do(Command.HoldOffsite(g.Id));
        Assert.Equal(before + 80, g.Me.Staff[0].Morale);
        Assert.True(Crunch.Active(g.Me));
    }

    [Fact]
    public void Lifting_morale_keeps_a_person_who_would_quit_by_blog_post()
    {
        Game quits = Lab();
        quits.Me.Staff[0].TraitId = "quits-by-blog-post";
        quits.Me.Staff[0].Morale = 295;
        int count = quits.Me.Staff.Count;
        quits.Weeks(1);
        Assert.Equal(count - 1, quits.Me.Staff.Count);

        Game stays = Lab();
        stays.Me.Staff[0].TraitId = "quits-by-blog-post";
        stays.Me.Staff[0].Morale = 295;
        stays.Do(Command.HoldOffsite(stays.Id));
        stays.Weeks(1);
        Assert.Equal(count, stays.Me.Staff.Count);
    }

    [Fact]
    public void An_offsite_in_debt_still_costs_and_cash_is_still_explained()
    {
        Game g = Lab(rich: false);
        g.Me.Cash = Offsite.Cost(g.C, g.Me) + 10;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Do(Command.HoldOffsite(g.Id));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }

    [Fact]
    public void Morale_lifted_decays_back_to_where_it_settles_at_the_drift()
    {
        Game g = Lab();
        g.Do(Command.HoldOffsite(g.Id));
        g.Weeks(10);
        Assert.All(g.Me.Staff, p => Assert.Equal(580 - 10 * g.C.StaffPeople.MoraleDriftPerWeek, p.Morale));
    }

    // ---- save, load, replay, data

    [Fact]
    public void A_save_after_an_offsite_loads_back_the_same()
    {
        Game g = Lab();
        g.Do(Command.HoldOffsite(g.Id));
        g.Weeks(5);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game_after_an_offsite()
    {
        Game g = Game.Solo(1).WithStarterLab(researchers: 4, gpus: 4);
        g.Do(Command.HoldOffsite(g.Id));
        g.Weeks(20);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void The_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(80, c.StaffPeople.OffsiteMoraleLift);
        Assert.Empty(ConfigValidator.Problems(c));
        c.StaffPeople.OffsiteSalaryPermille = -1;
        c.StaffPeople.OffsiteMoraleLift = 1001;
        c.StaffPeople.OffsiteMaxAboveResting = -5;
        c.StaffPeople.OffsiteCultureLift = 1001;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "offsiteSalaryPermille", "offsiteMoraleLift", "offsiteMaxAboveResting", "offsiteCultureLift" })
            Assert.Contains(problems, p => p.Contains(key));
    }
}
