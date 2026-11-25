using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The treasury reserve. See <see cref="TreasuryConfig"/>. Money in the reserve is out of the lab's cash and on the ledger's capital line, so the books still balance, and it comes
    /// back the same way. Only the player's lab keeps one.
    /// </summary>
    public static class Treasury
    {
        public static bool Enabled(GameConfig c) { return c.Treasury.Enabled > 0; }

        /// <summary>What a planned draw of this much would leave in the lab's hands.</summary>
        public static long NetOfDrawFee(GameConfig c, long gross) { return gross - MathI.MulPermille(gross, c.Treasury.DrawFeePermille); }

        public static long WeeklyYield(GameConfig c, LabState lab) { return lab.Reserve * c.Treasury.WeeklyYieldPpm / 1_000_000; }

        public static void SetAside(GameState state, LabState lab, long amount)
        {
            Finance.Spend(lab, LedgerLine.CapitalIn, amount);
            lab.Reserve += amount;
            GameEvent e = Events.Add(state, EventKind.ReserveMoved, lab.Id, -1, amount, "");
            e.Pick = 1;
        }

        public static void Draw(GameConfig c, GameState state, LabState lab, long gross)
        {
            lab.Reserve -= gross;
            Finance.Earn(lab, LedgerLine.CapitalIn, NetOfDrawFee(c, gross));
            GameEvent e = Events.Add(state, EventKind.ReserveMoved, lab.Id, -1, gross, "");
            e.Pick = 0;
        }

        /// <summary>The yield comes in, then a lab in debt is covered from the reserve as far as it goes, at the dearer fee.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Reserve <= 0) return;
            long yield = WeeklyYield(c, lab);
            if (yield > 0) Finance.Earn(lab, LedgerLine.CapitalIn, yield);
            if (lab.Cash >= 0) return;
            long need = -lab.Cash;
            long keep = MathI.Permille - c.Treasury.CoverFeePermille;
            long gross = MathI.Min(lab.Reserve, (need * MathI.Permille + keep - 1) / keep);
            long net = gross - MathI.MulPermille(gross, c.Treasury.CoverFeePermille);
            if (net <= 0) return;
            lab.Reserve -= gross;
            Finance.Earn(lab, LedgerLine.CapitalIn, net);
            Events.Add(state, EventKind.ReserveCovered, lab.Id, -1, gross, "");
        }
    }
}
