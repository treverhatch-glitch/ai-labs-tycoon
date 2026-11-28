using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// Paying to be heard by a government. The price, the odds and the answer are all worked out from the lab's own books, its policy staff and a
    /// hash of the week, so asking twice in the same week gets the same answer and nothing here draws from the shared dice.
    /// </summary>
    public static class Lobbying
    {
        public const string Quiet = "quiet";
        public const string Subsidy = "subsidy";

        public static bool IsGoal(string goal) { return goal == Quiet || goal == Subsidy; }

        public static long WeekCost(GameConfig c, GameState state, LabState lab)
        {
            return MathI.Max(c.Lobbying.MinWeekCost, LabQueries.WeeklyRunningCost(c, state, lab));
        }

        public static long Cost(GameConfig c, GameState state, LabState lab, string goal)
        {
            int weeks = goal == Subsidy ? c.Lobbying.SubsidyCostWeeks : c.Lobbying.QuietCostWeeks;
            return weeks * WeekCost(c, state, lab);
        }

        /// <summary>The chance, in permille, that a push becomes a scandal. Policy staff cut it, down to the floor.</summary>
        public static int BackfireChancePermille(GameConfig c, LabState lab)
        {
            LobbyConfig l = c.Lobbying;
            long chance = l.BackfirePermille - (long)WorldQueries.PolicyStaffWithConsultants(c, lab) * l.BackfireSavingPermillePerStaff;
            return (int)MathI.Max(l.BackfireFloorPermille, chance);
        }

        /// <summary>The chance, in permille, that a subsidy push wins.</summary>
        public static int SubsidyChancePermille(GameConfig c, LabState lab)
        {
            LobbyConfig l = c.Lobbying;
            long chance = l.SubsidyChancePermille + (long)WorldQueries.PolicyStaffWithConsultants(c, lab) * l.SubsidyChancePermillePerStaff;
            return (int)MathI.Min(l.SubsidyChanceCapPermille, chance);
        }

        /// <summary>The weeks left before the government can be lobbied again, or 0.</summary>
        public static int CooldownLeft(GameState state, BlocStanding standing)
        {
            int left = standing.LobbyUntilTick - state.Tick;
            return left > 0 ? left : 0;
        }

        static long Mix(GameState state, LabState lab, BlocStanding standing, int salt)
        {
            long mix = (state.Seed * 31 + state.Tick) * 31 + lab.Id * 17 + salt;
            for (int i = 0; i < standing.BlocId.Length; i++) mix = mix * 131 + standing.BlocId[i];
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 8;
        }

        /// <summary>
        /// Runs the push the player has already paid for. The result is "landed", "missed" or "backfired". A scandal comes first: a backfire ruins
        /// either goal. Otherwise a quiet word always lands, and a subsidy push lands on its own odds.
        /// </summary>
        public static string Run(GameConfig c, GameState state, LabState lab, BlocStanding standing, string goal)
        {
            LobbyConfig l = c.Lobbying;
            int cd = LobbyingEraCooldown.EffectiveCooldownWeeks(c, state);
            int cut = l.CooldownWeeks - cd;
            standing.LobbyUntilTick = state.Tick + cd;
            LobbyingEraCooldown.OnLobbied(c, state, lab, goal, cd, cut);
            if ((int)(Mix(state, lab, standing, 1) % MathI.Permille) < BackfireChancePermille(c, lab))
            {
                standing.Attention = (int)MathI.Clamp((long)standing.Attention + l.BackfireAttention, 0, MathI.Permille);
                int loss = PressSystem.Damp(c, lab, l.BackfireReputation);
                lab.Reputation = (int)MathI.Clamp((long)lab.Reputation - loss, 0, MathI.Permille);
                return "backfired";
            }
            if (goal == Quiet)
            {
                standing.Attention = (int)MathI.Clamp((long)standing.Attention - l.QuietAttention, 0, MathI.Permille);
                return "landed";
            }
            if ((int)(Mix(state, lab, standing, 2) % MathI.Permille) >= SubsidyChancePermille(c, lab)) return "missed";
            Finance.Earn(lab, LedgerLine.Grants, l.SubsidyReturnWeeks * WeekCost(c, state, lab));
            return "landed";
        }
    }
}
