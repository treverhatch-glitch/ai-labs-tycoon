using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The ethics board. See <see cref="EthicsBoardConfig"/>. Only the player's lab seats one.</summary>
    public static class EthicsBoard
    {
        public static bool Enabled(GameConfig c) { return c.Board.MaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.EthicsLevel, 0, c.Board.MaxLevel); }

        /// <summary>The highest level the lab's safety staff cover right now.</summary>
        public static int StaffCovers(GameConfig c, LabState lab)
        {
            int per = c.Board.SafetyStaffPerLevel;
            int covered = per <= 0 ? c.Board.MaxLevel : LabQueries.StaffByFunction(c, lab, RoleFunction.Safety) / per;
            return (int)MathI.Min(c.Board.MaxLevel, covered);
        }

        public static long WeeklyCost(GameConfig c, LabState lab) { return Level(c, lab) * c.Board.WeeklyCostPerLevel; }

        public static long SetupCost(GameConfig c, LabState lab, int level)
        {
            return level <= Level(c, lab) ? 0 : (level - Level(c, lab)) * c.Board.SetupCostPerLevel;
        }

        public static int ScandalCutPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(900, Level(c, lab) * (long)c.Board.ScandalCutPermillePerLevel);
        }

        public static int AttentionReliefPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(900, Level(c, lab) * (long)c.Board.AttentionPermillePerLevel);
        }

        public static int HearingPoints(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.Board.HearingPointsPerLevel); }

        /// <summary>A board of any level will not sit through a rushed launch.</summary>
        public static bool BlocksRush(GameConfig c, LabState lab) { return Level(c, lab) >= 1; }

        /// <summary>True when the board can veto and the model's true alignment is under the bar.</summary>
        public static bool Vetoes(GameConfig c, LabState lab, ModelState model)
        {
            return c.Board.VetoLevel > 0 && Level(c, lab) >= c.Board.VetoLevel && model.Alignment < c.Board.VetoAlignment;
        }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.EthicsLockedUntil; }

        public static void Set(GameConfig c, GameState state, LabState lab, int level, long cost)
        {
            int was = Level(c, lab);
            if (level < was)
            {
                lab.Reputation = (int)MathI.Max(0, lab.Reputation - c.Board.ResignationReputation);
                lab.EthicsLockedUntil = state.Tick + c.Board.LockWeeks;
            }
            lab.EthicsLevel = level;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            GameEvent e = Events.Add(state, EventKind.EthicsBoardChanged, lab.Id, level, cost, "");
            e.Pick = level > was ? 1 : 0;
        }

        /// <summary>The board is paid weekly. If the safety staff behind it have gone, it resigns: the same price as stepping down.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId || Level(c, lab) == 0) return;
            int covered = StaffCovers(c, lab);
            if (covered < Level(c, lab))
            {
                int was = Level(c, lab);
                lab.Reputation = (int)MathI.Max(0, lab.Reputation - c.Board.ResignationReputation);
                lab.EthicsLockedUntil = state.Tick + c.Board.LockWeeks;
                lab.EthicsLevel = covered;
                GameEvent e = Events.Add(state, EventKind.EthicsBoardResigned, lab.Id, covered, was, "");
                e.Pick = covered;
            }
            Finance.Spend(lab, LedgerLine.Salaries, WeeklyCost(c, lab));
        }
    }
}
