using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>How the company is controlled. See <see cref="GovernanceConfig"/>. Only the player's lab.</summary>
    public static class Governance
    {
        public const int Dual = 1;
        public const int Nonprofit = 2;

        public static bool Enabled(GameConfig c) { return c.Governance.Enabled > 0; }

        /// <summary>The voting control the founders hold, in permille, after dual-class shares or a non-profit parent.</summary>
        public static int ControlShare(GameConfig c, LabState lab)
        {
            if (!Enabled(c)) return lab.FounderSharePermille;
            if (lab.GovForm == Nonprofit) return MathI.Permille;
            if (lab.GovForm == Dual) return (int)MathI.Min(MathI.Permille, MathI.MulPermille(lab.FounderSharePermille, c.Governance.DualVoteMult));
            return lab.FounderSharePermille;
        }

        /// <summary>The cash a funding round brings after what the structure costs.</summary>
        public static long RaiseCash(GameConfig c, LabState lab, long cash)
        {
            if (!Enabled(c)) return cash;
            if (lab.GovForm == Dual) return MathI.MulPermille(cash, MathI.Permille - c.Governance.DualCashCut);
            if (lab.GovForm == Nonprofit) return MathI.MulPermille(cash, MathI.Permille - c.Governance.NonprofitCashCut);
            return cash;
        }

        /// <summary>The yearly chance of board drama, in permille, once the directors have calmed it.</summary>
        public static int DramaChance(GameConfig c, LabState lab, int chance)
        {
            if (!Enabled(c) || lab.GovDirectors <= 0) return chance;
            return (int)MathI.MulPermille(chance, MathI.Max(0, MathI.Permille - (long)lab.GovDirectors * c.Governance.DramaCut));
        }

        public static long WeeklyCost(GameConfig c, LabState lab) { return (long)lab.GovDirectors * c.Governance.DirectorWeekly; }

        public static void Set(GameConfig c, GameState state, LabState lab, int form)
        {
            lab.GovForm = form;
            lab.GovSince = state.Tick + 1;
            Events.Add(state, EventKind.GovernanceChanged, lab.Id, form, 0, "");
        }

        public static void StartSearch(GameConfig c, LabState lab)
        {
            lab.GovSearch = c.Governance.DirectorWeeks;
            if (c.Governance.DirectorFee > 0) Finance.Spend(lab, LedgerLine.Hiring, c.Governance.DirectorFee);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.GovDirectors > 0)
            {
                long cost = WeeklyCost(c, lab);
                if (lab.Cash < cost)
                {
                    lab.GovDirectors--;
                    lab.GovResigned++;
                    Events.Add(state, EventKind.DirectorResigned, lab.Id, lab.GovDirectors, 0, "");
                }
                else if (cost > 0) Finance.Spend(lab, LedgerLine.Salaries, cost);
            }
            if (lab.GovSearch > 0)
            {
                lab.GovSearch--;
                if (lab.GovSearch == 0)
                {
                    lab.GovDirectors++;
                    Events.Add(state, EventKind.DirectorSeated, lab.Id, lab.GovDirectors, 0, "");
                }
            }
        }
    }
}
