using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Softer trust hit when closing open weights in late era (Q537, schema 198).</summary>
    public sealed class OpenWeightsCloseEraReliefConfig : ISync
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