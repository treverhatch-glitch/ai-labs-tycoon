using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>
    /// A pile of identical hardware in one place. A lab's fleet is a short list of these: the same hardware in the
    /// same place merges when it arrives. Exactly one of <see cref="GpuGenId"/> (one of the vendor's GPUs) or
    /// <see cref="ChipDesignId"/> (a chip the lab or another lab designed) is set; the other is empty or -1.
    /// </summary>
    public sealed class HardwareBatch : ISync
    {
        public int Id;
        /// <summary>-1 for the HQ's racks, else the id of one of the lab's holdings at a site.</summary>
        public int HoldingId;
        public string GpuGenId;
        public int ChipDesignId;
        /// <summary>The lab whose design a chip batch is, or -1.</summary>
        public int DesignLabId;
        public int Count;
        public int ArrivedTick;
        /// <summary>Weekly tick until resale is allowed; -1 = never locked (normal hardware).</summary>
        public int ResaleLockedUntilTick = -1;

        public const int Hq = -1;
        /// <summary>Listed chips run in a vendor cloud deal; no site or kilowatts on the lab's map.</summary>
        public const int Cloud = -2;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Int("holdingId", ref HoldingId);
            s.Str("gpuGenId", ref GpuGenId);
            s.Int("chipDesignId", ref ChipDesignId);
            s.Int("designLabId", ref DesignLabId);
            s.Int("count", ref Count);
            s.Int("arrivedTick", ref ArrivedTick);
            s.Int("resaleLockedUntilTick", ref ResaleLockedUntilTick);
        }
    }
}
