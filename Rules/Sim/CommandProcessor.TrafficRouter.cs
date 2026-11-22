using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetRouteShare(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab sets route shares.");
            if (!TrafficRouter.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no traffic router in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.TrafficRouter.MinEra) return Fail(CommandError.EraLocked, "Traffic routing is not used in this era.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale.");
            if (cmd.Count < 0 || cmd.Count > MathI.Ppm) return Fail(CommandError.InvalidArgument, "The share is from 0 to " + MathI.Ppm + " ppm.");
            int total = TrafficRouter.RouteTotal(lab) - model.RoutePpm + cmd.Count;
            if (total > MathI.Ppm) return Fail(CommandError.InvalidArgument, "Route shares cannot add up to more than all traffic.");
            TrafficRouter.SetShare(c, state, lab, model, cmd.Count);
            return CommandResult.Done(model.Id);
        }
    }
}