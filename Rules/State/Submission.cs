using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>A paper out for review: the venue tier (1 workshop, 2 main, 3 flagship), the effort put in, the research points paid and the week of the decision.</summary>
    public sealed class Submission : ISync
    {
        public int Tier;
        public int Effort;
        public long Cost;
        public int Due;

        public void Sync(Syncer s)
        {
            s.Int("tier", ref Tier);
            s.Int("effort", ref Effort);
            s.Long("cost", ref Cost);
            s.Int("due", ref Due);
        }
    }
}
