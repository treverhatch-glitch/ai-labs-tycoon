using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>What a set of choices makes. Worked out from the config every time, so a design stores only its choices.</summary>
    public struct ChipSpec
    {
        public bool Valid;
        /// <summary>Compute per chip a week, on paper: before the owner's software, yield or heat.</summary>
        public long Compute;
        public long Points;
        public long WaferCost;
        public int Kw;
        public int InitialYieldPermille;
        public int MaxYieldPermille;
        public bool Trains;
        public int Tier;
    }

    /// <summary>Read-only answers about chip designs, orders and the foundry.</summary>
    public static class ChipQueries
    {
        const long KwScale = 1000L * 1000L * 1000L * 1000L;

        public static ChipSpec Derive(GameConfig c, string targetId, string nodeId, string dieId, string formatId, string memoryId, string coolingId)
        {
            var spec = new ChipSpec();
            ChipsConfig k = c.Chips;
            ChipTargetConfig target = k.Target(targetId);
            ChipNodeConfig node = k.Node(nodeId);
            ChipOptionConfig die = k.Option(ChipOptionGroup.Die, dieId);
            ChipOptionConfig format = k.Option(ChipOptionGroup.Format, formatId);
            ChipOptionConfig memory = k.Option(ChipOptionGroup.Memory, memoryId);
            ChipOptionConfig cooling = k.Option(ChipOptionGroup.Cooling, coolingId);
            if (target == null || node == null || die == null || format == null || memory == null || cooling == null) return spec;

            long compute = MathI.MulPermille(node.ComputePerChipWeek, target.ComputePermille);
            compute = MathI.MulPermille(compute, die.ComputePermille);
            compute = MathI.MulPermille(compute, format.ComputePermille);
            compute = MathI.MulPermille(compute, memory.ComputePermille);
            compute = MathI.MulPermille(compute, cooling.ComputePermille);

            long points = MathI.MulPermille(target.Points, die.PointsPermille);
            points = MathI.MulPermille(points, format.PointsPermille);
            points = MathI.MulPermille(points, memory.PointsPermille);
            points = MathI.MulPermille(points, cooling.PointsPermille);
            points = MathI.MulPermille(points, node.PointsPermille);

            long wafer = MathI.MulPermille(node.WaferCostPerChip, die.CostPermille);
            wafer = MathI.MulPermille(wafer, memory.CostPermille);
            wafer = MathI.MulPermille(wafer, cooling.CostPermille);

            long kwNumerator = (long)node.KwPerChip * die.KwPermille * format.KwPermille * memory.KwPermille * cooling.KwPermille;
            long kw = (kwNumerator + KwScale - 1) / KwScale;
            spec.Kw = (int)MathI.Max(1, kw);

            int adjust = die.YieldAdjust + format.YieldAdjust + memory.YieldAdjust + cooling.YieldAdjust;
            spec.MaxYieldPermille = MathI.Clamp(node.MaxYieldPermille + adjust, 100, 950);
            int initial = node.InitialYieldPermille + adjust;
            if (initial < 100) initial = 100;
            if (initial > spec.MaxYieldPermille) initial = spec.MaxYieldPermille;
            spec.InitialYieldPermille = initial;

            spec.Compute = compute;
            spec.Points = points < 1 ? 1 : points;
            spec.WaferCost = wafer < 1 ? 1 : wafer;
            spec.Trains = target.Trains;
            spec.Tier = node.Tier;
            spec.Valid = true;
            return spec;
        }

        public static ChipSpec Spec(GameConfig c, ChipDesign d)
        {
            return Derive(c, d.Target, d.NodeId, d.DieId, d.FormatId, d.MemoryId, d.CoolingId);
        }

        /// <summary>The design a chip batch is of, or null when the lab that made it no longer has it.</summary>
        public static ChipDesign DesignOf(GameState state, HardwareBatch batch)
        {
            if (batch.ChipDesignId < 0) return null;
            LabState owner = state.Lab(batch.DesignLabId);
            return owner == null ? null : owner.Design(batch.ChipDesignId);
        }

        public static int KwPerChip(GameConfig c, GameState state, HardwareBatch batch)
        {
            ChipDesign d = DesignOf(state, batch);
            if (d == null) return 1;
            ChipSpec spec = Spec(c, d);
            return spec.Valid ? spec.Kw : 1;
        }

        /// <summary>Kilowatts a holding keeps free for chips that are on order.</summary>
        public static int ReservedKw(LabState lab, int holdingId)
        {
            int kw = 0;
            for (int i = 0; i < lab.ChipOrders.Count; i++) if (lab.ChipOrders[i].HoldingId == holdingId) kw += lab.ChipOrders[i].ReservedKw;
            return kw;
        }

        public static int PendingOrdersAt(LabState lab, int holdingId)
        {
            int n = 0;
            for (int i = 0; i < lab.ChipOrders.Count; i++) if (lab.ChipOrders[i].HoldingId == holdingId) n++;
            return n;
        }

        public static int Architects(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.ChipDesign); }

        public static int Engineers(GameConfig c, LabState lab) { return LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering); }

        public static bool HasTestLab(GameConfig c, LabState lab)
        {
            for (int i = 0; i < lab.Rooms.Count; i++) if (lab.Rooms[i].TypeId == c.Chips.TestLabRoomId) return true;
            return false;
        }

        /// <summary>Parts of owner chip software permille; each part sums to <see cref="EffectiveSoftware"/>.</summary>
        public struct OwnerSoftwareBreakdown
        {
            public int FromLab;
            public int FromTech;
            /// <summary>Negative when <see cref="FromLab"/> + <see cref="FromTech"/> exceeds the config cap.</summary>
            public int CapClip;

            public int Total { get { return FromLab + FromTech + CapClip; } }
        }

        public static OwnerSoftwareBreakdown SoftwareParts(GameConfig c, LabState lab)
        {
            var b = new OwnerSoftwareBreakdown();
            b.FromLab = lab.SoftwarePermille;
            if (b.FromLab <= 0) return b;
            b.FromTech = (int)LabQueries.TechBonus(c, lab, TechEffectKind.SoftwarePermille);
            int capped = EffectiveSoftware(c, lab);
            b.CapClip = capped - b.FromLab - b.FromTech;
            return b;
        }

        /// <summary>The software a lab's chips run, in permille of paper speed: what its compiler has matured to, plus its compiler techs.</summary>
        public static int EffectiveSoftware(GameConfig c, LabState lab)
        {
            if (lab.SoftwarePermille <= 0) return 0;
            long total = lab.SoftwarePermille + LabQueries.TechBonus(c, lab, TechEffectKind.SoftwarePermille);
            return (int)MathI.Min(c.Chips.SoftwareCap, total);
        }

        /// <summary>Compute of a batch of chips this week at a place with no heat or distance applied.</summary>
        public static long BatchCompute(GameConfig c, GameState state, HardwareBatch batch)
        {
            ChipDesign d = DesignOf(state, batch);
            if (d == null) return 0;
            ChipSpec spec = Spec(c, d);
            if (!spec.Valid) return 0;
            LabState owner = state.Lab(batch.DesignLabId);
            return MathI.MulPermille(batch.Count * spec.Compute, EffectiveSoftware(c, owner));
        }

        public static bool BatchTrains(GameConfig c, GameState state, HardwareBatch batch)
        {
            ChipDesign d = DesignOf(state, batch);
            if (d == null) return false;
            ChipTargetConfig target = c.Chips.Target(d.Target);
            return target != null && target.Trains;
        }

        public static bool TargetUnlocked(GameConfig c, LabState lab, string targetId)
        {
            for (int i = 0; i < lab.Techs.Count; i++)
            {
                TechConfig tech = c.Tech(lab.Techs[i]);
                if (tech == null) continue;
                for (int k = 0; k < tech.Effects.Count; k++)
                    if (tech.Effects[k].Kind == TechEffectKind.UnlockChipTarget && tech.Effects[k].Id == targetId) return true;
            }
            return false;
        }

        public static bool OptionAvailable(LabState lab, ChipOptionConfig option)
        {
            return string.IsNullOrEmpty(option.RequiresTech) || LabQueries.HasTech(lab, option.RequiresTech);
        }

        public static bool NodeOpen(GameConfig c, ChipNodeConfig node, int tick)
        {
            return GameDate.Year(c.Calendar, tick) >= node.OpensYear;
        }

        /// <summary>The design in progress (Designing or TapingOut), or null.</summary>
        public static ChipDesign DesignInProgress(LabState lab)
        {
            for (int i = 0; i < lab.ChipDesigns.Count; i++)
            {
                ChipDesignStatus s = lab.ChipDesigns[i].Status;
                if (s == ChipDesignStatus.Designing) return lab.ChipDesigns[i];
            }
            return null;
        }

        public static int NextVersion(LabState lab)
        {
            int v = 0;
            for (int i = 0; i < lab.ChipDesigns.Count; i++) if (lab.ChipDesigns[i].Version > v) v = lab.ChipDesigns[i].Version;
            return v + 1;
        }

        /// <summary>The weeks a design in progress has left at the lab's current architects, or -1 when nobody is working on it.</summary>
        public static int WeeksToFinishDesign(GameConfig c, LabState lab, ChipDesign d)
        {
            long perWeek = (long)Architects(c, lab) * c.Chips.PointsPerArchitectWeek;
            if (perWeek <= 0) return -1;
            long left = d.PointsRequired - d.PointsDone;
            return left <= 0 ? 0 : (int)((left + perWeek - 1) / perWeek);
        }

        /// <summary>The share of respins a tape-out of this node rolls for at this lab: the node's own, less what engineers take off.</summary>
        public static int RespinChancePermille(GameConfig c, LabState lab, ChipNodeConfig node)
        {
            long engineers = MathI.Min(Engineers(c, lab), c.Chips.RespinEngineerCap);
            return (int)MathI.Max(0, node.RespinPermille - engineers * c.Chips.RespinPerEngineerPermille);
        }

        public static long RespinCost(GameConfig c, ChipNodeConfig node) { return MathI.MulPermille(node.TapeoutCost, c.Chips.RespinCostPermille); }

        /// <summary>What a yield of this many permille costs per chip that works.</summary>
        public static long CostPerGoodChip(ChipSpec spec, int yieldPermille)
        {
            return yieldPermille < 1 ? spec.WaferCost : spec.WaferCost * MathI.Permille / yieldPermille;
        }

        /// <summary>What the foundry charges for this many working chips at a yield: the wafers, divided by the yield.</summary>
        public static long OrderCost(ChipSpec spec, int yieldPermille, int count)
        {
            return yieldPermille < 1 ? spec.WaferCost * count : spec.WaferCost * count * MathI.Permille / yieldPermille;
        }

        public static int MonthIndex(GameConfig c, int tick) { return tick / c.Calendar.WeeksPerMonth; }

        public static long BookedIn(GameState state, string nodeId, int monthIndex)
        {
            for (int i = 0; i < state.Foundry.Count; i++)
                if (state.Foundry[i].NodeId == nodeId && state.Foundry[i].MonthIndex == monthIndex) return state.Foundry[i].Booked;
            return 0;
        }

        /// <summary>The first foundry month from this one on with room for the whole order, or -1 when none within the booking window has.</summary>
        public static int FirstFreeMonth(GameConfig c, GameState state, ChipNodeConfig node, int count)
        {
            long room = ChipShortageSystem.EffectiveFoundryRoom(c, state, node);
            if (count > room) return -1;
            int now = MonthIndex(c, state.Tick);
            for (int m = now; m < now + c.Chips.BookAheadMonths; m++)
                if (BookedIn(state, node.IdValue, m) + count <= room) return m;
            return -1;
        }

        public static int DeliveryTickOf(GameConfig c, ChipNodeConfig node, int monthIndex)
        {
            return monthIndex * c.Calendar.WeeksPerMonth + node.LeadWeeks;
        }

        /// <summary>Chips that work in the lab's own training fleet, as a share of the lab's general compute, in permille.</summary>
        public static int OwnChipsShare(GameConfig c, GameState state, LabState lab)
        {
            ComputeSplit split = LabQueries.Split(c, state, lab, false);
            long passive = ExpansionSystem.PassiveExpansionGeneralCompute(c, state, lab);
            long general = split.General - passive;
            if (general <= 0) return 0;
            return (int)(split.OwnChips * MathI.Permille / general);
        }

        /// <summary>True when the lab has trained or run any training chip of its own: the vendor charges it more.</summary>
        public static bool RunsOwnTrainingChips(GameConfig c, GameState state, LabState lab)
        {
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.ChipDesignId >= 0 && b.DesignLabId == lab.Id && b.Count > 0 && BatchTrains(c, state, b)) return true;
            }
            return false;
        }

        /// <summary>Whether a design's chips can be put in a place this lab owns, after the export rules and permits.</summary>
        public static bool CloudDelivery(int holdingId) { return holdingId == HardwareBatch.Cloud; }

        public static string CloudPlaceProblem(GameConfig c, GameState state, LabState lab, int tier, string designerHomeBlocId)
        {
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId)) return "frozen";
            if (!LabQueries.ExportAllowedFrom(c, state, lab.HomeBlocId, tier, lab.HomeBlocId)) return "export";
            if (!LabQueries.ExportAllowedFrom(c, state, designerHomeBlocId, tier, lab.HomeBlocId)) return "export";
            return null;
        }

        public static long WeeklyCloudChipRent(GameConfig c, GameState state, LabState lab)
        {
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            if (gen.ComputePerGpuWeek < 1) return 0;
            long rent = 0;
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch batch = lab.Fleet[i];
                if (batch.HoldingId != HardwareBatch.Cloud || batch.ChipDesignId < 0 || batch.Count <= 0) continue;
                long compute = BatchCompute(c, state, batch);
                long units = (compute + gen.ComputePerGpuWeek - 1) / gen.ComputePerGpuWeek;
                rent += MathI.MulPermille(units * gen.RentPerGpuWeek, c.Chips.CloudRentPermille);
            }
            return rent;
        }

        public static string PlaceProblem(GameConfig c, GameState state, LabState lab, int holdingId, int tier, string designerHomeBlocId)
        {
            string blocId = WorldQueries.BlocOfPlace(c, lab, holdingId);
            if (WorldQueries.IsFrozen(lab, blocId)) return "frozen";
            if (!LabQueries.ExportAllowedFrom(c, state, lab.HomeBlocId, tier, blocId)) return "export";
            if (!LabQueries.ExportAllowedFrom(c, state, designerHomeBlocId, tier, blocId)) return "export";
            return null;
        }

        public static List<ChipDesign> ReadyDesigns(LabState lab)
        {
            var list = new List<ChipDesign>();
            for (int i = 0; i < lab.ChipDesigns.Count; i++) if (lab.ChipDesigns[i].Status == ChipDesignStatus.Ready) list.Add(lab.ChipDesigns[i]);
            return list;
        }
    }
}
