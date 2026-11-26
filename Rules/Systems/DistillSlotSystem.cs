using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q221: player buys prepaid distill slots per rival; consume on distill run start; lots expire.</summary>
    public static class DistillSlotSystem
    {
        public static bool Enabled(GameConfig c, GameState state, LabState lab)
        {
            if (lab == null || lab.Id != state.PlayerLabId || c.DistillSlots == null) return false;
            return GameDate.Year(c.Calendar, state.Tick) >= c.DistillSlots.OpensYear;
        }

        public static DistillSlotLot LotForRival(LabState lab, int rivalLabId)
        {
            DistillSlotLot best = null;
            for (int i = 0; i < lab.DistillSlotLots.Count; i++)
            {
                DistillSlotLot lot = lab.DistillSlotLots[i];
                if (lot.RivalLabId != rivalLabId || lot.SlotsRemaining <= 0) continue;
                if (best == null || lot.ExpiresTick < best.ExpiresTick) best = lot;
            }
            return best;
        }

        public static bool HasSlotForRival(GameConfig c, GameState state, LabState lab, int rivalLabId)
        {
            if (!Enabled(c, state, lab)) return true;
            DistillSlotLot lot = LotForRival(lab, rivalLabId);
            if (lot == null) return false;
            if (lot.ExpiresTick > 0 && state.Tick >= lot.ExpiresTick) return false;
            return lot.SlotsRemaining > 0;
        }

        public static int PrepDiscountPermille(GameConfig c, GameState state, LabState lab, int rivalLabId)
        {
            if (!Enabled(c, state, lab)) return 0;
            DistillSlotLot lot = LotForRival(lab, rivalLabId);
            if (lot == null) return 0;
            DistillSlotPackConfig pack = c.DistillSlots.Pack(lot.PackId);
            return pack?.PrepDiscountPermille ?? 0;
        }

        public static bool TryConsumeSlot(GameConfig c, GameState state, LabState lab, int rivalLabId)
        {
            if (!Enabled(c, state, lab)) return true;
            DistillSlotLot lot = LotForRival(lab, rivalLabId);
            if (lot == null || lot.SlotsRemaining <= 0) return false;
            if (lot.ExpiresTick > 0 && state.Tick >= lot.ExpiresTick) return false;
            lot.SlotsRemaining--;
            if (lot.SlotsRemaining <= 0)
            {
                LabState rival = state.Lab(rivalLabId);
                Events.Add(state, EventKind.DistillSlotsExhausted, lab.Id, rivalLabId, 0, rival?.Name ?? "");
            }
            return true;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.DistillSlotLots.Count == 0) return;
            for (int i = lab.DistillSlotLots.Count - 1; i >= 0; i--)
            {
                DistillSlotLot lot = lab.DistillSlotLots[i];
                if (lot.SlotsRemaining <= 0) { lab.DistillSlotLots.RemoveAt(i); continue; }
                if (lot.ExpiresTick > 0 && state.Tick >= lot.ExpiresTick)
                {
                    LabState rival = state.Lab(lot.RivalLabId);
                    Events.Add(state, EventKind.DistillSlotsExpired, lab.Id, lot.RivalLabId, lot.SlotsRemaining, rival?.Name ?? "");
                    lab.DistillSlotLots.RemoveAt(i);
                }
            }
        }

        public static void Purchase(GameConfig c, GameState state, LabState lab, int rivalLabId, DistillSlotPackConfig pack)
        {
            var lot = new DistillSlotLot
            {
                RivalLabId = rivalLabId,
                PackId = pack.Id,
                SlotsRemaining = pack.Slots,
                PurchasedTick = state.Tick,
                ExpiresTick = pack.ValidWeeks > 0 ? state.Tick + pack.ValidWeeks : -1,
            };
            lab.DistillSlotLots.Add(lot);
            LabState rival = state.Lab(rivalLabId);
            Events.Add(state, EventKind.DistillSlotsPurchased, lab.Id, rivalLabId, pack.Slots, rival?.Name ?? "");
        }
    }
}