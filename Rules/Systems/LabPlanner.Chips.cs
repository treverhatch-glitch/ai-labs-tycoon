using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The planner's chip programme: staff, design, tape-out, orders, listings and buying other labs' lists.</summary>
    public static partial class LabPlanner
    {
        static void Chips(Ctx x, int weeks)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            PlanConfig plan = x.Plan;
            if (plan.ChipProgramYear <= 0 || GameDate.Year(c.Calendar, x.State.Tick) < plan.ChipProgramYear) return;
            if (!plan.OwnGpus || plan.SiteAppetitePermille <= 0) return;
            if (!LabQueries.HasTech(lab, "chip-team") || Overstretched(x)) return;

            Architects(x);
            StartDesign(x);
            TapeOutDesign(x);
            int target = Grown(x, plan.Gpus, plan.GpuGrowthPermillePerYear, weeks);
            OrderOwnChips(x, target);
            if (plan.BuysListedChips) BuyLists(x, target);
            if (plan.ListsChipsMarkupPermille > 0) ListChips(x);
        }

        static void Architects(Ctx x)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            RoleConfig role = c.Role(c.Chips.ArchitectRoleId);
            int target = Math.Max(4, LabQueries.StaffByFunction(c, lab, RoleFunction.Research) / 3);
            int have = LabQueries.StaffCount(lab, role.Id);
            if (have >= target) return;

            int seats = LabQueries.Seats(c, lab, role.Id);
            if (seats < target)
            {
                RoomTypeConfig type = RoomFor(c, lab, role.Id, x.State.Tick);
                if (type == null) return;
                BuildRoom(x, type, Math.Max(target - seats, MinSeatsPerRoom) * type.TilesPerSeat);
                seats = LabQueries.Seats(c, lab, role.Id);
            }
            long perHire = role.HireCost + role.WeeklySalary * x.Plan.ReserveWeeks;
            long spare = lab.Cash - Reserve(x);
            int affordable = perHire <= 0 ? target : (int)MathI.Clamp(spare / perHire, 0, target);
            int hire = Math.Min(Math.Min(target - have, seats - have), affordable);
            if (hire > 0) Decide(x, Command.Hire(lab.Id, role.Id, hire), DecisionAction.Hired, role.Id, hire, Reason.Of(ReasonCode.ChipArchitects, have + hire), Runway(x));
        }

        static ChipOptionConfig BestOption(Ctx x, ChipOptionGroup group)
        {
            ChipOptionConfig best = null;
            for (int i = 0; i < x.C.Chips.Options.Count; i++)
            {
                ChipOptionConfig o = x.C.Chips.Options[i];
                if (o.Group == group && ChipQueries.OptionAvailable(x.Lab, o)) best = o;
            }
            return best;
        }

        /// <summary>
        /// Starts a design when none is in the works and the lab has nothing newer than the newest process that is open and
        /// affordable: the best target it can design, that process, a medium die and the best format, memory and cooling it knows.
        /// </summary>
        static void StartDesign(Ctx x)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            ChipsConfig k = c.Chips;
            if (ChipQueries.DesignInProgress(lab) != null || ChipQueries.Architects(c, lab) < 1) return;
            for (int i = 0; i < lab.ChipDesigns.Count; i++) if (lab.ChipDesigns[i].Status == ChipDesignStatus.TapingOut) return;

            ChipTargetConfig target = null;
            for (int i = 0; i < k.Targets.Count; i++) if (ChipQueries.TargetUnlocked(c, lab, k.Targets[i].IdValue)) target = k.Targets[i];
            if (target == null) return;

            long spare = lab.Cash - Reserve(x);
            ChipNodeConfig node = null;
            for (int i = 0; i < k.Nodes.Count; i++)
            {
                ChipNodeConfig n = k.Nodes[i];
                if (ChipQueries.NodeOpen(c, n, x.State.Tick) && n.TapeoutCost + ChipQueries.RespinCost(c, n) <= spare) node = n;
            }
            if (node == null) return;

            ChipDesign newest = null;
            for (int i = 0; i < lab.ChipDesigns.Count; i++)
                if (lab.ChipDesigns[i].Status == ChipDesignStatus.Ready && (newest == null || lab.ChipDesigns[i].Version > newest.Version)) newest = lab.ChipDesigns[i];
            if (newest != null)
            {
                int had = k.Nodes.IndexOf(k.Node(newest.NodeId));
                bool newerTarget = !newest.Target.Equals(target.IdValue) && target.Trains;
                if (k.Nodes.IndexOf(node) <= had && !newerTarget) return;
            }

            ChipOptionConfig format = BestOption(x, ChipOptionGroup.Format);
            ChipOptionConfig memory = BestOption(x, ChipOptionGroup.Memory);
            ChipOptionConfig cooling = BestOption(x, ChipOptionGroup.Cooling);
            if (format == null || memory == null || cooling == null) return;
            ChipSpec spec = ChipQueries.Derive(c, target.IdValue, node.IdValue, "medium", format.IdValue, memory.IdValue, cooling.IdValue);
            string name = lab.FamilyName + " Silicon " + ChipQueries.NextVersion(lab);
            Decide(x, Command.StartChipDesign(lab.Id, target.IdValue, node.IdValue, "medium", format.IdValue, memory.IdValue, cooling.IdValue),
                DecisionAction.StartedChip, name, spec.Points,
                Reason.Of(ReasonCode.ChipArchitects, ChipQueries.Architects(c, lab)), Reason.Of(ReasonCode.DesignPointsNeeded, spec.Points), Runway(x));
        }

        static void TapeOutDesign(Ctx x)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            ChipDesign d = ChipQueries.DesignInProgress(lab);
            if (d == null || d.PointsDone < d.PointsRequired) return;
            ChipNodeConfig node = c.Chips.Node(d.NodeId);
            if (!CanSpend(x, node.TapeoutCost)) return;
            Decide(x, Command.TapeOut(lab.Id, d.Id), DecisionAction.TapedOut, d.Name, node.TapeoutCost,
                Reason.Of(ReasonCode.DesignPoints, d.PointsDone), Reason.Of(ReasonCode.DesignPointsNeeded, d.PointsRequired), Runway(x));
        }

        /// <summary>The holding with the most free power that the lab's own chips of this tier may go to, or null.</summary>
        static Holding RoomiestHolding(Ctx x, int tier, string designerHomeBlocId, int kwPerChip)
        {
            Holding best = null;
            int mostFree = 0;
            for (int i = 0; i < x.Lab.Holdings.Count; i++)
            {
                Holding h = x.Lab.Holdings[i];
                if (ChipQueries.PlaceProblem(x.C, x.State, x.Lab, h.Id, tier, designerHomeBlocId) != null) continue;
                int free = WorldQueries.FreeKw(x.C, x.State, x.Lab, h);
                if (free >= kwPerChip && (best == null || free > mostFree)) { best = h; mostFree = free; }
            }
            return best;
        }

        /// <summary>Once a design's yield is good enough: books half of the gap to the GPU target as the lab's own chips, in the best design it has.</summary>
        static void OrderOwnChips(Ctx x, int target)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            int gap = target - LabQueries.OwnedUnits(lab) - LabQueries.ChipsOnOrder(lab);
            if (gap < 2) return;

            ChipDesign best = null;
            ChipSpec bestSpec = new ChipSpec();
            for (int i = 0; i < lab.ChipDesigns.Count; i++)
            {
                ChipDesign d = lab.ChipDesigns[i];
                if (d.Status != ChipDesignStatus.Ready || d.YieldPermille < c.Chips.PlannerMinYieldPermille) continue;
                ChipSpec spec = ChipQueries.Spec(c, d);
                if (!spec.Valid) continue;
                if (best == null || ChipQueries.CostPerGoodChip(spec, d.YieldPermille) * bestSpec.Compute < ChipQueries.CostPerGoodChip(bestSpec, best.YieldPermille) * spec.Compute)
                {
                    best = d;
                    bestSpec = spec;
                }
            }
            if (best == null) return;

            Holding holding = RoomiestHolding(x, bestSpec.Tier, lab.HomeBlocId, bestSpec.Kw);
            if (holding == null) return;
            int count = Math.Min(gap / 2, WorldQueries.FreeKw(c, x.State, lab, holding) / bestSpec.Kw);
            long perChip = ChipQueries.OrderCost(bestSpec, best.YieldPermille, 1);
            long spare = lab.Cash - Reserve(x);
            if (perChip > 0) count = (int)Math.Min(count, spare / perChip);
            while (count >= 1)
            {
                int month = ChipQueries.FirstFreeMonth(c, x.State, c.Chips.Node(best.NodeId), count);
                if (month >= 0)
                {
                    Decide(x, Command.OrderChips(lab.Id, best.Id, holding.Id, count), DecisionAction.OrderedChips, best.Name, count,
                        Reason.Of(ReasonCode.YieldPermille, best.YieldPermille), Reason.Of(ReasonCode.FoundryMonth, month), Reason.Of(ReasonCode.GpuTargetGap, gap), Runway(x));
                    return;
                }
                count /= 2;
            }
        }

        /// <summary>Lists every working design at what a good chip costs the lab, plus the plan's mark-up. The price follows the yield.</summary>
        static void ListChips(Ctx x)
        {
            GameConfig c = x.C;
            LabState lab = x.Lab;
            for (int i = 0; i < lab.ChipDesigns.Count; i++)
            {
                ChipDesign d = lab.ChipDesigns[i];
                if (d.Status != ChipDesignStatus.Ready) continue;
                ChipSpec spec = ChipQueries.Spec(c, d);
                if (!spec.Valid) continue;
                long price = MathI.MulPermille(ChipQueries.CostPerGoodChip(spec, d.YieldPermille), MathI.Permille + x.Plan.ListsChipsMarkupPermille);
                if (price < 1 || price > int.MaxValue) continue;
                ChipListing listing = lab.Listing(d.Id);
                if (listing != null && listing.PricePerChip == price) continue;
                bool first = listing == null;
                CommandResult result = x.Apply(Command.SetChipListing(lab.Id, d.Id, (int)price, c.Chips.ListingMaxPerMonth));
                if (result.Ok && first)
                    DecisionLog.Add(c, lab, x.State.Tick, DecisionAction.ListedChips, d.Name, price, Reason.Of(ReasonCode.YieldPermille, d.YieldPermille));
            }
        }

        /// <summary>
        /// Buys other labs' listed chips when a chip costs no more per unit of compute than the vendor's GPU (less the plan's
        /// limit), up to the lab's GPU gap. It does not buy from a lab it is feuding with, nor in the weeks after the vendor's
        /// keynote upstaged its own chip.
        /// </summary>
        static void BuyLists(Ctx x, int target)
        {
            GameConfig c = x.C;
            GameState state = x.State;
            LabState lab = x.Lab;
            if (state.Tick < lab.ChipAmbushUntilTick) return;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            long vendorPrice = VendorSystem.PricePerGpu(c, state, lab, gen);

            for (int s = 0; s < state.Labs.Count; s++)
            {
                LabState seller = state.Labs[s];
                if (seller.Id == lab.Id || seller.Bankrupt) continue;
                bool toPlayer = seller.Id == state.Player.Id;
                if (toPlayer && RivalSystem.IsFeud(c, lab)) continue;
                int limit = toPlayer && RivalSystem.IsFriendly(c, lab) ? c.Rivalry.Feelings.FriendlyBuyLimitPermille : c.Rivalry.Feelings.ListedBuyLimitPermille;

                for (int l = 0; l < seller.ChipListings.Count; l++)
                {
                    ChipListing listing = seller.ChipListings[l];
                    ChipDesign d = seller.Design(listing.DesignId);
                    if (d == null || d.Status != ChipDesignStatus.Ready) continue;
                    ChipSpec spec = ChipQueries.Spec(c, d);
                    long effective = MathI.MulPermille(spec.Compute, ChipQueries.EffectiveSoftware(c, seller));
                    if (!spec.Valid || effective < 1) continue;
                    if (listing.PricePerChip * gen.ComputePerGpuWeek * MathI.Permille > vendorPrice * effective * limit) continue;

                    int gap = target - LabQueries.OwnedUnits(lab) - LabQueries.ChipsOnOrder(lab);
                    Holding holding = RoomiestHolding(x, spec.Tier, seller.HomeBlocId, spec.Kw);
                    if (gap < 1 || holding == null) continue;
                    int count = Math.Min(Math.Min(gap, listing.MaxPerMonth - listing.SoldThisMonth), WorldQueries.FreeKw(c, state, lab, holding) / spec.Kw);
                    long spare = lab.Cash - Reserve(x);
                    if (listing.PricePerChip > 0) count = (int)Math.Min(count, spare / listing.PricePerChip);
                    if (count < 1) continue;

                    long ppc = listing.PricePerChip * gen.ComputePerGpuWeek * MathI.Permille / (vendorPrice * effective);
                    Decide(x, Command.BuyListedChips(lab.Id, seller.Id, d.Id, holding.Id, count), DecisionAction.BoughtChips, d.Name, count,
                        Reason.Of(ReasonCode.PricePerComputePermille, ppc), Reason.Of(ReasonCode.GpuTargetGap, gap), Runway(x));
                }
            }
        }
    }
}
