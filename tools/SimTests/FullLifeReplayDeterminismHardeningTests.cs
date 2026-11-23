using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>From a 1995 start, a full multi-decade autopilot run replays from the same journal on several seeds.</summary>
public class FullLifeReplayDeterminismHardeningTests
{
    const int SixtyYearsWeeks = 60 * 48;

    [Theory]
    [InlineData(11)]
    [InlineData(17)]
    [InlineData(23)]
    public void Sixty_years_from_1995_replays_to_the_same_digest(int seed)
    {
        Game g = Game.New(seed);
        g.Sim.StepMany(SixtyYearsWeeks);
        string digest = g.Sim.Digest();
        Assert.Empty(SaveGame.Problems(g.C, g.State));
        HardeningInvariants.AssertBooksBalance(g.C, g.State);
        Simulation replayed = Simulation.Replay(g.C, g.Sim.Journal);
        Assert.Equal(digest, replayed.Digest());
    }

    [Theory]
    [InlineData(31)]
    [InlineData(41)]
    public void Sixty_years_with_random_mid_run_save_load_still_replays_the_same(int seed)
    {
        Game played = Game.New(seed);
        var loadWeeks = new Rng(seed ^ 0x71FE_1995L);
        played.Sim.StepMany(12);
        for (int w = 0; w < SixtyYearsWeeks - 12; w++)
        {
            if (GameDate.IsMonthStart(played.C.Calendar, played.Tick)) played.Sim.RunPlan(played.Id, played.C.Autopilot);
            played.Sim.Step();
            if ((loadWeeks.NextU64() & 31) == 0) played = new Game(Simulation.Load(played.C, played.Sim.Save()));
        }
        string digest = played.Sim.Digest();
        Simulation replayed = Simulation.Replay(played.C, played.Sim.Journal);
        Assert.Equal(digest, replayed.Digest());
    }
}