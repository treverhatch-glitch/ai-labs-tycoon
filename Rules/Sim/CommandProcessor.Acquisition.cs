using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
  public static partial class CommandProcessor
  {
    static CommandResult AcquireLab(GameConfig c, GameState state, LabState buyer, Command cmd)
    {
      LabState target = state.Lab(cmd.TargetId);
      if (target == null) return Fail(CommandError.UnknownId, "There is no lab " + cmd.TargetId + ".");
      if (!AcquisitionSystem.CanAcquire(c, state, buyer, target, out string reason))
        return Fail(CommandError.TargetNotEligible, reason);
      AcquisitionSystem.Apply(c, state, buyer, target);
      return CommandResult.Done(target.Id);
    }
  }
}