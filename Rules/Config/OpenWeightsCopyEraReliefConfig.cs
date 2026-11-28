using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Softer trust hit when open weights are copied in late era (Q524, schema 188).</summary>
    public sealed class OpenWeightsCopyEraReliefConfig : ISync
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