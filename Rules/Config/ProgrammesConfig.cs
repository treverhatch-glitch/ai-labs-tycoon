using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Multi-stage research programmes. A programme aims at one costly tech and goes through stages; each stage is run carefully (dearer, slower, certain) or boldly (cheaper, quicker, may fail). A failed bold stage waits for a decision: retry it, fall back to the careful way, or give up and take part of the points back. A finished programme gives the tech and pays back research points and prestige for each bold stage that worked. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ProgrammesConfig : ISync
    {
        public int Enabled;
        /// <summary>Stages a programme has to get through (1 to 6).</summary>
        public int Stages;
        /// <summary>Research points a tech must cost to be worth a programme.</summary>
        public int MinCost;
        /// <summary>Weeks a careful stage takes.</summary>
        public int SafeWeeks;
        /// <summary>Weeks a bold stage takes.</summary>
        public int BoldWeeks;
        /// <summary>What a careful stage costs, as a share of the tech cost (permille).</summary>
        public int SafeCostPermille;
        /// <summary>What a bold stage costs, as a share of the tech cost (permille).</summary>
        public int BoldCostPermille;
        /// <summary>Chance a bold stage works (permille).</summary>
        public int BoldChancePermille;
        /// <summary>Research points paid back at the end for each bold stage that worked, as a share of the tech cost.</summary>
        public int InsightPermille;
        /// <summary>Prestige each bold stage that worked adds at the end.</summary>
        public int InsightPrestige;
        /// <summary>Share of the points spent that comes back when a programme is abandoned.</summary>
        public int RefundPermille;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("stages", ref Stages);
            s.Int("minCost", ref MinCost);
            s.Int("safeWeeks", ref SafeWeeks);
            s.Int("boldWeeks", ref BoldWeeks);
            s.Int("safeCostPermille", ref SafeCostPermille);
            s.Int("boldCostPermille", ref BoldCostPermille);
            s.Int("boldChancePermille", ref BoldChancePermille);
            s.Int("insightPermille", ref InsightPermille);
            s.Int("insightPrestige", ref InsightPrestige);
            s.Int("refundPermille", ref RefundPermille);
        }
    }
}
