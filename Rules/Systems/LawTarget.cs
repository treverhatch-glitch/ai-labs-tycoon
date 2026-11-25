using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LawTarget
    {
        public static bool Enabled(GameConfig c) { return c.LawTarget.Enabled > 0; }

        public static void Set(GameConfig c, GameState state, LabState lab, string goal)
        {
            lab.LawTarget = goal;
            Events.Add(state, EventKind.LawTargetSet, lab.Id, goal == LawShaping.Strict ? 1 : 0, 0, goal);
            LawTargetEraAmplify.OnSet(c, state, lab, goal);
        }
    }
}