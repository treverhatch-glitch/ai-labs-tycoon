using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetPayLevel(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab sets its own pay.");
            if (!Pay.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no pay policy in this game.");
            StaffPeopleConfig s = c.StaffPeople;
            if (cmd.Count < s.PayMinLevel || cmd.Count > s.PayMaxLevel) return Fail(CommandError.InvalidArgument, "Pay is from level " + s.PayMinLevel + " to " + s.PayMaxLevel + ".");
            if (cmd.Count == lab.PayLevel) return Fail(CommandError.InvalidArgument, "The lab already pays at that level.");
            if (state.Tick < lab.PayLockedUntil) return Fail(CommandError.PayLocked, "Pay was changed recently and cannot be changed before week " + lab.PayLockedUntil + ".");
            Pay.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
