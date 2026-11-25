using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Shorten lobby cooldown after shaping law once era allows (Q490, schema 161).</summary>
    public sealed class LawShapingEraCooldownConfig : ISync
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