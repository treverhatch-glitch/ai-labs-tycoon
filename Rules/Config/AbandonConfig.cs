using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Scrapping a training run. The player can stop a run that is still training: the compute already spent is gone and the model is removed,
    /// but part of it comes back as research points (what the team learned). The further along the run was, the more the press notices and
    /// the more the team minds. Only the player's lab does it. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class AbandonConfig : ISync
    {
        /// <summary>1 when runs can be scrapped, 0 when not.</summary>
        public int Enabled;
        /// <summary>The share of the compute already spent that comes back as research points, in permille.</summary>
        public int SalvagePermille;
        /// <summary>Reputation points lost when a run that was all but finished is scrapped (less for less progress).</summary>
        public int ReputationAtFull;
        /// <summary>Morale every person loses when a run that was all but finished is scrapped (less for less progress).</summary>
        public int MoraleAtFull;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("salvagePermille", ref SalvagePermille);
            s.Int("reputationAtFull", ref ReputationAtFull);
            s.Int("moraleAtFull", ref MoraleAtFull);
        }
    }
}
