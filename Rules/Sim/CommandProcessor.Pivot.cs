using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PivotStyle(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab pivots.");
            StyleConfig style = c.Style(cmd.Id);
            if (style == null) return Fail(CommandError.UnknownId, "There is no style \"" + cmd.Id + "\".");
            if (lab.StyleId == style.Id) return Fail(CommandError.InvalidArgument, "The lab is already " + style.Name + ".");
            if (Pivot.Spent(c, state, lab)) return Fail(CommandError.PivotSpent, "The lab has already pivoted this era. It can again when the next era begins.");
            long cost = Pivot.Cost(c, state, lab);
            if (lab.Cash < cost) return NeedCash(lab, cost);

            Finance.Spend(lab, LedgerLine.Hiring, cost);
            string from = lab.StyleId;
            int left = Pivot.Leave(c, lab);
            lab.StyleId = style.Id;
            lab.PivotEraId = Pivot.EraId(c, state);
            lab.PivotUntilTick = state.Tick + c.Pivot.ConfusionWeeks;
            Events.Add(state, EventKind.StylePivoted, lab.Id, left, cost, from + "|" + style.Id);
            return CommandResult.Done(-1);
        }
    }
}
