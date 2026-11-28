using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LobbyingGoalEraAmplify
    {
        public static bool Enabled(GameConfig c) { return c.LobbyingGoalEraAmplify.Enabled > 0; }

        public static int Magnitude(GameConfig c, GameState state, int amount)
        {
            if (!Enabled(c) || state == null || amount <= 0) return amount;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.LobbyingGoalEraAmplify.MinEra) return amount;
            int amp = c.LobbyingGoalEraAmplify.AmplifyPermille;
            if (amp <= 0) return amount;
            return (int)MathI.MulPermille(amount, MathI.Permille + amp);
        }

        public static void OnLanded(GameConfig c, GameState state, LabState lab, string goal, string kind, int listAmount, int applied)
        {
            if (!Enabled(c) || applied <= listAmount) return;
            int extra = applied - listAmount;
            if (extra <= 0) return;
            Events.Add(state, EventKind.LobbyingGoalEraAmplified, lab.Id, extra, applied, goal + "|" + kind);
        }
    }
}