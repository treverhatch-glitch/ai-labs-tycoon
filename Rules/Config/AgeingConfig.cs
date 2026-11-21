using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Rooms that age (bible ch. 14: "old rooms visibly go out of date: beige computers, dusty racks, staff complaining, giving a reason to
    /// rebuild"). A room gets one era older every time a new era begins. The older it is, the more it costs to keep and the more the people who
    /// sit in it complain, until the lab pays to refurbish it. Only the player's rooms age; rivals' rooms stay as they are.
    /// </summary>
    public sealed class AgeingConfig : ISync
    {
        /// <summary>What a refurbishment costs, in permille of what the room cost to build.</summary>
        public int RefurbishCostPermille;
        /// <summary>The most morale the lab's dated rooms can take off where its people settle.</summary>
        public int MaxMoralePenalty;
        public List<AgeStageConfig> Stages = new List<AgeStageConfig>();

        public void Sync(Syncer s)
        {
            s.Int("refurbishCostPermille", ref RefurbishCostPermille);
            s.Int("maxMoralePenalty", ref MaxMoralePenalty);
            s.ObjList("stages", ref Stages);
        }
    }

    /// <summary>How a room looks and what it costs once it is this many eras behind: the stage with the highest <see cref="MinEras"/> at or below its age.</summary>
    public sealed class AgeStageConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public int MinEras;
        /// <summary>More upkeep, in permille of the room's normal upkeep.</summary>
        public int UpkeepExtraPermille;
        /// <summary>Morale the people in rooms this old lose, as a share of where they would settle (counted by tiles of the rooms that seat people).</summary>
        public int MoralePenalty;
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("minEras", ref MinEras);
            s.Int("upkeepExtraPermille", ref UpkeepExtraPermille);
            s.Int("moralePenalty", ref MoralePenalty);
            s.Str("blurb", ref Blurb);
        }
    }
}
