using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class AiFactoryHallSystem
  {
    public static int ExtraConcurrentRuns(GameConfig c, GameState state, LabState lab)
    {
      AiFactoryHallProgramConfig cfg = c.RoomPrograms?.AiFactoryHall;
      if (cfg == null || lab == null || lab.Bankrupt) return 0;
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, "")) return 0;
      if (!RoomProgramSupport.ActiveUntil(lab.FactoryLineUntilTick, state.Tick)) return 0;
      return cfg.ExtraConcurrentRuns;
    }
  }
}