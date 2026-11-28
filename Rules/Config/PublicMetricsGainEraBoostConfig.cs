using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra trust on good public metrics in late era (Q540, schema 199).</summary>
    public sealed class PublicMetricsGainEraBoostConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int BoostPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("boostPermille", ref BoostPermille);
        }
    }
}