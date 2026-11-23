namespace AiLabs.Unity.Hq
{
    /// <summary>HUD regions in CI capture (see CAPTURE-HUD-SNAPSHOT.md).</summary>
    public static class CampusCaptureHudSnapshotPresentationSpec
    {
        public const int TopBarHeightPixels = 72;
        public const int TabRailTabCount = 5;
        public static long MinArtifactBytes => CampusCapturePresentationSpec.MinArtifactBytes;
    }
}