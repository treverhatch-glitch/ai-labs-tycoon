using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Agent sandboxes (bible ch. 6, agent-sandboxes room): a released model with agents spends weeks in isolation before code and data tools are allowed. Passing graduation gates the tool mask; failing leaves the model on web-only until retried. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class AgentSandboxConfig : ISync
    {
        public int Enabled;
        /// <summary>First era index for agent sandboxes.</summary>
        public int MinEra;
        /// <summary>Cash to start a sandbox run.</summary>
        public int Cost;
        /// <summary>Weeks a sandbox runs.</summary>
        public int Weeks;
        /// <summary>Safety people needed to start a sandbox.</summary>
        public int Staff;
        /// <summary>Score (out of 1000) a model needs to pass graduation.</summary>
        public int PassBar;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("cost", ref Cost);
            s.Int("weeks", ref Weeks);
            s.Int("staff", ref Staff);
            s.Int("passBar", ref PassBar);
        }
    }
}