namespace AiLabs.Unity.Hq
{
    /// <summary>A11y chrome index (see A11Y-CHROME-INDEX.md).</summary>
    public static class CampusA11yChromeIndexPresentationSpec
    {
        public const int DocumentedA11yLayers = 2;

        public const float MinHitTargetPixels = CampusFocusRingPresentationSpec.MinHitTargetPixels;

        public const int CaptureTopBarHeightPixels = CampusCaptureHudSnapshotPresentationSpec.TopBarHeightPixels;

        public const string FocusRingDoc = "FOCUS-RING-PRESENTATION.md";
        public const string CaptureHudDoc = "CAPTURE-HUD-SNAPSHOT.md";
    }
}