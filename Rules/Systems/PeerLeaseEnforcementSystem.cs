using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q171: overdue labs lose peer-leased GPUs each week until they are solvent again.</summary>
    public static class PeerLeaseEnforcementSystem
    {
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt || lab.WeeksInDebt < 1 || lab.PeerComputeLeases.Count == 0) return;
            int trim = c.Compute.PeerLeaseDebtGpuTrimPerWeek;
            if (trim < 1) return;
            for (int n = 0; n < trim; n++)
            {
                if (lab.PeerComputeLeases.Count == 0) break;
                PeerComputeLease lease = lab.PeerComputeLeases[0];
                lease.GpuCount--;
                if (lease.GpuCount <= 0) lab.PeerComputeLeases.RemoveAt(0);
                Events.Add(state, EventKind.PeerLeaseTrimmed, lab.Id, lease.LandlordLabId, 1, "debt");
            }
        }
    }
}