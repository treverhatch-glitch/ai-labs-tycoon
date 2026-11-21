using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RedTeamModel(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab pays for a red team.");
            if (!RedTeam.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no red teams in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released)
                return Fail(CommandError.InvalidArgument, model.Name + " is not a finished model on the lab's books, so there is nothing to attack.");
            if (RedTeam.Active(state, model)) return Fail(CommandError.InvalidArgument, model.Name + " was red-teamed recently and its fixes last until week " + model.RedTeamUntil + ".");
            if (RedTeam.Attackers(c, lab) < c.RedTeam.MinStaff)
                return Fail(CommandError.NotEnoughStaff, "A red team needs at least " + c.RedTeam.MinStaff + " red-teamer" + (c.RedTeam.MinStaff == 1 ? "" : "s") + ".");
            long cost = RedTeam.Cost(c, state, lab, model);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            RedTeam.Run(c, state, lab, model, cost);
            return CommandResult.Done(-1);
        }
    }
}
