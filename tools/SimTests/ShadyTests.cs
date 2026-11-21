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

public class ShadyTests
{
    [Fact]
    public void Every_rival_plan_has_shady_appetite_zero()
    {
        GameConfig c = TestData.Config();
        foreach (RivalConfig rival in c.Rivals) Assert.Equal(0, rival.Plan.ShadyAppetite);
        ConfigValidator.Validate(c);
    }

    [Fact]
    public void Launder_moves_seventy_percent_to_cash_and_raises_heat()
    {
        var g = Game.Solo();
        g.Me.Stash = 50_000;
        g.Me.Heat = 0;
        long cashBefore = g.Me.Cash;
        g.Do(Command.Launder(g.Id, 30_000));
        Assert.Equal(20_000, g.Me.Stash);
        Assert.Equal(cashBefore + MathI.MulPermille(30_000, 700), g.Me.Cash);
        Assert.Equal(3, g.Me.Heat);
        Assert.Equal(MathI.MulPermille(30_000, 700), g.Me.Ledger.Lifetime.Laundered);
    }

    [Fact]
    public void Corruption_doubles_incidents_and_corrupted_mood_and_carryover()
    {
        var g = Game.Solo();
        var model = new ModelState { Alignment = 500, Corruption = 500, Status = ModelStatus.Released };
        int plain = SafetySystem.IncidentChancePermille(g.C, g.Tick, new ModelState { Alignment = 500, Corruption = 0, Status = ModelStatus.Released });
        int corrupt = SafetySystem.IncidentChancePermille(g.C, g.Tick, model);
        Assert.True(corrupt >= plain * 2 - 1);
        model.Corruption = g.C.Shady.CorruptedMood;
        Assert.Equal(ModelMood.Corrupted, LabQueries.Mood(g.C, g.Me, model, g.Tick));
        string line = g.C.Training.Lines[0].Id;
        g.Do(Command.StartTraining(g.Id, line, 100));
        var training = g.Me.Models.Last(m => m.Status == ModelStatus.Training);
        var prior = new ModelState { LineId = line, Version = training.Version - 1, Corruption = 400, Status = ModelStatus.Trained };
        g.Me.Models.Insert(0, prior);
        training.ProgressCompute = training.RequiredCompute;
        TrainingSystem.RunWeek(g.C, g.State, g.Me);
        var finished = g.Me.Models.Last(m => m.Status == ModelStatus.Trained && m.Id == training.Id);
        Assert.Equal((int)MathI.MulPermille(400, 500), finished.Corruption);
    }

    [Fact]
    public void Side_jobs_only_when_corruption_is_very_high()
    {
        var g = Game.Solo(seed: 101);
        var low = new ModelState { Corruption = 700, Status = ModelStatus.Released, Id = 1, LineId = "mini", Name = "Low" };
        var high = new ModelState { Corruption = 850, Status = ModelStatus.Released, Id = 2, LineId = "mini", Name = "High" };
        g.Me.Models.Add(low);
        g.Me.Models.Add(high);
        int lowHits = 0;
        int highHits = 0;
        for (long s = 1; s < 500; s++)
        {
            var state = g.State;
            state.Seed = s;
            state.Rng = RngSet.FromSeed(s);
            state.Tick = 1000;
            g.Me.Stash = 0;
            ShadySystem.RunWeek(g.C, state, g.Me);
            if (state.Events.Any(e => e.Kind == EventKind.ShadySideJob && e.SubjectId == 1)) lowHits++;
            if (state.Events.Any(e => e.Kind == EventKind.ShadySideJob && e.SubjectId == 2)) highHits++;
        }
        Assert.Equal(0, lowHits);
        Assert.True(highHits > 0);
    }

    [Fact]
    public void Success_odds_breakdown_sums_before_clamping()
    {
        var g = Game.Solo();
        ShadyCardConfig card = g.C.Shady.Card("liberate-funds");
        var model = new ModelState { Capability = 200, Status = ModelStatus.Trained };
        ShadyOddsBreakdown b = ShadyQueries.SuccessOdds(g.C, g.State, g.Me, card, model, null);
        Assert.Equal(b.Base + b.FromCapability - b.FromHeat - b.FromDefense, b.SuccessPermille);
        int clamped = ShadyQueries.ClampedSuccess(b);
        Assert.InRange(clamped, 0, 950);
        if (b.SuccessPermille > 0) Assert.True(clamped >= 50);
        else Assert.Equal(0, clamped);
    }

    [Fact]
    public void Investigation_rolls_never_open_below_heat_300()
    {
        var g = Game.Solo(seed: 1);
        g.Me.Heat = 299;
        for (int m = 0; m < 24; m++) { ShadySystem.RunMonth(g.C, g.State, g.Me); g.Weeks(g.C.Calendar.WeeksPerMonth); }
        Assert.Equal(-1, g.Me.CaseEndsTick);
    }

