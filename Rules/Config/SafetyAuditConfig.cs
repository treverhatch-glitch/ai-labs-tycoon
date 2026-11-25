using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The external safety audit (bible ch. 10: outsiders check the lab's own numbers). The player pays outside auditors to test one model. They
    /// publish the real alignment, so the shown meter drops to it. A model whose numbers held up earns reputation and an audit certificate for a
    /// year, which eases government attention and hearings; one that was hiding a big gap costs reputation. An incident on a certified model
    /// pulls the certificate. Never run unless the player asks.
    /// </summary>
    public sealed class SafetyAuditConfig : ISync
    {
        /// <summary>Weeks a certificate lasts. Zero turns audits off.</summary>
        public int Weeks;
        /// <summary>Safety staff the lab needs on the payroll to give the auditors somebody to work with.</summary>
        public int MinSafetyStaff;
        /// <summary>The fee is this many weeks of the model's revenue.</summary>
        public int RevenueWeeks;
        /// <summary>But never less than this.</summary>
        public long MinCost;
        /// <summary>A gap this small or smaller counts as honest: the model is certified and the lab gains reputation.</summary>
        public int CleanGap;
        public int CleanReputation;
        /// <summary>A gap this big or bigger is a scandal when it is published.</summary>
        public int ShameGap;
        public int ShameReputation;
        public int CulturePoints;
        /// <summary>While any model holds a certificate: the share off the monthly attention gain, in permille.</summary>
        public int AttentionReliefPermille;
        /// <summary>While any model holds a certificate: reputation points added to how a hearing goes.</summary>
        public int HearingPoints;

        public void Sync(Syncer s)
        {
            s.Int("weeks", ref Weeks);
            s.Int("minSafetyStaff", ref MinSafetyStaff);
            s.Int("revenueWeeks", ref RevenueWeeks);
            s.Long("minCost", ref MinCost);
            s.Int("cleanGap", ref CleanGap);
            s.Int("cleanReputation", ref CleanReputation);
            s.Int("shameGap", ref ShameGap);
            s.Int("shameReputation", ref ShameReputation);
            s.Int("culturePoints", ref CulturePoints);
            s.Int("attentionReliefPermille", ref AttentionReliefPermille);
            s.Int("hearingPoints", ref HearingPoints);
        }
    }
}
