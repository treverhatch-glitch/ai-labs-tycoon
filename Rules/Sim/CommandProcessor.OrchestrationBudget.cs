using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetOrchBudget(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab sets the orchestration budget.");
            if (!OrchestrationBudget.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no orchestration budget in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.OrchestrationBudget.MinEra) return Fail(CommandError.EraLocked, "The orchestration budget is not used in this era.");
            if (cmd.Count < 0 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The budget goes from level 0 to 3.");
            if (cmd.Count == OrchestrationBudget.Level(c, lab)) return Fail(CommandError.InvalidArgument, "The budget is already at level " + cmd.Count + ".");
            int added = (int)MathI.Max(0, cmd.Count - OrchestrationBudget.Level(c, lab));
            long cost = added * (long)c.OrchestrationBudget.SetupPerLevel;
            if (cost > 0 && lab.Cash < cost) return NeedCash(lab, cost);
            OrchestrationBudget.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}