using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Corporate tax. The player's lab owes a share of its weekly profit, paid at the year end. A tax structure (a holding company, or an offshore one) is built over some weeks for a fee, takes a share off the tax and adds a weekly fee and an exposure that builds toward a tax audit; claiming the research credit takes a share of the year's training spend off the bill but adds exposure too. An audit is answered by cooperating (pay back what was saved with interest and unwind the structure), settling (pay part and keep the structure) or contesting (the claim goes on the litigation docket). Unanswered, it is lost by default. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class TaxConfig : ISync
    {
        public int Enabled;
        /// <summary>Share of each week's profit owed as tax.</summary>
        public int RatePermille;
        /// <summary>Profit a week that is not taxed.</summary>
        public int AllowancePerWeek;
        /// <summary>What a holding company costs to set up.</summary>
        public int HoldingCost;
        /// <summary>Weeks it takes to set up.</summary>
        public int HoldingWeeks;
        /// <summary>Share of the tax it takes off.</summary>
        public int HoldingRelief;
        /// <summary>Exposure it adds each week.</summary>
        public int HoldingExposure;
        /// <summary>What an offshore structure costs to set up.</summary>
        public int OffshoreCost;
        /// <summary>Weeks it takes to set up.</summary>
        public int OffshoreWeeks;
        /// <summary>Share of the tax it takes off.</summary>
        public int OffshoreRelief;
        /// <summary>Exposure it adds each week.</summary>
        public int OffshoreExposure;
        /// <summary>Weekly fee for each tier of structure kept.</summary>
        public int FeePerTier;
        /// <summary>Share of the year's training spend the research credit takes off the bill.</summary>
        public int CreditPermille;
        /// <summary>Exposure a claim of the credit adds.</summary>
        public int ClaimExposure;
        /// <summary>Exposure at a year end that opens an audit.</summary>
        public int AuditThreshold;
        /// <summary>Weeks to answer an audit.</summary>
        public int AuditWeeks;
        /// <summary>Interest on what is paid back when the lab cooperates, in permille.</summary>
        public int CooperateInterest;
        /// <summary>Share of what is owed that settling costs.</summary>
        public int SettlePermille;
        /// <summary>Share of what is owed that the claim against the lab is for when it contests.</summary>
        public int ContestClaimPermille;
        /// <summary>Strength of the claim before the structure is counted.</summary>
        public int ContestStrength;
        /// <summary>Strength the claim gains for each tier of structure.</summary>
        public int StructureStrength;
        /// <summary>Extra share of what is owed that an unanswered audit costs.</summary>
        public int LapsePermille;
        /// <summary>Reputation an audit of an offshore structure costs, before the press damps it.</summary>
        public int ExposedReputation;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("ratePermille", ref RatePermille);
            s.Int("allowancePerWeek", ref AllowancePerWeek);
            s.Int("holdingCost", ref HoldingCost);
            s.Int("holdingWeeks", ref HoldingWeeks);
            s.Int("holdingRelief", ref HoldingRelief);
            s.Int("holdingExposure", ref HoldingExposure);
            s.Int("offshoreCost", ref OffshoreCost);
            s.Int("offshoreWeeks", ref OffshoreWeeks);
            s.Int("offshoreRelief", ref OffshoreRelief);
            s.Int("offshoreExposure", ref OffshoreExposure);
            s.Int("feePerTier", ref FeePerTier);
            s.Int("creditPermille", ref CreditPermille);
            s.Int("claimExposure", ref ClaimExposure);
            s.Int("auditThreshold", ref AuditThreshold);
            s.Int("auditWeeks", ref AuditWeeks);
            s.Int("cooperateInterest", ref CooperateInterest);
            s.Int("settlePermille", ref SettlePermille);
            s.Int("contestClaimPermille", ref ContestClaimPermille);
            s.Int("contestStrength", ref ContestStrength);
            s.Int("structureStrength", ref StructureStrength);
            s.Int("lapsePermille", ref LapsePermille);
            s.Int("exposedReputation", ref ExposedReputation);
        }
    }
}
