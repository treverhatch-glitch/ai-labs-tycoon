using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnswerComplaint(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ComplaintResponseConfig response = c.Trust.Response(cmd.Id);
            if (response == null) return Fail(CommandError.UnknownId, "There is no way to answer a complaint called \"" + cmd.Id + "\".");
            Complaint complaint = TrustSystem.Complaint(lab, cmd.TargetId);
            if (complaint == null) return Fail(CommandError.ComplaintGone, "That customer has stopped waiting.");
            long cost = TrustSystem.AnswerCost(c, state, lab, complaint, response);
            if (lab.Cash < cost) return NeedCash(lab, cost);

            if (cost > 0) Finance.Spend(lab, LedgerLine.Hiring, cost);
            lab.Complaints.Remove(complaint);
            bool works = state.Rng.Rivals.Chance(TrustSystem.AnswerChance(c, lab, response).Total);
            int points = works
                ? (int)MathI.MulPermille(complaint.Stake, response.RestorePermille)
                : -(int)MathI.MulPermille(complaint.Stake, response.BackfirePermille);
            int moved = TrustSystem.Shift(c, state, lab, complaint.SectorId, points, false);
            Events.Add(state, EventKind.ComplaintAnswered, lab.Id, complaint.Id, moved, response.Id + "|" + complaint.SectorId).Pick = works ? 1 : 0;
            return CommandResult.Done(-1);
        }
    }
}
