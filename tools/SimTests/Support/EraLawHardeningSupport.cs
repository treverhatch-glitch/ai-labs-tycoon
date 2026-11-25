using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;

namespace AiLabs.SimTests.Support;

/// <summary>Set law target when schema 148 law-target gate is on, then shape law.</summary>
public static class EraLawHardeningSupport
{
    public static void SetLawTarget(Game g, string goal)
    {
        if (LawTarget.Enabled(g.C))
            g.Do(Command.SetLawTarget(g.Id, goal));
    }

    public static void ShapeLaw(Game g, string goal, string blocId = null)
    {
        blocId ??= g.Me.HomeBlocId;
        if (string.IsNullOrEmpty(blocId)) return;
        if (LawTarget.Enabled(g.C) && g.Me.LawTarget != goal)
            g.Do(Command.SetLawTarget(g.Id, goal));
        g.Do(Command.ShapeLaw(g.Id, blocId, goal));
    }

    public static void TryShapeLaw(Game g, string goal, string blocId = null)
    {
        blocId ??= g.Me.HomeBlocId;
        if (string.IsNullOrEmpty(blocId)) return;
        if (LawTarget.Enabled(g.C) && g.Me.LawTarget != goal)
            g.Try(Command.SetLawTarget(g.Id, goal));
        g.Try(Command.ShapeLaw(g.Id, blocId, goal));
    }
}