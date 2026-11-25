using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetPledge(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab makes a safety pledge.");
            if (!Pledge.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no safety pledges in this game.");
            if (cmd.Count < 0 || cmd.Count > c.Pledge.MaxLevel) return Fail(CommandError.InvalidArgument, "The pledge goes from level 0 to " + c.Pledge.MaxLevel + ".");
            if (cmd.Count == Pledge.Level(c, lab)) return Fail(CommandError.InvalidArgument, "The pledge is already at level " + cmd.Count + ".");
            if (Pledge.Locked(state, lab)) return Fail(CommandError.PledgeLocked, "The pledge changed recently and holds until week " + lab.PledgeLockedUntil + ".");
            Pledge.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
