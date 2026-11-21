using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class LiquidCoolingPodSystem
  {
    public static int CoolingCostCutPermille(GameConfig c, GameState state, LabState lab)
    {
      LiquidCoolingPodProgramConfig cfg = c.RoomPrograms?.LiquidCoolingPod;
      if (cfg == null || lab == null || lab.Bankrupt) return 0;
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, "")) return 0;
      if (!RoomProgramSupport.ActiveUntil(lab.CoolantBoostUntilTick, state.Tick)) return 0;
      return cfg.CoolingCostCutPermille;
    }
  }
}