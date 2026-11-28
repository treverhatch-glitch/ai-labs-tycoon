using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class EvalHarnessEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.EvalHarnessEraRelief.Enabled > 0 && EvalHarness.Enabled(c); }

        public static long RegisterCost(GameConfig c, GameState state, long cost)
        {
            if (!Enabled(c) || state == null || cost <= 0) return cost;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.EvalHarnessEraRelief.MinEra) return cost;
            int cut = c.EvalHarnessEraRelief.ReliefPermille;
            if (cut <= 0) return cost;
            return MathI.MulPermille(cost, MathI.Permille - cut);
        }

        public static void OnRegister(GameConfig c, GameState state, LabState lab, ModelState model, long listCost, long paid)
        {
            if (!Enabled(c) || paid >= listCost) return;
            long saved = listCost - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.EvalHarnessEraRelieved, lab.Id, model.Id, saved, model.Name);
        }
    }
}