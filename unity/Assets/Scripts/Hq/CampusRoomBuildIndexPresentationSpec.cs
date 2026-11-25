namespace AiLabs.Unity.Hq
{
    /// <summary>Room build presentation index (see ROOM-BUILD-INDEX.md).</summary>
    public static class CampusRoomBuildIndexPresentationSpec
    {
        public const int DocumentedRoomBuildLayers = 2;

        public static float BuildPanelWidthFraction =>
            CampusRoomBuildPresentationSpec.BuildPanelWidthFraction;

        public static float BuildPanelHeightFraction =>
            CampusRoomBuildPresentationSpec.BuildPanelHeightFraction;

        public static float LotViewportWidthFraction =>
            CampusRoomBuildPresentationSpec.LotViewportWidthFraction;

        public const string BuildPresentationDoc = "ROOM-BUILD-PRESENTATION.md";
        public const string BreakdownDoc = "DISPLAY-BREAKDOWN-PRESENTATION.md";
    }
}