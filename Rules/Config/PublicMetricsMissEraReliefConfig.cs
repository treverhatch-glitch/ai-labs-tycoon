using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Softer trust hit when published metrics miss on incident in late era (Q528, schema 190).</summary>
    public sealed class PublicMetricsMissEraReliefConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int ReliefPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("reliefPermille", ref ReliefPermille);
        }
    }
}