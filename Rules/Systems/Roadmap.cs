using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The public roadmap. See <see cref="RoadmapConfig"/>. Only the player's lab announces one, and nothing runs until it does.</summary>
    public static class Roadmap
    {
        public static bool Enabled(GameConfig c) { return c.Roadmap.MaxLevel > 0; }

        public static bool Active(LabState lab) { return lab.RoadmapLevel > 0; }

        /// <summary>The capability of the lab's best finished model, 0 when it has none yet.</summary>
        public static int Best(LabState lab)
        {
            int best = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Status != ModelStatus.Training && lab.Models[i].Capability > best) best = lab.Models[i].Capability;
            return best;
        }

        /// <summary>The capability a promise of this level asks for: the best so far, stretched, and always at least one point above it.</summary>
        public static int Goal(GameConfig c, LabState lab, int level)
        {
            int best = Best(lab);
            long stretch = (best * (long)c.Roadmap.StretchPerLevel * level + MathI.Permille - 1) / MathI.Permille;
            return (int)(best + MathI.Max(1, stretch));
        }

        public static void Announce(GameConfig c, GameState state, LabState lab, int level, int weeks)
        {
            lab.RoadmapLevel = level;
            lab.RoadmapStart = state.Tick;
            lab.RoadmapDue = state.Tick + weeks;
            lab.RoadmapGoal = Goal(c, lab, level);
            lab.Reputation = (int)MathI.Min(MathI.Permille, lab.Reputation + (long)c.Roadmap.HypePerLevel * level);
            GameEvent e = Events.Add(state, EventKind.RoadmapAnnounced, lab.Id, level, lab.RoadmapGoal, "");
            e.Pick = weeks;
        }

        static bool Delivered(LabState lab)
        {
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status == ModelStatus.Released && m.ReleasedTick >= lab.RoadmapStart && m.Capability >= lab.RoadmapGoal) return true;
            }
            return false;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab)) return;
            bool met = Delivered(lab);
            if (!met && state.Tick + 1 < lab.RoadmapDue) return;
            int level = lab.RoadmapLevel;
            int change = met ? c.Roadmap.DeliverPerLevel * level : -c.Roadmap.MissPerLevel * level;
            lab.Reputation = (int)MathI.Clamp(lab.Reputation + change, 0, MathI.Permille);
            GameEvent e = Events.Add(state, EventKind.RoadmapSettled, lab.Id, level, lab.RoadmapGoal, "");
            e.Pick = met ? 1 : 0;
            lab.RoadmapLevel = 0;
            lab.RoadmapStart = 0;
            lab.RoadmapDue = 0;
            lab.RoadmapGoal = 0;
        }
    }
}
