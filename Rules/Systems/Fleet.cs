using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Changes to a lab's fleet: hardware arriving, merging into batches, and leaving oldest generation first.</summary>
    public static class Fleet
    {
        /// <summary>Puts GPUs of one generation at a place. They join the batch already there, so the list stays short.</summary>
        public static HardwareBatch AddGpus(GameState state, LabState lab, int holdingId, string gpuGenId, int count)
        {
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.HoldingId == holdingId && b.GpuGenId == gpuGenId && b.ChipDesignId < 0)
                {
                    b.Count += count;
                    return b;
                }
            }
            var batch = new HardwareBatch();
            batch.Id = state.TakeId();
            batch.HoldingId = holdingId;
            batch.GpuGenId = gpuGenId;
            batch.ChipDesignId = -1;
            batch.DesignLabId = -1;
            batch.Count = count;
            batch.ArrivedTick = state.Tick;
            lab.Fleet.Add(batch);
            return batch;
        }

        /// <summary>The GPU batches at a place (chips are not sold with them), oldest generation first; the same generation by arrival, then by id.</summary>
        public static List<HardwareBatch> OldestFirst(GameConfig c, LabState lab, int holdingId)
        {
            var list = new List<HardwareBatch>();
            for (int i = 0; i < lab.Fleet.Count; i++) if (lab.Fleet[i].HoldingId == holdingId && lab.Fleet[i].ChipDesignId < 0) list.Add(lab.Fleet[i]);
            list.Sort((a, b) =>
            {
                int byTier = TierOf(c, a).CompareTo(TierOf(c, b));
                if (byTier != 0) return byTier;
                int byArrival = a.ArrivedTick.CompareTo(b.ArrivedTick);
                return byArrival != 0 ? byArrival : a.Id.CompareTo(b.Id);
            });
            return list;
        }

        /// <summary>The export tier of a batch's hardware.</summary>
        public static int TierOf(GameConfig c, HardwareBatch batch)
        {
            GpuGenerationConfig gen = c.Compute.Generation(batch.GpuGenId);
            return gen == null ? 0 : gen.Tier;
        }

        /// <summary>Takes units out of a batch and drops the batch when it is empty.</summary>
        public static void Take(LabState lab, HardwareBatch batch, int count)
        {
            batch.Count -= count;
            if (batch.Count <= 0) lab.Fleet.Remove(batch);
        }

        /// <summary>Oldest HQ GPU batch a planner would sell when <c>ReplaceTierGap</c> or more generations behind current.</summary>
        public static HardwareBatch StaleGpuBatch(GameConfig c, GameState state, LabState lab)
        {
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            HardwareBatch stale = null;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.GpuGenId == null || b.ChipDesignId >= 0 || gen.Tier - TierOf(c, b) < c.Rivalry.ReplaceTierGap) continue;
                if (stale == null || TierOf(c, b) < TierOf(c, stale)
                    || (TierOf(c, b) == TierOf(c, stale) && b.Id < stale.Id))
                    stale = b;
            }
            return stale;
        }
    }
}
