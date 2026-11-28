using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Discount on data-source renew payment in late era (Q517, schema 183).</summary>
    public sealed class DataSourceRenewEraDiscountConfig : ISync
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