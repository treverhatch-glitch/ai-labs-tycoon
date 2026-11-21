using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnswerPress(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            PressResponseConfig response = c.Press.Response(cmd.Id);
            if (response == null) return Fail(CommandError.UnknownId, "There is no press response \"" + cmd.Id + "\".");
            PressInquiry inquiry = PressSystem.Inquiry(lab, cmd.TargetId);
            if (inquiry == null) return Fail(CommandError.InquiryGone, "That journalist has stopped waiting.");
            long cost = PressSystem.AnswerCost(c, state.Tick, response);
            if (lab.Cash < cost) return NeedCash(lab, cost);

            if (cost > 0) Finance.Spend(lab, LedgerLine.Hiring, cost);
            lab.PressInquiries.Remove(inquiry);
            int chance = PressSystem.AnswerChance(c, lab, inquiry, response).Total;
            bool works = state.Rng.Rivals.Chance(chance);
            int moved = works ? PressSystem.RecoverPoints(inquiry, response) : -PressSystem.BackfirePoints(c, inquiry, response);
            int before = lab.Reputation;
            PressSystem.AddReputation(lab, moved);
            GameEvent e = Events.Add(state, EventKind.PressAnswered, lab.Id, inquiry.Id, lab.Reputation - before, response.Id + "|" + inquiry.JournalistId + "|" + inquiry.BeatId);
            e.Pick = works ? 1 : 0;
            return CommandResult.Done(-1);
        }

        static CommandResult LeakStory(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            LabState rival = state.Lab(cmd.TargetId);
            if (rival == null || rival.Id == lab.Id) return Fail(CommandError.UnknownLab, "There is no rival lab with that id.");
            if (rival.Controller != LabController.Plan || rival.Bankrupt || rival.AbsorbedByLabId >= 0)
                return Fail(CommandError.TargetNotEligible, "That lab is not in the news.");
            if (!PressSystem.IsLeakable(c, state, rival))
                return Fail(CommandError.TargetNotEligible, rival.Name + " has had no incident lately, so there is nothing to leak.");
            long fee = PressSystem.LeakFee(c, state.Tick);
            if (lab.Cash < fee) return NeedCash(lab, fee);

            string model = PressSystem.LastIncidentModelName(c, state, rival);
            Finance.Spend(lab, LedgerLine.Hiring, fee);
            bool runs = state.Rng.Rivals.Chance(PressSystem.LeakChance(c, lab).Total);
            if (runs)
            {
                int before = rival.Reputation;
                PressSystem.AddReputation(rival, -PressSystem.Damp(c, rival, c.Press.LeakHitPoints));
                RivalSystem.Feel(c, rival, -c.Press.LeakFeelingHit);
                Events.Add(state, EventKind.StoryLeaked, lab.Id, rival.Id, before - rival.Reputation, model).Pick = 1;
                return CommandResult.Done(rival.Id);
            }
            int own = lab.Reputation;
            PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Press.LeakBackfirePoints));
            RivalSystem.Feel(c, rival, -c.Press.LeakFailFeelingHit);
            Events.Add(state, EventKind.StoryLeaked, lab.Id, rival.Id, own - lab.Reputation, model).Pick = 0;
            return CommandResult.Done(-1);
        }
    }
}
