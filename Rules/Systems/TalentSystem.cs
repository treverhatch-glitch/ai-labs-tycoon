using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The talent market (bible ch. 7). A rival's offer to one of the player's people waits for an answer; the answer is a counter (money, compute,
    /// a title, a mission) that costs a share of the person's package and works with a chance that rises with the person's morale and the lab's name;
    /// an offer nobody answers is taken. The player can try the same on a rival's people, for a headhunter's fee and a package.
    /// </summary>
    public static class TalentSystem
    {
        /// <summary>A chance taken apart: parts that add up to the total, the last part being whatever the floor or the ceiling changed.</summary>
        public struct ChanceParts
        {
            public int Base;
            public int MoralePart;
            public int ReputationPart;
            public int Clamp;

            public int Total { get { return Base + MoralePart + ReputationPart + Clamp; } }
        }

        /// <summary>What a person is worth to anyone who wants them: the era's hiring fee times the package multiple, more for a star.</summary>
        public static long Package(GameConfig c, StaffMember person, int tick)
        {
            RoleConfig role = c.Role(person.RoleId);
            long fee = role == null ? 0 : EraRules.HireFee(c, role, tick);
            long package = fee * c.Talent.PackageMultiple;
            StaffTraitConfig trait = c.StaffPeople.Trait(person.TraitId);
            if (trait != null && trait.Star) package = MathI.MulPermille(package, c.Talent.StarPackagePermille);
            return package;
        }

        public static long CounterCost(GameConfig c, CounterConfig counter, long package)
        {
            return MathI.MulPermille(package, counter.CostPermille);
        }

        public static ChanceParts CounterChance(GameConfig c, LabState lab, StaffMember person, CounterConfig counter)
        {
            var parts = new ChanceParts();
            parts.Base = counter.BaseSuccessPermille + Prestige.CounterBonusPermille(c, lab);
            parts.MoralePart = (int)(((long)person.Morale - c.StaffPeople.MoraleBaseline) * counter.MoraleSlopePermille / MathI.Permille);
            parts.ReputationPart = (int)(((long)lab.Reputation - 500) * counter.ReputationSlopePermille / MathI.Permille);
            return Bounded(c, parts);
        }

        public static ChanceParts PoachBackChance(GameConfig c, LabState lab, StaffMember target)
        {
            var parts = new ChanceParts();
            TalentConfig t = c.Talent;
            parts.Base = t.PoachBackBasePermille;
            parts.MoralePart = (int)(((long)c.StaffPeople.MoraleBaseline - target.Morale) * t.PoachBackMoraleSlopePermille / MathI.Permille);
            parts.ReputationPart = (int)(((long)lab.Reputation - 500) * t.PoachBackReputationSlopePermille / MathI.Permille);
            return Bounded(c, parts);
        }

        static ChanceParts Bounded(GameConfig c, ChanceParts parts)
        {
            int raw = parts.Base + parts.MoralePart + parts.ReputationPart;
            int bounded = (int)MathI.Clamp(raw, c.Talent.MinSuccessPermille, MathI.Permille);
            parts.Clamp = bounded - raw;
            return parts;
        }

        public static PoachOffer OfferFor(LabState lab, int staffId)
        {
            for (int i = 0; i < lab.PoachOffers.Count; i++) if (lab.PoachOffers[i].StaffId == staffId) return lab.PoachOffers[i];
            return null;
        }

        public static PoachOffer Offer(LabState lab, int offerId)
        {
            for (int i = 0; i < lab.PoachOffers.Count; i++) if (lab.PoachOffers[i].Id == offerId) return lab.PoachOffers[i];
            return null;
        }

        public static StaffMember Person(LabState lab, int staffId)
        {
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].Id == staffId) return lab.Staff[i];
            return null;
        }

        /// <summary>A rival makes an offer to a person of the player's lab. Called when the rival's dice say it tries.</summary>
        public static void MakeOffer(GameConfig c, GameState state, LabState rival, LabState player, StaffMember target)
        {
            if (c.Talent.OfferWeeks <= 0)
            {
                Leave(c, state, rival, player, target);
                return;
            }
            var offer = new PoachOffer();
            offer.Id = state.TakeId();
            offer.StaffId = target.Id;
            offer.RivalLabId = rival.Id;
            offer.Package = Package(c, target, state.Tick);
            offer.ExpiresTick = state.Tick + c.Talent.OfferWeeks;
            player.PoachOffers.Add(offer);
            Events.Add(state, EventKind.PoachOffered, rival.Id, player.Id, offer.Package, target.RoleId);
        }

        /// <summary>The person goes. Their colleagues notice, and the rival writes it down.</summary>
        public static void Leave(GameConfig c, GameState state, LabState rival, LabState player, StaffMember person)
        {
            player.Staff.Remove(person);
            StaffSystem.OnPoachWitness(c, player);
            Events.Add(state, EventKind.StaffPoached, rival == null ? -1 : rival.Id, player.Id, 1, person.RoleId);
            if (rival != null)
                DecisionLog.Add(c, rival, state.Tick, DecisionAction.PoachedStaff, person.RoleId, 1,
                    Reason.Of(ReasonCode.FeelingTowardPlayer, rival.FeelingToPlayer));
        }

        /// <summary>An offer about someone who is no longer on the payroll (let go, say) is closed.</summary>
        public static void DropGoneOffers(LabState lab)
        {
            for (int i = lab.PoachOffers.Count - 1; i >= 0; i--)
                if (Person(lab, lab.PoachOffers[i].StaffId) == null) lab.PoachOffers.RemoveAt(i);
        }

        /// <summary>Each week: offers whose time is up are taken, and offers about someone who is already gone are dropped.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            for (int i = lab.PoachOffers.Count - 1; i >= 0; i--)
            {
                PoachOffer offer = lab.PoachOffers[i];
                StaffMember person = Person(lab, offer.StaffId);
                if (person == null) { lab.PoachOffers.RemoveAt(i); continue; }
                if (state.Tick < offer.ExpiresTick) continue;
                lab.PoachOffers.RemoveAt(i);
                Leave(c, state, state.Lab(offer.RivalLabId), lab, person);
            }
        }

        /// <summary>The person a rival would best be raided for: a star first, else the unhappiest of the role.</summary>
        public static StaffMember BestTarget(GameConfig c, LabState lab, string roleId)
        {
            StaffMember best = null;
            bool bestStar = false;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember p = lab.Staff[i];
                if (p.RoleId != roleId) continue;
                StaffTraitConfig trait = c.StaffPeople.Trait(p.TraitId);
                bool star = trait != null && trait.Star;
                if (best == null || (star && !bestStar) || (star == bestStar && p.Morale < best.Morale)) { best = p; bestStar = star; }
            }
            return best;
        }
    }
}
