using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>A journalist's question about a beat, waiting for an answer until <see cref="ExpiresTick"/>.</summary>
    public sealed class PressInquiry : ISync
    {
        public int Id;
        public string JournalistId = "";
        public string BeatId = "";
        /// <summary>The reputation at play: what a good answer can win, and what a bad one can lose a share of.</summary>
        public int Stake;
        public int ExpiresTick;
        /// <summary>What the story is about (the model's name, for a launch or an incident).</summary>
        public string Ref = "";

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("journalistId", ref JournalistId);
            s.Str("beatId", ref BeatId);
            s.Int("stake", ref Stake);
            s.Int("expiresTick", ref ExpiresTick);
            s.Str("ref", ref Ref);
        }
    }
}
