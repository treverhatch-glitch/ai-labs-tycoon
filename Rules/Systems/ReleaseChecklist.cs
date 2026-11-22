using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
  public static class ReleaseChecklist
  {
        public const int Red = 1;
        public const int Docs = 2;
        public const int Eval = 4;
        public const int Policy = 8;
        public const int All = 15;

        public static bool Enabled(GameConfig c) { return c.ReleaseChecklist.Enabled > 0; }

        public static bool Blocks(GameConfig c, GameState state, ModelState model)
        {
            if (!Enabled(c) || c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.ReleaseChecklist.MinEra) return false;
            return (model.ChkDone & All) != All;
        }

        public static bool CanSign(GameConfig c, GameState state, ModelState model, int item)
        {
            if (item == Red) return RedTeam.Active(state, model);
            if (item == Docs) return model.DocLevel >= c.ReleaseChecklist.DocBar;
            if (item == Eval) return model.EvalSuite > 0 || model.EvalRuns > 0;
            if (item == Policy) return model.ToolMask > 0;
            return false;
        }

        public static void Sign(GameConfig c, GameState state, LabState lab, ModelState model, int item)
        {
            model.ChkDone |= item;
            Events.Add(state, EventKind.ChecklistSigned, lab.Id, model.Id, item, model.Name);
        }

        public static void OnFirstRelease(GameConfig c, ModelState model)
        {
            if (!Enabled(c)) return;
            model.ChkMissed = All & ~model.ChkDone;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || c.ReleaseChecklist.MissTrust >= 0) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released || m.ChkMissed == 0) continue;
                int bits = 0;
                for (int b = 1; b <= 8; b <<= 1) if ((m.ChkMissed & b) != 0) bits++;
                int loss = bits * c.ReleaseChecklist.MissTrust;
                for (int s = 0; s < m.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, m.Sectors[s], loss, false);
            }
        }
    }
}
