using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetTrainingObjective(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Training)
                return Fail(CommandError.WrongModelStatus, model.Name + " has finished training; what it was trained for is settled.");
            ObjectiveConfig o = c.Objectives.Objective(cmd.Id);
            if (o == null) return Fail(CommandError.UnknownId, "There is no training objective \"" + cmd.Id + "\".");
            bool same = (string.IsNullOrEmpty(model.Objective) ? c.Objectives.DefaultId : model.Objective) == o.IdValue;
            model.Objective = o.IdValue == c.Objectives.DefaultId ? "" : o.IdValue;
            if (!same) Events.Add(state, EventKind.ObjectiveSet, lab.Id, model.Id, 0, model.Objective);
            return CommandResult.Done(model.Id);
        }
    }
}
