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
                person.TraitId = star != null && state.Rng.World.Chance(cfg.StarHirePermille) ? star : Traits.RollHireTrait(c, lab, person.Id, steady);
                person.Morale = Traits.RestingMorale(c, lab, person);
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
            int drift = c.StaffPeople.MoraleDriftPerWeek;
            int debtDrift = c.StaffPeople.MoraleDebtDriftPerWeek;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember p = lab.Staff[i];
                int baseline = Traits.RestingMorale(c, lab, p);
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
            for (int i = 0; i < lab.Staff.Count; i++)
                lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - drop);
        }

        public static void OnPoachWitness(GameConfig c, LabState lab)
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
            if (!state.Rng.Rivals.Chance(Workplace.PoachChancePermille(c, player))) return;

            if (c.Talent.OfferWeeks > 0 && player.PoachOffers.Count >= c.Talent.MaxOpenOffers) return;
            StaffMember target = BestPoachTarget(c, player);
            if (target == null) return;
            TalentSystem.MakeOffer(c, state, rival, player, target);
        }

        static StaffMember BestPoachTarget(GameConfig c, LabState lab)
        {
            StaffMember best = null;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember p = lab.Staff[i];
                StaffTraitConfig trait = c.StaffPeople.Trait(p.TraitId);
                if (trait == null || !(trait.Star || trait.OfferPermille > 0)) continue;
                if (TalentSystem.OfferFor(lab, p.Id) != null) continue;
                if (best == null || p.Morale < best.Morale) best = p;
            }
            return best;
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