using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult HostScholars(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab hosts scholars.");
            if (!Scholars.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no visiting scholars in this game.");
            if (Scholars.Active(lab)) return Fail(CommandError.InvalidArgument, "A group of scholars is already in the lab.");
            if (cmd.Count < 1 || cmd.Count > c.Scholars.MaxScholars) return Fail(CommandError.InvalidArgument, "A group is from 1 to " + c.Scholars.MaxScholars + " scholars.");
            int free = LabQueries.FreeSeats(c, lab, Scholars.RoleId);
            if (free < cmd.Count) return Fail(CommandError.NoSeats, "There are " + free + " free researcher seats for the scholars.");
            long total = Scholars.TotalStipend(c, state, lab, cmd.Count);
            if (lab.Cash < total) return NeedCash(lab, total);
            Scholars.Start(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult SendScholarsHome(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab hosts scholars.");
            if (!Scholars.Active(lab)) return Fail(CommandError.InvalidArgument, "There are no scholars in the lab.");
            Scholars.Finish(c, state, lab, true);
            return CommandResult.Done(-1);
        }
    }
}
