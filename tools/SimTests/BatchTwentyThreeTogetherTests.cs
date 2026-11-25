using System;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q501 combination wiring for batch twenty-three systems.</summary>
public class BatchTwentyThreeTogetherTests
{
    static Game Lab(Action<GameConfig> tweak = null)
    {
        Game g = Game.Solo(94, c =>
        {
            foreach (StaffTraitConfig t in c.StaffPeople.Traits) t.HirePermille = 0;
            c.Office.ChancePermille = 0;
            if (tweak != null) tweak(c);
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Hearing_answer_fires_era_boost()
    {
        Game g = Lab(c =>
        {
            c.HearingSharePrice.MinFloatPpm = 1;
            c.HearingSharePriceEraBoost.Enabled = 1;
            c.HearingSharePriceEraBoost.MinEra = 0;
            c.HearingSharePriceEraBoost.BoostPermille = 40;
        });
        g.Me.PublicFloatPpm = 100_000;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        OfficeEventConfig q = g.C.Office.Event(g.Me.OfficeEventId);
        string choice = q.Choices.First(c => c.Effects.Any(f => f.Kind == OfficeEffectKind.Reputation && f.Amount > 0)).IdValue;
        g.Do(Command.AnswerOfficeEvent(g.Id, choice));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.HearingShareEraBoosted);
    }

    [Fact]
    public void Rung_six_cooldown_shortens_in_era()
    {
        Game g = Lab(c =>
        {
            c.RungSixCrisis.MinEra = 0;
            c.RungSixCrisis.CorruptionMin = 100;
            c.RungSixCrisis.CooldownWeeks = 20;
            c.RungSixEraCooldown.Enabled = 1;
            c.RungSixEraCooldown.MinEra = 0;
            c.RungSixEraCooldown.CooldownCutWeeks = 6;
        });
        ModelState m = g.Train("mini", 0);
        g.UntilTrained(m);
        g.Do(Command.ReleaseModel(g.Id, m.Id, 0, "game-ai"));
        m.Corruption = 800;
        OfficeEvents.OnEscape(g.C, g.State, g.Me);
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        RungSixCrisis.RunWeek(g.C, g.State, g.Me);
        OfficeEventConfig e = g.C.Office.Event(g.Me.OfficeEventId);
        g.Do(Command.RespondRungSix(g.Id, e.Choices[0].IdValue));
        Assert.Contains(g.State.Events, ev => ev.Kind == EventKind.RungSixEraCooldownShortened);
        Assert.Equal(g.Tick + 14, g.Me.RungSixCooldownUntil);
    }

    [Fact]
    public void Data_source_acquire_discount_event()
    {
        Game g = Lab(c =>
        {
            c.DataSources.Enabled = 1;
            c.DataSourceAcquireEraDiscount.Enabled = 1;
            c.DataSourceAcquireEraDiscount.MinEra = 0;
            c.DataSourceAcquireEraDiscount.DiscountPermille = 250;
        });
        string sid = g.C.DataSources.Sources[0].Id;
        long cash = g.Me.Cash;
        g.Do(Command.AcquireDataSource(g.Id, sid));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.DataSourceAcquireEraDiscounted);
        Assert.True(g.Me.Cash < cash);
    }
}