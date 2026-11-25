using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult NoteGate(GameConfig c, GameState state, LabState lab, bool needNote)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can issue a convertible note.");
            if (!Notes.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody lends on convertible notes in this game.");
            if (needNote && !Notes.Owing(lab)) return Fail(CommandError.InvalidArgument, "The lab has no convertible note.");
            return CommandResult.Done(-1);
        }

        static CommandResult IssueNote(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = NoteGate(c, state, lab, false);
            if (!gate.Ok) return gate;
            if (Notes.Owing(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a convertible note.");
            if (cmd.TargetId < 1 || cmd.TargetId > 3) return Fail(CommandError.InvalidArgument, "The note is tier 1, 2 or 3.");
            if (cmd.Count < 1 || cmd.Count > c.ConvertibleNotes.MaxThousands) return Fail(CommandError.InvalidArgument, "A note is from 1 to " + c.ConvertibleNotes.MaxThousands + " thousand.");
            Notes.Issue(c, state, lab, cmd.TargetId, cmd.Count * 1000L);
            return CommandResult.Done(-1);
        }

        static CommandResult RepayNote(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = NoteGate(c, state, lab, true);
            if (!gate.Ok) return gate;
            if (lab.Cash < lab.NoteOwed) return NeedCash(lab, lab.NoteOwed);
            Notes.Repay(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult ExtendNote(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = NoteGate(c, state, lab, true);
            if (!gate.Ok) return gate;
            if (lab.NoteExtended == 1) return Fail(CommandError.InvalidArgument, "The note has been extended once already.");
            if (c.ConvertibleNotes.ExtendWeeks < 1) return Fail(CommandError.InvalidArgument, "The holders do not extend.");
            Notes.Extend(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
