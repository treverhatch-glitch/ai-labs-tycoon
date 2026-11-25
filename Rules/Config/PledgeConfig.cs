using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The public safety pledge. The lab promises that every model it launches will show at least some alignment, at a level from 1 to the top. While the pledge holds, customers
    /// trust the lab a little more every week in every sector it sells in. A launch that shows less than the pledge breaks it: the level drops to nothing, reputation and trust fall,
    /// and the pledge cannot be made again for a while. Walking a pledge back by hand costs reputation. The pledge is judged on the alignment a launch shows, not the one it has, so
    /// a lab that overstates to keep its pledge is the whistleblower's business. Only the player's lab pledges. Zero in <c>MaxLevel</c> turns it off.
    /// </summary>
    public sealed class PledgeConfig : ISync
    {
        /// <summary>The top level; 0 turns pledges off.</summary>
        public int MaxLevel;
        /// <summary>The alignment a level 1 pledge asks of a launch.</summary>
        public int BaseFloor;
        /// <summary>How much more alignment each level above 1 asks.</summary>
        public int FloorStep;
        /// <summary>Extra trust a sector gives the lab every week for each level.</summary>
        public int TrustPerLevel;
        /// <summary>Reputation lost when a launch breaks the pledge.</summary>
        public int BreachReputation;
        /// <summary>Trust lost in every sector the lab sells in when a launch breaks the pledge.</summary>
        public int BreachTrust;
        /// <summary>Reputation lost by lowering or dropping a pledge on purpose.</summary>
        public int WalkBackReputation;
        /// <summary>Weeks after a pledge is made, walked back or broken before the level can change.</summary>
        public int LockWeeks;

        public void Sync(Syncer s)
        {
            s.Int("maxLevel", ref MaxLevel);
            s.Int("baseFloor", ref BaseFloor);
            s.Int("floorStep", ref FloorStep);
            s.Int("trustPerLevel", ref TrustPerLevel);
            s.Int("breachReputation", ref BreachReputation);
            s.Int("breachTrust", ref BreachTrust);
            s.Int("walkBackReputation", ref WalkBackReputation);
            s.Int("lockWeeks", ref LockWeeks);
        }
    }
}
