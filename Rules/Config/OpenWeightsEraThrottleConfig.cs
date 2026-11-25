using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Cut open-weights copy chance once an era threshold passes (Q486, schema 158).</summary>
    public sealed class OpenWeightsEraThrottleConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int CopyChanceCutPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("copyChanceCutPermille", ref CopyChanceCutPermille);
        }
    }
}