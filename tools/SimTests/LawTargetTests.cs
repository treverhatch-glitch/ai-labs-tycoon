using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class LawTargetTests
{
  [Fact]
    public void Shape_law_requires_target_first()
    {
        Game g = Game.Solo(57, c => { c.LawShaping.MinEra = 0; c.Lobbying.BackfirePermille = 0; c.Lobbying.BackfireFloorPermille = 0; });
        g.Me.Cash = 1_000_000_000_000;
        g.Refused(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict), CommandError.InvalidArgument);
        g.Do(Command.SetLawTarget(g.Id, LawShaping.Strict));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LawTargetSet);
        g.Do(Command.ShapeLaw(g.Id, g.Me.HomeBlocId, LawShaping.Strict));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.LawShaped);
    }

    [Fact]
    public void Save_at_147_has_no_law_target()
    {
        Game g = Game.Solo(58);
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 147, g.Sim.Save()));
        Assert.Equal("", older.Player.LawTarget);
    }
}