using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ConformityGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab goes through conformity assessment.");
            if (!Conformity.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody assesses conformity in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult StartConformity(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ConformityGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released || model.Sectors.Count == 0) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale in any sector.");
            if (model.CertStage != 0) return Fail(CommandError.InvalidArgument, model.Name + " is already in an assessment or certified.");
            if (cmd.Count < 1 || cmd.Count > 2) return Fail(CommandError.InvalidArgument, "The scope is 1 narrow or 2 broad.");
            if (Conformity.Active(lab) >= c.Conformity.MaxActive) return Fail(CommandError.InvalidArgument, "The lab can have " + c.Conformity.MaxActive + " models in the pipeline at a time.");
            long cost = Conformity.DossierCost(c, cmd.Count);
            if (lab.ResearchPoints < cost) return Fail(CommandError.NotEnoughResearch, "The dossier costs " + cost + " research points and the lab has " + lab.ResearchPoints + ".");
            Conformity.Start(c, state, lab, model, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult SubmitEvidence(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ConformityGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.CertStage != Conformity.Evidence) return Fail(CommandError.InvalidArgument, model.Name + " is not waiting for evidence.");
            if (model.CertLeft > 0) return Fail(CommandError.InvalidArgument, model.Name + " is still being fixed after findings, for " + model.CertLeft + " more weeks.");
            if (!Conformity.HasEvidence(state, model)) return Fail(CommandError.MissingPrerequisite, model.CertScope == 2 ? "A broad scope needs a live red-team result and a live external audit." : "The assessor needs a live red-team result.");
            Conformity.Submit(c, model);
            return CommandResult.Done(-1);
        }

        static CommandResult WithdrawConformity(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ConformityGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.CertStage == 0) return Fail(CommandError.InvalidArgument, model.Name + " is not in an assessment.");
            Conformity.End(c, state, lab, model, 0);
            return CommandResult.Done(-1);
        }
    }
}
