using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Era and reputation gates on buying the current GPU generation (Q476, schema 151).</summary>
    public sealed class GpuTierUnlockConfig : ISync
    {
        public int Enabled;
        /// <summary>Minimum lab reputation before the current generation may be bought or queued.</summary>
        public int UnlockPermille;
        /// <summary>Minimum era index (from <c>eras</c>) before tier purchases apply.</summary>
        public int MinEra;
        /// <summary>Apply the gate only when the generation tier is at least this.</summary>
        public int TierMin;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("unlockPermille", ref UnlockPermille);
            s.Int("minEra", ref MinEra);
            s.Int("tierMin", ref TierMin);
        }
    }
}