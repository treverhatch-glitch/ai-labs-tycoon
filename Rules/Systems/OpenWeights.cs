using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class OpenWeights
    {
        public const int Closed = 0;
        public const int Open = 1;

        public static bool Enabled(GameConfig c) { return c.OpenWeights.Enabled > 0; }

        public static int AppealPermille(GameConfig c, ModelState model, string sectorId)
        {
            if (!Enabled(c) || model.WgtOpen <= 0) return 0;
            for (int i = 0; i < model.Sectors.Count; i++)
                if (model.Sectors[i] == sectorId) return c.OpenWeights.TrustGain / 10;
            return 0;
        }

        public static void SetPolicy(GameConfig c, GameState state, LabState lab, ModelState model, int policy)
        {
            model.WgtOpen = policy > 0 ? Open : Closed;
            if (model.WgtOpen == Open)
            {
                for (int i = 0; i < model.Sectors.Count; i++)
                    TrustSystem.Shift(c, state, lab, model.Sectors[i], c.OpenWeights.TrustGain, false);
                Events.Add(state, EventKind.WeightsOpened, lab.Id, model.Id, c.OpenWeights.TrustGain, model.Name);
            }
            else Events.Add(state, EventKind.WeightsClosed, lab.Id, model.Id, 0, model.Name);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int m = 0; m < lab.Models.Count; m++)
            {
                ModelState model = lab.Models[m];
                if (model.WgtOpen != Open || model.Status != ModelStatus.Released) continue;
                int chance = OpenWeightsEraThrottle.EffectiveCopyChancePermille(c, state);
                if ((int)(state.Rng.World.NextU64() % MathI.Permille) >= chance) continue;
                for (int i = 0; i < model.Sectors.Count; i++)
                    TrustSystem.Shift(c, state, lab, model.Sectors[i], -c.OpenWeights.CopyTrustLoss, false);
                Events.Add(state, EventKind.WeightsCopied, lab.Id, model.Id, c.OpenWeights.CopyTrustLoss, model.Name);
                OpenWeightsEraThrottle.OnCopied(c, state, lab, model);
            }
        }
    }
}