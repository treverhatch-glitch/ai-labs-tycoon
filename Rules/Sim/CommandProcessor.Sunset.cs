using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult AnnounceSunset(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab gives end-of-life notice.");
            if (!Sunset.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no end-of-life notice in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " is " + model.Status + "; only a model on sale can be retired.");
            if (Sunset.Pending(model)) return Fail(CommandError.InvalidArgument, model.Name + " already has a notice, leaving in week " + model.SunsetTick + ".");
            if (Pledges.Held(model)) return Fail(CommandError.InvalidArgument, model.Name + " is under a support pledge for " + model.LtsLeft + " more weeks.");
            if (cmd.Count < c.Sunset.MinWeeks || cmd.Count > c.Sunset.MaxWeeks)
                return Fail(CommandError.InvalidArgument, "A notice runs from " + c.Sunset.MinWeeks + " to " + c.Sunset.MaxWeeks + " weeks.");
            if (Sunset.SuccessorOf(lab, model) == null)
                return Fail(CommandError.InvalidArgument, model.Name + " has no successor on sale: a released model, at least as capable, that it helped design.");
            Sunset.Announce(c, state, lab, model, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult CancelSunset(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab gives end-of-life notice.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (!Sunset.Pending(model)) return Fail(CommandError.InvalidArgument, model.Name + " has no notice to call off.");
            Sunset.CallOff(c, state, lab, model, true);
            return CommandResult.Done(-1);
        }
    }
}
