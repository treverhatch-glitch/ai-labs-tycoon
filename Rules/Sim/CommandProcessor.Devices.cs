using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult DeviceGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can make a device.");
            if (!Devices.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody makes devices in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult DesignDevice(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DeviceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Devices.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a device line.");
            if (cmd.TargetId < 1 || cmd.TargetId > 3) return Fail(CommandError.InvalidArgument, "The device is 1 speaker, 2 glasses or 3 home robot.");
            if (cmd.Count < 0 || cmd.Count > c.Devices.MaxTestWeeks) return Fail(CommandError.InvalidArgument, "Testing is from 0 to " + c.Devices.MaxTestWeeks + " weeks.");
            if (Devices.EraNow(c, state) < Devices.Era(c, cmd.TargetId)) return Fail(CommandError.MissingPrerequisite, "That device is for a later era.");
            ModelState best = LabQueries.BestReleased(lab);
            if (best == null || best.Capability < c.Devices.MinCapability) return Fail(CommandError.MissingPrerequisite, "The lab needs a released model of capability " + c.Devices.MinCapability + " to put in a box.");
            long cost = Devices.Cost(c, cmd.TargetId);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Devices.Design(c, state, lab, cmd.TargetId, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult SetDevicePrice(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DeviceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Devices.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no device line.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The price is tier 1, 2 or 3.");
            lab.DevTier = cmd.Count;
            return CommandResult.Done(-1);
        }

        static CommandResult WithdrawDevice(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = DeviceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Devices.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no device line.");
            Devices.Withdraw(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
