using System;
using System.Linq;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Guards PR rules workflow filter: LongRunning replay matrix is opt-in via workflow_dispatch.</summary>
public class RulesCiSubsetHardeningTests
{
    static bool HasLongRunningTrait(Type t) => t.GetCustomAttributesData().Any(a =>
        a.AttributeType.Name == "TraitAttribute"
        && a.ConstructorArguments.Count >= 2
        && a.ConstructorArguments[0].Value as string == "Category"
        && a.ConstructorArguments[1].Value as string == "LongRunning");

    [Fact]
    public void Long_running_replay_hardening_classes_are_tagged_for_ci_subset()
    {
        Assert.True(HasLongRunningTrait(typeof(FullStackReplayDeterminismHardeningTests)));
        Assert.True(HasLongRunningTrait(typeof(FullLifeReplayDeterminismHardeningTests)));
    }
}