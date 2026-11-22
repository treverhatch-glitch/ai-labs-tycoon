using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The training-data pipeline: units come in raw (scraped, licensed, synthetic or from the lab's own users), are cleaned in jobs, and the clean corpus lifts the capability of each new model. Scraped data brings claims once the era turns; scraped units can be licensed after the fact. Too much synthetic data makes models strange. A lab that never sources data has no corpus and none of this applies. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class DatasetsConfig : ISync
    {
        public int Enabled;
        /// <summary>Most units one purchase brings in.</summary>
        public int MaxBuy;
        /// <summary>Most units the lab can hold, raw and clean together.</summary>
        public int StockCap;
        /// <summary>Cost of a unit scraped from the web.</summary>
        public int CostScrape;
        /// <summary>Cost of a unit of licensed archives.</summary>
        public int CostLicense;
        /// <summary>Cost of a unit of synthetic data.</summary>
        public int CostSynth;
        /// <summary>Weeks one cleaning job takes.</summary>
        public int CleanWeeks;
        /// <summary>Most units one cleaning job takes.</summary>
        public int CleanMax;
        /// <summary>Cost of cleaning a unit.</summary>
        public int CleanCost;
        /// <summary>Units of a job each engineer covers.</summary>
        public int PerEngineer;
        /// <summary>Share of scraped units that survive cleaning (permille).</summary>
        public int YieldScrape;
        /// <summary>Same for licensed units.</summary>
        public int YieldLicense;
        /// <summary>Same for synthetic units.</summary>
        public int YieldSynth;
        /// <summary>Same for units from the lab own users.</summary>
        public int YieldUsers;
        /// <summary>How much a clean scraped unit is worth to a run (permille).</summary>
        public int QScrape;
        /// <summary>Same for licensed units.</summary>
        public int QLicense;
        /// <summary>Same for synthetic units.</summary>
        public int QSynth;
        /// <summary>Same for units from users.</summary>
        public int QUsers;
        /// <summary>Capability gain of a new model for each 100 worth of clean units (permille of the base of the line).</summary>
        public int BonusPer100;
        /// <summary>Most capability gain the corpus gives (permille).</summary>
        public int BonusCap;
        /// <summary>Share of synthetic units in the clean corpus above which models go strange (permille).</summary>
        public int CollapseShare;
        /// <summary>Bonus lost for each permille of synthetic share above that.</summary>
        public int CollapseSlope;
        /// <summary>Most of the bonus the collapse can take (permille).</summary>
        public int CollapseCap;
        /// <summary>Era (by place in the list) from which scraped data brings claims.</summary>
        public int SuitEra;
        /// <summary>Weeks between claims once they begin.</summary>
        public int SuitWeeks;
        /// <summary>Fine for each scraped unit held when a claim lands.</summary>
        public int SuitFine;
        /// <summary>Cost of licensing a scraped unit after the fact.</summary>
        public int BackfillCost;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxBuy", ref MaxBuy);
            s.Int("stockCap", ref StockCap);
            s.Int("costScrape", ref CostScrape);
            s.Int("costLicense", ref CostLicense);
            s.Int("costSynth", ref CostSynth);
            s.Int("cleanWeeks", ref CleanWeeks);
            s.Int("cleanMax", ref CleanMax);
            s.Int("cleanCost", ref CleanCost);
            s.Int("perEngineer", ref PerEngineer);
            s.Int("yieldScrape", ref YieldScrape);
            s.Int("yieldLicense", ref YieldLicense);
            s.Int("yieldSynth", ref YieldSynth);
            s.Int("yieldUsers", ref YieldUsers);
            s.Int("qScrape", ref QScrape);
            s.Int("qLicense", ref QLicense);
            s.Int("qSynth", ref QSynth);
            s.Int("qUsers", ref QUsers);
            s.Int("bonusPer100", ref BonusPer100);
            s.Int("bonusCap", ref BonusCap);
            s.Int("collapseShare", ref CollapseShare);
            s.Int("collapseSlope", ref CollapseSlope);
            s.Int("collapseCap", ref CollapseCap);
            s.Int("suitEra", ref SuitEra);
            s.Int("suitWeeks", ref SuitWeeks);
            s.Int("suitFine", ref SuitFine);
            s.Int("backfillCost", ref BackfillCost);
        }
    }
}
