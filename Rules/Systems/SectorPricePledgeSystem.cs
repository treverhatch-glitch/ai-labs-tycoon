using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q182: locked sector pricing costs weekly marketing while the pledge runs.</summary>
    public static class SectorPricePledgeSystem
    {
        public static bool PledgeActive(SectorEntry entry, int tick) =>
            entry != null && entry.PricePledgeUntilTick > tick;

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Bankrupt) return;
            long each = c.Market.SectorPricePledgeWeeklyUpkeep;
            if (each < 1) return;
            int tick = state.Tick;
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorEntry entry = state.Sectors[s].Entry(lab.Id);
                if (!PledgeActive(entry, tick)) continue;
                if (lab.Cash < each) continue;
                Finance.Spend(lab, LedgerLine.RoomUpkeep, each);
            }
        }
    }
}