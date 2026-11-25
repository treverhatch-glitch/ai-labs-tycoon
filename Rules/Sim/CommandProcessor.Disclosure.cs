using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult DisclosureGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps an incident register.");
            if (!Disclosure.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody keeps an incident register in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult JoinRegister(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DisclosureGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.DiscOn > 0) return Fail(CommandError.InvalidArgument, "The lab is already on the register.");
            Disclosure.Join(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult LeaveRegister(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DisclosureGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.DiscOn == 0) return Fail(CommandError.InvalidArgument, "The lab is not on the register.");
            if (lab.DiscOpen > 0 || lab.DiscPrep > 0) return Fail(CommandError.InvalidArgument, "A report is waiting or being prepared.");
            Disclosure.Leave(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult FileReport(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DisclosureGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.DiscOn == 0) return Fail(CommandError.InvalidArgument, "The lab is not on the register.");
            if (cmd.Count != 1 && cmd.Count != 2) return Fail(CommandError.InvalidArgument, "A report is a summary (1) or a full report (2).");
            if (lab.DiscOpen == 0) return Fail(CommandError.InvalidArgument, "No report is waiting.");
            if (cmd.Count == 2)
            {
                if (lab.DiscPrep > 0) return Fail(CommandError.InvalidArgument, "A full report is already being prepared.");
                int staff = Disclosure.SafetyStaff(c, lab);
                if (staff < c.Disclosure.FullStaff) return Fail(CommandError.NotEnoughStaff, "A full report needs " + c.Disclosure.FullStaff + " safety people and the lab has " + staff + ".");
            }
            Disclosure.File(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}
