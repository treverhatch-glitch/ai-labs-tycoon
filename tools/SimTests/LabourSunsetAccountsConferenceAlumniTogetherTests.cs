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

/// <summary>Labour relations, end-of-life notices, account teams, conference papers and the alumni pool running in one lab.</summary>
public class LabourSunsetAccountsConferenceAlumniTogetherTests
{
    const string A = "game-ai";
    const string B = "game-bots";

    static (Game g, ModelState old, ModelState next) Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            c.Labour.MoraleBelow = 1000;
            c.Labour.LowMoralePerWeek = 20;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        ModelState old = g.Train("mini", 300);
        g.UntilTrained(old);
        g.Release(old, A, B);
        ModelState next = g.Train("mini", 300);
        g.Do(Command.DesignWithModel(g.Id, next.Id, old.Id));
        g.UntilTrained(next);
        g.Release(next, A);
        g.Weeks(2);
        return (g, old, next);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void A_layoff_fills_the_pool_and_adds_pressure_and_taking_someone_back_leaves_the_pressure_alone()
    {
        var (g, _, _) = Lab();
        g.Do(Command.Layoff(g.Id, 300));
        Assert.NotEmpty(g.Me.Alumni);
        Assert.All(g.Me.Alumni, a => Assert.Equal(0, a.Terms));
        int pressure = g.Me.LabourPressure;
        Assert.True(pressure > 0);
        Alumnus back = g.Me.Alumni[0];
        int pool = g.Me.Alumni.Count;
        g.Do(Command.RehireAlumnus(g.Id, back.Id));
        Assert.Equal(pool - 1, g.Me.Alumni.Count);
        Assert.Equal(pressure, g.Me.LabourPressure);
        Assert.Single(Of(g, EventKind.AlumniRehired));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Retiring_a_model_moves_its_sectors_to_the_successor_and_the_account_teams_stay_with_them()
    {
        var (g, old, next) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        g.Do(Command.AssignAccountTeam(g.Id, B, 1));
        Assert.Equal(2, g.Me.AccountTeams.Count);
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 4));
        g.Weeks(8);
        Assert.Single(Of(g, EventKind.SunsetDone));
        Assert.Contains(g.Me.AccountTeams, t => t.SectorId == A);
        Assert.Contains(g.Me.AccountTeams, t => t.SectorId == B);
        Assert.Equal(next.Id, g.State.Sector(B).Entry(g.Id).ModelId);
        Assert.DoesNotContain(Of(g, EventKind.AccountTeamChanged), e => e.Pick == 4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void An_account_team_and_an_outside_bonus_share_one_cap()
    {
        GameConfig c = TestData.Config();
        int cap = c.Accounts.ComboCapPermille;
        Assert.Equal(0, AccountTeams.Effective(c, cap, cap));
        Assert.Equal(60, AccountTeams.Effective(c, 60, 0));
        Assert.True(AccountTeams.Effective(c, 100, 100) + 100 <= cap);
        var (g, _, _) = Lab();
        Assert.Equal(0, AccountTeams.ExternalBonusPermille(g.C, g.State, g.Me, A));
    }

    [Fact]
    public void A_paper_is_decided_while_the_lab_is_in_labour_turmoil_and_the_pool_keeps_filling()
    {
        var (g, _, _) = Lab(c => c.Labour.LowMoralePerWeek = 200);
        g.Weeks(30);
        g.Do(Command.SubmitPaper(g.Id, 1, 1));
        g.Weeks(8);
        g.Do(Command.Layoff(g.Id, 300));
        g.Weeks(4);
        Assert.Single(Of(g, EventKind.PaperDecided));
        Assert.NotEmpty(g.Me.Alumni);
        Assert.Empty(g.Me.Submissions);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_save_with_all_five_in_flight_is_sound_and_loads_and_plays_on_the_same()
    {
        var (g, old, next) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        g.Do(Command.SubmitPaper(g.Id, 2, 1));
        g.Do(Command.AnnounceSunset(g.Id, old.Id, 10));
        g.Do(Command.Layoff(g.Id, 300));
        g.Weeks(2);
        Assert.NotEmpty(g.Me.Alumni);
        Assert.NotEmpty(g.Me.Submissions);
        Assert.NotEmpty(g.Me.AccountTeams);
        Assert.True(g.Me.LabourPressure > 0);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        loaded.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void An_old_save_from_before_all_five_loads_with_them_idle_and_plays_on()
    {
        var (g, _, _) = Lab();
        string old = SaveSchemaPretend.At(g.C, 72, g.Sim.Save());
        Assert.DoesNotContain("alumni", old);
        Assert.DoesNotContain("accountTeams", old);
        Assert.DoesNotContain("labourStage", old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        LabState lab = loaded.State.Labs.First(l => l.Id == g.Id);
        Assert.Empty(lab.Alumni);
        Assert.Empty(lab.AccountTeams);
        Assert.Empty(lab.Submissions);
        Assert.Equal(0, lab.LabourStage);
        loaded.StepMany(10);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }
}
