using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetAsideReserve(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a reserve.");
            if (!Treasury.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no treasury reserve in this game.");
            if (cmd.Count < c.Treasury.MinMove) return Fail(CommandError.InvalidArgument, "The least that can be set aside is $" + c.Treasury.MinMove + ".");
            if (lab.Cash < cmd.Count) return NeedCash(lab, cmd.Count);
            Treasury.SetAside(state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult DrawReserve(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a reserve.");
            if (!Treasury.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no treasury reserve in this game.");
            if (cmd.Count < c.Treasury.MinMove) return Fail(CommandError.InvalidArgument, "The least that can be taken back is $" + c.Treasury.MinMove + ".");
            if (cmd.Count > lab.Reserve) return Fail(CommandError.InvalidArgument, "The reserve holds $" + lab.Reserve + ".");
            Treasury.Draw(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
