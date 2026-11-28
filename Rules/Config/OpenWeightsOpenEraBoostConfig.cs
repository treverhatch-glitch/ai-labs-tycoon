using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra trust when opening weights in late era (Q529, schema 192).</summary>
    public sealed class OpenWeightsOpenEraBoostConfig : ISync
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