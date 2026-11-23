using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Let scenario wins continue in endless play (Q477, schema 152).</summary>
    public sealed class EndlessScenarioConfig : ISync
    {
        public int Enabled;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
        }
    }
}