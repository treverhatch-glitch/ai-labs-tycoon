using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public struct ShadyOddsBreakdown
    {
        public int Base;
        public int FromCapability;
        public int FromHeat;
        public int FromDefense;

        public int SuccessPermille { get { return Base + FromCapability - FromHeat - FromDefense; } }
    }

    public static class ShadyQueries
    {
        public static bool CardOpen(GameConfig c, ShadyCardConfig card, int tick)
        {
            return GameDate.Year(c.Calendar, tick) >= card.OpensYear;
        }

        public static bool IsLocked(LabState lab, int tick)
        {
            return lab.ShadyLockedUntilTick >= 0 && tick < lab.ShadyLockedUntilTick;
        }

        public static bool CaseOpen(LabState lab, int tick)
        {
            return lab.CaseEndsTick >= 0 && tick < lab.CaseEndsTick;
        }

        public static bool ModelOnJob(LabState lab, int modelId)
        {
            for (int i = 0; i < lab.ShadyOps.Count; i++) if (lab.ShadyOps[i].ModelId == modelId) return true;
            return false;
        }

        public static int TargetDefense(GameConfig c, LabState target)
        {
            int staff = LabQueries.StaffByFunction(c, target, RoleFunction.Safety);
            int v = staff * c.Shady.TargetSafetyDefensePerStaff;
            return (int)MathI.Min(v, c.Shady.TargetSafetyDefenseCap);
        }

        public static ShadyOddsBreakdown SuccessOdds(GameConfig c, GameState state, LabState lab, ShadyCardConfig card, ModelState model, LabState target)
        {
            var b = new ShadyOddsBreakdown();
            b.Base = card.SuccessPermille;
            b.FromCapability = (int)(MathI.Max(0, model.Capability - card.MinCap) / 2);
            b.FromHeat = lab.Heat / 4;
            b.FromDefense = target == null ? 0 : TargetDefense(c, target);
            return b;
        }

        public static int ClampedSuccess(ShadyOddsBreakdown b)
        {
            return (int)MathI.Clamp(b.SuccessPermille, 50, 950);
        }

        public struct TracePermilleBreakdown
        {
            public int Base;
            public int FromHeat;
            public int Total { get { return Base + FromHeat; } }
        }

        public static TracePermilleBreakdown TracePermilleParts(GameConfig c, LabState lab, ShadyCardConfig card)
        {
            var b = new TracePermilleBreakdown { Base = card.TracePermille, FromHeat = lab.Heat / 5 };
            return b;
        }

        public static int TracePermille(GameConfig c, LabState lab, ShadyCardConfig card)
        {
            return (int)MathI.Clamp(TracePermilleParts(c, lab, card).Total, 0, 950);
        }

        public static int PolicyHeatDecay(GameConfig c, LabState lab)
        {
            int staff = WorldQueries.PolicyStaff(c, lab);
            if (staff > c.Shady.HeatPolicyCap) staff = c.Shady.HeatPolicyCap;
            return c.Shady.HeatDecayPerWeek + staff * c.Shady.HeatPolicyPerStaff;
        }

        public struct InvestigationRollBreakdown
        {
            public int Threshold;
            public int HeatOverThreshold;
            public int RollPermille;
        }

        public static InvestigationRollBreakdown InvestigationRollParts(GameConfig c, LabState lab)
        {
            var b = new InvestigationRollBreakdown { Threshold = c.Shady.InvestigationThreshold };
            if (lab.Heat < b.Threshold) return b;
            b.HeatOverThreshold = lab.Heat - b.Threshold;
            b.RollPermille = InvestigationOpenPermille(c, lab);
            return b;
        }

        public static int InvestigationOpenPermille(GameConfig c, LabState lab)
        {
            if (lab.Heat < c.Shady.InvestigationThreshold) return 0;
            return (int)(MathI.Max(0, lab.Heat - c.Shady.InvestigationThreshold) / c.Shady.InvestigationDivisor);
        }

        public struct ConvictionChanceBreakdown
        {
            public int FromHeat;
            public int FromPolicyStaff;
            public int Total;
        }

        public static ConvictionChanceBreakdown ConvictionChanceParts(GameConfig c, LabState lab)
        {
            return ConvictionChanceParts(c, null, lab);
        }

        public static ConvictionChanceBreakdown ConvictionChanceParts(GameConfig c, GameState state, LabState lab)
        {
            ShadyConfig cfg = c.Shady;
            var b = new ConvictionChanceBreakdown();
            b.FromHeat = lab.Heat / cfg.ConvictionDivisor;
            int staff = state == null ? WorldQueries.PolicyStaff(c, lab) : WorldQueries.EffectivePolicyStaff(c, state, lab);
            int cut = staff * cfg.ConvictionPolicyPerStaff;
            b.FromPolicyStaff = -cut;
            b.Total = (int)MathI.Clamp(b.FromHeat + b.FromPolicyStaff, 0, 900);
            return b;
        }

        public struct GreyComputePayoutBreakdown
        {
            public long InstalledKw;
            public long BilledKw;
            public long RatePerKw;
            public long Total;
        }

        public static GreyComputePayoutBreakdown GreyWeeklyPayoutParts(GameConfig c, GameState state, LabState lab, int holdingId)
        {
            var b = new GreyComputePayoutBreakdown();
            Holding h = lab.Holding(holdingId);
            if (h == null) return b;
            ShadyCardConfig card = c.Shady.Card("grey-compute");
            if (card == null) return b;
            b.InstalledKw = WorldQueries.InstalledKw(c, state, lab, holdingId);
            b.BilledKw = b.InstalledKw / 2;
            b.RatePerKw = card.BasePayout;
            b.Total = b.RatePerKw * b.BilledKw;
            return b;
        }

        public static long GreyWeeklyPayout(GameConfig c, GameState state, LabState lab, int holdingId)
        {
            return GreyWeeklyPayoutParts(c, state, lab, holdingId).Total;
        }
    }
}