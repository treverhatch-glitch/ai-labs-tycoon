using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum ReleaseReviewPhase
    {
        Submitted,
        NeedsRevision,
        Approved,
        Rejected,
        Withdrawn,
    }

    /// <summary>One model waiting on or cleared by the monthly release review board (player only).</summary>
    public sealed class ReleaseReviewCase : ISync
    {
        public int ModelId;
        public ReleaseReviewPhase Phase = ReleaseReviewPhase.Submitted;
        public List<string> SectorIds = new List<string>();
        public int ClaimedAlignmentPermille;
        public int RevisionRounds;
        public int SubmittedTick;
        public bool Expedited;

        public void Sync(Syncer s)
        {
            s.Int("modelId", ref ModelId);
            s.Enum("phase", ref Phase);
            s.StrList("sectorIds", ref SectorIds);
            s.Int("claimedAlignmentPermille", ref ClaimedAlignmentPermille);
            s.Int("revisionRounds", ref RevisionRounds);
            s.Int("submittedTick", ref SubmittedTick);
            s.Bool("expedited", ref Expedited);
        }
    }
}