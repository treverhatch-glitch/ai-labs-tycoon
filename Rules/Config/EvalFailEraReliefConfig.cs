using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Softer trust loss on failed eval publish in late era (Q525, schema 189).</summary>
    public sealed class EvalFailEraReliefConfig : ISync
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