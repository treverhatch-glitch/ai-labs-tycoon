using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult FineTuneModel(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (ModelRepair.GlitchWeeksLeft(c, model, state.Tick) <= 0)
                return Fail(CommandError.InvalidArgument, model.Name + " is not glitching, so there is nothing to fine-tune.");
            long cost = ModelRepair.FineTuneCost(c, state, lab, model);
            if (lab.Cash < cost) return NeedCash(lab, cost);

            Finance.Spend(lab, LedgerLine.Training, cost);
            ModelRepair.FineTune(c, state, lab, model);
            TechDebt.Add(c, state, lab, c.TechDebt.FineTunePatch);
            Events.Add(state, EventKind.ModelFineTuned, lab.Id, model.Id, cost, model.Name);
            return CommandResult.Done(-1);
        }

        static CommandResult RollBackModel(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (ModelRepair.GlitchWeeksLeft(c, model, state.Tick) <= 0)
                return Fail(CommandError.InvalidArgument, model.Name + " is not glitching, so there is nothing to roll back.");
            ModelState older = ModelRepair.RollbackTarget(lab, model);
            if (older == null) return Fail(CommandError.InvalidArgument, "There is no earlier version of " + model.Name + " to go back to.");
            if (SafetyCulture.GateBlocks(lab, older))
                return Fail(CommandError.BelowReleaseGate, older.Name + " shows alignment " + SafetySystem.DisplayedAlignment(older) + " and your own release gate asks for " + lab.ReleaseGate + ".");
            if (!EraRules.MeetsReleaseBar(c, older, state.Tick))
                return Fail(CommandError.BelowEraSafetyBar, older.Name + " no longer meets the law's safety bar of " + EraRules.ReleaseBar(c, state.Tick) + ".");

            ModelRepair.RollBack(c, state, lab, model, older);
            Events.Add(state, EventKind.ModelRolledBack, lab.Id, older.Id, c.Repair.RollbackReputationLoss, model.Name);
            return CommandResult.Done(-1);
        }
    }
}
