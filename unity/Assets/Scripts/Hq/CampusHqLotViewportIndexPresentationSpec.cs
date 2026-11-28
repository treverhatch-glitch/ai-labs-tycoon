namespace AiLabs.Unity.Hq
{
    /// <summary>HQ lot viewport index (see HQ-LOT-VIEWPORT-INDEX.md).</summary>
    public static class CampusHqLotViewportIndexPresentationSpec
    {
        public const int DocumentedLotViewportLayers = 2;

        public static float ViewportWidthFraction =>
            CampusHqLotViewportPresentationSpec.ViewportWidthFraction;

        public static float CameraYawDegrees =>
            CampusHqLotViewportPresentationSpec.CameraYawDegrees;

        public const string ViewportDoc = "HQ-LOT-VIEWPORT-PRESENTATION.md";
        public const string PropHintsDoc = "PROP-PLACEMENT-HINTS.md";
    }
}