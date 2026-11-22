using System.Collections.Generic;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Sixty-year full-stack autopilot runs replay from the journal on many seeds.</summary>
public class FullStackReplayDeterminismHardeningTests
{
    const int SixtyYearsWeeks = 60 * 48;

    public static IEnumerable<object[]> ReplaySeeds()
    {
        for (int seed = 1; seed <= 24; seed++)
            yield return new object[] { seed };
    }

    [Theory]
    [MemberData(nameof(ReplaySeeds))]
    public void Sixty_years_on_seed_replays_to_the_same_digest(int seed)
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
    [MemberData(nameof(ReplaySeeds))]
    public void Sixty_years_with_scattered_save_load_still_replays(int seed)
    {
        Game played = Game.New(seed);
        var loadWeeks = new Rng(seed ^ 0x71FE_2040L);
        for (int w = 0; w < SixtyYearsWeeks; w++)
        {
            if (GameDate.IsMonthStart(played.C.Calendar, played.Tick)) played.Sim.RunPlan(played.Id, played.C.Autopilot);
            played.Sim.Step();
            if ((loadWeeks.NextU64() & 63) == 0) played = new Game(Simulation.Load(played.C, played.Sim.Save()));
        }
        string digest = played.Sim.Digest();
        Simulation replayed = Simulation.Replay(played.C, played.Sim.Journal);
        Assert.Equal(digest, replayed.Digest());
    }
}