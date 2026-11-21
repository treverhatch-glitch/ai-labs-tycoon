using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class PatentSystem
    {
        public static int ActivePatentCount(LabState lab, int tick)
        {
            int n = 0;
            for (int i = 0; i < lab.Patents.Count; i++)
            {
                PatentState p = lab.Patents[i];
                if (p.ExpiresTick > tick) n++;
            }
            return n;
        }

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
            int active = ActivePatentCount(lab, state.Tick);
            if (active > 0 && c.Research.Patents != null)
            {
                int repEach = c.Research.Patents.ReputationPerActivePatent;
                if (repEach > 0)
                    lab.Reputation = (int)MathI.Clamp(lab.Reputation + repEach * active, 0, MathI.Permille);
                long rpEach = c.Research.Patents.ResearchPerActivePatent;
                if (rpEach > 0)
                    lab.ResearchPoints += rpEach * active;
            }
        }
    }
}