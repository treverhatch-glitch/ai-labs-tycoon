using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Weekly domain leaderboards and market appeal when a lab leads a capability domain.</summary>
    public static class CapabilityDomainSystem
    {
        public static CapabilityDomainLeaderState Leader(GameState state, string domainId)
        {
            if (state.DomainLeaders == null) return null;
            for (int i = 0; i < state.DomainLeaders.Count; i++)
            {
                CapabilityDomainLeaderState row = state.DomainLeaders[i];
                if (row.DomainId == domainId) return row;
            }
            return null;
        }

        public static bool LeadsDomain(GameState state, LabState lab, string domainId)
        {
            CapabilityDomainLeaderState row = Leader(state, domainId);
            return row != null && row.LeaderLabId == lab.Id && row.Footprint > 0;
        }

        public static string DomainForSector(GameConfig c, string sectorId)
        {
            for (int i = 0; i < c.Market.CapabilityDomains.Count; i++)
            {
                CapabilityDomainConfig domain = c.Market.CapabilityDomains[i];
                for (int k = 0; k < domain.SectorIds.Count; k++)
                    if (domain.SectorIds[k] == sectorId) return domain.Id;
            }
            return null;
        }

        /// <summary>Appeal permille when this lab leads the sector's capability domain this week.</summary>
        public static int LeaderAppealPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (lab.Bankrupt) return 0;
            string domainId = DomainForSector(c, sectorId);
            if (domainId == null) return 0;
            if (!LeadsDomain(state, lab, domainId)) return 0;
            CapabilityDomainConfig domain = c.Market.Domain(domainId);
            return domain == null || domain.LeaderAppealPermille < 1 ? 0 : domain.LeaderAppealPermille;
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            state.DomainLeaders.Clear();
            for (int d = 0; d < c.Market.CapabilityDomains.Count; d++)
            {
                CapabilityDomainConfig domain = c.Market.CapabilityDomains[d];
                if (domain.SectorIds == null || domain.SectorIds.Count == 0) continue;
                int leaderId = -1;
                long bestFoot = 0;
                int bestCap = 0;
                for (int i = 0; i < state.Labs.Count; i++)
                {
                    LabState lab = state.Labs[i];
                    if (lab.Bankrupt) continue;
                    long foot = CapabilityDomainQueries.DomainFootprint(c, state, lab, domain.Id);
                    if (foot < 1) continue;
                    int cap = CapabilityDomainQueries.BestCapability(c, lab, domain.Id);
                    bool better = foot > bestFoot
                        || (foot == bestFoot && cap > bestCap)
                        || (foot == bestFoot && cap == bestCap && (leaderId < 0 || lab.Id < leaderId));
                    if (!better) continue;
                    bestFoot = foot;
                    bestCap = cap;
                    leaderId = lab.Id;
                }
                if (leaderId < 0 || bestFoot < 1) continue;
                state.DomainLeaders.Add(new CapabilityDomainLeaderState
                {
                    DomainId = domain.Id,
                    LeaderLabId = leaderId,
                    Footprint = bestFoot,
                    BestCapability = bestCap,
                });
            }
        }
    }
}