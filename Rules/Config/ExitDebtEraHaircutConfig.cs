using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Haircut debt subtracted from exit offers in a late era (Q507, schema 175).</summary>
    public sealed class ExitDebtEraHaircutConfig : ISync
    {
        public int Enabled;
        public int MinEra;
        public int HaircutPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("haircutPermille", ref HaircutPermille);
        }
    }
}