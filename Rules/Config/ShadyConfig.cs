using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public enum ShadyPayout { None, Stash, Cash }

    public enum ShadySpecial
    {
        None,
        CopyModel,
        SabotageRun,
        HackGovernment,
        GreyCompute,
        SmuggleChips
    }

    public sealed class ShadyConfig : ISync
    {
        public List<ShadyCardConfig> Cards = new List<ShadyCardConfig>();
        public int HeatDecayPerWeek;
        public int HeatPolicyPerStaff;
        public int HeatPolicyCap;
        public int InvestigationThreshold;
        public int InvestigationDivisor;
        public int CaseWeeks;
        public int ConvictionDivisor;
        public int ConvictionPolicyPerStaff;
        public int AcquittalHeatDrop;
        public int LaunderKeepPermille;
        public long LaunderHeatPer;
        public int TargetSafetyDefensePerStaff;
        public int TargetSafetyDefenseCap;
        public int CorruptedMood;
        public int SideJobFrom;
        public int SideJobPermille;
        public int CarryCorruptionPermille;
        public int CorruptionIncidentWeight;
        public int CorruptionTrustWeight;
        public int TemptationRunwayWeeks;
        public int CopyCapabilityPermille;
        public int CopyAlignmentPermille;
        public int CopyCorruption;
        public int SabotageProgressPermille;
        public long HeatBuydownCashCost = 25_000;
        public int HeatBuydownAmount = 100;
        public int CancelShadyOpHeatPenalty = 50;
        /// <summary>Weekly stash storage fee as permille of balance (Q172).</summary>
        public int StashStoragePermillePerWeek;
        public long GreyReserveCashCost = 6_000;
        public int GreyReserveWeeks = 8;
        /// <summary>Extra grey payout permille while <see cref="LabState.GreyPayoutBoostUntilTick"/> is active (Q190).</summary>
        public int GreyReserveBoostPermille = 300;

        public ShadyCardConfig Card(string id)
        {
            for (int i = 0; i < Cards.Count; i++) if (Cards[i].Id == id) return Cards[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.ObjList("cards", ref Cards);
            s.Int("heatDecayPerWeek", ref HeatDecayPerWeek);
            s.Int("heatPolicyPerStaff", ref HeatPolicyPerStaff);
            s.Int("heatPolicyCap", ref HeatPolicyCap);
            s.Int("investigationThreshold", ref InvestigationThreshold);
            s.Int("investigationDivisor", ref InvestigationDivisor);
            s.Int("caseWeeks", ref CaseWeeks);
            s.Int("convictionDivisor", ref ConvictionDivisor);
            s.Int("convictionPolicyPerStaff", ref ConvictionPolicyPerStaff);
            s.Int("acquittalHeatDrop", ref AcquittalHeatDrop);
            s.Int("launderKeepPermille", ref LaunderKeepPermille);
            s.Long("launderHeatPer", ref LaunderHeatPer);
            s.Int("targetSafetyDefensePerStaff", ref TargetSafetyDefensePerStaff);
            s.Int("targetSafetyDefenseCap", ref TargetSafetyDefenseCap);
            s.Int("corruptedMood", ref CorruptedMood);
            s.Int("sideJobFrom", ref SideJobFrom);
            s.Int("sideJobPermille", ref SideJobPermille);
            s.Int("carryCorruptionPermille", ref CarryCorruptionPermille);
            s.Int("corruptionIncidentWeight", ref CorruptionIncidentWeight);
            s.Int("corruptionTrustWeight", ref CorruptionTrustWeight);
            s.Int("temptationRunwayWeeks", ref TemptationRunwayWeeks);
            s.Int("copyCapabilityPermille", ref CopyCapabilityPermille);
            s.Int("copyAlignmentPermille", ref CopyAlignmentPermille);
            s.Int("copyCorruption", ref CopyCorruption);
            s.Int("sabotageProgressPermille", ref SabotageProgressPermille);
            s.Long("heatBuydownCashCost", ref HeatBuydownCashCost);
            s.Int("heatBuydownAmount", ref HeatBuydownAmount);
            s.Int("cancelShadyOpHeatPenalty", ref CancelShadyOpHeatPenalty);
            s.Int("stashStoragePermillePerWeek", ref StashStoragePermillePerWeek);
            s.Long("greyReserveCashCost", ref GreyReserveCashCost);
            s.Int("greyReserveWeeks", ref GreyReserveWeeks);
            s.Int("greyReserveBoostPermille", ref GreyReserveBoostPermille);
        }
    }

    public sealed class ShadyCardConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public int OpensYear;
        public int MinCap;
        public int Weeks;
        public long BasePayout;
        public ShadyPayout PayoutTo;
        public int SuccessPermille;
        public int HeatOk;
        public int HeatFail;
        public int TracePermille;
        public int Corruption;
        public ShadySpecial Special;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("opensYear", ref OpensYear);
            s.Int("minCap", ref MinCap);
            s.Int("weeks", ref Weeks);
            s.Long("basePayout", ref BasePayout);
            s.Enum("payoutTo", ref PayoutTo);
            s.Int("successPermille", ref SuccessPermille);
            s.Int("heatOk", ref HeatOk);
            s.Int("heatFail", ref HeatFail);
            s.Int("tracePermille", ref TracePermille);
            s.Int("corruption", ref Corruption);
            s.Enum("special", ref Special);
        }
    }
}