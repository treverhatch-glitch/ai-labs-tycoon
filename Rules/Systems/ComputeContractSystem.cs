using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ComputeContractSystem
    {
        public static int ReservedGpus(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.ReservedContracts.Count; i++) n += lab.ReservedContracts[i].GpuCount;
            return n;
        }

        public static long WeeklyReservedRent(GameConfig c, GameState state, LabState lab)
        {
            int count = ReservedGpus(lab);
            if (count == 0) return 0;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            long spot = gen.RentPerGpuWeek * count;
            return MathI.MulPermille(spot, c.Compute.ReservedRentPermille);
        }

        public static void RunWeek(GameConfig c, LabState lab)
        {
            for (int i = lab.ReservedContracts.Count - 1; i >= 0; i--)
            {
                ReservedContract contract = lab.ReservedContracts[i];
                contract.WeeksLeft--;
                if (contract.WeeksLeft <= 0) lab.ReservedContracts.RemoveAt(i);
            }
        }

        public static void Sign(GameConfig c, LabState lab, int count)
        {
            var contract = new ReservedContract();
            contract.GpuCount = count;
            contract.WeeksLeft = c.Compute.ReservedContractWeeks;
            lab.ReservedContracts.Add(contract);
        }

        public static long Deposit(GameConfig c, GameState state, LabState lab, int count)
        {
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            long spot = gen.RentPerGpuWeek * count;
            long weekRent = MathI.MulPermille(spot, c.Compute.ReservedRentPermille);
            return weekRent * c.Compute.ReservedDepositWeeks;
        }

        public static void FulfillQueues(GameConfig c, GameState state)
        {
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Bankrupt) continue;
                VendorStanding standing = state.Vendor.Standing(lab.Id);
                if (standing == null || standing.QueuedGpus <= 0) continue;
                int holdingId = standing.QueueHoldingId;
                GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
                long unitPrice = VendorSystem.PricePerGpu(c, state, lab, gen);
                while (standing.QueuedGpus > 0)
                {
                    int left = VendorSystem.AllocationLeft(state, lab);
                    if (left <= 0) break;
                    int buy = standing.QueuedGpus < left ? standing.QueuedGpus : left;
                    if (unitPrice > 0)
                    {
                        int affordable = (int)MathI.Min(lab.Cash / unitPrice, buy);
                        buy = affordable;
                    }
                    if (buy < 1) break;
                    CommandResult result = CommandProcessor.Apply(c, state, Command.BuyGpus(lab.Id, buy, holdingId));
                    if (!result.Ok)
                    {
                        Events.Add(state, EventKind.GpuQueueStalled, lab.Id, holdingId, standing.QueuedGpus, result.Message);
                        standing.QueuedGpus = 0;
                        break;
                    }
                    standing.QueuedGpus -= buy;
                }
            }
        }
    }
}