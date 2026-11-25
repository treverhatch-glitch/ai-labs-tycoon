using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class EvalHarnessEraBoost
    {
        public static bool Enabled(GameConfig c) { return c.EvalHarnessEraBoost.Enabled > 0; }

        public static void OnRegister(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (!Enabled(c) || !EvalHarness.Enabled(c)) return;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.EvalHarnessEraBoost.MinEra) return;
            int boost = c.EvalHarnessEraBoost.TrustBoostPermille;
            if (boost <= 0) return;
            for (int s = 0; s < model.Sectors.Count; s++)
                TrustSystem.Shift(c, state, lab, model.Sectors[s], boost, false);
            Events.Add(state, EventKind.EvalHarnessEraBoosted, lab.Id, model.Id, boost, model.Name);
        }
    }
}