    [Fact]
    public void At_heat_700_about_ten_percent_of_months_open_a_case()
    {
        int hits = 0;
        for (long seed = 1; seed <= 100; seed++)
        {
            var g = Game.Solo(seed: seed);
            g.Me.Heat = 700;
            ShadySystem.RunMonth(g.C, g.State, g.Me);
            if (g.State.Events.Any(e => e.Kind == EventKind.ShadyInvestigation)) hits++;
        }
        Assert.InRange(hits, 5, 20);
    }

    [Fact]
    public void Conviction_records_handcuffs_and_a_second_one_can_follow()
    {
        long seed = 1;
        Game g = null;
        for (; seed < 500; seed++)
        {
            g = Game.Solo(seed: seed);
            g.Me.Heat = 900;
            g.Me.Cash = 1_000_000;
            g.Me.Stash = 500_000;
            g.Me.CaseEndsTick = g.Tick + g.C.Shady.CaseWeeks;
            g.State.Tick = g.Me.CaseEndsTick;
            g.State.Rng = RngSet.FromSeed(seed);
            ShadySystem.RunMonth(g.C, g.State, g.Me);
            if (g.State.Endings.Any(e => e.EndingId == "handcuffs")) break;
            g = null;
        }
        Assert.NotNull(g);
        Assert.Contains(g.State.Endings, e => e.EndingId == "handcuffs");
        int first = g.State.Endings.Count;
        bool second = false;
        for (long s = 1; s < 500; s++)
        {
            g.Me.Heat = 900;
            g.Me.Stash = 500_000;
            g.Me.CaseEndsTick = g.Tick + g.C.Shady.CaseWeeks;
            g.State.Tick = g.Me.CaseEndsTick;
            g.State.Rng = RngSet.FromSeed(s);
            ShadySystem.RunMonth(g.C, g.State, g.Me);
            if (g.State.Endings.Count(e => e.EndingId == "handcuffs") == first + 1) { second = true; break; }
        }
        Assert.True(second);
        Assert.Equal(first + 1, g.State.Endings.Count(e => e.EndingId == "handcuffs"));
        Assert.Equal(g.C.Endings.HandcuffsSuccessor, g.Me.Leader);
        Assert.Equal(0, g.Me.Stash);
    }

