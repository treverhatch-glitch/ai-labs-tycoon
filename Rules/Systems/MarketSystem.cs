using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Sectors, shares and revenue. Each lab has at most one model on sale per sector. Customers drift
    /// toward the lab they would pick today, a little every week, so a better model takes time to win a market.
    /// </summary>
    public static class MarketSystem
    {
        /// <summary>How strongly a lab's sales force pulls customers, in permille (1000 = no sales staff).</summary>
        public static int SalesPermille(GameConfig c, LabState lab)
        {
            long bonus = (long)LabQueries.StaffByFunction(c, lab, RoleFunction.Sales) * c.Market.SalesBonusPermille;
            return MathI.Permille + (int)MathI.Min(bonus, c.Market.SalesBonusCapPermille);
        }

        /// <summary>
        /// The pull of one entry on a sector's customers. Quality counts twice over (squared): buyers
        /// flock to the best model. Reputation, sales staff and last year's bracket win scale it.
        /// </summary>
        public static long Appeal(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            return DisplayQueries.Appeal(c, state, sector, sectorState, lab, model);
        }

        /// <summary>How much of a sector has a use for AI at all: the best model on sale against the sector's saturation bar.</summary>
        public static int Adoption(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState)
        {
            long best = 0;
            for (int i = 0; i < sectorState.Entries.Count; i++)
            {
                SectorEntry entry = sectorState.Entries[i];
                LabState entryLab = state.Lab(entry.LabId);
                ModelState model = entryLab.Model(entry.ModelId);
                best = MathI.Max(best, LabQueries.Quality(c, sector, model, state.Tick, entryLab));
            }
            return (int)MathI.Min(MathI.Permille, best * MathI.Permille / sector.SaturationCapability);
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            var demand = new long[state.Labs.Count];
            var revenue = new long[state.Labs.Count];

            // Pass 1: shares move, and each entry's LastRevenue holds what it could earn with enough compute.
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sectorState = state.Sectors[s];
                SectorConfig sector = c.Sector(sectorState.Id);
                if (sectorState.Entries.Count == 0) { sectorState.AdoptionPermille = 0; continue; }

                sectorState.AdoptionPermille = Adoption(c, state, sector, sectorState);

                long totalAppeal = 0;
                var appeals = new long[sectorState.Entries.Count];
                for (int i = 0; i < appeals.Length; i++)
                {
                    SectorEntry entry = sectorState.Entries[i];
                    LabState lab = state.Lab(entry.LabId);
                    appeals[i] = Appeal(c, state, sector, sectorState, lab, lab.Model(entry.ModelId));
                    totalAppeal += appeals[i];
                }

                long served = MathI.MulPermille(sectorState.PoolPerWeek, sectorState.AdoptionPermille);
                for (int i = 0; i < appeals.Length; i++)
                {
                    SectorEntry entry = sectorState.Entries[i];
                    long target = totalAppeal == 0 ? 0 : ShareOf(appeals[i], totalAppeal);
                    // Rounded down, so the shares of a sector can never add up to more than everyone.
                    entry.SharePpm += (int)MathI.FloorDiv((target - entry.SharePpm) * c.Market.SwitchRatePermille, MathI.Permille);

                    long potential = served * entry.SharePpm / MathI.Ppm;
                    int labIndex = IndexOfLab(state, entry.LabId);
                    LabState entryLab = state.Labs[labIndex];
                    ModelState entryModel = entryLab.Model(entry.ModelId);
                    long entryRevenue = MathI.MulPermille(potential, entry.PricePermille);
                    if (entryModel.OpenWeights)
                        entryRevenue = MathI.MulPermille(entryRevenue, c.Market.OpenWeightsRevenuePermille);
                    int workshopRev = DeviceWorkshopSystem.RevenueBonusPermille(c, state, entryLab, sector.Id);
                    if (workshopRev > 0)
                        entryRevenue = MathI.MulPermille(entryRevenue, MathI.Permille + workshopRev);
                    entry.LastRevenue = entryRevenue;

                    ModelLineConfig line = c.Line(entryModel.LineId);
                    demand[labIndex] += ServingDemand(sector, line, entryRevenue);
                }
            }

            // Pass 2: a lab short on serving compute earns only the part of its demand it can serve.
            var servedPermille = new int[state.Labs.Count];
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                long serving = lab.LastWeek.ServingCompute;
                servedPermille[i] = demand[i] <= serving ? MathI.Permille : (int)(serving * MathI.Permille / demand[i]);
                lab.LastWeek.ServingDemand = demand[i];
                lab.LastWeek.ServedPermille = servedPermille[i];
            }

            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sectorState = state.Sectors[s];
                for (int i = 0; i < sectorState.Entries.Count; i++)
                {
                    SectorEntry entry = sectorState.Entries[i];
                    int labIndex = IndexOfLab(state, entry.LabId);
                    entry.LastRevenue = MathI.MulPermille(entry.LastRevenue, servedPermille[labIndex]);
                    revenue[labIndex] += entry.LastRevenue;
                }
            }

            for (int i = 0; i < state.Labs.Count; i++)
            {
                state.Labs[i].LastWeek.Revenue = revenue[i];
                Finance.Earn(state.Labs[i], LedgerLine.Revenue, revenue[i]);
            }
        }

        /// <summary>Compute units a week of this revenue needs. Bigger lines cost more to serve per dollar.</summary>
        public static long ServingDemand(SectorConfig sector, ModelLineConfig line, long revenue)
        {
            return revenue * line.ServingCostPermille / (sector.RevenuePerComputeUnit * MathI.Permille);
        }

        /// <summary>What a model earned last week across every sector it is on sale in.</summary>
        public static long ModelRevenue(GameState state, int labId, int modelId)
        {
            long total = 0;
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorEntry entry = state.Sectors[s].Entry(labId);
                if (entry != null && entry.ModelId == modelId) total += entry.LastRevenue;
            }
            return total;
        }

        /// <summary>A sector's pool is what it pays in its opening year, and grows from then on. Sectors not yet open wait at that size.</summary>
        public static void GrowYear(GameConfig c, GameState state)
        {
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorConfig sector = c.Sector(state.Sectors[s].Id);
                if (!LabQueries.SectorOpen(c, sector, state.Tick)) continue;
                state.Sectors[s].PoolPerWeek = MathI.MulPermille(state.Sectors[s].PoolPerWeek, MathI.Permille + EraSystem.SectorGrowthPermille(sector, GameDate.Year(c.Calendar, state.Tick)));
            }
        }

        static int IndexOfLab(GameState state, int labId)
        {
            for (int i = 0; i < state.Labs.Count; i++) if (state.Labs[i].Id == labId) return i;
            return -1;
        }

        /// <summary>A part of a total in parts per million, safe for totals too large to multiply by a million.</summary>
        static long ShareOf(long part, long total)
        {
            while (total > long.MaxValue / MathI.Ppm) { part >>= 1; total >>= 1; }
            return part * MathI.Ppm / total;
        }
    }
}
