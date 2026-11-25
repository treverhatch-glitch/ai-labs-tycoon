using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The world's news landing on the lab. The big dated events of each era fall into three kinds: a squeeze (talent, chips, a heatwave), a backlash (a scare, a lawsuit, a recall) and a boom. The player has a few weeks to answer: brace (pay now to take less of a hit, or to gain more of a boom), ride it, or counter it with a model good enough to turn it round. An answer that is not given is a ride. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ShocksConfig : ISync
    {
        public int Enabled;
        /// <summary>Weeks the lab has to answer a shock before it is taken as it comes.</summary>
        public int WindowWeeks;
        /// <summary>What bracing costs for each person on the staff.</summary>
        public int BraceCost;
        /// <summary>Share of the hit a lab that braced still takes, in permille.</summary>
        public int BracePermille;
        /// <summary>Reputation a backlash costs a lab that rides it out (damped).</summary>
        public int HitReputation;
        /// <summary>Morale a squeeze costs every person at a lab that rides it out.</summary>
        public int HitMorale;
        /// <summary>Cash a squeeze costs for each person on the staff.</summary>
        public int HitCash;
        /// <summary>Reputation a boom brings a lab that rides it.</summary>
        public int BoomReputation;
        /// <summary>What an investing lab gains from a boom, in permille of that.</summary>
        public int BoomBrace;
        /// <summary>Capability of the best released model a lab needs to counter a shock.</summary>
        public int CounterBar;
        /// <summary>What a lab that countered without the model takes, in permille of the hit or the gain it loses.</summary>
        public int CounterMiss;
        /// <summary>Reputation a lab gains for a counter that works on a squeeze or a backlash.</summary>
        public int CounterGain;
        /// <summary>What a lab gains for a counter that works on a boom, in permille of the boom.</summary>
        public int CounterBoom;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("windowWeeks", ref WindowWeeks);
            s.Int("braceCost", ref BraceCost);
            s.Int("bracePermille", ref BracePermille);
            s.Int("hitReputation", ref HitReputation);
            s.Int("hitMorale", ref HitMorale);
            s.Int("hitCash", ref HitCash);
            s.Int("boomReputation", ref BoomReputation);
            s.Int("boomBrace", ref BoomBrace);
            s.Int("counterBar", ref CounterBar);
            s.Int("counterMiss", ref CounterMiss);
            s.Int("counterGain", ref CounterGain);
            s.Int("counterBoom", ref CounterBoom);
        }
    }
}
