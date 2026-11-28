using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Soften conformity review push on bad public metrics in late era (Q532, schema 194).</summary>
    public sealed class PublicMetricsConformityEraSlackConfig : ISync
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