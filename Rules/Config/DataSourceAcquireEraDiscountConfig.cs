using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Discount first acquire payment on data sources in late era (Q500, schema 169).</summary>
    public sealed class DataSourceAcquireEraDiscountConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int DiscountPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("discountPermille", ref DiscountPermille);
        }
    }
}