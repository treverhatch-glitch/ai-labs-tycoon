using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult CleanUpNaming(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (VersionNaming.Mess(c, lab) <= 0) return Fail(CommandError.InvalidArgument, "The lab's model names are already tidy.");
            long cost = VersionNaming.CleanCost(c, state.Tick);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            if (cost > 0) Finance.Spend(lab, LedgerLine.Hiring, cost);
            VersionNaming.Clean(c, lab);
            return CommandResult.Done(-1);
        }
    }
}
