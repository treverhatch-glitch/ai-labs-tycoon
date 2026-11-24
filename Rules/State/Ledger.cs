using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum LedgerLine
    {
        Revenue, Grants,
        Salaries, RoomUpkeep, GpuRent, GpuUpkeep,
        Hiring, Construction, Hardware, Training, Fines,
        Land, SiteBuild, Power, ChipDevelopment,
        ChipSales, GreyRevenue, Laundered, CapitalIn,
        Acquisition, AssetSale,
        Insurance, Sublet
    }

    /// <summary>
    /// Money in and out by line. Income lines are positive and cost lines are negative,
    /// so <see cref="Net"/> is always the change in cash the totals explain.
    /// </summary>
    public sealed class LedgerTotals : ISync
    {
        public long Revenue;
        public long Grants;
        public long Salaries;
        public long RoomUpkeep;
        public long GpuRent;
        public long GpuUpkeep;
        public long Hiring;
        public long Construction;
        public long Hardware;
        public long Training;
        public long Fines;
        public long Land;
        public long SiteBuild;
        public long Power;
        public long ChipDevelopment;
        public long ChipSales;
        public long GreyRevenue;
        public long Laundered;
        public long CapitalIn;
        public long Acquisition;
        public long AssetSale;
        public long Insurance;
        public long Sublet;

        public long Income { get { return Revenue + Grants; } }

        public long Net
        {
            get
            {
                return Revenue + Grants + AssetSale + Sublet + Salaries + RoomUpkeep + GpuRent + GpuUpkeep + Hiring + Construction + Hardware + Training + Fines
                + Land + SiteBuild + Power + ChipDevelopment + ChipSales + GreyRevenue + Laundered + CapitalIn + Acquisition + Insurance;
            }
        }

        public long Get(LedgerLine line)
        {
            switch (line)
            {
                case LedgerLine.Revenue: return Revenue;
                case LedgerLine.Grants: return Grants;
                case LedgerLine.Salaries: return Salaries;
                case LedgerLine.RoomUpkeep: return RoomUpkeep;
                case LedgerLine.GpuRent: return GpuRent;
                case LedgerLine.GpuUpkeep: return GpuUpkeep;
                case LedgerLine.Hiring: return Hiring;
                case LedgerLine.Construction: return Construction;
                case LedgerLine.Hardware: return Hardware;
                case LedgerLine.Training: return Training;
                case LedgerLine.Fines: return Fines;
                case LedgerLine.Land: return Land;
                case LedgerLine.SiteBuild: return SiteBuild;
                case LedgerLine.Power: return Power;
                case LedgerLine.ChipDevelopment: return ChipDevelopment;
                case LedgerLine.ChipSales: return ChipSales;
                case LedgerLine.GreyRevenue: return GreyRevenue;
                case LedgerLine.Laundered: return Laundered;
                case LedgerLine.CapitalIn: return CapitalIn;
                case LedgerLine.Acquisition: return Acquisition;
                case LedgerLine.AssetSale: return AssetSale;
                case LedgerLine.Insurance: return Insurance;
                case LedgerLine.Sublet: return Sublet;
                default: return 0;
            }
        }

        public void Add(LedgerLine line, long amount)
        {
            switch (line)
            {
                case LedgerLine.Revenue: Revenue += amount; break;
                case LedgerLine.Grants: Grants += amount; break;
                case LedgerLine.Salaries: Salaries += amount; break;
                case LedgerLine.RoomUpkeep: RoomUpkeep += amount; break;
                case LedgerLine.GpuRent: GpuRent += amount; break;
                case LedgerLine.GpuUpkeep: GpuUpkeep += amount; break;
                case LedgerLine.Hiring: Hiring += amount; break;
                case LedgerLine.Construction: Construction += amount; break;
                case LedgerLine.Hardware: Hardware += amount; break;
                case LedgerLine.Training: Training += amount; break;
                case LedgerLine.Fines: Fines += amount; break;
                case LedgerLine.Land: Land += amount; break;
                case LedgerLine.SiteBuild: SiteBuild += amount; break;
                case LedgerLine.Power: Power += amount; break;
                case LedgerLine.ChipDevelopment: ChipDevelopment += amount; break;
                case LedgerLine.ChipSales: ChipSales += amount; break;
                case LedgerLine.GreyRevenue: GreyRevenue += amount; break;
                case LedgerLine.Laundered: Laundered += amount; break;
                case LedgerLine.CapitalIn: CapitalIn += amount; break;
                case LedgerLine.Acquisition: Acquisition += amount; break;
                case LedgerLine.AssetSale: AssetSale += amount; break;
                case LedgerLine.Insurance: Insurance += amount; break;
                case LedgerLine.Sublet: Sublet += amount; break;
            }
        }

        public void Sync(Syncer s)
        {
            s.Long("revenue", ref Revenue);
            s.Long("grants", ref Grants);
            s.Long("salaries", ref Salaries);
            s.Long("roomUpkeep", ref RoomUpkeep);
            s.Long("gpuRent", ref GpuRent);
            s.Long("gpuUpkeep", ref GpuUpkeep);
            s.Long("hiring", ref Hiring);
            s.Long("construction", ref Construction);
            s.Long("hardware", ref Hardware);
            s.Long("training", ref Training);
            s.Long("fines", ref Fines);
            s.Long("land", ref Land);
            s.Long("siteBuild", ref SiteBuild);
            s.Long("power", ref Power);
            s.Long("chipDevelopment", ref ChipDevelopment);
            s.Long("chipSales", ref ChipSales);
            s.Long("greyRevenue", ref GreyRevenue);
            s.Long("laundered", ref Laundered);
            s.Long("capitalIn", ref CapitalIn);
            s.Long("acquisition", ref Acquisition);
            s.Long("assetSale", ref AssetSale);
            s.Long("insurance", ref Insurance);
            s.Long("sublet", ref Sublet);
        }
    }

    public sealed class MonthRecord : ISync
    {
        public int Year;
        public int Month;
        public long EndCash;
        public LedgerTotals Totals = new LedgerTotals();

        public void Sync(Syncer s)
        {
            s.Int("year", ref Year);
            s.Int("month", ref Month);
            s.Long("endCash", ref EndCash);
            s.Obj("totals", ref Totals);
        }
    }

    public sealed class LedgerState : ISync
    {
        public long StartCash;
        public LedgerTotals ThisWeek = new LedgerTotals();
        public LedgerTotals LastWeek = new LedgerTotals();
        public LedgerTotals ThisMonth = new LedgerTotals();
        public LedgerTotals Lifetime = new LedgerTotals();
        public List<MonthRecord> History = new List<MonthRecord>();

        public void Sync(Syncer s)
        {
            s.Long("startCash", ref StartCash);
            s.Obj("thisWeek", ref ThisWeek);
            s.Obj("lastWeek", ref LastWeek);
            s.Obj("thisMonth", ref ThisMonth);
            s.Obj("lifetime", ref Lifetime);
            s.ObjList("history", ref History);
        }
    }

    public sealed class WeekReport : ISync
    {
        public long Compute;
        public long TrainingCompute;
        public long ServingCompute;
        public long ServingDemand;
        public int ServedPermille;
        public long Revenue;
        public long ResearchGained;

        public void Sync(Syncer s)
        {
            s.Long("compute", ref Compute);
            s.Long("trainingCompute", ref TrainingCompute);
            s.Long("servingCompute", ref ServingCompute);
            s.Long("servingDemand", ref ServingDemand);
            s.Int("servedPermille", ref ServedPermille);
            s.Long("revenue", ref Revenue);
            s.Long("researchGained", ref ResearchGained);
        }
    }
}