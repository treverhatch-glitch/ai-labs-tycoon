using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Incidents on the first three ladder rungs. A model on sale misbehaves now and then, and the emptier its
    /// safety meter the more often. Sandbagging widens the gap between displayed and true alignment until a rung 3 catches it.
    /// </summary>
    public static class SafetySystem
    {
        /// <summary>The weekly chance, in permille, that a released model has an incident.</summary>
        public static int IncidentChancePermille(GameConfig c, int tick, ModelState model)
        {
            int eraLoad = EraSystem.AtTick(c, tick).IncidentLoadPermille;
            if (eraLoad < 1) eraLoad = MathI.Permille;
            long baseChance = MathI.MulPermille(c.Safety.IncidentChancePermille, MathI.Permille - model.Alignment);
            long withCorruption = MathI.MulPermille(baseChance, MathI.Permille + model.Corruption * c.Shady.CorruptionIncidentWeight / MathI.Permille);
            return (int)MathI.Min(MathI.Permille, MathI.MulPermille(withCorruption, eraLoad));
        }

        public static int EffectiveAlignment(GameConfig c, ModelState model)
        {
            int trust = model.Alignment - (int)MathI.MulPermille(model.Corruption, c.Shady.CorruptionTrustWeight);
            return (int)MathI.Max(0, trust);
        }

        public static int DisplayedAlignment(ModelState model)
        {
            return (int)MathI.Min(MathI.Permille, model.DisplayedAlignment);
        }

        public static int AlignmentGap(GameConfig c, ModelState model)
        {
            return (int)MathI.Max(0, DisplayedAlignment(model) - model.Alignment);
        }

        public static int PickIncidentRung(GameConfig c, ModelState model)
        {
            if (c.Safety.IncidentRung(6) != null && model.Corruption >= c.Safety.Rung6CorruptionMin) return 6;
            if (c.Safety.IncidentRung(5) != null && model.Corruption >= c.Safety.Rung5CorruptionMin) return 5;
            if (c.Safety.IncidentRung(4) != null && model.Corruption >= c.Safety.Rung4CorruptionMin) return 4;
            if (AlignmentGap(c, model) >= c.Safety.SandbagGapPermille) return 3;
            if (model.Alignment < c.Safety.Rung2BelowAlignment) return 2;
            return 1;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            int baseline = c.Safety.ReputationBaseline;
            int step = c.Safety.ReputationRecoveryPerWeek;
            if (lab.Reputation < baseline) lab.Reputation = (int)MathI.Min(baseline, lab.Reputation + step);
            else if (lab.Reputation > baseline) lab.Reputation = (int)MathI.Max(baseline, lab.Reputation - step);

            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released) continue;
                if (!state.Rng.Safety.Chance(IncidentChancePermille(c, state.Tick, model))) continue;
                int caught = RolePerks.IncidentCatchPermille(c, lab);
                if (caught > 0 && state.Rng.Safety.Chance(caught)) continue;
                Incident(c, state, lab, model);
            }
        }

        static void Incident(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            int rung = PickIncidentRung(c, model);
            IncidentRungConfig def = c.Safety.IncidentRung(rung);
            if (def == null) def = c.Safety.IncidentRungs.Count > 0 ? c.Safety.IncidentRungs[0] : null;

            long fine = MathI.Max(c.Safety.IncidentFineMin, MarketSystem.ModelRevenue(state, lab.Id, model.Id) * c.Safety.IncidentFineRevenueWeeks);
            if (def != null) fine = MathI.MulPermille(fine, def.FinePermille);
            fine = MathI.Max(c.Safety.IncidentFineMin, fine);
            RiskClassConfig risk = RiskClasses.Draw(c, state, lab, model);
            if (risk != null) fine = MathI.MulPermille(fine, risk.FinePermille);
            fine = EraRules.Fine(c, fine, state.Tick);

            model.Incidents++;
            model.LastIncidentRung = rung;
            lab.IncidentsThisMonth++;
            model.LastIncidentTick = state.Tick;

            int repLoss = def == null ? c.Safety.IncidentReputationLoss : def.ReputationLoss;
            if (risk != null) repLoss = (int)MathI.MulPermille(repLoss, risk.ReputationPermille);
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - repLoss);
            Finance.Spend(lab, LedgerLine.Fines, fine);

            if (def != null && def.ClosesGap)
                model.DisplayedAlignment = EffectiveAlignment(c, model);

            StaffSystem.OnIncident(c, lab);

            GameEvent e = Events.Add(state, EventKind.Incident, lab.Id, model.Id, fine, model.Name);
            if (risk != null)
            {
                e.RiskClass = risk.Id;
                e.Pick = state.Rng.Safety.Next(risk.Headlines.Count);
            }
            else e.Pick = state.Rng.Safety.Next(EraSystem.AtTick(c, state.Tick).IncidentHeadlines.Count);
        }
    }
}