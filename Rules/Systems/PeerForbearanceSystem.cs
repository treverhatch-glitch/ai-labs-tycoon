using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q181: landlords can grant forbearance so renters skip peer debt trims; weekly subsidy optional.</summary>
    public static class PeerForbearanceSystem
    {
        public static bool RenterShielded(GameState state, LabState renter)
        {
            if (renter == null || renter.PeerComputeLeases.Count == 0) return false;
            int tick = state.Tick;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState landlord = state.Labs[i];
                if (landlord.Bankrupt) continue;
                for (int k = 0; k < landlord.PeerForbearances.Count; k++)
                {
                    PeerForbearance grant = landlord.PeerForbearances[k];
                    if (grant.RenterLabId != renter.Id || grant.UntilTick <= tick) continue;
                    for (int p = 0; p < renter.PeerComputeLeases.Count; p++)
                        if (renter.PeerComputeLeases[p].LandlordLabId == landlord.Id) return true;
                }
            }
            return false;
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            long rate = c.Compute.PeerForbearanceSubsidyPerGpuWeek;
            if (rate < 1) return;
            int tick = state.Tick;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState landlord = state.Labs[i];
                if (landlord.Bankrupt || landlord.PeerForbearances.Count == 0) continue;
                for (int k = landlord.PeerForbearances.Count - 1; k >= 0; k--)
                {
                    PeerForbearance grant = landlord.PeerForbearances[k];
                    if (grant.UntilTick <= tick)
                    {
                        landlord.PeerForbearances.RemoveAt(k);
                        continue;
                    }
                    LabState renter = state.Lab(grant.RenterLabId);
                    if (renter == null || renter.Bankrupt) continue;
                    PeerComputeLease lease = PeerComputeSystem.LeaseFrom(renter, landlord.Id);
                    if (lease == null || lease.GpuCount < 1) continue;
                    long pay = rate * lease.GpuCount;
                    if (pay > 0 && landlord.Cash >= pay && renter.Cash + pay >= 0)
                    {
                        Finance.Spend(landlord, LedgerLine.Grants, pay);
                        Finance.Earn(renter, LedgerLine.Grants, pay);
                    }
                }
            }
        }
    }
}