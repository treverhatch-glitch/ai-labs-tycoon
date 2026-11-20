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
        int plain = SafetySystem.IncidentChancePermille(g.C, new ModelState { Alignment = 500, Corruption = 0, Status = ModelStatus.Released });
        int corrupt = SafetySystem.IncidentChancePermille(g.C, model);
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
    public void Success_odds_breakdown_matches_clamped_value()
    {
        var g = Game.Solo();
        ShadyCardConfig card = g.C.Shady.Card("liberate-funds");
        var model = new ModelState { Capability = 200, Status = ModelStatus.Trained };
        ShadyOddsBreakdown b = ShadyQueries.SuccessOdds(g.C, g.State, g.Me, card, model, null);
        Assert.Equal(ShadyQueries.ClampedSuccess(b), b.SuccessPermille < 50 ? 50 : b.SuccessPermille > 950 ? 950 : b.SuccessPermille);
    }
}