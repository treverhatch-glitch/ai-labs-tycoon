using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Promotions. The player picks a person and makes them senior: a fee and a raise now, and from then on they count for more in their function
    /// (a senior researcher makes more research, a senior engineer or safety tester or salesperson counts for more than one head). Seats are scarce,
    /// so a senior is the way to get more out of the same chair, but a function can be only so senior and the salary rises with the weight.
    /// Seniors are the last to go when people are let go. Only the player's lab promotes. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class SeniorityConfig : ISync
    {
        /// <summary>1 when people can be promoted, 0 when not.</summary>
        public int Enabled;
        /// <summary>How many people a senior counts for, in permille (1600 is one and a half people and a bit more).</summary>
        public int WeightPermille;
        /// <summary>The raise a senior gets on the role's weekly salary, in permille.</summary>
        public int RaisePermille;
        /// <summary>The one-off fee, as a share of the role's hire cost, in permille.</summary>
        public int FeePermille;
        /// <summary>The most seniors a function can have, as a share of its people in permille, rounded up (so a small team can have one).</summary>
        public int MaxSharePermille;
        /// <summary>The morale a person needs to be promoted at all.</summary>
        public int MinMorale;
        /// <summary>Morale the promoted person gains.</summary>
        public int MoraleBoost;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("weightPermille", ref WeightPermille);
            s.Int("raisePermille", ref RaisePermille);
            s.Int("feePermille", ref FeePermille);
            s.Int("maxSharePermille", ref MaxSharePermille);
            s.Int("minMorale", ref MinMorale);
            s.Int("moraleBoost", ref MoraleBoost);
        }
    }
}
