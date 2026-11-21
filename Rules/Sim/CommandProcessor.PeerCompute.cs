using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult RentPeerCompute(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Controller == LabController.Plan)
                return Fail(CommandError.InvalidArgument, "Plan labs rent from the vendor, not each other.");
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Rent at least one GPU.");
            if (cmd.TargetId < 0) return Fail(CommandError.InvalidArgument, "Name the landlord lab.");
            if (cmd.TargetId == lab.Id) return Fail(CommandError.InvalidArgument, "A lab cannot rent from itself.");
            LabState landlord = state.Lab(cmd.TargetId);
            if (landlord == null) return Fail(CommandError.UnknownLab, "There is no lab " + cmd.TargetId + ".");
            if (landlord.Bankrupt) return Fail(CommandError.LabBankrupt, landlord.Name + " is bankrupt.");
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return Fail(CommandError.PermitFrozen, c.World.Bloc(lab.HomeBlocId).Agency + " has frozen " + lab.Name + "'s permits at home.");
            int free = PeerComputeSystem.HqGpusFree(state, landlord);
            if (free < cmd.Count)
                return Fail(CommandError.NotEnoughGpus, landlord.Name + " has " + free + " HQ GPUs free to lease (you asked for " + cmd.Count + ").");
            long weekRent = PeerComputeSystem.WeeklyRent(c, state, cmd.Count);
            if (lab.Cash < weekRent) return NeedCash(lab, weekRent);
            PeerComputeSystem.Sign(lab, landlord.Id, cmd.Count);
            Finance.Spend(lab, LedgerLine.GpuRent, weekRent);
            Finance.Earn(landlord, LedgerLine.Revenue, weekRent);
            return CommandResult.Done(-1);
        }

        static CommandResult EndPeerCompute(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "End at least one GPU of peer rent.");
            if (cmd.TargetId < 0) return Fail(CommandError.InvalidArgument, "Name the landlord lab.");
            PeerComputeLease lease = PeerComputeSystem.LeaseFrom(lab, cmd.TargetId);
            if (lease == null || lease.GpuCount < cmd.Count)
                return Fail(CommandError.InvalidArgument, "This lab does not rent that many GPUs from lab " + cmd.TargetId + ".");
            PeerComputeSystem.Cancel(lab, cmd.TargetId, cmd.Count);
            return CommandResult.Done(-1);
        }
    }
}