using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SatisfyMarginCall(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player answers a margin call.");
            if (!MarginCall.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no margin calls in this game.");
            if (lab.MarginCallLeft < 1) return Fail(CommandError.InvalidArgument, "No margin call is outstanding.");
            if (cmd.Count > 0)
            {
                MarginCall.SatisfySell(c, state, lab, cmd.Count);
                if (lab.MarginCallLeft > 0) return Fail(CommandError.InvalidArgument, "Collateral is still short after the sale.");
                return CommandResult.Done(-1);
            }
            long listPay = MarginCall.CashDue(c, state, lab);
            long pay = MarginSatisfyEraRelief.CashDue(c, state, listPay);
            if (pay < 1) pay = cmd.Permille > 0 ? cmd.Permille : Loans.Outstanding(lab);
            if (lab.Cash < pay) return NeedCash(lab, pay);
            MarginCall.SatisfyPay(c, state, lab, pay);
            if (listPay > 0) MarginSatisfyEraRelief.OnSatisfy(c, state, lab, listPay, pay);
            if (lab.MarginCallLeft > 0 && MarginCall.InBreach(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Collateral is still short after the payment.");
            return CommandResult.Done(-1);
        }

        static CommandResult RespondRungSix(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player answers a rung-six crisis.");
            if (!OfficeEvents.IsPending(lab)) return Fail(CommandError.NoOfficeEvent, "Nothing is waiting for an answer.");
            OfficeEventConfig e = c.Office.Event(lab.OfficeEventId);
            if (e == null || (e.Trigger != OfficeTrigger.RungSix && e.Trigger != OfficeTrigger.ShutdownCompute))
                return Fail(CommandError.NoOfficeEvent, "There is no rung-six crisis waiting.");
            OfficeChoiceConfig choice = e.Choice(cmd.Id);
            if (choice == null) return Fail(CommandError.UnknownId, "\"" + e.Name + "\" has no answer called \"" + cmd.Id + "\".");
            OfficeEvents.Answer(c, state, lab, e, choice);
            RungSixCrisis.OnAnswered(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult SetLawTarget(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player sets a law target.");
            if (!LawTarget.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no law target in this game.");
            string goal = cmd.Id;
            if (!LawShaping.IsGoal(goal)) return Fail(CommandError.UnknownId, "Set law target to \"strict\" or \"loose\", not \"" + goal + "\".");
            LawTarget.Set(c, state, lab, goal);
            return CommandResult.Done(-1);
        }
    }
}