using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class MuseumSystem
    {
        public static void RunWeek(GameConfig c, LabState lab)
        {
            int cap = c.Endings.Museum.PerLine;
            if (cap < 1) return;
            var lines = new List<string>();
            for (int i = 0; i < lab.Models.Count; i++)
            {
                string line = lab.Models[i].LineId;
                if (!lines.Contains(line)) lines.Add(line);
            }
            for (int l = 0; l < lines.Count; l++) PruneLine(lab, lines[l], cap);
        }

        static void PruneLine(LabState lab, string lineId, int cap)
        {
            var retired = new List<ModelState>();
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.LineId == lineId && m.Status == ModelStatus.Retired) retired.Add(m);
            }
            if (retired.Count <= cap) return;
            retired.Sort((a, b) => RetireOrder(a).CompareTo(RetireOrder(b)));
            int remove = retired.Count - cap;
            for (int i = 0; i < remove; i++) lab.Models.Remove(retired[i]);
        }

        static int RetireOrder(ModelState m)
        {
            if (m.ReleasedTick >= 0) return m.ReleasedTick;
            if (m.TrainedTick >= 0) return m.TrainedTick;
            return m.StartedTick;
        }
    }
}