using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class DataSourceItemConfig : ISync
    {
        public string Id = "";
        public int WeeklyCost;
        public int DurationWeeks;
        /// <summary>Training quality permille bonus while active.</summary>
        public int QualityPermille;

        public void Sync(Syncer s)
        {
            s.Str("id", ref Id);
            s.Int("weeklyCost", ref WeeklyCost);
            s.Int("durationWeeks", ref DurationWeeks);
            s.Int("qualityPermille", ref QualityPermille);
        }
    }

    /// <summary>Named data sources with upkeep (Q458, schema 136). Player only.</summary>
    public sealed class DataSourcesConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public List<DataSourceItemConfig> Sources = new List<DataSourceItemConfig>();

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.ObjList("sources", ref Sources);
        }
    }
}