using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Revenue-based financing. A lab with steady revenue can take one cash advance against it and pay it back as a share of each week's revenue until a fixed multiple of the advance has been swept. A bigger share clears it sooner for a lower multiple; a slump stretches it out. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class RbfConfig : ISync
    {
        public int Enabled;
        /// <summary>Weekly revenue a lab needs to take an advance.</summary>
        public int MinWeeklyRevenue;
        /// <summary>Weeks of revenue the largest advance is.</summary>
        public int AdvanceWeeks;
        /// <summary>Tier 1 share of each week's revenue swept, in permille.</summary>
        public int ShareA;
        /// <summary>Tier 2 share.</summary>
        public int ShareB;
        /// <summary>Tier 3 share.</summary>
        public int ShareC;
        /// <summary>Tier 1 total repaid, in permille of the advance.</summary>
        public int CapA;
        /// <summary>Tier 2 total repaid.</summary>
        public int CapB;
        /// <summary>Tier 3 total repaid.</summary>
        public int CapC;
        /// <summary>A week with revenue under this share of what it was at the advance is a slump week.</summary>
        public int SlumpPermille;
        /// <summary>What a slump week adds to what is owed, in permille of the advance.</summary>
        public int SlumpCap;
        /// <summary>Weeks before the advance can be bought out.</summary>
        public int BuyoutAfter;
        /// <summary>Price of buying out, in permille of what is still owed.</summary>
        public int BuyoutPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minWeeklyRevenue", ref MinWeeklyRevenue);
            s.Int("advanceWeeks", ref AdvanceWeeks);
            s.Int("shareA", ref ShareA);
            s.Int("shareB", ref ShareB);
            s.Int("shareC", ref ShareC);
            s.Int("capA", ref CapA);
            s.Int("capB", ref CapB);
            s.Int("capC", ref CapC);
            s.Int("slumpPermille", ref SlumpPermille);
            s.Int("slumpCap", ref SlumpCap);
            s.Int("buyoutAfter", ref BuyoutAfter);
            s.Int("buyoutPermille", ref BuyoutPermille);
        }
    }
}
