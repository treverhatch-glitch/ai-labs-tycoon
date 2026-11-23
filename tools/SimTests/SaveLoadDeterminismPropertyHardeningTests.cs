using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Property-style check: autopilot with random mid-run save/load matches uninterrupted play for the same seed.</summary>
public class SaveLoadDeterminismPropertyHardeningTests
{
    static void RunProperty(long masterSeed, int warmupWeeks)
    {
        var master = new Rng(masterSeed);
        long gameSeed = (long)master.NextU64() % 100_000 + 1;
        int weeks = warmupWeeks + 80 + (int)(master.NextU64() % 120);

        Game straight = Game.New(gameSeed);
        Game choppy = Game.New(gameSeed);
        var loadRng = new Rng(masterSeed ^ gameSeed ^ 0x5A11_5A42L);

        for (int w = 0; w < weeks && !straight.Me.Bankrupt && !choppy.Me.Bankrupt; w++)
        {
            straight.Autopilot(1);
            choppy.Autopilot(1);
            if (w >= warmupWeeks && (loadRng.NextU64() & 15) == 0)
                choppy = new Game(Simulation.Load(choppy.C, choppy.Sim.Save()));
        }

        Assert.Equal(straight.Sim.Digest(), choppy.Sim.Digest());
        Assert.Equal(straight.Tick, choppy.Tick);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(202)]
    [InlineData(303)]
    [InlineData(404)]
    [InlineData(505)]
  public void Random_mid_run_saves_match_straight_autopilot_for_five_master_seeds(int masterSeed)
    {
        RunProperty(masterSeed, warmupWeeks: 0);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(22)]
    [InlineData(33)]
    public void Random_saves_after_a_warmup_month_still_match(int masterSeed)
    {
        RunProperty(masterSeed, warmupWeeks: 12);
    }

    [Fact]
    public void Two_full_replays_of_the_same_master_seed_agree()
    {
        RunProperty(909, warmupWeeks: 8);
        RunProperty(909, warmupWeeks: 8);
    }
}