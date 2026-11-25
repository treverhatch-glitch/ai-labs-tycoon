using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Internships. See <see cref="InternshipsConfig"/>. Only the player's lab takes interns.</summary>
    public static class Internships
    {
        public static bool Enabled(GameConfig c) { return c.Internships.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.InternCount > 0; }

        public static long Fee(GameConfig c, GameState state, LabState lab, string roleId)
        {
            return RolePerks.HireCost(c, lab, c.Role(roleId), 1, state.Tick);
        }

        /// <summary>What the whole cohort's stipend comes to over its weeks.</summary>
        public static long TotalStipend(GameConfig c, GameState state, LabState lab, string roleId, int count)
        {
            return MathI.MulPermille(Fee(c, state, lab, roleId), c.Internships.StipendPermille) * count;
        }

        public static long WeeklyStipend(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab)) return 0;
            return TotalStipend(c, state, lab, lab.InternRole, lab.InternCount) / c.Internships.Weeks;
        }

        public static void Start(GameConfig c, GameState state, LabState lab, string roleId, int count)
        {
            lab.InternRole = roleId;
            lab.InternCount = count;
            lab.InternStart = state.Tick;
            lab.InternDue = state.Tick + c.Internships.Weeks;
            GameEvent e = Events.Add(state, EventKind.InternshipStarted, lab.Id, -1, count, roleId);
            e.Pick = c.Internships.Weeks;
        }

        /// <summary>Ends the cohort. Unless it is cut short, each intern is offered the job for a share of the fee while there are seats and cash; the rest join the alumni pool on good terms.</summary>
        public static void Finish(GameConfig c, GameState state, LabState lab, bool early)
        {
            RoleConfig role = c.Role(lab.InternRole);
            int asked = lab.InternCount;
            int hired = 0;
            long fee = MathI.MulPermille(Fee(c, state, lab, role.Id), c.Internships.ConvertPermille);
            for (int i = 0; i < asked; i++)
            {
                var person = new StaffMember();
                person.Id = state.TakeId();
                person.RoleId = role.Id;
                person.TraitId = StaffSystem.DefaultTrait(c.StaffPeople);
                person.Morale = (int)MathI.Clamp((long)Traits.RestingMorale(c, lab, person) + c.Internships.ConvertMorale, 0, MathI.Permille);
                bool seat = LabQueries.FreeSeats(c, lab, role.Id) > 0;
                if (!early && seat && lab.Cash >= fee)
                {
                    Finance.Spend(lab, LedgerLine.Hiring, fee);
                    lab.Staff.Add(person);
                    hired++;
                }
                else Alumni.Record(c, state, lab, person, Alumni.Good);
            }
            GameEvent e = Events.Add(state, EventKind.InternshipsEnded, lab.Id, asked, hired, role.Id);
            e.Pick = early ? 0 : 1;
            lab.InternRole = "";
            lab.InternCount = 0;
            lab.InternStart = 0;
            lab.InternDue = 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab) || lab.Id != state.PlayerLabId) return;
            Finance.Spend(lab, LedgerLine.Salaries, WeeklyStipend(c, state, lab));
            if (state.Tick + 1 >= lab.InternDue) Finish(c, state, lab, false);
        }
    }
}
