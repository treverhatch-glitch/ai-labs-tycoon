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

/// <summary>Sector account teams: salespeople assigned to a sector build familiarity, leave the lab-wide force, and share a cap with any other account-level deal.</summary>
public class AccountTeamsTests
{
    const string A = "game-ai";
    const string B = "game-bots";

    static (Game g, ModelState model) Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(5, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        ModelState m = g.Train("mini", 300);
        g.UntilTrained(m);
        g.Release(m, A, B);
        g.Weeks(2);
        return (g, m);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    static DisplayQueries.AppealBreakdown Appeal(Game g, ModelState m, string sector)
    {
        SectorConfig s = g.C.Sector(sector);
        return DisplayQueries.AppealParts(g.C, g.State, s, g.State.Sector(sector), g.Me, m);
    }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Accounts.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Accounts.Enabled = 2;
        c.Accounts.MaxPeople = 0;
        c.Accounts.KeepFree = -1;
        c.Accounts.PerPersonPermille = 501;
        c.Accounts.StartPermille = 1000;
        c.Accounts.MaxWeeks = 0;
        c.Accounts.ComboCapPermille = 1001;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxPeople", "keepFree", "perPersonPermille", "startPermille", "maxWeeks", "comboCapPermille" })
            Assert.True(problems.Any(p => p.Contains("accounts." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void A_lab_with_no_teams_gains_nothing_in_any_sector()
    {
        var (g, m) = Lab();
        Assert.Equal(0, Appeal(g, m, A).AccountEffect);
        Assert.Equal(0, AccountTeams.AppealPermille(g.C, g.State, g.Me, A));
        Assert.Empty(Of(g, EventKind.AccountTeamChanged));
        long total = Appeal(g, m, A).Total;
        Assert.Equal(total, DisplayQueries.Appeal(g.C, g.State, g.C.Sector(A), g.State.Sector(A), g.Me, m));
    }

    [Fact]
    public void A_team_leaves_the_sales_force_and_lifts_its_own_sector_only()
    {
        var (g, m) = Lab();
        int sales = MarketSystem.SalesPermille(g.C, g.Me);
        long before = Appeal(g, m, A).Total;
        long otherBefore = Appeal(g, m, B).Total;
        g.Do(Command.AssignAccountTeam(g.Id, A, 2));
        Assert.True(MarketSystem.SalesPermille(g.C, g.Me) < sales);
        AccountTeam team = AccountTeams.Team(g.Me, A);
        Assert.Equal(2, team.People);
        Assert.Equal(0, team.Weeks);
        GameEvent e = Of(g, EventKind.AccountTeamChanged).Single();
        Assert.Equal(1, e.Pick);
        Assert.Equal(2, e.Amount);
        Assert.Contains("account team", EventText.Describe(g.C, g.State, e));
        DisplayQueries.AppealBreakdown parts = Appeal(g, m, A);
        Assert.True(parts.AccountEffect > 0);
        Assert.Equal(parts.Core + parts.ReputationBoost + parts.SalesBoost + parts.RoleBoost + parts.TrustBoost + parts.ChampionBoost + parts.NamingConfusion + parts.ObjectiveEffect + parts.LaunchEffect + parts.PivotConfusion + parts.CampaignEffect + parts.AccountEffect, parts.Total);
        Assert.Equal(0, Appeal(g, m, B).AccountEffect);
        Assert.True(Appeal(g, m, B).Total < otherBefore);
        Assert.True(before > 0);
    }

    [Fact]
    public void Familiarity_grows_week_by_week_to_the_full_effect()
    {
        var (g, m) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 2));
        AccountTeam team = AccountTeams.Team(g.Me, A);
        Assert.Equal(300, AccountTeams.Familiarity(g.C, 0));
        Assert.Equal(1000, AccountTeams.Familiarity(g.C, 52));
        Assert.Equal(1000, AccountTeams.Familiarity(g.C, 400));
        Assert.Equal(24, AccountTeams.TeamBonus(g.C, team));
        g.Weeks(26);
        Assert.Equal(26, team.Weeks);
        Assert.Equal(650, AccountTeams.Familiarity(g.C, team.Weeks));
        long half = Appeal(g, m, A).AccountEffect;
        g.Weeks(40);
        Assert.Equal(52, team.Weeks);
        Assert.Equal(80, AccountTeams.TeamBonus(g.C, team));
        Assert.True(Appeal(g, m, A).AccountEffect > half);
    }

    [Fact]
    public void Resizing_dilutes_what_a_team_built_and_a_person_moved_starts_over()
    {
        var (g, m) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 2));
        g.Weeks(20);
        Assert.Equal(20, AccountTeams.Team(g.Me, A).Weeks);
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        Assert.Equal(10, AccountTeams.Team(g.Me, A).Weeks);
        Assert.Equal(0, Of(g, EventKind.AccountTeamChanged).Last().Pick);
        g.Do(Command.AssignAccountTeam(g.Id, B, 1));
        AccountTeam moved = AccountTeams.Team(g.Me, B);
        Assert.Equal(0, moved.Weeks);
        Assert.Equal(2, AccountTeams.Assigned(g.Me));
        Assert.True(AccountTeams.TeamBonus(g.C, AccountTeams.Team(g.Me, A)) > AccountTeams.TeamBonus(g.C, moved));
    }

