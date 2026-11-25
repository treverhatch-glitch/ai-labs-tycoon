using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetOnCall(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs an on-call rota.");
            if (!OnCall.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no on-call rota in this game.");
            if (cmd.Count < 0 || cmd.Count > c.OnCall.MaxLevel) return Fail(CommandError.InvalidArgument, "The rota is from level 0 to " + c.OnCall.MaxLevel + ".");
            int level = OnCall.Level(c, lab);
            if (cmd.Count == level) return Fail(CommandError.InvalidArgument, "The rota is already at that level.");
            if (OnCall.Locked(state, lab)) return Fail(CommandError.OnCallLocked, "The rota was changed recently and stays as it is until week " + lab.OnCallLockedUntil + ".");
            if (cmd.Count > level && cmd.Count > OnCall.StaffCover(c, lab))
                return Fail(CommandError.NotEnoughStaff, "Level " + cmd.Count + " needs " + (cmd.Count * c.OnCall.StaffPerLevel) + " safety people and engineers on the payroll.");
            OnCall.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
