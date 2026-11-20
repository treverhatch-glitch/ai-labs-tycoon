using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class BubbleConfig : ISync
    {
        /// <summary>How much the meter drops each month without new hot money.</summary>
        public int DecayPermillePerMonth;
        /// <summary>When the meter reaches this, a pop fires without waiting for the news event.</summary>
        public int PopAtPermille;
        /// <summary>World event id that always pops the bubble when it fires.</summary>
        public string PopWorldEventId = "";
        /// <summary>Listed-chip trades and funding rounds add these amounts to the meter.</summary>
        public int ListedChipBoostPermille;
        public int DefaultFundingBoostPermille;
        /// <summary>On pop: labs with revenue share at or above this survive; weaker labs lose cash.</summary>
        public int SurvivorRevenuePpm;
        public int WeakCashHaircutPermille;

        public void Sync(Syncer s)
        {
            s.Int("decayPermillePerMonth", ref DecayPermillePerMonth);
            s.Int("popAtPermille", ref PopAtPermille);
            s.Str("popWorldEventId", ref PopWorldEventId);
            s.Int("listedChipBoostPermille", ref ListedChipBoostPermille);
            s.Int("defaultFundingBoostPermille", ref DefaultFundingBoostPermille);
            s.Int("survivorRevenuePpm", ref SurvivorRevenuePpm);
            s.Int("weakCashHaircutPermille", ref WeakCashHaircutPermille);
        }
    }
}