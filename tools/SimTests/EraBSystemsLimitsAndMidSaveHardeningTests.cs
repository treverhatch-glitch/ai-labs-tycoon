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

/// <summary>Bankrupt and max-cash edges for era-b systems, and save/load in the middle of multi-week processes.</summary>
public class EraBSystemsLimitsAndMidSaveHardeningTests
{
    static Game Lab(long seed = 7, Action<GameConfig> tweak = null, long? cash = null)
    {
        Game g = Game.Solo(seed, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Litigation.Enabled = 0;
            c.Security.Enabled = 0;
            c.GovContracts.Enabled = 0;
            c.Scaling.Enabled = 0;
            c.Charter.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.Covenants.Enabled = 0;
            c.Allowances.Enabled = 0;
            c.Listing.Enabled = 0;
            c.Rbf.Enabled = 0;
            c.Foundation.Enabled = 0;
            c.Leaseback.Enabled = 0;
            c.ConvertibleNotes.Enabled = 0;
            c.PlayerStart.Cash = 1_000_000_000_000;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 8);
        if (cash.HasValue) g.Me.Cash = cash.Value;
        return g;
    }

    [Fact]
    public void Internships_refuse_without_cash_and_run_clean_at_max_cash()
    {
        Game broke = Lab(3, cash: 1);
        broke.Refused(Command.StartInternships(broke.Id, "researcher", 1), CommandError.NotEnoughCash);
        Game rich = Lab(4);
        rich.Do(Command.Fire(rich.Id, "researcher", 1));
        rich.Do(Command.StartInternships(rich.Id, "researcher", 1));
        rich.Weeks(rich.C.Internships.Weeks);
        HardeningInvariants.AssertAllLabs(rich.C, rich.State, eraBounds: true, eraBBounds: true);
    }

    [Fact]
    public void Support_desk_with_zero_cash_mid_run_and_at_max_wealth_still_balances()
    {
        Game rich = Lab(5);
        rich.Do(Command.StaffSupportDesk(rich.Id, 2));
        rich.Weeks(12);
        HardeningInvariants.AssertBooksBalance(rich.C, rich.State);

        Game broke = Lab(6);
        broke.Do(Command.StaffSupportDesk(broke.Id, 1));
        broke.Me.Cash = 0;
        broke.Weeks(8);
        Assert.Empty(SaveGame.Problems(broke.C, broke.State));
    }

