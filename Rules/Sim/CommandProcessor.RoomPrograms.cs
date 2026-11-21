using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
  public static partial class CommandProcessor
  {
    static CommandResult LaunchSectorCampaign(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      ProductStudioProgramConfig cfg = c.RoomPrograms?.ProductStudio;
      if (cfg == null) return Fail(CommandError.UnknownId, "Product studio is not configured.");
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, cfg.StaffRoleId))
        return Fail(CommandError.EraLocked, "Build a staffed product studio before launching a campaign.");
      SectorConfig sector = c.Sector(cmd.Id);
      if (sector == null) return Fail(CommandError.UnknownId, "There is no sector \"" + cmd.Id + "\".");
      if (cfg.CampaignCashCost > 0 && lab.Cash < cfg.CampaignCashCost) return NeedCash(lab, cfg.CampaignCashCost);

      if (cfg.CampaignCashCost > 0) Finance.Spend(lab, LedgerLine.Construction, cfg.CampaignCashCost);
      lab.SectorCampaignSectorId = sector.Id;
      lab.SectorCampaignUntilTick = state.Tick + cfg.CampaignWeeks;
      Events.Add(state, EventKind.WorldNews, lab.Id, -1, cfg.CampaignCashCost, sector.Id);
      return CommandResult.Done(-1);
    }

    static CommandResult EndSectorCampaign(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      ProductStudioProgramConfig cfg = c.RoomPrograms?.ProductStudio;
      if (cfg == null) return Fail(CommandError.UnknownId, "Product studio is not configured.");
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, cfg.StaffRoleId))
        return Fail(CommandError.EraLocked, "Build a staffed product studio before ending a campaign.");
      if (!RoomProgramSupport.ActiveUntil(lab.SectorCampaignUntilTick, state.Tick))
        return Fail(CommandError.InvalidArgument, "There is no sector campaign running.");
      lab.SectorCampaignSectorId = "";
      lab.SectorCampaignUntilTick = -1;
      return CommandResult.Done(-1);
    }

    static CommandResult EngageFactoryLine(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      AiFactoryHallProgramConfig cfg = c.RoomPrograms?.AiFactoryHall;
      if (cfg == null) return Fail(CommandError.UnknownId, "AI factory hall is not configured.");
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, ""))
        return Fail(CommandError.EraLocked, "Build an AI factory hall before engaging a factory line.");
      if (cfg.EngageCashCost > 0 && lab.Cash < cfg.EngageCashCost) return NeedCash(lab, cfg.EngageCashCost);

      if (cfg.EngageCashCost > 0) Finance.Spend(lab, LedgerLine.Construction, cfg.EngageCashCost);
      lab.FactoryLineUntilTick = state.Tick + cfg.EngageWeeks;
      Events.Add(state, EventKind.WorldNews, lab.Id, -1, cfg.EngageCashCost, "factory-line");
      return CommandResult.Done(-1);
    }

    static CommandResult EndFactoryLine(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      AiFactoryHallProgramConfig cfg = c.RoomPrograms?.AiFactoryHall;
      if (cfg == null) return Fail(CommandError.UnknownId, "AI factory hall is not configured.");
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, ""))
        return Fail(CommandError.EraLocked, "Build an AI factory hall before ending a factory line.");
      if (!RoomProgramSupport.ActiveUntil(lab.FactoryLineUntilTick, state.Tick))
        return Fail(CommandError.InvalidArgument, "No factory line is engaged.");
      lab.FactoryLineUntilTick = -1;
      return CommandResult.Done(-1);
    }

    static CommandResult FlushCoolant(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      LiquidCoolingPodProgramConfig cfg = c.RoomPrograms?.LiquidCoolingPod;
      if (cfg == null) return Fail(CommandError.UnknownId, "Liquid cooling pod is not configured.");
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, ""))
        return Fail(CommandError.EraLocked, "Build a liquid cooling pod before flushing coolant.");
      if (cfg.FlushCashCost > 0 && lab.Cash < cfg.FlushCashCost) return NeedCash(lab, cfg.FlushCashCost);

      if (cfg.FlushCashCost > 0) Finance.Spend(lab, LedgerLine.SiteBuild, cfg.FlushCashCost);
      lab.CoolantBoostUntilTick = state.Tick + cfg.FlushWeeks;
      Events.Add(state, EventKind.WorldNews, lab.Id, -1, cfg.FlushCashCost, "coolant-flush");
      return CommandResult.Done(-1);
    }

    static CommandResult EndCoolantFlush(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      LiquidCoolingPodProgramConfig cfg = c.RoomPrograms?.LiquidCoolingPod;
      if (cfg == null) return Fail(CommandError.UnknownId, "Liquid cooling pod is not configured.");
      if (!RoomProgramSupport.Gate(c, state, lab, cfg.EraId, cfg.RoomTypeId, ""))
        return Fail(CommandError.EraLocked, "Build a liquid cooling pod before ending a coolant flush.");
      if (!RoomProgramSupport.ActiveUntil(lab.CoolantBoostUntilTick, state.Tick))
        return Fail(CommandError.InvalidArgument, "There is no active coolant flush.");
      lab.CoolantBoostUntilTick = -1;
      return CommandResult.Done(-1);
    }
  }
}