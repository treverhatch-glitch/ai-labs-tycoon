using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult OpenRequisition(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab places orders for people.");
            if (!Requisitions.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no open requisitions in this game.");
            RoleConfig role = c.Role(cmd.Id);
            if (role == null) return Fail(CommandError.UnknownId, "There is no role \"" + cmd.Id + "\".");
            if (cmd.Count < 1 || cmd.Count > c.Requisitions.MaxCount) return Fail(CommandError.InvalidArgument, "An order is from 1 to " + c.Requisitions.MaxCount + " people.");
            if (lab.Requisitions.Count >= c.Requisitions.MaxOpen) return Fail(CommandError.InvalidArgument, "The lab already has " + lab.Requisitions.Count + " open orders.");
            long deposit = Requisitions.DepositEach(c, Requisitions.LockedFee(c, state, lab, role)) * cmd.Count;
            if (lab.Cash < deposit) return NeedCash(lab, deposit);
            Requisitions.Open(c, state, lab, role, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult CancelRequisition(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab places orders for people.");
            if (cmd.TargetId < 0 || cmd.TargetId >= lab.Requisitions.Count) return Fail(CommandError.UnknownId, "There is no open order " + cmd.TargetId + ".");
            Requisitions.Cancel(c, state, lab, lab.Requisitions[cmd.TargetId]);
            return CommandResult.Done(-1);
        }
    }
}
