using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ComputeEquipment
    {
        public static bool Enabled(GameConfig c) { return c.ComputeEquipment.Enabled > 0 && c.ComputeEquipment.Tiers.Count > 0; }

        public static ComputeEquipmentTierConfig Current(GameConfig c, LabState lab) { return Tier(c, lab.ComputeEquipTier); }

        public static ComputeEquipmentTierConfig Tier(GameConfig c, int index) { return c.ComputeEquipment.Tier(index); }

        public static ComputeEquipmentTierConfig Next(GameConfig c, LabState lab)
        {
            int n = lab.ComputeEquipTier + 1;
            return Tier(c, n);
        }

        public static bool Unlocked(GameConfig c, GameState state, ComputeEquipmentTierConfig tier)
        {
            if (tier == null) return false;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            return era >= tier.MinEra;
        }

        public static int ResearchPermille(GameConfig c, LabState lab)
        {
            ComputeEquipmentTierConfig t = Current(c, lab);
            return t == null ? 0 : t.ResearchPermille;
        }

        public static int TrainingPermille(GameConfig c, LabState lab)
        {
            ComputeEquipmentTierConfig t = Current(c, lab);
            return t == null ? 0 : t.TrainingPermille;
        }

        public static void Upgrade(GameConfig c, GameState state, LabState lab)
        {
            ComputeEquipmentTierConfig next = Next(c, lab);
            Finance.Spend(lab, LedgerLine.CapitalIn, next.UpgradeCost);
            lab.ComputeEquipTier++;
            Events.Add(state, EventKind.ComputeEquipmentUpgraded, lab.Id, lab.ComputeEquipTier, next.UpgradeCost, next.IdValue);
        }
    }
}