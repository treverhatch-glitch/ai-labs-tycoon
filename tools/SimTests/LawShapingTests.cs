using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class LawShapingTests
{
    static string Home(Game g) => g.Me.HomeBlocId;

    static Game Lab()
    {
        Game g = Game.Solo(42, c =>
        {
            c.LawShaping.MinEra = 0;
            c.Lobbying.CooldownWeeks = 1;
            c.Lobbying.BackfirePermille = 0;
            c.Lobbying.BackfireFloorPermille = 0;
        }).WithStarterLab();
        g.Me.Cash = 1_000_000_000_000;
        return g;
    }

    [Fact]
    public void Strict_shape_raises_release_bar_next_era()
    {
        Game g = Lab();
        int baseBar = EraRules.ReleaseBar(g.C, g.Tick);
        g.Do(Command.ShapeLaw(g.Id, Home(g), LawShaping.Strict));
        Assert.NotEqual(0, g.Me.LawShapePending);
        string eraId = EraSystem.AtTick(g.C, g.Tick).Id;
        LawShaping.OnEraBegan(g.C, g.State, g.Me, eraId);
        Assert.True(EraRules.ReleaseBar(g.C, g.State, g.Me, g.Tick) > baseBar);
    }

    [Fact]
    public void Cooling_down_blocks_second_shape()
    {
        Game g = Lab();
        g.Do(Command.ShapeLaw(g.Id, Home(g), LawShaping.Loose));
        g.Refused(Command.ShapeLaw(g.Id, Home(g), LawShaping.Loose), CommandError.LobbyCooling);
    }

    [Fact]
    public void Save_at_139_has_no_law_shape()
    {
        Game g = Lab();
        g.Do(Command.ShapeLaw(g.Id, Home(g), LawShaping.Strict));
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 139, g.Sim.Save()));
        Assert.Equal(0, older.Player.LawShapePending);
    }
}