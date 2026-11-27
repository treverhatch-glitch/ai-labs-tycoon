using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra weeks to answer a takeoff crisis in a late era (Q504, schema 172).</summary>
    public sealed class TakeoffCrisisEraGraceConfig : ISync
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