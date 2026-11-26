using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum EnterpriseDealPhase
    {
        Prospect,
        Active,
        AtRisk,
        Churned,
        Completed,
    }

    /// <summary>Player enterprise seat: signed SLA tier, bound model, strike ladder until churn or renewal.</summary>
    public sealed class EnterpriseDealState : ISync
    {
        public string SectorId = "";
        public string TierId = "";
        public EnterpriseDealPhase Phase = EnterpriseDealPhase.Prospect;
        public int ModelId;
        public int SlaServedPermille;
        public int RevenueBonusPermille;
        public int StrikeLimit;
        public int Strikes;
        public int ProspectEndsTick = -1;
        public int EndsTick = -1;
        public int SignedTick;

        public void Sync(Syncer s)
        {
            s.Str("sectorId", ref SectorId);
            s.Str("tierId", ref TierId);
            s.Enum("phase", ref Phase);
            s.Int("modelId", ref ModelId);
            s.Int("slaServedPermille", ref SlaServedPermille);
            s.Int("revenueBonusPermille", ref RevenueBonusPermille);
            s.Int("strikeLimit", ref StrikeLimit);
            s.Int("strikes", ref Strikes);
            s.Int("prospectEndsTick", ref ProspectEndsTick);
            s.Int("endsTick", ref EndsTick);
            s.Int("signedTick", ref SignedTick);
        }
    }
}