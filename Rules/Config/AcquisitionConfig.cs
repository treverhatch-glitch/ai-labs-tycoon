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
    /// <summary>Chance per distressed pair that a rival rollup happens during a crash, permille out of 1000.</summary>
    public int RivalRollupChancePermille;
    /// <summary>Each planning week in crash, chance a healthy rival tries to acquire the weakest distressed peer, permille out of 1000.</summary>
    public int PlannerAcquireChancePermille;
    /// <summary>Weeks after M&amp;A during which integrated GPU batches cannot be sold (arbitrage bar).</summary>
    public int FleetResaleLockWeeks;
    /// <summary>Book value permille applied to ready chip designs in acquisition quotes.</summary>
    public int ReadyChipBookPermille;
    /// <summary>Book value permille applied to on-fleet ready chip batches in quotes.</summary>
    public int ChipFleetBookPermille;

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
      s.Int("rivalRollupChancePermille", ref RivalRollupChancePermille);
      s.Int("plannerAcquireChancePermille", ref PlannerAcquireChancePermille);
      s.Int("fleetResaleLockWeeks", ref FleetResaleLockWeeks);
      s.Int("readyChipBookPermille", ref ReadyChipBookPermille);
      s.Int("chipFleetBookPermille", ref ChipFleetBookPermille);
    }
  }
}