using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Alumni network. People who leave the player's lab (poached, laid off or fired, or quit by blog post) stay in a pool for a limited time, remembered with their role, their trait and how they left. The player can bring one back: the fee is a share of the hiring fee that is smaller for a better parting, the person comes back with the same trait and a morale that depends on how they left, and their colleagues react once. A full pool drops its oldest leaver and old leavers expire. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class AlumniConfig : ISync
    {
        public int Enabled;
        /// <summary>People the pool holds; when it is full the oldest leaver drops out.</summary>
        public int MaxSize;
        /// <summary>Weeks a leaver stays in the pool.</summary>
        public int ExpireWeeks;
        /// <summary>What rehiring costs, as a share of the role's hiring fee, by how the person left: on good terms (poached to something better).</summary>
        public int FeeGoodPermille;
        /// <summary>Laid off or fired.</summary>
        public int FeeLaidOffPermille;
        /// <summary>Quit by blog post.</summary>
        public int FeeQuitPermille;
        /// <summary>Morale a returning person arrives with, added to where people settle, by how they left. May be negative.</summary>
        public int MoraleGood;
        public int MoraleLaidOff;
        public int MoraleQuit;
        /// <summary>Morale every colleague gains or loses, once, when the person returns, by how they left. May be negative.</summary>
        public int PeerGood;
        public int PeerLaidOff;
        public int PeerQuit;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxSize", ref MaxSize);
            s.Int("expireWeeks", ref ExpireWeeks);
            s.Int("feeGoodPermille", ref FeeGoodPermille);
            s.Int("feeLaidOffPermille", ref FeeLaidOffPermille);
            s.Int("feeQuitPermille", ref FeeQuitPermille);
            s.Int("moraleGood", ref MoraleGood);
            s.Int("moraleLaidOff", ref MoraleLaidOff);
            s.Int("moraleQuit", ref MoraleQuit);
            s.Int("peerGood", ref PeerGood);
            s.Int("peerLaidOff", ref PeerLaidOff);
            s.Int("peerQuit", ref PeerQuit);
        }
    }
}
