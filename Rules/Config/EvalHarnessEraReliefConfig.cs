using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Register-cost relief on eval harness in late era (Q520, schema 184).</summary>
    public sealed class EvalHarnessEraReliefConfig : ISync
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