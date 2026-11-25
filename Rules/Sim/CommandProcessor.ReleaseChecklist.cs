using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SignChecklistItem(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab signs the checklist.");
            if (!ReleaseChecklist.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no release checklist in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is not ready for release.");
            if (cmd.Count != ReleaseChecklist.Red && cmd.Count != ReleaseChecklist.Docs && cmd.Count != ReleaseChecklist.Eval && cmd.Count != ReleaseChecklist.Policy)
                return Fail(CommandError.InvalidArgument, "Sign item 1, 2, 4 or 8.");
            if ((model.ChkDone & cmd.Count) != 0) return Fail(CommandError.InvalidArgument, "That item is already signed.");
            if (!ReleaseChecklist.CanSign(c, state, model, cmd.Count)) return Fail(CommandError.MissingPrerequisite, "The evidence for that item is not there yet.");
            ReleaseChecklist.Sign(c, state, lab, model, cmd.Count);
            return CommandResult.Done(model.Id);
        }
    }
}
