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
            bool wasOpen = model.WgtOpen == Open;
            model.WgtOpen = policy > 0 ? Open : Closed;
            if (model.WgtOpen == Open)
            {
                for (int i = 0; i < model.Sectors.Count; i++)
                    TrustSystem.Shift(c, state, lab, model.Sectors[i], c.OpenWeights.TrustGain, false);
                Events.Add(state, EventKind.WeightsOpened, lab.Id, model.Id, c.OpenWeights.TrustGain, model.Name);
                OpenWeightsOpenEraBoost.OnOpened(c, state, lab, model);
            }
            else
            {
                if (wasOpen && c.OpenWeights.TrustGain > 0)
                {
                    int listLoss = -c.OpenWeights.TrustGain;
                    int applied = OpenWeightsCloseEraRelief.TrustLoss(c, state, listLoss);
                    for (int i = 0; i < model.Sectors.Count; i++)
                        TrustSystem.Shift(c, state, lab, model.Sectors[i], applied, false);
                    OpenWeightsCloseEraRelief.OnClosed(c, state, lab, model, listLoss, applied);
                }
                Events.Add(state, EventKind.WeightsClosed, lab.Id, model.Id, 0, model.Name);
            }
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
                int listLoss = c.OpenWeights.CopyTrustLoss;
                int applied = OpenWeightsCopyEraRelief.CopyTrustLoss(c, state, listLoss);
                for (int i = 0; i < model.Sectors.Count; i++)
                    TrustSystem.Shift(c, state, lab, model.Sectors[i], -applied, false);
                Events.Add(state, EventKind.WeightsCopied, lab.Id, model.Id, applied, model.Name);
                OpenWeightsEraThrottle.OnCopied(c, state, lab, model);
                OpenWeightsCopyEraRelief.OnCopied(c, state, lab, model, listLoss, applied);
            }
        }
    }
}