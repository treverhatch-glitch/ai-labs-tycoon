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

/// <summary>Chief officers: a named chief scientist lifts research, a chief safety officer cuts incident chance, and losing one costs the team its spirit.</summary>
public class ChiefsTests
{
    static Game Lab(Action<GameConfig> tweak = null, bool rich = true)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.RoomPerks.Clear();
            c.RolePerks.Clear();
            c.Safety.SectorFlavours.Clear();
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 3, engineers: 2, safety: 2, gpus: 8);
        if (rich) g.Me.Cash = 1_000_000_000_000;
        foreach (StaffMember p in g.Me.Staff) { p.TraitId = "steady"; p.Morale = 500; }
        return g;
    }

    static StaffMember Person(Game g, string roleId) { return g.Me.Staff.First(p => p.RoleId == roleId); }

    static List<GameEvent> Changes(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.ChiefChanged).ToList(); }

    static ModelState Released(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai", "game-bots"));
        return m;
    }

    static int Chance(Game g, ModelState m) { return SafetySystem.WeeklyChancePermille(g.C, g.State, g.Me, m); }

    // ---- nothing happens by itself

    [Fact]
    public void Nobody_is_a_chief_until_the_player_names_one()
    {
        Game g = Lab();
        long research = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        int poach = Workplace.PoachDefencePermille(g.C, g.Me);
        g.Weeks(30);
        Assert.Equal(-1, g.Me.ChiefScientistId);
        Assert.Equal(-1, g.Me.ChiefSafetyId);
        Assert.Equal(0, Chiefs.Count(g.C, g.Me));
        Assert.Equal(0, Chiefs.ResearchPermille(g.C, g.Me));
        Assert.Equal(0, Chiefs.IncidentCutPermille(g.C, g.Me));
        Assert.Equal(0, Chiefs.WeeklyPremium(g.C, g.Me));
        Assert.Equal(research, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
        Assert.Equal(poach, Workplace.PoachDefencePermille(g.C, g.Me));
        Assert.Empty(Changes(g));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.ChiefLeft);
    }

    // ---- the command

    [Fact]
    public void Naming_a_chief_scientist_costs_the_fee_sets_the_id_and_logs_it()
    {
        Game g = Lab();
        StaffMember who = Person(g, "researcher");
        long cash = g.Me.Cash;
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, who.Id));
        Assert.Equal(who.Id, g.Me.ChiefScientistId);
        Assert.Equal(-1, g.Me.ChiefSafetyId);
        Assert.Equal(cash - 30000, g.Me.Cash);
        GameEvent e = Changes(g).Single();
        Assert.Equal(who.Id, e.SubjectId);
        Assert.Equal(30000, e.Amount);
        Assert.Equal(1, e.Pick);
        Assert.Contains("named a chief scientist officer for $30000", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void The_title_goes_only_to_someone_in_the_right_kind_of_job()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Refused(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "engineer").Id), CommandError.InvalidArgument);
        g.Refused(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "researcher").Id), CommandError.InvalidArgument);
        g.Refused(Command.AppointChief(g.Id, "ceo", Person(g, "researcher").Id), CommandError.UnknownId);
        g.Refused(Command.AppointChief(g.Id, Chiefs.Scientist, 9999), CommandError.UnknownId);
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(-1, g.Me.ChiefScientistId);
        Assert.Empty(Changes(g));
    }

    [Fact]
    public void A_small_company_a_poor_lab_a_rival_and_a_game_without_chiefs_refuse_it()
    {
        Game small = Lab(c => c.Chiefs.MinStaff = 20);
        small.Refused(Command.AppointChief(small.Id, Chiefs.Scientist, Person(small, "researcher").Id), CommandError.NotEnoughStaff);

        Game poor = Lab(rich: false);
        poor.Me.Cash = 29_999;
        poor.Refused(Command.AppointChief(poor.Id, Chiefs.Scientist, Person(poor, "researcher").Id), CommandError.NotEnoughCash);
        Assert.Equal(-1, poor.Me.ChiefScientistId);

        Game g = Game.New(1);
        LabState rival = g.State.Labs.First(l => l.Id != g.State.PlayerLabId);
        g.Refused(Command.AppointChief(rival.Id, Chiefs.Scientist, 0), CommandError.InvalidArgument);

        Game off = Lab(c => c.Chiefs.Enabled = false);
        off.Refused(Command.AppointChief(off.Id, Chiefs.Scientist, Person(off, "researcher").Id), CommandError.InvalidArgument);
        Assert.False(Chiefs.Enabled(off.C));
    }

    [Fact]
    public void A_bankrupt_lab_cannot_name_a_chief()
    {
        Game g = Lab();
        StaffMember who = Person(g, "researcher");
        g.Me.Bankrupt = true;
        g.Refused(Command.AppointChief(g.Id, Chiefs.Scientist, who.Id), CommandError.LabBankrupt);
        Assert.Equal(-1, g.Me.ChiefScientistId);
    }

    [Fact]
    public void Naming_the_same_person_again_is_refused_and_naming_another_demotes_the_old_chief()
    {
        Game g = Lab();
        StaffMember first = g.Me.Staff.First(p => p.RoleId == "researcher");
        StaffMember second = g.Me.Staff.Last(p => p.RoleId == "researcher");
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, first.Id));
        g.Refused(Command.AppointChief(g.Id, Chiefs.Scientist, first.Id), CommandError.InvalidArgument);
        long cash = g.Me.Cash;
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, second.Id));
        Assert.Equal(second.Id, g.Me.ChiefScientistId);
        Assert.Equal(cash - 30000, g.Me.Cash);
        Assert.Equal(400, first.Morale);
        Assert.Equal(500, second.Morale);
    }

    [Fact]
    public void Standing_a_chief_down_is_free_costs_them_morale_and_empties_the_seat()
    {
        Game g = Lab();
        StaffMember who = Person(g, "safety");
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, who.Id));
        long cash = g.Me.Cash;
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, -1));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(-1, g.Me.ChiefSafetyId);
        Assert.Equal(400, who.Morale);
        GameEvent e = Changes(g).Last();
        Assert.Equal(0, e.Pick);
        Assert.Contains("stood down its chief safety officer", EventText.Describe(g.C, g.State, e));
        g.Refused(Command.AppointChief(g.Id, Chiefs.Safety, -1), CommandError.InvalidArgument);
    }

    // ---- what a chief does

    [Fact]
    public void A_chief_scientist_lifts_research_by_the_data_s_share()
    {
        Game g = Lab();
        long before = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "researcher").Id));
        long after = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        Assert.Equal(MathI.MulPermille(before, 1120), after);
        Assert.Equal(120, Chiefs.ResearchPermille(g.C, g.Me));
        g.Weeks(1);
        Assert.Equal(after, g.Me.LastWeek.ResearchGained);
    }

    [Fact]
    public void A_chief_safety_officer_cuts_the_weekly_incident_chance_and_the_breakdown_shows_it()
    {
        Game g = Lab();
        ModelState m = Released(g);
        int before = Chance(g, m);
        Assert.True(before > 0);
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        int after = Chance(g, m);
        Assert.Equal(before + Chiefs.IncidentChange(g.C, g.Me, before), after);
        Assert.True(after < before);
        DisplayQueries.IncidentBreakdown parts = DisplayQueries.IncidentParts(g.C, g.State, g.Me, m);
        Assert.Equal(after, parts.Total);
        Assert.True(parts.FromChief < 0);
    }

    [Fact]
    public void The_two_titles_do_their_own_jobs_and_nothing_else()
    {
        Game g = Lab();
        ModelState m = Released(g);
        long research = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        int chance = Chance(g, m);
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "researcher").Id));
        Assert.Equal(chance, Chance(g, m));
        Assert.Equal(0, Chiefs.IncidentCutPermille(g.C, g.Me));
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        Assert.True(TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick) > research);
        Assert.Equal(2, Chiefs.Count(g.C, g.Me));
    }

    [Fact]
    public void Each_sitting_chief_is_paid_a_premium_every_week()
    {
        Game g = Lab();
        Game h = Lab();
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "researcher").Id));
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        g.Weeks(1);
        h.Weeks(1);
        Assert.Equal(2400, Chiefs.WeeklyPremium(g.C, g.Me));
        Assert.Equal(2400, h.Me.Ledger.Lifetime.Salaries - g.Me.Ledger.Lifetime.Salaries);
    }

    [Fact]
    public void Chiefs_make_the_lab_easier_to_raid()
    {
        Game g = Lab();
        int none = Workplace.PoachDefencePermille(g.C, g.Me);
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "researcher").Id));
        Assert.Equal(none - 40, Workplace.PoachDefencePermille(g.C, g.Me));
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        Assert.Equal(none - 80, Workplace.PoachDefencePermille(g.C, g.Me));
        Assert.True(Workplace.PoachChancePermille(g.C, g.Me) > 0);
    }

    // ---- losing a chief

    [Fact]
    public void A_chief_who_leaves_empties_the_seat_and_the_team_loses_morale()
    {
        Game g = Lab();
        StaffMember who = Person(g, "researcher");
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, who.Id));
        g.Me.Staff.Remove(who);
        Assert.Equal(0, Chiefs.ResearchPermille(g.C, g.Me));
        Chiefs.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(-1, g.Me.ChiefScientistId);
        Assert.All(g.Me.Staff, p => Assert.Equal(400, p.Morale));
        GameEvent e = g.State.Events.Single(x => x.Kind == EventKind.ChiefLeft);
        Assert.Equal(100, e.Amount);
        Assert.Contains("lost its chief scientist officer", EventText.Describe(g.C, g.State, e));
        Chiefs.RunWeek(g.C, g.State, g.Me);
        Assert.Single(g.State.Events, x => x.Kind == EventKind.ChiefLeft);
    }

    [Fact]
    public void A_chief_who_is_retrained_to_another_field_stops_counting_at_once_and_the_seat_empties_on_the_week()
    {
        Game g = Lab();
        StaffMember who = Person(g, "researcher");
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, who.Id));
        g.Do(Command.RetrainStaff(g.Id, who.Id, "engineer"));
        Assert.Equal(0, Chiefs.ResearchPermille(g.C, g.Me));
        Assert.Null(Chiefs.Holder(g.C, g.Me, Chiefs.Scientist));
        g.Weeks(1);
        Assert.Equal(-1, g.Me.ChiefScientistId);
        Assert.Single(g.State.Events, x => x.Kind == EventKind.ChiefLeft);
    }

    [Fact]
    public void Firing_the_whole_role_loses_the_chief_and_naming_a_new_one_is_possible_afterwards()
    {
        Game g = Lab();
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        g.Do(Command.Fire(g.Id, "safety", 2));
        g.Weeks(1);
        Assert.Equal(-1, g.Me.ChiefSafetyId);
        g.Hire("safety", 1);
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        Assert.Equal(1, Chiefs.Count(g.C, g.Me));
    }

    // ---- edge cases

    [Fact]
    public void Zero_values_in_the_data_make_a_chief_a_title_with_a_premium_only()
    {
        Game g = Lab(c => { c.Chiefs.ResearchPermille = 0; c.Chiefs.IncidentCutPermille = 0; c.Chiefs.PoachPenaltyPermille = 0; c.Chiefs.WeeklyPremium = 0; c.Chiefs.Fee = 0; });
        ModelState m = Released(g);
        long research = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick);
        int chance = Chance(g, m);
        int poach = Workplace.PoachDefencePermille(g.C, g.Me);
        long cash = g.Me.Cash;
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "researcher").Id));
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        Assert.Equal(cash, g.Me.Cash);
        Assert.Equal(research, TrainingSystem.ResearchPerWeek(g.C, g.Me, g.Tick));
        Assert.Equal(chance, Chance(g, m));
        Assert.Equal(poach, Workplace.PoachDefencePermille(g.C, g.Me));
    }

    [Fact]
    public void The_largest_cut_in_the_data_still_leaves_some_chance()
    {
        Game g = Lab(c => c.Chiefs.IncidentCutPermille = 900);
        ModelState m = Released(g);
        int before = Chance(g, m);
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        int after = Chance(g, m);
        Assert.True(after > 0);
        Assert.Equal(before - (int)MathI.MulPermille(before, 900), after);
    }

    [Fact]
    public void A_morale_shock_never_takes_morale_below_zero()
    {
        Game g = Lab();
        StaffMember who = Person(g, "researcher");
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, who.Id));
        foreach (StaffMember p in g.Me.Staff) p.Morale = 30;
        g.Me.Staff.Remove(who);
        Chiefs.RunWeek(g.C, g.State, g.Me);
        Assert.All(g.Me.Staff, p => Assert.Equal(0, p.Morale));
    }

    // ---- saving, loading, migrating

    [Fact]
    public void Chiefs_survive_save_and_load_and_the_game_goes_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "researcher").Id));
        g.Do(Command.AppointChief(g.Id, Chiefs.Safety, Person(g, "safety").Id));
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Assert.Equal(g.Me.ChiefScientistId, back.Player.ChiefScientistId);
        Assert.Equal(g.Me.ChiefSafetyId, back.Player.ChiefSafetyId);
        g.Weeks(8);
        for (int i = 0; i < 8; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.New(2, c => c.Chiefs.Fee = 100);
        g.Autopilot(20);
        StaffMember who = g.Me.Staff.FirstOrDefault(p => g.C.Role(p.RoleId).Function == RoleFunction.Research);
        Assert.NotNull(who);
        Assert.True(g.Try(Command.AppointChief(g.Id, Chiefs.Scientist, who.Id)).Ok);
        g.Autopilot(60);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
        Assert.Equal(g.Me.ChiefScientistId, again.Player.ChiefScientistId);
    }

    [Fact]
    public void An_older_save_loads_with_no_chiefs()
    {
        Game g = Lab();
        g.Do(Command.AppointChief(g.Id, Chiefs.Scientist, Person(g, "researcher").Id));
        string old = SaveSchemaPretend.At(g.C, 53, g.Sim.Save());
        Assert.DoesNotContain("chiefScientistId", old);
        Assert.DoesNotContain("chiefSafetyId", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => { Assert.Equal(-1, lab.ChiefScientistId); Assert.Equal(-1, lab.ChiefSafetyId); });
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void A_save_with_an_impossible_chief_id_is_reported()
    {
        Game g = Lab();
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("chief"));
        g.Me.ChiefScientistId = -2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("chief"));
        g.Me.ChiefScientistId = -1;
        g.Me.ChiefSafetyId = -5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("chief"));
    }

    // ---- the data

    [Fact]
    public void The_shipped_data_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.True(c.Chiefs.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Chiefs.MinStaff = -1;
        c.Chiefs.Fee = -1;
        c.Chiefs.WeeklyPremium = -1;
        c.Chiefs.ResearchPermille = 1001;
        c.Chiefs.IncidentCutPermille = 1000;
        c.Chiefs.PoachPenaltyPermille = 301;
        c.Chiefs.ShockMorale = 1001;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "minStaff", "fee", "weeklyPremium", "researchPermille", "incidentCutPermille", "poachPenaltyPermille", "shockMorale" })
            Assert.Contains(problems, p => p.Contains("chiefs." + key));
    }

    // ---- whole games

    [Fact]
    public void Rivals_and_the_autopilot_never_name_chiefs()
    {
        Game g = Game.New(3);
        g.Autopilot(150);
        Assert.All(g.State.Labs, lab => { Assert.Equal(-1, lab.ChiefScientistId); Assert.Equal(-1, lab.ChiefSafetyId); });
        Assert.Empty(Changes(g));
    }

    [Fact]
    public void A_game_with_chiefs_who_come_and_go_stays_sound_and_the_books_balance()
    {
        Game g = Game.New(2);
        g.Autopilot(30);
        g.Me.Cash += 5_000_000;
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int named = 0;
        for (int i = 0; i < 200; i++)
        {
            g.Autopilot(1);
            if (i % 40 == 5)
            {
                StaffMember who = g.Me.Staff.FirstOrDefault(p => g.C.Role(p.RoleId).Function == RoleFunction.Research);
                if (who != null && g.Try(Command.AppointChief(g.Id, Chiefs.Scientist, who.Id)).Ok) named++;
            }
            if (i % 40 == 25 && g.Me.ChiefScientistId >= 0) g.Try(Command.AppointChief(g.Id, Chiefs.Scientist, -1));
        }
        Assert.True(named >= 1);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
    }
}
