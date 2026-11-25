using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Open requisitions. See <see cref="RequisitionsConfig"/>. Only the player's lab places orders.</summary>
    public static class Requisitions
    {
        public static bool Enabled(GameConfig c) { return c.Requisitions.Enabled > 0; }

        /// <summary>The price per person an order locks today.</summary>
        public static long LockedFee(GameConfig c, GameState state, LabState lab, RoleConfig role)
        {
            return MathI.MulPermille(RolePerks.HireCost(c, lab, role, 1, state.Tick), c.Requisitions.FeePermille);
        }

        public static long DepositEach(GameConfig c, long fee) { return MathI.MulPermille(fee, c.Requisitions.DepositPermille); }

        public static void Open(GameConfig c, GameState state, LabState lab, RoleConfig role, int count)
        {
            var order = new Requisition();
            order.RoleId = role.Id;
            order.Count = count;
            order.Due = state.Tick + c.Requisitions.LeadWeeks;
            order.Fee = LockedFee(c, state, lab, role);
            lab.Requisitions.Add(order);
            Finance.Spend(lab, LedgerLine.Hiring, DepositEach(c, order.Fee) * count);
            Events.Add(state, EventKind.RequisitionOpened, lab.Id, -1, count, role.Id).Pick = c.Requisitions.LeadWeeks;
        }

        public static void Cancel(GameConfig c, GameState state, LabState lab, Requisition order)
        {
            lab.Requisitions.Remove(order);
            Events.Add(state, EventKind.RequisitionClosed, lab.Id, order.Count, 0, order.RoleId).Pick = -1;
        }

        /// <summary>The people arrive: each needs a free seat and the rest of the locked price. The rest are lost, with their deposit.</summary>
        static void Arrive(GameConfig c, GameState state, LabState lab, Requisition order)
        {
            RoleConfig role = c.Role(order.RoleId);
            long rest = order.Fee - DepositEach(c, order.Fee);
            int hired = 0;
            for (int i = 0; i < order.Count; i++)
            {
                if (LabQueries.FreeSeats(c, lab, role.Id) < 1 || lab.Cash < rest) break;
                Finance.Spend(lab, LedgerLine.Hiring, rest);
                StaffSystem.Hire(c, state, lab, role, 1);
                hired++;
            }
            lab.Requisitions.Remove(order);
            Events.Add(state, EventKind.RequisitionClosed, lab.Id, order.Count, hired, role.Id).Pick = hired == order.Count ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Requisitions.Count == 0 || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Requisitions.Count; i++)
            {
                Requisition order = lab.Requisitions[i];
                if (state.Tick + 1 < order.Due) continue;
                Arrive(c, state, lab, order);
                i--;
            }
        }
    }
}
