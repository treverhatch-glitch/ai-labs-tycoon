using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q179: paid incident-response plan cuts odds while active and costs upkeep each week.</summary>
    public static class IncidentPlanSystem
    {
        public static bool Active(LabState lab, int tick) => lab != null && lab.IncidentPlanUntilTick > tick;

        public static int IncidentCutPermille(GameConfig c, LabState lab, int tick)
        {
            if (!Active(lab, tick)) return 0;
            return c.Safety.IncidentPlanCutPermille;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Active(lab, state.Tick)) return;
            long upkeep = c.Safety.IncidentPlanWeeklyUpkeep;
            if (upkeep > 0 && lab.Cash >= upkeep)
                Finance.Spend(lab, LedgerLine.Fines, upkeep);
        }
    }
}