using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AppointChief(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab appoints chiefs.");
            if (!Chiefs.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no chief officers in this game.");
            if (!Chiefs.Known(cmd.Id)) return Fail(CommandError.UnknownId, "There is no chief title \"" + cmd.Id + "\".");

            if (cmd.TargetId < 0)
            {
                StaffMember sitting = Chiefs.Holder(c, lab, cmd.Id);
                if (sitting == null) return Fail(CommandError.InvalidArgument, "Nobody holds the " + cmd.Id + " title.");
                Chiefs.StepDown(c, state, lab, cmd.Id, sitting);
                return CommandResult.Done(-1);
            }

            StaffMember person = Retraining.Person(lab, cmd.TargetId);
            if (person == null) return Fail(CommandError.UnknownId, "There is nobody with id " + cmd.TargetId + " on the payroll.");
            RoleConfig role = c.Role(person.RoleId);
            if (role == null || role.Function != Chiefs.FunctionOf(cmd.Id))
                return Fail(CommandError.InvalidArgument, "The " + cmd.Id + " title goes to someone in a " + Chiefs.FunctionOf(cmd.Id) + " role.");
            if (lab.Staff.Count < c.Chiefs.MinStaff) return Fail(CommandError.NotEnoughStaff, "A chief needs a company of at least " + c.Chiefs.MinStaff + " people.");
            if (Chiefs.Holder(c, lab, cmd.Id) == person) return Fail(CommandError.InvalidArgument, "They already hold that title.");
            if (lab.Cash < c.Chiefs.Fee) return NeedCash(lab, c.Chiefs.Fee);
            Chiefs.Appoint(c, state, lab, cmd.Id, person, c.Chiefs.Fee);
            return CommandResult.Done(-1);
        }
    }
}
