using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult DesignWithModel(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState run = lab.Model(cmd.TargetId);
            if (run == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            ModelState helper = lab.Model(cmd.Count);
            if (helper == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.Count + ".");
            if (run.Status != ModelStatus.Training)
                return Fail(CommandError.WrongModelStatus, run.Name + " has finished training; it is too late to ask for help designing it.");
            if (helper.Status == ModelStatus.Training)
                return Fail(CommandError.WrongModelStatus, helper.Name + " is still training; it cannot help yet.");
            if (SuccessorDesign.IsAssisted(run))
                return Fail(CommandError.InvalidArgument, run.Name + " is already being designed with help.");
            long bar = SuccessorDesign.HelperBar(c, lab, run);
            if (helper.Capability < bar)
                return Fail(CommandError.BelowSectorBar, helper.Name + " has capability " + helper.Capability + " and designing " + run.Name + " needs at least " + bar + ".");
            SuccessorDesign.Start(c, lab, run, helper);
            Events.Add(state, EventKind.SuccessorDesigned, lab.Id, run.Id, helper.Id, run.Name);
            return CommandResult.Done(run.Id);
        }
    }
}
