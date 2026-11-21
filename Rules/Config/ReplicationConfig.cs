using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Other labs trying to reproduce the lab's papers. Each paper the lab publishes or has accepted waits for a decision: put the code and data out (after as many weeks of clean-up as the lab can pay for), or keep it back at a small cost to its standing. A paper that goes out is tried by other labs; if it is reproducible enough for the lab's standing it replicates and adds prestige, if not it is a scandal. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ReplicationConfig : ISync
    {
        public int Enabled;
        /// <summary>Most papers waiting for the lab to decide about their code.</summary>
        public int PendingMax;
        /// <summary>Weeks a paper waits before the lab is taken to have kept its code back.</summary>
        public int PendingWeeks;
        /// <summary>Weeks another lab takes to try to reproduce a paper.</summary>
        public int Weeks;
        /// <summary>Most weeks of cleaning up the code before it goes out.</summary>
        public int MaxEffort;
        /// <summary>Research points each week of clean-up costs.</summary>
        public int EffortPoints;
        /// <summary>How reproducible a paper is before anyone cleans it up.</summary>
        public int RigorBase;
        /// <summary>What each week of clean-up adds.</summary>
        public int RigorStep;
        /// <summary>What each person doing research adds, up to ten of them.</summary>
        public int RigorPerWriter;
        /// <summary>How reproducible a paper has to be to hold up.</summary>
        public int BarBase;
        /// <summary>What prestige adds to the bar at full prestige: a famous lab is checked harder.</summary>
        public int BarPerPrestige;
        /// <summary>Prestige a paper that replicates adds.</summary>
        public int WinPrestige;
        /// <summary>Reputation it adds.</summary>
        public int WinReputation;
        /// <summary>Prestige a paper that does not replicate costs.</summary>
        public int LossPrestige;
        /// <summary>Reputation it costs (damped).</summary>
        public int LossReputation;
        /// <summary>Prestige keeping the code back costs.</summary>
        public int WithholdPrestige;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("pendingMax", ref PendingMax);
            s.Int("pendingWeeks", ref PendingWeeks);
            s.Int("weeks", ref Weeks);
            s.Int("maxEffort", ref MaxEffort);
            s.Int("effortPoints", ref EffortPoints);
            s.Int("rigorBase", ref RigorBase);
            s.Int("rigorStep", ref RigorStep);
            s.Int("rigorPerWriter", ref RigorPerWriter);
            s.Int("barBase", ref BarBase);
            s.Int("barPerPrestige", ref BarPerPrestige);
            s.Int("winPrestige", ref WinPrestige);
            s.Int("winReputation", ref WinReputation);
            s.Int("lossPrestige", ref LossPrestige);
            s.Int("lossReputation", ref LossReputation);
            s.Int("withholdPrestige", ref WithholdPrestige);
        }
    }
}
