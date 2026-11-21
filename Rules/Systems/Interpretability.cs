using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The interpretability programme. See <see cref="InterpConfig"/>. Only the player's lab runs one, and nothing runs until it asks.</summary>
    public static class Interpretability
    {
        public static int Level(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(lab.InterpLevel, 0, c.Interp.MaxLevel);
        }

        /// <summary>The tech the lab needs for a level (1 to the highest), or empty when there is no such level.</summary>
        public static string TechFor(GameConfig c, int level)
        {
            return level >= 1 && level <= c.Interp.MaxLevel ? c.Interp.LevelTechs[level - 1] : "";
        }

        /// <summary>The highest level the lab's techs allow.</summary>
        public static int HighestAllowed(GameConfig c, LabState lab)
        {
            int best = 0;
            for (int level = 1; level <= c.Interp.MaxLevel; level++)
            {
                if (!LabQueries.HasTech(lab, TechFor(c, level))) break;
                best = level;
            }
            return best;
        }

        public static long WeeklyCost(GameConfig c, LabState lab)
        {
            return Level(c, lab) * c.Interp.WeeklyCostPerLevel;
        }

        public static int ResearchCostPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(900, Level(c, lab) * (long)c.Interp.ResearchCostPermillePerLevel);
        }

        public static int StaleReliefPermille(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(MathI.Permille, Level(c, lab) * (long)c.Interp.StaleReliefPermillePerLevel);
        }

        /// <summary>The points of gap the programme would close on a model this week: its share of the gap, never less than a point, never more than the gap.</summary>
        public static int GapClosed(GameConfig c, LabState lab, ModelState model)
        {
            int gap = SafetySystem.AlignmentGap(c, model);
            if (gap <= 0 || Level(c, lab) == 0) return 0;
            long share = MathI.MulPermille(gap, Level(c, lab) * (long)c.Interp.GapClosePermillePerLevel);
            return (int)MathI.Min(gap, MathI.Max(1, share));
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId || Level(c, lab) == 0) return;
            Finance.Spend(lab, LedgerLine.Training, WeeklyCost(c, lab));
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released) continue;
                int closed = GapClosed(c, lab, model);
                if (closed <= 0) continue;
                model.DisplayedAlignment -= closed;
                if (SafetySystem.AlignmentGap(c, model) == 0) Events.Add(state, EventKind.GapFound, lab.Id, model.Id, closed, model.Name);
            }
        }
    }
}
