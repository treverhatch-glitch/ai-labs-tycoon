using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>A loan against the lab's chips: what was borrowed, what is still owed (interest included), the weekly payment and how many in a row were missed.</summary>
    public sealed class Loan : ISync
    {
        public int Id;
        public long Principal;
        /// <summary>Still owed, interest and penalties included.</summary>
        public long Balance;
        public long Payment;
        public int Misses;
        public int StartTick;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Long("principal", ref Principal);
            s.Long("balance", ref Balance);
            s.Long("payment", ref Payment);
            s.Int("misses", ref Misses);
            s.Int("startTick", ref StartTick);
        }
    }
}
