using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LobbyingCostEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.LobbyingCostEraRelief.Enabled > 0; }

        public static long Cost(GameConfig c, GameState state, long cost)
        {
            if (!Enabled(c) || state == null || cost <= 0) return cost;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.LobbyingCostEraRelief.MinEra) return cost;
            int cut = c.LobbyingCostEraRelief.ReliefPermille;
            if (cut <= 0) return cost;
            return MathI.MulPermille(cost, MathI.Permille - cut);
        }

        public static void OnPaid(GameConfig c, GameState state, LabState lab, string blocId, string goal, long listCost, long paid)
        {
            if (!Enabled(c) || paid >= listCost) return;
            long saved = listCost - paid;
            if (saved <= 0) return;
            Events.Add(state, EventKind.LobbyingCostEraRelieved, lab.Id, -1, (int)saved, blocId + "|" + goal);
        }
    }
}