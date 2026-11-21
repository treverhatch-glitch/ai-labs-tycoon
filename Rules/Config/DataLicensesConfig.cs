using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class DataLicenseTierConfig : ISync, IHasId
    {
        public string IdValue;
        public string Id { get { return IdValue; } }
        public int TermWeeks;
        public int StrikeLimit;
        public int PrepDiscountPermille;
        public int WeeklyResearchBonus;
        public long PursuitCash;
        public long SignCash;
        public long BreakPenalty;

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("termWeeks", ref TermWeeks);
            s.Int("strikeLimit", ref StrikeLimit);
            s.Int("prepDiscountPermille", ref PrepDiscountPermille);
            s.Int("weeklyResearchBonus", ref WeeklyResearchBonus);
            s.Long("pursuitCash", ref PursuitCash);
            s.Long("signCash", ref SignCash);
            s.Long("breakPenalty", ref BreakPenalty);
        }
    }

    public sealed class DataLicensesConfig : ISync
    {
        public int OpensYear;
        public int ProspectWindowWeeks;
        public int RevokedReputationPenalty;
        public int MinResearchers;
        public List<string> LicenseableSourceIds = new List<string>();
        public List<DataLicenseTierConfig> Tiers = new List<DataLicenseTierConfig>();

        public DataLicenseTierConfig Tier(string id)
        {
            for (int i = 0; i < Tiers.Count; i++) if (Tiers[i].Id == id) return Tiers[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("opensYear", ref OpensYear);
            s.Int("prospectWindowWeeks", ref ProspectWindowWeeks);
            s.Int("revokedReputationPenalty", ref RevokedReputationPenalty);
            s.Int("minResearchers", ref MinResearchers);
            s.StrList("licenseableSourceIds", ref LicenseableSourceIds);
            s.ObjList("tiers", ref Tiers);
        }
    }
}