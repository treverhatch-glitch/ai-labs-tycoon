using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Down rounds, investor board drama, and post-venture investor events.</summary>
    public sealed class FundingFollowOnConfig : ISync
    {
        public int DownRoundBubblePermille;
        public int DownRoundCashPermille;
        public int DownRoundExtraDilutionPermille;
        public int BoardDramaMinInvestorSeats;
        public int BoardDramaPermillePerYear;
        public int BoardDramaWeeks;
        public int InvestorEventMinRounds;
        public int InvestorEventPermillePerMonth;
        public int InvestorEventReputation;
        public int InvestorEventBubblePermille;

        public void Sync(Syncer s)
        {
            s.Int("downRoundBubblePermille", ref DownRoundBubblePermille);
            s.Int("downRoundCashPermille", ref DownRoundCashPermille);
            s.Int("downRoundExtraDilutionPermille", ref DownRoundExtraDilutionPermille);
            s.Int("boardDramaMinInvestorSeats", ref BoardDramaMinInvestorSeats);
            s.Int("boardDramaPermillePerYear", ref BoardDramaPermillePerYear);
            s.Int("boardDramaWeeks", ref BoardDramaWeeks);
            s.Int("investorEventMinRounds", ref InvestorEventMinRounds);
            s.Int("investorEventPermillePerMonth", ref InvestorEventPermillePerMonth);
            s.Int("investorEventReputation", ref InvestorEventReputation);
            s.Int("investorEventBubblePermille", ref InvestorEventBubblePermille);
        }
    }
}