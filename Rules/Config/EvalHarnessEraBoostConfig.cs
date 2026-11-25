using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra trust when registering eval harness in a late era (Q487, schema 159).</summary>
    public sealed class EvalHarnessEraBoostConfig : ISync
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