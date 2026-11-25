using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>GPUs sold and leased back. See <see cref="LeasebackConfig"/>. Only the player's lab.</summary>
    public static class Leaseback
    {
        public static bool Enabled(GameConfig c) { return c.Leaseback.Enabled > 0; }

        public static bool Leasing(LabState lab) { return lab.LeaseUnits > 0; }

        public static int Free(LabState lab)
        {
            int free = LabQueries.OwnedGpuUnits(lab, HardwareBatch.Hq) - lab.LeaseUnits;
            return free < 0 ? 0 : free;
        }

        /// <summary>What the oldest units at the head office would fetch today.</summary>
        public static long ValueOf(GameConfig c, GameState state, LabState lab, int count)
        {
            long value = 0;
            int left = count;
            foreach (HardwareBatch batch in Fleet.OldestFirst(c, lab, HardwareBatch.Hq))
            {
                if (left == 0) break;
                int take = left < batch.Count ? left : batch.Count;
                value += take * LabQueries.ResalePerUnit(c, c.Compute.Generation(batch.GpuGenId), state.Tick);
                left -= take;
            }
            return value;
        }

        public static long Advance(GameConfig c, long value) { return MathI.MulPermille(value, c.Leaseback.AdvancePermille); }

        public static void Sell(GameConfig c, GameState state, LabState lab, int count)
        {
            long value = ValueOf(c, state, lab, count);
            long advance = Advance(c, value);
            lab.LeaseUnits = count;
            lab.LeaseValue = value;
            lab.LeaseRent = MathI.MulPermille(value, c.Leaseback.RentPermille);
            lab.LeaseBuyback = MathI.MulPermille(value, c.Leaseback.BuybackPermille);
            lab.LeaseLeft = c.Leaseback.TermWeeks;
            lab.LeaseMissed = 0;
            lab.LeaseExtended = 0;
            Finance.Earn(lab, LedgerLine.CapitalIn, advance);
            Events.Add(state, EventKind.GpusLeasedBack, lab.Id, count, advance, "");
        }

        static void Clear(LabState lab)
        {
            lab.LeaseUnits = 0;
            lab.LeaseValue = 0;
            lab.LeaseRent = 0;
            lab.LeaseBuyback = 0;
            lab.LeaseLeft = 0;
            lab.LeaseMissed = 0;
            lab.LeaseExtended = 0;
        }

        public static void BuyBack(GameConfig c, GameState state, LabState lab)
        {
            long price = lab.LeaseBuyback;
            Finance.Spend(lab, LedgerLine.CapitalIn, price);
            Clear(lab);
            Events.Add(state, EventKind.LeaseBoughtBack, lab.Id, -1, price, "");
        }

        public static void Extend(GameConfig c, GameState state, LabState lab)
        {
            lab.LeaseLeft += c.Leaseback.ExtendWeeks;
            lab.LeaseRent = MathI.MulPermille(lab.LeaseRent, c.Leaseback.ExtendRentPermille);
            lab.LeaseExtended = 1;
            Events.Add(state, EventKind.LeaseExtended, lab.Id, -1, lab.LeaseRent, "");
        }

        /// <summary>The units go back to the lessor, the oldest first, and the lease ends.</summary>
        public static void GiveBack(GameConfig c, GameState state, LabState lab, int pick)
        {
            int units = lab.LeaseUnits;
            int left = units;
            foreach (HardwareBatch batch in Fleet.OldestFirst(c, lab, HardwareBatch.Hq))
            {
                if (left == 0) break;
                int take = left < batch.Count ? left : batch.Count;
                Fleet.Take(lab, batch, take);
                left -= take;
            }
            Clear(lab);
            if (pick == 2) PressSystem.AddReputation(lab, -PressSystem.Damp(c, lab, c.Leaseback.RepoReputation));
            Events.Add(state, EventKind.LeaseEnded, lab.Id, -1, units - left, "").Pick = pick;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Leasing(lab)) return;
            int owned = LabQueries.OwnedGpuUnits(lab, HardwareBatch.Hq);
            if (owned < lab.LeaseUnits)
            {
                long share = owned * MathI.Permille / lab.LeaseUnits;
                lab.LeaseValue = MathI.MulPermille(lab.LeaseValue, share);
                lab.LeaseRent = MathI.MulPermille(lab.LeaseRent, share);
                lab.LeaseBuyback = MathI.MulPermille(lab.LeaseBuyback, share);
                lab.LeaseUnits = owned;
                if (owned == 0 || lab.LeaseValue < 1)
                {
                    Clear(lab);
                    return;
                }
            }
            if (lab.Cash >= lab.LeaseRent)
            {
                Finance.Spend(lab, LedgerLine.GpuRent, lab.LeaseRent);
                lab.LeaseMissed = 0;
            }
            else
            {
                lab.LeaseMissed++;
                Events.Add(state, EventKind.LeaseRentMissed, lab.Id, lab.LeaseMissed, lab.LeaseRent, "");
                if (lab.LeaseMissed >= c.Leaseback.MissLimit)
                {
                    GiveBack(c, state, lab, 2);
                    return;
                }
            }
            lab.LeaseLeft--;
            if (lab.LeaseLeft <= 0) GiveBack(c, state, lab, 0);
        }
    }
}
