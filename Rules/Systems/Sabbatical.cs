using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Sabbaticals. See <see cref="SabbaticalConfig"/>. Only the player's lab sends people away.</summary>
    public static class Sabbatical
    {
        public const int Recalled = -1;
        public const int Restless = 0;
        public const int Rested = 1;
        public const int Refreshed = 2;

        public static bool Enabled(GameConfig c) { return c.Sabbatical.Enabled > 0; }

        public static bool Away(GameConfig c, LabState lab, StaffMember person) { return person.LeaveDue > 0; }

        public static int Away(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].LeaveDue > 0) n++;
            return n;
        }

        public static StaffMember Find(LabState lab, int id)
        {
            for (int i = 0; i < lab.Staff.Count; i++) if (lab.Staff[i].Id == id) return lab.Staff[i];
            return null;
        }

        public static void Send(GameConfig c, GameState state, LabState lab, StaffMember person)
        {
            person.LeaveDue = state.Tick + c.Sabbatical.Weeks;
            person.LeaveMorale = person.Morale;
            Events.Add(state, EventKind.SabbaticalStarted, lab.Id, person.Id, c.Sabbatical.Weeks, person.RoleId);
        }

        /// <summary>How a person would come back if they went with this morale.</summary>
        public static int OutcomeFor(GameConfig c, int morale)
        {
            return morale < c.Sabbatical.BurnoutBelow ? Refreshed : morale > c.Sabbatical.RestlessAbove ? Restless : Rested;
        }

        public static void Return(GameConfig c, GameState state, LabState lab, StaffMember person, bool early)
        {
            int outcome = early ? Recalled : OutcomeFor(c, person.LeaveMorale);
            int morale = outcome == Recalled ? person.LeaveMorale
                : outcome == Refreshed ? c.Sabbatical.RefreshedMorale
                : outcome == Restless ? c.Sabbatical.RestlessMorale
                : Traits.RestingMorale(c, lab, person) + c.Sabbatical.RestedBonus;
            person.Morale = (int)MathI.Clamp(morale, 0, MathI.Permille);
            person.LeaveDue = 0;
            person.LeaveMorale = 0;
            Events.Add(state, EventKind.SabbaticalEnded, lab.Id, person.Id, person.Morale, person.RoleId).Pick = outcome;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember person = lab.Staff[i];
                if (person.LeaveDue > 0 && state.Tick + 1 >= person.LeaveDue) Return(c, state, lab, person, false);
            }
        }
    }
}
