using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Cut sector-pricing overload in a late era (Q492, schema 163).</summary>
    public sealed class SectorPricingEraReliefConfig : ISync
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