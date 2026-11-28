using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class OpenWeightsCopyEraSlack
    {
        public static bool Enabled(GameConfig c) { return c.OpenWeightsCopyEraSlack.Enabled > 0 && OpenWeights.Enabled(c); }

        public static int CopyTrustLoss(GameConfig c, GameState state, int loss)
        {
            if (!Enabled(c) || state == null || loss <= 0) return loss;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.OpenWeightsCopyEraSlack.MinEra) return loss;
            int cut = c.OpenWeightsCopyEraSlack.SlackPermille;
            if (cut <= 0) return loss;
            return (int)MathI.MulPermille(loss, MathI.Permille - cut);
        }

        public static void OnCopied(GameConfig c, GameState state, LabState lab, ModelState model, int listLoss, int applied)
        {
            if (!Enabled(c) || applied >= listLoss) return;
            int saved = listLoss - applied;
            if (saved <= 0) return;
            Events.Add(state, EventKind.OpenWeightsCopyEraSlacked, lab.Id, model.Id, saved, model.Name);
        }
    }
}