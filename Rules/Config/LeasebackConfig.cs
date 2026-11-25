using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Selling the lab's own GPUs and leasing them straight back. The lab gets cash now and keeps the compute, but pays rent every week, cannot sell the units, and has less to borrow against. It can buy them back at a set price, extend once, hand them back early, or be repossessed if it stops paying. Only the player's lab, and only the GPUs at the head office. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class LeasebackConfig : ISync
    {
        public int Enabled;
        /// <summary>Share of the units' resale value paid now.</summary>
        public int AdvancePermille;
        /// <summary>Weekly rent, in permille of the units' resale value at the sale.</summary>
        public int RentPermille;
        /// <summary>Weeks the lease runs before the units go back.</summary>
        public int TermWeeks;
        /// <summary>Price of buying the units back, in permille of their value at the sale.</summary>
        public int BuybackPermille;
        /// <summary>Weeks one extension adds.</summary>
        public int ExtendWeeks;
        /// <summary>Rent after an extension, in permille of the old rent.</summary>
        public int ExtendRentPermille;
        /// <summary>Weeks of missed rent in a row before the units are repossessed.</summary>
        public int MissLimit;
        /// <summary>Reputation lost to a repossession.</summary>
        public int RepoReputation;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("advancePermille", ref AdvancePermille);
            s.Int("rentPermille", ref RentPermille);
            s.Int("termWeeks", ref TermWeeks);
            s.Int("buybackPermille", ref BuybackPermille);
            s.Int("extendWeeks", ref ExtendWeeks);
            s.Int("extendRentPermille", ref ExtendRentPermille);
            s.Int("missLimit", ref MissLimit);
            s.Int("repoReputation", ref RepoReputation);
        }
    }
}
