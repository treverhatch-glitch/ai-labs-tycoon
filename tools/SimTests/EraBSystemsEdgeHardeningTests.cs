using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Zero, max, save/load and bankrupt edges for internships, support desk, sabbaticals, requisitions, and visiting scholars (schema 77–80).</summary>
public class EraBSystemsEdgeHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null)
    {
        return Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
    }

    [Fact]
    public void Internship_at_max_cohort_is_refused_and_zero_count_after_end_leaves_idle_fields()
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, "researcher", 1));
        g.Do(Command.StartInternships(g.Id, "researcher", g.C.Internships.MaxCohort));
        g.Refused(Command.StartInternships(g.Id, "researcher", 1), CommandError.InvalidArgument);
        g.Weeks(g.C.Internships.Weeks);
        Assert.Equal(0, g.Me.InternCount);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true);
    }

    [Fact]
    public void Support_desk_with_no_engineers_is_refused_and_save_load_keeps_backlog_non_negative()
    {
        Game g = Lab(8, c => c.Support.Enabled = 1);
        while (LabQueries.StaffCount(g.Me, "engineer") > 0) g.Do(Command.Fire(g.Id, "engineer", 1));
        g.Refused(Command.StaffSupportDesk(g.Id, 1), CommandError.NotEnoughStaff);
        g = Lab(9, c => c.Support.Enabled = 1);
        g.Do(Command.StaffSupportDesk(g.Id, 2));
        string mid = g.Sim.Save();
        g.Weeks(8);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.True(back.Player.SupportBacklog >= 0);
        HardeningInvariants.AssertBooksBalance(g.C, back.State);
    }

    [Fact]
    public void Sabbatical_on_one_engineer_survives_save_load()
    {
        Game g = Lab();
        StaffMember engineer = g.Me.Staff.First(p => p.RoleId == "engineer");
        g.Do(Command.SendOnSabbatical(g.Id, engineer.Id));
        Assert.Equal(1, Sabbatical.Away(g.Me));
        string mid = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, mid);
        Assert.Equal(1, Sabbatical.Away(back.Player));
        HardeningInvariants.AssertAllLabs(g.C, back.State, eraBounds: true, eraBBounds: true);
    }

    [Fact]
    public void Requisition_arrives_when_a_seat_was_freed_first()
    {
        Game g = Lab();
        g.Do(Command.Fire(g.Id, "engineer", 1));
        int staff = g.Me.Staff.Count;
        g.Do(Command.OpenRequisition(g.Id, "engineer", 1));
        g.Weeks(g.C.Requisitions.LeadWeeks);
        Assert.Equal(staff + 1, g.Me.Staff.Count);
        Assert.Empty(g.Me.Requisitions);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true);
    }

    [Fact]
    public void Scholars_fill_free_seats_and_save_from_schema_76_loads_idle()
    {
        Game g = Lab();
        while (LabQueries.FreeSeats(g.C, g.Me, "researcher") < g.C.Scholars.MaxScholars && LabQueries.StaffCount(g.Me, "researcher") > 0)
            g.Do(Command.Fire(g.Id, "researcher", 1));
        g.Do(Command.HostScholars(g.Id, g.C.Scholars.MaxScholars));
        Assert.Equal(0, LabQueries.FreeSeats(g.C, g.Me, "researcher"));
        g.Refused(Command.HostScholars(g.Id, 1), CommandError.InvalidArgument);
        string save = g.Sim.Save();
        string old = SaveSchemaPretend.At(g.C, 76, save);
        Simulation sim = Simulation.Load(g.C, old);
        LabState lab = sim.Player;
        Assert.Equal(0, lab.InternCount);
        Assert.Equal(0, lab.SupportPeople);
        Assert.Equal(0, lab.ScholarCount);
        Assert.Empty(lab.Requisitions);
        HardeningInvariants.StepWeeksChecking(sim, 16, eraBounds: true, eraBBounds: true);
    }

    [Fact]
    public void All_five_systems_disabled_matches_plain_autopilot_on_the_same_seed()
    {
        Game plain = Lab(4, DisableEraB);
        Game on = Lab(4);
        plain.Autopilot(100);
        on.Autopilot(100);
        Assert.Equal(0, on.Me.InternCount);
        Assert.Equal(0, on.Me.SupportPeople);
        Assert.Equal(0, on.Me.ScholarCount);
        Assert.Equal(plain.Me.Cash, on.Me.Cash);
        Assert.Equal(plain.Me.Reputation, on.Me.Reputation);
    }

    static void DisableEraB(GameConfig c)
    {
        c.Internships.Enabled = 0;
        c.Support.Enabled = 0;
        c.Sabbatical.Enabled = 0;
        c.Requisitions.Enabled = 0;
        c.Scholars.Enabled = 0;
    }

    [Fact]
    public void Schema_77_through_80_saves_round_trip_and_play_twenty_weeks()
    {
        Game g = Lab();
        g.Do(Command.StartInternships(g.Id, "safety", 1));
        g.Do(Command.StaffSupportDesk(g.Id, 1));
        string current = g.Sim.Save();
        for (int schema = 77; schema <= GameState.CurrentSchema; schema++)
        {
            string old = SaveSchemaPretend.At(g.C, schema, current);
            Simulation sim = Simulation.Load(g.C, old);
            HardeningInvariants.StepWeeksChecking(sim, 20, eraBounds: true, eraBBounds: true);
        }
    }
}