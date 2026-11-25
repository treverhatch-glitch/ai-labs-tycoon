using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Open vs closed weights per model (Q459, schema 137). Player only.</summary>
    public sealed class OpenWeightsConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        /// <summary>Trust gained in every sector where the model sells when opened.</summary>
        public int TrustGain;
        /// <summary>Trust lost when competitors copy.</summary>
        public int CopyTrustLoss;
        public int CopyChancePermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("trustGain", ref TrustGain);
            s.Int("copyTrustLoss", ref CopyTrustLoss);
            s.Int("copyChancePermille", ref CopyChancePermille);
        }
    }
}