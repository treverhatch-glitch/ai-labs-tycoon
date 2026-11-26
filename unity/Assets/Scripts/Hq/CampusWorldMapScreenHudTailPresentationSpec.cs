namespace AiLabs.Unity.Hq
{
    /// <summary>World map screen HUD tail (map 70% / site card 30%).</summary>
    public static class CampusWorldMapScreenHudTailPresentationSpec
    {
        public const float MapPlaneWidthFraction = 0.70f;

        public static float SiteCardWidthFraction =>
            1f - MapPlaneWidthFraction;

        public static float SidePanelWidthFraction =>
            CampusWorldMapHostPresentationSpec.SidePanelWidthFraction;

        public static int PostResearchHudBatch => 277;

        public const string TailDoc = "WORLD-MAP-SCREEN-HUD-TAIL.md";
    }
}