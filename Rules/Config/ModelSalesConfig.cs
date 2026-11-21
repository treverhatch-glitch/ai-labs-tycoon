using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
  public sealed class ModelSalesConfig : ISync
  {
    public int UnprofitableSalePermille;
    public int EarningsMultipleWeeks;
    public int SectorReputationHit;
    public long MinOffer;
    /// <summary>Blend sector last-week income with the lab's last closed week revenue share, permille toward the lab week.</summary>
    public int IncomeSmoothingPermille;
    /// <summary>While <see cref="Systems.BubbleSystem"/> is in crash and the seller is acquisition-distressed, multiply the cash offer by this.</summary>
    public int CrashDistressOfferPermille;

    public void Sync(Syncer s)
    {
      s.Int("unprofitableSalePermille", ref UnprofitableSalePermille);
      s.Int("earningsMultipleWeeks", ref EarningsMultipleWeeks);
      s.Int("sectorReputationHit", ref SectorReputationHit);
      s.Long("minOffer", ref MinOffer);
      s.Int("incomeSmoothingPermille", ref IncomeSmoothingPermille);
      s.Int("crashDistressOfferPermille", ref CrashDistressOfferPermille);
    }
  }
}