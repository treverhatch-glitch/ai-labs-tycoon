using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ReservedComputeGateTests
{
    static Game Lab()
    {
        return Game.Solo(71, c =>
        {
            c.ReservedComputeGate.Enabled = 1;
            c.ReservedComputeGate.MinReputationPermille = 600;
            c.ReservedComputeGate.CooldownWeeks = 4;
        });
    }

    [Fact]
    public void Low_reputation_blocks_signing()
    {
        Game g = Lab();
        g.Me.Reputation = 100;
        g.Refused(Command.SignReservedCompute(g.Id, 4), CommandError.InvalidArgument);
    }

    [Fact]
    public void Successful_sign_sets_cooldown_and_event()
    {
        Game g = Lab();
        g.Me.Reputation = 800;
        int tick = g.Tick;
        g.Do(Command.SignReservedCompute(g.Id, 3));
        Assert.Equal(tick + g.C.ReservedComputeGate.CooldownWeeks, g.Me.ReservedSignCooldownUntil);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReservedComputeSigned);
    }

    [Fact]
    public void Cooldown_blocks_second_sign()
    {
        Game g = Lab();
        g.Me.Reputation = 800;
        g.Me.Cash = 50_000_000;
        g.Do(Command.SignReservedCompute(g.Id, 2));
        g.Refused(Command.SignReservedCompute(g.Id, 2), CommandError.InvalidArgument);
    }

    [Fact]
    public void Save_at_148_has_no_cooldown_field()
    {
        Game g = Game.Solo(71, c => c.ReservedComputeGate.Enabled = 0);
        Simulation older = Simulation.Load(g.C, SaveSchemaPretend.At(g.C, 148, g.Sim.Save()));
        Assert.Equal(0, older.Player.ReservedSignCooldownUntil);
    }
}