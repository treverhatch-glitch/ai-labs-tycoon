using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Harness-era device workshop: appeal bump in the consumer devices sector once the lab has a released product there.</summary>
    public static class DeviceWorkshopSystem
    {
        public static bool HasRoom(LabState lab, string roomTypeId)
        {
            for (int i = 0; i < lab.Rooms.Count; i++)
                if (lab.Rooms[i].TypeId == roomTypeId) return true;
            return false;
        }

        public static bool HasReleasedInSector(LabState lab, string sectorId)
        {
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Released) continue;
                for (int k = 0; k < model.Sectors.Count; k++)
                    if (model.Sectors[k] == sectorId) return true;
            }
            return false;
        }

        public static int AppealBonusPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            return WorkshopBonusPermille(c, state, lab, sectorId, c.Market.DeviceWorkshop?.AppealBonusPermille ?? 0);
        }

        public static int RevenueBonusPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            return WorkshopBonusPermille(c, state, lab, sectorId, c.Market.DeviceWorkshop?.RevenueBonusPermille ?? 0);
        }

        /// <summary>Training prep discount when the lab runs a consumer-devices programme.</summary>
        public static int PrepCostCutPermille(GameConfig c, GameState state, LabState lab)
        {
            DeviceWorkshopConfig workshop = c.Market.DeviceWorkshop;
            if (workshop == null) return 0;
            return WorkshopBonusPermille(c, state, lab, workshop.SectorId, workshop.PrepCostCutPermille);
        }

        static int WorkshopBonusPermille(GameConfig c, GameState state, LabState lab, string sectorId, int bonus)
        {
            if (lab.Bankrupt || bonus <= 0) return 0;
            DeviceWorkshopConfig workshop = c.Market.DeviceWorkshop;
            if (workshop == null) return 0;
            if (sectorId != workshop.SectorId) return 0;
            if (!EraSystem.Open(c, workshop.EraId, state.Tick)) return 0;
            if (!HasRoom(lab, workshop.RoomTypeId)) return 0;
            if (!HasReleasedInSector(lab, workshop.SectorId)) return 0;
            return bonus;
        }
    }
}