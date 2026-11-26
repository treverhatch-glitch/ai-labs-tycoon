using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Service-level pledge config (era Q329, PR #20). MaxLevel zero disables. Enterprise seat deals (Q214) read
    /// <see cref="BreachBelowPermille"/> only for overlap — no <c>SetSla</c> on this branch.
    /// </summary>
    public sealed class SlaConfig : ISync
    {
        public int MaxLevel;
        public int BreachBelowPermille;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Int("breachBelowPermille", ref BreachBelowPermille);
        }
    }
}