using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
  public sealed class ProductStudioProgramConfig : ISync
  {
    public string EraId = "chat-boom";
    public string RoomTypeId = "product-studio";
    public string StaffRoleId = "product-lead";
    public long CampaignCashCost = 25_000;
    public int CampaignWeeks = 12;
    public int CampaignAppealPermille = 120;
    public int CampaignRevenuePermille = 100;
    public long ExtendCampaignCashCost = 12_000;
    public int ExtendCampaignWeeks = 4;

    public void Sync(Syncer s)
    {
      s.Str("eraId", ref EraId);
      s.Str("roomTypeId", ref RoomTypeId);
      s.Str("staffRoleId", ref StaffRoleId);
      s.Long("campaignCashCost", ref CampaignCashCost);
      s.Int("campaignWeeks", ref CampaignWeeks);
      s.Int("campaignAppealPermille", ref CampaignAppealPermille);
      s.Int("campaignRevenuePermille", ref CampaignRevenuePermille);
      s.Long("extendCampaignCashCost", ref ExtendCampaignCashCost);
      s.Int("extendCampaignWeeks", ref ExtendCampaignWeeks);
    }
  }

  public sealed class AiFactoryHallProgramConfig : ISync
  {
    public string EraId = "harnesses";
    public string RoomTypeId = "ai-factory-hall";
    public long EngageCashCost = 40_000;
    public int EngageWeeks = 16;
    public int ExtraConcurrentRuns = 1;

    public void Sync(Syncer s)
    {
      s.Str("eraId", ref EraId);
      s.Str("roomTypeId", ref RoomTypeId);
      s.Long("engageCashCost", ref EngageCashCost);
      s.Int("engageWeeks", ref EngageWeeks);
      s.Int("extraConcurrentRuns", ref ExtraConcurrentRuns);
    }
  }

  public sealed class LiquidCoolingPodProgramConfig : ISync
  {
    public string EraId = "chat-boom";
    public string RoomTypeId = "liquid-cooling-pod";
    public long FlushCashCost = 12_000;
    public int FlushWeeks = 8;
    public int CoolingCostCutPermille = 150;

    public void Sync(Syncer s)
    {
      s.Str("eraId", ref EraId);
      s.Str("roomTypeId", ref RoomTypeId);
      s.Long("flushCashCost", ref FlushCashCost);
      s.Int("flushWeeks", ref FlushWeeks);
      s.Int("coolingCostCutPermille", ref CoolingCostCutPermille);
    }
  }

  public sealed class RoomProgramsConfig : ISync
  {
    public ProductStudioProgramConfig ProductStudio = new ProductStudioProgramConfig();
    public AiFactoryHallProgramConfig AiFactoryHall = new AiFactoryHallProgramConfig();
    public LiquidCoolingPodProgramConfig LiquidCoolingPod = new LiquidCoolingPodProgramConfig();

    public void Sync(Syncer s)
    {
      s.Obj("productStudio", ref ProductStudio);
      s.Obj("aiFactoryHall", ref AiFactoryHall);
      s.Obj("liquidCoolingPod", ref LiquidCoolingPod);
    }
  }
}