using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The public safety pledge. See <see cref="PledgeConfig"/>. Only the player's lab pledges, and a lab at level 0 is as it always was.</summary>
    public static class Pledge
    {
        public static bool Enabled(GameConfig c) { return c.Pledge.MaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.PledgeLevel, 0, c.Pledge.MaxLevel); }

        public static bool Locked(GameState state, LabState lab) { return state.Tick < lab.PledgeLockedUntil; }

        /// <summary>The alignment a launch must show to keep a pledge of this level.</summary>
        public static int Floor(GameConfig c, int level) { return level <= 0 ? 0 : c.Pledge.BaseFloor + c.Pledge.FloorStep * (level - 1); }

        /// <summary>Extra trust a sector gives the lab each week while the pledge holds.</summary>
        public static int TrustBonus(GameConfig c, LabState lab) { return Level(c, lab) * c.Pledge.TrustPerLevel; }

        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            int before = Level(c, lab);
            int lost = 0;
            if (level < before)
            {
                lost = (int)MathI.Min(lab.Reputation, c.Pledge.WalkBackReputation);
                lab.Reputation -= lost;
            }
            lab.PledgeLevel = level;
            lab.PledgeLockedUntil = state.Tick + c.Pledge.LockWeeks;
            GameEvent e = Events.Add(state, EventKind.PledgeSet, lab.Id, level, lost, "");
            e.Pick = level > before ? 1 : 0;
        }

        /// <summary>A model has just gone on sale for the first time. If it shows less than the pledge, the pledge is broken.</summary>
        public static void OnFirstRelease(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            int level = Level(c, lab);
            if (level <= 0) return;
            int floor = Floor(c, level);
            if (SafetySystem.DisplayedAlignment(model) >= floor) return;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - c.Pledge.BreachReputation);
            for (int s = 0; s < state.Sectors.Count; s++)
                if (state.Sectors[s].Entry(lab.Id) != null) TrustSystem.Shift(c, state, lab, state.Sectors[s].Id, -c.Pledge.BreachTrust, false);
            lab.PledgeLevel = 0;
            lab.PledgeLockedUntil = state.Tick + c.Pledge.LockWeeks;
            GameEvent e = Events.Add(state, EventKind.PledgeBroken, lab.Id, model.Id, floor, model.Name);
            e.Pick = level;
        }
    }
}
