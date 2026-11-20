using AiLabs.Rules.Config;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>
    /// Presentation-only colours for the HQ cutaway. Era skins can change floor and accent materials later;
    /// room blocks reserve an emission channel for the model glow that will propagate through the building.
    /// </summary>
    public static class HqVisualPalette
    {
        public struct EraSkin
        {
            public Color Floor;
            public Color GridLine;
            public Color Accent;
            public Color Ambient;
        }

        public static EraSkin EraSkinFor(string eraId)
        {
            if (eraId == "dial-up-days")
                return new EraSkin
                {
                    Floor = new Color(0.22f, 0.2f, 0.18f),
                    GridLine = new Color(0.35f, 0.32f, 0.28f),
                    Accent = new Color(0.55f, 0.45f, 0.35f),
                    Ambient = new Color(0.45f, 0.42f, 0.38f)
                };
            if (eraId == "the-punchline")
                return new EraSkin
                {
                    Floor = new Color(0.16f, 0.18f, 0.22f),
                    GridLine = new Color(0.28f, 0.32f, 0.38f),
                    Accent = new Color(0.35f, 0.55f, 0.75f),
                    Ambient = new Color(0.55f, 0.65f, 0.72f)
                };
            if (eraId == "it-sees")
                return new EraSkin
                {
                    Floor = new Color(0.14f, 0.17f, 0.2f),
                    GridLine = new Color(0.25f, 0.35f, 0.4f),
                    Accent = new Color(0.3f, 0.7f, 0.65f),
                    Ambient = new Color(0.5f, 0.72f, 0.68f)
                };
            if (eraId == "it-writes")
                return new EraSkin
                {
                    Floor = new Color(0.12f, 0.14f, 0.2f),
                    GridLine = new Color(0.22f, 0.28f, 0.38f),
                    Accent = new Color(0.55f, 0.4f, 0.9f),
                    Ambient = new Color(0.62f, 0.55f, 0.85f)
                };
            if (eraId == "the-chat-boom")
                return new EraSkin
                {
                    Floor = new Color(0.1f, 0.12f, 0.16f),
                    GridLine = new Color(0.2f, 0.25f, 0.32f),
                    Accent = new Color(0.95f, 0.55f, 0.35f),
                    Ambient = new Color(0.85f, 0.6f, 0.45f)
                };
            if (eraId == "agents")
                return new EraSkin
                {
                    Floor = new Color(0.08f, 0.1f, 0.14f),
                    GridLine = new Color(0.18f, 0.22f, 0.3f),
                    Accent = new Color(0.4f, 0.85f, 0.95f),
                    Ambient = new Color(0.45f, 0.8f, 0.92f)
                };
            if (eraId == "harnesses")
                return new EraSkin
                {
                    Floor = new Color(0.06f, 0.08f, 0.12f),
                    GridLine = new Color(0.15f, 0.2f, 0.28f),
                    Accent = new Color(0.75f, 0.45f, 1f),
                    Ambient = new Color(0.7f, 0.55f, 0.95f)
                };

            return new EraSkin
            {
                Floor = new Color(0.15f, 0.15f, 0.18f),
                GridLine = new Color(0.25f, 0.25f, 0.3f),
                Accent = new Color(0.5f, 0.7f, 0.9f),
                Ambient = new Color(0.55f, 0.6f, 0.75f)
            };
        }

        public static Color RoomColorEraTint(Color baseColor, EraSkin skin)
        {
            return Color.Lerp(baseColor, skin.Accent, 0.12f);
        }

        public static Color RoomColor(GameConfig config, string roomTypeId)
        {
            RoomTypeConfig type = config.RoomType(roomTypeId);
            if (type == null) return new Color(0.5f, 0.5f, 0.5f);
            return PurposeColor(type.Purpose);
        }

        public static Color PurposeColor(RoomPurpose purpose)
        {
            switch (purpose)
            {
                case RoomPurpose.Research: return new Color(0.35f, 0.55f, 0.95f);
                case RoomPurpose.Engineering: return new Color(0.4f, 0.75f, 0.45f);
                case RoomPurpose.Safety: return new Color(0.95f, 0.75f, 0.35f);
                case RoomPurpose.Sales: return new Color(0.95f, 0.5f, 0.55f);
                case RoomPurpose.Compute: return new Color(0.55f, 0.4f, 0.95f);
                case RoomPurpose.Infrastructure: return new Color(0.6f, 0.6f, 0.65f);
                case RoomPurpose.Policy: return new Color(0.75f, 0.55f, 0.85f);
                default: return new Color(0.6f, 0.6f, 0.6f);
            }
        }

        /// <summary>Maps rules permille glow (future) to an emission multiplier for room materials.</summary>
        public static Color GlowEmission(Color baseColor, int glowPermille)
        {
            float t = Mathf.Clamp01(glowPermille / 1000f);
            return baseColor * (1f + t * 2.5f);
        }
    }
}