    [Fact]
    public void Disbanding_a_team_returns_the_people_to_the_sales_force()
    {
        var (g, m) = Lab();
        int sales = MarketSystem.SalesPermille(g.C, g.Me);
        g.Do(Command.AssignAccountTeam(g.Id, A, 2));
        g.Do(Command.AssignAccountTeam(g.Id, A, 0));
        Assert.Null(AccountTeams.Team(g.Me, A));
        Assert.Equal(sales, MarketSystem.SalesPermille(g.C, g.Me));
        Assert.Equal(2, Of(g, EventKind.AccountTeamChanged).Last().Pick);
        Assert.Equal(0, Appeal(g, m, A).AccountEffect);
    }

    [Fact]
    public void Losing_salespeople_thins_the_teams_and_losing_the_sector_disbands_one()
    {
        var (g, m) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        g.Weeks(10);
        g.Do(Command.AssignAccountTeam(g.Id, B, 1));
        g.Do(Command.Fire(g.Id, "sales", 1));
        g.Weeks(1);
        Assert.Equal(1, AccountTeams.Assigned(g.Me));
        Assert.NotNull(AccountTeams.Team(g.Me, A));
        Assert.Null(AccountTeams.Team(g.Me, B));
        Assert.Equal(2, Of(g, EventKind.AccountTeamChanged).Last().Pick);
        g.Do(Command.PullOutOfSector(g.Id, m.Id, A));
        g.Weeks(1);
        Assert.Null(AccountTeams.Team(g.Me, A));
        GameEvent e = Of(g, EventKind.AccountTeamChanged).Last();
        Assert.Equal(4, e.Pick);
        Assert.Contains("left the sector", EventText.Describe(g.C, g.State, e));
        Assert.Equal(0, AccountTeams.Assigned(g.Me));
    }

