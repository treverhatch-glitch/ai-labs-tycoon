using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetGauntletTuning(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (c.Gauntlet.MaxTuningPermille < 1) return Fail(CommandError.InvalidArgument, "Nobody can tune for " + c.Gauntlet.Name + " in this game.");
            if (cmd.Permille < 0 || cmd.Permille > c.Gauntlet.MaxTuningPermille)
                return Fail(CommandError.InvalidArgument, "Tuning goes from 0 to " + c.Gauntlet.MaxTuningPermille + ".");
            lab.GauntletTuning = cmd.Permille;
            return CommandResult.Done(-1);
        }
    }
}
