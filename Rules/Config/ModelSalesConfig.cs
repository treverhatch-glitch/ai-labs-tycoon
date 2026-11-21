using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
  public sealed class ModelSalesConfig : ISync
  {
    public int UnprofitableSalePermille;
    public int EarningsMultipleWeeks;
    public int SectorReputationHit;
    public long MinOffer;

    public void Sync(Syncer s)
    {
      s.Int("unprofitableSalePermille", ref UnprofitableSalePermille);
      s.Int("earningsMultipleWeeks", ref EarningsMultipleWeeks);
      s.Int("sectorReputationHit", ref SectorReputationHit);
      s.Long("minOffer", ref MinOffer);
    }
  }
}