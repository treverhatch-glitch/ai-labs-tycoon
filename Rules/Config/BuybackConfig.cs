using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Buying a funding round back (bible ch. 5, control of your own company). A lab can pay the investors of a round it has raised to leave: it costs a multiple of the cash the round brought, dearer in a bubble, and the founders get that round's dilution back along with its board seats. A round can be bought once, and cannot be raised again. Only the player's lab. Zero in <c>Enabled</c> turns it off.
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

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("priceMult", ref PriceMult);
            s.Int("bubblePrice", ref BubblePrice);
            s.Int("buybackTrust", ref BuybackTrust);
        }
    }
}
