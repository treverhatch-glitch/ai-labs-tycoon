using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SendOnSabbatical(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab sends people on sabbatical.");
            if (!Sabbatical.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no sabbaticals in this game.");
            StaffMember person = Sabbatical.Find(lab, cmd.TargetId);
            if (person == null) return Fail(CommandError.UnknownId, "There is nobody " + cmd.TargetId + " on the payroll.");
            if (person.LeaveDue > 0) return Fail(CommandError.InvalidArgument, "That person is already away.");
            if (person.MentorId != 0 || Mentoring.IsMentor(lab, person)) return Fail(CommandError.InvalidArgument, "That person is in a mentoring pair.");
            if (Skunkworks.Away(c, lab, person) || AccountTeams.Away(c, lab, person) || SupportDesk.Away(c, lab, person)) return Fail(CommandError.InvalidArgument, "That person is already working away from their desk.");
            if (Sabbatical.Away(lab) >= c.Sabbatical.MaxAway) return Fail(CommandError.InvalidArgument, c.Sabbatical.MaxAway + " people are already away.");
            Sabbatical.Send(c, state, lab, person);
            return CommandResult.Done(-1);
        }

        static CommandResult RecallFromSabbatical(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab sends people on sabbatical.");
            StaffMember person = Sabbatical.Find(lab, cmd.TargetId);
            if (person == null) return Fail(CommandError.UnknownId, "There is nobody " + cmd.TargetId + " on the payroll.");
            if (person.LeaveDue == 0) return Fail(CommandError.InvalidArgument, "That person is not away.");
            Sabbatical.Return(c, state, lab, person, true);
            return CommandResult.Done(-1);
        }
    }
}
