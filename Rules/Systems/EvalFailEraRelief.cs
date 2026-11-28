using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class EvalFailEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.EvalFailEraRelief.Enabled > 0 && EvalHarness.Enabled(c); }

        public static int TrustLoss(GameConfig c, GameState state, int loss)
        {
            if (!Enabled(c) || state == null || loss == 0) return loss;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.EvalFailEraRelief.MinEra) return loss;
            int cut = c.EvalFailEraRelief.ReliefPermille;
            if (cut <= 0) return loss;
            int sign = loss < 0 ? -1 : 1;
            int mag = (int)MathI.MulPermille(sign * loss, MathI.Permille - cut);
            return sign < 0 ? -mag : mag;
        }

        public static void OnFailed(GameConfig c, GameState state, LabState lab, ModelState model, int listLoss, int applied)
        {
            if (!Enabled(c) || applied == listLoss) return;
            int saved = System.Math.Abs(listLoss - applied);
            if (saved <= 0) return;
            Events.Add(state, EventKind.EvalFailEraRelieved, lab.Id, model.Id, saved, model.Name);
        }
    }
}