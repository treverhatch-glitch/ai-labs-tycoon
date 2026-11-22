using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public sealed class SpecialistKindConfig : ISync
    {
        public string Id = "";
        public List<string> Sectors = new List<string>();

        public void Sync(Syncer s)
        {
            s.Str("id", ref Id);
            s.StrList("sectors", ref Sectors);
        }
    }

    /// <summary>Branch a trained model into a domain specialist (Q457, schema 135). Player only.</summary>
    public sealed class SpecialistLinesConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int Cost;
        /// <summary>Extra appeal permille in the specialist's sectors.</summary>
        public int BoostPermille;
        /// <summary>Appeal cut permille outside those sectors.</summary>
        public int PenaltyPermille;
        public List<SpecialistKindConfig> Kinds = new List<SpecialistKindConfig>();

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("cost", ref Cost);
            s.Int("boostPermille", ref BoostPermille);
            s.Int("penaltyPermille", ref PenaltyPermille);
            s.ObjList("kinds", ref Kinds);
        }
    }
}