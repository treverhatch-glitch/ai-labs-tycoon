using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Mentoring. See <see cref="MentoringConfig"/>. Only the player's lab pairs people.</summary>
    public static class Mentoring
    {
        public static bool Enabled(GameConfig c) { return c.Mentoring.Enabled > 0; }

        public static bool IsMentee(StaffMember person) { return person.MentorId > 0; }

        public static bool IsMentor(LabState lab, StaffMember person)
        {
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].MentorId == person.Id) return true;
            return false;
        }

        public static int Pairs(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].MentorId > 0) n++;
            return n;
        }

        /// <summary>What a person counts for in their function, in permille of a whole person: a senior counts for the data's weight unless mentoring, a junior in a pair for the data's lower weight.</summary>
        public static int Weight(GameConfig c, LabState lab, StaffMember person)
        {
            if (person.Senior == 1) return IsMentor(lab, person) ? MathI.Permille : c.Seniority.WeightPermille;
            return person.MentorId > 0 ? c.Mentoring.MenteeWeightPermille : MathI.Permille;
        }

        static RoleFunction? Function(GameConfig c, StaffMember person)
        {
            RoleConfig role = c.Role(person.RoleId);
            if (role == null) return null;
            return role.Function;
        }

        public static void Start(GameConfig c, GameState state, LabState lab, StaffMember mentor, StaffMember mentee)
        {
            mentee.MentorId = mentor.Id;
            mentee.MentorWeeks = 0;
            Events.Add(state, EventKind.MentoringStarted, lab.Id, mentee.Id, mentor.Id, mentee.RoleId);
        }

        /// <summary>Ends a pair. Pick 0 is a graduation, 1 a pair that broke up, 2 the player calling it off.</summary>
        public static void End(GameState state, LabState lab, StaffMember mentee, int pick)
        {
            int weeks = mentee.MentorWeeks;
            mentee.MentorId = 0;
            mentee.MentorWeeks = 0;
            GameEvent e = Events.Add(state, EventKind.MentoringEnded, lab.Id, mentee.Id, weeks, mentee.RoleId);
            e.Pick = pick;
        }

        /// <summary>One week of every pair: the junior's morale climbs, the weeks add up and a pair that has held long enough graduates when a senior seat is free.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember mentee = lab.Staff[i];
                if (mentee.MentorId <= 0) continue;
                StaffMember mentor = Seniority.Person(lab, mentee.MentorId);
                RoleFunction? f = Function(c, mentee);
                if (mentor == null || mentor.Senior != 1 || mentee.Senior == 1 || f == null || Function(c, mentor) != f)
                {
                    End(state, lab, mentee, 1);
                    continue;
                }
                mentee.MentorWeeks++;
                if (mentee.Morale < c.Mentoring.MoraleCeiling)
                    mentee.Morale = (int)MathI.Min(c.Mentoring.MoraleCeiling, mentee.Morale + (long)c.Mentoring.MoralePerWeek);
                if (mentee.MentorWeeks < c.Mentoring.Weeks) continue;
                if (Seniority.Seniors(c, lab, f.Value) >= Seniority.Allowed(c, lab, f.Value)) continue;
                mentee.Senior = 1;
                mentee.Morale = (int)MathI.Min(MathI.Permille, mentee.Morale + (long)c.Mentoring.GraduateMorale);
                mentor.Morale = (int)MathI.Min(MathI.Permille, mentor.Morale + (long)c.Mentoring.MentorMorale);
                End(state, lab, mentee, 0);
            }
        }
    }
}
