using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q167: weekly stipend for each sector whose Gauntlet champion is this lab.</summary>
    public static class SectorChampionSystem
    {
        public static int ChampionSectorCount(GameState state, int labId)
        {
            int n = 0;
            for (int i = 0; i < state.Sectors.Count; i++)
                if (state.Sectors[i].ChampionLabId == labId) n++;
            return n;
        }

        public static long WeeklyStipend(GameConfig c, GameState state, LabState lab)
        {
            long each = c.Market?.SectorChampionCashPerWeek ?? 0;
            if (each <= 0 || lab == null || lab.Bankrupt) return 0;
            return each * ChampionSectorCount(state, lab.Id);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            long pay = WeeklyStipend(c, state, lab);
            if (pay > 0) Finance.Earn(lab, LedgerLine.Grants, pay);
        }
    }
}