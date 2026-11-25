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

/// <summary>Customer support desk: engineers clear tickets, and a backlog past the limit costs trust.</summary>
public class SupportDeskTests
{
    const string A = "game-ai";

    static (Game g, ModelState m) Lab(Action<GameConfig> tweak = null)
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
        g.Release(m, A);
        g.Weeks(3);
        return (g, m);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void The_data_ships_and_is_valid_and_bad_values_are_caught()
    {
        GameConfig c = TestData.Config();
        Assert.Equal(1, c.Support.Enabled);
        Assert.Empty(ConfigValidator.Problems(c));
        c.Support.Enabled = 2;
        c.Support.MaxPeople = 0;
        c.Support.RevenuePerTicket = 0;
        c.Support.PerPerson = 0;
        c.Support.MaxBacklog = 0;
        c.Support.BacklogLimit = 5;
        c.Support.TrustPerTicket = 101;
        c.Support.MaxWeeklyTrust = 1001;
        List<string> problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "maxPeople", "revenuePerTicket", "perPerson", "maxBacklog", "backlogLimit", "trustPerTicket", "maxWeeklyTrust" })
            Assert.True(problems.Any(p => p.Contains("support." + key)), key + " in " + string.Join(" | ", problems));
    }

    [Fact]
    public void The_command_is_checked_and_the_last_engineer_stays_off_the_desk()
    {
        var (g, _) = Lab();
        Assert.Equal(3, SupportDesk.MostPeople(g.C, g.Me));
        g.Refused(Command.StaffSupportDesk(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.StaffSupportDesk(g.Id, 4), CommandError.NotEnoughStaff);
        g.Refused(Command.StaffSupportDesk(g.Id, 0), CommandError.InvalidArgument);
        g.Do(Command.StaffSupportDesk(g.Id, 2));
        Assert.Equal(2, g.Me.SupportPeople);
        g.Refused(Command.StaffSupportDesk(g.Id, 2), CommandError.InvalidArgument);
        g.Do(Command.StaffSupportDesk(g.Id, 0));
        Assert.Equal(2, Of(g, EventKind.SupportDeskSet).Count);
        var (off, _) = Lab(c => c.Support.Enabled = 0);
        off.Refused(Command.StaffSupportDesk(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void People_on_the_desk_leave_engineering_work()
    {
        var (g, _) = Lab();
        long before = LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering);
        g.Do(Command.StaffSupportDesk(g.Id, 2));
        long after = LabQueries.HeadsPermille(g.C, g.Me, RoleFunction.Engineering);
        Assert.True(after < before);
        var engineers = g.Me.Staff.Where(p => p.RoleId == "engineer").ToList();
        Assert.True(SupportDesk.Away(g.C, g.Me, engineers[0]));
        Assert.True(SupportDesk.Away(g.C, g.Me, engineers[1]));
        Assert.False(SupportDesk.Away(g.C, g.Me, engineers[2]));
        Assert.False(SupportDesk.Away(g.C, g.Me, g.Me.Staff.First(p => p.RoleId == "researcher")));
    }

    [Fact]
    public void An_unstaffed_desk_lets_the_backlog_grow_and_a_staffed_one_clears_it()
    {
        var (g, _) = Lab(c => { c.Support.RevenuePerTicket = 1000; c.Support.BaseClear = 0; c.Support.PerPerson = 1000; });
        int demand = SupportDesk.Demand(g.C, g.State, g.Me);
        Assert.True(demand > 0);
        g.Weeks(3);
        Assert.True(g.Me.SupportBacklog > 0);
        g.Do(Command.StaffSupportDesk(g.Id, 1));
        g.Weeks(5);
        Assert.Equal(0, g.Me.SupportBacklog);
        Assert.Equal(g.C.Support.PerPerson, SupportDesk.Capacity(g.C, g.Me));
    }

    [Fact]
    public void A_backlog_past_the_limit_costs_trust_until_the_desk_catches_up()
    {
        Action<GameConfig> hot = c => { c.Support.RevenuePerTicket = 1000; c.Support.BaseClear = 0; c.Support.BacklogLimit = 0; c.Support.PerPerson = 1000; };
        var (g, _) = Lab(hot);
        var (twin, _) = Lab(hot);
        twin.Do(Command.StaffSupportDesk(twin.Id, 1));
        g.Weeks(6);
        twin.Weeks(6);
        Assert.True(g.Me.SupportBacklog > 0);
        Assert.Equal(0, twin.Me.SupportBacklog);
        Assert.True(g.Me.SupportBacklog > g.C.Support.BacklogLimit);
        int trust = TrustSystem.Of(g.C, g.Me, A);
        int calm = TrustSystem.Of(twin.C, twin.Me, A);
        Assert.True(trust < calm);
        Assert.Single(Of(g, EventKind.SupportBacklogChanged).Where(e => e.Pick == 1).ToList());
        g.Do(Command.StaffSupportDesk(g.Id, 1));
        g.Weeks(6);
        Assert.Equal(0, g.Me.SupportBacklog);
        Assert.Single(Of(g, EventKind.SupportBacklogChanged).Where(e => e.Pick == 0).ToList());
    }

    [Fact]
    public void The_backlog_stops_at_its_cap_and_a_lab_with_nothing_on_sale_has_none()
    {
        var (g, _) = Lab(c => { c.Support.RevenuePerTicket = 1; c.Support.BaseClear = 0; c.Support.MaxBacklog = 50; c.Support.PerPerson = 1; });
        g.Weeks(20);
        Assert.Equal(50, g.Me.SupportBacklog);
        Game bare = Game.Solo(5, c => c.PlayerStart.Cash = 1_000_000_000_000).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        bare.Weeks(10);
        Assert.Equal(0, bare.Me.SupportBacklog);
    }

    [Fact]
    public void A_save_with_a_desk_loads_and_plays_on_the_same_and_an_old_save_has_none()
    {
        var (g, _) = Lab(c => c.Support.RevenuePerTicket = 1000);
        g.Do(Command.StaffSupportDesk(g.Id, 1));
        g.Weeks(8);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        loaded.StepMany(30);
        g.Weeks(30);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        g.Me.SupportPeople = 99;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("support desk"));
        var (fresh, _) = Lab();
        string old = SaveSchemaPretend.At(fresh.C, 78, fresh.Sim.Save());
        Assert.DoesNotContain("supportPeople", old);
        Simulation back = Simulation.Load(fresh.C, old);
        Assert.Equal(0, back.State.Labs.First(l => l.Id == fresh.Id).SupportPeople);
    }
}
