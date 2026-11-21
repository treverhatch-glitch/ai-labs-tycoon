using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class EndingSystem
    {
        public const string BankruptId = "bankrupt";
        public const string HandcuffsId = "handcuffs";
        public const string ExitId = "exit";
        public const string MonopolyId = "monopoly";
        public const string OustedId = "ousted";

        public static void RecordBankrupt(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return;
            if (HasOpen(state, BankruptId)) return;
            Add(state, lab.Id, BankruptId);
            Events.Add(state, EventKind.EndingReached, lab.Id, -1, 0, BankruptId);
        }

        public static void RecordOusted(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return;
            if (HasOpen(state, OustedId)) return;
            string successor = c.Funding.OustedSuccessor;
            if (!string.IsNullOrEmpty(successor)) lab.Leader = successor;
            EndingRecord record = Add(state, lab.Id, OustedId);
            record.Choice = "done";
            record.ChoiceTick = state.Tick;
            Events.Add(state, EventKind.EndingReached, lab.Id, -1, 0, OustedId);
        }

        public static void RecordHandcuffs(GameConfig c, GameState state, LabState lab)
        {
            EndingsConfig e = c.Endings;
            long fine = MathI.MulPermille(lab.Cash, e.HandcuffsFinePermille);
            if (fine > 0) Finance.Spend(lab, LedgerLine.Fines, fine);
            lab.Stash = 0;
            lab.Heat = 0;
            lab.ShadyLockedUntilTick = state.Tick + e.HandcuffsShadyLockWeeks;
            lab.Leader = e.HandcuffsSuccessor;
            BlocStanding home = lab.Bloc(lab.HomeBlocId);
            if (home != null) home.Attention = (int)MathI.Min(MathI.Permille, home.Attention + 200);
            EndingRecord record = Add(state, lab.Id, HandcuffsId);
            record.Choice = "done";
            record.ChoiceTick = state.Tick;
            Events.Add(state, EventKind.EndingReached, lab.Id, -1, 0, HandcuffsId);
        }

        public static void RecordExit(GameConfig c, GameState state, int labId, string mode)
        {
            Add(state, labId, ExitId);
            Events.Add(state, EventKind.EndingReached, labId, -1, 0, ExitId + ":" + mode);
        }

        public static void TryMonopoly(GameConfig c, GameState state)
        {
            if (state.PlayerLabId < 0) return;
            LabState player = state.Player;
            if (player == null || player.Bankrupt) return;
            EndingsConfig cfg = c.Endings;
            int year = GameDate.Year(c.Calendar, state.Tick);
            if (RecentEnding(c, state, MonopolyId, year, cfg.MonopolyCooldownYears)) return;
            if (!CrownStreak(state, player.Id, cfg.MonopolyCrownYears)) return;
            if (GovernmentSystem.RevenueShareLastClosedMonth(state, player) < cfg.MonopolyRevenuePpm) return;
            if (ComputeSharePpm(state, player) < cfg.MonopolyComputePpm) return;

            BlocStanding home = player.Bloc(player.HomeBlocId);
            if (home != null) home.Attention = (int)MathI.Min(MathI.Permille, home.Attention + cfg.MonopolyAttention);
            for (int h = 0; h < player.Holdings.Count; h++)
            {
                string bloc = WorldQueries.BlocOfHolding(c, player.Holdings[h]);
                BlocStanding s = player.Bloc(bloc);
                if (s != null) s.Attention = (int)MathI.Min(MathI.Permille, s.Attention + cfg.MonopolyAttention);
            }
            Add(state, player.Id, MonopolyId);
            Events.Add(state, EventKind.EndingReached, player.Id, -1, 0, MonopolyId);
        }

        static bool CrownStreak(GameState state, int labId, int years)
        {
            if (state.Gauntlets.Count < years) return false;
            for (int i = state.Gauntlets.Count - years; i < state.Gauntlets.Count; i++)
                if (state.Gauntlets[i].CrownLabId != labId) return false;
            return true;
        }

        public static int ComputeSharePpm(GameState state, LabState lab)
        {
            long all = 0;
            for (int i = 0; i < state.Labs.Count; i++) all += state.Labs[i].LastWeek.Compute;
            return all <= 0 ? 0 : (int)(lab.LastWeek.Compute * MathI.Ppm / all);
        }

        static bool RecentEnding(GameConfig c, GameState state, string endingId, int year, int years)
        {
            for (int i = state.Endings.Count - 1; i >= 0; i--)
            {
                EndingRecord r = state.Endings[i];
                if (r.EndingId != endingId) continue;
                int y = GameDate.Year(c.Calendar, r.Tick);
                if (year - y < years) return true;
            }
            return false;
        }

        static bool HasOpen(GameState state, string endingId)
        {
            for (int i = 0; i < state.Endings.Count; i++)
                if (state.Endings[i].EndingId == endingId && state.Endings[i].Choice == "") return true;
            return false;
        }

        static EndingRecord Add(GameState state, int labId, string endingId)
        {
            var r = new EndingRecord();
            r.Tick = state.Tick;
            r.EndingId = endingId;
            r.LabId = labId;
            r.Choice = "";
            r.ChoiceTick = -1;
            state.Endings.Add(r);
            return r;
        }

        public static long RevenueLast12Months(LabState lab)
        {
            long total = lab.Ledger.ThisMonth.Revenue;
            int take = 11;
            for (int i = lab.Ledger.History.Count - 1; i >= 0 && take > 0; i--)
            {
                total += lab.Ledger.History[i].Totals.Revenue;
                take--;
            }
            return total;
        }

        public static long SellPrice(GameConfig c, GameState state, LabState lab)
        {
            long rev = RevenueLast12Months(lab) * c.Endings.ExitRevenueYears;
            long fleet = MathI.MulPermille(LabQueries.FleetResaleValue(c, state, lab), c.Endings.ExitFleetPermille);
            return rev + fleet + lab.Cash;
        }

        public static bool AllOpenTechsDone(GameConfig c, LabState lab, int tick)
        {
            for (int i = 0; i < c.Research.Techs.Count; i++)
            {
                TechConfig tech = c.Research.Techs[i];
                if (!EraSystem.TechOpen(c, tech, tick)) continue;
                if (!LabQueries.HasTech(lab, tech.Id)) return false;
            }
            return true;
        }

        /// <summary>Next frontier purchase cost in parts: base plus growth from prior levels.</summary>
        public struct FrontierCostBreakdown
        {
            public long Base;
            public long FromGrowth;
            public int Level;
            public bool Capped;

            public long Total { get { return Capped ? -1 : Base + FromGrowth; } }
        }

        public static FrontierCostBreakdown FrontierCostParts(GameConfig c, LabState lab)
        {
            FrontierConfig f = c.Endings.Frontier;
            var b = new FrontierCostBreakdown();
            b.Level = lab.FrontierLevel;
            if (lab.FrontierLevel >= f.MaxLevel)
            {
                b.Capped = true;
                return b;
            }
            b.Base = f.BaseCost;
            long cost = f.BaseCost;
            for (int i = 0; i < lab.FrontierLevel; i++) cost = MathI.MulPermille(cost, f.GrowthPermille);
            b.FromGrowth = cost - b.Base;
            return b;
        }

        public static long FrontierCost(GameConfig c, LabState lab)
        {
            FrontierCostBreakdown parts = FrontierCostParts(c, lab);
            return parts.Total;
        }

        public static void ApplyBailout(GameConfig c, GameState state, LabState lab)
        {
            lab.Bankrupt = false;
            lab.WeeksInDebt = 0;
            Finance.Earn(lab, LedgerLine.CapitalIn, c.Endings.BailoutCash);
            lab.Reputation = c.Endings.BailoutReputation;
            if (lab.GrantEndsTick >= 0 && lab.GrantEndsTick <= state.Tick) { /* grant stays ended */ }
        }
    }
}