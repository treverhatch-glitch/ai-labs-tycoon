using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult CounterPoach(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CounterConfig counter = c.Talent.Counter(cmd.Id);
            if (counter == null) return Fail(CommandError.UnknownId, "There is no counter \"" + cmd.Id + "\".");
            PoachOffer offer = TalentSystem.Offer(lab, cmd.TargetId);
            if (offer == null) return Fail(CommandError.OfferGone, "That offer is not open any more.");
            StaffMember person = TalentSystem.Person(lab, offer.StaffId);
            if (person == null)
            {
                lab.PoachOffers.Remove(offer);
                return Fail(CommandError.OfferGone, "That person has already gone.");
            }
            long cost = TalentSystem.CounterCost(c, counter, offer.Package);
            if (lab.Cash < cost) return NeedCash(lab, cost);

            if (cost > 0) Finance.Spend(lab, LedgerLine.Hiring, cost);
            lab.PoachOffers.Remove(offer);
            int chance = TalentSystem.CounterChance(c, lab, person, counter).Total;
            bool stays = state.Rng.Rivals.Chance(chance);
            Events.Add(state, EventKind.PoachCountered, lab.Id, offer.RivalLabId, cost, counter.Id).Pick = stays ? 1 : 0;
            if (!stays)
            {
                TalentSystem.Leave(c, state, state.Lab(offer.RivalLabId), lab, person);
                return CommandResult.Done(-1);
            }
            person.Morale = (int)MathI.Min(MathI.Permille, person.Morale + counter.MoraleGain);
            return CommandResult.Done(person.Id);
        }

        static CommandResult PoachStaff(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            RoleConfig role = c.Role(cmd.Id);
            if (role == null) return Fail(CommandError.UnknownId, "There is no role \"" + cmd.Id + "\".");
            LabState rival = state.Lab(cmd.TargetId);
            if (rival == null || rival.Id == lab.Id) return Fail(CommandError.UnknownLab, "There is no rival lab with that id.");
            if (rival.Controller != LabController.Plan || rival.Bankrupt || rival.AbsorbedByLabId >= 0)
                return Fail(CommandError.TargetNotEligible, "That lab has no people to poach.");
            StaffMember target = TalentSystem.BestTarget(c, rival, role.Id);
            if (target == null) return Fail(CommandError.NotEnoughStaff, rival.Name + " has no " + role.Name + " to poach.");
            int free = LabQueries.FreeSeats(c, lab, role.Id);
            if (free < 1) return Fail(CommandError.NoSeats, "There is no free seat for a " + role.Name + ". Build a room for them first.");

            long package = TalentSystem.Package(c, target, state.Tick);
            long fee = MathI.MulPermille(package, c.Talent.PoachBackFeePermille);
            if (lab.Cash < fee + package) return NeedCash(lab, fee + package);

            int chance = TalentSystem.PoachBackChance(c, lab, target).Total;
            bool works = state.Rng.Rivals.Chance(chance);
            Finance.Spend(lab, LedgerLine.Hiring, fee);
            if (!works)
            {
                RivalSystem.Feel(c, rival, -c.Talent.PoachBackFailFeelingHit);
                Events.Add(state, EventKind.PoachBackTried, lab.Id, rival.Id, fee, role.Id).Pick = 0;
                return CommandResult.Done(-1);
            }
            Finance.Spend(lab, LedgerLine.Hiring, package);
            rival.Staff.Remove(target);
            target.Morale = c.StaffPeople.MoraleBaseline;
            lab.Staff.Add(target);
            RivalSystem.Feel(c, rival, -c.Talent.PoachBackFeelingHit);
            Events.Add(state, EventKind.PoachBackTried, lab.Id, rival.Id, fee + package, role.Id).Pick = 1;
            return CommandResult.Done(target.Id);
        }
    }
}
