using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ReliabilityGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps a warranty or spares.");
            if (!HardwareReliability.Enabled(c)) return Fail(CommandError.InvalidArgument, "GPUs do not fail in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult SetWarranty(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ReliabilityGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.Count < 0 || cmd.Count > 2) return Fail(CommandError.InvalidArgument, "The warranty is tier 0 (none), 1 or 2.");
            if (cmd.Count == lab.WarrantyTier) return Fail(CommandError.InvalidArgument, "The warranty is already at that tier.");
            if (cmd.Count > lab.WarrantyTier && lab.Cash < c.Reliability.SwitchFee) return NeedCash(lab, c.Reliability.SwitchFee);
            HardwareReliability.SetWarranty(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult StockSpares(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ReliabilityGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Buy at least one spare.");
            if (lab.Spares + cmd.Count > c.Reliability.MaxSpares) return Fail(CommandError.InvalidArgument, "The lab can hold " + c.Reliability.MaxSpares + " spares.");
            long price = HardwareReliability.SparePrice(c, state) * cmd.Count;
            if (lab.Cash < price) return NeedCash(lab, price);
            HardwareReliability.Stock(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult ExpediteReplacements(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ReliabilityGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.RmaUnits == 0) return Fail(CommandError.InvalidArgument, "No replacements are waiting.");
            long price = HardwareReliability.ExpeditePrice(c, state, lab);
            if (lab.Cash < price) return NeedCash(lab, price);
            HardwareReliability.Expedite(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
