using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A layoff round. The player lets go of the same share of every function in one command, on a collective package that is cheaper per head than letting
    /// people go one at a time. The cost is social: the people who stay lose morale and the press loses some respect, both in proportion to the share of the
    /// staff let go. Only the player's lab does it. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class LayoffsConfig : ISync
    {
        /// <summary>1 when rounds are allowed, 0 when not.</summary>
        public int Enabled;
        /// <summary>The largest share of each function one round can let go, in permille.</summary>
        public int MaxPermille;
        /// <summary>What each person costs, as a share of their role's severance, in permille.</summary>
        public int SeverancePermille;
        /// <summary>Reputation points lost if the whole staff were let go (less for a share of it).</summary>
        public int ReputationAtAll;
        /// <summary>Morale each person who stays loses if the whole staff were let go (less for a share of it).</summary>
        public int MoraleAtAll;
        /// <summary>The share of the whole staff, in permille, from which a round counts as a deep cut.</summary>
        public int DeepPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxPermille", ref MaxPermille);
            s.Int("severancePermille", ref SeverancePermille);
            s.Int("reputationAtAll", ref ReputationAtAll);
            s.Int("moraleAtAll", ref MoraleAtAll);
            s.Int("deepPermille", ref DeepPermille);
        }
    }
}
