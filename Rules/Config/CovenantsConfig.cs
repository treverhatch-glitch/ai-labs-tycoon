using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Loan covenants. While the player's lab owes on loans against chips the lender watches how far the chips would cover the debt. Below the cover the lab is in breach: a surcharge is added each week and, after the cure weeks, the lender calls the loans. The lab can pay the debt down to cure the breach, or pay a waiver fee for a spell in which the lender looks away (a few times only). Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class CovenantsConfig : ISync
    {
        public int Enabled;
        /// <summary>The chips' resale value over what is owed, in permille, below which the lab is in breach.</summary>
        public int MinCoverPermille;
        /// <summary>Weeks in breach before the lender calls the loans.</summary>
        public int CureWeeks;
        /// <summary>Share of what is owed added each week in breach, in permille.</summary>
        public int SurchargePermille;
        /// <summary>Share of what is owed that a waiver costs, in permille.</summary>
        public int WaiverFeePermille;
        /// <summary>Weeks a waiver lasts.</summary>
        public int WaiverWeeks;
        /// <summary>Waivers the lender grants in all.</summary>
        public int MaxWaivers;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minCoverPermille", ref MinCoverPermille);
            s.Int("cureWeeks", ref CureWeeks);
            s.Int("surchargePermille", ref SurchargePermille);
            s.Int("waiverFeePermille", ref WaiverFeePermille);
            s.Int("waiverWeeks", ref WaiverWeeks);
            s.Int("maxWaivers", ref MaxWaivers);
        }
    }
}
