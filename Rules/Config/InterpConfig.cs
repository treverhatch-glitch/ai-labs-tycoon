using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The interpretability programme (bible ch. 10: "research into what the model is actually doing inside"). A standing team that reads
    /// the inside of the models on sale: it slowly closes the gap between the shown and the real alignment, and keeps older safety
    /// techs from going stale as quickly. It costs a weekly bill and some research, for as long as it runs.
    /// </summary>
    public sealed class InterpConfig : ISync
    {
        /// <summary>The tech the lab needs for each level, level 1 first. The highest level the game has is how many ids there are; empty means no programme.</summary>
        public List<string> LevelTechs = new List<string>();
        /// <summary>The weekly bill for each level, on the training ledger line.</summary>
        public long WeeklyCostPerLevel;
        /// <summary>The share of the gap between shown and real alignment each level closes every week, in permille (at least one point while a gap remains).</summary>
        public int GapClosePermillePerLevel;
        /// <summary>Research each level takes from the lab's research output, in permille.</summary>
        public int ResearchCostPermillePerLevel;
        /// <summary>The share of the loss from stale safety techs each level takes away, in permille.</summary>
        public int StaleReliefPermillePerLevel;

        public int MaxLevel { get { return LevelTechs.Count; } }

        public void Sync(Syncer s)
        {
            s.StrList("levelTechs", ref LevelTechs);
            s.Long("weeklyCostPerLevel", ref WeeklyCostPerLevel);
            s.Int("gapClosePermillePerLevel", ref GapClosePermillePerLevel);
            s.Int("researchCostPermillePerLevel", ref ResearchCostPermillePerLevel);
            s.Int("staleReliefPermillePerLevel", ref StaleReliefPermillePerLevel);
        }
    }
}
