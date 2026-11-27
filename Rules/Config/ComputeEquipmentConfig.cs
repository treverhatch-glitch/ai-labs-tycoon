using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Era-gated workstation tiers: visual ids and small research/training bonuses (Q506, schema 173).</summary>
    public sealed class ComputeEquipmentConfig : ISync
    {
        public int Enabled;
        public List<ComputeEquipmentTierConfig> Tiers = new List<ComputeEquipmentTierConfig>();

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.ObjList("tiers", ref Tiers);
        }

        public ComputeEquipmentTierConfig Tier(int index)
        {
            if (index < 0 || index >= Tiers.Count) return null;
            return Tiers[index];
        }
    }

    public sealed class ComputeEquipmentTierConfig : ISync, IHasId
    {
        public string IdValue;
        public int MinEra;
        public long UpgradeCost;
        public int ResearchPermille;
        public int TrainingPermille;
        /// <summary>Art era suffix (1 = beige ninety-five kit).</summary>
        public int VisualEra;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("minEra", ref MinEra);
            s.Long("upgradeCost", ref UpgradeCost);
            s.Int("researchPermille", ref ResearchPermille);
            s.Int("trainingPermille", ref TrainingPermille);
            s.Int("visualEra", ref VisualEra);
        }
    }
}