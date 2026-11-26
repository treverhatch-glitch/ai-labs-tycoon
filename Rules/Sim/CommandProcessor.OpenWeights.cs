using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    partial class CommandProcessor
    {
        static CommandResult SetOpenWeights(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released && model.Status != ModelStatus.Trained)
                return Fail(CommandError.WrongModelStatus, model.Name + " must be trained or on sale to change open weights.");
            if (model.Sectors.Count == 0 && model.Status == ModelStatus.Trained)
                return Fail(CommandError.InvalidArgument, "Release " + model.Name + " before publishing weights.");
            if (cmd.Count != 0)
            {
                DataSourceConfig source = c.Training.DataSource(model.DataSourceId);
                if (source != null && source.BlocksOpenWeights)
                    return Fail(CommandError.DataSourceBlocksOpenWeights, model.Name + " was trained on " + source.Name + ", which cannot be released as open weights.");
            }
            model.OpenWeights = cmd.Count != 0;
            return CommandResult.Done(-1);
        }

        static CommandResult SetTrainingDataSource(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return Fail(CommandError.PermitFrozen, c.World.Bloc(lab.HomeBlocId).Agency + " has frozen " + lab.Name + "'s permits at home.");
            if (string.IsNullOrEmpty(cmd.Id)) return Fail(CommandError.InvalidArgument, "Name the data source.");
            DataSourceConfig source = c.Training.DataSource(cmd.Id);
            if (source == null) return Fail(CommandError.UnknownId, "There is no data source \"" + cmd.Id + "\".");
            if (source.PlayerOnly && lab.Controller == LabController.Plan)
                return Fail(CommandError.PlayerOnlyDataSource, source.Name + " is not how " + lab.Name + " trains.");
            if (!TrainingQueries.DataSourceOpen(c, source, state.Tick))
                return Fail(CommandError.EraLocked, source.Name + " is not available until " + source.OpensYear + ".");
            if (source.RequiresDistillTarget && lab.DistillTargetLabId < 0)
                return Fail(CommandError.DistillTargetRequired, "Pick a rival to distill from before choosing " + source.Name + ".");
            lab.TrainingDataSourceId = cmd.Id;
            return CommandResult.Done(-1);
        }
    }
}