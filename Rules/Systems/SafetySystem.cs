using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Incidents. A model on sale misbehaves now and then, and the emptier its safety meter the more often.
    /// An incident costs reputation and a fine, and the model glitches for a few weeks.
    /// </summary>
    public static class SafetySystem
    {
        /// <summary>The weekly chance, in permille, that a released model has an incident.</summary>
        public static int IncidentChancePermille(GameConfig c, ModelState model)
        {
            long baseChance = MathI.MulPermille(c.Safety.IncidentChancePermille, MathI.Permille - model.Alignment);
            return (int)MathI.MulPermille(baseChance, MathI.Permille + model.Corruption * c.Shady.CorruptionIncidentWeight / MathI.Permille);
        }

        public static int EffectiveAlignment(GameConfig c, ModelState model)
        {
            int trust = model.Alignment - (int)MathI.MulPermille(model.Corruption, c.Shady.CorruptionTrustWeight);
            return (int)MathI.Max(0, trust);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            // Reputation heals first, so a week with an incident ends exactly one incident lower.
            int baseline = c.Safety.ReputationBaseline;
            int step = c.Safety.ReputationRecoveryPerWeek;
            if (lab.Reputation < baseline) lab.Reputation = (int)MathI.Min(baseline, lab.Reputation + step);
            else if (lab.Reputation > baseline) lab.Reputation = (int)MathI.Max(baseline, lab.Reputation - step);

            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released) continue;
                if (state.Rng.Safety.Chance(IncidentChancePermille(c, model))) Incident(c, state, lab, model);
            }
        }

        static void Incident(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            long fine = MathI.Max(c.Safety.IncidentFineMin, MarketSystem.ModelRevenue(state, lab.Id, model.Id) * c.Safety.IncidentFineRevenueWeeks);
            model.Incidents++;
            lab.IncidentsThisMonth++;
            model.LastIncidentTick = state.Tick;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - c.Safety.IncidentReputationLoss);
            Finance.Spend(lab, LedgerLine.Fines, fine);

            GameEvent e = Events.Add(state, EventKind.Incident, lab.Id, model.Id, fine, model.Name);
            e.Pick = state.Rng.Safety.Next(EraSystem.AtTick(c, state.Tick).IncidentHeadlines.Count);
        }
    }
}
