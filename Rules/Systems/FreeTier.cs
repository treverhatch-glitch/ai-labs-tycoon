using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A free tier of the best released model. See <see cref="FreeTierConfig"/>. Only the player's lab.</summary>
    public static class FreeTier
    {
        public static bool Enabled(GameConfig c) { return c.FreeTier.Enabled > 0; }

        public static bool Open(LabState lab) { return lab.FtLevel > 0; }

        public static int Lift(GameConfig c, int level) { return level == 1 ? c.FreeTier.LiftA : level == 2 ? c.FreeTier.LiftB : level == 3 ? c.FreeTier.LiftC : 0; }

        public static int Step(GameConfig c, int level) { return level == 1 ? c.FreeTier.StepA : level == 2 ? c.FreeTier.StepB : level == 3 ? c.FreeTier.StepC : 0; }

        public static long BaseCost(GameConfig c, int level) { return level == 1 ? c.FreeTier.CostA : level == 2 ? c.FreeTier.CostB : level == 3 ? c.FreeTier.CostC : 0; }

        /// <summary>What this week's serving costs: the level's cost, up with the reach.</summary>
        public static long WeeklyCost(GameConfig c, LabState lab)
        {
            return MathI.MulPermille(BaseCost(c, lab.FtLevel), MathI.Permille + lab.FtReach);
        }

        /// <summary>The extra revenue this week: the level's ceiling scaled by the reach. Called from the market with what the lab earned.</summary>
        public static long RevenueLift(GameConfig c, GameState state, LabState lab, long revenue)
        {
            if (!Enabled(c) || !Open(lab) || lab.Id != state.PlayerLabId || revenue <= 0) return 0;
            long lift = MathI.MulPermille(MathI.MulPermille(revenue, Lift(c, lab.FtLevel)), lab.FtReach);
            lab.FtEarned += lift;
            return lift;
        }

        public static void Begin(GameConfig c, GameState state, LabState lab, int level)
        {
            lab.FtLevel = level;
            lab.FtReach = 0;
            lab.FtWeeks = 0;
            Events.Add(state, EventKind.FreeTierOpened, lab.Id, level, level, "");
        }

        static void End(GameConfig c, GameState state, LabState lab, int pick)
        {
            long rep = pick == 1 ? PressSystem.Damp(c, lab, (int)MathI.MulPermille(lab.FtReach, c.FreeTier.CloseLoss)) : 0;
            if (rep > 0) PressSystem.AddReputation(lab, -(int)rep);
            int weeks = lab.FtWeeks;
            lab.FtLevel = 0;
            lab.FtReach = 0;
            lab.FtWeeks = 0;
            Events.Add(state, EventKind.FreeTierClosed, lab.Id, weeks, rep, "").Pick = pick;
        }

        public static void Close(GameConfig c, GameState state, LabState lab) { End(c, state, lab, 1); }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Open(lab)) return;
            if (LabQueries.BestReleased(lab) == null)
            {
                End(c, state, lab, 0);
                return;
            }
            long cost = WeeklyCost(c, lab);
            Finance.Spend(lab, LedgerLine.GpuUpkeep, cost);
            lab.FtSpent += cost;
            lab.FtWeeks++;
            lab.FtReach = (int)MathI.Min(MathI.Permille, lab.FtReach + (long)Step(c, lab.FtLevel));
        }
    }
}
