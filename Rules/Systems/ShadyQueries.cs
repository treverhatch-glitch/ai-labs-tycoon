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

        public static int TracePermille(GameConfig c, LabState lab, ShadyCardConfig card)
        {
            return (int)MathI.Clamp(card.TracePermille + lab.Heat / 5, 0, 950);
        }

        public static int PolicyHeatDecay(GameConfig c, LabState lab)
        {
            int staff = WorldQueries.PolicyStaff(c, lab);
            if (staff > c.Shady.HeatPolicyCap) staff = c.Shady.HeatPolicyCap;
            return c.Shady.HeatDecayPerWeek + staff * c.Shady.HeatPolicyPerStaff;
        }
    }
}