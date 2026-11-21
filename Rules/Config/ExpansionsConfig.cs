using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>Expansion-era kits: special rooms that boost appeal in named sectors once the era is live.</summary>
    public sealed class ExpansionsConfig : ISync
    {
        public ExpansionEraKitConfig Bodies;
        public ExpansionEraKitConfig Upward;

        public void Sync(Syncer s)
        {
            s.Obj("bodies", ref Bodies);
            s.Obj("upward", ref Upward);
        }
    }

    public sealed class ExpansionEraKitConfig : ISync
    {
        public string EraId = "";
        public string RoomTypeId = "";
        public List<string> SectorIds = new List<string>();
        /// <summary>Extra appeal in permille when the lab has the room and the sector matches.</summary>
        public int AppealBonusPermille;
        /// <summary>Finished runs on this kit get capability ×(1000 + this)‰ once the era is live and the room is built.</summary>
        public int CapabilityBonusPermille;
        /// <summary>Training speed permille added while the era is live and the room is built (orbital uplink).</summary>
        public int TrainingSpeedBonusPermille;
        /// <summary>Each uplink tile adds general compute × current GPU week × this permille (orbit carries signal, not racks).</summary>
        public int PassiveComputePermillePerTile;

        public void Sync(Syncer s)
        {
            s.Str("eraId", ref EraId);
            s.Str("roomTypeId", ref RoomTypeId);
            s.StrList("sectorIds", ref SectorIds);
            s.Int("appealBonusPermille", ref AppealBonusPermille);
            s.Int("capabilityBonusPermille", ref CapabilityBonusPermille);
            s.Int("trainingSpeedBonusPermille", ref TrainingSpeedBonusPermille);
            s.Int("passiveComputePermillePerTile", ref PassiveComputePermillePerTile);
        }
    }
}