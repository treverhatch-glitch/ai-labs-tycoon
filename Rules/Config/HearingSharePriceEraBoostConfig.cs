using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Extra bubble nudge on positive hearing answers in late era (Q498, schema 167).</summary>
    public sealed class HearingSharePriceEraBoostConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int BoostPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("boostPermille", ref BoostPermille);
        }
    }
}