using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnswerWhistle(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has a whistleblower.");
            if (!Whistle.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody blows the whistle in this game.");
            if (!Whistle.Open(lab)) return Fail(CommandError.InvalidArgument, "Nobody has raised a concern.");
            if (cmd.Count != Whistle.Clean && cmd.Count != Whistle.Silence) return Fail(CommandError.InvalidArgument, "Answer 0 to come clean or 1 to pay for silence.");
            if (cmd.Count == Whistle.Silence)
            {
                long cost = Whistle.SilenceCost(c, lab);
                if (lab.Cash < cost) return NeedCash(lab, cost);
            }
            Whistle.Answer(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
