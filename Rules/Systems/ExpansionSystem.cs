using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Bodies and Upward expansion rooms give a sector appeal bonus once their era has begun.</summary>
    public static class ExpansionSystem
    {
        public static bool HasRoom(LabState lab, string roomTypeId)
        {
            for (int i = 0; i < lab.Rooms.Count; i++)
                if (lab.Rooms[i].TypeId == roomTypeId) return true;
            return false;
        }

        public static int AppealBonusPermille(GameConfig c, GameState state, LabState lab, string sectorId)
        {
            if (c.Expansions == null) return 0;
            int bonus = KitBonus(c, state, lab, sectorId, c.Expansions.Bodies);
            if (bonus > 0) return bonus;
            return KitBonus(c, state, lab, sectorId, c.Expansions.Upward);
        }

        static int KitBonus(GameConfig c, GameState state, LabState lab, string sectorId, ExpansionEraKitConfig kit)
        {
            if (kit == null || kit.AppealBonusPermille <= 0) return 0;
            if (!EraSystem.Open(c, kit.EraId, state.Tick)) return 0;
            if (!HasRoom(lab, kit.RoomTypeId)) return 0;
            for (int i = 0; i < kit.SectorIds.Count; i++)
                if (kit.SectorIds[i] == sectorId) return kit.AppealBonusPermille;
            return 0;
        }

        /// <summary>Whole-number multiplier applied to appeal after crown boost (permille added to 1000).</summary>
        public static long ScaleAppeal(long appeal, int bonusPermille)
        {
            if (bonusPermille <= 0) return appeal;
            return MathI.MulPermille(appeal, MathI.Permille + bonusPermille);
        }

        public static int CapabilityBonusPermille(GameConfig c, GameState state, LabState lab)
        {
            if (c.Expansions == null) return 0;
            int bonus = KitCapability(c, state, lab, c.Expansions.Bodies);
            if (bonus > 0) return bonus;
            return KitCapability(c, state, lab, c.Expansions.Upward);
        }

        public static int TrainingSpeedBonusPermille(GameConfig c, GameState state, LabState lab)
        {
            if (c.Expansions == null) return 0;
            int bonus = KitSpeed(c, state, lab, c.Expansions.Upward);
            if (bonus > 0) return bonus;
            return KitSpeed(c, state, lab, c.Expansions.Bodies);
        }

        static int KitCapability(GameConfig c, GameState state, LabState lab, ExpansionEraKitConfig kit)
        {
            if (kit == null || kit.CapabilityBonusPermille <= 0) return 0;
            if (!EraSystem.Open(c, kit.EraId, state.Tick)) return 0;
            if (!HasRoom(lab, kit.RoomTypeId)) return 0;
            return kit.CapabilityBonusPermille;
        }

        static int KitSpeed(GameConfig c, GameState state, LabState lab, ExpansionEraKitConfig kit)
        {
            if (kit == null || kit.TrainingSpeedBonusPermille <= 0) return 0;
            if (!EraSystem.Open(c, kit.EraId, state.Tick)) return 0;
            if (!HasRoom(lab, kit.RoomTypeId)) return 0;
            return kit.TrainingSpeedBonusPermille;
        }
    }
}