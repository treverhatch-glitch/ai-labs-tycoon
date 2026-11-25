using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Crunch time: the player's team works late for more research and faster training, and pays in morale, incidents and
    /// (when morale gives out) quits. It ends when the player says so, when the team is worn out, or after the longest run
    /// the data allows. Rivals never crunch.
    /// </summary>
    public static class Crunch
    {
        /// <summary><c>CrunchWeeks</c> is 1 in the first week of crunch and counts up by one each week, so the weeks worked are one less.</summary>
        public static bool Active(LabState lab) { return lab.CrunchWeeks > 0; }

        /// <summary>The team's average morale, 0 to 1000, or 0 with no one on the payroll.</summary>
        public static int AverageMorale(LabState lab)
        {
            if (lab.Staff.Count == 0) return 0;
            long total = 0;
            for (int i = 0; i < lab.Staff.Count; i++) total += lab.Staff[i].Morale;
            return (int)(total / lab.Staff.Count);
        }

        public static int ResearchPermille(GameConfig c, LabState lab) { return Active(lab) ? c.StaffPeople.CrunchResearchPermille : 0; }

        public static int TrainingPermille(GameConfig c, LabState lab) { return Active(lab) ? c.StaffPeople.CrunchTrainingPermille : 0; }

        /// <summary>What crunch adds to a released model's weekly incident chance: more with every week of it, up to the cap.</summary>
        public static int IncidentPermille(GameConfig c, LabState lab)
        {
            if (!Active(lab)) return 0;
            return (int)MathI.Min(c.StaffPeople.CrunchIncidentCap, (long)lab.CrunchWeeks * c.StaffPeople.CrunchIncidentPerWeek);
        }

        /// <summary>Ends crunch time. <c>weeksWorked</c> is how many weeks of it the team has done.</summary>
        public static void End(GameState state, LabState lab, string why, int weeksWorked)
        {
            GameEvent e = Events.Add(state, EventKind.CrunchEnded, lab.Id, -1, weeksWorked, why);
            e.Pick = why == "tired" ? 1 : why == "stopped" ? 0 : -1;
            lab.CrunchWeeks = 0;
        }

        /// <summary>One week of crunch: everyone loses morale, then the team may be worn out or have gone as far as it can.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab)) return;
            TechDebt.Add(c, state, lab, c.TechDebt.CrunchPerWeek);
            if (lab.Staff.Count == 0) { End(state, lab, "empty", lab.CrunchWeeks - 1); return; }
            int drop = c.StaffPeople.CrunchMoraleDrop;
            for (int i = 0; i < lab.Staff.Count; i++)
                lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - drop);
            if (AverageMorale(lab) < c.StaffPeople.CrunchBreakMorale) End(state, lab, "tired", lab.CrunchWeeks);
            else if (lab.CrunchWeeks >= c.StaffPeople.CrunchMaxWeeks) End(state, lab, "spent", lab.CrunchWeeks);
            else lab.CrunchWeeks++;
        }
    }
}
