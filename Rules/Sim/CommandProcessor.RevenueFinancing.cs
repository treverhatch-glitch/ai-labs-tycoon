using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult TakeRevenueAdvance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can take a revenue advance.");
            if (!RevenueFinancing.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody lends against revenue in this game.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The advance is tier 1, 2 or 3.");
            if (cmd.Permille < 1 || cmd.Permille > MathI.Permille) return Fail(CommandError.InvalidArgument, "Ask for 1 to 1000 permille of the largest advance.");
            if (RevenueFinancing.Owing(lab)) return Fail(CommandError.InvalidArgument, "The lab already owes on an advance.");
            if (RevenueFinancing.Revenue(lab) < c.Rbf.MinWeeklyRevenue) return Fail(CommandError.MissingPrerequisite, "The lender wants " + c.Rbf.MinWeeklyRevenue + " a week in revenue.");
            long advance = MathI.MulPermille(RevenueFinancing.Largest(c, lab), cmd.Permille);
            if (advance < 1) return Fail(CommandError.InvalidArgument, "That is too little to lend.");
            RevenueFinancing.Take(c, state, lab, cmd.Count, advance);
            return CommandResult.Done(-1);
        }

        static CommandResult BuyOutAdvance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId || !RevenueFinancing.Enabled(c) || !RevenueFinancing.Owing(lab)) return Fail(CommandError.InvalidArgument, "There is no advance to buy out.");
            if (lab.RbfWeeks < c.Rbf.BuyoutAfter) return Fail(CommandError.InvalidArgument, "The lender will not take a buyout this early.");
            long cost = RevenueFinancing.Buyout(c, lab);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            RevenueFinancing.BuyOut(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
