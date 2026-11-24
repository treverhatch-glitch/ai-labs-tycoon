using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetRevenueTarget(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab sets a revenue target.");
            if (!RevenueTargets.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no revenue targets in this game.");
            if (cmd.Count < 1 || cmd.Count > c.RevenueTargets.MaxLevel)
                return Fail(CommandError.InvalidArgument, "Pick a target level from 1 to " + c.RevenueTargets.MaxLevel + ".");
            if (RevenueTargets.Active(lab)) return Fail(CommandError.InvalidArgument, "The board is already holding the lab to a target.");
            if (lab.LastWeek.Revenue < c.RevenueTargets.MinWeeklyRevenue)
                return Fail(CommandError.InvalidArgument, "The lab makes too little to set a target ($" + lab.LastWeek.Revenue + " last week, needs $" + c.RevenueTargets.MinWeeklyRevenue + ").");
            RevenueTargets.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
