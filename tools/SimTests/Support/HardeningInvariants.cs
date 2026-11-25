using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using Xunit;

namespace AiLabs.SimTests.Support;

/// <summary>Shared assertions for migration fuzz, cross-system soak and era edge hardening.</summary>
public static class HardeningInvariants
{
    public static void AssertBooksBalance(GameConfig c, GameState state)
    {
        foreach (LabState lab in state.Labs)
        {
            long expected = lab.Ledger.StartCash + lab.Ledger.Lifetime.Net;
            Assert.Equal(expected, lab.Cash);
        }
    }

    public static void AssertEconomyBounds(GameConfig c, GameState state)
    {
        const long cashCeiling = 1_000_000_000_000L;
        const long cashFloor = -1_000_000_000L;
        foreach (LabState lab in state.Labs)
        {
            Assert.InRange(lab.Cash, cashFloor, cashCeiling);
            Assert.InRange(lab.Reputation, 0, 1000);
            Assert.True(lab.Reserve >= 0);
            Assert.True(LabQueries.TotalStaff(lab) >= 0);
            Assert.All(lab.Staff, p => Assert.InRange(p.Morale, 0, 1000));
        }
        Assert.InRange(state.BubblePermille, 0, MathI.Permille);
    }

    public static void AssertNewEraStateBounds(GameConfig c, LabState lab)
    {
        if (c.Labour.Enabled != 0)
        {
            Assert.InRange(lab.LabourPressure, 0, 1000);
            Assert.InRange(lab.LabourStage, Labour.Quiet, Labour.Strike);
        }
        if (c.Accounts.Enabled != 0)
            Assert.All(lab.AccountTeams, t => Assert.InRange(t.People, 1, c.Accounts.MaxPeople));
        if (c.Conference.Enabled != 0) Assert.True(lab.Submissions.Count <= c.Conference.MaxPending);
        if (c.Alumni.Enabled != 0) Assert.True(lab.Alumni.Count <= c.Alumni.MaxSize);
        if (c.Pledge.MaxLevel > 0) Assert.InRange(lab.PledgeLevel, 0, c.Pledge.MaxLevel);
        if (c.Roadmap.MaxLevel > 0) Assert.InRange(lab.RoadmapLevel, 0, c.Roadmap.MaxLevel);
        if (c.Mentoring.Enabled != 0) Assert.InRange(Mentoring.Pairs(lab), 0, c.Mentoring.MaxPairs);
        foreach (ModelState m in lab.Models)
        {
            if (m.SunsetStart > 0) Assert.True(m.SunsetTick >= m.SunsetStart);
        }
    }

    public static void AssertAllLabs(GameConfig c, GameState state, bool eraBounds)
    {
        Assert.Empty(SaveGame.Problems(c, state));
        AssertBooksBalance(c, state);
        AssertEconomyBounds(c, state);
        if (!eraBounds) return;
        foreach (LabState lab in state.Labs)
            if (lab.Id == state.PlayerLabId) AssertNewEraStateBounds(c, lab);
    }

    public static void StepWeeksChecking(Simulation sim, int weeks, bool eraBounds)
    {
        for (int w = 0; w < weeks; w++)
        {
            sim.Step();
            AssertAllLabs(sim.Config, sim.State, eraBounds);
        }
    }
}