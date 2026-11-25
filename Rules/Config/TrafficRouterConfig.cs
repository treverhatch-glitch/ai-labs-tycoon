using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Traffic router (bible ch. 6): split serving demand across released models. Skewed routes overload compute and add incident risk when the lab cannot serve. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class TrafficRouterConfig : ISync
    {
        public int Enabled;
        /// <summary>First era index for traffic routing.</summary>
        public int MinEra;
        /// <summary>Share of total route ppm above which traffic counts as skewed (ppm).</summary>
        public int SkewBar;
        /// <summary>Extra weekly incident permille when skewed and under-served.</summary>
        public int OverloadAdd;
        /// <summary>Served permille at or below this counts as under-served.</summary>
        public int ServedBelow;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("skewBar", ref SkewBar);
            s.Int("overloadAdd", ref OverloadAdd);
            s.Int("servedBelow", ref ServedBelow);
        }
    }
}