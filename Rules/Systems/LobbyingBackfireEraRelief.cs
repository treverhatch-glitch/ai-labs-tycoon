using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LobbyingBackfireEraRelief
    {
        public static bool Enabled(GameConfig c) { return c.LobbyingBackfireEraRelief.Enabled > 0; }

        public static int ReputationLoss(GameConfig c, GameState state, int loss)
        {
            if (!Enabled(c) || state == null || loss <= 0) return loss;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.LobbyingBackfireEraRelief.MinEra) return loss;
            int cut = c.LobbyingBackfireEraRelief.ReliefPermille;
            if (cut <= 0) return loss;
            return (int)MathI.MulPermille(loss, MathI.Permille - cut);
        }

        public static void OnBackfire(GameConfig c, GameState state, LabState lab, string goal, int listLoss, int applied)
        {
            if (!Enabled(c) || applied >= listLoss) return;
            int saved = listLoss - applied;
            if (saved <= 0) return;
            Events.Add(state, EventKind.LobbyingBackfireEraRelieved, lab.Id, -1, saved, goal);
        }
    }
}