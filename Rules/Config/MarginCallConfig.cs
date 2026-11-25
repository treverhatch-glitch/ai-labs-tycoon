using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Margin call when GPU collateral falls below loan value (Q468, schema 144).</summary>
    public sealed class MarginCallConfig : ISync
    {
        public int Enabled;
        /// <summary>Minimum collateral permille per dollar owed before a call fires.</summary>
        public int CovenantPermille;
        /// <summary>Weeks to satisfy a call before the lender accelerates default.</summary>
        public int CureWeeks;
        /// <summary>Fraction of outstanding the lab can pay in cash instead of selling GPUs.</summary>
        public int PayPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("covenantPermille", ref CovenantPermille);
            s.Int("cureWeeks", ref CureWeeks);
            s.Int("payPermille", ref PayPermille);
        }
    }
}