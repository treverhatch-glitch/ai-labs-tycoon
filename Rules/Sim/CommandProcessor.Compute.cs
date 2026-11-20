using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SignReservedCompute(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return FrozenIn(c, lab, lab.HomeBlocId);
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Reserve at least one GPU.");
            long deposit = ComputeContractSystem.Deposit(c, state, lab, cmd.Count);
            if (lab.Cash < deposit) return NeedCash(lab, deposit);
            Finance.Spend(lab, LedgerLine.GpuRent, deposit);
            ComputeContractSystem.Sign(c, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult QueueGpuPurchase(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.Count < 0) return Fail(CommandError.InvalidArgument, "Queue count cannot be negative.");
            VendorStanding standing = VendorSystem.EnsureStanding(c, state, lab.Id);
            if (cmd.Count == 0)
            {
                standing.QueuedGpus = 0;
                standing.QueueHoldingId = HardwareBatch.Hq;
                return CommandResult.Done(-1);
            }
            int holdingId = cmd.TargetId;
            if (standing.QueuedGpus > 0 && standing.QueueHoldingId != holdingId)
                return Fail(CommandError.InvalidArgument, "The queue already targets holding " + standing.QueueHoldingId + "; cancel or fulfil it before queuing another site.");
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            int total = standing.QueuedGpus + cmd.Count;
            CommandResult place = CheckPlaceForHardware(c, state, lab, holdingId, gen, total);
            if (!place.Ok) return place;
            standing.QueuedGpus += cmd.Count;
            standing.QueueHoldingId = holdingId;
            return CommandResult.Done(-1);
        }
    }
}