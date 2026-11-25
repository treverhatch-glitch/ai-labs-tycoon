using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult LaunchWatchGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a launch watch.");
            if (!LaunchWatch.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody watches launches in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult OpenWatchDesk(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LaunchWatchGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (LaunchWatch.On(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a watch desk.");
            if (lab.Cash < c.LaunchWatch.OpenCost) return NeedCash(lab, c.LaunchWatch.OpenCost);
            LaunchWatch.Open(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult CloseWatchDesk(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LaunchWatchGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!LaunchWatch.On(lab)) return Fail(CommandError.InvalidArgument, "The lab has no watch desk.");
            LaunchWatch.Close(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult AnswerLaunch(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LaunchWatchGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!LaunchWatch.Waiting(lab)) return Fail(CommandError.InvalidArgument, "No rival launch is waiting for an answer.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The answer is 1 counter-programme, 2 hold or 3 own the safety story.");
            if (cmd.Count == 1 && lab.Cash < LaunchWatch.CounterCost(c, lab)) return NeedCash(lab, LaunchWatch.CounterCost(c, lab));
            if (cmd.Count == 3 && !LaunchWatch.CanOwnSafety(c, lab))
                return Fail(CommandError.MissingPrerequisite, "The lab's best released model is not safe enough to talk about.");
            LaunchWatch.Resolve(c, state, lab, cmd.Count, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
