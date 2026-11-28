using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Shorten lobby cooldown after a push once era allows (Q527, schema 190).</summary>
    public sealed class LobbyingEraCooldownConfig : ISync
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