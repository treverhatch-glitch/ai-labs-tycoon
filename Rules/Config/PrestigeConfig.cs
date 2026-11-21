using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Published papers (bible ch. 7: researchers want to publish). The lab spends research points to put a paper out. It earns a little
    /// reputation at once and builds the lab's academic prestige, which fades again every week. While it lasts, people are happier and hiring
    /// is cheaper and counter-offers work better, but rivals find the lab's people more tempting. Zero papers and zero prestige change nothing.
    /// </summary>
    public sealed class PrestigeConfig : ISync
    {
        /// <summary>The most prestige a lab can hold. Zero turns papers off.</summary>
        public int Max;
        /// <summary>Prestige one paper adds (never past the most).</summary>
        public int PaperPrestige;
        /// <summary>Prestige that fades every week.</summary>
        public int DecayPerWeek;
        /// <summary>A paper costs this many weeks of the lab's research output at the time.</summary>
        public int PaperResearchWeeks;
        /// <summary>But never less than this many research points.</summary>
        public long MinPoints;
        /// <summary>Researchers the lab needs on the payroll to write one.</summary>
        public int MinResearchers;
        /// <summary>Reputation points a paper adds at once.</summary>
        public int PaperReputation;
        /// <summary>At full prestige: morale added to where people settle.</summary>
        public int MoraleAtMax;
        /// <summary>At full prestige: the share off hiring fees, in permille.</summary>
        public int HireDiscountAtMax;
        /// <summary>At full prestige: the share a rival's offers are likelier, in permille.</summary>
        public int PoachAtMax;
        /// <summary>At full prestige: points added to the chance a counter-offer works, in permille.</summary>
        public int CounterAtMax;

        public void Sync(Syncer s)
        {
            s.Int("max", ref Max);
            s.Int("paperPrestige", ref PaperPrestige);
            s.Int("decayPerWeek", ref DecayPerWeek);
            s.Int("paperResearchWeeks", ref PaperResearchWeeks);
            s.Long("minPoints", ref MinPoints);
            s.Int("minResearchers", ref MinResearchers);
            s.Int("paperReputation", ref PaperReputation);
            s.Int("moraleAtMax", ref MoraleAtMax);
            s.Int("hireDiscountAtMax", ref HireDiscountAtMax);
            s.Int("poachAtMax", ref PoachAtMax);
            s.Int("counterAtMax", ref CounterAtMax);
        }
    }
}
