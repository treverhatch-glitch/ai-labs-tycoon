using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RehireAlumnus(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has an alumni network.");
            if (!Alumni.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no alumni network in this game.");
            Alumnus gone = Alumni.Find(lab, cmd.TargetId);
            if (gone == null) return Fail(CommandError.UnknownId, "There is no former employee " + cmd.TargetId + " to ask back.");
            RoleConfig role = c.Role(gone.RoleId);
            if (role == null) return Fail(CommandError.UnknownId, "There is no role \"" + gone.RoleId + "\".");
            int free = LabQueries.FreeSeats(c, lab, role.Id);
            if (free < 1) return Fail(CommandError.NoSeats, "There is no free seat for a " + role.Name + ". Build a room for them first.");
            long fee = Alumni.Fee(c, state, lab, gone);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Alumni.Rehire(c, state, lab, gone, fee);
            return CommandResult.Done(-1);
        }
    }
}
