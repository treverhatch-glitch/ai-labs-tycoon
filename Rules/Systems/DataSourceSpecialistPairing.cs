using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class DataSourceSpecialistPairing
    {
        public static bool Enabled(GameConfig c) { return c.DataSourceSpecialistPairing.Enabled > 0; }

        static bool EraOpen(GameConfig c, GameState state)
        {
            int era = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, state.Tick));
            return era >= c.DataSourceSpecialistPairing.MinEra;
        }

        static DataSourceSpecialistPairingItemConfig FindPair(GameConfig c, string kindId, string sourceId)
        {
            for (int i = 0; i < c.DataSourceSpecialistPairing.Pairs.Count; i++)
            {
                DataSourceSpecialistPairingItemConfig p = c.DataSourceSpecialistPairing.Pairs[i];
                if (p.SpecialistKindId == kindId && p.DataSourceId == sourceId) return p;
            }
            return null;
        }

        static string KindId(GameConfig c, int specKind)
        {
            int idx = specKind - 1;
            if (idx < 0 || idx >= c.SpecialistLines.Kinds.Count) return "";
            return c.SpecialistLines.Kinds[idx].Id;
        }

        public static int BonusPermille(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || !DataSources.Enabled(c) || !SpecialistLines.Enabled(c) || !EraOpen(c, state)) return 0;
            int add = 0;
            for (int m = 0; m < lab.Models.Count; m++)
            {
                ModelState model = lab.Models[m];
                if (model.SpecKind <= 0) continue;
                string kind = KindId(c, model.SpecKind);
                for (int i = 0; i < c.DataSourceSpecialistPairing.Pairs.Count; i++)
                {
                    DataSourceSpecialistPairingItemConfig p = c.DataSourceSpecialistPairing.Pairs[i];
                    if (p.SpecialistKindId != kind) continue;
                    int src = DataSources.Index(c, p.DataSourceId);
                    if (src < 0) continue;
                    int weeks = src == 0 ? lab.SrcWeeks0 : src == 1 ? lab.SrcWeeks1 : src == 2 ? lab.SrcWeeks2 : lab.SrcWeeks3;
                    if (weeks > 0) add += p.BonusPermille;
                }
            }
            return add;
        }

        public static void OnAcquire(GameConfig c, GameState state, LabState lab, string sourceId)
        {
            if (!Enabled(c) || !EraOpen(c, state)) return;
            for (int m = 0; m < lab.Models.Count; m++)
            {
                ModelState model = lab.Models[m];
                if (model.SpecKind <= 0) continue;
                string kind = KindId(c, model.SpecKind);
                if (FindPair(c, kind, sourceId) == null) continue;
                Events.Add(state, EventKind.DataSourceSpecialistPaired, lab.Id, model.Id, model.SpecKind, sourceId);
                return;
            }
        }
    }
}