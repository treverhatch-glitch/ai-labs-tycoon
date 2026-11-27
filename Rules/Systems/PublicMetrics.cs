using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Customer-visible uptime metrics per model. See <see cref="PublicMetricsConfig"/>. Only the player's lab.</summary>
    public static class PublicMetrics
    {
        public static bool Enabled(GameConfig c) { return c.PublicMetrics.Enabled > 0; }

        public static bool Publishing(ModelState model) { return model.PubOn > 0; }

        public static void Publish(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            long listCost = c.PublicMetrics.PublishCost;
            long paid = PublicMetricsEraDiscount.PublishCost(c, state, listCost);
            if (paid > 0) Finance.Spend(lab, LedgerLine.Training, paid);
            model.PubOn = 1;
            model.PubMiss = 0;
            model.PubScore = lab.LastWeek.ServedPermille;
            Events.Add(state, EventKind.MetricsPublished, lab.Id, model.Id, model.PubScore, model.Name);
            PublicMetricsEraLoad.OnPublish(c, state, lab, model);
            PublicMetricsEraDiscount.OnPublish(c, state, lab, model, listCost, paid);
        }

        public static void Stop(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            model.PubOn = 0;
            Events.Add(state, EventKind.MetricsStopped, lab.Id, model.Id, model.PubScore, model.Name);
        }

        public static void OnIncident(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            if (!Enabled(c) || !Publishing(model)) return;
            model.PubMiss++;
            for (int s = 0; s < model.Sectors.Count; s++)
                TrustSystem.Shift(c, state, lab, model.Sectors[s], c.PublicMetrics.MissTrust, false);
        }

        static void ShiftTrust(GameConfig c, GameState state, LabState lab, ModelState model, int score)
        {
            int delta = score >= c.PublicMetrics.Bar ? c.PublicMetrics.TrustGain : c.PublicMetrics.TrustLoss;
            if (delta == 0) return;
            for (int s = 0; s < model.Sectors.Count; s++)
                TrustSystem.Shift(c, state, lab, model.Sectors[s], delta, false);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            int served = lab.LastWeek.ServedPermille;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.PubOn <= 0) continue;
                if (m.Status != ModelStatus.Released || m.Sectors.Count == 0)
                {
                    m.PubOn = 0;
                    continue;
                }
                int before = m.PubScore;
                m.PubScore = served;
                if (before != m.PubScore) ShiftTrust(c, state, lab, m, m.PubScore);
                if (m.PubScore < c.PublicMetrics.Bar && m.CertStage == Conformity.Review && m.CertLeft > 0)
                    m.CertLeft += c.PublicMetrics.ConformityPush;
            }
        }
    }
}