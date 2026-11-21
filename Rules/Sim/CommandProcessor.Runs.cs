using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetRunApproach(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Training)
                return Fail(CommandError.WrongModelStatus, model.Name + " has finished training; how it was built is settled.");
            ApproachConfig a = c.Runs.Approach(cmd.Id);
            if (a == null) return Fail(CommandError.UnknownId, "There is no approach \"" + cmd.Id + "\".");
            bool same = TrainingRuns.ApproachOf(c, model).IdValue == a.IdValue;
            model.Approach = a.IdValue == c.Runs.DefaultApproachId ? "" : a.IdValue;
            if (!same) Events.Add(state, EventKind.RunSetUp, lab.Id, model.Id, 0, "approach|" + model.Approach);
            return CommandResult.Done(model.Id);
        }

        static CommandResult SetCheckpoints(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Training)
                return Fail(CommandError.WrongModelStatus, model.Name + " has finished training; there is nothing left to checkpoint.");
            CheckpointConfig k = c.Runs.Checkpoint(cmd.Id);
            if (k == null) return Fail(CommandError.UnknownId, "There is no checkpoint habit \"" + cmd.Id + "\".");
            bool same = TrainingRuns.CheckpointOf(c, model).IdValue == k.IdValue;
            model.Checkpoints = k.IdValue == c.Runs.DefaultCheckpointId ? "" : k.IdValue;
            if (!same) Events.Add(state, EventKind.RunSetUp, lab.Id, model.Id, 0, "checkpoints|" + model.Checkpoints);
            return CommandResult.Done(model.Id);
        }
    }
}