    [Fact]
    public void An_account_team_and_another_account_deal_share_one_cap()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(150, c.Accounts.ComboCapPermille);
        Assert.Equal(80, AccountTeams.Effective(c, 80, 0));
        Assert.Equal(50, AccountTeams.Effective(c, 80, 100));
        Assert.Equal(0, AccountTeams.Effective(c, 80, 150));
        Assert.Equal(0, AccountTeams.Effective(c, 80, 400));
        Assert.Equal(80, AccountTeams.Effective(c, 80, -50));
        Assert.Equal(0, AccountTeams.Effective(c, -10, 0));
        for (int other = 0; other <= 300; other += 10)
            for (int team = 0; team <= 150; team += 10)
                Assert.True(AccountTeams.Effective(c, team, other) + other <= System.Math.Max(other, c.Accounts.ComboCapPermille));
        var (g, m) = Lab();
        Assert.Equal(0, AccountTeams.ExternalBonusPermille(g.C, g.State, g.Me, A));
        g.Do(Command.AssignAccountTeam(g.Id, A, 2));
        g.Weeks(52);
        Assert.Equal(80, AccountTeams.AppealPermille(g.C, g.State, g.Me, A));
        g.C.Accounts.ComboCapPermille = 50;
        Assert.Equal(50, AccountTeams.AppealPermille(g.C, g.State, g.Me, A));
    }

    [Fact]
    public void Refused_assignments_leave_the_game_exactly_as_it_was()
    {
        var (g, m) = Lab();
        string d = g.Sim.Digest();
        g.Refused(Command.AssignAccountTeam(g.Id, "nowhere", 1), CommandError.UnknownId);
        g.Refused(Command.AssignAccountTeam(g.Id, A, 4), CommandError.InvalidArgument);
        g.Refused(Command.AssignAccountTeam(g.Id, A, -1), CommandError.InvalidArgument);
        g.Refused(Command.AssignAccountTeam(g.Id, A, 0), CommandError.InvalidArgument);
        g.Refused(Command.AssignAccountTeam(g.Id, "expert-systems", 1), CommandError.InvalidArgument);
        g.Refused(Command.AssignAccountTeam(g.Id, A, 3), CommandError.NotEnoughStaff);
        Assert.Equal(d, g.Sim.Digest());
        g.Do(Command.AssignAccountTeam(g.Id, A, 2));
        d = g.Sim.Digest();
        g.Refused(Command.AssignAccountTeam(g.Id, A, 2), CommandError.InvalidArgument);
        g.Refused(Command.AssignAccountTeam(g.Id, B, 1), CommandError.NotEnoughStaff);
        Assert.Equal(d, g.Sim.Digest());
        var (off, _) = Lab(c => c.Accounts.Enabled = 0);
        off.Refused(Command.AssignAccountTeam(off.Id, A, 1), CommandError.InvalidArgument);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        Assert.False(w.Sim.Apply(Command.AssignAccountTeam(rival.Id, A, 1)).Ok);
    }

    [Fact]
    public void Save_and_load_keep_the_teams_and_play_on_the_same()
    {
        var (g, m) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        g.Do(Command.AssignAccountTeam(g.Id, B, 1));
        g.Weeks(9);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        LabState lab = back.State.Lab(g.Id);
        Assert.Equal(2, lab.AccountTeams.Count);
        Assert.Equal(9, AccountTeams.Team(lab, A).Weeks);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        g.Weeks(12);
        for (int i = 0; i < 12; i++) back.Step();
        Assert.Equal(g.Sim.Digest(), back.Digest());
    }

    [Fact]
    public void An_older_save_loads_with_no_teams()
    {
        var (g, m) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        string old = SaveSchemaPretend.At(g.C, 74, g.Sim.Save());
        Assert.DoesNotContain("\"accountTeams\"", old);
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.All(sim.State.Labs, lab => Assert.Empty(lab.AccountTeams));
        Assert.Empty(SaveGame.Problems(sim.Config, sim.State));
    }

    [Fact]
    public void The_save_check_catches_an_impossible_team()
    {
        var (g, m) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        Assert.DoesNotContain(SaveGame.Problems(g.C, g.State), p => p.Contains("account team"));
        AccountTeam team = g.Me.AccountTeams[0];
        team.People = 9;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("account team"));
        team.People = 1;
        team.Weeks = -1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("account team"));
        team.Weeks = 0;
        team.SectorId = "nowhere";
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("account team"));
        team.SectorId = A;
        var twin = new AccountTeam { SectorId = A, People = 1 };
        g.Me.AccountTeams.Add(twin);
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("account team"));
        g.Me.AccountTeams.Remove(twin);
        Game w = Game.New(5);
        LabState rival = w.State.Labs.First(l => l.Id != w.State.PlayerLabId);
        rival.AccountTeams.Add(new AccountTeam { SectorId = A, People = 1 });
        Assert.Contains(SaveGame.Problems(w.C, w.State), p => p.Contains("account team"));
    }

    [Fact]
    public void The_journal_replays_to_the_same_game()
    {
        var (g, m) = Lab();
        g.Do(Command.AssignAccountTeam(g.Id, A, 2));
        g.Weeks(12);
        g.Do(Command.AssignAccountTeam(g.Id, A, 1));
        g.Do(Command.AssignAccountTeam(g.Id, B, 1));
        g.Weeks(12);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }

    [Fact]
    public void A_long_game_with_teams_stays_sound_and_the_books_balance()
    {
        var (g, m) = Lab();
        long gap = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        for (int i = 0; i < 200; i++)
        {
            g.Weeks(1);
            if (i % 30 == 0) g.Try(Command.AssignAccountTeam(g.Id, i % 60 == 0 ? A : B, 1 + i % 2));
            if (i % 30 == 15) g.Try(Command.AssignAccountTeam(g.Id, A, 0));
        }
        Assert.NotEmpty(Of(g, EventKind.AccountTeamChanged));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Assert.Equal(gap, g.Me.Cash - g.Me.Ledger.Lifetime.Net);
        Simulation again = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(g.Sim.Digest(), again.Digest());
    }
}
