using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PledgeGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab makes support pledges.");
            if (!Pledges.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody holds labs to support pledges in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult PledgeSupport(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PledgeGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released || model.Sectors.Count == 0) return Fail(CommandError.WrongModelStatus, model.Name + " is not on sale in any sector.");
            if (Pledges.Held(model)) return Fail(CommandError.InvalidArgument, model.Name + " is already under a pledge.");
            if (Sunset.Pending(model)) return Fail(CommandError.InvalidArgument, model.Name + " has an end-of-life notice.");
            if (cmd.Count < c.Pledges.MinWeeks || cmd.Count > c.Pledges.MaxWeeks) return Fail(CommandError.InvalidArgument, "A pledge runs from " + c.Pledges.MinWeeks + " to " + c.Pledges.MaxWeeks + " weeks.");
            if (Pledges.Count(lab) >= c.Pledges.MaxPledges) return Fail(CommandError.InvalidArgument, "The lab can hold " + c.Pledges.MaxPledges + " support pledges at a time.");
            Pledges.Pledge(c, state, lab, model, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult BreakSupportPledge(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PledgeGate(c, state, lab);
            if (!gate.Ok) return gate;
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (!Pledges.Held(model)) return Fail(CommandError.InvalidArgument, model.Name + " is not under a pledge.");
            long fee = Pledges.BreachFee(c, state, lab, model);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Pledges.Break(c, state, lab, model);
            return CommandResult.Done(-1);
        }
    }
}
