using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RetrainStaff(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab retrains its people.");
            if (!Retraining.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no retraining in this game.");
            StaffMember person = Retraining.Person(lab, cmd.TargetId);
            if (person == null) return Fail(CommandError.UnknownId, "There is nobody with id " + cmd.TargetId + " on the payroll.");
            RoleConfig to = c.Role(cmd.Id);
            if (to == null) return Fail(CommandError.UnknownId, "There is no role \"" + cmd.Id + "\".");
            RoleConfig from = c.Role(person.RoleId);
            if (from == null) return Fail(CommandError.UnknownId, "That person has a role the data does not have.");
            if (from.Id == to.Id) return Fail(CommandError.InvalidArgument, "They already work as a " + to.Name + ".");
            if (person.Morale < c.Retrain.MinMorale)
                return Fail(CommandError.TeamTooTired, "They are too low to learn a new job (morale " + person.Morale + ", needs " + c.Retrain.MinMorale + ").");
            if (LabQueries.Seats(c, lab, to.Id) - LabQueries.StaffCount(lab, to.Id) < 1)
                return Fail(CommandError.NoSeats, "There is no free seat for a " + to.Name + ". Build a room for them first.");
            long fee = Retraining.Fee(c, lab, from, to, state.Tick);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Retraining.Retrain(c, state, lab, person, to, fee);
            return CommandResult.Done(-1);
        }
    }
}
