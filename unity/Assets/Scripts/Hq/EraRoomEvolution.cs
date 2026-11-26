using AiLabs.Rules.Config;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>
    /// How far a room type has "aged" since its era: older room types look more modern as the game era advances
    /// (presentation only; room mechanics are unchanged).
    /// </summary>
    public static class EraRoomEvolution
    {
        public static int Step(GameConfig config, string roomEraId, string currentEraId)
        {
            int roomIndex = EraIndex(config, roomEraId);
            int currentIndex = EraIndex(config, currentEraId);
            if (roomIndex < 0 || currentIndex < 0) return 0;
            int delta = currentIndex - roomIndex;
            if (delta < 0) return 0;
            return delta > 8 ? 8 : delta;
        }

        public static float HeightScale(int evolutionStep)
        {
            return 1f + evolutionStep * 0.035f;
        }

        public static Color EvolvedRoomColor(GameConfig config, string roomTypeId, HqVisualPalette.EraSkin skin, string currentEraId)
        {
            RoomTypeConfig type = config.RoomType(roomTypeId);
            string roomEra = type != null ? type.Era : currentEraId;
            int step = Step(config, roomEra, currentEraId);
            Color baseColor = HqVisualPalette.RoomColor(config, roomTypeId);
            float lerp = Mathf.Clamp(0.12f + step * 0.028f, 0.12f, 0.38f);
            Color modern = Color.Lerp(baseColor, skin.Accent, lerp);
            return Color.Lerp(modern, skin.Ambient, step * 0.02f);
        }

        static int EraIndex(GameConfig config, string eraId)
        {
            for (int i = 0; i < config.Eras.Count; i++)
                if (config.Eras[i].Id == eraId) return i;
            return -1;
        }
    }
}