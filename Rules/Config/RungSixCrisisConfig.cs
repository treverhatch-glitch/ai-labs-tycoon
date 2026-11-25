using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Timed office follow-on after escape or takeoff arcs (Q470, schema 146).</summary>
    public sealed class RungSixCrisisConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        /// <summary>Corruption on a released model required to pose the crisis.</summary>
        public int CorruptionMin;
        /// <summary>Weeks after answering escape or takeoff during which the arc can trigger.</summary>
        public int ArcWeeks;
        /// <summary>Weeks between posed rung-six crises after one is answered.</summary>
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("corruptionMin", ref CorruptionMin);
            s.Int("arcWeeks", ref ArcWeeks);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}