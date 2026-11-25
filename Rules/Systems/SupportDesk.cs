using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Customer support desk. See <see cref="SupportConfig"/>. Only the player's lab keeps one.</summary>
    public static class SupportDesk
    {
        public const string RoleId = "engineer";

        public static bool Enabled(GameConfig c) { return c.Support.Enabled > 0; }

        public static int Engineers(LabState lab) { return LabQueries.StaffCount(lab, RoleId); }

        /// <summary>Engineers the desk can draw on: those not away on a sabbatical.</summary>
        public static int Available(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
                if (lab.Staff[i].RoleId == RoleId && lab.Staff[i].LeaveDue == 0) n++;
            return n;
        }

        public static int MostPeople(GameConfig c, LabState lab) { return (int)MathI.Max(0, MathI.Min(c.Support.MaxPeople, Available(lab) - c.Support.KeepFree)); }

        /// <summary>The engineers actually on the desk: the number set, never more than the lab has left.</summary>
        public static int OnDesk(GameConfig c, LabState lab) { return (int)MathI.Min(lab.SupportPeople, Available(lab)); }

        /// <summary>True when the person is on the desk: the first engineers in the order the lab lists them.</summary>
        public static bool Away(GameConfig c, LabState lab, StaffMember person)
        {
            if (lab.SupportPeople <= 0 || person.RoleId != RoleId || person.LeaveDue > 0) return false;
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember other = lab.Staff[i];
                if (other.RoleId != RoleId || other.LeaveDue > 0) continue;
                if (other == person) return n < lab.SupportPeople;
                n++;
            }
            return false;
        }

        /// <summary>The weekly revenue of the lab's models on sale.</summary>
        public static long Revenue(GameState state, LabState lab)
        {
            long total = 0;
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorEntry entry = state.Sectors[s].Entry(lab.Id);
                if (entry != null) total += entry.LastRevenue;
            }
            return total;
        }

        public static int Demand(GameConfig c, GameState state, LabState lab) { return (int)MathI.Min(100000, Revenue(state, lab) / c.Support.RevenuePerTicket); }

        public static int Capacity(GameConfig c, LabState lab) { return c.Support.BaseClear + OnDesk(c, lab) * c.Support.PerPerson; }

        public static bool InTrouble(GameConfig c, LabState lab) { return lab.SupportBacklog > c.Support.BacklogLimit; }

        public static int WeeklyTrust(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(c.Support.MaxWeeklyTrust, (long)(lab.SupportBacklog - c.Support.BacklogLimit) * c.Support.TrustPerTicket);
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int people)
        {
            lab.SupportPeople = people;
            Events.Add(state, EventKind.SupportDeskSet, lab.Id, -1, people, "");
        }

        /// <summary>One week: the tickets that came in less those cleared, and the trust a backlog past the limit costs in each sector the lab sells in.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            bool before = InTrouble(c, lab);
            long next = lab.SupportBacklog + Demand(c, state, lab) - Capacity(c, lab);
            lab.SupportBacklog = (int)MathI.Clamp(next, 0, c.Support.MaxBacklog);
            bool after = InTrouble(c, lab);
            if (after)
            {
                int loss = WeeklyTrust(c, lab);
                for (int s = 0; s < state.Sectors.Count; s++)
                    if (state.Sectors[s].Entry(lab.Id) != null) TrustSystem.Shift(c, state, lab, state.Sectors[s].Id, -loss, false);
            }
            if (before != after) Events.Add(state, EventKind.SupportBacklogChanged, lab.Id, -1, lab.SupportBacklog, "").Pick = after ? 1 : 0;
        }
    }
}
