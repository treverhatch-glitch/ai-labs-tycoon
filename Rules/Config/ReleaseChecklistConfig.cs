using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Release checklist (bible ch. 9, what must be true before first sale). A trained model must sign off red-team coverage, documentation, a public eval and a tool policy before its first release when the checklist is on. Missed items are remembered and cost trust every week after release. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ReleaseChecklistConfig : ISync
    {
        public int Enabled;
        /// <summary>First era for a release checklist.</summary>
        public int MinEra;
        /// <summary>Documentation level needed to sign the docs item.</summary>
        public int DocBar;
        /// <summary>Trust lost each week in every sector for each missed checklist item on a released model.</summary>
        public int MissTrust;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("docBar", ref DocBar);
            s.Int("missTrust", ref MissTrust);
        }
    }
}
