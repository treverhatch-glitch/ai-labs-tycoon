using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ApiGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab opens APIs.");
            if (!Integrations.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody builds on APIs in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult OpenApi(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ApiGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released || model.Sectors.Count == 0) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale in any sector.");
            if (model.ApiState != 0) return Fail(CommandError.InvalidArgument, model.Name + " already has an API.");
            if (Integrations.OpenCount(lab) >= c.Integrations.MaxOpen) return Fail(CommandError.InvalidArgument, "The lab can run " + c.Integrations.MaxOpen + " APIs at a time.");
            Integrations.Open(c, state, lab, model);
            return CommandResult.Done(-1);
        }

        static CommandResult DeprecateApi(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ApiGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.ApiState != 1) return Fail(CommandError.InvalidArgument, model.Name + " has no open API to retire.");
            if (cmd.Count < c.Integrations.MinWeeks || cmd.Count > c.Integrations.MaxWeeks) return Fail(CommandError.InvalidArgument, "A deadline runs from " + c.Integrations.MinWeeks + " to " + c.Integrations.MaxWeeks + " weeks.");
            int successorId = 0;
            if (!int.TryParse(cmd.Id, out successorId) || successorId < 0) return Fail(CommandError.InvalidArgument, "The successor is a model id, or 0 for none.");
            if (successorId > 0)
            {
                ModelState next = lab.Model(successorId);
                if (next == null) return Fail(CommandError.UnknownId, "The lab has no model " + successorId + ".");
                if (next.Id == model.Id) return Fail(CommandError.InvalidArgument, "A model cannot succeed itself.");
                if (next.ApiState != 1) return Fail(CommandError.InvalidArgument, next.Name + " needs an open API to take the integrations.");
            }
            Integrations.Deprecate(c, state, lab, model, successorId, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult CancelDeprecation(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ApiGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.ApiState != 2) return Fail(CommandError.InvalidArgument, model.Name + " is not being deprecated.");
            Integrations.Cancel(c, lab, model);
            return CommandResult.Done(-1);
        }
    }
}
