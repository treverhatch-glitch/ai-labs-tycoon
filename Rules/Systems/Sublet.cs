using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Compute let to outside customers. See <see cref="SubletConfig"/>. Only the player's lab sublets, and nothing runs until it asks.</summary>
    public static class Sublet
    {
        public static bool Enabled(GameConfig c) { return c.Sublet.MaxPermille > 0; }

        /// <summary>The share of general compute the lab lets, in permille, kept inside the data's limit.</summary>
        public static int Share(GameConfig c, LabState lab)
        {
            return (int)MathI.Clamp(lab.SubletPermille, 0, c.Sublet.MaxPermille);
        }

        /// <summary>The compute units the lab lets this week.</summary>
        public static long Compute(GameConfig c, GameState state, LabState lab)
        {
            return Share(c, lab) == 0 ? 0 : LabQueries.Split(c, state, lab, false).Sublet;
        }

        /// <summary>What the sublet compute earns in a week: the rent of the GPUs it equals, at the data's share.</summary>
        public static long WeeklyRevenue(GameConfig c, GameState state, LabState lab)
        {
            long compute = Compute(c, state, lab);
            if (compute <= 0) return 0;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            if (gen == null || gen.ComputePerGpuWeek <= 0) return 0;
            long rent = compute * gen.RentPerGpuWeek / gen.ComputePerGpuWeek;
            return MathI.MulPermille(rent, c.Sublet.RevenuePermille);
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int permille)
        {
            lab.SubletPermille = permille;
            lab.SubletLockedUntil = state.Tick + c.Sublet.NoticeWeeks;
            Events.Add(state, EventKind.SubletChanged, lab.Id, permille, WeeklyRevenue(c, state, lab), "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId || lab.SubletPermille <= 0) return;
            long revenue = WeeklyRevenue(c, state, lab);
            if (revenue > 0) Finance.Earn(lab, LedgerLine.Sublet, revenue);
        }
    }
}
