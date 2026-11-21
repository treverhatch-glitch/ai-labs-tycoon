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

/// <summary>Internships, the support desk, sabbaticals, open requisitions and visiting scholars running in one lab, and the rules for where they meet.</summary>
public class InternsSupportSabbaticalRequisitionsScholarsTogetherTests
{
    static (Game g, ModelState m) Lab(Action<GameConfig> tweak = null, int spareResearchers = 0, int spareEngineers = 0)
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
        g.Release(m, "game-ai");
        g.Weeks(3);
        if (spareResearchers > 0) g.Do(Command.Fire(g.Id, "researcher", spareResearchers));
        if (spareEngineers > 0) g.Do(Command.Fire(g.Id, "engineer", spareEngineers));
        g.Me.Alumni.Clear();
        return (g, m);
    }

    static List<GameEvent> Of(Game g, EventKind kind) { return g.State.Events.Where(e => e.Kind == kind).ToList(); }

    [Fact]
    public void Scholars_hold_researcher_seats_against_hiring_internships_and_orders()
    {
        var (g, _) = Lab(spareResearchers: 2);
        g.Do(Command.HostScholars(g.Id, 2));
        Assert.Equal(0, LabQueries.FreeSeats(g.C, g.Me, "researcher"));
        g.Refused(Command.Hire(g.Id, "researcher", 1), CommandError.NoSeats);
        g.Do(Command.OpenRequisition(g.Id, "researcher", 1));
        g.Weeks(g.C.Requisitions.LeadWeeks);
        Assert.Equal(0, Of(g, EventKind.RequisitionClosed).Single().Amount);
        Assert.Equal(2, g.Me.ScholarCount);
        var (h, _) = Lab(c => c.Scholars.Weeks = 40, spareResearchers: 1);
        h.Do(Command.StartInternships(h.Id, "researcher", 1));
        h.Do(Command.HostScholars(h.Id, 1));
        h.Weeks(h.C.Internships.Weeks);
        Assert.Equal(0, Of(h, EventKind.InternshipsEnded).Single().Amount);
        Assert.Single(h.Me.Alumni);
    }

    [Fact]
    public void An_intern_who_could_not_be_taken_on_can_be_rehired_at_the_alumni_price_later()
    {
        var (g, _) = Lab(spareEngineers: 0);
        g.Do(Command.StartInternships(g.Id, "engineer", 1));
        g.Weeks(g.C.Internships.Weeks);
        Alumnus left = g.Me.Alumni.Single();
        Assert.Equal(Alumni.Good, left.Terms);
        g.Do(Command.Fire(g.Id, "engineer", 1));
        g.Me.Alumni.RemoveAll(a => a.Id != left.Id);
        long fee = Alumni.Fee(g.C, g.State, g.Me, left);
        Assert.Equal(MathI.MulPermille(RolePerks.HireCost(g.C, g.Me, g.C.Role("engineer"), 1, g.State.Tick), g.C.Alumni.FeeGoodPermille), fee);
        g.Do(Command.RehireAlumnus(g.Id, left.Id));
        Assert.Empty(g.Me.Alumni);
    }

    [Fact]
    public void Nobody_on_the_desk_or_a_team_can_also_go_on_sabbatical_and_the_desk_never_draws_on_someone_away()
    {
        var (g, _) = Lab();
        List<StaffMember> engineers = g.Me.Staff.Where(p => p.RoleId == "engineer").ToList();
        g.Do(Command.SendOnSabbatical(g.Id, engineers[0].Id));
        g.Do(Command.StaffSupportDesk(g.Id, 2));
        Assert.False(SupportDesk.Away(g.C, g.Me, engineers[0]));
        Assert.True(SupportDesk.Away(g.C, g.Me, engineers[1]));
        Assert.True(SupportDesk.Away(g.C, g.Me, engineers[2]));
        Assert.False(SupportDesk.Away(g.C, g.Me, engineers[3]));
        Assert.Equal(2, SupportDesk.OnDesk(g.C, g.Me));
        g.Refused(Command.SendOnSabbatical(g.Id, engineers[1].Id), CommandError.InvalidArgument);
        g.Do(Command.SendOnSabbatical(g.Id, engineers[3].Id));
        Assert.Equal(2, Sabbatical.Away(g.Me));
        Assert.Equal(1, SupportDesk.MostPeople(g.C, g.Me));
        List<StaffMember> sales = g.Me.Staff.Where(p => p.RoleId == "sales").ToList();
        g.Do(Command.AssignAccountTeam(g.Id, "game-ai", 1));
        Assert.Empty(g.Me.Staff.Where(p => p.RoleId == "sales" && p.LeaveDue > 0).ToList());
        g.Refused(Command.SendOnSabbatical(g.Id, sales[0].Id), CommandError.InvalidArgument);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_desk_staffed_around_someone_on_sabbatical_keeps_the_backlog_clear()
    {
        Action<GameConfig> hot = c => { c.Support.RevenuePerTicket = 1000; c.Support.BaseClear = 0; c.Support.PerPerson = 1000; };
        var (g, _) = Lab(hot);
        List<StaffMember> engineers = g.Me.Staff.Where(p => p.RoleId == "engineer").ToList();
        g.Do(Command.SendOnSabbatical(g.Id, engineers[0].Id));
        Assert.Equal(2, SupportDesk.MostPeople(g.C, g.Me));
        g.Do(Command.StaffSupportDesk(g.Id, 2));
        g.Weeks(g.C.Sabbatical.Weeks);
        Assert.Equal(0, engineers[0].LeaveDue);
        Assert.Equal(0, g.Me.SupportBacklog);
        Assert.Single(Of(g, EventKind.SabbaticalEnded));
    }

    [Fact]
    public void Interns_and_scholars_are_not_staff_so_layoffs_and_labour_do_not_see_them()
    {
        var (g, _) = Lab(spareResearchers: 2);
        int staff = g.Me.Staff.Count;
        g.Do(Command.HostScholars(g.Id, 2));
        g.Do(Command.StartInternships(g.Id, "safety", 1));
        Assert.Equal(staff, g.Me.Staff.Count);
        Assert.Equal(staff, LabQueries.TotalStaff(g.Me));
        g.Do(Command.Layoff(g.Id, 300));
        Assert.Equal(2, g.Me.ScholarCount);
        Assert.Equal(1, g.Me.InternCount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_save_with_all_five_in_flight_is_sound_and_loads_and_plays_on_the_same()
    {
        var (g, _) = Lab(spareResearchers: 2, spareEngineers: 0);
        g.Do(Command.StartInternships(g.Id, "safety", 1));
        g.Do(Command.StaffSupportDesk(g.Id, 1));
        g.Do(Command.SendOnSabbatical(g.Id, g.Me.Staff.First(p => p.RoleId == "sales").Id));
        g.Do(Command.OpenRequisition(g.Id, "researcher", 1));
        g.Do(Command.HostScholars(g.Id, 1));
        g.Weeks(3);
        Assert.Equal(1, g.Me.InternCount);
        Assert.Equal(1, g.Me.SupportPeople);
        Assert.Equal(1, Sabbatical.Away(g.Me));
        Assert.Single(g.Me.Requisitions);
        Assert.Equal(1, g.Me.ScholarCount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        loaded.StepMany(40);
        g.Weeks(40);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void An_old_save_from_before_all_five_loads_with_them_idle_and_plays_on()
    {
        var (g, _) = Lab();
        string old = SaveSchemaPretend.At(g.C, 76, g.Sim.Save());
        foreach (string key in new[] { "internRole", "supportPeople", "leaveDue", "requisitions", "scholarCount" })
            Assert.DoesNotContain(key, old);
        Simulation loaded = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, loaded.State.Schema);
        LabState lab = loaded.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.InternCount);
        Assert.Equal(0, lab.SupportPeople);
        Assert.Equal(0, Sabbatical.Away(lab));
        Assert.Empty(lab.Requisitions);
        Assert.Equal(0, lab.ScholarCount);
        loaded.StepMany(10);
        Assert.Empty(SaveGame.Problems(g.C, loaded.State));
    }
}
