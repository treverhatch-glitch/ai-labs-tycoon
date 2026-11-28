using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Shorter takeoff cooldown after an answer in a late era (Q508, schema 176).</summary>
    public sealed class TakeoffAnswerEraCooldownConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int CooldownCutWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("cooldownCutWeeks", ref CooldownCutWeeks);
        }
    }
}