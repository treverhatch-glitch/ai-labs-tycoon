using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PublishPostMortem(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab publishes post-mortems.");
            if (!PostMortem.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no post-mortems in this game.");
            if (cmd.Count != PostMortem.Summary && cmd.Count != PostMortem.Full) return Fail(CommandError.InvalidArgument, "A post-mortem is a summary (1) or a full write-up (2).");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Incidents == 0) return Fail(CommandError.InvalidArgument, model.Name + " has not had an incident.");
            if (PostMortem.Published(model)) return Fail(CommandError.InvalidArgument, "The post-mortem for " + model.Name + "'s last incident is already out.");
            if (!PostMortem.Open(c, state, model)) return Fail(CommandError.InvalidArgument, "The last incident of " + model.Name + " is more than " + c.PostMortem.WindowWeeks + " weeks old. Nobody is asking any more.");
            if (PostMortem.SafetyStaff(c, lab) < c.PostMortem.MinSafetyStaff) return Fail(CommandError.NotEnoughStaff, "A post-mortem needs at least " + c.PostMortem.MinSafetyStaff + " safety people to write it.");
            long cost = PostMortem.Cost(c, cmd.Count);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            PostMortem.Publish(c, state, lab, model, cmd.Count, cost);
            return CommandResult.Done(-1);
        }
    }
}
