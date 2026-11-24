using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>
    /// A model that came out better than it had any right to be, and nobody knows why. It waits on the desk until <see cref="ExpiresTick"/> or
    /// until the lab follows it, which takes research points and weeks and ends in whatever was behind it.
    /// </summary>
    public sealed class ResearchLead : ISync
    {
        public int Id;
        public int ModelId;
        /// <summary>How much better than expected the run came out, in permille of its capability.</summary>
        public int Luck;
        public int FoundTick;
        public int ExpiresTick;
        /// <summary>The tick the investigation ends, or 0 while nobody is following the lead.</summary>
        public int DoneTick;
        /// <summary>The model's name.</summary>
        public string Ref = "";

        public bool Following { get { return DoneTick > 0; } }

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Int("modelId", ref ModelId);
            s.Int("luck", ref Luck);
            s.Int("foundTick", ref FoundTick);
            s.Int("expiresTick", ref ExpiresTick);
            s.Int("doneTick", ref DoneTick);
            s.Str("ref", ref Ref);
        }
    }
}
