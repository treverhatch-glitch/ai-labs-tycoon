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
        public static int IncidentChancePermille(GameConfig c, ModelState model, LabState lab = null)
        {
            long baseChance = MathI.MulPermille(c.Safety.IncidentChancePermille, MathI.Permille - model.Alignment);
            int chance = (int)MathI.MulPermille(baseChance, MathI.Permille + model.Corruption * c.Shady.CorruptionIncidentWeight / MathI.Permille);
            if (lab != null) chance = ApplySafetyStaffCut(c, lab, chance);
            return chance;
        }

        static int ApplySafetyStaffCut(GameConfig c, LabState lab, int chance)
        {
            int each = c.Safety.SafetyStaffIncidentCutPermillePerStaff;
            if (each < 1) return chance;
            int staff = LabQueries.StaffByFunction(c, lab, RoleFunction.Safety);
            if (staff < 1) return chance;
            int cut = (int)MathI.Min(MathI.Permille, staff * each);
            return (int)MathI.MulPermille(chance, MathI.Permille - cut);
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
            if (AlignmentGap(c, model) >= c.Safety.SandbagGapPermille) return 3;
            if (model.Alignment < c.Safety.Rung2BelowAlignment) return 2;
            return 1;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            int baseline = c.Safety.ReputationBaseline;
            int sales = LabQueries.StaffByFunction(c, lab, RoleFunction.Sales);
            int step = c.Safety.ReputationRecoveryPerWeek
                + sales * c.Safety.SalesReputationRecoveryPermillePerStaff;
            if (lab.Reputation < baseline) lab.Reputation = (int)MathI.Min(baseline, lab.Reputation + step);
            else if (lab.Reputation > baseline) lab.Reputation = (int)MathI.Max(baseline, lab.Reputation - step);

            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released) continue;
                if (state.Rng.Safety.Chance(IncidentChancePermille(c, model, lab))) Incident(c, state, lab, model);
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

            model.Incidents++;
            model.LastIncidentRung = rung;
            lab.IncidentsThisMonth++;
            model.LastIncidentTick = state.Tick;

            int repLoss = def == null ? c.Safety.IncidentReputationLoss : def.ReputationLoss;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - repLoss);
            Finance.Spend(lab, LedgerLine.Fines, fine);

            if (def != null && def.ClosesGap)
                model.DisplayedAlignment = EffectiveAlignment(c, model);

            StaffSystem.OnIncident(c, lab);

            EraConfig era = EraSystem.AtTick(c, state.Tick);
            string sectorId = model.Sectors.Count > 0 ? model.Sectors[0] : "";
            var sectorHeadlines = SafetyQueries.SectorIncidentHeadlines(c, sectorId);
            var headlines = sectorHeadlines ?? era.IncidentHeadlines;
            string headlineRef = sectorHeadlines != null ? sectorId : model.Name;
            GameEvent e = Events.Add(state, EventKind.Incident, lab.Id, model.Id, fine, headlineRef);
            e.Pick = headlines.Count > 0 ? state.Rng.Safety.Next(headlines.Count) : 0;
        }
    }
}