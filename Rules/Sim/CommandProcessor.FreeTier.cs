using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult FreeTierGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs a free tier.");
            if (!FreeTier.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody offers free tiers in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult OpenFreeTier(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FreeTierGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (FreeTier.Open(lab)) return Fail(CommandError.InvalidArgument, "The lab already runs a free tier.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The level is 1, 2 or 3.");
            if (LabQueries.BestReleased(lab) == null) return Fail(CommandError.MissingPrerequisite, "A free tier needs a released model.");
            FreeTier.Begin(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult CloseFreeTier(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = FreeTierGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!FreeTier.Open(lab)) return Fail(CommandError.InvalidArgument, "The lab runs no free tier.");
            FreeTier.Close(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
