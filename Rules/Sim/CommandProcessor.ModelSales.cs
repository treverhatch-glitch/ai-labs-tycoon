using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
  public static partial class CommandProcessor
  {
    static CommandResult SellModel(GameConfig c, GameState state, LabState lab, Command cmd)
    {
      ModelState model = lab.Model(cmd.TargetId);
      if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
      if (!ModelSalesSystem.CanSell(lab, model, out string reason))
        return Fail(CommandError.TargetNotEligible, reason);
      ModelSalesSystem.Apply(c, state, lab, model);
      return CommandResult.Done(model.Id);
    }
  }
}