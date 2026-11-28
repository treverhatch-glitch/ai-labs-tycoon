using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PublicMetricsConformityEraSlack
    {
        public static bool Enabled(GameConfig c) { return c.PublicMetricsConformityEraSlack.Enabled > 0 && PublicMetrics.Enabled(c); }

        public static int ConformityPush(GameConfig c, GameState state, int push)
        {
            if (!Enabled(c) || state == null || push <= 0) return push;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.PublicMetricsConformityEraSlack.MinEra) return push;
            int cut = c.PublicMetricsConformityEraSlack.SlackPermille;
            if (cut <= 0) return push;
            return (int)MathI.MulPermille(push, MathI.Permille - cut);
        }

        public static void OnSlack(GameConfig c, GameState state, LabState lab, ModelState model, int listPush, int applied)
        {
            if (!Enabled(c) || applied >= listPush) return;
            int saved = listPush - applied;
            if (saved <= 0) return;
            Events.Add(state, EventKind.PublicMetricsConformityEraSlacked, lab.Id, model.Id, saved, model.Name);
        }
    }
}