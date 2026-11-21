using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PatentSystem
    {
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Patents.Count == 0) return;
            var expired = new List<PatentState>();
            for (int i = 0; i < lab.Patents.Count; i++)
            {
                PatentState p = lab.Patents[i];
                if (p.ExpiresTick > state.Tick) continue;
                expired.Add(p);
            }
            for (int i = 0; i < expired.Count; i++)
            {
                lab.Patents.Remove(expired[i]);
                Events.Add(state, EventKind.PatentExpired, lab.Id, -1, 0, expired[i].TechId);
            }
        }
    }
}