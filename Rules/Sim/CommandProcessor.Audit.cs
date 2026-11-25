using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult CommissionAudit(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab pays for an external audit.");
            if (!SafetyAudit.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no external audits in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released)
                return Fail(CommandError.InvalidArgument, model.Name + " is not a finished model on the lab's books, so there is nothing to audit.");
            if (SafetyAudit.Certified(state, model)) return Fail(CommandError.InvalidArgument, model.Name + " already holds a certificate until week " + model.AuditedUntil + ".");
            if (SafetyAudit.SafetyStaff(c, lab) < c.Audit.MinSafetyStaff)
                return Fail(CommandError.NotEnoughStaff, "Auditors need at least " + c.Audit.MinSafetyStaff + " safety staff to work with.");
            long cost = SafetyAudit.Cost(c, state, lab, model);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            SafetyAudit.Run(c, state, lab, model, cost);
            return CommandResult.Done(-1);
        }
    }
}
