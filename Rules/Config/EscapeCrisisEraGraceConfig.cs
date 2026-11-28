using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra weeks to answer escape crises in a late era (Q515, schema 181).</summary>
    public sealed class EscapeCrisisEraGraceConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int GraceWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("graceWeeks", ref GraceWeeks);
        }
    }
}