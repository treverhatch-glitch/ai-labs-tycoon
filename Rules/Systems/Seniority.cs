using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Promotions. See <see cref="SeniorityConfig"/>. Only the player's lab promotes.</summary>
    public static class Seniority
    {
        public static bool Enabled(GameConfig c) { return c.Seniority.Enabled > 0; }

        public static StaffMember Person(LabState lab, int id)
        {
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].Id == id) return lab.Staff[i];
            return null;
        }

        public static int Seniors(GameConfig c, LabState lab, RoleFunction function)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                RoleConfig role = c.Role(lab.Staff[i].RoleId);
                if (lab.Staff[i].Senior == 1 && role != null && role.Function == function) n++;
            }
            return n;
        }

        /// <summary>The most seniors a function can have: its people times the share, rounded up.</summary>
        public static int Allowed(GameConfig c, LabState lab, RoleFunction function)
        {
            int staff = LabQueries.StaffByFunction(c, lab, function);
            return (int)(((long)staff * c.Seniority.MaxSharePermille + MathI.Permille - 1) / MathI.Permille);
        }

        public static long Fee(GameConfig c, RoleConfig role) { return MathI.MulPermille(role.HireCost, c.Seniority.FeePermille); }

        public static void Promote(GameConfig c, GameState state, LabState lab, StaffMember person, RoleConfig role)
        {
            long fee = Fee(c, role);
            Finance.Spend(lab, LedgerLine.Hiring, fee);
            person.Senior = 1;
            person.Morale = (int)MathI.Min(MathI.Permille, person.Morale + (long)c.Seniority.MoraleBoost);
            Events.Add(state, EventKind.StaffPromoted, lab.Id, person.Id, fee, role.Id);
        }
    }
}
