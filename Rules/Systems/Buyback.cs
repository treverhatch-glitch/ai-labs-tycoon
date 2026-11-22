using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Buying a funding round back. See <see cref="BuybackConfig"/>. Only the player's lab.</summary>
    public static class Buyback
    {
        public static bool Enabled(GameConfig c) { return c.Buyback.Enabled > 0; }

        public static bool Bought(LabState lab, string roundId) { return lab.BoughtRounds.Contains(roundId); }

        public static long Price(GameConfig c, GameState state, FundingRoundConfig round)
        {
            long mult = c.Buyback.PriceMult + (long)state.BubblePermille * c.Buyback.BubblePrice / MathI.Permille;
            return MathI.MulPermille(round.Cash, mult);
        }

        public static void Do(GameConfig c, GameState state, LabState lab, FundingRoundConfig round)
        {
            long price = Price(c, state, round);
            Finance.Spend(lab, LedgerLine.CapitalIn, price);
            long back = MathI.Permille - round.DilutionPermille;
            if (back > 0) lab.FounderSharePermille = (int)MathI.Min(MathI.Permille, (long)lab.FounderSharePermille * MathI.Permille / back);
            lab.BoardInvestorSeats = (int)MathI.Max(0, lab.BoardInvestorSeats - round.BoardSeats);
            lab.BoughtRounds.Add(round.Id);
            if (c.Buyback.BuybackTrust > 0) lab.Reputation = MathI.Clamp(lab.Reputation + (int)(c.Buyback.BuybackTrust / 1000), 0, MathI.Permille);
            Events.Add(state, EventKind.RoundBoughtBack, lab.Id, -1, price, round.Id);
        }
    }
}
