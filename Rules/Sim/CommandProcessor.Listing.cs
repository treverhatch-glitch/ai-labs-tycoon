using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ListingGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can list.");
            if (!Listing.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody lists in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult ListCompany(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ListingGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Listing.Listed(lab)) return Fail(CommandError.InvalidArgument, "The lab is already listed.");
            if (lab.Charter == Charter.Nonprofit) return Fail(CommandError.InvalidArgument, "A nonprofit cannot list.");
            if (lab.Reputation < c.Listing.MinReputation) return Fail(CommandError.MissingPrerequisite, "The market wants more reputation than that.");
            if (Listing.Revenue(lab) < c.Listing.MinWeeklyRevenue) return Fail(CommandError.MissingPrerequisite, "The market wants " + c.Listing.MinWeeklyRevenue + " a week in revenue.");
            Listing.List(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult ListedGate(GameConfig c, GameState state, LabState lab)
        {
            CommandResult gate = ListingGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Listing.Listed(lab)) return Fail(CommandError.InvalidArgument, "The lab is not listed.");
            return CommandResult.Done(-1);
        }

        static CommandResult SetGuidance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ListedGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "Guidance is level 1, 2 or 3.");
            if (!Listing.WindowOpen(c, state, lab)) return Fail(CommandError.InvalidArgument, "Guidance is set in the first weeks of a quarter, once.");
            Listing.SetGuidance(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult FollowOn(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ListedGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (state.Tick < lab.ListFollowOn + c.Listing.FollowOnWeeks) return Fail(CommandError.InvalidArgument, "The market has not forgotten the last offering.");
            Listing.FollowOn(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult BuyBackShares(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ListedGate(c, state, lab);
            if (!gate.Ok) return gate;
            long amount = cmd.Count * 1000L;
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Spend something on the buyback.");
            if (lab.ListKeep >= MathI.Permille) return Fail(CommandError.InvalidArgument, "There are no shares of the public's to buy.");
            if (lab.Cash < amount) return NeedCash(lab, amount);
            Listing.BuyBack(c, state, lab, amount);
            return CommandResult.Done(-1);
        }

        static CommandResult TakePrivate(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ListedGate(c, state, lab);
            if (!gate.Ok) return gate;
            long cost = Listing.TakePrivateCost(c, lab);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Listing.TakePrivate(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
