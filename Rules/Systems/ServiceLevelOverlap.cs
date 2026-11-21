using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q214: shared served-rate signal between era service-level pledge (Q329) and enterprise seat deals (Q208).</summary>
    public static class ServiceLevelOverlap
    {
        /// <summary>
        /// True when the lab has an active pledge level and the same week already fails the pledge's served-rate bar.
        /// Incident-only pledge breaks (era) are not covered — enterprise deals may still strike.
        /// </summary>
        public static bool PledgeServedBreachSuppressesDealStrike(GameConfig c, LabState lab)
        {
            SlaConfig cfg = c.Sla;
            if (cfg == null || cfg.MaxLevel <= 0 || lab.SlaLevel <= 0) return false;
            if (lab.LastWeek.ServingDemand <= 0) return false;
            int bar = cfg.BreachBelowPermille;
            if (bar <= 0) bar = MathI.Permille;
            return lab.LastWeek.ServedPermille < bar;
        }
    }
}