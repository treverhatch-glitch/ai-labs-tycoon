using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum DataLicensePhase
    {
        Prospect,
        Active,
        AtRisk,
        Revoked,
        Completed,
    }

    /// <summary>Q215: player data-source license — pursue, sign tier, weekly training-use obligation.</summary>
    public sealed class DataLicenseState : ISync
    {
        public string SourceId = "";
        public string TierId = "";
        public DataLicensePhase Phase = DataLicensePhase.Prospect;
        public int PrepDiscountPermille;
        public int WeeklyResearchBonus;
        public int StrikeLimit;
        public int Strikes;
        public int ProspectEndsTick = -1;
        public int EndsTick = -1;
        public int SignedTick;

        public void Sync(Syncer s)
        {
            s.Str("sourceId", ref SourceId);
            s.Str("tierId", ref TierId);
            s.Enum("phase", ref Phase);
            s.Int("prepDiscountPermille", ref PrepDiscountPermille);
            s.Int("weeklyResearchBonus", ref WeeklyResearchBonus);
            s.Int("strikeLimit", ref StrikeLimit);
            s.Int("strikes", ref Strikes);
            s.Int("prospectEndsTick", ref ProspectEndsTick);
            s.Int("endsTick", ref EndsTick);
            s.Int("signedTick", ref SignedTick);
        }
    }
}