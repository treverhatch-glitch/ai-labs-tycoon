using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LawTargetEraAmplify
    {
        public static bool Enabled(GameConfig c) { return c.LawTargetEraAmplify.Enabled > 0 && LawTarget.Enabled(c); }

        public static void OnSet(GameConfig c, GameState state, LabState lab, string goal)
        {
            if (!Enabled(c)) return;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.LawTargetEraAmplify.MinEra) return;
            Events.Add(state, EventKind.LawTargetEraAmplified, lab.Id, era, goal == LawShaping.Strict ? 1 : 0, goal);
        }
    }
}