using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public enum ChipOptionGroup { Die, Format, Memory, Cooling }

    /// <summary>
    /// A lab's own accelerators: what a design is made of, how long it takes, what the foundry will make and how fast the
    /// lab's software matures. Every number in the chip rules is here.
    /// </summary>
    public sealed class ChipsConfig : ISync
    {
        public int PointsPerArchitectWeek;
        public int SoftwareStart;
        public int SoftwareCap;
        public int SoftwareBaseCap;
        public int SoftwarePerArchitectWeek;
        public int InHouseTrainingBonusPermille;
        public int TapeoutWeeks;
        public int RespinWeeks;
        public int RespinCostPermille;
        /// <summary>Each engineer takes this much off the respin chance, up to <see cref="RespinEngineerCap"/> engineers.</summary>
        public int RespinPerEngineerPermille;
        public int RespinEngineerCap;
        public int YieldRampPerWeek;
        public int YieldRampPerEngineer;
        public int YieldRampEngineerCap;
        public int TestLabRampPermille;
        public int YieldCrashPermille;
        public int YieldCrashKeepPermille;
        public int FoundryVendorSharePermille;
        public int BookAheadMonths;
        /// <summary>The most chips a plan-run lab lists for sale in a month.</summary>
        public int ListingMaxPerMonth;
        /// <summary>Weekly rent on cloud-hosted chips, as permille of spot GPU rent for the same compute.</summary>
        public int CloudRentPermille;
        /// <summary>A plan orders chips only once a design's yield has reached this.</summary>
        public int PlannerMinYieldPermille;
        public string ArchitectRoleId;
        public string TestLabRoomId;
        public List<ChipTargetConfig> Targets;
        public List<ChipNodeConfig> Nodes;
        public List<ChipOptionConfig> Options;
        public ChipShortageConfig Shortage = new ChipShortageConfig();

        public void Sync(Syncer s)
        {
            s.Int("pointsPerArchitectWeek", ref PointsPerArchitectWeek);
            s.Int("softwareStart", ref SoftwareStart);
            s.Int("softwareCap", ref SoftwareCap);
            s.Int("softwareBaseCap", ref SoftwareBaseCap);
            s.Int("softwarePerArchitectWeek", ref SoftwarePerArchitectWeek);
            s.Int("inHouseTrainingBonusPermille", ref InHouseTrainingBonusPermille);
            s.Int("tapeoutWeeks", ref TapeoutWeeks);
            s.Int("respinWeeks", ref RespinWeeks);
            s.Int("respinCostPermille", ref RespinCostPermille);
            s.Int("respinPerEngineerPermille", ref RespinPerEngineerPermille);
            s.Int("respinEngineerCap", ref RespinEngineerCap);
            s.Int("yieldRampPerWeek", ref YieldRampPerWeek);
            s.Int("yieldRampPerEngineer", ref YieldRampPerEngineer);
            s.Int("yieldRampEngineerCap", ref YieldRampEngineerCap);
            s.Int("testLabRampPermille", ref TestLabRampPermille);
            s.Int("yieldCrashPermille", ref YieldCrashPermille);
            s.Int("yieldCrashKeepPermille", ref YieldCrashKeepPermille);
            s.Int("foundryVendorSharePermille", ref FoundryVendorSharePermille);
            s.Int("bookAheadMonths", ref BookAheadMonths);
            s.Int("listingMaxPerMonth", ref ListingMaxPerMonth);
            s.Int("cloudRentPermille", ref CloudRentPermille);
            s.Int("plannerMinYieldPermille", ref PlannerMinYieldPermille);
            s.Str("architectRoleId", ref ArchitectRoleId);
            s.Str("testLabRoomId", ref TestLabRoomId);
            s.ObjList("targets", ref Targets);
            s.ObjList("nodes", ref Nodes);
            s.ObjList("options", ref Options);
            s.Obj("shortage", ref Shortage);
        }

        public ChipTargetConfig Target(string id)
        {
            for (int i = 0; i < Targets.Count; i++) if (Targets[i].IdValue == id) return Targets[i];
            return null;
        }

        public ChipNodeConfig Node(string id)
        {
            for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IdValue == id) return Nodes[i];
            return null;
        }

        public ChipOptionConfig Option(ChipOptionGroup group, string id)
        {
            for (int i = 0; i < Options.Count; i++) if (Options[i].Group == group && Options[i].IdValue == id) return Options[i];
            return null;
        }

        /// <summary>The foundry's room for one node in one month, in chips.</summary>
        public long FoundryRoom(ChipNodeConfig node)
        {
            return node.CapacityPerMonth * (1000 - FoundryVendorSharePermille) / 1000;
        }
    }

    /// <summary>What a design is for. An inference chip only serves customers; a training chip also trains.</summary>
    public sealed class ChipTargetConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public int Points;
        public int ComputePermille;
        public bool Trains;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("points", ref Points);
            s.Int("computePermille", ref ComputePermille);
            s.Bool("trains", ref Trains);
        }
    }

    /// <summary>One of the foundry's processes (TSMSea's). Its tier is the export tier of the chips made on it.</summary>
    public sealed class ChipNodeConfig : ISync, IHasId
    {
        public string IdValue;
        public string Name;
        public int OpensYear;
        public int Tier;
        public int PointsPermille;
        public int ComputePerChipWeek;
        public int KwPerChip;
        public long WaferCostPerChip;
        public long TapeoutCost;
        public int RespinPermille;
        public int InitialYieldPermille;
        public int MaxYieldPermille;
        public int LeadWeeks;
        public long CapacityPerMonth;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("opensYear", ref OpensYear);
            s.Int("tier", ref Tier);
            s.Int("pointsPermille", ref PointsPermille);
            s.Int("computePerChipWeek", ref ComputePerChipWeek);
            s.Int("kwPerChip", ref KwPerChip);
            s.Long("waferCostPerChip", ref WaferCostPerChip);
            s.Long("tapeoutCost", ref TapeoutCost);
            s.Int("respinPermille", ref RespinPermille);
            s.Int("initialYieldPermille", ref InitialYieldPermille);
            s.Int("maxYieldPermille", ref MaxYieldPermille);
            s.Int("leadWeeks", ref LeadWeeks);
            s.Long("capacityPerMonth", ref CapacityPerMonth);
        }
    }

    /// <summary>How long a world-news chip crunch lasts and how strong events may be.</summary>
    public sealed class ChipShortageConfig : ISync
    {
        public int DurationWeeks = 24;
        public int MinStrengthPermille = 1050;
        public int MaxStrengthPermille = 1600;

        public void Sync(Syncer s)
        {
            s.Int("durationWeeks", ref DurationWeeks);
            s.Int("minStrengthPermille", ref MinStrengthPermille);
            s.Int("maxStrengthPermille", ref MaxStrengthPermille);
        }
    }

    /// <summary>A pick on the design screen: a die size, a number format, a memory or a cooling, with what it does to the numbers.</summary>
    public sealed class ChipOptionConfig : ISync
    {
        public ChipOptionGroup Group;
        public string IdValue;
        public string Name;
        public string RequiresTech;
        public int ComputePermille;
        public int CostPermille;
        public int KwPermille;
        public int PointsPermille;
        public int YieldAdjust;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Enum("group", ref Group);
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("requiresTech", ref RequiresTech);
            s.Int("computePermille", ref ComputePermille);
            s.Int("costPermille", ref CostPermille);
            s.Int("kwPermille", ref KwPermille);
            s.Int("pointsPermille", ref PointsPermille);
            s.Int("yieldAdjust", ref YieldAdjust);
        }
    }
}
