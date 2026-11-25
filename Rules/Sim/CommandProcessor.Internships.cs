using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StartInternships(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab takes interns.");
            if (!Internships.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no internships in this game.");
            RoleConfig role = c.Role(cmd.Id);
            if (role == null) return Fail(CommandError.UnknownId, "There is no role \"" + cmd.Id + "\".");
            if (Internships.Active(lab)) return Fail(CommandError.InvalidArgument, "A cohort of interns is already in the lab.");
            if (cmd.Count < 1 || cmd.Count > c.Internships.MaxCohort) return Fail(CommandError.InvalidArgument, "A cohort is from 1 to " + c.Internships.MaxCohort + " interns.");
            long total = Internships.TotalStipend(c, state, lab, role.Id, cmd.Count);
            if (lab.Cash < total) return NeedCash(lab, total);
            Internships.Start(c, state, lab, role.Id, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult EndInternships(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab takes interns.");
            if (!Internships.Active(lab)) return Fail(CommandError.InvalidArgument, "There are no interns in the lab.");
            Internships.Finish(c, state, lab, true);
            return CommandResult.Done(-1);
        }
    }
}
