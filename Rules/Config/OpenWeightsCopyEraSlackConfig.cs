using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Softer weekly open-weights copy trust hit in late era (Q541, schema 200).</summary>
    public sealed class OpenWeightsCopyEraSlackConfig : ISync
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