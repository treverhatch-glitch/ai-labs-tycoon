using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class ProductStudioSystem
  {
    public static void RunWeek(GameConfig c, GameState state, LabState lab)
    {
      ProductStudioProgramConfig cfg = c.RoomPrograms?.ProductStudio;
      if (cfg == null || lab == null || lab.Bankrupt) return;
      if (lab.SectorCampaignUntilTick >= 0 && state.Tick >= lab.SectorCampaignUntilTick)
      {
        string sector = lab.SectorCampaignSectorId;
        lab.SectorCampaignSectorId = "";
        lab.SectorCampaignUntilTick = -1;
        if (!string.IsNullOrEmpty(sector))
          Events.Add(state, EventKind.SectorCampaignEnded, lab.Id, -1, 0, sector);
        return;
      }
      if (string.IsNullOrEmpty(lab.SectorCampaignSectorId)) return;
      if (!RoomProgramSupport.ActiveUntil(lab.SectorCampaignUntilTick, state.Tick)) return;
      if (cfg.CampaignWeeklyUpkeepCash > 0 && lab.Cash >= cfg.CampaignWeeklyUpkeepCash)
        Finance.Spend(lab, LedgerLine.RoomUpkeep, cfg.CampaignWeeklyUpkeepCash);
      if (cfg.CampaignWeeklyMarketingCash > 0)
        Finance.Earn(lab, LedgerLine.Grants, cfg.CampaignWeeklyMarketingCash);
    }
    public static bool CampaignLive(GameConfig c, GameState state, LabState lab, string sectorId)
    {
      ProductStudioProgramConfig cfg = c.RoomPrograms?.ProductStudio;
      if (cfg == null || lab == null) return false;
      if (!RoomProgramSupport.ActiveUntil(lab.SectorCampaignUntilTick, state.Tick)) return false;
      if (lab.SectorCampaignSectorId != sectorId) return false;
      return RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, cfg.StaffRoleId);
    }

    public static int CampaignAppealPermille(GameConfig c, GameState state, LabState lab, string sectorId)
    {
      ProductStudioProgramConfig cfg = c.RoomPrograms?.ProductStudio;
      if (cfg == null || !CampaignLive(c, state, lab, sectorId)) return 0;
      return cfg.CampaignAppealPermille;
    }

    public static int CampaignRevenuePermille(GameConfig c, GameState state, LabState lab, string sectorId)
    {
      ProductStudioProgramConfig cfg = c.RoomPrograms?.ProductStudio;
      if (cfg == null || !CampaignLive(c, state, lab, sectorId)) return 0;
      return cfg.CampaignRevenuePermille;
    }
  }
}