using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RegisterEval(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab registers public evals.");
            if (!EvalHarness.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no public eval harness in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.EvalHarness.MinEra) return Fail(CommandError.EraLocked, "Public evals are not used in this era.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is not trained yet.");
            if (cmd.Count < 1 || cmd.Count > 2) return Fail(CommandError.InvalidArgument, "Choose suite 1 (narrow) or 2 (broad).");
            if (model.EvalSuite != 0) return Fail(CommandError.InvalidArgument, model.Name + " is already on a public eval cadence.");
            if (lab.ResearchPoints < c.EvalHarness.Points) return Fail(CommandError.MissingPrerequisite, "Publishing needs " + c.EvalHarness.Points + " research points.");
            if (lab.Cash < c.EvalHarness.Cost) return NeedCash(lab, c.EvalHarness.Cost);
            EvalHarness.Register(c, state, lab, model, cmd.Count);
            return CommandResult.Done(model.Id);
        }
    }
}