    [Fact]
    public void Sabbatical_on_a_bankrupt_lab_after_go_bankrupt_still_validates()
    {
        Game g = Lab(7, c => c.PlayerStart.Cash = 500_000);
        g.Do(Command.SendOnSabbatical(g.Id, g.Me.Staff.First(p => p.RoleId == "safety").Id));
        Finance.GoBankrupt(g.State, g.Me);
        Assert.True(g.Me.Bankrupt);
        string save = g.Sim.Save();
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, save);
        Assert.True(back.Player.Bankrupt);
        Assert.Empty(SaveGame.Problems(back.Config, back.State));
    }

    [Fact]
    public void Requisitions_refuse_deposit_when_broke_and_at_max_cash_arrive_on_schedule()
    {
        Game broke = Lab(8, cash: 0);
        broke.Refused(Command.OpenRequisition(broke.Id, "engineer", 1), CommandError.NotEnoughCash);
        Game g = Lab(9);
        g.Do(Command.Fire(g.Id, "engineer", 2));
        int staff = g.Me.Staff.Count;
        g.Do(Command.OpenRequisition(g.Id, "engineer", 2));
        g.Weeks(g.C.Requisitions.LeadWeeks);
        Assert.Equal(staff + 2, g.Me.Staff.Count);
        HardeningInvariants.AssertBooksBalance(g.C, g.State);
    }

    [Fact]
    public void Scholars_refuse_stipend_without_cash_and_finish_at_max_cash()
    {
        Game g = Lab(10);
        while (LabQueries.FreeSeats(g.C, g.Me, "researcher") < g.C.Scholars.MaxScholars && LabQueries.StaffCount(g.Me, "researcher") > 0)
            g.Do(Command.Fire(g.Id, "researcher", 1));
        Game broke = Lab(11, cash: 0);
        while (LabQueries.FreeSeats(broke.C, broke.Me, "researcher") < 1 && LabQueries.StaffCount(broke.Me, "researcher") > 0)
            broke.Do(Command.Fire(broke.Id, "researcher", 1));
        broke.Refused(Command.HostScholars(broke.Id, 1), CommandError.NotEnoughCash);
        g.Do(Command.HostScholars(g.Id, g.C.Scholars.MaxScholars));
        g.Weeks(g.C.Scholars.Weeks);
        Assert.Equal(0, g.Me.ScholarCount);
        HardeningInvariants.AssertAllLabs(g.C, g.State, eraBounds: true, eraBBounds: true);
    }

    [Fact]
    public void Requisition_mid_lead_save_load_matches_uninterrupted_arrival()
    {
        Game straight = Lab(12);
        straight.Do(Command.Fire(straight.Id, "engineer", 1));
        straight.Do(Command.OpenRequisition(straight.Id, "engineer", 1));
        int lead = straight.C.Requisitions.LeadWeeks;
        int half = lead / 2;
        straight.Weeks(lead);

        Game choppy = Lab(12);
        choppy.Do(Command.Fire(choppy.Id, "engineer", 1));
        choppy.Do(Command.OpenRequisition(choppy.Id, "engineer", 1));
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(lead - half);

        Assert.Equal(straight.Me.Staff.Count, choppy.Me.Staff.Count);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Sabbatical_mid_save_load_matches_uninterrupted_return()
    {
        Game straight = Lab(13);
        StaffMember person = straight.Me.Staff.First(p => p.RoleId == "engineer");
        straight.Do(Command.SendOnSabbatical(straight.Id, person.Id));
        straight.Weeks(straight.C.Sabbatical.Weeks);

        Game choppy = Lab(13);
        StaffMember other = choppy.Me.Staff.First(p => p.RoleId == "engineer");
        choppy.Do(Command.SendOnSabbatical(choppy.Id, other.Id));
        int half = choppy.C.Sabbatical.Weeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Sabbatical.Weeks - half);

        Assert.Equal(0, Sabbatical.Away(straight.Me));
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Scholars_mid_save_load_matches_uninterrupted_departure()
    {
        Game straight = Lab(14);
        while (LabQueries.FreeSeats(straight.C, straight.Me, "researcher") < 2 && LabQueries.StaffCount(straight.Me, "researcher") > 0)
            straight.Do(Command.Fire(straight.Id, "researcher", 1));
        straight.Do(Command.HostScholars(straight.Id, 2));
        straight.Weeks(straight.C.Scholars.Weeks);

        Game choppy = Lab(14);
        while (LabQueries.FreeSeats(choppy.C, choppy.Me, "researcher") < 2 && LabQueries.StaffCount(choppy.Me, "researcher") > 0)
            choppy.Do(Command.Fire(choppy.Id, "researcher", 1));
        choppy.Do(Command.HostScholars(choppy.Id, 2));
        int half = choppy.C.Scholars.Weeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Scholars.Weeks - half);

        Assert.Equal(0, straight.Me.ScholarCount);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }

    [Fact]
    public void Internship_mid_save_load_matches_uninterrupted_graduation()
    {
        Game straight = Lab(15);
        straight.Do(Command.Fire(straight.Id, "researcher", 1));
        straight.Do(Command.StartInternships(straight.Id, "researcher", 1));
        straight.Weeks(straight.C.Internships.Weeks);

        Game choppy = Lab(15);
        choppy.Do(Command.Fire(choppy.Id, "researcher", 1));
        choppy.Do(Command.StartInternships(choppy.Id, "researcher", 1));
        int half = choppy.C.Internships.Weeks / 2;
        choppy.Weeks(half);
        choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        choppy.Weeks(choppy.C.Internships.Weeks - half);

        Assert.Equal(0, straight.Me.InternCount);
        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
    }
}