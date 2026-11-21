using System.Collections.Generic;
using AiLabs.Rules.Config;

namespace AiLabs.Rules.Systems
{
    /// <summary>Specialist model lines get a quality bonus in their home sectors only.</summary>
    public static class SpecialistSystem
    {
        public static int QualityBonusPermille(GameConfig c, string lineId, string sectorId)
        {
            ModelLineConfig line = c.Line(lineId);
            if (line == null || line.SpecialistQualityBonusPermille <= 0) return 0;
            List<string> sectors = line.SpecialistSectorIds;
            for (int i = 0; i < sectors.Count; i++)
                if (sectors[i] == sectorId) return line.SpecialistQualityBonusPermille;
            return 0;
        }
    }
}