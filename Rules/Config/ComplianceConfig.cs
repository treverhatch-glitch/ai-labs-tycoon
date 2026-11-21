using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The compliance office (bible ch. 12: governments watch the labs, and a lab can meet them halfway). A standing legal and policy
    /// desk for a weekly retainer: it takes a share off every government's growing attention to the lab, off the fine at an audit and
    /// makes a hearing go better. Each level needs policy staff on the payroll, and a level the staff no longer cover is dropped.
    /// </summary>
    public sealed class ComplianceConfig : ISync
    {
        /// <summary>The highest level. Zero turns the office off.</summary>
        public int MaxLevel;
        /// <summary>Policy staff each level needs on the payroll.</summary>
        public int StaffPerLevel;
        /// <summary>The weekly retainer for each level, on the salaries ledger line.</summary>
        public long WeeklyCostPerLevel;
        /// <summary>The share each level takes off the attention a government gains each month (after policy staff have taken theirs), in permille.</summary>
        public int AttentionPermillePerLevel;
        /// <summary>The share each level takes off an audit fine, in permille.</summary>
        public int AuditFinePermillePerLevel;
        /// <summary>Reputation points each level adds to how a hearing goes (a smaller loss, or a bigger gain when the lab came prepared).</summary>
        public int HearingPointsPerLevel;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Int("staffPerLevel", ref StaffPerLevel);
            s.Long("weeklyCostPerLevel", ref WeeklyCostPerLevel);
            s.Int("attentionPermillePerLevel", ref AttentionPermillePerLevel);
            s.Int("auditFinePermillePerLevel", ref AuditFinePermillePerLevel);
            s.Int("hearingPointsPerLevel", ref HearingPointsPerLevel);
        }
    }
}
