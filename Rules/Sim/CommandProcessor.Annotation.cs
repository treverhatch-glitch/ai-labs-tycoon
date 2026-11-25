using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnnotationGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab contracts annotators.");
            if (!Annotation.Enabled(c) || !Datasets.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody sells annotation in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult ContractAnnotators(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = AnnotationGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The vendors are volume (1), licensed (2) and expert (3).");
            if (lab.AnnVendor != 0) return Fail(CommandError.InvalidArgument, "A vendor is already under contract.");
            long first = Annotation.Weekly(c, cmd.Count);
            if (lab.Cash < first) return NeedCash(lab, first);
            Annotation.Contract(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult EndAnnotators(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = AnnotationGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.AnnVendor == 0) return Fail(CommandError.InvalidArgument, "No vendor is under contract.");
            long fee = Annotation.EarlyFee(c, lab);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Annotation.EndEarly(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult AuditVendor(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = AnnotationGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.AnnVendor == 0) return Fail(CommandError.InvalidArgument, "No vendor is under contract.");
            if (lab.AnnAuditWeek > 0 && state.Tick + 1 < lab.AnnAuditWeek + c.Annotation.AuditWeeks) return Fail(CommandError.InvalidArgument, "The vendor was audited too recently.");
            if (lab.Cash < c.Annotation.AuditCost) return NeedCash(lab, c.Annotation.AuditCost);
            Annotation.Audit(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
