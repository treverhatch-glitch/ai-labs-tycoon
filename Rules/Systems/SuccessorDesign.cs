using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>A model helping to design its successor. See <see cref="SuccessorConfig"/>.</summary>
    public static class SuccessorDesign
    {
        public static bool IsAssisted(ModelState model) { return model.DesignerId >= 0; }

        /// <summary>How many earlier links the helper had behind it: 0 when the helper was designed by people alone.</summary>
        public static int Links(GameConfig c, LabState lab, ModelState helper)
        {
            int links = 0;
            ModelState at = helper;
            while (at != null && at.DesignerId >= 0 && links < c.Successor.MaxChain)
            {
                links++;
                at = lab.Model(at.DesignerId);
            }
            return links;
        }

        /// <summary>The alignment points the successor of this helper will lose when it finishes.</summary>
        public static int AlignmentLoss(GameConfig c, LabState lab, ModelState helper)
        {
            return c.Successor.AlignmentPoints + c.Successor.ChainPoints * Links(c, lab, helper);
        }

        /// <summary>The capability the helper must have for the run in training, or 0 when the line is unknown.</summary>
        public static long HelperBar(GameConfig c, LabState lab, ModelState run)
        {
            ModelLineConfig line = c.Line(run.LineId);
            if (line == null) return 0;
            return MathI.MulPermille(TrainingSystem.Forecast(c, lab, line).Total, c.Successor.HelperMinPermille);
        }

        public static void Start(GameConfig c, LabState lab, ModelState run, ModelState helper)
        {
            run.DesignerId = helper.Id;
            long remaining = MathI.Max(0, run.RequiredCompute - run.ProgressCompute);
            long saved = MathI.MulPermille(remaining, c.Successor.ComputeSavedPermille);
            run.RequiredCompute = MathI.Max(1, run.RequiredCompute - saved);
        }

        /// <summary>Applies the cost and the gain to a successor whose run has just finished.</summary>
        public static void OnTrained(GameConfig c, LabState lab, ModelState model)
        {
            if (!IsAssisted(model)) return;
            ModelState helper = lab.Model(model.DesignerId);
            int loss = c.Successor.AlignmentPoints + c.Successor.ChainPoints * (helper == null ? 0 : Links(c, lab, helper));
            long delta = MathI.MulPermille(model.Capability, c.Successor.CapabilityPermille);
            model.CapLuck += (int)delta;
            model.Capability += (int)delta;
            model.Alignment = (int)MathI.Clamp((long)model.Alignment - loss, 0, MathI.Permille);
            model.DisplayedAlignment = model.Alignment;
            SafetyCulture.Move(c, lab, -c.Successor.CulturePoints);
        }
    }
}
