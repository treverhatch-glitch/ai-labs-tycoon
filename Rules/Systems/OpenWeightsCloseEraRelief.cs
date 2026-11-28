using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class OpenWeightsCloseEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.OpenWeightsCloseEraRelief.Enabled > 0 && OpenWeights.Enabled(c); }

        public static int TrustLoss(GameConfig c, GameState state, int loss)
        {
            if (!Enabled(c) || state == null || loss >= 0) return loss;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.OpenWeightsCloseEraRelief.MinEra) return loss;
            int cut = c.OpenWeightsCloseEraRelief.ReliefPermille;
            if (cut <= 0) return loss;
            int mag = (int)MathI.MulPermille(-loss, MathI.Permille - cut);
            return -mag;
        }

        public static void OnClosed(GameConfig c, GameState state, LabState lab, ModelState model, int listLoss, int applied)
        {
            if (!Enabled(c) || applied == listLoss) return;
            int saved = System.Math.Abs(listLoss - applied);
            if (saved <= 0) return;
            Events.Add(state, EventKind.OpenWeightsCloseEraRelieved, lab.Id, model.Id, saved, model.Name);
        }
    }
}