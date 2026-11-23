using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra appeal permille once specialist and open-weights play unlock by era (Q480, schema 154).</summary>
    public sealed class SpecialistEraHooksConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int SpecialistPermille;
        public int OpenWeightsPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("specialistPermille", ref SpecialistPermille);
            s.Int("openWeightsPermille", ref OpenWeightsPermille);
        }
    }
}