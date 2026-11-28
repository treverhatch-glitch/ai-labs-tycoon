using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Stronger quiet/subsidy lobby outcomes in late era (Q535, schema 195).</summary>
    public sealed class LobbyingGoalEraAmplifyConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int AmplifyPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("amplifyPermille", ref AmplifyPermille);
        }
    }
}