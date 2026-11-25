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

public class FabricTests
{
    static Game Lab(int gpus = 10, Action<GameConfig> more = null)
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
            c.Fabric.EraA = 0;
            c.Fabric.EraB = 0;
            c.Fabric.EraC = 0;
            if (more != null) more(c);
        }).WithStarterLab(researchers: 3, engineers: 3, safety: 2, sales: 2, gpus: 0);
        g.Me.Cash = 1_000_000_000;
        if (gpus > 0) Fleet.AddGpus(g.State, g.Me, HardwareBatch.Hq, LabQueries.CurrentGeneration(g.C, g.Tick).Id, gpus);
        return g;
    }

    static long Paid(Game g, long before) { return before - g.Me.Ledger.Lifetime.Hardware; }

    [Fact]
    public void A_lab_that_never_orders_has_no_fabric_and_no_effect()
    {
        Game g = Lab();
        int speed = TrainingSystem.SpeedPermille(g.C, g.Me);
        g.Weeks(8);
        Assert.Equal(0, g.Me.FabTier);
        Assert.Equal(0, Fabric.Permille(g.C, g.Me));
        Assert.Equal(speed, TrainingSystem.SpeedPermille(g.C, g.Me));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.FabricOrdered || e.Kind == EventKind.FabricDelivered);
    }

    [Fact]
    public void An_order_needs_gpus_cash_and_an_open_era_and_only_one_at_a_time()
    {
        Game few = Lab(2);
        few.Refused(Command.OrderFabric(few.Id), CommandError.NotEnoughGpus);
        Game off = Lab(10, c => c.Fabric.Enabled = 0);
        off.Refused(Command.OrderFabric(off.Id), CommandError.InvalidArgument);
        Game late = Lab(10, c => { c.Fabric.EraA = c.Eras.Count - 1; c.Fabric.EraB = c.Eras.Count - 1; c.Fabric.EraC = c.Eras.Count - 1; });
        late.Refused(Command.OrderFabric(late.Id), CommandError.EraLocked);
        Game poor = Lab();
        poor.Me.Cash = 0;
        poor.Refused(Command.OrderFabric(poor.Id), CommandError.NotEnoughCash);
        Game g = Lab();
        g.Refused(Command.CancelFabric(g.Id), CommandError.InvalidArgument);
        g.Do(Command.OrderFabric(g.Id));
        g.Refused(Command.OrderFabric(g.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void The_deposit_is_paid_at_the_order_and_the_balance_at_delivery()
    {
        Game g = Lab();
        long cost = Fabric.Cost(g.C, g.State, g.Me, 1, Fabric.OwnedGpus(g.Me));
        long deposit = Fabric.Deposit(g.C, cost);
        Assert.True(deposit > 0 && deposit < cost);
        long before = g.Me.Ledger.Lifetime.Hardware;
        g.Do(Command.OrderFabric(g.Id));
        Assert.Equal(deposit, Paid(g, before));
        Assert.Equal(1, g.Me.FabBuild);
        Assert.Equal(g.C.Fabric.LeadA, g.Me.FabLeft);
        g.Weeks(g.C.Fabric.LeadA);
        Assert.Equal(1, g.Me.FabTier);
        Assert.Equal(0, g.Me.FabBuild);
        Assert.Equal(cost, Paid(g, before));
        Assert.Equal(1, g.Me.FabBuilt);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.FabricDelivered && e.SubjectId == 1);
    }

    [Fact]
    public void The_cluster_is_down_for_the_last_weeks_of_a_build()
    {
        Game g = Lab();
        int speed = TrainingSystem.SpeedPermille(g.C, g.Me);
        g.Do(Command.OrderFabric(g.Id));
        Assert.Equal(speed, TrainingSystem.SpeedPermille(g.C, g.Me));
        g.Weeks(g.C.Fabric.LeadA - g.C.Fabric.CutoverWeeks);
        Assert.True(Fabric.Cutover(g.C, g.Me));
        Assert.Equal(-g.C.Fabric.CutoverLoss, Fabric.Permille(g.C, g.Me));
        Assert.Equal(speed - g.C.Fabric.CutoverLoss, TrainingSystem.SpeedPermille(g.C, g.Me));
        g.Weeks(g.C.Fabric.CutoverWeeks);
        Assert.Equal(speed + g.C.Fabric.GainA, TrainingSystem.SpeedPermille(g.C, g.Me));
    }

    [Fact]
    public void A_delivery_waits_for_the_balance_and_the_cluster_stays_down()
    {
        Game g = Lab();
        g.Do(Command.OrderFabric(g.Id));
        g.Me.Cash = 0;
        g.Weeks(g.C.Fabric.LeadA + 2);
        Assert.Equal(1, g.Me.FabBuild);
        Assert.Equal(0, g.Me.FabLeft);
        Assert.Equal(0, g.Me.FabTier);
        Assert.Equal(-g.C.Fabric.CutoverLoss, Fabric.Permille(g.C, g.Me));
        g.Me.Cash = 1_000_000_000;
        g.Weeks(1);
        Assert.Equal(1, g.Me.FabTier);
        Assert.Equal(g.C.Fabric.GainA, Fabric.Permille(g.C, g.Me));
    }

    [Fact]
    public void Tiers_come_in_order_and_each_replaces_the_one_before()
    {
        Game g = Lab();
        for (int tier = 1; tier <= 3; tier++)
        {
            g.Do(Command.OrderFabric(g.Id));
            Assert.Equal(tier, g.Me.FabBuild);
            g.Weeks(Fabric.Lead(g.C, tier));
            Assert.Equal(tier, g.Me.FabTier);
            Assert.Equal(Fabric.Gain(g.C, tier), Fabric.Permille(g.C, g.Me));
        }
        Assert.True(Fabric.Gain(g.C, 3) > Fabric.Gain(g.C, 1));
        g.Refused(Command.OrderFabric(g.Id), CommandError.InvalidArgument);
        Assert.Equal(3, g.Me.FabBuilt);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void A_later_tier_waits_for_its_era()
    {
        Game g = Lab(10, c => { c.Fabric.EraB = c.Eras.Count - 1; c.Fabric.EraC = c.Eras.Count - 1; });
        g.Do(Command.OrderFabric(g.Id));
        g.Weeks(g.C.Fabric.LeadA);
        Assert.Equal(1, g.Me.FabTier);
        g.Refused(Command.OrderFabric(g.Id), CommandError.EraLocked);
    }

    [Fact]
    public void A_cancelled_order_loses_the_deposit_and_the_cost_is_fixed_when_ordered()
    {
        Game g = Lab();
        long before = g.Me.Ledger.Lifetime.Hardware;
        g.Do(Command.OrderFabric(g.Id));
        int gpus = g.Me.FabGpus;
        long deposit = Paid(g, before);
        g.Weeks(3);
        g.Do(Command.CancelFabric(g.Id));
        Assert.Equal(0, g.Me.FabBuild);
        Assert.Equal(0, g.Me.FabLeft);
        Assert.Equal(1, g.Me.FabCancelled);
        Assert.Equal(deposit, Paid(g, before));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.FabricCancelled && e.Amount == deposit);
        g.Do(Command.OrderFabric(g.Id));
        Assert.Equal(gpus, g.Me.FabGpus);
        Assert.Equal(g.C.Fabric.LeadA, g.Me.FabLeft);
        Assert.Empty(SaveGame.Problems(g.C, g.State));
    }

    [Fact]
    public void Saves_round_trip_in_the_middle_of_a_build_and_older_saves_have_no_fabric()
    {
        Game g = Lab();
        g.Do(Command.OrderFabric(g.Id));
        g.Weeks(4);
        Simulation back = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(g.Sim.Digest(), back.Digest());
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 112, g.Sim.Save()));
        LabState lab = older.State.Labs.First(l => l.Id == g.Id);
        Assert.Equal(0, lab.FabBuild);
        Assert.Equal(0, lab.FabTier);
        Assert.Empty(SaveGame.Problems(g.C, older.State));
    }
}
