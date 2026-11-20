using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The weekly life of the sites: halls and power deals finishing, heatwaves starting and ending.</summary>
    public static class SiteSystem
    {
        /// <summary>
        /// Finishes what is due, before training, so new capacity works the week it opens. Heatwaves that have run their
        /// course are cleared.
        /// </summary>
        public static void RunWeek(GameConfig c, GameState state)
        {
            int tick = state.Tick;
            for (int s = 0; s < state.Sites.Count; s++)
            {
                SiteState site = state.Sites[s];
                if (site.HeatwaveUntilTick >= 0 && tick >= site.HeatwaveUntilTick) site.HeatwaveUntilTick = -1;
            }

            for (int l = 0; l < state.Labs.Count; l++)
            {
                LabState lab = state.Labs[l];
                if (lab.Bankrupt) continue;
                for (int h = 0; h < lab.Holdings.Count; h++)
                {
                    Holding holding = lab.Holdings[h];
                    if (holding.HallDoneTick >= 0 && tick >= holding.HallDoneTick)
                    {
                        holding.HallKw += holding.HallBuildingKw;
                        Events.Add(state, EventKind.HallBuilt, lab.Id, holding.Id, holding.HallBuildingKw, holding.SiteId);
                        holding.HallBuildingKw = 0;
                        holding.HallDoneTick = -1;
                    }
                    if (holding.DealDoneTick >= 0 && tick >= holding.DealDoneTick)
                    {
                        PowerDealConfig deal = c.World.PowerDeal(holding.DealId);
                        holding.PrivateKw = deal.AddsKw;
                        holding.DealDoneTick = -1;
                        Events.Add(state, EventKind.PowerDealReady, lab.Id, holding.Id, deal.AddsKw, deal.Id);
                    }
                }
            }
        }

        /// <summary>
        /// At the start of a summer month, each site where somebody has a holding may roll a heatwave. The chance is the site's
        /// yearly odds split over the summer months. Rolled on the world dice, in the config's site order.
        /// </summary>
        public static void RollHeatwaves(GameConfig c, GameState state)
        {
            HeatwaveConfig hw = c.World.Heatwave;
            int month = GameDate.Month(c.Calendar, state.Tick);
            if (month < hw.FromMonth || month > hw.ToMonth) return;

            for (int s = 0; s < c.World.Sites.Count; s++)
            {
                SiteConfig config = c.World.Sites[s];
                SiteState site = state.Site(config.Id);
                if (site == null || !AnyoneThere(state, config.Id)) continue;
                if (InWave(state, site)) continue;
                if (!state.Rng.World.Chance(config.SummerHeatwavePermille / hw.RollDivisor)) continue;

                site.HeatwaveUntilTick = state.Tick + hw.Weeks;
                for (int l = 0; l < state.Labs.Count; l++)
                {
                    LabState lab = state.Labs[l];
                    if (lab.Bankrupt) continue;
                    for (int h = 0; h < lab.Holdings.Count; h++)
                        if (lab.Holdings[h].SiteId == config.Id) { Events.Add(state, EventKind.Heatwave, lab.Id, lab.Holdings[h].Id, hw.Weeks, config.Id); break; }
                }
            }
        }

        static bool InWave(GameState state, SiteState site) { return site.HeatwaveUntilTick >= 0 && state.Tick < site.HeatwaveUntilTick; }

        static bool AnyoneThere(GameState state, string siteId)
        {
            for (int l = 0; l < state.Labs.Count; l++)
            {
                if (state.Labs[l].Bankrupt) continue;
                for (int h = 0; h < state.Labs[l].Holdings.Count; h++)
                    if (state.Labs[l].Holdings[h].SiteId == siteId) return true;
            }
            return false;
        }
    }
}
