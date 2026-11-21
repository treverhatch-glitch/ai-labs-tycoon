using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>GPUs a lab rents from another lab's HQ fleet each week.</summary>
    public sealed class PeerComputeLease : ISync
    {
        public int LandlordLabId;
        public int GpuCount;

        public void Sync(Syncer s)
        {
            s.Int("landlordLabId", ref LandlordLabId);
            s.Int("gpuCount", ref GpuCount);
        }
    }
}