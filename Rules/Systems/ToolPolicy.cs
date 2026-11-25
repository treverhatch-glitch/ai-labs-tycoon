using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Per-model tool allow-lists for agents. See <see cref="ToolPolicyConfig"/>. Only the player's lab.</summary>
    public static class ToolPolicy
    {
        public const int Web = 1;
        public const int Code = 2;
        public const int Data = 4;

        public static bool Enabled(GameConfig c) { return c.ToolPolicy.Enabled > 0; }

        public static int LiftBonus(GameConfig c, ModelState model)
        {
            if (!Enabled(c) || model.ToolMask == 0) return MathI.Permille;
            int bonus = 0;
            if ((model.ToolMask & Web) != 0) bonus += c.ToolPolicy.LiftWeb;
            if ((model.ToolMask & Code) != 0) bonus += c.ToolPolicy.LiftCode;
            if ((model.ToolMask & Data) != 0) bonus += c.ToolPolicy.LiftData;
            return (int)MathI.Min(2500, MathI.Permille + bonus);
        }

        public static int IncidentExtra(GameConfig c, ModelState model)
        {
            if (!Enabled(c) || model.ToolMask == 0) return 0;
            int n = 0;
            if ((model.ToolMask & Web) != 0) n += c.ToolPolicy.IncidentWeb;
            if ((model.ToolMask & Code) != 0) n += c.ToolPolicy.IncidentCode;
            if ((model.ToolMask & Data) != 0) n += c.ToolPolicy.IncidentData;
            return n;
        }

        public static int BlowupExtra(GameConfig c, ModelState model)
        {
            if (!Enabled(c) || model.ToolMask == 0 || model.AgentLevel < 2) return 0;
            int n = 0;
            if ((model.ToolMask & Web) != 0) n += c.ToolPolicy.BlowupWeb;
            if ((model.ToolMask & Code) != 0) n += c.ToolPolicy.BlowupCode;
            if ((model.ToolMask & Data) != 0) n += c.ToolPolicy.BlowupData;
            return n;
        }

        public static int QueueAccrue(GameConfig c, ModelState model)
        {
            if (!Enabled(c) || model.AgentLevel < 2 || model.ToolMask == 0) return 0;
            int n = 0;
            if ((model.ToolMask & Web) != 0) n += c.ToolPolicy.QueueWeb;
            if ((model.ToolMask & Code) != 0) n += c.ToolPolicy.QueueCode;
            if ((model.ToolMask & Data) != 0) n += c.ToolPolicy.QueueData;
            return n;
        }

        public static void Set(GameConfig c, GameState state, LabState lab, ModelState model, int mask)
        {
            if (c.ToolPolicy.SetupCost > 0) Finance.Spend(lab, LedgerLine.Training, c.ToolPolicy.SetupCost);
            if (AgentSandbox.Enabled(c) && model.AgentLevel > 0 && model.SandGrad != 1)
                mask &= Web;
            model.ToolMask = mask;
            Events.Add(state, EventKind.ToolPolicySet, lab.Id, model.Id, mask, model.Name);
        }
    }
}
