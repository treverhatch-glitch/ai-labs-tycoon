using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Inference optimisation. See <see cref="OptimisationConfig"/>. Only the player's lab does it.</summary>
    public static class Optimisation
    {
        public static bool Enabled(GameConfig c) { return c.Optimise.MaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.OptimiseLevel, 0, c.Optimise.MaxLevel); }

        /// <summary>The highest level the lab's engineers cover right now.</summary>
        public static int EngineersCover(GameConfig c, LabState lab)
        {
            int per = c.Optimise.EngineersPerLevel;
            int covered = per <= 0 ? c.Optimise.MaxLevel : LabQueries.StaffByFunction(c, lab, RoleFunction.Engineering) / per;
            return (int)MathI.Min(c.Optimise.MaxLevel, covered);
        }

        /// <summary>The level that counts: the set level, held down by the engineers behind it.</summary>
        public static int Effective(GameConfig c, LabState lab) { return (int)MathI.Min(Level(c, lab), EngineersCover(c, lab)); }

        public static long SetupCost(GameConfig c, LabState lab, int level)
        {
            return level <= Level(c, lab) ? 0 : (level - Level(c, lab)) * c.Optimise.SetupCostPerLevel;
        }

        /// <summary>The share off the compute needed to serve customers, in permille.</summary>
        public static int DemandCutPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(900, Effective(c, lab) * (long)c.Optimise.DemandCutPermillePerLevel);
        }

        /// <summary>The weekly incident chance the work adds, in permille.</summary>
        public static int IncidentPermille(GameConfig c, LabState lab)
        {
            return (int)(Effective(c, lab) * (long)c.Optimise.IncidentPermillePerLevel);
        }

        public static long Serve(GameConfig c, LabState lab, long demand) { return MathI.MulPermille(demand, MathI.Permille - DemandCutPermille(c, lab)); }

        public static void Set(GameConfig c, GameState state, LabState lab, int level, long cost)
        {
            int was = Level(c, lab);
            lab.OptimiseLevel = level;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            GameEvent e = Events.Add(state, EventKind.OptimisationSet, lab.Id, level, cost, "");
            e.Pick = level > was ? 1 : 0;
        }
    }
}
