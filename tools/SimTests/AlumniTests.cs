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

/// <summary>Alumni network: people who leave stay in a pool for a while and can be asked back, with their old trait and consequences for morale.</summary>
public class AlumniTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        return Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    static long Hire(Game g, string role) { return RolePerks.HireCost(g.C, g.Me, g.C.Role(role), 1, g.State.Tick); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Alumni.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Alumni.Enabled = 2;
        c.Alumni.MaxSize = 0;
        c.Alumni.ExpireWeeks = 0;
        c.Alumni.FeeGoodPermille = 800;
        c.Alumni.FeeLaidOffPermille = 700;
        c.Alumni.MoraleGood = -100;
        c.Alumni.MoraleLaidOff = 0;
        c.Alumni.PeerGood = -50;
        c.Alumni.PeerLaidOff = 0;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxSize", "expireWeeks", "feeGoodPermille", "moraleGood", "peerGood" })
            Assert.True(problems.Any(p => p.Contains("alumni." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_lab_where_nobody_leaves_has_no_alumni()
    {
        Game g = Lab();
        g.Weeks(80);
        Assert.Empty(g.Me.Alumni);
        Assert.Empty(Of(g, EventKind.AlumniRehired));
    }

    [Fact]
    public void A_person_let_go_joins_the_pool_and_can_come_back_with_their_trait_at_a_discount()
    {
        Game g = Lab();
        StaffMember before = g.Me.Staff.Where(p => p.RoleId == "engineer").OrderBy(p => p.Morale).First();
        string trait = before.TraitId;
        g.Do(Command.Fire(g.Id, "engineer", 1));
        Alumnus gone = g.Me.Alumni.Single();
        Assert.Equal("engineer", gone.RoleId);
        Assert.Equal(trait, gone.TraitId);
        Assert.Equal(Alumni.LaidOff, gone.Terms);
        Assert.Equal(g.State.Tick, gone.Left);
        long fee = Hire(g, "engineer") * 700 / 1000;
        Assert.Equal(fee, Alumni.Fee(g.C, g.State, g.Me, gone));
        long cash = g.Me.Cash;
        int count = g.Me.Staff.Count;
        long peers = g.Me.Staff.Sum(p => p.Morale);
        g.Do(Command.RehireAlumnus(g.Id, gone.Id));
        Assert.Equal(cash - fee, g.Me.Cash);
        Assert.Equal(count + 1, g.Me.Staff.Count);
        Assert.Empty(g.Me.Alumni);
        StaffMember back = g.Me.Staff.Last();
        Assert.Equal(trait, back.TraitId);
        Assert.Equal("engineer", back.RoleId);
        Assert.NotEqual(gone.Id, back.Id);
        Assert.Equal(MathI.Clamp(Traits.RestingMorale(g.C, g.Me, back) - 40, 0, 1000), back.Morale);
        Assert.True(g.Me.Staff.Where(p => p != back).Sum(p => p.Morale) < peers);
        GameEvent e = Of(g, EventKind.AlumniRehired).Single();
        Assert.Equal(fee, e.Amount);
        Assert.Equal(0, e.Pick);
        Assert.Contains("laid off", EventText.Describe(g.C, g.State, e));
    }

    [Fact]
    public void A_layoff_round_sends_everyone_it_cuts_to_the_pool()
    {
        Game g = Lab();
        int total = Layoffs.TotalCut(g.C, g.Me, 300);
        Assert.True(total > 0);
        g.Do(Command.Layoff(g.Id, 300));
        Assert.Equal(total, g.Me.Alumni.Count);
        Assert.All(g.Me.Alumni, a => Assert.Equal(Alumni.LaidOff, a.Terms));
        Assert.Equal(total, g.Me.Alumni.Select(a => a.Id).Distinct().Count());
    }

    [Fact]
    public void Someone_poached_leaves_on_good_terms_and_returns_cheaper_and_happier()
    {
        Game g = Lab();
        StaffMember person = g.Me.Staff.First(p => p.RoleId == "researcher");
        TalentSystem.Leave(g.C, g.State, null, g.Me, person);
        Alumnus gone = g.Me.Alumni.Single();
        Assert.Equal(Alumni.Good, gone.Terms);
        Assert.Equal(person.Id, gone.Id);
        Assert.Equal(Hire(g, "researcher") * 500 / 1000, Alumni.Fee(g.C, g.State, g.Me, gone));
        long peers = g.Me.Staff.Sum(p => p.Morale);
        g.Do(Command.RehireAlumnus(g.Id, gone.Id));
        StaffMember back = g.Me.Staff.Last();
        Assert.Equal(MathI.Clamp(Traits.RestingMorale(g.C, g.Me, back) + 80, 0, 1000), back.Morale);
        Assert.True(g.Me.Staff.Where(p => p != back).Sum(p => p.Morale) > peers);
        Assert.Equal(1, Of(g, EventKind.AlumniRehired).Single().Pick);
    }

    [Fact]
    public void People_who_quit_are_remembered_the_pool_is_capped_and_a_return_costs_nearly_full_price()
    {
        Game g = Lab(c => c.StaffPeople.Traits.First(t => t.QuitBelowMorale <= 0).QuitBelowMorale = c.StaffPeople.MoraleBaseline);
        int staff = g.Me.Staff.Count;
        Assert.True(staff > 12);
        g.Do(Command.SetCrunch(g.Id, true));
        g.Weeks(3);
        Assert.Empty(g.Me.Staff);
        Assert.Equal(12, g.Me.Alumni.Count);
        Assert.All(g.Me.Alumni, a => Assert.Equal(Alumni.Quit, a.Terms));
        Alumnus one = g.Me.Alumni.First();
        Assert.Equal(Hire(g, one.RoleId) * 900 / 1000, Alumni.Fee(g.C, g.State, g.Me, one));
    }

    [Fact]
    public void Leavers_expire_from_the_pool_after_their_weeks()
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, "engineer", 1));
        g.Weeks(100);
        Assert.Single(g.Me.Alumni);
        g.Weeks(5);
        Assert.Empty(g.Me.Alumni);
        Assert.Empty(Of(g, EventKind.AlumniRehired));
    }

