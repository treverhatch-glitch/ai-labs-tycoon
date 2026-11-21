using AiLabs.Rules.State;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Maps rules ModelMood to mascot colour and scale (presentation only).</summary>
    public static class ModelMascotPresentation
    {
        public struct MascotLook
        {
            public Color Body;
            public Color Glow;
            public float Scale;
            public float BobSpeed;
        }

        public static MascotLook ForMood(ModelMood mood)
        {
            switch (mood)
            {
                case ModelMood.Overworked:
                    return new MascotLook
                    {
                        Body = new Color(0.95f, 0.65f, 0.35f),
                        Glow = new Color(0.9f, 0.5f, 0.2f),
                        Scale = 0.85f,
                        BobSpeed = 2.5f
                    };
                case ModelMood.Glitching:
                    return new MascotLook
                    {
                        Body = new Color(0.95f, 0.3f, 0.55f),
                        Glow = new Color(1f, 0.1f, 0.4f),
                        Scale = 0.95f,
                        BobSpeed = 6f
                    };
                default:
                    return new MascotLook
                    {
                        Body = new Color(0.45f, 0.85f, 0.95f),
                        Glow = new Color(0.35f, 0.75f, 1f),
                        Scale = 1f,
                        BobSpeed = 1.2f
                    };
            }
        }
    }
}