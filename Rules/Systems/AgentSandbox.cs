using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Per-model agent sandbox before full tools. See <see cref="AgentSandboxConfig"/>. Only the player's lab.</summary>
    public static class AgentSandbox
    {
        public static bool Enabled(GameConfig c) { return c.AgentSandbox.Enabled > 0; }

        public static int Score(GameConfig c, ModelState model)
        {
            int raw = (model.Capability + model.Alignment) / 2;
            if (Knowledge.Enabled(c) && model.DocLevel > 0) raw += model.DocLevel * 15;
            return (int)MathI.Min(1000, raw);
        }

        public static bool InSandbox(ModelState model) { return model.SandLeft > 0; }

        public static bool Graduated(ModelState model) { return model.SandGrad > 0; }

        static long Mix(GameState state, LabState lab, ModelState model)
        {
            long mix = (state.Seed * 53 + state.Tick) * 53 + lab.Id * 29 + model.Id * 11 + 3;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 10;
        }

        public static void Start(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (c.AgentSandbox.Cost > 0) Finance.Spend(lab, LedgerLine.Training, c.AgentSandbox.Cost);
            model.SandGrad = 0;
            model.SandLeft = c.AgentSandbox.Weeks;
            model.ToolMask &= ToolPolicy.Web;
            Events.Add(state, EventKind.SandboxStarted, lab.Id, model.Id, c.AgentSandbox.Weeks, model.Name);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.SandLeft <= 0) continue;
                m.SandLeft--;
                if (m.SandLeft > 0) continue;
                bool pass = Score(c, m) >= c.AgentSandbox.PassBar && (int)(Mix(state, lab, m) % MathI.Permille) < MathI.Min(MathI.Permille, Score(c, m));
                if (pass) m.SandGrad = 1;
                GameEvent e = Events.Add(state, EventKind.SandboxFinished, lab.Id, m.Id, Score(c, m), m.Name);
                e.Pick = pass ? 1 : 0;
            }
        }
    }
}