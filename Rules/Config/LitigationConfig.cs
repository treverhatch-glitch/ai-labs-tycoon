using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The litigation docket. Claims are filed against the player's lab: by people hurt in an incident, by people let go in a layoff round, by customers after a breach of security and by a challenge to a change of charter. Each has a strength fixed on the day it is filed. Before the hearing the lab can settle for a share of the claim, retain an outside firm to weaken the claim, or leave it to its in-house counsel. At the hearing the claim wins when its strength less the counsel's is at the judge's mark; a win is paid at once and can be appealed once. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class LitigationConfig : ISync
    {
        public int Enabled;
        /// <summary>Most claims open at once; further ones are not filed.</summary>
        public int MaxOpen;
        /// <summary>Weeks from filing to the hearing.</summary>
        public int HearingWeeks;
        /// <summary>The smallest claim that is filed.</summary>
        public int MinClaim;
        /// <summary>An incident claim, as a share of what the incident cost the lab.</summary>
        public int IncidentPermille;
        /// <summary>A dismissal claim, as a share of the severance a layoff round paid.</summary>
        public int DismissalPermille;
        /// <summary>What customers claim after a breach of security.</summary>
        public int PrivacyClaim;
        /// <summary>Settling before the hearing costs this share of the claim.</summary>
        public int SettlePermille;
        /// <summary>What the lab's own counsel takes off a claim's strength.</summary>
        public int InHouseStrength;
        /// <summary>What an outside firm takes off a claim's strength.</summary>
        public int FirmStrength;
        /// <summary>What retaining a firm costs, as a share of the claim.</summary>
        public int FirmFeePermille;
        /// <summary>A claim wins when its strength less the counsel's reaches this, from 1 to 1000.</summary>
        public int JudgeAt;
        /// <summary>What a lost claim costs, as a share of the claim.</summary>
        public int JudgmentPermille;
        /// <summary>What an appeal costs, as a share of the claim.</summary>
        public int AppealFeePermille;
        /// <summary>What an appeal takes off a claim's strength on top of the counsel's.</summary>
        public int AppealStrength;
        /// <summary>Weeks from a lost hearing to the payment, or to the appeal's ruling.</summary>
        public int AppealWeeks;
        /// <summary>Reputation a paid judgment costs, before the press damps it.</summary>
        public int LossReputation;
        /// <summary>Reputation a won claim gives back.</summary>
        public int WinReputation;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxOpen", ref MaxOpen);
            s.Int("hearingWeeks", ref HearingWeeks);
            s.Int("minClaim", ref MinClaim);
            s.Int("incidentPermille", ref IncidentPermille);
            s.Int("dismissalPermille", ref DismissalPermille);
            s.Int("privacyClaim", ref PrivacyClaim);
            s.Int("settlePermille", ref SettlePermille);
            s.Int("inHouseStrength", ref InHouseStrength);
            s.Int("firmStrength", ref FirmStrength);
            s.Int("firmFeePermille", ref FirmFeePermille);
            s.Int("judgeAt", ref JudgeAt);
            s.Int("judgmentPermille", ref JudgmentPermille);
            s.Int("appealFeePermille", ref AppealFeePermille);
            s.Int("appealStrength", ref AppealStrength);
            s.Int("appealWeeks", ref AppealWeeks);
            s.Int("lossReputation", ref LossReputation);
            s.Int("winReputation", ref WinReputation);
        }
    }
}
