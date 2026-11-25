using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Going public. A lab with enough revenue and reputation can list on the market: it sells a slice of itself for cash, pays to report, and every quarter sets guidance and is judged by whether it beat it. The share price moves with the results; a high price lets it sell more shares or buy them back, a low one brings an activist onto the board, and the lab can take itself private again. Only the player's lab, and not a nonprofit. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ListingConfig : ISync
    {
        public int Enabled;
        /// <summary>Reputation a lab needs to list.</summary>
        public int MinReputation;
        /// <summary>Weekly revenue a lab needs to list.</summary>
        public int MinWeeklyRevenue;
        /// <summary>Weeks of the lab's smoothed revenue its value is taken to be at a price of 1000.</summary>
        public int ValuationWeeks;
        /// <summary>Share of the lab sold at the listing, in permille.</summary>
        public int FloatPermille;
        /// <summary>Share of the proceeds the listing costs.</summary>
        public int ListingFeePermille;
        /// <summary>What reporting costs each week once listed.</summary>
        public int ReportingCost;
        /// <summary>Weeks between earnings reports.</summary>
        public int QuarterWeeks;
        /// <summary>Weeks at the start of a quarter in which guidance can be set.</summary>
        public int GuidanceWindow;
        /// <summary>Guidance level 1 target, in permille of the expected quarter.</summary>
        public int Conservative;
        /// <summary>Guidance level 2 target.</summary>
        public int Base;
        /// <summary>Guidance level 3 target.</summary>
        public int Stretch;
        /// <summary>How far over the target, in permille, counts as a beat.</summary>
        public int BeatMargin;
        /// <summary>Price gain of a beat, times the guidance level, in permille of the price.</summary>
        public int BeatMove;
        /// <summary>Price gain of meeting guidance.</summary>
        public int MeetMove;
        /// <summary>Price loss of a miss, times the guidance level, in permille of the price.</summary>
        public int MissMove;
        /// <summary>Price loss of a quarter with no guidance.</summary>
        public int NoGuidanceMove;
        /// <summary>The lowest the price falls.</summary>
        public int PriceFloor;
        /// <summary>The highest the price rises.</summary>
        public int PriceCap;
        /// <summary>Share of the lab a follow-on offering sells.</summary>
        public int FollowOnPermille;
        /// <summary>Weeks between follow-on offerings.</summary>
        public int FollowOnWeeks;
        /// <summary>Price gain from spending a share of the lab's value on buying shares back, in permille of that share.</summary>
        public int BuybackImpact;
        /// <summary>A price below this at an earnings report brings the activist closer.</summary>
        public int ActivistBelow;
        /// <summary>Reputation each step of the activist costs.</summary>
        public int ActivistReputation;
        /// <summary>Steps of the activist.</summary>
        public int MaxActivist;
        /// <summary>Price paid to buy the public's shares back, in permille of their value.</summary>
        public int TakePrivatePermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minReputation", ref MinReputation);
            s.Int("minWeeklyRevenue", ref MinWeeklyRevenue);
            s.Int("valuationWeeks", ref ValuationWeeks);
            s.Int("floatPermille", ref FloatPermille);
            s.Int("listingFeePermille", ref ListingFeePermille);
            s.Int("reportingCost", ref ReportingCost);
            s.Int("quarterWeeks", ref QuarterWeeks);
            s.Int("guidanceWindow", ref GuidanceWindow);
            s.Int("conservative", ref Conservative);
            s.Int("base", ref Base);
            s.Int("stretch", ref Stretch);
            s.Int("beatMargin", ref BeatMargin);
            s.Int("beatMove", ref BeatMove);
            s.Int("meetMove", ref MeetMove);
            s.Int("missMove", ref MissMove);
            s.Int("noGuidanceMove", ref NoGuidanceMove);
            s.Int("priceFloor", ref PriceFloor);
            s.Int("priceCap", ref PriceCap);
            s.Int("followOnPermille", ref FollowOnPermille);
            s.Int("followOnWeeks", ref FollowOnWeeks);
            s.Int("buybackImpact", ref BuybackImpact);
            s.Int("activistBelow", ref ActivistBelow);
            s.Int("activistReputation", ref ActivistReputation);
            s.Int("maxActivist", ref MaxActivist);
            s.Int("takePrivatePermille", ref TakePrivatePermille);
        }
    }
}
