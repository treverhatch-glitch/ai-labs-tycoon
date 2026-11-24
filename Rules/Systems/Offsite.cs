using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The offsite: the player takes the whole team away for a day. Everyone's morale rises, but never past a little above where it
    /// settles, so a second offsite straight after the first buys nothing and is refused. It costs about a week of salaries.
    /// </summary>
    public static class Offsite
    {
        /// <summary>The highest an offsite can take this person: a set number of points above their resting morale, on the scale.</summary>
        public static int Ceiling(GameConfig c, LabState lab, StaffMember person)
        {
            return (int)MathI.Min(MathI.Permille, (long)Traits.RestingMorale(c, lab, person) + c.StaffPeople.OffsiteMaxAboveResting);
        }

        /// <summary>What this person gains if they go: the lift, kept under their ceiling, or 0.</summary>
        public static int Gain(GameConfig c, LabState lab, StaffMember person)
        {
            int room = Ceiling(c, lab, person) - person.Morale;
            if (room <= 0) return 0;
            return (int)MathI.Min(c.StaffPeople.OffsiteMoraleLift, room);
        }

        /// <summary>How many people an offsite would lift right now.</summary>
        public static int Lifted(GameConfig c, LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++) if (Gain(c, lab, lab.Staff[i]) > 0) n++;
            return n;
        }

        public static long Cost(GameConfig c, LabState lab)
        {
            return MathI.MulPermille(LabQueries.WeeklySalaries(c, lab), c.StaffPeople.OffsiteSalaryPermille);
        }

        public static void Hold(GameConfig c, GameState state, LabState lab, long cost)
        {
            int lifted = 0;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                int gain = Gain(c, lab, lab.Staff[i]);
                if (gain <= 0) continue;
                lab.Staff[i].Morale += gain;
                lifted++;
            }
            Finance.Spend(lab, LedgerLine.Hiring, cost);
            SafetyCulture.Move(c, lab, c.StaffPeople.OffsiteCultureLift);
            Events.Add(state, EventKind.OffsiteHeld, lab.Id, lifted, cost, "");
        }
    }
}
