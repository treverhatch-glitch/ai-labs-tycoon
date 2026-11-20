using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// What governments make of the labs. Every month each government's attention to each lab moves with the lab's
    /// footprint in its country, and the steps of scrutiny it reaches have consequences.
    /// </summary>
    public static class GovernmentSystem
    {
        /// <summary>
        /// Closes the month for one lab. It reads this month's revenue and incidents, so it runs for every lab before
        /// the ledgers roll over.
        /// </summary>
        public static void CloseMonth(GameConfig c, GameState state, LabState lab)
        {
            AttentionConfig a = c.World.Attention;
            int sharePpm = RevenueSharePpm(state, lab);

            for (int b = 0; b < c.World.Blocs.Count; b++)
            {
                string blocId = c.World.Blocs[b].Id;
                BlocStanding standing = lab.Bloc(blocId);
                AttentionGain gain = WorldQueries.GainFor(c, state, lab, blocId, sharePpm);

                long attention = standing.Attention + gain.Total - MathI.MulPermille(standing.Attention, a.DecayPermillePerMonth);
                standing.Attention = (int)MathI.Clamp(attention, 0, MathI.Permille);

                int before = standing.Level;
                while (standing.Level < BlocStanding.Freeze && standing.Attention >= a.EntryPoint(standing.Level + 1))
                {
                    standing.Level++;
                    Reached(c, state, lab, standing);
                }
                if (standing.Level == before)
                {
                    while (standing.Level > 0 && standing.Attention < DropBelow(a, standing.Level))
                    {
                        Events.Add(state, EventKind.AttentionFell, lab.Id, standing.Level - 1, 0, blocId);
                        standing.Level--;
                    }
                }
            }
            lab.IncidentsThisMonth = 0;
        }

        /// <summary>The lab's share of every lab's revenue this month, in parts per million.</summary>
        public static int RevenueSharePpm(GameState state, LabState lab)
        {
            long all = 0;
            for (int i = 0; i < state.Labs.Count; i++) all += state.Labs[i].Ledger.ThisMonth.Revenue;
            return all <= 0 ? 0 : (int)(lab.Ledger.ThisMonth.Revenue * MathI.Ppm / all);
        }

        static int DropBelow(AttentionConfig a, int level)
        {
            return level == BlocStanding.Freeze ? a.FreezeExitBelow : a.EntryPoint(level) - a.DropMargin;
        }

        static void Reached(GameConfig c, GameState state, LabState lab, BlocStanding standing)
        {
            AttentionConfig a = c.World.Attention;
            long amount = 0;
            if (standing.Level == BlocStanding.Hearing)
            {
                amount = WorldQueries.PolicyStaff(c, lab) >= a.PreparedPolicyStaff ? a.HearingReputationPrepared : a.HearingReputation;
                lab.Reputation = (int)MathI.Clamp(lab.Reputation + amount, 0, MathI.Permille);
            }
            else if (standing.Level == BlocStanding.Audit)
            {
                amount = lab.LastWeek.Revenue * a.AuditFineRevenueWeeks;
                Finance.Spend(lab, LedgerLine.Fines, amount);
            }
            Events.Add(state, EventKind.AttentionRose, lab.Id, standing.Level, amount, standing.BlocId);
        }
    }
}
