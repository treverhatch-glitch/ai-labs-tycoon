using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class DistillSlotTests
{
    static Game At(int year = 2010)
    {
        var g = ExpansionTestSupport.DuoAtYear(year, tweak: c =>
        {
            c.DistillSlots.OpensYear = 2005;
            c.Training.DataSource("distill-rival").OpensYear = 0;
        });
        g.WithStarterLab(gpus: 24);
        g.Me.Cash += 2_000_000;
        return g;
    }

    static LabState PrepRival(Game g)
    {
        LabState rival = g.Rival("openaye");
        g.Try(Command.SetRentedGpus(rival.Id, 48));
        ModelState theirs = rival.Model(g.Try(Command.StartTraining(rival.Id, "mini", 40)).CreatedId);
        g.UntilTrained(theirs);
        g.Do(Command.ReleaseModel(rival.Id, theirs.Id, 1, "game-ai"));
        return rival;
    }

    [Fact]
    public void Shipped_distill_slots_config_validates()
    {
        ConfigValidator.Validate(TestData.Config());
    }

    [Fact]
    public void Before_opens_purchase_refused()
    {
        var g = ExpansionTestSupport.DuoAtYear(2003, tweak: c => c.DistillSlots.OpensYear = 2010).WithStarterLab();
        g.Refused(Command.PurchaseDistillSlots(g.Id, PrepRival(g).Id, "short"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Purchase_adds_lot_with_slots()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        Assert.Single(g.Me.DistillSlotLots);
        Assert.Equal(3, g.Me.DistillSlotLots[0].SlotsRemaining);
    }

    [Fact]
    public void Set_target_refused_without_slots()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Refused(Command.SetDistillTarget(g.Id, rival.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Start_training_consumes_one_slot()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        g.Do(Command.StartTraining(g.Id, "mini", 400));
        Assert.Equal(2, g.Me.DistillSlotLots[0].SlotsRemaining);
    }

    [Fact]
    public void Bulk_pack_more_slots_and_discount()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "bulk"));
        Assert.Equal(8, g.Me.DistillSlotLots[0].SlotsRemaining);
        Assert.Equal(g.C.DistillSlots.Pack("bulk").PrepDiscountPermille,
            DistillSlotSystem.PrepDiscountPermille(g.C, g.State, g.Me, rival.Id));
    }

    [Fact]
    public void Expired_lot_removed_and_event()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        g.Me.DistillSlotLots[0].ExpiresTick = g.Tick;
        DistillSlotSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.DistillSlotLots);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DistillSlotsExpired);
    }

    [Fact]
    public void Last_slot_exhausted_event()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        g.Me.DistillSlotLots[0].SlotsRemaining = 1;
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        g.Do(Command.StartTraining(g.Id, "mini", 400));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DistillSlotsExhausted);
    }

    [Fact]
    public void Bankrupt_refuses_purchase()
    {
        var g = At();
        g.Me.Bankrupt = true;
        g.Refused(Command.PurchaseDistillSlots(g.Id, PrepRival(g).Id, "short"), CommandError.LabBankrupt);
    }

    [Fact]
    public void Not_enough_cash_refused()
    {
        var g = At();
        g.Me.Cash = 0;
        g.Refused(Command.PurchaseDistillSlots(g.Id, PrepRival(g).Id, "short"), CommandError.NotEnoughCash);
    }

    [Fact]
    public void Schema_33_migration()
    {
        var g = At();
        g.Do(Command.PurchaseDistillSlots(g.Id, PrepRival(g).Id, "short"));
        var loaded = Simulation.Load(g.C, g.Sim.Save());
        Assert.Equal(34, loaded.State.Schema);
        Assert.Single(loaded.Player.DistillSlotLots);
    }

    [Fact]
    public void Save_roundtrip_lot()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        var loaded = new Game(Simulation.Load(g.C, g.Sim.Save()));
        Assert.Equal(3, loaded.Me.DistillSlotLots[0].SlotsRemaining);
    }

    [Fact]
    public void Rival_never_enabled()
    {
        var g = At();
        Assert.False(DistillSlotSystem.Enabled(g.C, g.State, g.Rival("openaye")));
    }

    [Fact]
    public void Before_opens_free_distill_target()
    {
        var g = ExpansionTestSupport.DuoAtYear(2003).WithStarterLab(gpus: 24);
        var rival = PrepRival(g);
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        Assert.Equal(rival.Id, g.Me.DistillTargetLabId);
    }

    [Fact]
    public void Slots_are_per_rival_not_transferable()
    {
        var g = At();
        var a = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, a.Id, "short"));
        g.Me.DistillSlotLots.Clear();
        g.Refused(Command.SetDistillTarget(g.Id, a.Id), CommandError.InvalidArgument);
    }

    [Fact]
    public void Multiple_lots_same_rival_uses_earliest_expiry()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "bulk"));
        Assert.Equal(2, g.Me.DistillSlotLots.Count);
        int firstExpiry = g.Me.DistillSlotLots.Min(l => l.ExpiresTick);
        Assert.Equal(firstExpiry, DistillSlotSystem.LotForRival(g.Me, rival.Id).ExpiresTick);
    }

    [Fact]
    public void Zero_slots_run_week_noop()
    {
        var g = At();
        DistillSlotSystem.RunWeek(g.C, g.State, g.Me);
        Assert.Empty(g.Me.DistillSlotLots);
    }

    [Fact]
    public void Self_purchase_refused()
    {
        var g = At();
        g.Refused(Command.PurchaseDistillSlots(g.Id, g.Id, "short"), CommandError.InvalidArgument);
    }

    [Fact]
    public void Pursue_researchers_required()
    {
        var g = ExpansionTestSupport.DuoAtYear(2010, tweak: c =>
        {
            c.DistillSlots.OpensYear = 2005;
            c.DistillSlots.MinResearchers = 4;
        }).WithStarterLab();
        g.Refused(Command.PurchaseDistillSlots(g.Id, PrepRival(g).Id, "short"), CommandError.NotEnoughStaff);
    }

    [Fact]
    public void Unknown_pack_refused()
    {
        var g = At();
        g.Refused(Command.PurchaseDistillSlots(g.Id, PrepRival(g).Id, "nope"), CommandError.UnknownId);
    }

    [Fact]
    public void Events_on_purchase()
    {
        var g = At();
        g.Do(Command.PurchaseDistillSlots(g.Id, PrepRival(g).Id, "short"));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DistillSlotsPurchased);
    }

    [Fact]
    public void Two_runs_need_two_slots()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        g.Do(Command.StartTraining(g.Id, "mini", 400));
        g.Do(Command.StartTraining(g.Id, "mini", 400));
        Assert.Equal(1, g.Me.DistillSlotLots[0].SlotsRemaining);
    }

    [Fact]
    public void Refused_when_slots_exhausted()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        g.Me.DistillSlotLots[0].SlotsRemaining = 0;
        g.Refused(Command.StartTraining(g.Id, "mini", 400), CommandError.InvalidArgument);
    }

    [Fact]
    public void Prep_cheaper_with_bulk_pack()
    {
        var g = At();
        var rival = PrepRival(g);
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "bulk"));
        long bulk = g.Me.Cash;
        g.Do(Command.SetDistillTarget(g.Id, rival.Id));
        g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        long bulkSpend = bulk - g.Me.Cash;
        g.Me.DistillSlotLots.Clear();
        g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
        long shortCash = g.Me.Cash;
        g.Do(Command.StartTraining(g.Id, "mini", 100));
        Assert.True(bulkSpend < shortCash - g.Me.Cash);
    }

  [Fact]
  public void Q227_distill_finish_does_not_replace_successor_trade()
  {
      var g = At();
      var rival = PrepRival(g);
      g.Do(Command.PurchaseDistillSlots(g.Id, rival.Id, "short"));
      g.Do(Command.SetDistillTarget(g.Id, rival.Id));
      g.Do(Command.SetTrainingDataSource(g.Id, "distill-rival"));
      ModelState m = g.Train("mini", 400);
      g.UntilTrained(m);
      int baseline = g.Me.Model(m.Id).Capability;
      Assert.True(baseline > 0);
      Assert.Equal(rival.Id, m.DistillSourceLabId);
      Assert.True(m.Capability >= baseline);
  }

    [Fact]
    public void Ten_years_deterministic()
    {
        string a = RunSoak(8181, 10);
        string b = RunSoak(8181, 10);
        Assert.Equal(a, b);
    }

    static string RunSoak(long seed, int years)
    {
        var options = new NewGameOptions { Seed = seed, StartEraId = "punchline" };
        var g = new Game(Simulation.NewGame(TestData.Config(c => c.DistillSlots.OpensYear = 2005), options));
        g.WithStarterLab(gpus: 16);
        int weeks = years * g.C.Calendar.WeeksPerYear;
        for (int i = 0; i < weeks; i++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        return g.Sim.Digest();
    }
}