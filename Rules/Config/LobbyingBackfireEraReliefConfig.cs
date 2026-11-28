using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Softer reputation hit on lobby backfire in late era (Q539, schema 198).</summary>
    public sealed class LobbyingBackfireEraReliefConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int ReliefPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("reliefPermille", ref ReliefPermille);
        }
    }
}