using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  /// <summary>Q166: coolant fouling between flushes; flush rescues cooling cut and pays GPU-scaled rebates.</summary>
  public static class LiquidCoolingPodSystem
  {
    public static bool HasPod(GameConfig c, GameState state, LabState lab)
    {
      LiquidCoolingPodProgramConfig cfg = c.RoomPrograms?.LiquidCoolingPod;
      return cfg != null && lab != null && RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, "");
    }

    public static int CoolingCostCutPermille(GameConfig c, GameState state, LabState lab)
    {
      LiquidCoolingPodProgramConfig cfg = c.RoomPrograms?.LiquidCoolingPod;
      if (cfg == null || lab == null || lab.Bankrupt) return 0;
      if (!HasPod(c, state, lab)) return 0;
      int cut = cfg.CoolingCostCutPermille;
      if (cut < 1) return 0;
      if (!RoomProgramSupport.ActiveUntil(lab.CoolantBoostUntilTick, state.Tick)) return 0;
      if (lab.CoolantFoulingPermille > 0)
        cut = (int)MathI.MulPermille(cut, MathI.Permille - lab.CoolantFoulingPermille);
      return cut < 0 ? 0 : cut;
    }

    public static long WeeklyFlushRebate(GameConfig c, GameState state, LabState lab)
    {
      LiquidCoolingPodProgramConfig cfg = c.RoomPrograms?.LiquidCoolingPod;
      if (cfg == null || lab == null || lab.Bankrupt) return 0;
      if (!HasPod(c, state, lab)) return 0;
      if (!RoomProgramSupport.ActiveUntil(lab.CoolantBoostUntilTick, state.Tick)) return 0;
      long each = cfg.FlushRebatePerRentedGpu;
      if (each <= 0) return cfg.FlushWeeklyRebateCash;
      int gpus = lab.RentedGpus;
      int cap = cfg.FlushRebateRentedGpuCap;
      if (cap > 0 && gpus > cap) gpus = cap;
      return each * gpus;
    }

    public static void RunWeek(GameConfig c, GameState state, LabState lab)
    {
      LiquidCoolingPodProgramConfig cfg = c.RoomPrograms?.LiquidCoolingPod;
      if (cfg == null || lab == null || lab.Bankrupt) return;
      if (!HasPod(c, state, lab)) return;
      if (RoomProgramSupport.ActiveUntil(lab.CoolantBoostUntilTick, state.Tick))
      {
        long rebate = WeeklyFlushRebate(c, state, lab);
        if (rebate > 0) Finance.Earn(lab, LedgerLine.Grants, rebate);
        return;
      }
      if (cfg.FoulingGainPermillePerWeek > 0)
      {
        int max = cfg.MaxFoulingPermille;
        if (max < 1) max = MathI.Permille;
        lab.CoolantFoulingPermille = (int)MathI.Min(max, lab.CoolantFoulingPermille + cfg.FoulingGainPermillePerWeek);
      }
    }
  }
}