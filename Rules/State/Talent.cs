using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>A rival's offer to one of the player's people, waiting for an answer until <see cref="ExpiresTick"/>.</summary>
    public sealed class PoachOffer : ISync
    {
        public int Id;
        public int StaffId;
        public int RivalLabId;
        /// <summary>What the person is worth: the sum counters are priced against.</summary>
        public long Package;
        public int ExpiresTick;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Int("staffId", ref StaffId);
            s.Int("rivalLabId", ref RivalLabId);
            s.Long("package", ref Package);
            s.Int("expiresTick", ref ExpiresTick);
        }
    }
}
