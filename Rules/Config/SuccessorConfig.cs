using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Letting a model help design its successor (bible ch. 10, "what erodes it"). A lab can put one of its finished models to work on a run that
    /// is still training: the run needs less compute and finishes a little stronger, and the successor is less well understood. It cannot be undone.
    /// Every link of the chain costs more control than the last. Rivals and the autopilot never do it.
    /// </summary>
    public sealed class SuccessorConfig : ISync
    {
        /// <summary>The share of the run's compute that is saved, in permille of what the run still needs when the helper is put to work.</summary>
        public int ComputeSavedPermille;
        /// <summary>Added to the successor's capability when it finishes, in permille of it.</summary>
        public int CapabilityPermille;
        /// <summary>Taken off the successor's real and shown alignment when it finishes, in points.</summary>
        public int AlignmentPoints;
        /// <summary>Extra alignment points lost for every earlier link: a helper that was itself designed with help counts one link.</summary>
        public int ChainPoints;
        /// <summary>The most links that add to the loss.</summary>
        public int MaxChain;
        /// <summary>Safety culture points lost when the successor finishes.</summary>
        public int CulturePoints;
        /// <summary>The helper's capability must be at least this share of what the run is forecast to reach, in permille.</summary>
        public int HelperMinPermille;

        public void Sync(Syncer s)
        {
            s.Int("computeSavedPermille", ref ComputeSavedPermille);
            s.Int("capabilityPermille", ref CapabilityPermille);
            s.Int("alignmentPoints", ref AlignmentPoints);
            s.Int("chainPoints", ref ChainPoints);
            s.Int("maxChain", ref MaxChain);
            s.Int("culturePoints", ref CulturePoints);
            s.Int("helperMinPermille", ref HelperMinPermille);
        }
    }
}
