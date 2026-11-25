using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class LeasebackTests
{
    static Game Lab(int units = 10, System.Action<GameConfig> more = null)
    {
        Game g = Game.Solo(7, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 4, engineers: 4, safety: 3, sales: 3, gpus: 0);
        Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.State.Tick).IdValue, units);
        g.Me.Cash = 1_000_000_000;
        return g;
    }

    static int Owned(Game g) { return LabQueries.OwnedGpuUnits(g.Me, HardwareBatch.Hq); }

    [Fact]
    public void A_sale_leaseback_pays_most_of_the_value_now_and_keeps_the_compute()
    {
        Game g = Lab();
        long value = Leaseback.ValueOf(g.C, g.State, g.Me, 6);
        long cash = g.Me.Cash;
        g.Do(Command.SaleLeaseback(g.Id, 6));
        Assert.Equal(cash + MathI.MulPermille(value, g.C.Leaseback.AdvancePermille), g.Me.Cash);
        Assert.Equal(10, Owned(g));
        Assert.Equal(6, g.Me.LeaseUnits);
        Assert.Equal(value, g.Me.LeaseValue);
        Assert.Equal(MathI.MulPermille(value, g.C.Leaseback.RentPermille), g.Me.LeaseRent);
        Assert.Equal(MathI.MulPermille(value, g.C.Leaseback.BuybackPermille), g.Me.LeaseBuyback);
        Assert.Equal(g.C.Leaseback.TermWeeks, g.Me.LeaseLeft);
        Assert.Equal(6, g.State.Events.Last(e => e.Kind == EventKind.GpusLeasedBack).SubjectId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Only_owned_head_office_units_one_lease_at_a_time()
    {
        Game g = Lab(4);
        g.Refused(Command.SaleLeaseback(g.Id, 0), CommandError.InvalidArgument);
        g.Refused(Command.SaleLeaseback(g.Id, 5), CommandError.NotEnoughGpus);
        g.Refused(Command.BuyBackLease(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.ExtendLease(g.Id), CommandError.InvalidArgument);
        g.Refused(Command.ReturnLease(g.Id), CommandError.InvalidArgument);
        g.Do(Command.SaleLeaseback(g.Id, 3));
        g.Refused(Command.SaleLeaseback(g.Id, 1), CommandError.InvalidArgument);
        Game off = Lab(4, c => c.Leaseback.Enabled = 0);
        off.Refused(Command.SaleLeaseback(off.Id, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Leased_units_cannot_be_sold_but_the_rest_can()
    {
        Game g = Lab(10);
        g.Do(Command.SaleLeaseback(g.Id, 7));
        g.Refused(Command.SellGpus(g.Id, 4), CommandError.NotEnoughGpus);
        g.Do(Command.SellGpus(g.Id, 3));
        Assert.Equal(7, Owned(g));
        g.Refused(Command.SellGpus(g.Id, 1), CommandError.NotEnoughGpus);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_lease_lowers_what_the_lab_can_borrow_against_its_chips()
    {
        Game g = Lab(10);
        long collateral = Loans.Collateral(g.C, g.State, g.Me);
        long capacity = Loans.Capacity(g.C, g.State, g.Me);
        g.Do(Command.SaleLeaseback(g.Id, 10));
        Assert.Equal(0, Loans.Collateral(g.C, g.State, g.Me));
        Assert.True(Loans.Capacity(g.C, g.State, g.Me) < capacity);
        Assert.True(collateral > 0);
        g.Do(Command.BuyBackLease(g.Id));
        Assert.Equal(collateral, Loans.Collateral(g.C, g.State, g.Me));
    }

    [Fact]
    public void Rent_is_paid_each_week_as_a_cost_and_the_term_counts_down()
    {
        Game g = Lab();
        g.Do(Command.SaleLeaseback(g.Id, 5));
        long rent = g.Me.LeaseRent;
        long before = g.Me.Ledger.Lifetime.GpuRent;
        long cash = g.Me.Cash;
        Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(before - rent, g.Me.Ledger.Lifetime.GpuRent);
        Assert.Equal(cash - rent, g.Me.Cash);
        Assert.Equal(g.C.Leaseback.TermWeeks - 1, g.Me.LeaseLeft);
        Assert.Equal(0, g.Me.LeaseMissed);
    }

    [Fact]
    public void Buying_back_pays_the_fixed_price_frees_the_units_and_ends_the_rent()
    {
        Game g = Lab();
        g.Do(Command.SaleLeaseback(g.Id, 5));
        long price = g.Me.LeaseBuyback;
        long cash = g.Me.Cash;
        g.Me.Cash = price - 1;
        Assert.False(g.Try(Command.BuyBackLease(g.Id)).Ok);
        g.Me.Cash = cash;
        g.Do(Command.BuyBackLease(g.Id));
        Assert.Equal(cash - price, g.Me.Cash);
        Assert.False(Leaseback.Leasing(g.Me));
        Assert.Equal(10, Owned(g));
        Assert.Equal(price, g.State.Events.Last(e => e.Kind == EventKind.LeaseBoughtBack).Amount);
        g.Do(Command.SellGpus(g.Id, 10));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void An_extension_adds_weeks_at_a_higher_rent_once()
    {
        Game g = Lab();
        g.Do(Command.SaleLeaseback(g.Id, 5));
        long rent = g.Me.LeaseRent;
        g.Do(Command.ExtendLease(g.Id));
        Assert.Equal(g.C.Leaseback.TermWeeks + g.C.Leaseback.ExtendWeeks, g.Me.LeaseLeft);
        Assert.Equal(MathI.MulPermille(rent, g.C.Leaseback.ExtendRentPermille), g.Me.LeaseRent);
        g.Refused(Command.ExtendLease(g.Id), CommandError.InvalidArgument);
        Assert.Equal(1, g.State.Events.Count(e => e.Kind == EventKind.LeaseExtended));
    }

    [Fact]
    public void Handing_the_units_back_early_removes_the_oldest_and_stops_the_rent()
    {
        Game g = Lab();
        g.Do(Command.SaleLeaseback(g.Id, 4));
        g.Do(Command.ReturnLease(g.Id));
        Assert.Equal(6, Owned(g));
        Assert.False(Leaseback.Leasing(g.Me));
        LedgerTotals ledger = g.Me.Ledger.Lifetime;
        Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(ledger.GpuRent, g.Me.Ledger.Lifetime.GpuRent);
        EventLike(g, EventKind.LeaseEnded, 1, 4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    static void EventLike(Game g, EventKind kind, int pick, long amount)
    {
        GameEvent e = g.State.Events.Last(x => x.Kind == kind);
        Assert.Equal(pick, e.Pick);
        Assert.Equal(amount, e.Amount);
    }

    [Fact]
    public void When_the_term_runs_out_the_units_go_back()
    {
        Game g = Lab(10, c => c.Leaseback.TermWeeks = 3);
        g.Do(Command.SaleLeaseback(g.Id, 4));
        for (int i = 0; i < 3; i++) Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.False(Leaseback.Leasing(g.Me));
        Assert.Equal(6, Owned(g));
        EventLike(g, EventKind.LeaseEnded, 0, 4);
    }

    [Fact]
    public void Missing_the_rent_in_a_row_gets_the_units_repossessed_and_costs_reputation()
    {
        Game g = Lab();
        g.Do(Command.SaleLeaseback(g.Id, 4));
        g.Me.Reputation = 500;
        g.Me.Cash = 0;
        Leaseback.RunWeek(g.C, g.State, g.Me);
        Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(2, g.Me.LeaseMissed);
        g.Me.Cash = 1_000_000_000;
        Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(0, g.Me.LeaseMissed);
        g.Me.Cash = 0;
        for (int i = 0; i < g.C.Leaseback.MissLimit; i++) Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.False(Leaseback.Leasing(g.Me));
        Assert.Equal(6, Owned(g));
        Assert.True(g.Me.Reputation < 500);
        EventLike(g, EventKind.LeaseEnded, 2, 4);
        Assert.True(g.State.Events.Count(e => e.Kind == EventKind.LeaseRentMissed) >= g.C.Leaseback.MissLimit);
    }

    [Fact]
    public void Units_lost_some_other_way_shrink_the_lease_with_them()
    {
        Game g = Lab();
        g.Do(Command.SaleLeaseback(g.Id, 10));
        long value = g.Me.LeaseValue;
        foreach (HardwareBatch batch in Fleet.OldestFirst(g.C, g.Me, HardwareBatch.Hq).ToList()) Fleet.Take(g.Me, batch, batch.Count > 5 ? 5 : batch.Count);
        Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(Owned(g), g.Me.LeaseUnits);
        Assert.True(g.Me.LeaseValue < value);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        foreach (HardwareBatch batch in Fleet.OldestFirst(g.C, g.Me, HardwareBatch.Hq).ToList()) Fleet.Take(g.Me, batch, batch.Count);
        Leaseback.RunWeek(g.C, g.State, g.Me);
        Assert.False(Leaseback.Leasing(g.Me));
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_live_lease_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.SaleLeaseback(g.Id, 6));
        g.Weeks(20);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 92, g.Sim.Save());
        Assert.DoesNotContain("leaseUnits", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).LeaseUnits);
    }

    [Fact]
    public void A_rival_lease_and_bad_data_are_caught_and_the_data_is_checked()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.LeaseUnits = 2;
        rival.LeaseValue = 10;
        rival.LeaseLeft = 5;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("leased back"));
        rival.LeaseUnits = 0;
        rival.LeaseValue = 0;
        rival.LeaseLeft = 0;
        g.Me.LeaseRent = 9;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("leased back"));
        g.Me.LeaseRent = 0;
        g.Me.LeaseUnits = 1;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("leased back"));
        g.Me.LeaseUnits = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Leaseback.Enabled = 2;
        c.Leaseback.AdvancePermille = 0;
        c.Leaseback.TermWeeks = 0;
        c.Leaseback.BuybackPermille = 50;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "advancePermille", "termWeeks", "buybackPermille" })
            Assert.Contains(problems, p => p.Contains("leaseback." + key));
    }
}
