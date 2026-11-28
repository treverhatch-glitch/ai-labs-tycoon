using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Publish discount on public metrics in late era (Q521, schema 186).</summary>
    public sealed class PublicMetricsEraDiscountConfig : ISync
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