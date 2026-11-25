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

public class UserDataTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(4, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.Safety.SectorFlavours.Clear();
            c.Chiefs.MinStaff = 0;
            c.Support.Enabled = 0;
            c.Security.Enabled = 0;
            c.Tax.Enabled = 0;
            c.Standards.Enabled = 0;
            c.Reliability.Enabled = 0;
            c.GovContracts.Enabled = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab(researchers: 2, engineers: 4, safety: 1, gpus: 6);
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1_000_000;
        return g;
    }

    static void Week(Game g, long revenue)
    {
        g.Me.LastWeek.Revenue = revenue;
        UserData.RunWeek(g.C, g.State, g.Me);
    }

    static ModelState Selling(Game g)
    {
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        return m;
    }

    static void ComplaintTick(Game g)
    {
        int w = g.C.UserData.ComplaintWeeks;
        g.State.Tick = (2027 - 1995) * g.C.Calendar.WeeksPerYear;
        g.State.Tick -= g.State.Tick % w;
    }

    [Fact]
    public void A_lab_with_no_policy_collects_nothing_whatever_it_earns()
    {
        Game g = Lab();
        Week(g, 100_000_000);
        g.Weeks(5);
        Assert.Equal(0, g.Me.UserTotal);
        Assert.Equal(0, g.Me.DataRaw4);
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.UserDataChanged || e.Kind == EventKind.UserDataComplaint);
    }

    [Fact]
    public void The_policy_is_checked_and_locked_after_a_change()
    {
        Game off = Lab(c => c.UserData.Enabled = 0);
        off.Refused(Command.SetUserData(off.Id, 1), CommandError.InvalidArgument);
        Game g = Lab();
        g.Refused(Command.SetUserData(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.SetUserData(g.Id, -1), CommandError.InvalidArgument);
        g.Refused(Command.SetUserData(g.Id, 0), CommandError.InvalidArgument);
        g.Do(Command.SetUserData(g.Id, 1));
        g.Refused(Command.SetUserData(g.Id, 1), CommandError.InvalidArgument);
        g.Refused(Command.SetUserData(g.Id, 2), CommandError.InvalidArgument);
        g.Weeks(g.C.UserData.LockWeeks);
        g.Do(Command.SetUserData(g.Id, 2));
        Assert.Equal(2, g.Me.UserPolicy);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Opt_in_collects_less_than_a_default_policy_from_the_same_revenue()
    {
        Game a = Lab();
        a.Do(Command.SetUserData(a.Id, 1));
        Week(a, 1_000_000);
        Game b = Lab();
        b.Do(Command.SetUserData(b.Id, 2));
        Week(b, 1_000_000);
        Assert.Equal(1_000_000 / a.C.UserData.RevenuePerUnit * a.C.UserData.OptInPermille / 1000, a.Me.DataRaw4);
        Assert.Equal(1_000_000 / b.C.UserData.RevenuePerUnit * b.C.UserData.DefaultPermille / 1000, b.Me.DataRaw4);
        Assert.True(b.Me.DataRaw4 > a.Me.DataRaw4);
        Assert.Equal(a.Me.DataRaw4, a.Me.UserTotal);
    }

    [Fact]
    public void Collection_stops_at_the_stock_cap()
    {
        Game g = Lab(c => { c.Datasets.MaxBuy = 30; c.Datasets.StockCap = 30; });
        g.Do(Command.SetUserData(g.Id, 2));
        Week(g, 100_000_000);
        Assert.Equal(30, g.Me.DataRaw4);
        Week(g, 100_000_000);
        Assert.Equal(30, g.Me.DataRaw4);
        Assert.Equal(30, g.Me.UserTotal);
    }

    [Fact]
    public void A_default_policy_draws_a_complaint_that_fines_and_costs_trust_and_opt_in_does_not()
    {
        Game g = Lab();
        ModelState m = Selling(g);
        g.Do(Command.SetUserData(g.Id, 2));
        g.Me.UserBanked = g.C.UserData.ComplaintMin + 10;
        g.Me.UserTotal = g.Me.UserBanked;
        int trust = TrustSystem.Of(g.C, g.Me, m.Sectors[0]);
        long before = g.Me.Ledger.Lifetime.Fines;
        int banked = g.Me.UserBanked;
        ComplaintTick(g);
        Week(g, 0);
        Assert.Equal(0, g.Me.UserBanked);
        Assert.Equal(1, g.Me.UserComplaints);
        Assert.Equal(before - (long)banked * g.C.UserData.ComplaintFine, g.Me.Ledger.Lifetime.Fines);
        Assert.True(TrustSystem.Of(g.C, g.Me, m.Sectors[0]) < trust);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.UserDataComplaint);

        Game o = Lab();
        o.Do(Command.SetUserData(o.Id, 1));
        o.Me.UserBanked = 500;
        o.Me.UserTotal = 500;
        ComplaintTick(o);
        Week(o, 0);
        Assert.Equal(0, o.Me.UserComplaints);
        Game few = Lab();
        few.Do(Command.SetUserData(few.Id, 2));
        few.Me.UserBanked = few.C.UserData.ComplaintMin - 1;
        few.Me.UserTotal = few.Me.UserBanked;
        ComplaintTick(few);
        Week(few, 0);
        Assert.Equal(0, few.Me.UserComplaints);
    }

    [Fact]
    public void Moving_off_a_default_policy_deletes_most_of_the_data_but_moving_up_does_not()
    {
        Game g = Lab(c => c.UserData.LockWeeks = 0);
        g.Do(Command.SetUserData(g.Id, 2));
        g.Me.DataRaw4 = 100;
        g.Me.DataClean4 = 50;
        g.Me.UserBanked = 150;
        g.Me.UserTotal = 150;
        g.Do(Command.SetUserData(g.Id, 1));
        Assert.Equal(100 * g.C.UserData.KeepPermille / 1000, g.Me.DataRaw4);
        Assert.Equal(50 * g.C.UserData.KeepPermille / 1000, g.Me.DataClean4);
        Assert.Equal(0, g.Me.UserBanked);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.UserDataDeleted && e.Amount == 150 - 30 - 15);
        int raw = g.Me.DataRaw4;
        g.Do(Command.SetUserData(g.Id, 2));
        Assert.Equal(raw, g.Me.DataRaw4);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Users_data_goes_through_the_cleaning_stage_into_the_corpus()
    {
        Game g = Lab();
        g.Do(Command.SetUserData(g.Id, 2));
        Week(g, 4_000_000);
        int raw = (int)MathI.Min(100, g.Me.DataRaw4);
        Assert.True(raw > 0);
        g.Do(Command.CleanData(g.Id, 4, raw));
        g.Weeks(g.C.Datasets.CleanWeeks);
        Assert.Equal(raw * g.C.Datasets.YieldUsers / 1000, g.Me.DataClean4);
        Assert.True(Datasets.BonusPermille(g.C, g.Me) > 0);
    }

    [Fact]
    public void Saves_round_trip_and_older_saves_have_no_policy()
    {
        Game g = Lab();
        g.Do(Command.SetUserData(g.Id, 2));
        Week(g, 3_000_000);
        g.Weeks(1);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 115, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.UserPolicy);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
