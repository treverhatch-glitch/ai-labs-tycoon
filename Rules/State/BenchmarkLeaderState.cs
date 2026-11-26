using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>Global holder of a public benchmark crown after the latest weekly refresh.</summary>
    public sealed class BenchmarkLeaderState : ISync
    {
        public string BenchmarkId = "";
        public int LabId = -1;
        public int ModelId = -1;
        public long Score;

        public void Sync(Syncer s)
        {
            s.Str("benchmarkId", ref BenchmarkId);
            s.Int("labId", ref LabId);
            s.Int("modelId", ref ModelId);
            s.Long("score", ref Score);
        }
    }
}