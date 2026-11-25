using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ReservedComputeGate
    {
        public static bool Enabled(GameConfig c) { return c.ReservedComputeGate.Enabled > 0; }

        public static bool CanSign(GameConfig c, GameState state, LabState lab, out string problem)
        {
            problem = "";
            if (!Enabled(c)) return true;
            if (lab.Reputation < c.ReservedComputeGate.MinReputationPermille)
            {
                problem = "reputation";
                return false;
            }
            if (state.Tick < lab.ReservedSignCooldownUntil)
            {
                problem = "cooldown";
                return false;
            }
            return true;
        }

        public static void OnSigned(GameConfig c, GameState state, LabState lab, int count)
        {
            if (!Enabled(c)) return;
            lab.ReservedSignCooldownUntil = state.Tick + c.ReservedComputeGate.CooldownWeeks;
            GameEvent ev = Events.Add(state, EventKind.ReservedComputeSigned, lab.Id, -1, count, "");
            ev.Pick = 0;
        }
    }
}