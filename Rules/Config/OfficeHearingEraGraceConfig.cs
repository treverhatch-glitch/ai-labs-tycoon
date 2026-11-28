using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra weeks to answer hearing questions in a late era (Q523, schema 186).</summary>
    public sealed class OfficeHearingEraGraceConfig : ISync
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