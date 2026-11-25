using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

using System.Collections.Generic;

namespace AiLabs.Rules.Systems
{
    /// <summary>GPU failures, warranties and spares. See <see cref="ReliabilityConfig"/>. Only the player's lab.</summary>
    public static class HardwareReliability
    {
        public const long Million = 1000000L;

        public static bool Enabled(GameConfig c) { return c.Reliability.Enabled > 0; }

        public static long FeePerUnit(GameConfig c, int tier) { return tier == 2 ? c.Reliability.FeeTier2 : tier == 1 ? c.Reliability.FeeTier1 : 0; }

        public static int RmaWeeks(GameConfig c, int tier) { return tier == 2 ? c.Reliability.RmaWeeksTier2 : c.Reliability.RmaWeeksTier1; }

        /// <summary>The GPUs in the fleet that can fail: units, not chips the lab designed.</summary>
        public static int Units(GameConfig c, LabState lab)
        {
            int units = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
                if (lab.Fleet[i].ChipDesignId < 0 && c.Compute.Generation(lab.Fleet[i].GpuGenId) != null) units += lab.Fleet[i].Count;
            return units;
        }

        /// <summary>The chance, in millionths a week, that one unit of the batch fails.</summary>
        public static long RatePpm(GameConfig c, GameState state, LabState lab, HardwareBatch batch)
        {
            GpuGenerationConfig gen = c.Compute.Generation(batch.GpuGenId);
            if (gen == null) return 0;
            long years = MathI.Max(0, GameDate.Year(c.Calendar, state.Tick) - gen.OpensYear);
            long rate = c.Reliability.BaseFailPpm + c.Reliability.AgePpm * years;
            if (batch.HoldingId != HardwareBatch.Hq)
            {
                Holding holding = lab.Holding(batch.HoldingId);
                SiteConfig site = holding == null ? null : c.World.Site(holding.SiteId);
                if (site != null && WorldQueries.InHeatwave(state, site.Id)) rate = MathI.MulPermille(rate, c.Reliability.HeatwavePermille);
            }
            return rate;
        }

        /// <summary>What the fleet is expected to lose this week, in millionths of a unit.</summary>
        public static long ExpectedPpm(GameConfig c, GameState state, LabState lab)
        {
            long total = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.ChipDesignId < 0) total += b.Count * RatePpm(c, state, lab, b);
            }
            return total;
        }

        public static long SparePrice(GameConfig c, GameState state) { return LabQueries.CurrentGeneration(c, state.Tick).BuyPrice; }

        public static long ExpeditePrice(GameConfig c, GameState state, LabState lab)
        {
            return lab.RmaUnits * MathI.MulPermille(SparePrice(c, state), c.Reliability.ExpeditePermille);
        }

        public static void SetWarranty(GameConfig c, GameState state, LabState lab, int tier)
        {
            if (tier > lab.WarrantyTier) Finance.Spend(lab, LedgerLine.Insurance, c.Reliability.SwitchFee);
            lab.WarrantyTier = tier;
            Events.Add(state, EventKind.WarrantyChanged, lab.Id, -1, tier, "");
        }

        public static void Stock(GameConfig c, GameState state, LabState lab, int count)
        {
            Finance.Spend(lab, LedgerLine.Hardware, SparePrice(c, state) * count);
            lab.Spares += count;
            Events.Add(state, EventKind.SparesStocked, lab.Id, lab.Spares, count, "");
        }

        static HardwareBatch Oldest(GameConfig c, LabState lab)
        {
            HardwareBatch pick = null;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.ChipDesignId >= 0 || c.Compute.Generation(b.GpuGenId) == null) continue;
                if (pick == null) { pick = b; continue; }
                int byTier = Fleet.TierOf(c, b).CompareTo(Fleet.TierOf(c, pick));
                if (byTier < 0 || (byTier == 0 && b.Id < pick.Id)) pick = b;
            }
            return pick;
        }

        public static void Return(GameConfig c, GameState state, LabState lab, bool rushed)
        {
            int units = lab.RmaUnits;
            lab.RmaUnits = 0;
            lab.RmaDue = 0;
            Fleet.AddGpus(state, lab, HardwareBatch.Hq, LabQueries.CurrentGeneration(c, state.Tick).Id, units);
            Events.Add(state, EventKind.ReplacementsArrived, lab.Id, -1, units, "").Pick = rushed ? 1 : 0;
        }

        public static void Expedite(GameConfig c, GameState state, LabState lab)
        {
            Finance.Spend(lab, LedgerLine.Hardware, ExpeditePrice(c, state, lab));
            Return(c, state, lab, true);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            int units = Units(c, lab);
            long fee = FeePerUnit(c, lab.WarrantyTier) * units + c.Reliability.SpareUpkeep * lab.Spares;
            if (fee > 0) Finance.Spend(lab, LedgerLine.Insurance, fee);
            if (lab.RmaUnits > 0 && state.Tick + 1 >= lab.RmaDue) Return(c, state, lab, false);
            lab.FailCarry += ExpectedPpm(c, state, lab);
            int failed = (int)MathI.Min(lab.FailCarry / Million, units);
            lab.FailCarry %= Million;
            if (failed <= 0) return;
            int lost = 0, swapped = 0, sent = 0;
            GpuGenerationConfig now = LabQueries.CurrentGeneration(c, state.Tick);
            for (int n = 0; n < failed; n++)
            {
                HardwareBatch batch = Oldest(c, lab);
                if (batch == null) break;
                int holding = batch.HoldingId;
                Fleet.Take(lab, batch, 1);
                lab.GpuFailures++;
                if (lab.Spares > 0)
                {
                    lab.Spares--;
                    Fleet.AddGpus(state, lab, holding, now.Id, 1);
                    swapped++;
                }
                else if (lab.WarrantyTier > 0)
                {
                    if (lab.RmaUnits == 0) lab.RmaDue = state.Tick + RmaWeeks(c, lab.WarrantyTier);
                    lab.RmaUnits++;
                    sent++;
                }
                else lost++;
            }
            if (lost > 0) Events.Add(state, EventKind.GpusFailed, lab.Id, -1, lost, "").Pick = 0;
            if (swapped > 0) Events.Add(state, EventKind.GpusFailed, lab.Id, -1, swapped, "").Pick = 1;
            if (sent > 0) Events.Add(state, EventKind.GpusFailed, lab.Id, -1, sent, "").Pick = 2;
        }
    }
}
