using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra trust when branching a specialist line in late era (Q533, schema 195).</summary>
    public sealed class SpecialistBranchEraBoostConfig : ISync
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