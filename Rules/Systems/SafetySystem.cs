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
            int chance = (int)MathI.Min(MathI.Permille, IncidentChanceWithoutRush(c, tick, model) + RushedLaunch.IncidentPermille(c, tick, model));
            return chance + RedTeam.IncidentPermille(c, tick, model, chance);
        }

        /// <summary>
        /// The extra weekly chance of an incident while the lab cannot serve its customers (the creature is overworked and reliability drops):
        /// the switch value scaled by the share of last week's demand left unserved. Only the player's labs unless the data says so.
        /// </summary>
        public static int OverloadPermille(GameConfig c, GameState state, LabState lab)
        {
            if (c.Safety.OverloadIncidentPermille <= 0) return 0;
            if (lab.Id != state.PlayerLabId && !c.Safety.OverloadForRivals) return 0;
            if (lab.LastWeek.ServingDemand <= 0 || lab.LastWeek.ServedPermille >= MathI.Permille) return 0;
            int unserved = (int)MathI.Clamp(MathI.Permille - lab.LastWeek.ServedPermille, 0, MathI.Permille);
            return (int)MathI.MulPermille(c.Safety.OverloadIncidentPermille, unserved);
        }

        /// <summary>The chance the weekly roll uses: the model's own plus what an overloaded lab adds.</summary>
        public static int WeeklyChancePermille(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            long before = IncidentChancePermille(c, state.Tick, model) + OverloadPermille(c, state, lab) + Crunch.IncidentPermille(c, lab) + Optimisation.IncidentPermille(c, lab) + TechDebt.IncidentPermille(c, lab);
            long chance = IncidentFloor.Chance(c, before, before + Chiefs.IncidentChange(c, lab, before) + BugBounty.IncidentChange(c, lab, before));
            return (int)MathI.MulPermille(chance, MathI.Permille - Conformity.ChanceCutPermille(c, model));
        }

        /// <summary>The weekly chance before a rushed launch adds to it.</summary>
        public static int IncidentChanceWithoutRush(GameConfig c, int tick, ModelState model)
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
                if (!state.Rng.Safety.Chance(WeeklyChancePermille(c, state, lab, model))) continue;
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
            SectorFlavourConfig flavour = SectorFlavours.Leading(c, state, lab, model);
            if (flavour != null) fine = MathI.MulPermille(fine, flavour.FinePermille);
            RiskClassConfig risk = RiskClasses.Draw(c, state, lab, model);
            if (risk != null) fine = MathI.MulPermille(fine, risk.FinePermille);
            fine = EraRules.Fine(c, fine, state.Tick);
            fine = MathI.MulPermille(fine, MathI.Permille - Licence.FineCutPermille(c, lab));
            long gross = fine;
            fine = IncidentFloor.CutFine(c, gross, MathI.MulPermille(fine, MathI.Permille - OnCall.FineCutPermille(c, lab)));
            long makeGood = SectorFlavours.MakeGood(c, state, lab, model);
            long covered = Insurance.OnIncident(c, state, lab, model, rung, fine, IncidentFloor.MaxPayout(c, gross, fine));
            fine -= covered;

            model.Incidents++;
            model.LastIncidentRung = rung;
            lab.IncidentsThisMonth++;
            model.LastIncidentTick = state.Tick;

            int repLoss = def == null ? c.Safety.IncidentReputationLoss : def.ReputationLoss;
            if (risk != null) repLoss = (int)MathI.MulPermille(repLoss, risk.ReputationPermille);
            if (flavour != null) repLoss = (int)MathI.MulPermille(repLoss, flavour.ReputationPermille);
            repLoss = PressSystem.Damp(c, lab, repLoss);
            repLoss = (int)MathI.MulPermille(repLoss, MathI.Permille - EthicsBoard.ScandalCutPermille(c, lab));
            repLoss = (int)MathI.MulPermille(repLoss, MathI.Permille - OnCall.ReputationCutPermille(c, lab));
            int repBefore = lab.Reputation;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - repLoss);
            model.IncidentRepLoss = repBefore - lab.Reputation;
            model.PostMortemDone = 0;
            Finance.Spend(lab, LedgerLine.Fines, fine + makeGood);

            if (def != null && def.ClosesGap)
                model.DisplayedAlignment = EffectiveAlignment(c, model);

            Licence.OnIncident(c, state, lab);
            StaffSystem.OnIncident(c, lab);
            TrustSystem.OnIncident(c, state, lab, model);
            SafetyAudit.OnIncident(state, lab, model);
            MarketingCampaign.OnIncident(c, state, lab, model);
            Disclosure.OnIncident(c, lab, model);

            Litigation.OnIncident(c, state, lab, model, fine + makeGood);
            GameEvent e = Events.Add(state, EventKind.Incident, lab.Id, model.Id, fine + makeGood, model.Name);
            if (risk != null)
            {
                e.RiskClass = risk.Id;
                e.Pick = state.Rng.Safety.Next(risk.Headlines.Count);
            }
            else e.Pick = state.Rng.Safety.Next(EraSystem.AtTick(c, state.Tick).IncidentHeadlines.Count);
            PressSystem.OnBeat(c, state, lab, "incident", repLoss, model.Name);
            if (rung == 5) OfficeEvents.OnEscape(c, state, lab);
        }
    }
}