using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SubmitPaper(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab submits papers.");
            if (!Conference.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no conference submissions in this game.");
            if (cmd.TargetId < Conference.Workshop || cmd.TargetId > Conference.Flagship) return Fail(CommandError.InvalidArgument, "A venue is 1 (workshop), 2 (main conference) or 3 (flagship).");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "Effort is from 1 to 3.");
            if (lab.Submissions.Count >= c.Conference.MaxPending) return Fail(CommandError.InvalidArgument, "The lab already has " + lab.Submissions.Count + " papers out for review.");
            if (Prestige.Writers(c, lab) < c.Prestige.MinResearchers) return Fail(CommandError.NotEnoughStaff, "A paper needs at least " + c.Prestige.MinResearchers + " researchers.");
            long cost = Conference.Cost(c, state, lab, cmd.TargetId, cmd.Count);
            if (lab.ResearchPoints < cost) return Fail(CommandError.NotEnoughResearch, "That paper costs " + cost + " research points and the lab has " + lab.ResearchPoints + ".");
            Conference.Submit(c, state, lab, cmd.TargetId, cmd.Count, cost);
            return CommandResult.Done(-1);
        }

        static CommandResult WithdrawPaper(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab submits papers.");
            if (cmd.TargetId < 0 || cmd.TargetId >= lab.Submissions.Count) return Fail(CommandError.UnknownId, "There is no paper " + cmd.TargetId + " out for review.");
            Conference.Withdraw(c, state, lab, lab.Submissions[cmd.TargetId]);
            return CommandResult.Done(-1);
        }
    }
}
