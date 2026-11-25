using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PublishMetrics(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab publishes metrics.");
            if (!PublicMetrics.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no public metrics in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.PublicMetrics.MinEra) return Fail(CommandError.EraLocked, "Public metrics are not used in this era.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale.");
            if (model.Sectors.Count == 0) return Fail(CommandError.InvalidArgument, model.Name + " is not on sale in any sector.");
            if (model.PubOn > 0) return Fail(CommandError.InvalidArgument, model.Name + " already publishes metrics.");
            if (lab.Cash < c.PublicMetrics.PublishCost) return NeedCash(lab, c.PublicMetrics.PublishCost);
            PublicMetrics.Publish(c, state, lab, model);
            return CommandResult.Done(model.Id);
        }

        static CommandResult StopMetrics(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab stops metrics.");
            if (!PublicMetrics.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no public metrics in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.PubOn <= 0) return Fail(CommandError.InvalidArgument, model.Name + " does not publish metrics.");
            PublicMetrics.Stop(c, state, lab, model);
            return CommandResult.Done(model.Id);
        }
    }
}