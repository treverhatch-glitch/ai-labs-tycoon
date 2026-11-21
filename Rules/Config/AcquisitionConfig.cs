using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
  public sealed class AcquisitionConfig : ISync
  {
    public long CapabilityValuePerPoint;
    public long StaffValuePerHead;
    public int FleetResalePermille;
    public int IntegrationCostPermille;
    public int DistressPricePermille;
    public int CrashDistressPricePermille;
    public int MinInstalledModels;
    public long RegulatorDealRevenue;
    public int RegulatorAttention;

    public void Sync(Syncer s)
    {
      s.Long("capabilityValuePerPoint", ref CapabilityValuePerPoint);
      s.Long("staffValuePerHead", ref StaffValuePerHead);
      s.Int("fleetResalePermille", ref FleetResalePermille);
      s.Int("integrationCostPermille", ref IntegrationCostPermille);
      s.Int("distressPricePermille", ref DistressPricePermille);
      s.Int("crashDistressPricePermille", ref CrashDistressPricePermille);
      s.Int("minInstalledModels", ref MinInstalledModels);
      s.Long("regulatorDealRevenue", ref RegulatorDealRevenue);
      s.Int("regulatorAttention", ref RegulatorAttention);
    }
  }
}