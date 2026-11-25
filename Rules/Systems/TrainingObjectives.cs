using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The objective a model was trained for. See <see cref="ObjectivesConfig"/>.</summary>
    public static class TrainingObjectives
    {
        /// <summary>The model's objective, or null for the default (be correct).</summary>
        public static ObjectiveConfig Of(GameConfig c, ModelState model)
        {
            if (string.IsNullOrEmpty(model.Objective)) return null;
            return c.Objectives.Objective(model.Objective);
        }

        /// <summary>Appeal as a share of its usual value, in permille: 1000 for a model trained to be correct.</summary>
        public static int AppealPermille(GameConfig c, ModelState model)
        {
            ObjectiveConfig o = Of(c, model);
            if (o == null) return MathI.Permille;
            return (int)MathI.Max(1, MathI.Permille + o.AppealPermille);
        }

        /// <summary>Applies the objective to a model whose run has just finished.</summary>
        public static void OnTrained(GameConfig c, LabState lab, ModelState model)
        {
            ObjectiveConfig o = Of(c, model);
            if (o == null) return;
            if (o.CapabilityPermille != 0)
            {
                long delta = MathI.MulPermille(model.Capability, o.CapabilityPermille);
                if (model.Capability + delta < 1) delta = 1 - model.Capability;
                model.CapLuck += (int)delta;
                model.Capability += (int)delta;
            }
            if (o.AlignmentPoints != 0)
            {
                model.Alignment = (int)MathI.Clamp((long)model.Alignment + o.AlignmentPoints, 0, MathI.Permille);
                model.DisplayedAlignment = model.Alignment;
            }
            if (o.CulturePoints != 0) SafetyCulture.Move(c, lab, o.CulturePoints);
        }
    }
}
