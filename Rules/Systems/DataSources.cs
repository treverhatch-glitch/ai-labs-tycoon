using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class DataSources
    {
        public static bool Enabled(GameConfig c) { return c.DataSources.Enabled > 0; }

        public static int Index(GameConfig c, string id)
        {
            for (int i = 0; i < c.DataSources.Sources.Count; i++)
                if (c.DataSources.Sources[i].Id == id) return i;
            return -1;
        }

        public static void Acquire(GameConfig c, GameState state, LabState lab, string sourceId)
        {
            int i = Index(c, sourceId);
            DataSourceItemConfig src = c.DataSources.Sources[i];
            SetWeeks(lab, i, src.DurationWeeks);
            Finance.Spend(lab, LedgerLine.Training, src.WeeklyCost);
            Events.Add(state, EventKind.DataSourceAcquired, lab.Id, i, src.DurationWeeks, sourceId);
            DataSourceSpecialistPairing.OnAcquire(c, state, lab, sourceId);
        }

        public static void Renew(GameConfig c, GameState state, LabState lab, string sourceId)
        {
            int i = Index(c, sourceId);
            DataSourceItemConfig src = c.DataSources.Sources[i];
            SetWeeks(lab, i, src.DurationWeeks);
            Finance.Spend(lab, LedgerLine.Training, src.WeeklyCost);
            Events.Add(state, EventKind.DataSourceRenewed, lab.Id, i, src.DurationWeeks, sourceId);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < c.DataSources.Sources.Count; i++)
            {
                int left = Weeks(lab, i);
                if (left <= 0) continue;
                DataSourceItemConfig src = c.DataSources.Sources[i];
                if (src.WeeklyCost > 0) Finance.Spend(lab, LedgerLine.Training, src.WeeklyCost);
                left--;
                SetWeeks(lab, i, left);
                if (left == 0) Events.Add(state, EventKind.DataSourceLapsed, lab.Id, i, 0, src.Id);
            }
        }

        public static int QualityPermille(GameConfig c, LabState lab)
        {
            if (!Enabled(c)) return 0;
            int total = 0;
            for (int i = 0; i < c.DataSources.Sources.Count; i++)
                if (Weeks(lab, i) > 0) total += c.DataSources.Sources[i].QualityPermille;
            return total;
        }

        public static int QualityPermille(GameConfig c, GameState state, LabState lab)
        {
            int total = QualityPermille(c, lab);
            if (state != null) total += DataSourceSpecialistPairing.BonusPermille(c, state, lab);
            return total;
        }

        static int Weeks(LabState lab, int i)
        {
            if (i == 0) return lab.SrcWeeks0;
            if (i == 1) return lab.SrcWeeks1;
            if (i == 2) return lab.SrcWeeks2;
            return lab.SrcWeeks3;
        }

        static void SetWeeks(LabState lab, int i, int w)
        {
            if (i == 0) lab.SrcWeeks0 = w;
            else if (i == 1) lab.SrcWeeks1 = w;
            else if (i == 2) lab.SrcWeeks2 = w;
            else lab.SrcWeeks3 = w;
        }
    }
}