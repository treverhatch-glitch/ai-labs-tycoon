namespace AiLabs.Unity.Hq
{
    /// <summary>Explainable HUD cross-index tail (see EXPLAINABLE-HUD-TAIL-INDEX.md).</summary>
    public static class CampusExplainableHudTailIndexPresentationSpec
    {
        public const int TailEntryCount = 4;

        public static int DocumentedBreakdownDrawers =>
            CampusExplainableHudIndexPresentationSpec.DocumentedBreakdownDrawers;

        public static float SharedPartRowHeightPixels =>
            CampusExplainableHudIndexPresentationSpec.SharedPartRowHeightPixels;

        public static int RoomBuildLayers =>
            CampusRoomBuildIndexPresentationSpec.DocumentedRoomBuildLayers;

        public const string TailDoc = "EXPLAINABLE-HUD-TAIL-INDEX.md";
        public const string IndexDoc = "EXPLAINABLE-HUD-INDEX.md";
    }
}