    [Fact]
    public void Refused_rehires_leave_the_game_exactly_as_it_was()
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, "engineer", 1));
        Alumnus gone = g.Me.Alumni.Single();
        string d = g.Sim.Digest();
        g.Refused(Command.RehireAlumnus(g.Id, 9999), CommandError.UnknownId);
        Assert.Equal(d, g.Sim.Digest());
        g.Hire("engineer", 1);
        d = g.Sim.Digest();
        g.Refused(Command.RehireAlumnus(g.Id, gone.Id), CommandError.NoSeats);
        Assert.Equal(d, g.Sim.Digest());

        Game poor = Lab();
        poor.Do(Command.Fire(poor.Id, "engineer", 1));
        poor.Me.Cash = 10;
        d = poor.Sim.Digest();
        Assert.False(poor.Sim.Apply(Command.RehireAlumnus(poor.Id, poor.Me.Alumni[0].Id)).Ok);
        Assert.Equal(d, poor.Sim.Digest());
        Game off = Lab(c => c.Alumni.Enabled = 0);
        off.Do(Command.Fire(off.Id, "engineer", 1));
        Assert.Empty(off.Me.Alumni);
        off.Refused(Command.RehireAlumnus(off.Id, 1), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.RehireAlumnus(rival.Id, 1)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_pool_and_play_on_the_same()
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, "engineer", 2));
        g.Weeks(10);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(2, lab.Alumni.Count);
        Assert.Equal(g.Me.Alumni[0].TraitId, lab.Alumni[0].TraitId);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        int id = lab.Alumni[0].Id;
        g.Do(Command.RehireAlumnus(g.Id, id));
        back.Apply(Command.RehireAlumnus(g.Id, id));
        g.Weeks(5);
        for (int i = 0; i < 5; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_alumni()
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, "engineer", 1));
        string old = SaveSchemaPretend.At(g.C, 75, g.Sim.Save());
        Assert.DoesNotContain("\"alumni\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Empty(lab.Alumni));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_alumnus()
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, "engineer", 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("alumnus"));
        Alumnus a = g.Me.Alumni[0];
        a.Terms = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("alumnus"));
        a.Terms = 0;
        a.RoleId = "astronaut";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("alumnus"));
        a.RoleId = "engineer";
        a.Left = 99999;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("alumnus"));
        a.Left = 0;
        g.Me.Alumni.Add(new Alumnus { Id = a.Id, RoleId = "engineer", TraitId = a.TraitId });
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("alumnus"));
        g.Me.Alumni.RemoveAt(1);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.Alumni.Add(new Alumnus { Id = 1, RoleId = "engineer", TraitId = "" });
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("alumni"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        Game g = Lab();
        g.Do(Command.Layoff(g.Id, 300));
        g.Weeks(6);
        g.Do(Command.RehireAlumnus(g.Id, g.Me.Alumni[0].Id));
        g.Weeks(20);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_leavers_and_returns_stays_sound_and_the_books_balance()
    {
        Game g = Lab();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 260; i++)
        {
            g.Weeks(1);
            if (i % 26 == 3) g.Try(Command.Layoff(g.Id, 300));
            if (i % 26 == 9 && g.Me.Alumni.Count > 0) g.Try(Command.RehireAlumnus(g.Id, g.Me.Alumni[0].Id));
            if (i % 26 == 12 && g.Me.Alumni.Count > 0) g.Try(Command.RehireAlumnus(g.Id, g.Me.Alumni[g.Me.Alumni.Count - 1].Id));
        }
        Assert.NotEmpty(Of(g, EventKind.AlumniRehired));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
