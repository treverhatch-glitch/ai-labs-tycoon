using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>What a lab did. New values are appended only: saves store the name.</summary>
    public enum DecisionAction
    {
        StartedRun, SkippedRun, Released, HeldBack, Hired, Built, BoughtLand, BuiltHall, BoughtGpus, RentedGpus, Tightened,
        StartedChip, OrderedChips, BoughtChips, Published, Paused, Lobbied,
        /// <summary>The vendor moved a lab's share of the supply.</summary>
        ShiftedAllocation,
        /// <summary>The vendor courted the rivals of a lab that makes its own chips.</summary>
        Courted,
        /// <summary>A rival poached a star from the player's lab.</summary>
        PoachedStaff,
        /// <summary>A lab sold hardware that was generations behind and bought the current one.</summary>
        Replaced,
        TapedOut,
        ListedChips
    }

    /// <summary>The number behind a decision. New values are appended only.</summary>
    public enum ReasonCode
    {
        RunwayWeeks, ForecastGainPermille, MaxRunWeeks, RivalBestCapability, RivalWonBracket, CultureAlignmentPermille,
        AlignmentBelowGate, InDebt, AllocationLeft, CheapestSite, PowerPrice, GpuTargetGap, ChipProgramYear,
        PriceVsEnvydiaPermille, BoardDrama, FeelingTowardPlayer,
        ReleaseGate, GenerationsBehind, StandingNow, ShareNowPermille, SharePreviousPermille, StandingFromBuying, StandingFromChips,
        ChipArchitects, YieldPermille, DesignPoints, DesignPointsNeeded, FoundryMonth, PricePerComputePermille, ChipsOwnedSharePermille
    }

    public sealed class Reason : ISync
    {
        public ReasonCode Code;
        public int Value;

        public static Reason Of(ReasonCode code, long value)
        {
            var r = new Reason();
            r.Code = code;
            r.Value = value > int.MaxValue ? int.MaxValue : value < int.MinValue ? int.MinValue : (int)value;
            return r;
        }

        public void Sync(Syncer s)
        {
            s.Enum("code", ref Code);
            s.Int("value", ref Value);
        }
    }

    /// <summary>One line of a lab's decision log: what it did and the numbers it did it on. Never text; <c>DecisionText</c> reads it.</summary>
    public sealed class DecisionEntry : ISync
    {
        public int Tick;
        public DecisionAction Action;
        /// <summary>The thing it is about: a model name, a site, role, room type, tech or lab id. Or empty.</summary>
        public string Ref;
        public long Amount;
        public List<Reason> Reasons = new List<Reason>();

        public void Sync(Syncer s)
        {
            s.Int("tick", ref Tick);
            s.Enum("action", ref Action);
            s.Str("ref", ref Ref);
            s.Long("amount", ref Amount);
            s.ObjList("reasons", ref Reasons);
        }

        public int ReasonValue(ReasonCode code, int fallback = 0)
        {
            for (int i = 0; i < Reasons.Count; i++) if (Reasons[i].Code == code) return Reasons[i].Value;
            return fallback;
        }

        public bool Has(ReasonCode code)
        {
            for (int i = 0; i < Reasons.Count; i++) if (Reasons[i].Code == code) return true;
            return false;
        }
    }

    /// <summary>The GPU vendor's side of the books. It is an actor, not a lab: it has no cash and no models, only a supply, a standing for every lab and a log.</summary>
    public sealed class VendorState : ISync
    {
        /// <summary>Units the vendor can sell this month, after the keynote.</summary>
        public int SupplyThisMonth;
        public List<VendorStanding> Standings = new List<VendorStanding>();
        public List<DecisionEntry> Log = new List<DecisionEntry>();

        public VendorStanding Standing(int labId)
        {
            for (int i = 0; i < Standings.Count; i++) if (Standings[i].LabId == labId) return Standings[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("supplyThisMonth", ref SupplyThisMonth);
            s.ObjList("standings", ref Standings);
            s.ObjList("log", ref Log);
        }
    }

    public sealed class VendorStanding : ISync
    {
        public int LabId;
        /// <summary>0 to 1000. Buying raises it; making your own chips lowers it.</summary>
        public int Standing;
        public int BoughtThisMonth;
        /// <summary>The units this lab may buy this month, worked out when the month began.</summary>
        public int AllocationThisMonth;
        /// <summary>The share of the supply this lab was given, in permille, so a month-to-month move can be noticed.</summary>
        public int SharePermille;
        /// <summary>What moved the standing since the month began: from buying, and from the lab's own chips.</summary>
        public int FromBuying;
        public int FromChips;
        /// <summary>True once the lab has taped out a training chip (the first one costs standing, later ones do not).</summary>
        public bool TrainingChipSeen;
        /// <summary>True once the lab has listed chips for sale (the first listing costs standing, later ones do not).</summary>
        public bool ListingSeen;

        public void Sync(Syncer s)
        {
            s.Int("labId", ref LabId);
            s.Int("standing", ref Standing);
            s.Int("boughtThisMonth", ref BoughtThisMonth);
            s.Int("allocationThisMonth", ref AllocationThisMonth);
            s.Int("sharePermille", ref SharePermille);
            s.Int("fromBuying", ref FromBuying);
            s.Int("fromChips", ref FromChips);
            s.Bool("trainingChipSeen", ref TrainingChipSeen);
            s.Bool("listingSeen", ref ListingSeen);
        }
    }
}
