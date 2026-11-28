using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Bonus duration weeks on first data-source acquire in a late era (Q513, schema 180).</summary>
    public sealed class DataSourceAcquireEraWeeksConfig : ISync
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