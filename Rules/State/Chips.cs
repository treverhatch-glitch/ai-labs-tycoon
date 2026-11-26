using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public enum ChipDesignStatus { Designing, TapingOut, Ready, Cancelled }

    /// <summary>
    /// A chip a lab is designing or has made. It is a recipe: what for, which process, die, number format, memory and cooling,
    /// how many design points it needs, and once it works the yield the foundry gets from it.
    /// </summary>
    public sealed class ChipDesign : ISync
    {
        public int Id;
        public string Name;
        public int Version;
        public string Target;
        public string NodeId;
        public string DieId;
        public string FormatId;
        public string MemoryId;
        public string CoolingId;
        public long PointsRequired;
        public long PointsDone;
        public ChipDesignStatus Status;
        /// <summary>When a design is taping out: the tick its result comes back.</summary>
        public int StageEndsTick;
        public int Respins;
        /// <summary>Share of chips off the line that work, in permille. 0 until the design is ready.</summary>
        public int YieldPermille;
        public int ReadyTick;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("name", ref Name);
            s.Int("version", ref Version);
            s.Str("target", ref Target);
            s.Str("nodeId", ref NodeId);
            s.Str("dieId", ref DieId);
            s.Str("formatId", ref FormatId);
            s.Str("memoryId", ref MemoryId);
            s.Str("coolingId", ref CoolingId);
            s.Long("pointsRequired", ref PointsRequired);
            s.Long("pointsDone", ref PointsDone);
            s.Enum("status", ref Status);
            s.Int("stageEndsTick", ref StageEndsTick);
            s.Int("respins", ref Respins);
            s.Int("yieldPermille", ref YieldPermille);
            s.Int("readyTick", ref ReadyTick);
        }
    }

    /// <summary>Chips on order at the foundry for a buyer's holding. They arrive as a fleet batch when the delivery tick comes.</summary>
    public sealed class ChipOrder : ISync
    {
        public int Id;
        public int DesignId;
        public int DesignLabId;
        public int BuyerLabId;
        public int HoldingId;
        public int Count;
        public int DeliveryTick;
        /// <summary>Kilowatts the holding keeps free for these chips until they arrive.</summary>
        public int ReservedKw;
        /// <summary>The foundry month the order took room in, so a cancelled order can give it back.</summary>
        public int FoundryMonth;
        public string NodeId;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Int("designId", ref DesignId);
            s.Int("designLabId", ref DesignLabId);
            s.Int("buyerLabId", ref BuyerLabId);
            s.Int("holdingId", ref HoldingId);
            s.Int("count", ref Count);
            s.Int("deliveryTick", ref DeliveryTick);
            s.Int("reservedKw", ref ReservedKw);
            s.Int("foundryMonth", ref FoundryMonth);
            s.Str("nodeId", ref NodeId);
        }
    }

    /// <summary>A lab's offer to sell chips of one of its designs to other labs.</summary>
    public sealed class ChipListing : ISync
    {
        public int DesignId;
        public long PricePerChip;
        public int MaxPerMonth;
        public int SoldThisMonth;
        public int ListedSinceTick = -1;
        public bool Spotlight;
        /// <summary>Paused listings skip upkeep and sales until unpaused (Q177).</summary>
        public bool Paused;

        public void Sync(Syncer s)
        {
            s.Int("designId", ref DesignId);
            s.Long("pricePerChip", ref PricePerChip);
            s.Int("maxPerMonth", ref MaxPerMonth);
            s.Int("soldThisMonth", ref SoldThisMonth);
            s.Int("listedSinceTick", ref ListedSinceTick);
            s.Bool("spotlight", ref Spotlight);
            s.Bool("paused", ref Paused);
        }
    }

    /// <summary>How many chips of one process have been booked for one month, by every lab together.</summary>
    public sealed class FoundryMonth : ISync
    {
        public string NodeId;
        /// <summary>Months since the game's first tick.</summary>
        public int MonthIndex;
        public long Booked;

        public void Sync(Syncer s)
        {
            s.Str("nodeId", ref NodeId);
            s.Int("monthIndex", ref MonthIndex);
            s.Long("booked", ref Booked);
        }
    }
}
