using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StartSandbox(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab runs sandboxes.");
            if (!AgentSandbox.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no agent sandbox in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.AgentSandbox.MinEra) return Fail(CommandError.EraLocked, "Agent sandboxes are not used in this era.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale.");
            if (model.AgentLevel <= 0) return Fail(CommandError.InvalidArgument, model.Name + " has no agents yet.");
            if (model.SandLeft > 0) return Fail(CommandError.InvalidArgument, model.Name + " is already in the sandbox.");
            if (LabQueries.StaffCount(lab, "safety") < c.AgentSandbox.Staff) return Fail(CommandError.NotEnoughStaff, "A sandbox needs " + c.AgentSandbox.Staff + " safety people.");
            if (lab.Cash < c.AgentSandbox.Cost) return NeedCash(lab, c.AgentSandbox.Cost);
            AgentSandbox.Start(c, state, lab, model);
            return CommandResult.Done(model.Id);
        }
    }
}