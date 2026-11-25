using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra weeks when renewing a data source in a late era (Q491, schema 162).</summary>
    public sealed class DataSourceRenewEraBonusConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int BonusWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("bonusWeeks", ref BonusWeeks);
        }
    }
}