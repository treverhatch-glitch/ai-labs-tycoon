using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Stronger law-shape delta in late era (Q516, schema 181).</summary>
    public sealed class LawShapingEraAmplifyConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int AmplifyPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("amplifyPermille", ref AmplifyPermille);
        }
    }
}