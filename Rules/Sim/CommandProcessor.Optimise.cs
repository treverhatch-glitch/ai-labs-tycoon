using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetOptimisation(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab optimises its inference.");
            if (!Optimisation.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no inference optimisation in this game.");
            if (cmd.Count < 0 || cmd.Count > c.Optimise.MaxLevel) return Fail(CommandError.InvalidArgument, "Optimisation is from level 0 to " + c.Optimise.MaxLevel + ".");
            int level = Optimisation.Level(c, lab);
            if (cmd.Count == level) return Fail(CommandError.InvalidArgument, "The work is already at that level.");
            if (cmd.Count > level && cmd.Count > Optimisation.EngineersCover(c, lab))
                return Fail(CommandError.NotEnoughStaff, "Level " + cmd.Count + " needs " + (cmd.Count * c.Optimise.EngineersPerLevel) + " engineers on the payroll.");
            long cost = Optimisation.SetupCost(c, lab, cmd.Count);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Optimisation.Set(c, state, lab, cmd.Count, cost);
            return CommandResult.Done(-1);
        }
    }
}
