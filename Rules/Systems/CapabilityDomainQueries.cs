using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Groups sectors into bible-style capability domains for explainable leaderboards.</summary>
    public static class CapabilityDomainQueries
    {
        public static CapabilityDomainConfig Domain(GameConfig c, string domainId) => c.Market.Domain(domainId);

        /// <summary>Best released capability among models shipping in any sector of the domain.</summary>
        public static int BestCapability(GameConfig c, LabState lab, string domainId)
        {
            CapabilityDomainConfig domain = Domain(c, domainId);
            if (domain == null || lab.Bankrupt) return 0;
            int best = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released || model.Capability < 1) continue;
                if (!ModelInDomain(model, domain)) continue;
                if (model.Capability > best) best = model.Capability;
            }
            return best;
        }

        /// <summary>Sum of appeal-ready quality scores for released models in the domain's sectors this week.</summary>
        public static long DomainFootprint(GameConfig c, GameState state, LabState lab, string domainId)
        {
            CapabilityDomainConfig domain = Domain(c, domainId);
            if (domain == null || lab.Bankrupt) return 0;
            long total = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released) continue;
                for (int k = 0; k < domain.SectorIds.Count; k++)
                {
                    string sectorId = domain.SectorIds[k];
                    if (!ModelServesSector(model, sectorId)) continue;
                    SectorConfig sector = c.Sector(sectorId);
                    SectorState sectorState = state.Sector(sectorId);
                    if (sector == null || sectorState == null) continue;
                    total += LabQueries.Quality(c, sector, model, state.Tick);
                }
            }
            return total;
        }

        static bool ModelInDomain(ModelState model, CapabilityDomainConfig domain)
        {
            for (int k = 0; k < domain.SectorIds.Count; k++)
                if (ModelServesSector(model, domain.SectorIds[k])) return true;
            return false;
        }

        static bool ModelServesSector(ModelState model, string sectorId)
        {
            for (int i = 0; i < model.Sectors.Count; i++)
                if (model.Sectors[i] == sectorId) return true;
            return false;
        }
    }
}