namespace AiLabs.Unity.Hq
{
    /// <summary>URP garage hero lane — HDRI, post stack, 3/4 tycoon camera (matches Blender compose).</summary>
    public static class EarlyGameGarageUrPresentationSpec
    {
        public const string VolumeProfileAssetPath = "Assets/Art/EarlyGame/URP/GarageHeroVolumeProfile.asset";
        public const string HdriEnvironmentPath = "Assets/Art/EarlyGame/Environment/brown_photostudio_02_1k.exr";
        public const string HdriSourceUrl = "https://polyhaven.com/a/brown_photostudio_02";
        public const string HeroPreviewFileName = "garage-scene-urp-hero.png";

        public const float BloomThreshold = 0.9f;
        public const float BloomIntensity = 0.22f;
        public const int TonemappingModeAces = 2;
        public const float AmbientOcclusionIntensity = 0.35f;
        public const float AmbientOcclusionRadius = 0.4f;

        public const float TycoonAzimuthDegrees = 42f;
        public const float TycoonElevationDegrees = 30f;
        public const float TycoonLensMm = 42f;
        public const float TycoonFieldOfViewDegrees = 46f;
        public const float TycoonDistanceScale = 1.48f;

        public const float HdriStrength = 0.48f;
        public const float HdriRotationDegrees = 118f;
    }
}