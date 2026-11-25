using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A cash advance repaid out of revenue. See <see cref="RbfConfig"/>. Only the player's lab.</summary>
    public static class RevenueFinancing
    {
        public static bool Enabled(GameConfig c) { return c.Rbf.Enabled > 0; }

        public static bool Owing(LabState lab) { return lab.RbfOwed > 0; }

        public static long Revenue(LabState lab) { return MathI.Max(0, lab.Ledger.LastWeek.Revenue); }

        public static long Largest(GameConfig c, LabState lab) { return Revenue(lab) * c.Rbf.AdvanceWeeks; }

        public static int Share(GameConfig c, int tier) { return tier == 1 ? c.Rbf.ShareA : tier == 2 ? c.Rbf.ShareB : c.Rbf.ShareC; }

        public static int Cap(GameConfig c, int tier) { return tier == 1 ? c.Rbf.CapA : tier == 2 ? c.Rbf.CapB : c.Rbf.CapC; }

        public static long Buyout(GameConfig c, LabState lab) { return MathI.MulPermille(lab.RbfOwed, c.Rbf.BuyoutPermille); }

        public static void Take(GameConfig c, GameState state, LabState lab, int tier, long advance)
        {
            lab.RbfAdvance = advance;
            lab.RbfOwed = MathI.MulPermille(advance, Cap(c, tier));
            lab.RbfBase = Revenue(lab);
            lab.RbfShare = Share(c, tier);
            lab.RbfTier = tier;
            lab.RbfWeeks = 0;
            lab.RbfSlump = 0;
            Finance.Earn(lab, LedgerLine.CapitalIn, advance);
            Events.Add(state, EventKind.RevenueAdvanceTaken, lab.Id, tier, advance, "");
        }

        static void Clear(GameState state, LabState lab, int pick, long paid)
        {
            lab.RbfAdvance = 0;
            lab.RbfOwed = 0;
            lab.RbfBase = 0;
            lab.RbfShare = 0;
            lab.RbfTier = 0;
            lab.RbfWeeks = 0;
            lab.RbfSlump = 0;
            Events.Add(state, EventKind.RevenueAdvanceCleared, lab.Id, -1, paid, "").Pick = pick;
        }

        public static void BuyOut(GameConfig c, GameState state, LabState lab)
        {
            long cost = Buyout(c, lab);
            Finance.Spend(lab, LedgerLine.CapitalIn, cost);
            Clear(state, lab, 1, cost);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Owing(lab)) return;
            lab.RbfWeeks++;
            long revenue = Revenue(lab);
            if (revenue < MathI.MulPermille(lab.RbfBase, c.Rbf.SlumpPermille))
            {
                long added = MathI.MulPermille(lab.RbfAdvance, c.Rbf.SlumpCap);
                lab.RbfOwed += added;
                lab.RbfSlump++;
                if (added > 0) Events.Add(state, EventKind.RevenueAdvanceStretched, lab.Id, -1, added, "");
            }
            long sweep = MathI.Min(lab.RbfOwed, MathI.MulPermille(revenue, lab.RbfShare));
            lab.RbfOwed -= sweep;
            Finance.Spend(lab, LedgerLine.CapitalIn, sweep);
            if (lab.RbfOwed <= 0) Clear(state, lab, 0, sweep);
        }
    }
}
