using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q180: research bridge loan balance is repaid weekly until cleared.</summary>
    public static class ResearchLoanSystem
    {
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt || lab.ResearchLoanBalance < 1) return;
            long pay = c.Research.LoanWeeklyPayment;
            if (pay < 1) return;
            if (pay > lab.ResearchLoanBalance) pay = lab.ResearchLoanBalance;
            if (lab.Cash < pay) return;
            Finance.Spend(lab, LedgerLine.Fines, pay);
            lab.ResearchLoanBalance -= pay;
        }
    }
}