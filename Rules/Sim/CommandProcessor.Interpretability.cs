using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetInterpretability(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs an interpretability programme.");
            if (cmd.Count < 0 || cmd.Count > c.Interp.MaxLevel) return Fail(CommandError.InvalidArgument, "Interpretability is from level 0 to " + c.Interp.MaxLevel + ".");
            if (cmd.Count == lab.InterpLevel) return Fail(CommandError.InvalidArgument, "The programme already runs at that level.");
            if (cmd.Count > 0)
            {
                string tech = Interpretability.TechFor(c, cmd.Count);
                if (!LabQueries.HasTech(lab, tech))
                    return Fail(CommandError.MissingPrerequisite, "Level " + cmd.Count + " needs " + (c.Tech(tech) == null ? tech : c.Tech(tech).Name) + ".");
            }
            lab.InterpLevel = cmd.Count;
            return CommandResult.Done(-1);
        }
    }
}
