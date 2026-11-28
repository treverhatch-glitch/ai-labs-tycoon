using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Public eval cadence on released models. See <see cref="EvalHarnessConfig"/>. Only the player's lab.</summary>
    public static class EvalHarness
    {
        public static bool Enabled(GameConfig c) { return c.EvalHarness.Enabled > 0; }

        public static int Bar(GameConfig c, int suite) { return suite == 2 ? c.EvalHarness.BarBroad : c.EvalHarness.BarNarrow; }

        public static int Score(GameConfig c, ModelState model)
        {
            int raw = (model.Capability + model.Alignment) / 2;
            if (Knowledge.Enabled(c) && model.DocLevel > 0) raw += model.DocLevel * 20;
            return (int)MathI.Min(1000, raw);
        }

        public static void Register(GameConfig c, GameState state, LabState lab, ModelState model, int suite)
        {
            long listCost = c.EvalHarness.Cost;
            long paid = EvalHarnessEraRelief.RegisterCost(c, state, listCost);
            if (paid > 0) Finance.Spend(lab, LedgerLine.Training, paid);
            lab.ResearchPoints -= c.EvalHarness.Points;
            model.EvalSuite = suite;
            model.EvalLeft = c.EvalHarness.Cadence;
            model.EvalScore = 0;
            model.EvalStreak = 0;
            GameEvent e = Events.Add(state, EventKind.EvalRegistered, lab.Id, model.Id, c.EvalHarness.Cadence, model.Name);
            e.Pick = suite;
            EvalHarnessEraBoost.OnRegister(c, state, lab, model);
            EvalHarnessEraRelief.OnRegister(c, state, lab, model, listCost, paid);
        }

        public static void Publish(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            int score = Score(c, model);
            model.EvalScore = score;
            model.EvalRuns++;
            bool beat = score >= Bar(c, model.EvalSuite);
            if (beat)
            {
                model.EvalStreak++;
                for (int s = 0; s < model.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, model.Sectors[s], c.EvalHarness.TrustGain, false);
            }
            else
            {
                model.EvalStreak = 0;
                int listLoss = c.EvalHarness.TrustLoss;
                int applied = EvalFailEraRelief.TrustLoss(c, state, listLoss);
                for (int s = 0; s < model.Sectors.Count; s++) TrustSystem.Shift(c, state, lab, model.Sectors[s], applied, false);
                EvalFailEraRelief.OnFailed(c, state, lab, model, listLoss, applied);
                if (model.CertStage == Conformity.Review && model.CertLeft > 0) model.CertLeft += c.EvalHarness.ConformityPush;
                if (c.EvalHarness.FailTension > 0) BoardMotions.AddTension(c, lab, c.EvalHarness.FailTension);
            }
            model.EvalLeft = c.EvalHarness.Cadence;
            GameEvent e = Events.Add(state, EventKind.EvalPublished, lab.Id, model.Id, score, model.Name);
            e.Pick = beat ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.EvalSuite == 0) continue;
                if (m.Status != ModelStatus.Released || m.Sectors.Count == 0)
                {
                    m.EvalSuite = 0;
                    m.EvalLeft = 0;
                    m.EvalStreak = 0;
                    continue;
                }
                if (m.EvalLeft > 0) m.EvalLeft--;
                if (m.EvalLeft > 0) continue;
                Publish(c, state, lab, m);
            }
        }
    }
}
