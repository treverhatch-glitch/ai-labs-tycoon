using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Discount lobby push cost in late era (Q531, schema 193).</summary>
    public sealed class LobbyingCostEraReliefConfig : ISync
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