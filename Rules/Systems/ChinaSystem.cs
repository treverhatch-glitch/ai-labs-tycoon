using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ChinaSystem
    {
        public static int InstalledKwInBloc(GameConfig c, GameState state, LabState lab, string blocId)
        {
            int total = 0;
            for (int i = 0; i < lab.Holdings.Count; i++)
            {
                Holding h = lab.Holdings[i];
                if (WorldQueries.BlocOfHolding(c, h) != blocId) continue;
                total += WorldQueries.InstalledKw(c, state, lab, h.Id);
            }
            return total;
        }

        public static void RunMonth(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId || lab.Bankrupt) return;
            ChinaDealsConfig cfg = c.World.ChinaDeals;
            if (cfg == null || cfg.MinInstalledKw <= 0) return;
            if (lab.ChinaDealPending)
            {
                Decline(c, state, lab, auto: true);
                return;
            }
            if (lab.ChinaDealActive) return;
            if (lab.ChinaDealDeclinedUntilTick >= 0 && state.Tick < lab.ChinaDealDeclinedUntilTick) return;
            if (InstalledKwInBloc(c, state, lab, cfg.BlocId) < cfg.MinInstalledKw) return;
            lab.ChinaDealPending = true;
            Events.Add(state, EventKind.ChinaDealOffered, lab.Id, -1, 0, cfg.BlocId);
        }

        public static void Accept(GameConfig c, GameState state, LabState lab)
        {
            ChinaDealsConfig cfg = c.World.ChinaDeals;
            if (cfg == null || !lab.ChinaDealPending) return;
            lab.ChinaDealPending = false;
            lab.ChinaDealActive = true;
            BlocStanding china = lab.Bloc(cfg.BlocId);
            if (china != null) china.Attention = (int)MathI.Min(MathI.Permille, china.Attention + cfg.AcceptAttention);
            Events.Add(state, EventKind.ChinaDealAccepted, lab.Id, -1, 0, cfg.BlocId);
        }

        public static void Decline(GameConfig c, GameState state, LabState lab, bool auto = false)
        {
            ChinaDealsConfig cfg = c.World.ChinaDeals;
            if (cfg == null || !lab.ChinaDealPending) return;
            lab.ChinaDealPending = false;
            BlocStanding china = lab.Bloc(cfg.BlocId);
            if (china != null) china.Attention = (int)MathI.Min(MathI.Permille, china.Attention + cfg.DeclineAttention);
            if (cfg.DeclineCooldownMonths > 0)
                lab.ChinaDealDeclinedUntilTick = state.Tick + cfg.DeclineCooldownMonths * c.Calendar.MonthsPerYear * c.Calendar.WeeksPerMonth;
            Events.Add(state, EventKind.ChinaDealDeclined, lab.Id, -1, auto ? 1 : 0, cfg.BlocId);
        }
    }
}