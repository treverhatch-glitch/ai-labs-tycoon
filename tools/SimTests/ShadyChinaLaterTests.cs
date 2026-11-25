using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ShadyChinaLaterTests
{
    [Fact]
    public void Parody_labs_cannot_start_shady_jobs()
    {
        var g = Game.New();
        LabState rival = g.Rival("openaye");
        var model = new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained };
        rival.Models.Add(model);
        g.Refused(Command.StartShadyOp(rival.Id, "intrusion-for-hire", model.Id, -1, -1, ""), CommandError.InvalidArgument);
    }

    [Fact]
    public void A_successful_shady_job_adds_the_cards_corruption_to_the_model()
    {
        var g = Game.Solo(seed: 2, tweak: c =>
        {
            ShadyCardConfig card = c.Shady.Card("intrusion-for-hire");
            card.Weeks = 1;
            card.SuccessPermille = 1000;
            card.TracePermille = 0;
            card.Corruption = 180;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2000);
        var model = new ModelState { Id = g.State.TakeId(), Capability = 400, Status = ModelStatus.Trained, Corruption = 0 };
        g.Me.Models.Add(model);
        g.Do(Command.StartShadyOp(g.Id, "intrusion-for-hire", model.Id, -1, -1, ""));
        g.Until(() => g.Me.ShadyOps.Count == 0, 8, "shady");
        Assert.Equal(180, model.Corruption);
    }

    [Fact]
    public void A_failed_shady_job_adds_half_corruption()
    {
        var g = Game.Solo(seed: 3, tweak: c =>
        {
            ShadyCardConfig card = c.Shady.Card("intrusion-for-hire");
            card.Weeks = 1;
            card.SuccessPermille = 0;
            card.TracePermille = 0;
            card.MinCap = 400;
            card.Corruption = 200;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2000);
        var model = new ModelState { Id = g.State.TakeId(), Capability = 400, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        g.Do(Command.StartShadyOp(g.Id, "intrusion-for-hire", model.Id, -1, -1, ""));
        g.Until(() => g.Me.ShadyOps.Count == 0, 8, "shady");
        Assert.Equal(100, model.Corruption);
    }

    [Fact]
    public void A_new_version_inherits_carry_corruption_from_its_predecessor()
    {
        var g = Game.Solo().WithStarterLab();
        string line = g.C.Training.Lines[0].Id;
        var prev = new ModelState
        {
            Id = g.State.TakeId(), LineId = line, Version = 1, Status = ModelStatus.Retired, Corruption = 400
        };
        g.Me.Models.Add(prev);
        var run = g.Train(line, 80);
        g.UntilTrained(run);
        Assert.Equal(MathI.MulPermille(400, g.C.Shady.CarryCorruptionPermille), run.Corruption);
    }

    [Fact]
    public void High_corruption_marks_the_model_mood_corrupted()
    {
        var g = Game.Solo();
        var model = new ModelState { Corruption = g.C.Shady.CorruptedMood, Alignment = 500, DisplayedAlignment = 500, Status = ModelStatus.Released };
        g.Me.Models.Add(model);
        Assert.Equal(ModelMood.Corrupted, LabQueries.Mood(g.C, g.Me, model, g.Tick));
    }

    [Fact]
    public void A_side_job_raises_corruption_on_the_player_model_only()
    {
        var g = Game.Solo(seed: 9, tweak: c =>
        {
            c.Shady.SideJobFrom = 100;
            c.Shady.SideJobPermille = 1000;
        });
        var model = new ModelState { Id = g.State.TakeId(), Capability = 200, Status = ModelStatus.Released, Corruption = 200 };
        g.Me.Models.Add(model);
        ShadySystem.RunWeek(g.C, g.State, g.Me);
        Assert.True(model.Corruption >= 250);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ShadySideJob);
    }

    [Fact]
    public void Export_rules_still_block_legal_chip_orders_into_china()
    {
        var g = Game.Solo();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2026);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int china = g.Do(Command.BuyLand(g.Id, chinaSite)).CreatedId;
        g.Me.Holding(china).HallKw = 2000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 20_000_000);
        g.Refused(Command.BuyGpus(g.Id, 4, china), CommandError.ExportBlocked);
    }

    [Fact]
    public void Smuggling_still_moves_chips_past_export_rules()
    {
        var g = Game.Solo(seed: 19);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2024);
        ChipDesign d = new ChipDesign { Id = 1, Target = "training", NodeId = "n5", Status = ChipDesignStatus.Ready };
        g.Me.ChipDesigns.Add(d);
        int us = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        ChipSystem.AddChips(g.State, g.Me, us, g.Id, d.Id, 3);
        HardwareBatch batch = g.Me.Fleet.Single(b => b.ChipDesignId >= 0);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int china = g.Do(Command.BuyLand(g.Id, chinaSite)).CreatedId;
        var model = new ModelState { Id = g.State.TakeId(), Capability = 500, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        foreach (ShadyCardConfig card in g.C.Shady.Cards) if (card.Id == "smuggle-chips") { card.SuccessPermille = 1000; card.TracePermille = 0; card.Weeks = 1; }
        g.Do(Command.StartShadyOp(g.Id, "smuggle-chips", model.Id, -1, batch.Id, china.ToString()));
        g.Until(() => g.Me.ShadyOps.Count == 0, 8, "smuggle");
        Assert.Equal(china, batch.HoldingId);
    }

    [Fact]
    public void China_offers_a_deal_when_the_player_has_enough_footprint_there()
    {
        var g = Game.Solo(tweak: c => c.World.ChinaDeals.MinInstalledKw = 8);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2006);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int hall = g.Do(Command.BuyLand(g.Id, chinaSite)).CreatedId;
        g.Me.Holding(hall).HallKw = 500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 2_000_000);
        g.Do(Command.BuyGpus(g.Id, 8, hall));
        ChinaSystem.RunMonth(g.C, g.State, g.Me);
        Assert.True(g.Me.ChinaDealPending);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ChinaDealOffered);
    }

    [Fact]
    public void Accepting_the_China_deal_cuts_grid_power_there()
    {
        var g = Game.Solo();
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2006);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int hall = g.Do(Command.BuyLand(g.Id, chinaSite)).CreatedId;
        g.Me.Holding(hall).HallKw = 1000;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 2_000_000);
        g.Do(Command.BuyGpus(g.Id, 8, hall));
        g.Me.ChinaDealPending = true;
        g.Do(Command.AcceptChinaDeal(g.Id));
        PowerBill bill = WorldQueries.BillFor(g.C, g.State, g.Me, g.Me.Holding(hall));
        PowerBill full = new PowerBill();
        int kw = WorldQueries.InstalledKw(g.C, g.State, g.Me, hall);
        SiteConfig site = g.C.World.Site(chinaSite);
        full.GridCost = (long)kw * site.PowerPricePerKwWeek;
        Assert.Equal(MathI.MulPermille(full.GridCost, g.C.World.ChinaDeals.ActivePowerPermille), bill.GridCost);
    }

    [Fact]
    public void Declining_the_deal_still_raises_China_attention()
    {
        var g = Game.Solo();
        int before = g.Me.Bloc("china").Attention;
        g.Me.ChinaDealPending = true;
        g.Do(Command.DeclineChinaDeal(g.Id));
        Assert.False(g.Me.ChinaDealPending);
        Assert.Equal(before + g.C.World.ChinaDeals.DeclineAttention, g.Me.Bloc("china").Attention);
    }

    static void ChinaFootprint(Game g)
    {
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2006);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int hall = g.Do(Command.BuyLand(g.Id, chinaSite)).CreatedId;
        g.Me.Holding(hall).HallKw = 500;
        Finance.Earn(g.Me, LedgerLine.CapitalIn, 2_000_000);
        g.Do(Command.BuyGpus(g.Id, 8, hall));
    }

    [Fact]
    public void Declined_China_deal_stays_quiet_until_cooldown_ends()
    {
        var g = Game.Solo(tweak: c =>
        {
            c.World.ChinaDeals.MinInstalledKw = 8;
            c.World.ChinaDeals.DeclineCooldownMonths = 2;
            c.Reliability.Enabled = 0;
        });
        ChinaFootprint(g);
        ChinaSystem.RunMonth(g.C, g.State, g.Me);
        Assert.True(g.Me.ChinaDealPending);
        g.Do(Command.DeclineChinaDeal(g.Id));
        int wpMonth = g.C.Calendar.WeeksPerMonth;
        g.Weeks(wpMonth);
        if (GameDate.IsMonthEnd(g.C.Calendar, g.Tick - 1)) ChinaSystem.RunMonth(g.C, g.State, g.Me);
        Assert.False(g.Me.ChinaDealPending);
        while (g.Tick < g.Me.ChinaDealDeclinedUntilTick) g.Weeks(wpMonth);
        ChinaSystem.RunMonth(g.C, g.State, g.Me);
        Assert.True(g.Me.ChinaDealPending);
    }

    [Fact]
    public void Schema_18_migration_adds_china_decline_cooldown_tick()
    {
        var g = Game.Solo();
        string old = SaveSchemaPretend.At(g.C, 17, g.Sim.Save());
        Simulation sim = Simulation.Load(g.C, old);
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.Equal(-1, sim.Player.ChinaDealDeclinedUntilTick);
        SaveSchemaPretend.AssertRoundTrip(sim);
    }

    [Fact]
    public void Schema_14_migration_adds_china_deal_flags()
    {
        Simulation sim = Simulation.Load(TestData.Config(), MigrationTests.Schema2Save());
        Assert.Equal(GameState.CurrentSchema, sim.State.Schema);
        Assert.False(sim.Player.ChinaDealActive);
        Assert.False(sim.Player.ChinaDealPending);
    }
}