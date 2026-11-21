using System.Collections.Generic;
using AiLabs.Rules.Config;

namespace AiLabs.Rules.Systems
{
    /// <summary>Shared safety lookups. Era-content owns <c>safety.sectorFlavours</c> economics separately.</summary>
    public static class SafetyQueries
    {
        /// <summary>Headline lines for a sector, or null to use the era list. Text only; no fines or trust.</summary>
        public static List<string> SectorIncidentHeadlines(GameConfig c, string sectorId)
        {
            if (string.IsNullOrEmpty(sectorId) || c.Safety.SectorIncidentHeadlines == null) return null;
            for (int i = 0; i < c.Safety.SectorIncidentHeadlines.Count; i++)
            {
                SectorIncidentHeadlineConfig flavour = c.Safety.SectorIncidentHeadlines[i];
                if (flavour.SectorId != sectorId) continue;
                if (flavour.Headlines == null || flavour.Headlines.Count == 0) return null;
                return flavour.Headlines;
            }
            return null;
        }
    }
}