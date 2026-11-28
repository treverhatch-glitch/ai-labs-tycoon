using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Unity.Hq
{
    /// <summary>Formats training progress for UI bars (presentation only).</summary>
    public static class TrainingRunFormatter
    {
        public static int ProgressPermille(ModelState model)
        {
            if (model.RequiredCompute <= 0) return 0;
            long p = model.ProgressCompute * 1000L / model.RequiredCompute;
            if (p < 0) return 0;
            if (p > 1000) return 1000;
            return (int)p;
        }

        public static string LineLabel(GameConfig config, ModelState model)
        {
            ModelLineConfig line = config.Line(model.LineId);
            string lineName = line != null ? line.Name : model.LineId;
            return model.Name + " (" + lineName + " v" + model.Version + ")";
        }
    }
}