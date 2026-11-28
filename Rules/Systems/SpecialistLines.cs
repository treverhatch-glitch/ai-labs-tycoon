using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class SpecialistLines
    {
        public static bool Enabled(GameConfig c) { return c.SpecialistLines.Enabled > 0; }

        public static SpecialistKindConfig FindKind(GameConfig c, string id) => Kind(c, id);

        static SpecialistKindConfig Kind(GameConfig c, string id)
        {
            for (int i = 0; i < c.SpecialistLines.Kinds.Count; i++)
                if (c.SpecialistLines.Kinds[i].Id == id) return c.SpecialistLines.Kinds[i];
            return null;
        }

        static bool InSector(SpecialistKindConfig k, string sectorId)
        {
            for (int i = 0; i < k.Sectors.Count; i++) if (k.Sectors[i] == sectorId) return true;
            return false;
        }

        public static int AppealPermille(GameConfig c, ModelState model, string sectorId)
        {
            if (!Enabled(c) || model.SpecKind <= 0) return 0;
            int idx = model.SpecKind - 1;
            if (idx < 0 || idx >= c.SpecialistLines.Kinds.Count) return 0;
            SpecialistKindConfig k = c.SpecialistLines.Kinds[idx];
            return InSector(k, sectorId) ? c.SpecialistLines.BoostPermille : -c.SpecialistLines.PenaltyPermille;
        }

        public static void Branch(GameConfig c, GameState state, LabState lab, ModelState model, string kindId)
        {
            SpecialistKindConfig k = Kind(c, kindId);
            long cost = SpecialistBranchEraDiscount.BranchCost(c, state);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            model.SpecKind = (int)(c.SpecialistLines.Kinds.IndexOf(k) + 1);
            Events.Add(state, EventKind.SpecialistBranched, lab.Id, model.Id, model.SpecKind, model.Name);
            SpecialistBranchEraDiscount.OnBranch(c, state, lab, model, kindId, cost);
            SpecialistBranchEraBoost.OnBranch(c, state, lab, model, kindId);
        }
    }
}