using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult WaitlistGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a waitlist.");
            if (!Waitlist.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody keeps waitlists in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult OpenWaitlist(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = WaitlistGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Waitlist.Open(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a waitlist open.");
            ModelState m = null;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Id == cmd.TargetId) m = lab.Models[i];
            if (m == null) return Fail(CommandError.UnknownId, "The lab has no such model.");
            if (m.Status != ModelStatus.Training && m.Status != ModelStatus.Trained)
                return Fail(CommandError.InvalidArgument, m.Name + " is " + (m.Status == ModelStatus.Released ? "already out" : "retired") + ".");
            if (cmd.Count < c.Waitlist.MinWeeks || cmd.Count > c.Waitlist.MaxWeeks)
                return Fail(CommandError.InvalidArgument, "A waitlist promises between " + c.Waitlist.MinWeeks + " and " + c.Waitlist.MaxWeeks + " weeks.");
            Waitlist.Begin(c, state, lab, m, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult CloseWaitlist(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = WaitlistGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Waitlist.Open(lab)) return Fail(CommandError.InvalidArgument, "The lab has no waitlist open.");
            Waitlist.Close(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
