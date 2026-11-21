using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// A lab's own chips from design to delivery: design points, software, tape-out and respins, the yield ramp, the
    /// foundry's booked months, and the vendor's reactions. The commands that start things are in the command processor.
    /// </summary>
    public static class ChipSystem
    {
        /// <summary>Runs once a week for a lab that is still trading, after research and before training.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            ChipsConfig k = c.Chips;
            int architects = ChipQueries.Architects(c, lab);

            ChipDesign designing = ChipQueries.DesignInProgress(lab);
            if (designing != null && architects > 0)
            {
                designing.PointsDone = MathI.Min(designing.PointsRequired, designing.PointsDone + (long)architects * k.PointsPerArchitectWeek);
            }

            if (lab.SoftwarePermille > 0 && architects > 0 && lab.SoftwarePermille < k.SoftwareBaseCap)
            {
                long rate = (long)architects * k.SoftwarePerArchitectWeek;
                long gain = rate * (state.Tick + 1) / 10 - rate * state.Tick / 10;
                lab.SoftwarePermille = (int)MathI.Min(k.SoftwareBaseCap, lab.SoftwarePermille + gain);
            }

            RampYields(c, lab);
            ResolveTapeOuts(c, state, lab);
            Deliver(c, state, lab);
        }

        static void RampYields(GameConfig c, LabState lab)
        {
            ChipsConfig k = c.Chips;
            long engineers = MathI.Min(ChipQueries.Engineers(c, lab), k.YieldRampEngineerCap);
            long ramp = k.YieldRampPerWeek + engineers * k.YieldRampPerEngineer;
            if (ChipQueries.HasTestLab(c, lab)) ramp = MathI.MulPermille(ramp, k.TestLabRampPermille);
            for (int i = 0; i < lab.ChipDesigns.Count; i++)
            {
                ChipDesign d = lab.ChipDesigns[i];
                if (d.Status != ChipDesignStatus.Ready) continue;
                ChipSpec spec = ChipQueries.Spec(c, d);
                if (!spec.Valid || d.YieldPermille >= spec.MaxYieldPermille) continue;
                d.YieldPermille = (int)MathI.Min(spec.MaxYieldPermille, d.YieldPermille + ramp);
            }
        }

        static void ResolveTapeOuts(GameConfig c, GameState state, LabState lab)
        {
            ChipsConfig k = c.Chips;
            for (int i = 0; i < lab.ChipDesigns.Count; i++)
            {
                ChipDesign d = lab.ChipDesigns[i];
                if (d.Status != ChipDesignStatus.TapingOut || state.Tick < d.StageEndsTick) continue;
                ChipSpec spec = ChipQueries.Spec(c, d);
                ChipNodeConfig node = k.Node(d.NodeId);
                if (!spec.Valid || node == null) { d.Status = ChipDesignStatus.Cancelled; continue; }

                if (state.Rng.Chips.Chance(ChipQueries.RespinChancePermille(c, lab, node)))
                {
                    long cost = ChipQueries.RespinCost(c, node);
                    Finance.Spend(lab, LedgerLine.ChipDevelopment, cost);
                    d.StageEndsTick += k.RespinWeeks;
                    d.Respins++;
                    Events.Add(state, EventKind.ChipRespin, lab.Id, d.Id, cost, d.Name);
                    continue;
                }

                d.Status = ChipDesignStatus.Ready;
                d.YieldPermille = spec.InitialYieldPermille;
                d.ReadyTick = state.Tick;
                Events.Add(state, EventKind.ChipReady, lab.Id, d.Id, d.YieldPermille, d.Name).Pick = d.Version == 1 ? 1 : 0;
                if (spec.Trains)
                {
                    VendorStanding standing = VendorSystem.EnsureStanding(c, state, lab.Id);
                    if (!standing.TrainingChipSeen)
                    {
                        standing.TrainingChipSeen = true;
                        VendorSystem.Change(c, state, lab.Id, -c.Vendor.TrainingTapeOutDrop, true);
                    }
                }
            }
        }

        static void Deliver(GameConfig c, GameState state, LabState lab)
        {
            for (int i = 0; i < lab.ChipOrders.Count; i++)
            {
                ChipOrder order = lab.ChipOrders[i];
                if (state.Tick < order.DeliveryTick) continue;
                lab.ChipOrders.RemoveAt(i);
                i--;

                bool cloud = ChipQueries.CloudDelivery(order.HoldingId);
                Holding holding = cloud ? null : lab.Holding(order.HoldingId);
                LabState designer = state.Lab(order.DesignLabId);
                if ((!cloud && holding == null) || designer == null || designer.Design(order.DesignId) == null) continue;

                int count = order.Count;
                if (state.Rng.Chips.Chance(c.Chips.YieldCrashPermille))
                {
                    count = (int)MathI.Max(1, MathI.MulPermille(order.Count, c.Chips.YieldCrashKeepPermille));
                    Events.Add(state, EventKind.ChipYieldCrash, lab.Id, order.DesignId, count, designer.Design(order.DesignId).Name);
                }
                AddChips(state, lab, order.HoldingId, order.DesignLabId, order.DesignId, count);
                VendorSystem.Change(c, state, order.DesignLabId, -c.Vendor.DeliveryDrop, true);
                Events.Add(state, EventKind.ChipsDelivered, lab.Id, order.DesignId, count, cloud ? "cloud" : holding.SiteId);
            }
        }

        public static HardwareBatch AddChips(GameState state, LabState lab, int holdingId, int designLabId, int designId, int count)
        {
            for (int i = 0; i < lab.Fleet.Count; i++)
            {
                HardwareBatch b = lab.Fleet[i];
                if (b.HoldingId == holdingId && b.ChipDesignId == designId && b.DesignLabId == designLabId)
                {
                    b.Count += count;
                    return b;
                }
            }
            var batch = new HardwareBatch();
            batch.Id = state.TakeId();
            batch.HoldingId = holdingId;
            batch.GpuGenId = "";
            batch.ChipDesignId = designId;
            batch.DesignLabId = designLabId;
            batch.Count = count;
            batch.ArrivedTick = state.Tick;
            lab.Fleet.Add(batch);
            return batch;
        }

        /// <summary>Books chips in the foundry's month. The month's total is the sum of every lab's orders.</summary>
        public static void Book(GameState state, string nodeId, int monthIndex, int count)
        {
            for (int i = 0; i < state.Foundry.Count; i++)
            {
                FoundryMonth m = state.Foundry[i];
                if (m.NodeId == nodeId && m.MonthIndex == monthIndex) { m.Booked += count; return; }
            }
            var month = new FoundryMonth();
            month.NodeId = nodeId;
            month.MonthIndex = monthIndex;
            month.Booked = count;
            state.Foundry.Add(month);
        }

        public static void Unbook(GameState state, string nodeId, int monthIndex, int count)
        {
            for (int i = 0; i < state.Foundry.Count; i++)
            {
                FoundryMonth m = state.Foundry[i];
                if (m.NodeId != nodeId || m.MonthIndex != monthIndex) continue;
                m.Booked -= count;
                if (m.Booked <= 0) state.Foundry.RemoveAt(i);
                return;
            }
        }

        /// <summary>Runs on the first week of every month, before the labs plan.</summary>
        public static void RunMonth(GameConfig c, GameState state)
        {
            int now = ChipQueries.MonthIndex(c, state.Tick);
            state.Foundry.RemoveAll(m => m.MonthIndex < now);
            for (int l = 0; l < state.Labs.Count; l++)
                for (int i = 0; i < state.Labs[l].ChipListings.Count; i++) state.Labs[l].ChipListings[i].SoldThisMonth = 0;
        }

        /// <summary>A lab that closes loses its orders, its listings and the foundry room its orders held.</summary>
        public static void OnBankrupt(GameConfig c, GameState state, LabState lab)
        {
            for (int i = 0; i < lab.ChipOrders.Count; i++)
                Unbook(state, lab.ChipOrders[i].NodeId, lab.ChipOrders[i].FoundryMonth,
                    ChipQueries.OrderFoundrySlots(c, state, lab.ChipOrders[i]));
            lab.ChipOrders.Clear();
            lab.ChipListings.Clear();
        }

        /// <summary>The vendor's yearly keynote upstages the chip of a lab whose design only just came out.</summary>
        public static void OnKeynote(GameConfig c, GameState state)
        {
            for (int l = 0; l < state.Labs.Count; l++)
            {
                LabState lab = state.Labs[l];
                if (lab.Bankrupt) continue;
                for (int i = 0; i < lab.ChipDesigns.Count; i++)
                {
                    ChipDesign d = lab.ChipDesigns[i];
                    if (d.Status != ChipDesignStatus.Ready || state.Tick - d.ReadyTick > c.Vendor.AmbushLookbackWeeks) continue;
                    lab.ChipAmbushUntilTick = state.Tick + c.Vendor.AmbushWeeks;
                }
            }
        }

        /// <summary>The vendor courts every other lab while one lab runs a large share of its general compute on its own chips.</summary>
        public static void CourtRivals(GameConfig c, GameState state)
        {
            for (int l = 0; l < state.Labs.Count; l++)
            {
                LabState lab = state.Labs[l];
                if (lab.Bankrupt) continue;
                int share = ChipQueries.OwnChipsShare(c, state, lab);
                if (share < c.Vendor.CourtingSharePermille) continue;
                for (int o = 0; o < state.Labs.Count; o++)
                    if (o != l && !state.Labs[o].Bankrupt) VendorSystem.Change(c, state, state.Labs[o].Id, c.Vendor.CourtingStandingBoost, false);
                if (!VendorLoggedThisQuarter(c, state, DecisionAction.Courted, lab.Id.ToString()))
                {
                    DecisionLog.AddVendor(c, state, DecisionAction.Courted, lab.Id.ToString(), c.Vendor.CourtingStandingBoost,
                        Reason.Of(ReasonCode.ChipsOwnedSharePermille, share));
                    if (c.Vendor.CourtingMakerAttentionPermille > 0)
                    {
                        BlocStanding home = lab.Bloc(lab.HomeBlocId);
                        if (home != null)
                        {
                            int before = home.Attention;
                            home.Attention = (int)MathI.Min(MathI.Permille, home.Attention + c.Vendor.CourtingMakerAttentionPermille);
                            if (home.Attention > before)
                                Events.Add(state, EventKind.AttentionRose, lab.Id, -1, home.Attention - before, lab.HomeBlocId);
                        }
                    }
                }
            }
        }

        static bool VendorLoggedThisQuarter(GameConfig c, GameState state, DecisionAction action, string reference)
        {
            int quarter = DecisionLog.QuarterOf(c, state.Tick);
            List<DecisionEntry> log = state.Vendor.Log;
            for (int i = log.Count - 1; i >= 0; i--)
            {
                if (DecisionLog.QuarterOf(c, log[i].Tick) != quarter) return false;
                if (log[i].Action == action && log[i].Ref == reference) return true;
            }
            return false;
        }
    }
}
