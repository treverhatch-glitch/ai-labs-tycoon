using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Chief officers. See <see cref="ChiefsConfig"/>. Only the player's lab appoints them.</summary>
    public static class Chiefs
    {
        public const string Scientist = "scientist";
        public const string Safety = "safety";

        public static bool Enabled(GameConfig c) { return c.Chiefs.Enabled; }

        public static bool Known(string kind) { return kind == Scientist || kind == Safety; }

        public static RoleFunction FunctionOf(string kind) { return kind == Scientist ? RoleFunction.Research : RoleFunction.Safety; }

        /// <summary>The person named to the title, or -1 for nobody. The person may have left since; <see cref="Holder"/> checks.</summary>
        public static int IdOf(LabState lab, string kind) { return kind == Scientist ? lab.ChiefScientistId : lab.ChiefSafetyId; }

        static void SetId(LabState lab, string kind, int id)
        {
            if (kind == Scientist) lab.ChiefScientistId = id; else lab.ChiefSafetyId = id;
        }

        /// <summary>The person sitting in the title: still on the payroll and still in a role of the right function. Null when the seat is empty.</summary>
        public static StaffMember Holder(GameConfig c, LabState lab, string kind)
        {
            int id = IdOf(lab, kind);
            if (id < 0) return null;
            StaffMember person = Retraining.Person(lab, id);
            if (person == null) return null;
            RoleConfig role = c.Role(person.RoleId);
            return role != null && role.Function == FunctionOf(kind) ? person : null;
        }

        public static bool Sitting(GameConfig c, LabState lab, string kind) { return Enabled(c) && Holder(c, lab, kind) != null; }

        public static int Count(GameConfig c, LabState lab) { return (Sitting(c, lab, Scientist) ? 1 : 0) + (Sitting(c, lab, Safety) ? 1 : 0); }

        public static int ResearchPermille(GameConfig c, LabState lab) { return Sitting(c, lab, Scientist) ? c.Chiefs.ResearchPermille : 0; }

        public static int IncidentCutPermille(GameConfig c, LabState lab) { return Sitting(c, lab, Safety) ? c.Chiefs.IncidentCutPermille : 0; }

        /// <summary>What the chief safety officer takes off a weekly chance: zero or negative.</summary>
        public static int IncidentChange(GameConfig c, LabState lab, long chance)
        {
            return (int)-MathI.MulPermille(chance, IncidentCutPermille(c, lab));
        }

        public static int PoachPenaltyPermille(GameConfig c, LabState lab) { return (int)(Count(c, lab) * (long)c.Chiefs.PoachPenaltyPermille); }

        public static long WeeklyPremium(GameConfig c, LabState lab) { return Count(c, lab) * c.Chiefs.WeeklyPremium; }

        public static void Appoint(GameConfig c, GameState state, LabState lab, string kind, StaffMember person, long fee)
        {
            StaffMember old = Holder(c, lab, kind);
            if (old != null && old.Id != person.Id) old.Morale = (int)MathI.Max(0, old.Morale - c.Chiefs.ShockMorale);
            SetId(lab, kind, person.Id);
            Finance.Spend(lab, LedgerLine.Hiring, fee);
            GameEvent e = Events.Add(state, EventKind.ChiefChanged, lab.Id, person.Id, fee, kind);
            e.Pick = 1;
        }

        public static void StepDown(GameConfig c, GameState state, LabState lab, string kind, StaffMember person)
        {
            person.Morale = (int)MathI.Max(0, person.Morale - c.Chiefs.ShockMorale);
            SetId(lab, kind, -1);
            GameEvent e = Events.Add(state, EventKind.ChiefChanged, lab.Id, person.Id, 0, kind);
            e.Pick = 0;
        }

        /// <summary>A chief who has left the payroll or the field takes some of the team's spirit with them. Sitting chiefs are paid.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId || !Enabled(c)) return;
            string[] kinds = { Scientist, Safety };
            for (int i = 0; i < kinds.Length; i++)
            {
                if (IdOf(lab, kinds[i]) < 0 || Holder(c, lab, kinds[i]) != null) continue;
                SetId(lab, kinds[i], -1);
                for (int p = 0; p < lab.Staff.Count; p++) lab.Staff[p].Morale = (int)MathI.Max(0, lab.Staff[p].Morale - c.Chiefs.ShockMorale);
                Events.Add(state, EventKind.ChiefLeft, lab.Id, -1, c.Chiefs.ShockMorale, kinds[i]);
            }
            Finance.Spend(lab, LedgerLine.Salaries, WeeklyPremium(c, lab));
        }
    }
}
