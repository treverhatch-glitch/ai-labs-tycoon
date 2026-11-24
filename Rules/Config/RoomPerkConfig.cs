using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>What a room does for the lab beyond seating people or holding GPUs. Each kind is one rule read from the rooms a lab has today.</summary>
    public enum RoomPerkKind
    {
        /// <summary>Researchers earn more research points.</summary>
        ResearchSpeed,
        /// <summary>A unit of training compute does more work.</summary>
        TrainingSpeed,
        /// <summary>Catches a share of incidents before they happen (added to the red-teamers' share).</summary>
        IncidentCut,
        /// <summary>Raises the appeal of the lab's models in the perk's sectors.</summary>
        SectorAppeal,
        /// <summary>Takes a share off every reputation a scandal costs (an incident, a hearing) and helps the lab's answers to the press.</summary>
        ScandalDamping,
        /// <summary>A launch earns reputation: the press room's share of the launch buzz points.</summary>
        LaunchBuzz,
        /// <summary>Raises the level every staff member's morale settles at, and the level a new hire starts at (in morale points per tile, not permille).</summary>
        MoraleLift,
        /// <summary>Takes a share off the chance a rival's offer arrives (the rooms where people would rather stay).</summary>
        PoachDefence
    }

    /// <summary>
    /// A perk of a room type: every tile of that room adds <see cref="PerTilePermille"/>, up to <see cref="CapPermille"/>.
    /// Perks of one kind add up. Worked out from the rooms each time it is asked, so nothing is saved.
    /// </summary>
    public sealed class RoomPerkConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string RoomId = "";
        public RoomPerkKind Kind;
        public int PerTilePermille;
        public int CapPermille;
        /// <summary>The sectors a sector perk applies to; empty means every sector (and is the only value for other kinds).</summary>
        public List<string> Sectors = new List<string>();
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("roomId", ref RoomId);
            s.Enum("kind", ref Kind);
            s.Int("perTilePermille", ref PerTilePermille);
            s.Int("capPermille", ref CapPermille);
            s.StrList("sectors", ref Sectors);
            s.Str("blurb", ref Blurb);
        }
    }
}
