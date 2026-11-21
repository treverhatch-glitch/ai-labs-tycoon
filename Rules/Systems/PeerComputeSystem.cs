using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PeerComputeSystem
    {
        public static int LeasedInGpus(LabState renter)
        {
            int n = 0;
            for (int i = 0; i < renter.PeerComputeLeases.Count; i++) n += renter.PeerComputeLeases[i].GpuCount;
            return n;
        }

        public static int LeasedOutFrom(GameState state, int landlordLabId)
        {
            int n = 0;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                if (lab.Bankrupt) continue;
                for (int k = 0; k < lab.PeerComputeLeases.Count; k++)
                {
                    PeerComputeLease lease = lab.PeerComputeLeases[k];
                    if (lease.LandlordLabId == landlordLabId) n += lease.GpuCount;
                }
            }
            return n;
        }

        public static PeerComputeLease LeaseFrom(LabState renter, int landlordLabId)
        {
            for (int i = 0; i < renter.PeerComputeLeases.Count; i++)
            {
                PeerComputeLease lease = renter.PeerComputeLeases[i];
                if (lease.LandlordLabId == landlordLabId) return lease;
            }
            return null;
        }

        public static int HqGpusFree(GameState state, LabState landlord)
        {
            int hq = LabQueries.HqGpus(landlord);
            int outLeased = LeasedOutFrom(state, landlord.Id);
            return hq - outLeased;
        }

        public static long WeeklyRent(GameConfig c, GameState state, int gpuCount)
        {
            if (gpuCount < 1) return 0;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            long spot = gen.RentPerGpuWeek * gpuCount;
            return ChipShortageSystem.ScalePrice(state, MathI.MulPermille(spot, c.Compute.PeerLeaseRentPermille));
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            var billed = new List<KeyValuePair<LabState, long>>();
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState renter = state.Labs[i];
                if (renter.Bankrupt || renter.PeerComputeLeases.Count == 0) continue;
                for (int k = 0; k < renter.PeerComputeLeases.Count; k++)
                {
                    PeerComputeLease lease = renter.PeerComputeLeases[k];
                    if (lease.GpuCount < 1) continue;
                    LabState landlord = state.Lab(lease.LandlordLabId);
                    if (landlord == null || landlord.Bankrupt) continue;
                    long rent = WeeklyRent(c, state, lease.GpuCount);
                    if (rent < 1) continue;
                    Finance.Spend(renter, LedgerLine.GpuRent, rent);
                    billed.Add(new KeyValuePair<LabState, long>(landlord, rent));
                }
            }
            for (int i = 0; i < billed.Count; i++) Finance.Earn(billed[i].Key, LedgerLine.Revenue, billed[i].Value);
        }

        public static void Sign(LabState renter, int landlordLabId, int count)
        {
            PeerComputeLease existing = LeaseFrom(renter, landlordLabId);
            if (existing != null) existing.GpuCount += count;
            else renter.PeerComputeLeases.Add(new PeerComputeLease { LandlordLabId = landlordLabId, GpuCount = count });
        }

        public static void Cancel(LabState renter, int landlordLabId, int count)
        {
            PeerComputeLease lease = LeaseFrom(renter, landlordLabId);
            if (lease == null) return;
            lease.GpuCount -= count;
            if (lease.GpuCount <= 0) renter.PeerComputeLeases.Remove(lease);
        }
    }
}