using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class SpecialistBranchEraDiscount
    {
        public static bool Enabled(GameConfig c) { return c.SpecialistBranchEraDiscount.Enabled > 0 && SpecialistLines.Enabled(c); }

        public static long BranchCost(GameConfig c, GameState state)
        {
            long cost = c.SpecialistLines.Cost;
            if (!Enabled(c) || state == null || cost <= 0) return cost;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.SpecialistBranchEraDiscount.MinEra) return cost;
            int cut = c.SpecialistBranchEraDiscount.DiscountPermille;
            if (cut <= 0) return cost;
            return MathI.MulPermille(cost, MathI.Permille - cut);
        }

        public static void OnBranch(GameConfig c, GameState state, LabState lab, ModelState model, string kindId, long paid)
        {
            if (!Enabled(c) || paid >= c.SpecialistLines.Cost) return;
            long saved = c.SpecialistLines.Cost - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.SpecialistBranchEraDiscounted, lab.Id, model.Id, saved, kindId);
        }
    }
}