using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>One holding's power bill for a week, in parts that add up to the total.</summary>
    public struct PowerBill
    {
        public int HoldingId;
        public string SiteId;
        public int InstalledKw;
        public int PrivateKw;
        public int GridKw;
        /// <summary>Grid kilowatts at the site's price.</summary>
        public long GridCost;
        /// <summary>The holding's own power at the deal's price.</summary>
        public long PrivateCost;
        /// <summary>What cooling adds to the energy, negative at a cool site.</summary>
        public long Cooling;
        /// <summary>The colo markup on every leased kilowatt.</summary>
        public long ColoMarkup;
        public long Total { get { return GridCost + PrivateCost + Cooling + ColoMarkup; } }
    }

    /// <summary>How a bloc's attention to a lab moves this month, in parts that add up to the gain.</summary>
    public struct AttentionGain
    {
        public string BlocId;
        /// <summary>Kilowatts the lab has installed in the bloc, each holding weighted by its power deal's attention.</summary>
        public long FootprintKw;
        public long NoticeKw;
        /// <summary>0 to 100: how much of the size a government of the year notices the footprint is.</summary>
        public int Size;
        public long FromFootprint;
        public long FromIncidents;
        public long FromDominance;
        /// <summary>What policy staff take off (zero or negative).</summary>
        public long PolicySaving;
        public long Total { get { return FromFootprint + FromIncidents + FromDominance + PolicySaving; } }
    }

    /// <summary>Read-only answers about sites, holdings, power and governments.</summary>
    public static class WorldQueries
    {
        public static int InstalledKw(GameConfig c, GameState state, LabState lab, int holdingId)
        {
            int kw = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.HoldingId != holdingId) continue;
                GpuGenerationConfig gen = c.Compute.Generation(b.GpuGenId);
                if (gen != null) kw += b.Count * gen.KwPerGpu;
                else if (b.ChipDesignId >= 0) kw += b.Count * ChipQueries.KwPerChip(c, state, b);
            }
            return kw;
        }

        /// <summary>Power the holding still has free for new hardware: built capacity less what is installed.</summary>
        public static int FreeKw(GameConfig c, GameState state, LabState lab, Holding holding)
        {
            return holding.HallKw - InstalledKw(c, state, lab, holding.Id) - ChipQueries.ReservedKw(lab, holding.Id);
        }

        /// <summary>What a holding takes from the grid: its hall, built and being built, less the private power that covers part of it.</summary>
        public static int GridDrawKw(Holding holding)
        {
            int draw = holding.HallKw + (holding.HallBuildingKw > 0 ? holding.HallBuildingKw : 0) - holding.PrivateKw;
            return draw > 0 ? draw : 0;
        }

        public static long SiteGridUsed(GameState state, string siteId)
        {
            long used = 0;
            for (int l = 0; l < state.Labs.Count; l++)
                for (int h = 0; h < state.Labs[l].Holdings.Count; h++)
                    if (state.Labs[l].Holdings[h].SiteId == siteId) used += GridDrawKw(state.Labs[l].Holdings[h]);
            return used;
        }

        public static int PlotsTaken(GameState state, string siteId)
        {
            int n = 0;
            for (int l = 0; l < state.Labs.Count; l++)
                for (int h = 0; h < state.Labs[l].Holdings.Count; h++)
                    if (state.Labs[l].Holdings[h].SiteId == siteId && state.Labs[l].Holdings[h].Kind == HoldingKind.Owned) n++;
            return n;
        }

        public static Holding HoldingAt(LabState lab, string siteId, HoldingKind kind)
        {
            for (int i = 0; i < lab.Holdings.Count; i++)
                if (lab.Holdings[i].SiteId == siteId && lab.Holdings[i].Kind == kind) return lab.Holdings[i];
            return null;
        }

        public static bool SiteOpen(GameConfig c, SiteConfig site, int tick)
        {
            return GameDate.Year(c.Calendar, tick) >= site.OpensYear;
        }

        public static bool PowerDealOpen(GameConfig c, PowerDealConfig deal, int tick)
        {
            return GameDate.Year(c.Calendar, tick) >= deal.OpensYear;
        }

        public static bool InHeatwave(GameState state, string siteId)
        {
            SiteState site = state.Site(siteId);
            return site != null && site.HeatwaveUntilTick >= 0 && state.Tick < site.HeatwaveUntilTick;
        }

        /// <summary>The cooling bonus of the lab's techs, 0 to 1000.</summary>
        public static int CoolingTech(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(LabQueries.TechBonus(c, lab, TechEffectKind.CoolingPermille), 0, MathI.Permille);
        }

        /// <summary>The compute a heatwave takes from this lab's hardware at the site, in permille. Cooling tech halves it.</summary>
        public static int HeatwaveThrottle(GameConfig c, LabState lab)
        {
            int throttle = c.World.Heatwave.ThrottlePermille;
            return CoolingTech(c, lab) >= c.World.Heatwave.HalvedByCoolingPermille ? throttle / 2 : throttle;
        }

        /// <summary>
        /// What cooling does to a site's energy bill for this lab. Cooling tech only shrinks the part above 1000: a site below
        /// 1000 keeps its bonus.
        /// </summary>
        public static int EffectiveCooling(GameConfig c, LabState lab, SiteConfig site)
        {
            if (site.CoolingPermille <= MathI.Permille) return site.CoolingPermille;
            return MathI.Permille + (int)MathI.MulPermille(site.CoolingPermille - MathI.Permille, MathI.Permille - CoolingTech(c, lab));
        }

        public static int InfraStaff(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Infrastructure); }

        public static int PolicyStaff(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Policy); }

        /// <summary>Policy staff for hearings and investigations (era may add virtual staff on merge).</summary>
        public static int EffectivePolicyStaff(GameConfig c, GameState state, LabState lab)
        {
            return PolicyStaff(c, lab);
        }

        /// <summary>Weeks a hall extension of this size takes this lab: the base and the size, then trimmed by infrastructure staff, never under the floor.</summary>
        public static int BuildWeeks(GameConfig c, LabState lab, int kw)
        {
            HallConfig h = c.World.Halls;
            long weeks = h.BuildWeeksBase + kw / h.KwPerExtraWeek;
            long saving = MathI.Min(h.InfraSpeedCapPermille, (long)InfraStaff(c, lab) * h.InfraSpeedPermillePerStaff);
            weeks = MathI.MulPermille(weeks, MathI.Permille - saving);
            return (int)MathI.Max(h.MinBuildWeeks, weeks);
        }

        /// <summary>The chance the town votes against a new hall: the site's own odds plus a quarter of the bloc's attention.</summary>
        public static int TownVoteChance(GameConfig c, LabState lab, SiteConfig site)
        {
            BlocStanding standing = lab.Bloc(site.BlocId);
            int attention = standing == null ? 0 : standing.Attention;
            return (int)MathI.Min(c.World.Halls.TownVoteCapPermille, site.TownVotePermille + attention / c.World.Halls.TownVoteAttentionDivisor);
        }

        public static bool IsFrozen(LabState lab, string blocId)
        {
            BlocStanding standing = lab.Bloc(blocId);
            return standing != null && standing.Level >= BlocStanding.Freeze;
        }

        public static string BlocOfHolding(GameConfig c, Holding holding)
        {
            SiteConfig site = c.World.Site(holding.SiteId);
            return site == null ? "" : site.BlocId;
        }

        /// <summary>The bloc a place is in: a holding's site, or the lab's home for the HQ.</summary>
        public static string BlocOfPlace(GameConfig c, LabState lab, int holdingId)
        {
            if (holdingId == HardwareBatch.Hq || holdingId == HardwareBatch.Cloud) return lab.HomeBlocId;
            Holding holding = lab.Holding(holdingId);
            return holding == null ? "" : BlocOfHolding(c, holding);
        }

        /// <summary>The power bill of one holding for a week, in parts. A holding with nothing installed and nothing leased costs nothing.</summary>
        public static PowerBill BillFor(GameConfig c, GameState state, LabState lab, Holding holding)
        {
            var bill = new PowerBill();
            bill.HoldingId = holding.Id;
            bill.SiteId = holding.SiteId;
            SiteConfig site = c.World.Site(holding.SiteId);
            if (site == null) return bill;

            int installed = InstalledKw(c, state, lab, holding.Id);
            int privateUsed = installed < holding.PrivateKw ? installed : holding.PrivateKw;
            int gridUsed = installed - privateUsed;
            int dealPrice = site.PowerPricePerKwWeek;
            PowerDealConfig deal = string.IsNullOrEmpty(holding.DealId) ? null : c.World.PowerDeal(holding.DealId);
            if (deal != null && deal.PricePerKwWeek > 0) dealPrice = deal.PricePerKwWeek;

            bill.InstalledKw = installed;
            bill.PrivateKw = privateUsed;
            bill.GridKw = gridUsed;
            bill.GridCost = (long)gridUsed * site.PowerPricePerKwWeek;
            bill.PrivateCost = (long)privateUsed * dealPrice;
            long energy = MathI.MulPermille(bill.GridCost + bill.PrivateCost, EffectiveCooling(c, lab, site));
            bill.Cooling = energy - bill.GridCost - bill.PrivateCost;
            int podCut = LiquidCoolingPodSystem.CoolingCostCutPermille(c, state, lab);
            if (podCut > 0 && bill.Cooling > 0)
                bill.Cooling = MathI.MulPermille(bill.Cooling, MathI.Permille - podCut);
            if (holding.Kind == HoldingKind.Colo) bill.ColoMarkup = (long)c.World.Colo.MarkupPerKwWeek * holding.HallKw;
            return bill;
        }

        public struct PowerBillTotals
        {
            public long GridCost;
            public long PrivateCost;
            public long Cooling;
            public long ColoMarkup;

            public long Total { get { return GridCost + PrivateCost + Cooling + ColoMarkup; } }
        }

        /// <summary>Lab-wide power bill rolled up by line (sums per-holding <see cref="BillFor"/> rows).</summary>
        public static PowerBillTotals WeeklyPowerBillTotals(GameConfig c, GameState state, LabState lab)
        {
            var t = new PowerBillTotals();
            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                PowerBill bill = BillFor(c, state, lab, lab.Holdings[i]);
                t.GridCost += bill.GridCost;
                t.PrivateCost += bill.PrivateCost;
                t.Cooling += bill.Cooling;
                t.ColoMarkup += bill.ColoMarkup;
            }
            return t;
        }

        public static long WeeklyPowerBill(GameConfig c, GameState state, LabState lab)
        {
            return WeeklyPowerBillTotals(c, state, lab).Total;
        }

        /// <summary>Upkeep of the halls on owned land. A colo's leased space is paid in its power bill.</summary>
        public static long WeeklyHallUpkeep(GameConfig c, LabState lab)
        {
            long total = 0;
            for (int i = 0; i < lab.Holdings.Count; i++)
                if (lab.Holdings[i].Kind == HoldingKind.Owned) total += (long)lab.Holdings[i].HallKw * c.World.Halls.UpkeepPerKwWeek;
            return total;
        }

        /// <summary>The footprint a government sees in a bloc, in kilowatts. A holding with a power deal counts as much as the deal's attention says.</summary>
        public static long FootprintKw(GameConfig c, GameState state, LabState lab, string blocId)
        {
            long kw = 0;
            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                Holding holding = lab.Holdings[i];
                if (BlocOfHolding(c, holding) != blocId) continue;
                long installed = InstalledKw(c, state, lab, holding.Id);
                PowerDealConfig deal = holding.PrivateKw > 0 && !string.IsNullOrEmpty(holding.DealId) ? c.World.PowerDeal(holding.DealId) : null;
                kw += deal == null ? installed : MathI.MulPermille(installed, deal.AttentionPermille);
            }
            return kw;
        }

        /// <summary>The footprint a government of this year starts to take notice of.</summary>
        public static long NoticeKw(GameConfig c, int year)
        {
            var list = c.World.Attention.NoticeKwByYear;
            long kw = list[0].Kw;
            for (int i = 0; i < list.Count; i++) if (list[i].Year <= year) kw = list[i].Kw;
            return kw;
        }

        /// <summary>
        /// How much a bloc's attention to the lab rises this month, before decay. <paramref name="revenueSharePpm"/> is the lab's
        /// share of every lab's revenue this month.
        /// </summary>
        public static AttentionGain GainFor(GameConfig c, GameState state, LabState lab, string blocId, int revenueSharePpm)
        {
            AttentionConfig a = c.World.Attention;
            BlocConfig bloc = c.World.Bloc(blocId);
            var gain = new AttentionGain();
            gain.BlocId = blocId;
            gain.FootprintKw = FootprintKw(c, state, lab, blocId);
            gain.NoticeKw = NoticeKw(c, GameDate.Year(c.Calendar, state.Tick));
            gain.Size = (int)MathI.Min(100, gain.FootprintKw * 100 / gain.NoticeKw);
            gain.FromFootprint = MathI.MulPermille(gain.Size, bloc.StrictnessPermille);
            if (lab.HomeBlocId == blocId)
            {
                gain.FromIncidents = (long)lab.IncidentsThisMonth * a.IncidentGain;
                gain.FromDominance = MathI.Max(0, revenueSharePpm - a.DominanceFromPpm) / a.DominanceDivisor;
            }
            long before = gain.FromFootprint + gain.FromIncidents + gain.FromDominance;
            long saving = MathI.Min(a.PolicySavingCapPermille, (long)PolicyStaff(c, lab) * a.PolicySavingPermillePerStaff);
            gain.PolicySaving = MathI.MulPermille(before, MathI.Permille - saving) - before;
            return gain;
        }
    }
}
