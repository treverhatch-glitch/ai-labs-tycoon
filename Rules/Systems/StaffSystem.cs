using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class StaffSystem
    {
        public static void Hire(GameConfig c, GameState state, LabState lab, RoleConfig role, int count)
        {
            StaffPeopleConfig cfg = c.StaffPeople;
            string steady = DefaultTrait(cfg);
            string star = StarTrait(cfg);
            for (int i = 0; i < count; i++)
            {
                var person = new StaffMember();
                person.Id = state.TakeId();
                person.RoleId = role.Id;
                person.Morale = cfg.MoraleBaseline;
                person.TraitId = star != null && state.Rng.World.Chance(cfg.StarHirePermille) ? star : steady;
                lab.Staff.Add(person);
            }
        }

        public static void Fire(LabState lab, string roleId, int count)
        {
            for (int removed = 0; removed < count; removed++)
            {
                int idx = LowestMoraleIndex(lab, roleId);
                if (idx < 0) break;
                lab.Staff.RemoveAt(idx);
            }
        }

        static int LowestMoraleIndex(LabState lab, string roleId)
        {
            int pick = -1;
            int morale = int.MaxValue;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember p = lab.Staff[i];
                if (p.RoleId != roleId) continue;
                if (p.Morale < morale) { morale = p.Morale; pick = i; }
            }
            return pick;
        }

        public static void RunWeek(GameConfig c, LabState lab)
        {
            int baseline = c.StaffPeople.MoraleBaseline;
            int drift = c.StaffPeople.MoraleDriftPerWeek;
            int debtDrift = c.StaffPeople.MoraleDebtDriftPerWeek;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember p = lab.Staff[i];
                if (p.Morale < baseline) p.Morale = (int)MathI.Min(baseline, p.Morale + drift);
                else if (p.Morale > baseline) p.Morale = (int)MathI.Max(baseline, p.Morale - drift);
                if (lab.WeeksInDebt > 0 && debtDrift > 0)
                    p.Morale = (int)MathI.Max(0, p.Morale - debtDrift);
            }
        }

        public static void OnIncident(GameConfig c, LabState lab)
        {
            int drop = c.StaffPeople.MoraleIncidentDrop;
            if (drop <= 0) return;
            int mit = MoraleIncidentMitigationPermille(c, lab);
            if (mit > 0) drop = (int)MathI.MulPermille(drop, MathI.Permille - mit);
            if (drop < 1) return;
            for (int i = 0; i < lab.Staff.Count; i++)
                lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - drop);
        }

        static void OnPoachWitness(GameConfig c, LabState lab)
        {
            int drop = c.StaffPeople.MoralePoachWitnessDrop;
            if (drop <= 0) return;
            for (int i = 0; i < lab.Staff.Count; i++)
                lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - drop);
        }

        public static void TryPoach(GameConfig c, GameState state, LabState rival, LabState player)
        {
            if (player == null || player.Bankrupt || player.Staff.Count == 0) return;
            if (!RivalSystem.IsFeud(c, rival)) return;
            int chance = c.StaffPeople.PoachChancePermille - MoralePoachResistPermille(c, player);
            if (chance < 1) return;
            if (!state.Rng.Rivals.Chance(chance)) return;

            StaffMember target = BestPoachTarget(c, player);
            if (target == null) return;
            player.Staff.Remove(target);
            OnPoachWitness(c, player);
            Events.Add(state, EventKind.StaffPoached, rival.Id, player.Id, 1, target.RoleId);
            DecisionLog.Add(c, rival, state.Tick, DecisionAction.PoachedStaff, target.RoleId, 1,
                Reason.Of(ReasonCode.FeelingTowardPlayer, rival.FeelingToPlayer));
        }

        static StaffMember BestPoachTarget(GameConfig c, LabState lab)
        {
            StaffMember best = null;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember p = lab.Staff[i];
                StaffTraitConfig trait = c.StaffPeople.Trait(p.TraitId);
                if (trait == null || !trait.Star) continue;
                if (best == null || p.Morale < best.Morale) best = p;
            }
            return best;
        }

        public static int AverageMorale(LabState lab)
        {
            if (lab.Staff.Count < 1) return 0;
            long sum = 0;
            for (int i = 0; i < lab.Staff.Count; i++) sum += lab.Staff[i].Morale;
            return (int)(sum / lab.Staff.Count);
        }

        /// <summary>Permille hook from average morale versus baseline (negative when the team is unhappy).</summary>
        public static int MoraleDeltaPermille(GameConfig c, LabState lab, int permillePerPoint)
        {
            StaffPeopleConfig cfg = c.StaffPeople;
            if (cfg == null || permillePerPoint < 1 || lab.Staff.Count < 1) return 0;
            int delta = AverageMorale(lab) - cfg.MoraleBaseline;
            return delta * permillePerPoint;
        }

        public static int MoraleResearchPermille(GameConfig c, LabState lab) =>
            MoraleDeltaPermille(c, lab, c.StaffPeople?.MoraleResearchPermillePerPoint ?? 0);

        public static int MoraleAlignmentPermille(GameConfig c, LabState lab) =>
            MoraleDeltaPermille(c, lab, c.StaffPeople?.MoraleAlignmentPermillePerPoint ?? 0);

        public static int MoraleTrainingPermille(GameConfig c, LabState lab) =>
            MoraleDeltaPermille(c, lab, c.StaffPeople?.MoraleTrainingPermillePerPoint ?? 0);

        /// <summary>Flat permille cut to weekly collapse risk from happy staff (points, not a multiplier).</summary>
        public static int MoraleFailCutPermille(GameConfig c, LabState lab)
        {
            StaffPeopleConfig cfg = c.StaffPeople;
            if (cfg == null || cfg.MoraleFailCutPermillePerPoint < 1 || lab.Staff.Count < 1) return 0;
            int delta = AverageMorale(lab) - cfg.MoraleBaseline;
            if (delta <= 0) return 0;
            return delta * cfg.MoraleFailCutPermillePerPoint;
        }

        public static int MoralePoachResistPermille(GameConfig c, LabState lab)
        {
            StaffPeopleConfig cfg = c.StaffPeople;
            if (cfg == null || cfg.MoralePoachResistPermillePerPoint < 1 || lab.Staff.Count < 1) return 0;
            int delta = AverageMorale(lab) - cfg.MoraleBaseline;
            if (delta <= 0) return 0;
            return delta * cfg.MoralePoachResistPermillePerPoint;
        }

        public static int MoraleIncidentMitigationPermille(GameConfig c, LabState lab)
        {
            StaffPeopleConfig cfg = c.StaffPeople;
            if (cfg == null || cfg.MoraleIncidentMitigationPermillePerPoint < 1 || lab.Staff.Count < 1) return 0;
            int delta = AverageMorale(lab) - cfg.MoraleBaseline;
            if (delta <= 0) return 0;
            return (int)MathI.Min(MathI.Permille, delta * cfg.MoraleIncidentMitigationPermillePerPoint);
        }

        public static string DefaultTrait(StaffPeopleConfig cfg)
        {
            for (int i = 0; i < cfg.Traits.Count; i++) if (!cfg.Traits[i].Star) return cfg.Traits[i].Id;
            return cfg.Traits.Count > 0 ? cfg.Traits[0].Id : "";
        }

        static string StarTrait(StaffPeopleConfig cfg)
        {
            for (int i = 0; i < cfg.Traits.Count; i++) if (cfg.Traits[i].Star) return cfg.Traits[i].Id;
            return null;
        }

    }
}