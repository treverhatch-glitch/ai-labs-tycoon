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

    public static void RunWeek(GameConfig c, GameState state, LabState lab)
    {
      AiFactoryHallProgramConfig cfg = c.RoomPrograms?.AiFactoryHall;
      if (cfg == null || lab == null || lab.Bankrupt) return;
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, "")) return;
      if (!RoomProgramSupport.ActiveUntil(lab.FactoryLineUntilTick, state.Tick)) return;
      if (cfg.FactoryWeeklyUpkeepCash > 0 && lab.Cash >= cfg.FactoryWeeklyUpkeepCash)
        Finance.Spend(lab, LedgerLine.RoomUpkeep, cfg.FactoryWeeklyUpkeepCash);
      if (cfg.FactoryWeeklyResearchPoints > 0)
        lab.ResearchPoints += cfg.FactoryWeeklyResearchPoints;
    }
  }
}