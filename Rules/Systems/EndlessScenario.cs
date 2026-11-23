using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class EndlessScenario
    {
        public const string WinEndingId = "scenario-win";

        public static bool Enabled(GameConfig c) { return c.EndlessScenario.Enabled > 0; }

        public static void OnKeepGoing(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c)) return;
            lab.ScenarioEndlessAck = 1;
            Events.Add(state, EventKind.ScenarioEndlessAck, lab.Id, -1, 0, lab.ScenarioId);
        }
    }
}