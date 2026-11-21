using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class ProductStudioSystem
  {
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