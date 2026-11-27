using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Smaller bubble hit from a bad hearing answer in a late era (Q511, schema 177).</summary>
    public sealed class HearingShareEraDampenConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int DampenPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("dampenPermille", ref DampenPermille);
        }
    }
}