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

/// <summary>Promotions: a fee and a raise buy a person who counts for more in their function; a function can only be so senior.</summary>
public class PromotionsTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 6);
        return g;
    }

    static StaffMember Of(Game g, string role, int skip = 0) { return g.Me.Staff.Where(p => p.RoleId == role).Skip(skip).First(); }

    static List<GameEvent> Promoted(Game g) { return g.State.Events.Where(e => e.Kind == EventKind.StaffPromoted).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Seniority.Enabled);
        Assert.Equal(1600, c.Seniority.WeightPermille);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Seniority.Enabled = 2;
        c.Seniority.WeightPermille = 999;
        c.Seniority.RaisePermille = -1;
        c.Seniority.FeePermille = 10001;
        c.Seniority.MaxSharePermille = 0;
        c.Seniority.MinMorale = 1001;
        c.Seniority.MoraleBoost = 501;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "weightPermille", "raisePermille", "feePermille", "maxSharePermille", "minMorale", "moraleBoost" })
            Assert.Contains(problems, p => p.Contains("seniority." + key));
        GameConfig free = TestData.Config();
        free.Seniority.WeightPermille = 3000;
        free.Seniority.RaisePermille = 100;
        Assert.Contains(ConfigValidator.Problems(free), p => p.Contains("never a free gain"));
    }

    [Fact]
    public void Nobody_is_senior_until_the_player_says_so()
    {
        Game g = Lab();
        g.Weeks(10);
        Assert.All(g.Me.Staff, p => Assert.Equal(0, p.Senior));
        Assert.Empty(Promoted(g));
    }

    [Fact]
    public void A_promotion_costs_the_fee_lifts_morale_and_logs_it()
    {
        Game g = Lab();
        StaffMember person = Of(g, "researcher");
        person.Morale = 600;
        long cash = g.Me.Cash;
        long fee = MathI.MulPermille(g.C.Role("researcher").HireCost, 1500);
        g.Do(Command.PromoteStaff(g.Id, person.Id));
        Assert.Equal(1, person.Senior);
        Assert.Equal(750, person.Morale);
        Assert.Equal(cash - fee, g.Me.Cash);
        GameEvent e = Assert.Single(Promoted(g));
        Assert.Equal(person.Id, e.SubjectId);
        Assert.Equal(fee, e.Amount);
        Assert.Contains("promoted a researcher to senior", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_senior_researcher_makes_more_research()
    {
        Game g = Lab();
        long before = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        g.Do(Command.PromoteStaff(g.Id, Of(g, "researcher").Id));
        long after = TrainingSystem.ResearchPerWeek(g.C, g.Me, g.State.Tick);
        Assert.True(after > before);
        Assert.True(after < before * 2);
    }

    [Fact]
    public void A_senior_engineer_safety_tester_and_salesperson_each_count_for_more()
    {
        Game g = Lab();
        int speed = TrainingSystem.SpeedPermille(g.C, g.Me);
        int align = TrainingSystem.AlignmentWorkPermille(g.C, g.Me, g.State.Tick);
        int sales = MarketSystem.SalesPermille(g.C, g.Me);
        g.Do(Command.PromoteStaff(g.Id, Of(g, "engineer").Id));
        g.Do(Command.PromoteStaff(g.Id, Of(g, "safety").Id));
        g.Do(Command.PromoteStaff(g.Id, Of(g, "sales").Id));
        Assert.True(TrainingSystem.SpeedPermille(g.C, g.Me) > speed);
        Assert.True(TrainingSystem.AlignmentWorkPermille(g.C, g.Me, g.State.Tick) > align);
        Assert.True(MarketSystem.SalesPermille(g.C, g.Me) > sales);
    }

    [Fact]
    public void The_weight_counts_each_senior_as_the_data_says()
    {
        Game g = Lab();
        Assert.Equal(4000, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering));
        g.Do(Command.PromoteStaff(g.Id, Of(g, "engineer").Id));
        Assert.Equal(3000 + 1600, LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering));
        Assert.Equal(4, LabQueries.StaffByFunction(g.C, g.Me, RoleFunction.Engineering));
    }

    [Fact]
    public void A_senior_draws_a_higher_salary()
    {
        Game g = Lab();
        long before = LabQueries.WeeklySalaries(g.C, g.Me);
        g.Do(Command.PromoteStaff(g.Id, Of(g, "researcher").Id));
        long raise = MathI.MulPermille(g.C.Role("researcher").WeeklySalary, 400);
        Assert.Equal(before + raise, LabQueries.WeeklySalaries(g.C, g.Me));
    }

    [Fact]
    public void A_function_can_only_be_so_senior_and_a_person_only_once()
    {
        Game g = Lab();
        g.Do(Command.PromoteStaff(g.Id, Of(g, "engineer").Id));
        string before = g.Sim.Digest();
        g.Refused(Command.PromoteStaff(g.Id, Of(g, "engineer").Id), CommandError.InvalidArgument);
        g.Refused(Command.PromoteStaff(g.Id, Of(g, "engineer", 1).Id), CommandError.InvalidArgument);
        Assert.Equal(before, g.Sim.Digest());
        Assert.Equal(1, Seniority.Seniors(g.C, g.Me, RoleFunction.Engineering));
        Assert.Equal(1, Seniority.Allowed(g.C, g.Me, RoleFunction.Engineering));
    }

    [Fact]
    public void A_small_function_can_have_one_senior_and_a_bigger_one_more()
    {
        Game g = Lab();
        Assert.Equal(1, Seniority.Allowed(g.C, g.Me, RoleFunction.Sales));
        Game big = Lab(c => c.Seniority.MaxSharePermille = 500);
        Assert.Equal(2, Seniority.Allowed(big.C, big.Me, RoleFunction.Engineering));
        big.Do(Command.PromoteStaff(big.Id, Of(big, "engineer").Id));
        big.Do(Command.PromoteStaff(big.Id, Of(big, "engineer", 1).Id));
        big.Refused(Command.PromoteStaff(big.Id, Of(big, "engineer", 2).Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void An_unhappy_person_cannot_be_promoted_and_nor_can_a_stranger_or_a_pauper()
    {
        Game g = Lab();
        StaffMember sad = Of(g, "researcher");
        sad.Morale = 100;
        g.Refused(Command.PromoteStaff(g.Id, sad.Id), CommandError.InvalidArgument);
        g.Refused(Command.PromoteStaff(g.Id, 99999), CommandError.UnknownId);
        sad.Morale = 700;
        g.Me.Cash = 10;
        g.Refused(Command.PromoteStaff(g.Id, sad.Id), CommandError.NotEnoughCash);
        Assert.Equal(0, sad.Senior);
    }

    [Fact]
    public void A_rival_lab_and_a_game_with_it_switched_off_cannot_promote()
    {
        Game w = Game.New(5, c => { c.Office.ChancePermille = 0; });
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.Staff.Add(new StaffMember { Id = 900, RoleId = "researcher", Morale = 500 });
        Assert.False(w.Sim.Apply(Command.PromoteStaff(rival.Id, 900)).Ok);
        Assert.Equal(0, rival.Staff.Last().Senior);
        Game off = Lab(c => c.Seniority.Enabled = 0);
        off.Refused(Command.PromoteStaff(off.Id, Of(off, "researcher").Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Seniors_are_the_last_to_go_when_people_are_let_go()
    {
        Game g = Lab();
        StaffMember senior = Of(g, "engineer");
        senior.Morale = 300;
        senior.Senior = 1;
        foreach (StaffMember p in g.Me.Staff.Where(p => p.RoleId == "engineer" && p != senior)) p.Morale = 700;
        g.Do(Command.Fire(g.Id, "engineer", 3));
        Assert.Contains(g.Me.Staff, p => p.Id == senior.Id);
        g.Do(Command.Fire(g.Id, "engineer", 1));
        Assert.DoesNotContain(g.Me.Staff, p => p.Id == senior.Id);
    }

    [Fact]
    public void Moving_a_senior_to_another_function_starts_them_over()
    {
        Game g = Lab();
        StaffMember person = Of(g, "engineer");
        person.Morale = 900;
        g.Do(Command.PromoteStaff(g.Id, person.Id));
        Assert.Equal(1, person.Senior);
        g.Do(Command.Fire(g.Id, "researcher", 1));
        g.Do(Command.RetrainStaff(g.Id, person.Id, "researcher"));
        Assert.Equal(0, person.Senior);
    }

    [Fact]
    public void Save_and_load_keep_the_seniors_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.PromoteStaff(g.Id, Of(g, "researcher").Id));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(1, back.State.Lab(g.Id).Staff.Count(p => p.Senior == 1));
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(15);
        for (int i = 0; i < 15; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_nobody_senior()
    {
        Game g = Lab();
        g.Do(Command.PromoteStaff(g.Id, Of(g, "researcher").Id));
        string old = SaveSchemaPretend.At(g.C, 61, g.Sim.Save());
        Assert.DoesNotContain("\"senior\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.All(lab.Staff, p => Assert.Equal(0, p.Senior)));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_impossible_seniors()
    {
        Game g = Lab();
        Of(g, "researcher").Senior = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("senior person"));
        Of(g, "researcher").Senior = 1;
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("senior person"));
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.Staff.Add(new StaffMember { Id = 900, RoleId = "researcher", Morale = 500, Senior = 1 });
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("senior person"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Game.Solo(2, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 3, engineers: 3, gpus: 6);
        g.Weeks(3);
        g.Do(Command.PromoteStaff(g.Id, Of(g, "researcher").Id));
        g.Weeks(5);
        g.Do(Command.PromoteStaff(g.Id, Of(g, "engineer").Id));
        g.Weeks(10);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_promotions_stays_sound_and_the_books_balance()
    {
        Game g = Game.Solo(11, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 4, engineers: 4, safety: 2, sales: 2, gpus: 8);
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        int promoted = 0;
        for (int i = 0; i < 400; i++)
        {
            g.Autopilot(1);
            if (i % 45 == 5)
            {
                StaffMember next = g.Me.Staff.FirstOrDefault(p => p.Senior == 0);
                if (next != null && g.Try(Command.PromoteStaff(g.Id, next.Id)).Ok) promoted++;
            }
        }
        Assert.True(promoted > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
