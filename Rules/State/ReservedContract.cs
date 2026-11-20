using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>A multi-year rental at a discounted rate. Ends when <see cref="WeeksLeft"/> reaches zero.</summary>
    public sealed class ReservedContract : ISync
    {
        public int GpuCount;
        public int WeeksLeft;

        public void Sync(Syncer s)
        {
            s.Int("gpuCount", ref GpuCount);
            s.Int("weeksLeft", ref WeeksLeft);
        }
    }
}