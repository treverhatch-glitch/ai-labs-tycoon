using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Visiting scholars. See <see cref="ScholarsConfig"/>. Only the player's lab hosts them.</summary>
    public static class Scholars
    {
        public const string RoleId = "researcher";

        public static bool Enabled(GameConfig c) { return c.Scholars.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.ScholarCount > 0; }

        public static long TotalStipend(GameConfig c, GameState state, LabState lab, int count)
        {
            return MathI.MulPermille(RolePerks.HireCost(c, lab, c.Role(RoleId), 1, state.Tick), c.Scholars.StipendPermille) * count;
        }

        public static long WeeklyStipend(GameConfig c, GameState state, LabState lab)
        {
            return Active(lab) ? TotalStipend(c, state, lab, lab.ScholarCount) / c.Scholars.Weeks : 0;
        }

        /// <summary>The research points the group adds each week.</summary>
        public static long WeeklyPoints(GameConfig c, GameState state, LabState lab)
        {
            long perResearcher = MathI.MulPermille(c.Research.PointsPerResearcherWeek, EraSystem.AtTick(c, state.Tick).ResearchPermille);
            return MathI.MulPermille(perResearcher * lab.ScholarCount, c.Scholars.ResearchPermille);
        }

        public static void Start(GameConfig c, GameState state, LabState lab, int count)
        {
            lab.ScholarCount = count;
            lab.ScholarStart = state.Tick;
            lab.ScholarDue = state.Tick + c.Scholars.Weeks;
            Events.Add(state, EventKind.ScholarsArrived, lab.Id, -1, count, "").Pick = c.Scholars.Weeks;
        }

        public static void Finish(GameConfig c, GameState state, LabState lab, bool early)
        {
            int count = lab.ScholarCount;
            long gained = 0;
            if (!early && Prestige.Enabled(c))
            {
                int before = lab.Prestige;
                lab.Prestige = (int)MathI.Min(c.Prestige.Max, lab.Prestige + (long)count * c.Scholars.PrestigeEach);
                gained = lab.Prestige - before;
            }
            lab.ScholarCount = 0;
            lab.ScholarStart = 0;
            lab.ScholarDue = 0;
            Events.Add(state, EventKind.ScholarsLeft, lab.Id, count, gained, "").Pick = early ? 0 : 1;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab) || lab.Id != state.PlayerLabId) return;
            Finance.Spend(lab, LedgerLine.Salaries, WeeklyStipend(c, state, lab));
            lab.ResearchPoints += WeeklyPoints(c, state, lab);
            if (state.Tick + 1 >= lab.ScholarDue) Finish(c, state, lab, false);
        }
    }
}
