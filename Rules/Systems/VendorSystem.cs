using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The GPU vendor as an actor. It makes a fixed number of units a month, hands each lab a share by its standing, charges
    /// a price that depends on that standing, and holds a keynote when a new generation opens. It has no cash and no models.
    /// </summary>
    public static class VendorSystem
    {
        public static VendorState NewState(GameConfig c, GameState state)
        {
            var vendor = new VendorState();
            state.Vendor = vendor;
            for (int i = 0; i < state.Labs.Count; i++) EnsureStanding(c, state, state.Labs[i].Id);
            vendor.SupplyThisMonth = c.Vendor.SupplyInYear(GameDate.Year(c.Calendar, state.Tick));
            RunAllocation(c, state, false);
            return vendor;
        }

        public static VendorStanding EnsureStanding(GameConfig c, GameState state, int labId)
        {
            VendorStanding standing = state.Vendor.Standing(labId);
            if (standing != null) return standing;
            standing = new VendorStanding();
            standing.LabId = labId;
            standing.Standing = c.Vendor.StandingStart;
            state.Vendor.Standings.Add(standing);
            return standing;
        }

        /// <summary>Units the lab may still buy this month.</summary>
        public static int AllocationLeft(GameState state, LabState lab)
        {
            VendorStanding standing = state.Vendor.Standing(lab.Id);
            if (standing == null) return 0;
            int left = standing.AllocationThisMonth - standing.BoughtThisMonth;
            return left < 0 ? 0 : left;
        }

        /// <summary>What the vendor charges this lab, in permille of the list price.</summary>
        public static int PricePermille(GameConfig c, GameState state, LabState lab)
        {
            VendorConfig v = c.Vendor;
            int standing = StandingOf(state, lab);
            int price = MathI.Permille;
            if (standing < v.LowStandingBelow) price += v.LowStandingMarkupPermille;
            if (standing > v.HighStandingAbove) price -= v.HighStandingDiscountPermille;
            if (ChipQueries.RunsOwnTrainingChips(c, state, lab)) price += v.OwnChipsMarkupPermille;
            return price;
        }

        public static long PricePerGpu(GameConfig c, GameState state, LabState lab, GpuGenerationConfig gen)
        {
            return MathI.MulPermille(gen.BuyPrice, PricePermille(c, state, lab));
        }

        public static int StandingOf(GameState state, LabState lab)
        {
            VendorStanding standing = state.Vendor.Standing(lab.Id);
            return standing == null ? 0 : standing.Standing;
        }

        /// <summary>Records a purchase: it counts against the month's allocation, and every few GPUs earn a point of standing.</summary>
        public static void OnBought(GameConfig c, GameState state, LabState lab, int count)
        {
            VendorStanding standing = EnsureStanding(c, state, lab.Id);
            int before = standing.BoughtThisMonth;
            standing.BoughtThisMonth += count;
            int gain = standing.BoughtThisMonth / c.Vendor.GpusPerStanding - before / c.Vendor.GpusPerStanding;
            Change(c, state, lab.Id, gain, false);
        }

        /// <summary>Moves a lab's standing, keeping it from 0 to the maximum, and notes whether it came from the lab's own chips.</summary>
        public static void Change(GameConfig c, GameState state, int labId, int delta, bool fromChips)
        {
            if (delta == 0) return;
            VendorStanding standing = EnsureStanding(c, state, labId);
            int next = MathI.Clamp(standing.Standing + delta, 0, c.Vendor.StandingMax);
            int moved = next - standing.Standing;
            standing.Standing = next;
            if (fromChips) standing.FromChips += moved; else standing.FromBuying += moved;
        }

        /// <summary>The generation that opens in this year, or null.</summary>
        public static GpuGenerationConfig OpeningGeneration(GameConfig c, int year)
        {
            for (int i = 0; i < c.Compute.GpuGenerations.Count; i++)
                if (c.Compute.GpuGenerations[i].OpensYear == year) return c.Compute.GpuGenerations[i];
            return null;
        }

        /// <summary>Called in the first week of every month, before the labs plan.</summary>
        public static void RunMonth(GameConfig c, GameState state)
        {
            VendorConfig v = c.Vendor;
            int year = GameDate.Year(c.Calendar, state.Tick);
            int month = GameDate.Month(c.Calendar, state.Tick);

            for (int i = 0; i < state.Labs.Count; i++) EnsureStanding(c, state, state.Labs[i].Id);
            for (int i = 0; i < state.Vendor.Standings.Count; i++)
            {
                VendorStanding s = state.Vendor.Standings[i];
                if (s.Standing > v.StandingStart) s.Standing = (int)MathI.Max(v.StandingStart, s.Standing - v.DriftPerMonth);
                else if (s.Standing < v.StandingStart) s.Standing = (int)MathI.Min(v.StandingStart, s.Standing + v.DriftPerMonth);
            }

            ChipSystem.CourtRivals(c, state);

            int supply = v.SupplyInYear(year);
            GpuGenerationConfig opening = OpeningGeneration(c, year);
            if (opening != null && month == v.KeynoteMonth)
            {
                supply = (int)MathI.MulPermille(supply, v.KeynoteSupplyPermille);
                Events.Add(state, EventKind.Keynote, -1, -1, supply, opening.Id);
                ChipSystem.OnKeynote(c, state);
            }
            state.Vendor.SupplyThisMonth = supply;

            RunAllocation(c, state, true);
            ComputeContractSystem.FulfillQueues(c, state);
        }

        /// <summary>
        /// Splits the month's supply: every active lab gets the supply times (base + standing) over the sum of those, and
        /// what rounding leaves over goes to the lab with the highest standing (ties to the older lab). A move of a lab's share
        /// by the configured amount since last month gets a line in the vendor's log.
        /// </summary>
        static void RunAllocation(GameConfig c, GameState state, bool log)
        {
            VendorConfig v = c.Vendor;
            VendorState vendor = state.Vendor;
            long total = 0;
            for (int i = 0; i < state.Labs.Count; i++)
                if (!state.Labs[i].Bankrupt) total += v.AllocationBase + vendor.Standing(state.Labs[i].Id).Standing;

            long given = 0;
            VendorStanding top = null;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                VendorStanding s = vendor.Standing(state.Labs[i].Id);
                if (state.Labs[i].Bankrupt) { s.AllocationThisMonth = 0; continue; }
                s.AllocationThisMonth = (int)((long)vendor.SupplyThisMonth * (v.AllocationBase + s.Standing) / total);
                given += s.AllocationThisMonth;
                if (top == null || s.Standing > top.Standing || (s.Standing == top.Standing && s.LabId < top.LabId)) top = s;
            }
            if (top != null) top.AllocationThisMonth += (int)(vendor.SupplyThisMonth - given);

            for (int i = 0; i < state.Labs.Count; i++)
            {
                VendorStanding s = vendor.Standing(state.Labs[i].Id);
                int share = vendor.SupplyThisMonth == 0 ? 0 : (int)((long)s.AllocationThisMonth * MathI.Permille / vendor.SupplyThisMonth);
                int before = s.SharePermille;
                if (log && before > 0 && Abs(share - before) * MathI.Permille >= (long)before * v.ShareLogPermille)
                {
                    var reasons = new System.Collections.Generic.List<Reason>();
                    reasons.Add(Reason.Of(ReasonCode.ShareNowPermille, share));
                    reasons.Add(Reason.Of(ReasonCode.SharePreviousPermille, before));
                    reasons.Add(Reason.Of(ReasonCode.StandingNow, s.Standing));
                    if (s.FromBuying != 0) reasons.Add(Reason.Of(ReasonCode.StandingFromBuying, s.FromBuying));
                    if (s.FromChips != 0) reasons.Add(Reason.Of(ReasonCode.StandingFromChips, s.FromChips));
                    DecisionLog.AddVendor(c, state, DecisionAction.ShiftedAllocation, s.LabId.ToString(), s.AllocationThisMonth, reasons.ToArray());
                }
                s.SharePermille = share;
                if (log)
                {
                    s.BoughtThisMonth = 0;
                    s.FromBuying = 0;
                    s.FromChips = 0;
                }
            }
        }

        static int Abs(int x) { return x < 0 ? -x : x; }
    }
}
