using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult HoldBoardMeetings(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab holds board meetings.");
            if (!BoardMotions.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody holds board meetings in this game.");
            if (cmd.Count != 0 && cmd.Count != 1) return Fail(CommandError.InvalidArgument, "Meetings are on (1) or off (0).");
            if (cmd.Count == lab.MeetingsOn) return Fail(CommandError.InvalidArgument, cmd.Count == 1 ? "The board already meets." : "The board does not meet.");
            if (cmd.Count == 1 && lab.BoardInvestorSeats < c.BoardAgenda.MinSeats) return Fail(CommandError.MissingPrerequisite, "There are no investors on the board yet to hold a meeting with.");
            if (cmd.Count == 0 && (lab.MotionPend != 0 || lab.MotionKind != 0)) return Fail(CommandError.InvalidArgument, "Answer the motion and keep the commitment before the meetings stop.");
            BoardMotions.Hold(c, state, lab, cmd.Count == 1);
            return CommandResult.Done(-1);
        }

        static CommandResult AnswerMotion(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab answers board motions.");
            if (lab.MotionPend == 0) return Fail(CommandError.InvalidArgument, "No motion is on the table.");
            if (cmd.Count != 0 && cmd.Count != 1) return Fail(CommandError.InvalidArgument, "Accept (1) or decline (0).");
            BoardMotions.Answer(c, state, lab, cmd.Count == 1);
            return CommandResult.Done(-1);
        }
    }
}
