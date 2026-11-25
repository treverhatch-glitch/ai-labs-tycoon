using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Buying a funding round back (bible ch. 5, control of your own company). The board votes on it. Votes follow the equity: the founders carry what governance gives them (dual-class and a non-profit parent carry more), each round still held carries its dilution, independent directors carry a few each. The round being bought votes no, the other investors vote no if it drains the cash, directors vote no if it leaves too little runway. A passed buyback costs a multiple of the cash the round brought, dearer in a bubble, and gives that round's share and seats back, which changes every later vote; a refused one shuts the board for weeks. A round can be bought once, and cannot be raised again. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class BuybackConfig : ISync
    {
        public int Enabled;
        /// <summary>What buying a round back costs, as a multiple of the cash it brought (permille, 2000 is twice).</summary>
        public int PriceMult;
        /// <summary>Extra permille of the price for every permille of bubble pressure when the lab buys.</summary>
        public int BubblePrice;
        /// <summary>Reputation a lab gains from the market for buying out an investor (permille of a point, 1000 is one point).</summary>
        public int BuybackTrust;

        /// <summary>Weeks the board will not hear another buyback after it votes one down.</summary>
        public int VoteCooldown;

        /// <summary>Votes each independent director carries (permille of the equity).</summary>
        public int DirectorVote;

        /// <summary>Largest share of the cash the lab holds a buyback can take before the other investors vote it down (permille).</summary>
        public int SpendCapPermille;

        /// <summary>Weeks of running costs the lab must still have after a buyback for the directors to vote it through.</summary>
        public int RunwayWeeks;

        /// <summary>Share of all votes a buyback needs to pass (permille).</summary>
        public int PassPermille;

        /// <summary>Whether directors vote no on a buyback while a spot reclaim wave is on (1 yes, 0 no).</summary>
        public int WaveBlocksDirectors;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("priceMult", ref PriceMult);
            s.Int("bubblePrice", ref BubblePrice);
            s.Int("buybackTrust", ref BuybackTrust);
            s.Int("voteCooldown", ref VoteCooldown);
            s.Int("directorVote", ref DirectorVote);
            s.Int("spendCapPermille", ref SpendCapPermille);
            s.Int("runwayWeeks", ref RunwayWeeks);
            s.Int("passPermille", ref PassPermille);
            s.Int("waveBlocksDirectors", ref WaveBlocksDirectors);
        }
    }
}
