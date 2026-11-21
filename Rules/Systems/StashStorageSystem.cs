using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q172: idle stash erodes each week unless the player launders it.</summary>
    public static class StashStorageSystem
    {
        public static long WeeklyFee(GameConfig c, LabState lab)
        {
            int rate = c.Shady?.StashStoragePermillePerWeek ?? 0;
            if (rate <= 0 || lab == null || lab.Stash < 1) return 0;
            return MathI.MulPermille(lab.Stash, rate);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt) return;
            long fee = WeeklyFee(c, lab);
            if (fee < 1) return;
            lab.Stash = MathI.Max(0, lab.Stash - fee);
        }
    }
}