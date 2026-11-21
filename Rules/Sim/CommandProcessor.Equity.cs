using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult GrantStaffEquity(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab grants staff equity.");
            if (!StaffEquity.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no staff equity in this game.");
            if (lab.EquityLevel >= c.Equity.MaxLevel) return Fail(CommandError.InvalidArgument, "The team already has all the equity there is to give (level " + c.Equity.MaxLevel + ").");
            if (lab.Staff.Count < c.Equity.MinStaff) return Fail(CommandError.NotEnoughStaff, "Equity needs " + c.Equity.MinStaff + " people on the payroll.");
            if (StaffEquity.FounderAfterNext(c, lab) <= c.Funding.ControlPermille)
                return Fail(CommandError.WouldLoseControl, "Another grant would take the founder's stake to " + StaffEquity.FounderAfterNext(c, lab) / 10 + "%, and the board would take control.");
            StaffEquity.Grant(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
