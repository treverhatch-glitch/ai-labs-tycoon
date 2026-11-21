using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PayDownDebt(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps technical debt.");
            if (!TechDebt.Enabled(c)) return Fail(CommandError.InvalidArgument, "Technical debt is not kept in this game.");
            if (cmd.Count < 1 || cmd.Count > c.TechDebt.MaxPerCommand)
                return Fail(CommandError.InvalidArgument, "Pay down 1 to " + c.TechDebt.MaxPerCommand + " points at a time.");
            if (lab.TechDebt < 1) return Fail(CommandError.InvalidArgument, "The lab has no technical debt.");
            if (cmd.Count > lab.TechDebt) return Fail(CommandError.InvalidArgument, "The lab only carries " + lab.TechDebt + " points.");
            if (LabQueries.StaffCount(lab, "engineer") < c.TechDebt.MinEngineers)
                return Fail(CommandError.NotEnoughStaff, "Paying down debt takes at least " + c.TechDebt.MinEngineers + " engineers.");
            long cost = TechDebt.Cost(c, cmd.Count);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            TechDebt.PayDown(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
