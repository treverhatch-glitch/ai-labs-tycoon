using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class HearingSharePriceTests
{
    [Fact]
    public void Float_lab_nudges_bubble_after_hearing_answer()
    {
        Game g = Game.Solo(52, c => { c.Office.ChancePermille = 0; c.HearingSharePrice.MinFloatPpm = 1; });
        g.Me.Cash = 1_000_000_000_000;
        g.Me.PublicFloatPpm = 100_000;
        g.Me.Bloc("us").Level = BlocStanding.Letter;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        int bubble = g.State.BubblePermille;
        OfficeEventConfig q = g.C.Office.Event(g.Me.OfficeEventId);
        string choice = q.Choices.First(c => c.Effects.Any(f => f.Kind == OfficeEffectKind.Reputation && f.Amount > 0)).IdValue;
        g.Do(Command.AnswerOfficeEvent(g.Id, choice));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.HearingShareNudged);
        Assert.True(g.State.BubblePermille >= bubble);
    }

    [Fact]
    public void Private_lab_unchanged()
    {
        Game g = Game.Solo(53, c => { c.Office.ChancePermille = 0; });
        g.Me.PublicFloatPpm = 0;
        g.Me.Listed = 0;
        g.Me.Bloc("us").Attention = g.C.World.Attention.HearingAt + 80;
        GovernmentSystem.CloseMonth(g.C, g.State, g.Me);
        int bubble = g.State.BubblePermille;
        g.Do(Command.AnswerOfficeEvent(g.Id, g.C.Office.Event(g.Me.OfficeEventId).Choices[0].IdValue));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.HearingShareNudged);
        Assert.Equal(bubble, g.State.BubblePermille);
    }
}