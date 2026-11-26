using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Training runs, data sources, recipes and rival distillation.</summary>
    public static class TrainingQueries
    {
        public static TrainingRecipeConfig Recipe(GameConfig c, string recipeId)
        {
            if (c.Training.Recipes == null) return null;
            for (int i = 0; i < c.Training.Recipes.Count; i++)
                if (c.Training.Recipes[i].Id == recipeId) return c.Training.Recipes[i];
            return null;
        }

        public static bool RecipeOpen(GameConfig c, TrainingRecipeConfig recipe, int tick)
        {
            if (recipe == null) return false;
            if (recipe.OpensYear <= 0) return true;
            return GameDate.Year(c.Calendar, tick) >= recipe.OpensYear;
        }

        public static bool DataSourceOpen(GameConfig c, DataSourceConfig source, int tick)
        {
            if (source == null) return false;
            if (source.OpensYear <= 0) return true;
            return GameDate.Year(c.Calendar, tick) >= source.OpensYear;
        }

        public static bool SourceUsable(GameConfig c, GameState state, LabState lab, string sourceId, string lineId, out string message)
        {
            message = "";
            DataSourceConfig source = c.Training.DataSource(sourceId);
            if (source == null) { message = "There is no data source \"" + sourceId + "\"."; return false; }
            if (!DataSourceOpen(c, source, state.Tick)) { message = source.Name + " is not available yet."; return false; }
            if (source.PlayerOnly && lab.Controller == LabController.Plan)
            {
                message = source.Name + " is not how " + lab.Name + " trains.";
                return false;
            }
            if (!source.RequiresDistillTarget) return true;
            return DistillTargetOk(c, state, lab, lineId, lab.DistillTargetLabId, out message);
        }

        /// <summary>The mix a rival planner may use for the next run (never scrape or distill).</summary>
        public static string PlanDataSourceId(GameConfig c, LabState lab)
        {
            string id = string.IsNullOrEmpty(lab.TrainingDataSourceId) ? "internal" : lab.TrainingDataSourceId;
            DataSourceConfig source = c.Training.DataSource(id);
            if (source == null || source.PlayerOnly) return "internal";
            return id;
        }

        public static bool DistillTargetOk(GameConfig c, GameState state, LabState lab, string lineId, int targetLabId, out string message)
        {
            message = "";
            if (targetLabId < 0) { message = "Pick a rival to distill from first."; return false; }
            LabState rival = state.Lab(targetLabId);
            if (rival == null || rival.Id == lab.Id) { message = "Pick another lab to distill from."; return false; }
            ModelState theirs = BestReleasedAbove(c, lab, rival, lineId);
            if (theirs == null)
            {
                message = rival.Name + " has no released " + (c.Line(lineId)?.Name ?? lineId) + " model ahead of yours.";
                return false;
            }
            return true;
        }

        public static ModelState BestReleasedAbove(GameConfig c, LabState lab, LabState rival, string lineId)
        {
            ModelState bestTheirs = null;
            for (int i = 0; i < rival.Models.Count; i++)
            {
                ModelState m = rival.Models[i];
                if (m.Status != ModelStatus.Released && m.Status != ModelStatus.Trained) continue;
                if (m.LineId != lineId) continue;
                if (bestTheirs == null || m.Capability > bestTheirs.Capability) bestTheirs = m;
            }
            if (bestTheirs == null) return null;
            int mine = BestFinishedCapability(lab, lineId);
            int gap = c.Training.DistillMinLeadPermille;
            int threshold = mine + (int)MathI.MulPermille(mine, gap) / MathI.Permille;
            if (mine <= 0) threshold = 1;
            return bestTheirs.Capability > threshold ? bestTheirs : null;
        }

        public static int BestFinishedCapability(LabState lab, string lineId)
        {
            int mine = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId != lineId || m.Status == ModelStatus.Training || m.Status == ModelStatus.Failed) continue;
                if (m.Capability > mine) mine = m.Capability;
            }
            return mine;
        }

        public static long RequiredCompute(GameConfig c, ModelLineConfig line, int tick, int computePermille)
        {
            long need = EraSystem.RunCompute(c, line, tick);
            if (computePermille <= 0 || computePermille == MathI.Permille) return need;
            return MathI.MulPermille(need, computePermille);
        }

        public struct TrainingFailBreakdown
        {
            public int Base;
            public int DataSource;
            public int EngineerReduction;
            public int AlignmentReduction;
            public int MoraleReduction;

            public int Effective
            {
                get
                {
                    int risk = Base + DataSource - EngineerReduction - AlignmentReduction - MoraleReduction;
                    return (int)MathI.Clamp(risk, 0, 950);
                }
            }
        }

        public static TrainingFailBreakdown WeeklyFailParts(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            var b = new TrainingFailBreakdown();
            b.Base = c.Training.BaseFailPermille;
            DataSourceConfig source = c.Training.DataSource(model.DataSourceId);
            b.DataSource = source == null ? 0 : SyntheticDataLabSystem.EffectiveWeeklyFailRiskPermille(c, state, lab, source);
            b.EngineerReduction = LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering) * c.Training.FailEngineerCutPermille;
            b.AlignmentReduction = model.AlignmentSharePermille / 100 * c.Training.FailAlignmentCutPermille;
            b.MoraleReduction = StaffSystem.MoraleFailCutPermille(c, lab);
            return b;
        }
    }
}