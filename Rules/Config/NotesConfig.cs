using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Convertible notes. A bridge loan that turns into shares: the lab borrows against its future, the note earns interest, and it converts into equity at a discount at the next funding round or at a listing. The lab can repay it, extend it once, or let it reach maturity, when it converts on terms that dilute the founders hard. One note at a time. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class NotesConfig : ISync
    {
        public int Enabled;
        /// <summary>The most a note can be, in thousands.</summary>
        public int MaxThousands;
        /// <summary>Tier 1 weekly interest, in permille.</summary>
        public int RateA;
        /// <summary>Tier 2 weekly interest.</summary>
        public int RateB;
        /// <summary>Tier 3 weekly interest.</summary>
        public int RateC;
        /// <summary>Tier 1 discount at conversion, in permille.</summary>
        public int DiscountA;
        /// <summary>Tier 2 discount.</summary>
        public int DiscountB;
        /// <summary>Tier 3 discount.</summary>
        public int DiscountC;
        /// <summary>Tier 1 weeks to maturity.</summary>
        public int WeeksA;
        /// <summary>Tier 2 weeks to maturity.</summary>
        public int WeeksB;
        /// <summary>Tier 3 weeks to maturity.</summary>
        public int WeeksC;
        /// <summary>The most of the lab a note can convert into at once, in permille.</summary>
        public int MaxConvertPermille;
        /// <summary>Discount at maturity, in permille, in place of the note's own.</summary>
        public int MaturityDiscount;
        /// <summary>Weeks one extension adds.</summary>
        public int ExtendWeeks;
        /// <summary>What the rate becomes after an extension, in permille of the old rate.</summary>
        public int ExtendRate;
        /// <summary>Discount added by an extension, in permille.</summary>
        public int ExtendDiscount;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxThousands", ref MaxThousands);
            s.Int("rateA", ref RateA);
            s.Int("rateB", ref RateB);
            s.Int("rateC", ref RateC);
            s.Int("discountA", ref DiscountA);
            s.Int("discountB", ref DiscountB);
            s.Int("discountC", ref DiscountC);
            s.Int("weeksA", ref WeeksA);
            s.Int("weeksB", ref WeeksB);
            s.Int("weeksC", ref WeeksC);
            s.Int("maxConvertPermille", ref MaxConvertPermille);
            s.Int("maturityDiscount", ref MaturityDiscount);
            s.Int("extendWeeks", ref ExtendWeeks);
            s.Int("extendRate", ref ExtendRate);
            s.Int("extendDiscount", ref ExtendDiscount);
        }
    }
}
