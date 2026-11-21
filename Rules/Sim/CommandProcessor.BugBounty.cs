using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetBugBounty(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs a bug bounty.");
            if (!BugBounty.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no bug bounty in this game.");
            if (cmd.Count < 0 || cmd.Count > c.BugBounty.MaxLevel) return Fail(CommandError.InvalidArgument, "The bounty goes from level 0 to " + c.BugBounty.MaxLevel + ".");
            if (cmd.Count == BugBounty.Level(c, lab)) return Fail(CommandError.InvalidArgument, "The bounty is already at level " + cmd.Count + ".");
            if (BugBounty.Locked(state, lab)) return Fail(CommandError.BountyLocked, "The bounty was changed recently and holds until week " + lab.BountyLockedUntil + ".");
            long fee = BugBounty.Fee(c, lab, cmd.Count);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            BugBounty.Set(c, state, lab, cmd.Count, fee);
            return CommandResult.Done(-1);
        }
    }
}
