using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ScrapRun(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab scraps a training run.");
            if (!Abandon.Enabled(c)) return Fail(CommandError.InvalidArgument, "Training runs cannot be scrapped in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Training) return Fail(CommandError.WrongModelStatus, model.Name + " is " + model.Status + "; only a run that is still training can be scrapped.");
            Abandon.Scrap(c, state, lab, model);
            return CommandResult.Done(-1);
        }
    }
}
