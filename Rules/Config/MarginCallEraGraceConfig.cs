using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra margin-call cure weeks in a late era (Q496, schema 166).</summary>
    public sealed class MarginCallEraGraceConfig : ISync
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