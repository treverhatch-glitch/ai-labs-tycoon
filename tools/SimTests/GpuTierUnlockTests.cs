using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class GpuTierUnlockTests
{
    static Game Lab() => Game.Solo(72, c =>
    {
        c.GpuTierUnlock.Enabled = 1;
        c.GpuTierUnlock.UnlockPermille = 900;
        c.GpuTierUnlock.MinEra = 0;
        c.GpuTierUnlock.TierMin = 0;
    });

    [Fact]
    public void Low_reputation_blocks_buy()
    {
        Game g = Lab();
        g.Build("server-closet", 2, 2);
        g.Me.Reputation = 100;
        g.Me.Cash = 1_000_000_000;
        CommandResult buy = g.Sim.Apply(Command.BuyGpus(g.Id, 2));
        Assert.False(buy.Ok);
        Assert.Equal(CommandError.EraLocked, buy.Error);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GpuTierLocked);
    }

    [Fact]
    public void Unlocked_buy_succeeds()
    {
        Game g = Lab();
        g.Build("server-closet", 2, 2);
        g.Me.Reputation = 950;
        g.Me.Cash = 1_000_000_000;
        g.Do(Command.BuyGpus(g.Id, 1));
        Assert.DoesNotContain(g.State.Events, e => e.Kind == EventKind.GpuTierLocked);
    }

    [Fact]
    public void Queue_respects_tier_gate()
    {
        Game g = Lab();
        g.Me.Reputation = 100;
        g.Me.Cash = 1_000_000_000;
        CommandResult queue = g.Sim.Apply(Command.QueueGpuPurchase(g.Id, 1, HardwareBatch.Hq));
        Assert.False(queue.Ok);
        Assert.Equal(CommandError.EraLocked, queue.Error);
        Assert.Contains(g.State.Events, e => e.Kind == EventKind.GpuTierLocked);
    }
}