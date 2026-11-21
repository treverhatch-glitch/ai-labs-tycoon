using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class MuseumSystem
    {
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            int cap = c.Endings.Museum.PerLine;
            if (cap < 1) return;
            int permille = c.Endings.Museum.ResearchPermillePerCapability;
            var lines = new List<string>();
            for (int i = 0; i < lab.Models.Count; i++)
            {
                string line = lab.Models[i].LineId;
                if (!lines.Contains(line)) lines.Add(line);
            }
            long salvaged = 0;
            for (int l = 0; l < lines.Count; l++) salvaged += PruneLine(lab, lines[l], cap, permille);
            if (salvaged > 0)
            {
                lab.ResearchPoints += salvaged;
                Events.Add(state, EventKind.TechResearched, lab.Id, -1, salvaged, "museum-salvage");
            }
            int repEach = c.Endings.Museum.ReputationPerFullLine;
            if (repEach > 0 && cap > 0)
            {
                int full = 0;
                for (int l = 0; l < lines.Count; l++)
                    if (RetiredOnLine(lab, lines[l]) >= cap) full++;
                if (full > 0)
                    lab.Reputation = (int)MathI.Clamp(lab.Reputation + repEach * full, 0, MathI.Permille);
            }
        }

        public static int RetiredOnLine(LabState lab, string lineId)
        {
            int count = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId == lineId && m.Status == ModelStatus.Retired) count++;
            }
            return count;
        }

        static long PruneLine(LabState lab, string lineId, int cap, int researchPermillePerCapability)
        {
            var retired = new List<ModelState>();
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId == lineId && m.Status == ModelStatus.Retired) retired.Add(m);
            }
            if (retired.Count <= cap) return 0;
            retired.Sort((a, b) => RetireOrder(a).CompareTo(RetireOrder(b)));
            int remove = retired.Count - cap;
            long gained = 0;
            for (int i = 0; i < remove; i++)
            {
                if (researchPermillePerCapability > 0)
                    gained += MathI.MulPermille(retired[i].Capability, researchPermillePerCapability);
                lab.Models.Remove(retired[i]);
            }
            return gained;
        }

        static int RetireOrder(ModelState m)
        {
            if (m.ReleasedTick >= 0) return m.ReleasedTick;
            if (m.TrainedTick >= 0) return m.TrainedTick;
            return m.StartedTick;
        }
    }
}