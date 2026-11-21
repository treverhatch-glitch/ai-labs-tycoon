using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>Q221: prepaid distill runs from one rival — slots remaining until expiry tick.</summary>
    public sealed class DistillSlotLot : ISync
    {
        public int RivalLabId = -1;
        public string PackId = "";
        public int SlotsRemaining;
        public int ExpiresTick = -1;
        public int PurchasedTick;

        public void Sync(Syncer s)
        {
            s.Int("rivalLabId", ref RivalLabId);
            s.Str("packId", ref PackId);
            s.Int("slotsRemaining", ref SlotsRemaining);
            s.Int("expiresTick", ref ExpiresTick);
            s.Int("purchasedTick", ref PurchasedTick);
        }
    }
}