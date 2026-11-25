using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AssignMentor(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab pairs people up.");
            if (!Mentoring.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no mentoring in this game.");
            StaffMember mentor = Seniority.Person(lab, cmd.Count);
            StaffMember mentee = Seniority.Person(lab, cmd.TargetId);
            if (mentor == null) return Fail(CommandError.UnknownId, "The lab has nobody with the id " + cmd.Count + ".");
            if (mentee == null) return Fail(CommandError.UnknownId, "The lab has nobody with the id " + cmd.TargetId + ".");
            if (mentor.Id == mentee.Id) return Fail(CommandError.InvalidArgument, "Nobody mentors themselves.");
            RoleConfig mentorRole = c.Role(mentor.RoleId);
            RoleConfig menteeRole = c.Role(mentee.RoleId);
            if (mentorRole == null || menteeRole == null) return Fail(CommandError.UnknownId, "There is no such role.");
            if (mentor.Senior != 1) return Fail(CommandError.InvalidArgument, "Only a senior can mentor.");
            if (mentee.Senior == 1) return Fail(CommandError.InvalidArgument, "A senior has nothing to be mentored towards.");
            if (mentorRole.Function != menteeRole.Function) return Fail(CommandError.InvalidArgument, "A mentor and a junior need to work in the same function.");
            if (mentee.MentorId > 0) return Fail(CommandError.InvalidArgument, "That person already has a mentor.");
            if (Mentoring.IsMentor(lab, mentor)) return Fail(CommandError.InvalidArgument, "That senior is already mentoring someone.");
            if (Mentoring.Pairs(lab) >= c.Mentoring.MaxPairs) return Fail(CommandError.InvalidArgument, "The lab can run only " + c.Mentoring.MaxPairs + " mentoring pairs at once.");
            Mentoring.Start(c, state, lab, mentor, mentee);
            return CommandResult.Done(-1);
        }

        static CommandResult EndMentoring(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab pairs people up.");
            StaffMember mentee = Seniority.Person(lab, cmd.TargetId);
            if (mentee == null) return Fail(CommandError.UnknownId, "The lab has nobody with the id " + cmd.TargetId + ".");
            if (mentee.MentorId <= 0) return Fail(CommandError.InvalidArgument, "That person is not being mentored.");
            Mentoring.End(state, lab, mentee, 2);
            return CommandResult.Done(-1);
        }
    }
}
