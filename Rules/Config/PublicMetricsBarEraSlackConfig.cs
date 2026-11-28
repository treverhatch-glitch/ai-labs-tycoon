using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Lower published-metrics bar in late era (Q536, schema 196).</summary>
    public sealed class PublicMetricsBarEraSlackConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int SlackPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("slackPermille", ref SlackPermille);
        }
    }
}