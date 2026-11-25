using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Q479 combination wiring for batch seventeen systems.</summary>
public class BatchSeventeenTogetherTests
{
    [Fact]
    public void Reserved_sign_with_reputation_gate()
    {
        Game g = Game.Solo(75, c =>
        {
            c.ReservedComputeGate.Enabled = 1;
            c.ReservedComputeGate.MinReputationPermille = 500;
            c.GpuTierUnlock.Enabled = 0;
        });
        g.Me.Reputation = 700;
        g.Do(Command.SignReservedCompute(g.Id, 2));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ReservedComputeSigned);
    }

    [Fact]
    public void Gpu_gate_with_buy()
    {
        Game g = Game.Solo(76, c =>
        {
            c.GpuTierUnlock.Enabled = 1;
            c.GpuTierUnlock.UnlockPermille = 800;
            c.GpuTierUnlock.MinEra = 0;
        });
        g.Build("server-closet", 2, 2);
        g.Build("server-closet", 2, 2);
        g.Me.Reputation = 850;
        g.Me.Cash = 500_000_000;
        g.Do(Command.BuyGpus(g.Id, 1));
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GpusBought);
    }

    [Fact]
    public void Scenario_win_keep_going_with_endless_flag()
    {
        Game g = Game.Solo(77, c => c.EndlessScenario.Enabled = 1);
        g.State.Endings.Add(new EndingRecord { EndingId = "scenario-win", LabId = g.Id, Tick = g.Tick });
        g.Do(Command.ContinueAfterEnding(g.Id, g.State.Endings.Count - 1, "keep-going"));
        Assert.Equal(g.Id, g.State.PlayerLabId);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.ScenarioEndlessAck);
    }
}