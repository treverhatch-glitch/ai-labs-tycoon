using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetToolPolicy(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab sets tool policy.");
            if (!ToolPolicy.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no tool policy in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.ToolPolicy.MinEra) return Fail(CommandError.EraLocked, "Tool policy is not used in this era.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale.");
            if (model.AgentLevel <= 0) return Fail(CommandError.InvalidArgument, model.Name + " has no agents yet.");
            if (cmd.Count < 0 || cmd.Count > 7) return Fail(CommandError.InvalidArgument, "The mask is from 0 to 7.");
            if (AgentSandbox.Enabled(c) && model.AgentLevel > 0 && model.SandGrad != 1 && (cmd.Count & (ToolPolicy.Code | ToolPolicy.Data)) != 0)
                return Fail(CommandError.MissingPrerequisite, model.Name + " has not graduated the agent sandbox; only web tools are allowed.");
            if (lab.Cash < c.ToolPolicy.SetupCost) return NeedCash(lab, c.ToolPolicy.SetupCost);
            ToolPolicy.Set(c, state, lab, model, cmd.Count);
            return CommandResult.Done(model.Id);
        }
    }
}
