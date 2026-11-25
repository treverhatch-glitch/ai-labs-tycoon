using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra trust on publish once an era unlocks public-metrics load (Q484, schema 157).</summary>
    public sealed class PublicMetricsEraLoadConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int TrustBoostPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("trustBoostPermille", ref TrustBoostPermille);
        }
    }
}