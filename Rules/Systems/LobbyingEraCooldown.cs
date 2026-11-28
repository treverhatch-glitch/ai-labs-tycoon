using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class LobbyingEraCooldown
    {
        public static bool Enabled(GameConfig c) { return c.LobbyingEraCooldown.Enabled > 0; }

        public static int EffectiveCooldownWeeks(GameConfig c, GameState state)
        {
            int weeks = c.Lobbying.CooldownWeeks;
            if (!Enabled(c) || state == null) return weeks;
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            if (era < c.LobbyingEraCooldown.MinEra) return weeks;
            int cut = c.LobbyingEraCooldown.CooldownCutWeeks;
            if (cut <= 0) return weeks;
            return (int)MathI.Max(0, weeks - cut);
        }

        public static void OnLobbied(GameConfig c, GameState state, LabState lab, string goal, int appliedWeeks, int cut)
        {
            if (!Enabled(c) || cut <= 0) return;
            Events.Add(state, EventKind.LobbyingEraCooldownShortened, lab.Id, cut, appliedWeeks, goal);
        }
    }
}