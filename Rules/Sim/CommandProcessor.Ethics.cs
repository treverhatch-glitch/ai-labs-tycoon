using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetEthicsBoard(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab seats an ethics board.");
            if (!EthicsBoard.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no ethics board in this game.");
            if (cmd.Count < 0 || cmd.Count > c.Board.MaxLevel) return Fail(CommandError.InvalidArgument, "The ethics board is from level 0 to " + c.Board.MaxLevel + ".");
            int level = EthicsBoard.Level(c, lab);
            if (cmd.Count == level) return Fail(CommandError.InvalidArgument, "The board already sits at that level.");
            if (cmd.Count > level)
            {
                if (EthicsBoard.Locked(state, lab))
                    return Fail(CommandError.EthicsLocked, "The last board walked out and no one will join another before week " + lab.EthicsLockedUntil + ".");
                if (cmd.Count > EthicsBoard.StaffCovers(c, lab))
                    return Fail(CommandError.NotEnoughStaff, "Level " + cmd.Count + " needs " + (cmd.Count * c.Board.SafetyStaffPerLevel) + " safety people on the payroll.");
            }
            long cost = EthicsBoard.SetupCost(c, lab, cmd.Count);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            EthicsBoard.Set(c, state, lab, cmd.Count, cost);
            return CommandResult.Done(-1);
        }
    }
}
