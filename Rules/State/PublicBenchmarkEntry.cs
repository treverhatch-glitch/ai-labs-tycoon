using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum PublicBenchmarkPhase
    {
        Vetting,
        Live,
        Rejected,
        Withdrawn,
        Demoted
    }

    /// <summary>One model submitted to a public benchmark scoreboard (Q228+).</summary>
    public sealed class PublicBenchmarkEntry : ISync
    {
        public string BenchmarkId = "";
        public int ModelId;
        public PublicBenchmarkPhase Phase = PublicBenchmarkPhase.Vetting;
        public int SubmittedTick;
        public int VettingEndsTick = -1;
        public long LastScore;
        public bool DefendPledged;

        public void Sync(Syncer s)
        {
            s.Str("benchmarkId", ref BenchmarkId);
            s.Int("modelId", ref ModelId);
            s.Enum("phase", ref Phase);
            s.Int("submittedTick", ref SubmittedTick);
            s.Int("vettingEndsTick", ref VettingEndsTick);
            s.Long("lastScore", ref LastScore);
            s.Bool("defendPledged", ref DefendPledged);
        }
    }
}