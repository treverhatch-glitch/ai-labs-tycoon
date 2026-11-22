using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PostTrain(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab tunes models.");
            if (!PostTraining.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody tunes models in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained) return Fail(CommandError.WrongModelStatus, model.Name + " is " + model.Status + "; only a trained model that has not gone on sale can be tuned.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.PostTraining.MinEra) return Fail(CommandError.EraLocked, "Models are not tuned in this era.");
            if (cmd.Count != PostTraining.Preference && cmd.Count != PostTraining.Safety) return Fail(CommandError.InvalidArgument, "Tuning is preference (1) or safety (2).");
            if (model.PostKind != 0) return Fail(CommandError.InvalidArgument, model.Name + " is already being tuned.");
            int done = cmd.Count == PostTraining.Preference ? model.PostPref : model.PostSafety;
            if (done >= c.PostTraining.MaxPasses) return Fail(CommandError.InvalidArgument, model.Name + " has had all the tuning of this kind it can take.");
            if (PostTraining.Available(c, lab, cmd.Count) < c.PostTraining.Units)
                return Fail(CommandError.MissingPrerequisite, "A tuning run needs " + c.PostTraining.Units + " clean " + (cmd.Count == PostTraining.Preference ? "user conversation" : "licensed or scraped") + " units and the lab has " + PostTraining.Available(c, lab, cmd.Count) + ".");
            if (lab.Cash < c.PostTraining.Cost) return NeedCash(lab, c.PostTraining.Cost);
            PostTraining.Start(c, state, lab, model, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult AbandonPostTraining(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.PostKind == 0) return Fail(CommandError.InvalidArgument, model.Name + " is not being tuned.");
            PostTraining.Abandon(c, state, lab, model);
            return CommandResult.Done(-1);
        }
    }
}
