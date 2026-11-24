using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult Layoff(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs a layoff round.");
            if (!Layoffs.Enabled(c)) return Fail(CommandError.InvalidArgument, "Layoff rounds are not available in this game.");
            if (cmd.Permille < 1 || cmd.Permille > c.Layoffs.MaxPermille)
                return Fail(CommandError.InvalidArgument, "A round lets go of 1 to " + c.Layoffs.MaxPermille + " permille of each function.");
            if (Layoffs.TotalCut(c, lab, cmd.Permille) < 1)
                return Fail(CommandError.NotEnoughStaff, "That share rounds down to nobody in any function.");
            Layoffs.Run(c, state, lab, cmd.Permille);
            return CommandResult.Done(-1);
        }
    }
}
