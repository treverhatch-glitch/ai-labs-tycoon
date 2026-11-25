using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Cut specialist branch cost once era allows (Q495, schema 165).</summary>
    public sealed class SpecialistBranchEraDiscountConfig : ISync
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