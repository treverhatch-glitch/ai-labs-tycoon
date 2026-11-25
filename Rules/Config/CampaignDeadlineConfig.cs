using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Optional campaign run stop (Q462, schema 139).</summary>
    public sealed class CampaignDeadlineConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        /// <summary>Score bonus permille applied to run score at deadline.</summary>
        public int ScoreBonusPermille;
        /// <summary>Continuing ending id fired at deadline.</summary>
        public string EndingId = "campaign-stop";

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("scoreBonusPermille", ref ScoreBonusPermille);
            s.Str("endingId", ref EndingId);
        }
    }
}