using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Takeoff office crisis when capability and corruption cross bar (Q465, schema 142).</summary>
    public sealed class TakeoffCrisisConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int CapabilityMin;
        public int CorruptionMin;
        /// <summary>Weeks between takeoff prompts after one is answered.</summary>
        public int CooldownWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("capabilityMin", ref CapabilityMin);
            s.Int("corruptionMin", ref CorruptionMin);
            s.Int("cooldownWeeks", ref CooldownWeeks);
        }
    }
}