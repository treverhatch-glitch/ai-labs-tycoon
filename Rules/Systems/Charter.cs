using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab's charter. See <see cref="CharterConfig"/>. Only the player's lab has anything but the for-profit form.</summary>
    public static class Charter
    {
        public const int ForProfit = 0;
        public const int Capped = 1;
        public const int Nonprofit = 2;

        public static bool Enabled(GameConfig c) { return c.Charter.Enabled > 0; }

        public static bool UnderReview(LabState lab) { return lab.CharterTo > 0; }

        public static string Describe(int form) { return form == Nonprofit ? "a nonprofit" : form == Capped ? "a capped-profit company" : "a for-profit company"; }

        public static bool RefusesFunding(GameConfig c, LabState lab) { return Enabled(c) && lab.Charter == Nonprofit; }

        /// <summary>What a round raises under the lab's charter.</summary>
        public static long FundingCash(GameConfig c, LabState lab, long cash)
        {
            return Enabled(c) && lab.Charter == Capped ? MathI.MulPermille(cash, c.Charter.CappedFundingPermille) : cash;
        }

        public static int GrantPermille(GameConfig c, LabState lab)
        {
            if (!Enabled(c)) return MathI.Permille;
            return lab.Charter == Nonprofit ? c.Charter.NonprofitGrantPermille : lab.Charter == Capped ? c.Charter.CappedGrantPermille : c.Charter.ForProfitGrantPermille;
        }

        public static long GrantAward(GameConfig c, LabState lab, long award) { return MathI.MulPermille(award, GrantPermille(c, lab)); }

        public static long Donations(GameConfig c, LabState lab) { return lab.Charter == Nonprofit ? c.Charter.DonationPerPrestige * (long)lab.Prestige : 0; }

        public static long Distribution(GameConfig c, LabState lab)
        {
            if (lab.Charter != Capped || lab.CapPaid >= c.Charter.CapAmount) return 0;
            long net = lab.Ledger.LastWeek.Net;
            if (net <= 0) return 0;
            return MathI.Min(c.Charter.CapAmount - lab.CapPaid, MathI.MulPermille(net, c.Charter.DistributionPermille));
        }

        public static void Convert(GameConfig c, GameState state, LabState lab, int form)
        {
            Finance.Spend(lab, LedgerLine.Fines, c.Charter.ConvertFee);
            lab.CharterTo = form + 1;
            lab.CharterDue = state.Tick + c.Charter.ReviewWeeks;
            Events.Add(state, EventKind.CharterReviewStarted, lab.Id, -1, c.Charter.ConvertFee, "").Pick = form;
        }

        public static void Cancel(GameConfig c, GameState state, LabState lab)
        {
            int form = lab.CharterTo - 1;
            lab.CharterTo = 0;
            lab.CharterDue = 0;
            Events.Add(state, EventKind.CharterReviewDropped, lab.Id, -1, 0, "").Pick = form;
        }

        static void Mood(LabState lab, int delta)
        {
            for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Clamp((long)lab.Staff[i].Morale + delta, 0, MathI.Permille);
        }

        static void Finish(GameConfig c, GameState state, LabState lab)
        {
            int from = lab.Charter;
            int to = lab.CharterTo - 1;
            lab.Charter = to;
            lab.CharterTo = 0;
            lab.CharterDue = 0;
            if (to == Capped) lab.CapPaid = 0;
            if (from == Nonprofit && to != Nonprofit)
            {
                Mood(lab, -c.Charter.ProfitMorale);
                Litigation.File(c, state, lab, Litigation.Charter, c.Charter.ChallengeClaim, c.Charter.ChallengeStrength, "");
            }
            else if (to == Nonprofit) Mood(lab, c.Charter.MissionMorale);
            Events.Add(state, EventKind.CharterChanged, lab.Id, -1, from, "").Pick = to;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            long gift = Donations(c, lab);
            if (gift > 0) Finance.Earn(lab, LedgerLine.Grants, gift);
            long share = Distribution(c, lab);
            if (share > 0)
            {
                Finance.Spend(lab, LedgerLine.CapitalIn, share);
                lab.CapPaid += share;
            }
            if (UnderReview(lab) && state.Tick + 1 >= lab.CharterDue) Finish(c, state, lab);
        }
    }
}
