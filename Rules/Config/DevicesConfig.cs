using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Hardware side lines. Once the lab has a model worth putting in a box, it can design a device: an assistant speaker, smart glasses or a home robot. Designing takes weeks and money; testing before the launch buys quality; a launch under the bar flops on stage and a device that sells badly gets recalled. A good device sells a steady stream of units at the price tier the lab picks. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class DevicesConfig : ISync
    {
        public int Enabled;
        /// <summary>Capability of the best released model the lab needs to design a device.</summary>
        public int MinCapability;
        /// <summary>Index of the first era the speaker can be designed in.</summary>
        public int EraA;
        /// <summary>Index of the first era the glasses can be designed in.</summary>
        public int EraB;
        /// <summary>Index of the first era the home robot can be designed in.</summary>
        public int EraC;
        /// <summary>What designing the speaker costs.</summary>
        public int CostA;
        /// <summary>What designing the glasses costs.</summary>
        public int CostB;
        /// <summary>What designing the home robot costs.</summary>
        public int CostC;
        /// <summary>Weeks to design the speaker.</summary>
        public int WeeksA;
        /// <summary>Weeks to design the glasses.</summary>
        public int WeeksB;
        /// <summary>Weeks to design the home robot.</summary>
        public int WeeksC;
        /// <summary>Units the speaker sells a week at full quality and the middle price.</summary>
        public int DemandA;
        /// <summary>Units the glasses sell a week.</summary>
        public int DemandB;
        /// <summary>Units the home robot sells a week.</summary>
        public int DemandC;
        /// <summary>What the lab clears on a speaker at the middle price.</summary>
        public int MarginA;
        /// <summary>What it clears on glasses.</summary>
        public int MarginB;
        /// <summary>What it clears on a home robot.</summary>
        public int MarginC;
        /// <summary>Quality of a device before the model and the testing.</summary>
        public int BaseQuality;
        /// <summary>Most the model adds to the quality.</summary>
        public int ModelQuality;
        /// <summary>Capability at which the model adds its most.</summary>
        public int CapForMax;
        /// <summary>Quality each week of testing adds.</summary>
        public int TestQuality;
        /// <summary>The most weeks of testing.</summary>
        public int MaxTestWeeks;
        /// <summary>Quality a launch needs not to flop.</summary>
        public int LaunchBar;
        /// <summary>Reputation a flop costs.</summary>
        public int FlopReputation;
        /// <summary>Reputation a hit earns.</summary>
        public int HitReputation;
        /// <summary>Share of demand a flopped device keeps, in permille.</summary>
        public int FlopDemand;
        /// <summary>Share of the weekly demand lost each week on sale, in permille of the demand.</summary>
        public int FadePermille;
        /// <summary>Quality under which a device is recalled once it has sold a while.</summary>
        public int RecallBar;
        /// <summary>Weeks on sale before a poor device is recalled.</summary>
        public int RecallWeeks;
        /// <summary>What a recall costs for each device sold, by the speaker's scale (the others scale up).</summary>
        public int RecallCost;
        /// <summary>Tier 1 price, in permille of the middle price.</summary>
        public int LowPrice;
        /// <summary>Tier 3 price.</summary>
        public int HighPrice;
        /// <summary>Tier 1 demand, in permille of the middle price's.</summary>
        public int LowDemand;
        /// <summary>Tier 3 demand.</summary>
        public int HighDemand;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minCapability", ref MinCapability);
            s.Int("eraA", ref EraA);
            s.Int("eraB", ref EraB);
            s.Int("eraC", ref EraC);
            s.Int("costA", ref CostA);
            s.Int("costB", ref CostB);
            s.Int("costC", ref CostC);
            s.Int("weeksA", ref WeeksA);
            s.Int("weeksB", ref WeeksB);
            s.Int("weeksC", ref WeeksC);
            s.Int("demandA", ref DemandA);
            s.Int("demandB", ref DemandB);
            s.Int("demandC", ref DemandC);
            s.Int("marginA", ref MarginA);
            s.Int("marginB", ref MarginB);
            s.Int("marginC", ref MarginC);
            s.Int("baseQuality", ref BaseQuality);
            s.Int("modelQuality", ref ModelQuality);
            s.Int("capForMax", ref CapForMax);
            s.Int("testQuality", ref TestQuality);
            s.Int("maxTestWeeks", ref MaxTestWeeks);
            s.Int("launchBar", ref LaunchBar);
            s.Int("flopReputation", ref FlopReputation);
            s.Int("hitReputation", ref HitReputation);
            s.Int("flopDemand", ref FlopDemand);
            s.Int("fadePermille", ref FadePermille);
            s.Int("recallBar", ref RecallBar);
            s.Int("recallWeeks", ref RecallWeeks);
            s.Int("recallCost", ref RecallCost);
            s.Int("lowPrice", ref LowPrice);
            s.Int("highPrice", ref HighPrice);
            s.Int("lowDemand", ref LowDemand);
            s.Int("highDemand", ref HighDemand);
        }
    }
}
