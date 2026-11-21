using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Q170: diesel and emergency power surcharges while a site's heatwave is active.</summary>
    public static class HeatwavePowerSystem
    {
        public static long WeeklySurcharge(GameConfig c, GameState state, LabState lab)
        {
            long rate = c.World?.Heatwave?.WeeklySurchargeCashPerKw ?? 0;
            if (rate <= 0 || lab == null || lab.Bankrupt) return 0;
            long total = 0;
            for (int h = 0; h < lab.Holdings.Count; h++)
            {
                Holding holding = lab.Holdings[h];
                if (!WorldQueries.InHeatwave(state, holding.SiteId)) continue;
                int kw = holding.PrivateKw + holding.HallKw + holding.HallBuildingKw;
                if (kw > 0) total += rate * kw;
            }
            return total;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            long pay = WeeklySurcharge(c, state, lab);
            if (pay > 0 && lab.Cash >= pay)
                Finance.Spend(lab, LedgerLine.Power, pay);
        }
    }
}