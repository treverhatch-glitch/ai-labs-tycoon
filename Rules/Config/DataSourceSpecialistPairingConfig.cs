using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class DataSourceSpecialistPairingItemConfig : ISync
    {
        public string SpecialistKindId = "";
        public string DataSourceId = "";
        public int BonusPermille;

        public void Sync(Syncer s)
        {
            s.Str("specialistKindId", ref SpecialistKindId);
            s.Str("dataSourceId", ref DataSourceId);
            s.Int("bonusPermille", ref BonusPermille);
        }
    }

    /// <summary>Extra training quality when a specialist kind matches an active data source (Q483, schema 156).</summary>
    public sealed class DataSourceSpecialistPairingConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public List<DataSourceSpecialistPairingItemConfig> Pairs = new List<DataSourceSpecialistPairingItemConfig>();

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.ObjList("pairs", ref Pairs);
        }
    }
}