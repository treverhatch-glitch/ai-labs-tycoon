using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DevicesTests
{
    static Game Lab(int year = 2027, int capability = 800, System.Action<GameConfig> more = null)
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
        g.State.Tick = (year - 1995) * g.C.Calendar.WeeksPerYear;
        g.Me.Models.Clear();
        g.Me.Models.Add(new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = capability, Status = ModelStatus.Released, Alignment = 500, Name = "Boxed" });
        g.Me.Cash = 1_000_000_000;
        g.Me.Reputation = 500;
        return g;
    }

    [Fact]
    public void Designing_a_device_costs_the_design_and_starts_the_clock()
    {
        Game g = Lab();
        long cash = g.Me.Cash;
        g.Do(Command.DesignDevice(g.Id, 1, 2));
        Assert.Equal(cash - g.C.Devices.CostA, g.Me.Cash);
        Assert.Equal(1, g.Me.DevLine);
        Assert.Equal(1, g.Me.DevStage);
        Assert.Equal(2, g.Me.DevTier);
        Assert.Equal(g.C.Devices.WeeksA + 2, g.Me.DevWeeks);
        Assert.Equal(1, g.State.Events.Last(e => e.Kind == EventKind.DeviceDesigned).SubjectId);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_device_needs_its_era_a_good_model_one_line_and_a_valid_plan()
    {
        Game early = Lab(2015);
        early.Refused(Command.DesignDevice(early.Id, 1, 0), CommandError.MissingPrerequisite);
        Game weak = Lab(2027, 50);
        weak.Refused(Command.DesignDevice(weak.Id, 1, 0), CommandError.MissingPrerequisite);
        Game none = Lab();
        none.Me.Models.Clear();
        none.Refused(Command.DesignDevice(none.Id, 1, 0), CommandError.MissingPrerequisite);
        Game g = Lab();
        g.Refused(Command.DesignDevice(g.Id, 0, 0), CommandError.InvalidArgument);
        g.Refused(Command.DesignDevice(g.Id, 4, 0), CommandError.InvalidArgument);
        g.Refused(Command.DesignDevice(g.Id, 1, -1), CommandError.InvalidArgument);
        g.Refused(Command.DesignDevice(g.Id, 1, g.C.Devices.MaxTestWeeks + 1), CommandError.InvalidArgument);
        g.Refused(Command.SetDevicePrice(g.Id, 3), CommandError.InvalidArgument);
        g.Refused(Command.WithdrawDevice(g.Id), CommandError.InvalidArgument);
        g.Me.Cash = 10;
        g.Refused(Command.DesignDevice(g.Id, 1, 0), CommandError.NotEnoughCash);
        g.Me.Cash = 1_000_000_000;
        g.Do(Command.DesignDevice(g.Id, 1, 0));
        g.Refused(Command.DesignDevice(g.Id, 2, 0), CommandError.InvalidArgument);
        g.Refused(Command.SetDevicePrice(g.Id, 4), CommandError.InvalidArgument);
        Game off = Lab(2027, 800, c => c.Devices.Enabled = 0);
        off.Refused(Command.DesignDevice(off.Id, 1, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void Testing_and_the_model_buy_quality()
    {
        Game g = Lab();
        int none = Devices.Quality(g.C, g.Me, 0);
        int tested = Devices.Quality(g.C, g.Me, 3);
        Assert.Equal(none + 3 * g.C.Devices.TestQuality, tested);
        Game weak = Lab(2027, 150);
        Assert.True(Devices.Quality(weak.C, weak.Me, 0) < none);
        Assert.True(Devices.Quality(g.C, g.Me, 12) <= MathI.Permille);
    }

    [Fact]
    public void A_tested_device_launches_a_hit_and_adds_reputation()
    {
        Game g = Lab();
        g.Do(Command.DesignDevice(g.Id, 1, 3));
        int rep = g.Me.Reputation;
        g.Weeks(g.C.Devices.WeeksA + 3);
        Assert.Equal(2, g.Me.DevStage);
        Assert.Equal(g.C.Devices.DemandA, g.Me.DevBase);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.DeviceLaunched);
        Assert.Equal(1, e.Pick);
        Assert.True(g.Me.Reputation >= rep);
    }

    [Fact]
    public void An_untested_device_on_a_thin_model_flops_on_stage()
    {
        Game g = Lab(2027, 100);
        g.Do(Command.DesignDevice(g.Id, 1, 0));
        Assert.True(g.Me.DevQuality < g.C.Devices.LaunchBar);
        int rep = g.Me.Reputation;
        g.Weeks(g.C.Devices.WeeksA);
        Assert.Equal(2, g.Me.DevStage);
        Assert.Equal(0, g.State.Events.Last(x => x.Kind == EventKind.DeviceLaunched).Pick);
        Assert.Equal(MathI.MulPermille(g.C.Devices.DemandA, g.C.Devices.FlopDemand), g.Me.DevBase);
        Assert.True(g.Me.Reputation < rep);
    }

    static Game OnSale(int tier, int tests = 3, int capability = 800)
    {
        Game g = Lab(2027, capability);
        g.Do(Command.DesignDevice(g.Id, 1, tests));
        g.Weeks(g.C.Devices.WeeksA + tests);
        g.Do(Command.SetDevicePrice(g.Id, tier));
        return g;
    }

    [Fact]
    public void A_week_on_sale_earns_units_times_the_margin()
    {
        Game g = OnSale(2);
        long units = Devices.UnitsThisWeek(g.C, g.Me);
        long before = g.Me.Ledger.Lifetime.Revenue;
        long sold = g.Me.DevSold;
        Devices.RunWeek(g.C, g.State, g.Me);
        Assert.True(units > 0);
        Assert.Equal(before + units * g.C.Devices.MarginA, g.Me.Ledger.Lifetime.Revenue);
        Assert.Equal(sold + units, g.Me.DevSold);
        Assert.Equal(1, g.Me.DevWeeks);
    }

    [Fact]
    public void The_price_tier_trades_units_for_margin()
    {
        Game low = OnSale(1);
        Game mid = OnSale(2);
        Game high = OnSale(3);
        Assert.True(Devices.UnitsThisWeek(low.C, low.Me) > Devices.UnitsThisWeek(mid.C, mid.Me));
        Assert.True(Devices.UnitsThisWeek(mid.C, mid.Me) > Devices.UnitsThisWeek(high.C, high.Me));
        Assert.True(Devices.Clears(low.C, low.Me) < Devices.Clears(mid.C, mid.Me));
        Assert.True(Devices.Clears(mid.C, mid.Me) < Devices.Clears(high.C, high.Me));
    }

    [Fact]
    public void Demand_fades_the_longer_a_device_is_on_sale()
    {
        Game g = OnSale(2);
        long first = Devices.UnitsThisWeek(g.C, g.Me);
        for (int i = 0; i < 10; i++) Devices.RunWeek(g.C, g.State, g.Me);
        Assert.True(Devices.UnitsThisWeek(g.C, g.Me) < first);
    }

    [Fact]
    public void A_poor_device_is_recalled_after_a_while_at_a_cost_scaled_by_units_sold()
    {
        Game g = OnSale(2, 0, 100);
        long sold = 0;
        for (int i = 0; i < g.C.Devices.RecallWeeks; i++)
        {
            sold = g.Me.DevSold + Devices.UnitsThisWeek(g.C, g.Me);
            Devices.RunWeek(g.C, g.State, g.Me);
        }
        Assert.Equal(0, g.Me.DevLine);
        GameEvent e = g.State.Events.Last(x => x.Kind == EventKind.DeviceRecalled);
        Assert.Equal(sold * g.C.Devices.RecallCost / 4, e.Amount);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_good_device_is_never_recalled_and_can_be_withdrawn()
    {
        Game g = OnSale(2);
        for (int i = 0; i < g.C.Devices.RecallWeeks + 5; i++) Devices.RunWeek(g.C, g.State, g.Me);
        Assert.Equal(1, g.Me.DevLine);
        long sold = g.Me.DevSold;
        g.Do(Command.WithdrawDevice(g.Id));
        Assert.Equal(0, g.Me.DevLine);
        Assert.Equal(0, g.Me.DevSold);
        Assert.Equal(sold, g.State.Events.Last(x => x.Kind == EventKind.DeviceWithdrawn).Amount);
        g.Do(Command.DesignDevice(g.Id, 1, 0));
    }

    [Fact]
    public void A_rival_never_runs_a_device_line()
    {
        Game g = Game.New(2);
        LabState rival = g.State.Labs.First(l => l.Id != g.Id);
        rival.DevLine = 1;
        rival.DevStage = 2;
        rival.DevTier = 2;
        rival.DevBase = 100;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("device line"));
        rival.DevLine = 0;
        rival.DevStage = 0;
        rival.DevTier = 0;
        rival.DevBase = 0;
        g.Refused(Command.DesignDevice(rival.Id, 1, 0), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_live_device_saves_loads_and_matches_and_an_older_save_has_none()
    {
        Game g = Lab();
        g.Do(Command.DesignDevice(g.Id, 1, 2));
        g.Weeks(g.C.Devices.WeeksA + 6);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        back.StepMany(20);
        g.Weeks(20);
        Assert.Equal(g.Sim.Digest(), back.Digest());
        string old = SaveSchemaPretend.At(g.C, 94, g.Sim.Save());
        Assert.DoesNotContain("devLine", old);
        Assert.Equal(0, Simulation.Load(g.C, old).State.Labs.First(l => l.Id == g.Id).DevLine);
    }

    [Fact]
    public void Bad_device_data_is_caught_and_the_config_is_checked()
    {
        Game g = Game.New(2);
        g.Me.DevStage = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("device line"));
        g.Me.DevStage = 0;
        g.Me.DevLine = 2;
        Assert.Contains(SaveGame.Problems(g.C, g.State), p => p.Contains("device line"));
        g.Me.DevLine = 0;
        GameConfig c = TestData.Config();
        Assert.Empty(ConfigValidator.Problems(c));
        c.Devices.Enabled = 2;
        c.Devices.EraB = 0;
        c.Devices.WeeksA = 0;
        c.Devices.MarginC = -1;
        c.Devices.CapForMax = 0;
        c.Devices.LaunchBar = 2000;
        c.Devices.RecallWeeks = 0;
        c.Devices.LowPrice = 50;
        var problems = ConfigValidator.Problems(c);
        foreach (string key in new[] { "enabled", "eraA", "weeksA", "marginC", "capForMax", "launchBar", "recallWeeks", "lowPrice" })
            Assert.Contains(problems, p => p.StartsWith("devices.") && p.Contains(key));
    }
}
