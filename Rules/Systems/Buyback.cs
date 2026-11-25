using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Buying a funding round back, by a board vote. See <see cref="BuybackConfig"/>. Only the player's lab.</summary>
    public static class Buyback
    {
        public static bool Enabled(GameConfig c) { return c.Buyback.Enabled > 0; }

        public static bool Bought(LabState lab, string roundId) { return lab.BoughtRounds.Contains(roundId); }

        public static long Price(GameConfig c, GameState state, FundingRoundConfig round)
        {
            long mult = c.Buyback.PriceMult + (long)state.BubblePermille * c.Buyback.BubblePrice / MathI.Permille;
            return MathI.MulPermille(round.Cash, mult);
        }

        public static bool Held(LabState lab, string roundId) { return lab.FundingRounds.Contains(roundId) && !Bought(lab, roundId); }

        /// <summary>All the votes on the board: the founders, each round still held, and the independent directors.</summary>
        public static long TotalVotes(GameConfig c, LabState lab)
        {
            long total = Governance.ControlShare(c, lab) + (long)lab.GovDirectors * c.Buyback.DirectorVote;
            for (int i = 0; i < c.Funding.Rounds.Count; i++)
                if (Held(lab, c.Funding.Rounds[i].Id)) total += c.Funding.Rounds[i].DilutionPermille;
            return total;
        }

        /// <summary>The votes for buying this round back.</summary>
        public static long YesVotes(GameConfig c, GameState state, LabState lab, FundingRoundConfig target)
        {
            long price = Price(c, state, target);
            long yes = Governance.ControlShare(c, lab);
            bool drains = price * MathI.Permille > lab.Cash * c.Buyback.SpendCapPermille;
            for (int i = 0; i < c.Funding.Rounds.Count; i++)
            {
                FundingRoundConfig r = c.Funding.Rounds[i];
                if (r.Id != target.Id && Held(lab, r.Id) && !drains) yes += r.DilutionPermille;
            }
            bool runway = lab.Cash - price >= (long)c.Buyback.RunwayWeeks * LabQueries.WeeklyRunningCost(c, state, lab)
                && !(c.Buyback.WaveBlocksDirectors > 0 && lab.SpotOut > 0);
            if (runway) yes += (long)lab.GovDirectors * c.Buyback.DirectorVote;
            return yes;
        }

        public static bool Passes(GameConfig c, GameState state, LabState lab, FundingRoundConfig target)
        {
            return YesVotes(c, state, lab, target) * MathI.Permille > TotalVotes(c, lab) * c.Buyback.PassPermille;
        }

        public static bool Locked(GameConfig c, GameState state, LabState lab)
        {
            return lab.BoardVoteWeek > 0 && state.Tick + 1 < lab.BoardVoteWeek + c.Buyback.VoteCooldown;
        }

        public static void Vote(GameConfig c, GameState state, LabState lab, FundingRoundConfig round)
        {
            bool pass = Passes(c, state, lab, round);
            long share = YesVotes(c, state, lab, round) * MathI.Permille / MathI.Max(1, TotalVotes(c, lab));
            if (!pass)
            {
                lab.BoardVoteWeek = state.Tick + 1;
                lab.BoardLost++;
            }
            GameEvent e = Events.Add(state, EventKind.BoardVoted, lab.Id, -1, share, round.Id);
            e.Pick = pass ? 1 : 0;
            if (pass) Do(c, state, lab, round);
        }

        static void Do(GameConfig c, GameState state, LabState lab, FundingRoundConfig round)
        {
            long price = Price(c, state, round);
            Finance.Spend(lab, LedgerLine.CapitalIn, price);
            long back = MathI.Permille - round.DilutionPermille;
            if (back > 0) lab.FounderSharePermille = (int)MathI.Min(MathI.Permille, (long)lab.FounderSharePermille * MathI.Permille / back);
            lab.BoardInvestorSeats = (int)MathI.Max(0, lab.BoardInvestorSeats - round.BoardSeats);
            lab.BoughtRounds.Add(round.Id);
            lab.BoardWon++;
            if (c.Buyback.BuybackTrust > 0) lab.Reputation = MathI.Clamp(lab.Reputation + (int)(c.Buyback.BuybackTrust / 1000), 0, MathI.Permille);
            Events.Add(state, EventKind.RoundBoughtBack, lab.Id, -1, price, round.Id);
        }
    }
}
