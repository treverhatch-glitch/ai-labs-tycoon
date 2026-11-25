using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnnounceRoadmap(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab announces a roadmap.");
            if (!Roadmap.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no roadmaps in this game.");
            if (cmd.Count < 1 || cmd.Count > c.Roadmap.MaxLevel) return Fail(CommandError.InvalidArgument, "Pick a roadmap level from 1 to " + c.Roadmap.MaxLevel + ".");
            if (cmd.TargetId < c.Roadmap.MinWeeks || cmd.TargetId > c.Roadmap.MaxWeeks)
                return Fail(CommandError.InvalidArgument, "A roadmap runs from " + c.Roadmap.MinWeeks + " to " + c.Roadmap.MaxWeeks + " weeks.");
            if (Roadmap.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has already announced a roadmap and customers are waiting for it.");
            if (Roadmap.Best(lab) < 1) return Fail(CommandError.InvalidArgument, "A lab with no finished model has nothing to announce a successor to.");
            Roadmap.Announce(c, state, lab, cmd.Count, cmd.TargetId);
            return CommandResult.Done(-1);
        }
    }
}
