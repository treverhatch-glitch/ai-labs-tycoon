using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Incident insurance for the player's lab. Each level covers a share of an incident's fine for a weekly premium that grows
    /// with the lab's revenue and with the claims the insurer remembers. The worst incidents are never covered, and a lab with
    /// too many recent claims loses its policy and cannot buy another until some are forgotten.
    /// </summary>
    public static class Insurance
    {
        public struct Quote
        {
            public int Level;
            public int CoverPermille;
            public long WeeklyPremium;
            public int Claims;
            public int LockedUntilTick;
            public bool CanBuyMore;
        }

        public static int CoverPermille(GameConfig c, LabState lab)
        {
            int level = (int)MathI.Clamp(lab.InsuranceLevel, 0, c.Finance.InsuranceMaxLevel);
            return (int)MathI.Min(MathI.Permille, (long)level * c.Finance.InsuranceCoverPermille);
        }

        /// <summary>What the policy costs this week: per level the larger of the minimum and a share of last week's revenue, loaded for remembered claims.</summary>
        public static long Premium(GameConfig c, LabState lab)
        {
            int level = (int)MathI.Clamp(lab.InsuranceLevel, 0, c.Finance.InsuranceMaxLevel);
            if (level == 0) return 0;
            long perLevel = MathI.Max(c.Finance.InsuranceMinPremium, MathI.MulPermille(MathI.Max(0, lab.Ledger.LastWeek.Revenue), c.Finance.InsurancePremiumPermille));
            long loading = MathI.Permille + (long)lab.InsuranceClaims * c.Finance.InsuranceClaimLoadingPermille;
            return MathI.MulPermille(perLevel * level, loading);
        }

        public static Quote Describe(GameConfig c, LabState lab)
        {
            var q = new Quote();
            q.Level = lab.InsuranceLevel;
            q.CoverPermille = CoverPermille(c, lab);
            q.WeeklyPremium = Premium(c, lab);
            q.Claims = lab.InsuranceClaims;
            q.LockedUntilTick = lab.InsuranceLockedUntil;
            q.CanBuyMore = lab.InsuranceLevel < c.Finance.InsuranceMaxLevel && lab.InsuranceClaims < c.Finance.InsuranceDropClaims;
            return q;
        }

        /// <summary>The weekly premium, and one claim forgotten every <c>insuranceForgetWeeks</c>.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.InsuranceClaims > 0 && (state.Tick + 1) % c.Finance.InsuranceForgetWeeks == 0) lab.InsuranceClaims--;
            long premium = Premium(c, lab);
            if (premium > 0) Finance.Spend(lab, LedgerLine.Insurance, premium);
        }

        /// <summary>An incident with this fine happened. Returns what the insurer pays (0 when the lab has no cover or the rung is excluded).</summary>
        public static long OnIncident(GameConfig c, GameState state, LabState lab, ModelState model, int rung, long fine)
        {
            if (lab.InsuranceLevel <= 0 || rung >= c.Finance.InsuranceExcludedRung) return 0;
            long paid = MathI.MulPermille(fine, CoverPermille(c, lab));
            lab.InsuranceClaims++;
            if (paid > 0) Events.Add(state, EventKind.InsuranceClaimPaid, lab.Id, model.Id, paid, model.Name);
            if (lab.InsuranceClaims >= c.Finance.InsuranceDropClaims)
            {
                lab.InsuranceLevel = 0;
                Events.Add(state, EventKind.InsuranceDropped, lab.Id, -1, lab.InsuranceClaims, "");
            }
            return paid;
        }
    }
}
