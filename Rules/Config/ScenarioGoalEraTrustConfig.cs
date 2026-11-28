using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Trust bump when a scenario goal is met in a late era (Q509, schema 177).</summary>
    public sealed class ScenarioGoalEraTrustConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int TrustPoints;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("trustPoints", ref TrustPoints);
        }
    }
}