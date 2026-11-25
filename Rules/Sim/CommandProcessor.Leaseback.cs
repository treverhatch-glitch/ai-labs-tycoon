using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult LeaseGate(GameConfig c, GameState state, LabState lab, bool needLease)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can lease its GPUs back.");
            if (!Leaseback.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody leases GPUs back in this game.");
            if (needLease && !Leaseback.Leasing(lab)) return Fail(CommandError.InvalidArgument, "No GPUs are leased back.");
            return CommandResult.Done(-1);
        }

        static CommandResult SaleLeaseback(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LeaseGate(c, state, lab, false);
            if (!gate.Ok) return gate;
            if (Leaseback.Leasing(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a lease.");
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Lease back at least one GPU.");
            int free = Leaseback.Free(lab);
            if (cmd.Count > free) return Fail(CommandError.NotEnoughGpus, "The head office owns " + free + " GPUs to lease back.");
            Leaseback.Sell(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult BuyBackLease(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LeaseGate(c, state, lab, true);
            if (!gate.Ok) return gate;
            if (lab.Cash < lab.LeaseBuyback) return NeedCash(lab, lab.LeaseBuyback);
            Leaseback.BuyBack(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult ExtendLease(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LeaseGate(c, state, lab, true);
            if (!gate.Ok) return gate;
            if (lab.LeaseExtended == 1) return Fail(CommandError.InvalidArgument, "The lease has been extended once already.");
            if (c.Leaseback.ExtendWeeks < 1) return Fail(CommandError.InvalidArgument, "The lessor does not extend.");
            Leaseback.Extend(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult ReturnLease(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LeaseGate(c, state, lab, true);
            if (!gate.Ok) return gate;
            Leaseback.GiveBack(c, state, lab, 1);
            return CommandResult.Done(-1);
        }
    }
}
