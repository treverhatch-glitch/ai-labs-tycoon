using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class SpecialistBranchEraBoost
    {
        public static bool Enabled(GameConfig c) { return c.SpecialistBranchEraBoost.Enabled > 0 && SpecialistLines.Enabled(c); }

        public static void OnBranch(GameConfig c, GameState state, LabState lab, ModelState model, string kindId)
        {
            if (!Enabled(c) || state == null) return;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.SpecialistBranchEraBoost.MinEra) return;
            int boost = c.SpecialistBranchEraBoost.TrustBoostPermille;
            if (boost <= 0) return;
            for (int s = 0; s < model.Sectors.Count; s++)
                TrustSystem.Shift(c, state, lab, model.Sectors[s], boost, false);
            Events.Add(state, EventKind.SpecialistBranchEraBoosted, lab.Id, model.Id, boost, kindId);
        }
    }
}