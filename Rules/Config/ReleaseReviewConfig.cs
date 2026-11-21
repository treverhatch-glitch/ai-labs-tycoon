using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
  /// <summary>Monthly staff review before the player sells into enterprise sectors or oversells alignment.</summary>
  public sealed class ReleaseReviewConfig : ISync
  {
    /// <summary>Calendar year the API-era review loop turns on for the player.</summary>
    public int OpensYear;
    public int BaseSlotsPerMonth;
    public int SlotsPerSafetyStaff;
    public int MaxSlotsPerMonth;
    /// <summary>Reject or send back when claimed alignment exceeds true by at least this much.</summary>
    public int SandbagSendBackGapPermille;
    /// <summary>Minimum true alignment to sell into a strict sector.</summary>
    public int MinAlignmentStrictSector;
    public int MaxRevisionRounds;
    public int BacklogReputationPenalty;
    public int MaxBacklogBeforePenalty;
    public int RejectedReputationPenalty;
    public long AppealCashCost;
    public int AppealSuccessPermille;
    public int AppealFailReputationPenalty;
    public long ExpediteCashCost;
    public List<string> StrictSectorIds = new List<string>();

    public void Sync(Syncer s)
    {
      s.Int("opensYear", ref OpensYear);
      s.Int("baseSlotsPerMonth", ref BaseSlotsPerMonth);
      s.Int("slotsPerSafetyStaff", ref SlotsPerSafetyStaff);
      s.Int("maxSlotsPerMonth", ref MaxSlotsPerMonth);
      s.Int("sandbagSendBackGapPermille", ref SandbagSendBackGapPermille);
      s.Int("minAlignmentStrictSector", ref MinAlignmentStrictSector);
      s.Int("maxRevisionRounds", ref MaxRevisionRounds);
      s.Int("backlogReputationPenalty", ref BacklogReputationPenalty);
      s.Int("maxBacklogBeforePenalty", ref MaxBacklogBeforePenalty);
      s.Int("rejectedReputationPenalty", ref RejectedReputationPenalty);
      s.Long("appealCashCost", ref AppealCashCost);
      s.Int("appealSuccessPermille", ref AppealSuccessPermille);
      s.Int("appealFailReputationPenalty", ref AppealFailReputationPenalty);
      s.Long("expediteCashCost", ref ExpediteCashCost);
      s.StrList("strictSectorIds", ref StrictSectorIds);
    }
  }
}