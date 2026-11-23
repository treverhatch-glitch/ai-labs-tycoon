using System.Collections.Generic;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ScenarioBaggageTests
{
    [Fact]
    public void Pick_baggage_before_play()
    {
        Game g = Game.Solo(45, c => c.ScenarioBaggage.Enabled = 1);
        g.Refused(Command.PickScenarioBaggage(g.Id, "nope"), CommandError.UnknownId);
        g.Do(Command.PickScenarioBaggage(g.Id, "early-lawsuit"));
        Assert.Equal("early-lawsuit", g.Me.BaggageId);
        Assert.True(g.Me.Cases.Count > 0);
    }

    [Fact]
    public void New_game_options_apply_baggage()
    {
        var options = new NewGameOptions { Seed = 45, RivalIds = new List<string>(), BaggageId = "reputational-leak" };
        Simulation sim = Simulation.NewGame(TestData.Config(), options);
        Assert.Equal("reputational-leak", sim.Player.BaggageId);
        Assert.True(sim.Player.Reputation < TestData.Config().PlayerStart.Reputation);
    }

    [Fact]
    public void Save_at_142_has_no_baggage()
    {
        Game g = Game.Solo(45, c => c.ScenarioBaggage.Enabled = 1);
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 142, g.Sim.Save()));
        Assert.Equal("", older.Player.BaggageId);
    }
}