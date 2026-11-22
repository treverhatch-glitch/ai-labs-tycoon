namespace AiLabs.Unity.Hq
{
    /// <summary>Minimum UI hit targets at 1920×1080 (see ACCESSIBILITY-PRESENTATION.md).</summary>
    public static class CampusAccessibilitySpec
    {
        public const float MinHitTargetPixels = 44f;
        public const float DesignHeightPixels = 1080f;

        public static float MinHitTargetNormalized =>
            MinHitTargetPixels / DesignHeightPixels;
    }
}