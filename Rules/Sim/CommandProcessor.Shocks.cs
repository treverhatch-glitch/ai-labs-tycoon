using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnswerShock(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab answers the news.");
            if (!Shocks.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody answers the news in this game.");
            if (!Shocks.Pending(lab)) return Fail(CommandError.InvalidArgument, "Nothing is waiting for an answer.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The answer is 1 brace, 2 ride it out or 3 counter.");
            if (cmd.Count == 1 && lab.Cash < Shocks.BraceCost(c, lab)) return NeedCash(lab, Shocks.BraceCost(c, lab));
            Shocks.Resolve(c, state, lab, cmd.Count, 0);
            return CommandResult.Done(-1);
        }
    }
}