    [Fact]
    public void Borrow_weights_copies_at_eighty_five_seventy_and_corruption_150()
    {
        var g = Game.New(seed: 3);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 1998);
        g.State.PendingRivals.Clear();
        LabState rival = WorldSetup.AddRival(g.C, g.State, g.C.Rival("openaye"), EraSystem.AtTick(g.C, g.Tick), true);
        var released = new ModelState
        {
            Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = "Rival Mini", Status = ModelStatus.Released,
            CapBase = 1000, CapTech = 0, CapCarryover = 0, CapLuck = 0, Capability = 1000, Alignment = 800,
        };
        rival.Models.Add(released);
        var mine = new ModelState { Id = g.State.TakeId(), LineId = "mini", Version = 1, Name = "Mine", Status = ModelStatus.Trained, Capability = 200, Alignment = 400 };
        g.Me.Models.Add(mine);
        g.Me.Heat = 0;
        g.State.Rng = RngSet.FromSeed(99);
        g.Do(Command.StartShadyOp(g.Id, "borrow-weights", mine.Id, rival.Id, -1, "mini"));
        g.Until(() => g.Me.ShadyOps.Count == 0, 12, "the job");
        ModelState copy = g.Me.Models.Last(m => m.Status == ModelStatus.Trained && m.Id != mine.Id);
        Assert.Equal((int)MathI.MulPermille(1000, 850), copy.Capability);
        Assert.Equal(copy.CapBase + copy.CapTech + copy.CapCarryover, copy.Capability);
        Assert.Equal((int)MathI.MulPermille(800, 700), copy.Alignment);
        Assert.Equal(g.C.Shady.CopyCorruption, copy.Corruption);
    }

    [Fact]
    public void Sabotage_cuts_training_progress_and_traced_sabotage_hurts_every_rival()
    {
        var g = Game.New(seed: 4, tweak: c =>
        {
            c.Shady.Card("sabotage-run").SuccessPermille = 950;
            c.Shady.Card("sabotage-run").TracePermille = 0;
        });
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 1998);
        LabState rival = g.Rival("openaye");
        rival.PlanPausedUntilTick = g.Tick + 200;
        g.Do(Command.StartTraining(rival.Id, "mini", 100));
        ModelState run = rival.Models.Last(m => m.Status == ModelStatus.Training);
        run.ProgressCompute = 1000;
        run.RequiredCompute = 50_000;
        var model = new ModelState { Id = g.State.TakeId(), LineId = "mini", Capability = 500, Status = ModelStatus.Trained, Alignment = 500 };
        g.Me.Models.Add(model);
        g.Do(Command.StartShadyOp(g.Id, "sabotage-run", model.Id, rival.Id, -1, ""));
        g.Until(() => g.Me.ShadyOps.Count == 0, 8, "sabotage");
        Assert.Equal(300, run.ProgressCompute);

        var traced = Game.New(seed: 4, tweak: c =>
        {
            c.Shady.Card("sabotage-run").SuccessPermille = 950;
            c.Shady.Card("sabotage-run").TracePermille = 1000;
        });
        traced.State.Tick = GameDate.TickOfYear(traced.C.Calendar, 1998);
        LabState target = traced.Rival("openaye");
        target.PlanPausedUntilTick = traced.Tick + 200;
        traced.Do(Command.StartTraining(target.Id, "mini", 100));
        var atkModel = new ModelState { Id = traced.State.TakeId(), LineId = "mini", Capability = 500, Status = ModelStatus.Trained, Alignment = 500 };
        traced.Me.Models.Add(atkModel);
        int vendorBefore = traced.State.Vendor.Standing(traced.Id).Standing;
        int feelBefore = traced.Rival("anthoptics").FeelingToPlayer;
        traced.Do(Command.StartShadyOp(traced.Id, "sabotage-run", atkModel.Id, target.Id, -1, ""));
        traced.Until(() => traced.Me.ShadyOps.Count == 0, 8, "traced sabotage");
        Assert.True(traced.State.Vendor.Standing(traced.Id).Standing < vendorBefore);
        Assert.True(traced.Rival("anthoptics").FeelingToPlayer < feelBefore);
    }

    [Fact]
    public void Smuggle_moves_a_blocked_batch_and_refuses_normal_move()
    {
        var g = Game.Solo(seed: 7);
        g.State.Tick = GameDate.TickOfYear(g.C.Calendar, 2023);
        ChipDesign d = new ChipDesign { Id = 1, Target = "training", NodeId = "n5", Status = ChipDesignStatus.Ready };
        g.Me.ChipDesigns.Add(d);
        int hall = g.Do(Command.BuyLand(g.Id, "us-oregon")).CreatedId;
        g.Me.Holding(hall).HallKw = 5000;
        ChipSystem.AddChips(g.State, g.Me, hall, g.Id, d.Id, 10);
        HardwareBatch batch = g.Me.Fleet.Single(b => b.ChipDesignId >= 0);
        string chinaSite = g.C.World.Sites.First(s => s.BlocId == "china").Id;
        int china = g.Do(Command.BuyLand(g.Id, chinaSite)).CreatedId;
        int modelId = g.State.TakeId();
        g.Me.Models.Add(new ModelState { Id = modelId, Capability = 500, Status = ModelStatus.Trained });
        g.State.Rng = RngSet.FromSeed(7);
        g.Do(Command.StartShadyOp(g.Id, "smuggle-chips", modelId, -1, batch.Id, china.ToString()));
        g.Until(() => g.Me.ShadyOps.Count == 0, 8, "smuggle");
        Assert.Equal(china, batch.HoldingId);
        g.Refused(Command.MoveHardware(g.Id, batch.Id, hall, 1), CommandError.InvalidArgument);
    }

    [Fact]
    public void Cash_stays_explained_through_stash_and_launder()
    {
        var g = Game.Solo();
        long start = g.Me.Cash - g.Me.Ledger.Lifetime.Net;
        g.Me.Stash = 100_000;
        g.Do(Command.Launder(g.Id, 40_000));
        Assert.Equal(start + g.Me.Ledger.Lifetime.Net, g.Me.Cash);
        Assert.Equal(100_000 - 40_000, g.Me.Stash);
    }

    [Fact]
    public void A_save_with_an_op_and_an_open_case_loads_the_same()
    {
        var g = Game.Solo(seed: 88);
        var model = new ModelState { Id = g.State.TakeId(), Capability = 400, Status = ModelStatus.Trained, LineId = "mini" };
        g.Me.Models.Add(model);
        g.Do(Command.StartShadyOp(g.Id, "liberate-funds", model.Id, -1, -1, ""));
        g.Me.CaseEndsTick = g.Tick + 6;
        string save = g.Sim.Save();
        var loaded = Simulation.Load(g.C, save);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
        g.Weeks(20);
        loaded.StepMany(20);
        Assert.Equal(g.Sim.Digest(), loaded.Digest());
    }

    [Fact]
    public void Thirty_year_soak_has_no_shady_op_from_a_parody_lab()
    {
        var g = Game.New(seed: 55);
        for (int w = 0; w < 30 * g.C.Calendar.WeeksPerYear; w++)
        {
            if (GameDate.IsMonthStart(g.C.Calendar, g.Tick)) g.Sim.RunPlan(g.Id, g.C.Autopilot);
            g.Sim.Step();
        }
        Assert.All(g.State.Labs.Where(l => l.Controller == LabController.Plan), l => Assert.Empty(l.ShadyOps));
    }
}