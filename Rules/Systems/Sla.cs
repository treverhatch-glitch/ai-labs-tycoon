using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The service-level pledge. See <see cref="SlaConfig"/>. Only the player's lab pledges.</summary>
    public static class Sla
    {
        public static bool Enabled(GameConfig c) { return c.Sla.MaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.SlaLevel, 0, c.Sla.MaxLevel); }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.SlaLockedUntil; }

        public static int PremiumPermille(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.Sla.PremiumPermillePerLevel); }

        public static int RefundPermille(GameConfig c, LabState lab) { return (int)MathI.Min(MathI.Permille, Level(c, lab) * (long)c.Sla.RefundPermillePerLevel); }

        public static int BreachReputation(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.Sla.BreachReputationPerLevel); }

        /// <summary>Why the week broke the promise: 0 for none, 1 for too little serving compute, 2 for an incident.</summary>
        public static int Breach(GameConfig c, GameState state, LabState lab)
        {
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Incidents > 0 && m.LastIncidentTick == state.Tick) return 2;
            }
            if (lab.LastWeek.ServingDemand > 0 && lab.LastWeek.ServedPermille < c.Sla.BreachBelowPermille) return 1;
            return 0;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            int was = Level(c, lab);
            lab.SlaLevel = level;
            lab.SlaLockedUntil = state.Tick + c.Sla.LockWeeks;
            lab.SlaStrikes = 0;
            GameEvent e = Events.Add(state, EventKind.SlaPledged, lab.Id, level, 0, "");
            e.Pick = level > was ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            int level = Level(c, lab);
            if (level == 0) { lab.SlaStrikes = 0; return; }
            long revenue = lab.LastWeek.Revenue;
            if (revenue <= 0) { lab.SlaStrikes = 0; return; }
            int why = Breach(c, state, lab);
            if (why == 0)
            {
                lab.SlaStrikes = 0;
                long premium = MathI.MulPermille(revenue, PremiumPermille(c, lab));
                if (premium > 0)
                {
                    Finance.Earn(lab, LedgerLine.Revenue, premium);
                    lab.LastWeek.Revenue += premium;
                }
                return;
            }
            long refund = MathI.MulPermille(revenue, RefundPermille(c, lab));
            if (refund > 0) Finance.Spend(lab, LedgerLine.Fines, refund);
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - BreachReputation(c, lab));
            lab.SlaStrikes++;
            GameEvent e = Events.Add(state, EventKind.SlaBroken, lab.Id, level, refund, "");
            e.Pick = why == 2 ? 1 : 0;
            if (c.Sla.StrikeWeeks > 0 && lab.SlaStrikes >= c.Sla.StrikeWeeks)
            {
                lab.SlaLevel = level - 1;
                lab.SlaStrikes = 0;
                lab.SlaLockedUntil = state.Tick + c.Sla.LockWeeks;
                Events.Add(state, EventKind.SlaPledged, lab.Id, level - 1, 0, "").Pick = 0;
            }
        }
    }
}
