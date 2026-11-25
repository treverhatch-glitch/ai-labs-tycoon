using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Shorten rung-six crisis cooldown after answering in late era (Q499, schema 168).</summary>
    public sealed class RungSixEraCooldownConfig : ISync
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