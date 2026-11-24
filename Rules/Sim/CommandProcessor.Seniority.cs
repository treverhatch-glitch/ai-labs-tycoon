using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PromoteStaff(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab promotes people.");
            if (!Seniority.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody is promoted in this game.");
            StaffMember person = Seniority.Person(lab, cmd.TargetId);
            if (person == null) return Fail(CommandError.UnknownId, "The lab has nobody with the id " + cmd.TargetId + ".");
            RoleConfig role = c.Role(person.RoleId);
            if (role == null) return Fail(CommandError.UnknownId, "There is no role \"" + person.RoleId + "\".");
            if (person.Senior == 1) return Fail(CommandError.InvalidArgument, "That person is already senior.");
            if (person.Morale < c.Seniority.MinMorale)
                return Fail(CommandError.InvalidArgument, "That person is too unhappy to take it (morale " + person.Morale + ", needs " + c.Seniority.MinMorale + ").");
            if (Seniority.Seniors(c, lab, role.Function) >= Seniority.Allowed(c, lab, role.Function))
                return Fail(CommandError.InvalidArgument, "The " + role.Function + " function cannot have more seniors than " + c.Seniority.MaxSharePermille / 10 + "% of its people.");
            long fee = Seniority.Fee(c, role);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Seniority.Promote(c, state, lab, person, role);
            return CommandResult.Done(-1);
        }
    }
}
