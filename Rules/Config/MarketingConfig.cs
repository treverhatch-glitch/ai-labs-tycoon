using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A marketing campaign in one sector: for the data's weeks the lab's model there is more appealing, then for a while customers feel let
    /// down and it is less appealing than before. The boost is biggest where customers do not know the lab yet (it shrinks as the lab's
    /// share in the sector grows). An incident in that sector during the campaign ends it at once and costs reputation. One campaign at
    /// a time, then a cooldown. Only the player's lab runs campaigns. Zero levels turns it off.
    /// </summary>
    public sealed class MarketingConfig : ISync
    {
        /// <summary>The biggest budget level. Zero turns campaigns off.</summary>
        public int MaxLevel;
        /// <summary>Cash for each level, paid up front.</summary>
        public long CostPerLevel;
        /// <summary>How many weeks it runs.</summary>
        public int Weeks;
        /// <summary>The appeal each level adds at a share of nothing, in permille.</summary>
        public int AppealPermillePerLevel;
        /// <summary>The share of the lab in the sector, in permille of the sector, above which the campaign adds nothing more: the boost falls in a line to zero here.</summary>
        public int ShareCeilingPermille;
        /// <summary>The weeks customers feel let down once a campaign ends.</summary>
        public int HangoverWeeks;
        /// <summary>The appeal each level takes off during the hangover, in permille.</summary>
        public int HangoverPermillePerLevel;
        /// <summary>The reputation an incident in the sector costs during a campaign, on top of its usual loss.</summary>
        public int BacklashReputation;
        /// <summary>The weeks after a campaign starts before another can.</summary>
        public int CooldownWeeks;
        /// <summary>The sales people the lab needs to run one.</summary>
        public int MinSales;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Long("costPerLevel", ref CostPerLevel);
            s.Int("weeks", ref Weeks);
            s.Int("appealPermillePerLevel", ref AppealPermillePerLevel);
            s.Int("shareCeilingPermille", ref ShareCeilingPermille);
            s.Int("hangoverWeeks", ref HangoverWeeks);
            s.Int("hangoverPermillePerLevel", ref HangoverPermillePerLevel);
            s.Int("backlashReputation", ref BacklashReputation);
            s.Int("cooldownWeeks", ref CooldownWeeks);
            s.Int("minSales", ref MinSales);
        }
    }
}
