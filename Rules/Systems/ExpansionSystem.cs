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

        public static int CapabilityBonusPermille(GameConfig c, GameState state, LabState lab, string lineId)
        {
            if (c.Expansions == null) return 0;
            int bonus = KitCapability(c, state, lab, lineId, c.Expansions.Bodies);
            if (bonus > 0) return bonus;
            return KitCapability(c, state, lab, lineId, c.Expansions.Upward);
        }

        public static int TrainingSpeedBonusPermille(GameConfig c, GameState state, LabState lab)
        {
            if (c.Expansions == null) return 0;
            int bonus = KitSpeed(c, state, lab, c.Expansions.Upward);
            if (bonus > 0) return bonus;
            return KitSpeed(c, state, lab, c.Expansions.Bodies);
        }

        static int KitCapability(GameConfig c, GameState state, LabState lab, string lineId, ExpansionEraKitConfig kit)
        {
            if (kit == null || kit.CapabilityBonusPermille <= 0) return 0;
            if (!EraSystem.Open(c, kit.EraId, state.Tick)) return 0;
            if (!HasRoom(lab, kit.RoomTypeId)) return 0;
            if (!LineMatchesKitCapability(c, lineId, kit)) return 0;
            return kit.CapabilityBonusPermille;
        }

        /// <summary>General lines always match; specialist lines need a specialist sector on the kit list.</summary>
        static bool LineMatchesKitCapability(GameConfig c, string lineId, ExpansionEraKitConfig kit)
        {
            ModelLineConfig line = c.Line(lineId);
            if (line == null) return false;
            if (line.SpecialistSectorIds.Count == 0) return true;
            for (int i = 0; i < line.SpecialistSectorIds.Count; i++)
            {
                string sector = line.SpecialistSectorIds[i];
                for (int k = 0; k < kit.SectorIds.Count; k++)
                    if (kit.SectorIds[k] == sector) return true;
            }
            return false;
        }

        static int KitSpeed(GameConfig c, GameState state, LabState lab, ExpansionEraKitConfig kit)
        {
            if (kit == null || kit.TrainingSpeedBonusPermille <= 0) return 0;
            if (!EraSystem.Open(c, kit.EraId, state.Tick)) return 0;
            if (!HasRoom(lab, kit.RoomTypeId)) return 0;
            return kit.TrainingSpeedBonusPermille;
        }

        /// <summary>Expansion rooms that define passive compute contribute general power before fleet batches are counted.</summary>
        public static long PassiveExpansionGeneralCompute(GameConfig c, GameState state, LabState lab)
        {
            if (c.Expansions == null) return 0;
            long total = PassiveKitGeneralCompute(c, state, lab, c.Expansions.Bodies);
            total += PassiveKitGeneralCompute(c, state, lab, c.Expansions.Upward);
            return total;
        }

        /// <summary>Orbital uplink tiles only (tests and breakdowns that isolate upward).</summary>
        public static long PassiveOrbitalGeneralCompute(GameConfig c, GameState state, LabState lab)
        {
            return c.Expansions == null ? 0 : PassiveKitGeneralCompute(c, state, lab, c.Expansions.Upward);
        }

        static long PassiveKitGeneralCompute(GameConfig c, GameState state, LabState lab, ExpansionEraKitConfig kit)
        {
            if (kit == null || kit.PassiveComputePermillePerTile <= 0) return 0;
            if (!EraSystem.Open(c, kit.EraId, state.Tick)) return 0;
            int tiles = 0;
            for (int i = 0; i < lab.Rooms.Count; i++)
                if (lab.Rooms[i].TypeId == kit.RoomTypeId) tiles += lab.Rooms[i].Tiles;
            if (tiles <= 0) return 0;
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            long baseCompute = tiles * (long)gen.ComputePerGpuWeek;
            return MathI.MulPermille(baseCompute, kit.PassiveComputePermillePerTile);
        }
    }
}