using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q489 combination wiring for batch twenty systems.</summary>
public class BatchTwentyTogetherTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(87, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            c.Safety.IncidentChancePermille = 0;
            c.EvalHarness.MinEra = 0;
            c.CampaignDeadline.MinEra = 0;
            c.OpenWeights.MinEra = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        g.Me.ResearchPoints = 1000;
        return g;
    }

    [Fact]
    public void Eval_boost_with_register_eval()
    {
        Game g = Lab(c =>
        {
            c.EvalHarness.Enabled = 1;
            c.EvalHarnessEraBoost.Enabled = 1;
            c.EvalHarnessEraBoost.MinEra = 0;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        g.Do(Command.RegisterEval(g.Id, m.Id, 1));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.EvalHarnessEraBoosted);
    }

    [Fact]
    public void Campaign_grace_extends_deadline_tick()
    {
        Game g = Lab(c =>
        {
            c.CampaignDeadline.Enabled = 1;
            c.CampaignDeadlineEraGrace.Enabled = 1;
            c.CampaignDeadlineEraGrace.MinEra = 0;
            c.CampaignDeadlineEraGrace.GraceWeeks = 4;
        });
        int baseTick = g.Tick + 10;
        g.Do(Command.SetCampaignDeadline(g.Id, baseTick));
        Assert.True(g.Me.CampaignDeadlineTick > baseTick);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.CampaignDeadlineElongated);
    }
}