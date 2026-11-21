using AiLabs.Rules.Config;
using AiLabs.Rules.State;

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
            if (string.IsNullOrEmpty(cmd.Id)) return Fail(CommandError.InvalidArgument, "Name the data source.");
            if (c.Training.DataSource(cmd.Id) == null) return Fail(CommandError.UnknownId, "There is no data source \"" + cmd.Id + "\".");
            lab.TrainingDataSourceId = cmd.Id;
            return CommandResult.Done(-1);
        }
    }
}