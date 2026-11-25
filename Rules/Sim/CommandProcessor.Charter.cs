using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ConvertCharter(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can change its charter.");
            if (!Charter.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody changes charter in this game.");
            if (cmd.Count < Charter.ForProfit || cmd.Count > Charter.Nonprofit) return Fail(CommandError.InvalidArgument, "Choose 0 for-profit, 1 capped-profit or 2 nonprofit.");
            if (Charter.UnderReview(lab)) return Fail(CommandError.InvalidArgument, "A change of charter is already under review.");
            if (cmd.Count == lab.Charter) return Fail(CommandError.InvalidArgument, "The lab already has that charter.");
            if (lab.Cash < c.Charter.ConvertFee) return NeedCash(lab, c.Charter.ConvertFee);
            Charter.Convert(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult CancelCharterChange(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can change its charter.");
            if (!Charter.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody changes charter in this game.");
            if (!Charter.UnderReview(lab)) return Fail(CommandError.InvalidArgument, "No change of charter is under review.");
            Charter.Cancel(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
