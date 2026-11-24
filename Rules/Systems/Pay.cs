using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The player's pay policy. Better pay costs more every week, lifts the level morale settles at and turns more of a rival's offers away; worse
    /// pay does the opposite, and cutting it shocks everyone at once. See the pay fields of <see cref="StaffPeopleConfig"/>. Only the player's lab changes it.
    /// </summary>
    public static class Pay
    {
        public static bool Enabled(GameConfig c) { return c.StaffPeople.PayMinLevel < 0 || c.StaffPeople.PayMaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(lab.PayLevel, c.StaffPeople.PayMinLevel, c.StaffPeople.PayMaxLevel);
        }

        /// <summary>What every salary is raised (or cut) by, in permille.</summary>
        public static int SalaryPermille(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.StaffPeople.PaySalaryStepPermille); }

        public static int MoraleOffset(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.StaffPeople.PayMoraleStep); }

        public static int PoachDefencePermille(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.StaffPeople.PayPoachStepPermille); }

        public static string Describe(GameConfig c, int level)
        {
            if (level == 0) return "market rates";
            int step = (int)(level * (long)c.StaffPeople.PaySalaryStepPermille);
            return step > 0 ? (step / 10) + "% above market" : (-step / 10) + "% below market";
        }

        /// <summary>Changes the level. A cut shocks every person's morale at once, by the data's amount for each level it falls.</summary>
        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            int before = Level(c, lab);
            if (level < before)
            {
                int shock = (int)MathI.Min(MathI.Permille, (long)(before - level) * c.StaffPeople.PayCutShock);
                for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Max(0, lab.Staff[i].Morale - shock);
            }
            lab.PayLevel = level;
            lab.PayLockedUntil = state.Tick + c.StaffPeople.PayLockWeeks;
            GameEvent e = Events.Add(state, EventKind.PayChanged, lab.Id, level, LabQueries.WeeklySalaries(c, lab), "");
            e.Pick = level > before ? 1 : 0;
        }
    }
}
