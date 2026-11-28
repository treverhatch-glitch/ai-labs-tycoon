using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra weeks before the scenario goal mid-deadline check in a late era (Q503, schema 171).</summary>
    public sealed class ScenarioGoalsEraSlackConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int SlackWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("slackWeeks", ref SlackWeeks);
        }
    }
}