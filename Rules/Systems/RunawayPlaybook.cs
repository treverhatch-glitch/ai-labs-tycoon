using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Lab-wide runaway response posture. See <see cref="RunawayPlaybookConfig"/>. Only the player's lab.</summary>
    public static class RunawayPlaybook
    {
        public static bool Enabled(GameConfig c) { return c.RunawayPlaybook.Enabled > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.PlayLevel, 0, c.RunawayPlaybook.MaxLevel); }

        public static bool Armed(LabState lab) { return lab.PlayArmed > 0 && lab.PlayLeft > 0; }

        public static bool ArmLocked(GameState state, LabState lab) { return state.Tick < lab.PlayArmUntil; }

        public static int BlowupCutPermille(GameConfig c, LabState lab)
        {
            if (!Armed(lab) || Level(c, lab) <= 0) return 0;
            return Level(c, lab) * c.RunawayPlaybook.BlowupCut;
        }

        public static int TrustKeepPermille(GameConfig c, LabState lab)
        {
            if (!Armed(lab) || Level(c, lab) <= 0) return 0;
            return (int)MathI.Min(MathI.Permille, Level(c, lab) * (long)c.RunawayPlaybook.TrustKeep);
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int level)
        {
            int added = (int)MathI.Max(0, level - Level(c, lab));
            long cost = added * (long)c.RunawayPlaybook.SetupPerLevel;
            if (cost > 0) Finance.Spend(lab, LedgerLine.Training, cost);
            lab.PlayLevel = level;
            Events.Add(state, EventKind.PlaybookSet, lab.Id, level, cost, "");
        }

        public static void Arm(GameConfig c, GameState state, LabState lab)
        {
            lab.PlayArmed = 1;
            lab.PlayLeft = c.RunawayPlaybook.ArmWeeks;
            lab.PlayArmUntil = state.Tick + c.RunawayPlaybook.CooldownWeeks;
            Events.Add(state, EventKind.PlaybookArmed, lab.Id, -1, lab.PlayLeft, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (lab.PlayArmed <= 0 || lab.PlayLeft <= 0) return;
            lab.PlayLeft--;
            if (lab.PlayLeft > 0) return;
            lab.PlayArmed = 0;
        }
    }
}