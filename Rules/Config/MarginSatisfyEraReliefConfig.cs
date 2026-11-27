using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Cut cash due on margin satisfy in a late era (Q502, schema 170).</summary>
    public sealed class MarginSatisfyEraReliefConfig : ISync
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