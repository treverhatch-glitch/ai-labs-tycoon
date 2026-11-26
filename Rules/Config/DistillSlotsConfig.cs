using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class DistillSlotPackConfig : ISync, IHasId
    {
        public string IdValue;
        public string Id { get { return IdValue; } }
        public int Slots;
        public int ValidWeeks;
        public long CashCost;
        public int PrepDiscountPermille;

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("slots", ref Slots);
            s.Int("validWeeks", ref ValidWeeks);
            s.Long("cashCost", ref CashCost);
            s.Int("prepDiscountPermille", ref PrepDiscountPermille);
        }
    }

    public sealed class DistillSlotsConfig : ISync
    {
        public int OpensYear;
        public int MinResearchers;
        public List<DistillSlotPackConfig> Packs = new List<DistillSlotPackConfig>();

        public DistillSlotPackConfig Pack(string id)
        {
            for (int i = 0; i < Packs.Count; i++) if (Packs[i].Id == id) return Packs[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("opensYear", ref OpensYear);
            s.Int("minResearchers", ref MinResearchers);
            s.ObjList("packs", ref Packs);
        }
    }
}