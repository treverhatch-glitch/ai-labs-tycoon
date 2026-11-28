using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra weeks to answer release notes in a late era (Q512, schema 179).</summary>
    public sealed class OfficeReleaseEraGraceConfig : ISync
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