using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LawShapingEraAmplify
    {
        public static bool Enabled(GameConfig c) { return c.LawShapingEraAmplify.Enabled > 0 && LawShaping.Enabled(c); }

        public static int ShapeDelta(GameConfig c, GameState state, int delta)
        {
            if (!Enabled(c) || state == null || delta == 0) return delta;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.LawShapingEraAmplify.MinEra) return delta;
            int amp = c.LawShapingEraAmplify.AmplifyPermille;
            if (amp <= 0) return delta;
            return (int)MathI.MulPermille(delta, MathI.Permille + amp);
        }

        public static void OnShaped(GameConfig c, GameState state, LabState lab, string goal, int baseDelta, int shapedDelta)
        {
            if (!Enabled(c) || shapedDelta == baseDelta) return;
            Events.Add(state, EventKind.LawShapingEraAmplified, lab.Id, shapedDelta - baseDelta, baseDelta, goal);
        }
    }
}