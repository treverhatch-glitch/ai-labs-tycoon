using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    /// <summary>The commands of the chip programme: designing, taping out, ordering and listing chips, and buying a listing.</summary>
    public static partial class CommandProcessor
    {
        static readonly ChipOptionGroup[] OptionGroups = { ChipOptionGroup.Die, ChipOptionGroup.Format, ChipOptionGroup.Memory, ChipOptionGroup.Cooling };

        static CommandResult StartChipDesign(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (state.Tick < lab.ChipAmbushUntilTick)
                return Fail(CommandError.KeynoteAmbush, "Envydia's keynote buried your launch. Wait out the ambush before starting a new design.");
            ChipsConfig k = c.Chips;
            if (cmd.Ids == null || cmd.Ids.Count != 5) return Fail(CommandError.InvalidArgument, "A design needs a process, a die, a number format, a memory and a cooling.");
            ChipTargetConfig target = k.Target(cmd.Id);
            if (target == null) return Fail(CommandError.UnknownId, "There is no chip target \"" + cmd.Id + "\".");
            if (!ChipQueries.TargetUnlocked(c, lab, target.IdValue))
                return Fail(CommandError.MissingPrerequisite, "The lab has not researched how to design a " + target.Name.ToLowerInvariant() + " yet.");
            ChipNodeConfig node = k.Node(cmd.Ids[0]);
            if (node == null) return Fail(CommandError.UnknownId, "There is no process \"" + cmd.Ids[0] + "\".");
            if (!ChipQueries.NodeOpen(c, node, state.Tick)) return Fail(CommandError.EraLocked, node.Name + " opens in " + node.OpensYear + ".");
            for (int g = 0; g < OptionGroups.Length; g++)
            {
                ChipOptionConfig option = k.Option(OptionGroups[g], cmd.Ids[g + 1]);
                if (option == null) return Fail(CommandError.UnknownId, "There is no " + OptionGroups[g].ToString().ToLowerInvariant() + " option \"" + cmd.Ids[g + 1] + "\".");
                if (!ChipQueries.OptionAvailable(lab, option))
                    return Fail(CommandError.MissingPrerequisite, option.Name + " needs " + c.Tech(option.RequiresTech).Name + " first.");
            }
            if (ChipQueries.DesignInProgress(lab) != null) return Fail(CommandError.InvalidArgument, "The lab is already designing a chip. Finish it or cancel it first.");
            if (ChipQueries.Architects(c, lab) < 1) return Fail(CommandError.NotEnoughStaff, "A design needs at least one chip architect.");

            ChipSpec spec = ChipQueries.Derive(c, target.IdValue, node.IdValue, cmd.Ids[1], cmd.Ids[2], cmd.Ids[3], cmd.Ids[4]);
            var d = new ChipDesign();
            d.Id = state.TakeId();
            d.Version = ChipQueries.NextVersion(lab);
            d.Name = lab.FamilyName + " Silicon " + d.Version;
            d.Target = target.IdValue;
            d.NodeId = node.IdValue;
            d.DieId = cmd.Ids[1];
            d.FormatId = cmd.Ids[2];
            d.MemoryId = cmd.Ids[3];
            d.CoolingId = cmd.Ids[4];
            d.PointsRequired = spec.Points;
            d.Status = ChipDesignStatus.Designing;
            d.StageEndsTick = -1;
            d.ReadyTick = -1;
            lab.ChipDesigns.Add(d);
            if (lab.SoftwarePermille == 0) lab.SoftwarePermille = k.SoftwareStart;
            return CommandResult.Done(d.Id);
        }

        static CommandResult CancelChipDesign(LabState lab, Command cmd)
        {
            ChipDesign d = lab.Design(cmd.TargetId);
            if (d == null) return Fail(CommandError.UnknownId, "The lab has no design " + cmd.TargetId + ".");
            if (d.Status != ChipDesignStatus.Designing) return Fail(CommandError.InvalidArgument, d.Name + " is past the design stage.");
            d.Status = ChipDesignStatus.Cancelled;
            return CommandResult.Done(-1);
        }

        static CommandResult TapeOut(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ChipDesign d = lab.Design(cmd.TargetId);
            if (d == null) return Fail(CommandError.UnknownId, "The lab has no design " + cmd.TargetId + ".");
            if (d.Status != ChipDesignStatus.Designing) return Fail(CommandError.InvalidArgument, d.Name + " is not being designed.");
            if (d.PointsDone < d.PointsRequired)
                return Fail(CommandError.DesignNotReady, d.Name + " has " + d.PointsDone + " of " + d.PointsRequired + " design points.");
            ChipNodeConfig node = c.Chips.Node(d.NodeId);
            if (lab.Cash < node.TapeoutCost) return NeedCash(lab, node.TapeoutCost);

            Finance.Spend(lab, LedgerLine.ChipDevelopment, node.TapeoutCost);
            d.Status = ChipDesignStatus.TapingOut;
            d.StageEndsTick = state.Tick + c.Chips.TapeoutWeeks;
            Events.Add(state, EventKind.ChipTapedOut, lab.Id, d.Id, node.TapeoutCost, d.Name);
            return CommandResult.Done(-1);
        }

        /// <summary>The checks both ways of getting chips share, and the booking itself. Chips are paid for by the caller.</summary>
        static CommandResult CheckChipDelivery(GameConfig c, GameState state, LabState buyer, LabState designer, ChipDesign d, int holdingId, int count, out ChipSpec spec, out int month, out Holding holding)
        {
            spec = ChipQueries.Spec(c, d);
            month = -1;
            holding = buyer.Holding(holdingId);
            if (count < 1) return Fail(CommandError.InvalidArgument, "Order at least one chip.");
            if (ChipQueries.CloudDelivery(holdingId))
            {
                if (buyer.Holdings.Count > 0)
                    return Fail(CommandError.InvalidArgument, "Cloud chip deals are for labs that have no data centres.");
                string cloudProblem = ChipQueries.CloudPlaceProblem(c, state, buyer, spec.Tier, designer.HomeBlocId);
                if (cloudProblem == "frozen") return FrozenIn(c, buyer, buyer.HomeBlocId);
                if (cloudProblem == "export")
                    return Fail(CommandError.ExportBlocked, "Export rules keep chips made on " + c.Chips.Node(d.NodeId).Name + " out of " + c.World.Bloc(buyer.HomeBlocId).Name + ".");
            }
            else
            {
                if (holding == null) return Fail(CommandError.UnknownId, "The lab has no holding " + holdingId + ". Chips are delivered to a data centre.");
                string problem = ChipQueries.PlaceProblem(c, state, buyer, holdingId, spec.Tier, designer.HomeBlocId);
                if (problem == "frozen") return FrozenIn(c, buyer, WorldQueries.BlocOfPlace(c, buyer, holdingId));
                if (problem == "export")
                    return Fail(CommandError.ExportBlocked, "Export rules keep chips made on " + c.Chips.Node(d.NodeId).Name + " out of " + c.World.Bloc(WorldQueries.BlocOfPlace(c, buyer, holdingId)).Name + ".");
                int freeKw = WorldQueries.FreeKw(c, state, buyer, holding);
                if ((long)count * spec.Kw > freeKw)
                    return Fail(CommandError.NotEnoughPower, "That needs " + ((long)count * spec.Kw) + " kW and the hall has " + (freeKw < 0 ? 0 : freeKw) + " kW free.");
            }
            month = ChipQueries.FirstFreeMonth(c, state, c.Chips.Node(d.NodeId), d, count);
            if (month < 0) return Fail(CommandError.FoundryFull, "The foundry has no room for " + count + " chips in the next " + c.Chips.BookAheadMonths + " months.");
            return CommandResult.Done(-1);
        }

        static ChipOrder PlaceChipOrder(GameConfig c, GameState state, LabState buyer, LabState designer, ChipDesign d, ChipSpec spec, Holding holding, int count, int month)
        {
            ChipNodeConfig node = c.Chips.Node(d.NodeId);
            var order = new ChipOrder();
            order.Id = state.TakeId();
            order.DesignId = d.Id;
            order.DesignLabId = designer.Id;
            order.BuyerLabId = buyer.Id;
            order.HoldingId = holding != null ? holding.Id : HardwareBatch.Cloud;
            order.Count = count;
            order.DeliveryTick = ChipQueries.DeliveryTickOf(c, node, month);
            order.ReservedKw = order.HoldingId == HardwareBatch.Cloud ? 0 : count * spec.Kw;
            order.FoundryMonth = month;
            order.NodeId = node.IdValue;
            buyer.ChipOrders.Add(order);
            ChipSystem.Book(state, node.IdValue, month, ChipQueries.FoundrySlots(c, d, count));
            return order;
        }

        static CommandResult OrderChips(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ChipDesign d = lab.Design(cmd.TargetId);
            if (d == null) return Fail(CommandError.UnknownId, "The lab has no design " + cmd.TargetId + ".");
            if (d.Status != ChipDesignStatus.Ready) return Fail(CommandError.DesignNotReady, d.Name + " has not come back from the foundry yet.");
            if (state.Tick < lab.ChipAmbushUntilTick)
                return Fail(CommandError.KeynoteAmbush, "Envydia's keynote buried your launch. Wait out the ambush before ordering chips.");
            ChipSpec spec;
            int month;
            Holding holding;
            CommandResult check = CheckChipDelivery(c, state, lab, lab, d, cmd.X, cmd.Count, out spec, out month, out holding);
            if (!check.Ok) return check;
            long cost = ChipShortageSystem.ScalePrice(state, ChipQueries.OrderCost(spec, d.YieldPermille, cmd.Count));
            if (lab.Cash < cost) return NeedCash(lab, cost);

            ChipOrder order = PlaceChipOrder(c, state, lab, lab, d, spec, holding, cmd.Count, month);
            Finance.Spend(lab, LedgerLine.ChipDevelopment, cost);
            return CommandResult.Done(order.Id);
        }

        static CommandResult SetChipListing(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ChipDesign d = lab.Design(cmd.TargetId);
            if (d == null) return Fail(CommandError.UnknownId, "The lab has no design " + cmd.TargetId + ".");
            if (d.Status != ChipDesignStatus.Ready) return Fail(CommandError.DesignNotReady, d.Name + " has not come back from the foundry yet.");
            if (cmd.Count > 0 && state.Tick < lab.ChipAmbushUntilTick)
                return Fail(CommandError.KeynoteAmbush, "Envydia's keynote buried your launch. Wait out the ambush before listing chips.");
            ChipListing listing = lab.Listing(d.Id);
            if (cmd.Count < 0 || cmd.X < 0) return Fail(CommandError.InvalidArgument, "A price and a monthly limit cannot be negative.");
            if (cmd.Count == 0)
            {
                if (listing == null) return Fail(CommandError.InvalidArgument, d.Name + " is not listed.");
                lab.ChipListings.Remove(listing);
                return CommandResult.Done(-1);
            }
            if (cmd.X < 1) return Fail(CommandError.InvalidArgument, "List at least one chip a month.");
            if (listing == null)
            {
                listing = new ChipListing();
                listing.DesignId = d.Id;
                lab.ChipListings.Add(listing);
            }
            listing.PricePerChip = cmd.Count;
            listing.MaxPerMonth = cmd.X;

            VendorStanding standing = VendorSystem.EnsureStanding(c, state, lab.Id);
            if (!standing.ListingSeen)
            {
                standing.ListingSeen = true;
                VendorSystem.Change(c, state, lab.Id, -c.Vendor.ListingDrop, true);
            }
            return CommandResult.Done(-1);
        }

        static CommandResult BuyListedChips(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            LabState seller = state.Lab(cmd.TargetId);
            if (seller == null || seller.Id == lab.Id) return Fail(CommandError.UnknownLab, "There is no other lab " + cmd.TargetId + ".");
            if (seller.Bankrupt) return Fail(CommandError.LabBankrupt, seller.Name + " is bankrupt.");
            ChipListing listing = seller.Listing(cmd.Y);
            ChipDesign d = seller.Design(cmd.Y);
            if (listing == null || d == null || d.Status != ChipDesignStatus.Ready) return Fail(CommandError.UnknownId, seller.Name + " has no such chips on offer.");
            if (cmd.Count < 1) return Fail(CommandError.InvalidArgument, "Buy at least one chip.");
            if (state.Tick < lab.ChipAmbushUntilTick)
                return Fail(CommandError.KeynoteAmbush, "Envydia's keynote buried your launch. Wait out the ambush before buying listed chips.");
            if (listing.SoldThisMonth + cmd.Count > listing.MaxPerMonth)
                return Fail(CommandError.AllocationExhausted, seller.Name + " will sell " + (listing.MaxPerMonth - listing.SoldThisMonth) + " more of these this month.");

            ChipSpec spec;
            int month;
            Holding holding;
            CommandResult check = CheckChipDelivery(c, state, lab, seller, d, cmd.X, cmd.Count, out spec, out month, out holding);
            if (!check.Ok) return check;
            long price = ChipShortageSystem.ScalePrice(state, listing.PricePerChip * cmd.Count);
            if (lab.Cash < price) return NeedCash(lab, price);
            long wafers = ChipQueries.OrderCost(spec, d.YieldPermille, cmd.Count);
            if (seller.Cash < wafers) return Fail(CommandError.NotEnoughCash, seller.Name + " cannot pay the foundry for that many chips right now.");

            ChipOrder order = PlaceChipOrder(c, state, lab, seller, d, spec, holding, cmd.Count, month);
            listing.SoldThisMonth += cmd.Count;
            Finance.Spend(lab, LedgerLine.Hardware, price);
            Finance.Earn(seller, LedgerLine.ChipSales, price);
            Finance.Spend(seller, LedgerLine.ChipDevelopment, ChipQueries.OrderCost(spec, d.YieldPermille, cmd.Count));
            Events.Add(state, EventKind.ChipsBought, lab.Id, seller.Id, cmd.Count, d.Name);

            // A trade with the player warms a rival up to them, whichever way the chips go.
            LabState rival = lab.Id == state.Player.Id ? seller : seller.Id == state.Player.Id ? lab : null;
            if (rival != null) RivalSystem.Feel(c, rival, c.Rivalry.Feelings.ChipOrderPermille);
            return CommandResult.Done(order.Id);
        }
    }
}
