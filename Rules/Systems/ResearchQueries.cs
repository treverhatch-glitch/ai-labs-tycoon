using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Research forks, dead ends, and patent gates.</summary>
    public static class ResearchQueries
    {
        public static ResearchForkConfig ForkFor(GameConfig c, string techId)
        {
            if (c.Research.Forks == null) return null;
            for (int i = 0; i < c.Research.Forks.Count; i++)
            {
                ResearchForkConfig fork = c.Research.Forks[i];
                if (fork.Techs != null && fork.Techs.Contains(techId)) return fork;
            }
            return null;
        }

        /// <summary>After researching <paramref name="techId"/>, block the other paths in its fork.</summary>
        public static void ApplyFork(GameConfig c, LabState lab, string techId)
        {
            ResearchForkConfig fork = ForkFor(c, techId);
            if (fork == null || fork.Techs == null) return;
            for (int i = 0; i < fork.Techs.Count; i++)
            {
                string other = fork.Techs[i];
                if (other == techId || lab.BlockedTechs.Contains(other)) continue;
                if (lab.Techs.Contains(other)) continue;
                lab.BlockedTechs.Add(other);
            }
        }

        public static bool IsBlocked(LabState lab, string techId)
        {
            return lab.BlockedTechs.Contains(techId);
        }

        /// <summary>Another lab's active patent blocks researching the same tech.</summary>
        public static bool PatentedElsewhere(GameConfig c, GameState state, int labId, string techId, out LabState holder)
        {
            holder = null;
            if (c.Research.Patents == null || c.Research.Patents.DurationWeeks < 1) return false;
            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState other = state.Labs[i];
                if (other.Id == labId || other.Bankrupt) continue;
                if (ActivePatent(other, techId, state.Tick) != null)
                {
                    holder = other;
                    return true;
                }
            }
            return false;
        }

        public static PatentState ActivePatent(LabState lab, string techId, int tick)
        {
            for (int i = 0; i < lab.Patents.Count; i++)
            {
                PatentState p = lab.Patents[i];
                if (p.TechId != techId) continue;
                if (p.ExpiresTick > tick) return p;
            }
            return null;
        }

        public static long PatentFilingCost(GameConfig c, TechConfig tech)
        {
            int mul = c.Research.Patents == null ? 800 : c.Research.Patents.FilingCostPermille;
            return AiLabs.Rules.Core.MathI.MulPermille(tech.Cost, mul);
        }
    }
}