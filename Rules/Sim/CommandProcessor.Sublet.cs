using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetSublet(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab sublets compute.");
            if (cmd.Permille < 0 || cmd.Permille > c.Sublet.MaxPermille) return Fail(CommandError.InvalidArgument, "The sublet share is from 0 to " + c.Sublet.MaxPermille + ".");
            if (cmd.Permille == lab.SubletPermille) return Fail(CommandError.InvalidArgument, "The lab already sublets that share.");
            if (state.Tick < lab.SubletLockedUntil)
                return Fail(CommandError.SubletLocked, "The leases were changed recently and cannot be changed before week " + lab.SubletLockedUntil + ".");
            Sublet.Set(c, state, lab, cmd.Permille);
            return CommandResult.Done(-1);
        }
    }
}
