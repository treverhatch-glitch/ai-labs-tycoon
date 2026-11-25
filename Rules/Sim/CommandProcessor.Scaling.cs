using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AdoptPolicy(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a scaling policy.");
            if (!ScalingPolicy.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody keeps a scaling policy in this game.");
            if (cmd.Count < 0 || cmd.Count > c.Scaling.MaxLevel) return Fail(CommandError.InvalidArgument, "The policy is from level 0 to " + c.Scaling.MaxLevel + ".");
            if (cmd.Count == lab.PolicyLevel) return Fail(CommandError.InvalidArgument, "The policy is already at that level.");
            if (cmd.Count < lab.PolicyLevel && state.Tick < lab.PolicyLockedUntil)
                return Fail(CommandError.InvalidArgument, "The policy cannot be lowered until week " + lab.PolicyLockedUntil + ".");
            if (cmd.Count > lab.PolicyLevel)
            {
                long fee = ScalingPolicy.RaiseFee(c, lab, cmd.Count);
                if (lab.Cash < fee) return NeedCash(lab, fee);
            }
            ScalingPolicy.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult BreakPolicy(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a scaling policy.");
            if (!ScalingPolicy.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody keeps a scaling policy in this game.");
            if (ScalingPolicy.Tripped(c, lab) == 0) return Fail(CommandError.InvalidArgument, "No tripwire is holding the lab up.");
            ScalingPolicy.Break(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
