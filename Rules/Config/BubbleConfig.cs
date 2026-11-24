using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class BubbleConfig : ISync
    {
        /// <summary>How much the meter drops each month without new hot money.</summary>
        public int DecayPermillePerMonth;
        /// <summary>When the meter reaches this, a crash starts without waiting for the news event.</summary>
        public int PopAtPermille;
        /// <summary>World event id that always starts a crash when it fires.</summary>
        public string PopWorldEventId = "";
        /// <summary>Listed-chip trades and funding rounds add these amounts to the meter.</summary>
        public int ListedChipBoostPermille;
        public int DefaultFundingBoostPermille;
        /// <summary>On crash: labs with revenue share at or above this survive; weaker labs lose cash.</summary>
        public int SurvivorRevenuePpm;
        public int WeakCashHaircutPermille;
        /// <summary>When true, a player lab that takes a crash haircut gets a the-pop ending record.</summary>
        public bool ThePopEndingForPlayer;

        /// <summary>Above this permille, weak-return months count toward grace.</summary>
        public int HotPermille;
        /// <summary>Consecutive weak-return months while hot before a crash.</summary>
        public int GraceMonthsBeforeCrash;
        /// <summary>Monthly market revenue permille of capital raised needed to reset grace.</summary>
        public int ReturnsNeededPermille;
        /// <summary>Extra meter drop when returns beat the hurdle while hot.</summary>
        public int ReturnCoolingPermille;
        /// <summary>Months of crash effects after a burst.</summary>
        public int CrashDurationMonths;
        /// <summary>Meter after a crash ends; never returns to zero.</summary>
        public int PostCrashResidualPermille;
        /// <summary>Sector customer pools during a crash.</summary>
        public int CrashCustomerSpendPermille;
        /// <summary>Venture cash during a crash (stacks with down rounds).</summary>
        public int CrashFundingCashPermille;
        /// <summary>GPU list prices during a crash.</summary>
        public int CrashGpuPricePermille;
        /// <summary>Monthly rival public-float cut during a crash, in ppm.</summary>
        public int CrashFloatCutPpm;
        /// <summary>Warning level 2 when permille is within this many points of popAtPermille.</summary>
        public int PopWarningMarginPermille;
        /// <summary>When true, clear cumulative hot-money tally when a crash phase fully ends.</summary>
        public bool ResetCapitalRaisedAfterCrash;

        public void Sync(Syncer s)
        {
            s.Int("decayPermillePerMonth", ref DecayPermillePerMonth);
            s.Int("popAtPermille", ref PopAtPermille);
            s.Str("popWorldEventId", ref PopWorldEventId);
            s.Int("listedChipBoostPermille", ref ListedChipBoostPermille);
            s.Int("defaultFundingBoostPermille", ref DefaultFundingBoostPermille);
            s.Int("survivorRevenuePpm", ref SurvivorRevenuePpm);
            s.Int("weakCashHaircutPermille", ref WeakCashHaircutPermille);
            s.Bool("thePopEndingForPlayer", ref ThePopEndingForPlayer);
            s.Int("hotPermille", ref HotPermille);
            s.Int("graceMonthsBeforeCrash", ref GraceMonthsBeforeCrash);
            s.Int("returnsNeededPermille", ref ReturnsNeededPermille);
            s.Int("returnCoolingPermille", ref ReturnCoolingPermille);
            s.Int("crashDurationMonths", ref CrashDurationMonths);
            s.Int("postCrashResidualPermille", ref PostCrashResidualPermille);
            s.Int("crashCustomerSpendPermille", ref CrashCustomerSpendPermille);
            s.Int("crashFundingCashPermille", ref CrashFundingCashPermille);
            s.Int("crashGpuPricePermille", ref CrashGpuPricePermille);
            s.Int("crashFloatCutPpm", ref CrashFloatCutPpm);
            s.Int("popWarningMarginPermille", ref PopWarningMarginPermille);
            s.Bool("resetCapitalRaisedAfterCrash", ref ResetCapitalRaisedAfterCrash);
        }
    }
}