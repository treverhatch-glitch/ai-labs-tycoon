using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Era notice when committing a law-shaping target (Q494, schema 164).</summary>
    public sealed class LawTargetEraAmplifyConfig : ISync
    {
        public int Enabled;
        public int MinEra;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
        }
    }
}