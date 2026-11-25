using System.Collections.Generic;
using System.Text;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Unity.Hq
{
    /// <summary>Turns the latest GauntletRecord into show copy (presentation only).</summary>
    public static class GauntletShowFormatter
    {
        public static string Summary(GameConfig config, GameState state, GauntletRecord record)
        {
            if (record == null) return "No Gauntlet yet.";
            var sb = new StringBuilder();
            sb.Append("Gauntlet ").Append(record.Year).AppendLine();

            if (record.CrownLabId > 0)
            {
                LabState lab = state.Lab(record.CrownLabId);
                string labName = lab != null ? lab.Name : "Lab #" + record.CrownLabId;
                ModelState model = lab?.Model(record.CrownModelId);
                string modelName = model != null ? model.Name : "model";
                sb.Append("Crown: ").Append(labName).Append(" — ").Append(modelName).AppendLine();
            }

            List<string> brackets = TopSectorWins(record, 4);
            for (int i = 0; i < brackets.Count; i++) sb.AppendLine(brackets[i]);

            if (record.SafestLabId > 0)
            {
                LabState safest = state.Lab(record.SafestLabId);
                if (safest != null) sb.Append("Safest lab: ").Append(safest.Name);
            }

            return sb.ToString().TrimEnd();
        }

        public static List<string> TopSectorWins(GauntletRecord record, int max)
        {
            var lines = new List<string>();
            var seen = new HashSet<string>();
            for (int i = 0; i < record.Entries.Count && lines.Count < max; i++)
            {
                GauntletEntry e = record.Entries[i];
                if (e.Rank != 1) continue;
                if (e.Bracket == "crown") continue;
                if (!seen.Add(e.Bracket)) continue;
                lines.Add(e.Bracket + ": " + e.ModelName + " (" + e.Score + ")");
            }
            return lines;
        }
    }
}