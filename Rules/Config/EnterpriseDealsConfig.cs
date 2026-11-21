using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class EnterpriseDealTierConfig : ISync, IHasId
    {
        public string IdValue;
        public string Id { get { return IdValue; } }
        public int SlaServedPermille;
        public int RevenueBonusPermille;
        public int TermWeeks;
        public int StrikeLimit;
        public long PursuitCash;
        public long SignCash;
        public long BreakPenalty;

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("slaServedPermille", ref SlaServedPermille);
            s.Int("revenueBonusPermille", ref RevenueBonusPermille);
            s.Int("termWeeks", ref TermWeeks);
            s.Int("strikeLimit", ref StrikeLimit);
            s.Long("pursuitCash", ref PursuitCash);
            s.Long("signCash", ref SignCash);
            s.Long("breakPenalty", ref BreakPenalty);
        }
    }

    public sealed class EnterpriseDealsConfig : ISync
    {
        public int OpensYear;
        public int ProspectWindowWeeks;
        public int ChurnReputationPenalty;
        public int MinSalesStaff;
        public List<string> EligibleSectorIds = new List<string>();
        public List<EnterpriseDealTierConfig> Tiers = new List<EnterpriseDealTierConfig>();

        public EnterpriseDealTierConfig Tier(string id)
        {
            for (int i = 0; i < Tiers.Count; i++) if (Tiers[i].Id == id) return Tiers[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("opensYear", ref OpensYear);
            s.Int("prospectWindowWeeks", ref ProspectWindowWeeks);
            s.Int("churnReputationPenalty", ref ChurnReputationPenalty);
            s.Int("minSalesStaff", ref MinSalesStaff);
            s.StrList("eligibleSectorIds", ref EligibleSectorIds);
            s.ObjList("tiers", ref Tiers);
        }
    }
}