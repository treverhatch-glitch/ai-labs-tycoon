using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StartTraining(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelLineConfig line = c.Line(cmd.Id);
            if (line == null) return Fail(CommandError.UnknownId, "There is no model line \"" + cmd.Id + "\".");
            string sourceId = lab.Controller == LabController.Plan
                ? TrainingQueries.PlanDataSourceId(c, lab)
                : (string.IsNullOrEmpty(lab.TrainingDataSourceId) ? "internal" : lab.TrainingDataSourceId);
            return BeginTraining(c, state, lab, line, cmd.Permille, sourceId, "", MathI.Permille);
        }

        static CommandResult StartTrainingRecipe(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Controller == LabController.Plan)
                return Fail(CommandError.InvalidArgument, "Training recipes are for the player lab.");
            TrainingRecipeConfig recipe = TrainingQueries.Recipe(c, cmd.Id);
            if (recipe == null) return Fail(CommandError.UnknownId, "There is no training recipe \"" + cmd.Id + "\".");
            if (!TrainingQueries.RecipeOpen(c, recipe, state.Tick))
                return Fail(CommandError.EraLocked, recipe.Name + " opens in " + recipe.OpensYear + ".");
            ModelLineConfig line = c.Line(recipe.LineId);
            if (line == null) return Fail(CommandError.UnknownId, "Recipe \"" + recipe.Id + "\" points at an unknown line.");
            return BeginTraining(c, state, lab, line, recipe.AlignmentSharePermille, recipe.DataSourceId, recipe.Id, recipe.ComputePermille);
        }

        static CommandResult BeginTraining(GameConfig c, GameState state, LabState lab, ModelLineConfig line, int alignmentSharePermille,
            string sourceId, string recipeId, int computePermille)
        {
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return Fail(CommandError.PermitFrozen, c.World.Bloc(lab.HomeBlocId).Agency + " has frozen " + lab.Name + "'s permits at home.");
            if (!LabQueries.LineUnlocked(c, lab, line.Id))
                return Fail(CommandError.LineLocked, "The lab has not researched how to build a " + line.Name + " yet.");
            if (alignmentSharePermille < 0 || alignmentSharePermille > MaxAlignmentSharePermille)
                return Fail(CommandError.InvalidArgument, "The alignment share is from 0 to " + MaxAlignmentSharePermille + ": some of the run has to train the model.");
            if (LabQueries.ActiveRuns(lab) >= c.Training.MaxConcurrentRuns)
                return Fail(CommandError.TooManyRuns, "The lab can train " + c.Training.MaxConcurrentRuns + " models at once.");
            if (!TrainingQueries.SourceUsable(c, state, lab, sourceId, line.Id, out string sourceMessage))
            {
                DataSourceConfig source = c.Training.DataSource(sourceId);
                if (source != null && !TrainingQueries.DataSourceOpen(c, source, state.Tick))
                    return Fail(CommandError.EraLocked, sourceMessage);
                if (source != null && source.RequiresDistillTarget)
                    return Fail(CommandError.DistillTargetRequired, sourceMessage);
                return Fail(CommandError.UnknownId, sourceMessage);
            }

            DataSourceConfig mix = c.Training.DataSource(sourceId);
            long prepCost = EraSystem.RunPrepCost(c, line, state.Tick);
            prepCost = MathI.MulPermille(prepCost, mix.PrepCostPermille);
            if (lab.Cash < prepCost) return NeedCash(lab, prepCost);

            var model = new ModelState();
            model.Id = state.TakeId();
            model.LineId = line.Id;
            model.Version = LabQueries.NextVersion(lab, line.Id);
            model.Name = lab.FamilyName + " " + line.Name + " " + model.Version;
            model.Status = ModelStatus.Training;
            model.RequiredCompute = TrainingQueries.RequiredCompute(c, line, state.Tick, computePermille);
            model.AlignmentSharePermille = alignmentSharePermille;
            model.StartedTick = state.Tick;
            model.TrainedTick = -1;
            model.ReleasedTick = -1;
            model.LastIncidentTick = -1;
            model.DataSourceId = mix.Id;
            model.RecipeId = recipeId ?? "";
            if (mix.RequiresDistillTarget) model.DistillSourceLabId = lab.DistillTargetLabId;
            lab.Models.Add(model);
            Finance.Spend(lab, LedgerLine.Training, prepCost);
            Events.Add(state, EventKind.TrainingStarted, lab.Id, model.Id, prepCost, model.Name);
            return CommandResult.Done(model.Id);
        }

        static CommandResult SetDistillTarget(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Controller == LabController.Plan)
                return Fail(CommandError.InvalidArgument, "Rivals do not distill weights from each other.");
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return Fail(CommandError.PermitFrozen, c.World.Bloc(lab.HomeBlocId).Agency + " has frozen " + lab.Name + "'s permits at home.");
            if (cmd.TargetId < 0)
            {
                lab.DistillTargetLabId = -1;
                return CommandResult.Done(-1);
            }
            LabState rival = state.Lab(cmd.TargetId);
            if (rival == null || rival.Id == lab.Id)
                return Fail(CommandError.InvalidArgument, "Pick another lab to distill from.");
            lab.DistillTargetLabId = rival.Id;
            Events.Add(state, EventKind.DistillTargetSet, lab.Id, rival.Id, -1, rival.Name);
            return CommandResult.Done(-1);
        }
    }
}