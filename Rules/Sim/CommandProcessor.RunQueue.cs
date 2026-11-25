using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetRunPriority(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab queues its runs.");
            if (!RunQueue.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody queues training runs in this game.");
            ModelState m = null;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Id == cmd.TargetId) m = lab.Models[i];
            if (m == null) return Fail(CommandError.UnknownId, "The lab has no such model.");
            if (m.Status != ModelStatus.Training) return Fail(CommandError.InvalidArgument, m.Name + " is not in training.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The level is 1 background, 2 normal or 3 rush.");
            if (RunQueue.Level(m) == cmd.Count) return Fail(CommandError.InvalidArgument, m.Name + " is already at that level.");
            if (cmd.Count == 3 && RunQueue.Rushed(lab) >= c.RunQueue.MaxRush) return Fail(CommandError.InvalidArgument, "Only " + c.RunQueue.MaxRush + " run(s) can be rushed at a time.");
            RunQueue.Set(c, state, lab, m, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
