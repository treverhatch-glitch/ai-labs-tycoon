namespace AiLabs.Unity.Hq
{
    /// <summary>Capture HUD tail navigation (see CAPTURE-HUD-TAIL-INDEX.md).</summary>
    public static class CampusCaptureHudTailIndexPresentationSpec
    {
        public const int TailEntryCount = 4;

        public static int A11yLayers =>
            CampusA11yChromeIndexPresentationSpec.DocumentedA11yLayers;

        public static float MinHitTargetPixels =>
            CampusA11yChromeIndexPresentationSpec.MinHitTargetPixels;

        public static int CaptureTopBarHeightPixels =>
            CampusCaptureHudSnapshotPresentationSpec.TopBarHeightPixels;

        public const string TailDoc = "CAPTURE-HUD-TAIL-INDEX.md";
    }
}