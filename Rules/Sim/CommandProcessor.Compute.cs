using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SignReservedCompute(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Reserve at least one GPU.");
            long deposit = ComputeContractSystem.Deposit(c, state, lab, cmd.Count);
            if (lab.Cash < deposit) return NeedCash(lab, deposit);
            Finance.Spend(lab, LedgerLine.GpuRent, deposit);
            ComputeContractSystem.Sign(c, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult QueueGpuPurchase(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Queue at least one GPU.");
            int holdingId = cmd.TargetId;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            CommandResult place = CheckPlaceForHardware(c, state, lab, holdingId, gen, cmd.Count);
            if (!place.Ok) return place;
            VendorStanding standing = VendorSystem.EnsureStanding(c, state, lab.Id);
            standing.QueuedGpus += cmd.Count;
            standing.QueueHoldingId = holdingId;
            return CommandResult.Done(-1);
        }
    }
}