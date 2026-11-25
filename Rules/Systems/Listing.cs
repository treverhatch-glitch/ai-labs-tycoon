using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab as a listed company. See <see cref="ListingConfig"/>. Only the player's lab.</summary>
    public static class Listing
    {
        public static bool Enabled(GameConfig c) { return c.Listing.Enabled > 0; }

        public static bool Listed(LabState lab) { return lab.Listed == 1; }

        public static long Revenue(LabState lab) { return MathI.Max(0, lab.Ledger.LastWeek.Revenue); }

        public static long Value(GameConfig c, LabState lab)
        {
            long basis = lab.ListBase > 0 ? lab.ListBase : Revenue(lab);
            long atPar = basis * c.Listing.ValuationWeeks;
            return Listed(lab) ? atPar * lab.ListPrice / MathI.Permille : atPar;
        }

        public static bool CanList(GameConfig c, LabState lab)
        {
            return !Listed(lab) && lab.Charter != Charter.Nonprofit && lab.Reputation >= c.Listing.MinReputation && Revenue(lab) >= c.Listing.MinWeeklyRevenue;
        }

        public static long Proceeds(GameConfig c, LabState lab, int permille)
        {
            return MathI.MulPermille(Value(c, lab), permille);
        }

        static int Level(int guidance) { return guidance < 1 ? 1 : guidance; }

        public static int Mult(GameConfig c, int level) { return level == 1 ? c.Listing.Conservative : level == 2 ? c.Listing.Base : c.Listing.Stretch; }

        public static bool WindowOpen(GameConfig c, GameState state, LabState lab)
        {
            return Listed(lab) && lab.ListGuidance == 0 && lab.ListNext - state.Tick > c.Listing.QuarterWeeks - c.Listing.GuidanceWindow;
        }

        static void Dilute(GameConfig c, GameState state, LabState lab, int permille)
        {
            lab.FounderSharePermille = (int)MathI.MulPermille(lab.FounderSharePermille, MathI.Permille - permille);
            lab.ListKeep = (int)MathI.MulPermille(lab.ListKeep, MathI.Permille - permille);
            StaffEquity.OnRaise(lab, permille);
            FundingSystem.TryOust(c, state, lab);
        }

        public static void List(GameConfig c, GameState state, LabState lab)
        {
            long gross = Proceeds(c, lab, c.Listing.FloatPermille);
            long fee = MathI.MulPermille(gross, c.Listing.ListingFeePermille);
            lab.Listed = 1;
            lab.ListPrice = MathI.Permille;
            lab.ListBase = Revenue(lab);
            lab.ListKeep = MathI.Permille;
            lab.ListNext = state.Tick + c.Listing.QuarterWeeks;
            lab.ListGuidance = 0;
            lab.ListQuarter = 0;
            lab.ListTarget = 0;
            lab.ListStreak = 0;
            lab.ListFollowOn = state.Tick;
            Finance.Earn(lab, LedgerLine.CapitalIn, gross - fee);
            Events.Add(state, EventKind.CompanyListed, lab.Id, -1, gross - fee, "");
            Dilute(c, state, lab, c.Listing.FloatPermille);
            Notes.OnListing(c, state, lab);
        }

        public static void SetGuidance(GameConfig c, GameState state, LabState lab, int level)
        {
            lab.ListGuidance = level;
            lab.ListTarget = MathI.MulPermille(lab.ListBase * c.Listing.QuarterWeeks, Mult(c, level));
        }

        public static void FollowOn(GameConfig c, GameState state, LabState lab)
        {
            long gross = Proceeds(c, lab, c.Listing.FollowOnPermille);
            lab.ListFollowOn = state.Tick;
            Finance.Earn(lab, LedgerLine.CapitalIn, gross);
            Events.Add(state, EventKind.FollowOnSold, lab.Id, -1, gross, "");
            Dilute(c, state, lab, c.Listing.FollowOnPermille);
        }

        public static void BuyBack(GameConfig c, GameState state, LabState lab, long amount)
        {
            long value = MathI.Max(1, Value(c, lab));
            long share = amount * MathI.Permille / value;
            if (share > MathI.Permille - 1) share = MathI.Permille - 1;
            Finance.Spend(lab, LedgerLine.CapitalIn, amount);
            lab.ListPrice = (int)MathI.Clamp(lab.ListPrice + MathI.MulPermille(lab.ListPrice, MathI.MulPermille(share, c.Listing.BuybackImpact)), c.Listing.PriceFloor, c.Listing.PriceCap);
            int newKeep = (int)MathI.Min(MathI.Permille, MathI.MulPermille(lab.ListKeep, MathI.Permille + (int)share));
            int gain = newKeep - lab.ListKeep;
            lab.FounderSharePermille = (int)MathI.Min(MathI.Permille, lab.FounderSharePermille * newKeep / lab.ListKeep);
            lab.ListKeep = newKeep;
            Events.Add(state, EventKind.SharesBoughtBack, lab.Id, gain, amount, "");
        }

        public static long TakePrivateCost(GameConfig c, LabState lab)
        {
            return MathI.MulPermille(MathI.MulPermille(Value(c, lab), MathI.Permille - lab.ListKeep), c.Listing.TakePrivatePermille);
        }

        public static void TakePrivate(GameConfig c, GameState state, LabState lab)
        {
            long cost = TakePrivateCost(c, lab);
            Finance.Spend(lab, LedgerLine.CapitalIn, cost);
            lab.FounderSharePermille = (int)MathI.Min(MathI.Permille, lab.FounderSharePermille * MathI.Permille / MathI.Max(1, lab.ListKeep));
            lab.Listed = 0;
            lab.ListPrice = 0;
            lab.ListGuidance = 0;
            lab.ListTarget = 0;
            lab.ListQuarter = 0;
            lab.ListNext = 0;
            lab.ListKeep = 0;
            lab.ListStreak = 0;
            lab.ListActivist = 0;
            Events.Add(state, EventKind.CompanyDelisted, lab.Id, -1, cost, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            lab.ListBase = lab.ListBase == 0 ? Revenue(lab) : (lab.ListBase * 11 + Revenue(lab)) / 12;
            if (!Listed(lab)) return;
            Finance.Spend(lab, LedgerLine.Insurance, c.Listing.ReportingCost);
            lab.ListQuarter += Revenue(lab);
            if (state.Tick + 1 < lab.ListNext) return;
            int move;
            int pick;
            if (lab.ListGuidance == 0)
            {
                move = -c.Listing.NoGuidanceMove;
                pick = 0;
                lab.ListStreak = 0;
            }
            else
            {
                long ratio = lab.ListTarget <= 0 ? MathI.Permille : lab.ListQuarter * MathI.Permille / lab.ListTarget;
                if (ratio >= MathI.Permille + c.Listing.BeatMargin)
                {
                    move = c.Listing.BeatMove * lab.ListGuidance;
                    pick = 2;
                    lab.ListStreak++;
                }
                else if (ratio >= MathI.Permille)
                {
                    move = c.Listing.MeetMove;
                    pick = 1;
                    lab.ListStreak++;
                }
                else
                {
                    move = -c.Listing.MissMove * lab.ListGuidance;
                    pick = 0;
                    lab.ListStreak = 0;
                }
            }
            lab.ListPrice = (int)MathI.Clamp(lab.ListPrice + MathI.MulPermille(lab.ListPrice, move), c.Listing.PriceFloor, c.Listing.PriceCap);
            lab.ListGuidance = 0;
            lab.ListTarget = 0;
            lab.ListQuarter = 0;
            lab.ListNext = state.Tick + 1 + c.Listing.QuarterWeeks;
            Events.Add(state, EventKind.EarningsReported, lab.Id, -1, lab.ListPrice, "").Pick = pick;
            if (lab.ListPrice < c.Listing.ActivistBelow && lab.ListActivist < c.Listing.MaxActivist)
            {
                lab.ListActivist++;
                lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Listing.ActivistReputation));
                Events.Add(state, EventKind.ActivistArrived, lab.Id, -1, lab.ListActivist, "");
            }
            else if (lab.ListPrice >= c.Listing.ActivistBelow && lab.ListActivist > 0 && pick > 0) lab.ListActivist--;
        }
    }
}
