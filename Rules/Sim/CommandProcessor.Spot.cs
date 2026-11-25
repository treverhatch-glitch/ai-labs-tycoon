using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetSpotShare(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs spot capacity.");
            if (!Spot.Enabled(c)) return Fail(CommandError.InvalidArgument, "The cloud sells no spot capacity in this game.");
            if (cmd.Permille < 0 || cmd.Permille > c.Spot.MaxPermille) return Fail(CommandError.InvalidArgument, "The spot share is from 0 to " + c.Spot.MaxPermille + ".");
            if (cmd.Permille == lab.SpotPermille) return CommandResult.Done(-1);
            if (lab.SpotSince > 0 && state.Tick + 1 < lab.SpotSince + c.Spot.ChangeWeeks) return Fail(CommandError.InvalidArgument, "The share was changed too recently.");
            lab.SpotPermille = cmd.Permille;
            lab.SpotSince = state.Tick + 1;
            return CommandResult.Done(-1);
        }
    }
}
